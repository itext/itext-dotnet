/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Util;

namespace iText.IO.Font {
    public class TrueTypeFont : FontProgram {
        private OpenTypeParser fontParser;

        protected internal int[][] bBoxes;

        protected internal bool isVertical;

        private GlyphSubstitutionTableReader gsubTable;

        private GlyphPositioningTableReader gposTable;

        private OpenTypeGdefTableReader gdefTable;

        /// <summary>The map containing the kerning information.</summary>
        /// <remarks>
        /// The map containing the kerning information. It represents the content of
        /// table 'kern'. The key is an <c>Integer</c> where the top 16 bits
        /// are the glyph number for the first character and the lower 16 bits are the
        /// glyph number for the second character. The value is the amount of kerning in
        /// normalized 1000 units as an <c>Integer</c>. This value is usually negative.
        /// </remarks>
        protected internal IntHashtable kerning = new IntHashtable();

        private byte[] fontStreamBytes;

        private TrueTypeFont(OpenTypeParser fontParser) {
            this.fontParser = fontParser;
            this.fontParser.LoadTables(true);
            InitializeFontProperties();
        }

        protected internal TrueTypeFont() {
            fontNames = new FontNames();
        }

        public TrueTypeFont(String path)
            : this(new OpenTypeParser(path)) {
        }

        public TrueTypeFont(byte[] ttf)
            : this(new OpenTypeParser(ttf)) {
        }

        public TrueTypeFont(byte[] ttf, bool isLenientMode)
            : this(new OpenTypeParser(ttf, isLenientMode)) {
        }

//\cond DO_NOT_DOCUMENT
        internal TrueTypeFont(String ttcPath, int ttcIndex)
            : this(new OpenTypeParser(ttcPath, ttcIndex)) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal TrueTypeFont(byte[] ttc, int ttcIndex)
            : this(new OpenTypeParser(ttc, ttcIndex)) {
        }
//\endcond

        public override bool HasKernPairs() {
            return kerning.Size() > 0;
        }

        /// <summary>Gets the kerning between two glyphs.</summary>
        /// <param name="first">the first glyph</param>
        /// <param name="second">the second glyph</param>
        /// <returns>the kerning to be applied</returns>
        public override int GetKerning(Glyph first, Glyph second) {
            if (first == null || second == null) {
                return 0;
            }
            return kerning.Get((first.GetCode() << 16) + second.GetCode());
        }

        public virtual bool IsCff() {
            return fontParser.IsCff();
        }

        public virtual IDictionary<int, int[]> GetActiveCmap() {
            OpenTypeParser.CmapTable cmaps = fontParser.GetCmapTable();
            if (cmaps.cmap310 != null) {
                return cmaps.cmap310;
            }
            else {
                if (!cmaps.fontSpecific && cmaps.cmap31 != null) {
                    return cmaps.cmap31;
                }
                else {
                    if (cmaps.fontSpecific && cmaps.cmap10 != null) {
                        return cmaps.cmap10;
                    }
                    else {
                        if (cmaps.cmap31 != null) {
                            return cmaps.cmap31;
                        }
                        else {
                            return cmaps.cmap10;
                        }
                    }
                }
            }
        }

        public virtual byte[] GetFontStreamBytes() {
            if (fontStreamBytes != null) {
                return fontStreamBytes;
            }
            try {
                if (fontParser.IsCff()) {
                    fontStreamBytes = fontParser.ReadCffFont();
                }
                else {
                    fontStreamBytes = fontParser.GetFullFont();
                }
            }
            catch (System.IO.IOException e) {
                fontStreamBytes = null;
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IO_EXCEPTION, e);
            }
            return fontStreamBytes;
        }

        public override int GetPdfFontFlags() {
            int flags = 0;
            if (fontMetrics.IsFixedPitch()) {
                flags |= 1;
            }
            flags |= IsFontSpecific() ? 4 : 32;
            if (fontNames.IsItalic()) {
                flags |= 64;
            }
            if (fontNames.IsBold() || fontNames.GetFontWeight() > 500) {
                flags |= 262144;
            }
            return flags;
        }

        /// <summary>The offset from the start of the file to the table directory.</summary>
        /// <remarks>
        /// The offset from the start of the file to the table directory.
        /// It is 0 for TTF and may vary for TTC depending on the chosen font.
        /// </remarks>
        /// <returns>directory Offset</returns>
        public virtual int GetDirectoryOffset() {
            return fontParser.directoryOffset;
        }

        public virtual GlyphSubstitutionTableReader GetGsubTable() {
            return gsubTable;
        }

        public virtual GlyphPositioningTableReader GetGposTable() {
            return gposTable;
        }

        public virtual OpenTypeGdefTableReader GetGdefTable() {
            return gdefTable;
        }

        /// <summary>Gets subset based on the passed glyphs.</summary>
        /// <param name="glyphs">the glyphs to subset the font</param>
        /// <param name="subsetTables">
        /// whether subset tables (remove `name` and `post` tables) or not. It's used in case of ttc
        /// (true type collection) font where single "full" font is needed. Despite the value of that
        /// flag, only used glyphs will be left in the font
        /// </param>
        /// <returns>the subset font</returns>
        public virtual byte[] GetSubset(ICollection<int> glyphs, bool subsetTables) {
            return Subset(glyphs, subsetTables).GetSecond();
        }

        /// <summary>Gets subset and a number of glyphs in it based on the passed glyphs.</summary>
        /// <remarks>
        /// Gets subset and a number of glyphs in it based on the passed glyphs.
        /// <para />
        /// The number of glyphs in a subset is not just glyphs.size() here. It's the biggest glyph id + 1 (for glyph 0).
        /// It also may include possible composite glyphs.
        /// </remarks>
        /// <param name="glyphs">the glyphs to subset the font</param>
        /// <param name="subsetTables">
        /// whether subset tables (remove `name` and `post` tables) or not. It's used in case of ttc
        /// (true type collection) font where single "full" font is needed. Despite the value of that
        /// flag, only used glyphs will be left in the font
        /// </param>
        /// <returns>the subset of the font and the number of glyphs in it</returns>
        public virtual Tuple2<int, byte[]> Subset(ICollection<int> glyphs, bool subsetTables) {
            try {
                return fontParser.GetSubset(glyphs, subsetTables);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IO_EXCEPTION, e);
            }
        }

        /// <summary>Merges the passed font into one.</summary>
        /// <remarks>Merges the passed font into one. Used glyphs per each font are applied to subset the merged font.
        ///     </remarks>
        /// <param name="toMerge">the fonts to merge with used glyphs per each font</param>
        /// <param name="fontName">the name of fonts to merge</param>
        /// <returns>the raw data of merged font</returns>
        [System.ObsoleteAttribute(@"in favour of Merge(System.Collections.Generic.IDictionary{K, V}, System.String, bool)"
            )]
        public static byte[] Merge(IDictionary<iText.IO.Font.TrueTypeFont, ICollection<int>> toMerge, String fontName
            ) {
            return Merge(toMerge, fontName, true);
        }

        /// <summary>Merges the passed font into one.</summary>
        /// <remarks>Merges the passed font into one. Used glyphs per each font are applied to subset the merged font.
        ///     </remarks>
        /// <param name="toMerge">the fonts to merge with used glyphs per each font</param>
        /// <param name="fontName">the name of fonts to merge</param>
        /// <param name="isCmapCheckRequired">the flag which specifies whether 'cmap' table should be checked while merging or not
        ///     </param>
        /// <returns>the raw data of merged font</returns>
        public static byte[] Merge(IDictionary<iText.IO.Font.TrueTypeFont, ICollection<int>> toMerge, String fontName
            , bool isCmapCheckRequired) {
            try {
                IDictionary<OpenTypeParser, ICollection<int>> toMergeWithParsers = new LinkedDictionary<OpenTypeParser, ICollection
                    <int>>();
                foreach (KeyValuePair<iText.IO.Font.TrueTypeFont, ICollection<int>> entry in toMerge) {
                    toMergeWithParsers.Put(entry.Key.fontParser, entry.Value);
                }
                TrueTypeFontMerger trueTypeFontMerger = new TrueTypeFontMerger(fontName, toMergeWithParsers, isCmapCheckRequired
                    );
                return trueTypeFontMerger.Process().GetSecond();
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IO_EXCEPTION, e);
            }
        }

        /// <summary>
        /// Maps a set of glyph CIDs (as used in PDF file) to corresponding GID values
        /// (as a glyph primary identifier in the font file).
        /// </summary>
        /// <remarks>
        /// Maps a set of glyph CIDs (as used in PDF file) to corresponding GID values
        /// (as a glyph primary identifier in the font file).
        /// This call is only meaningful for fonts that return true for
        /// <see cref="IsCff()"/>.
        /// For other types of fonts, GID and CID are always the same, so that call would essentially
        /// return a set of the same values.
        /// </remarks>
        /// <param name="glyphs">a set of glyph CIDs</param>
        /// <returns>a set of glyph ids corresponding to the passed glyph CIDs</returns>
        public virtual ICollection<int> MapGlyphsCidsToGids(ICollection<int> glyphs) {
            return glyphs.Select((i) => {
                Glyph usedGlyph = GetGlyphByCode(i);
                if (usedGlyph is GidAwareGlyph) {
                    return ((GidAwareGlyph)usedGlyph).GetGid();
                }
                return i;
            }
            ).ToList();
        }

        /// <summary>
        /// Checks whether current
        /// <see cref="TrueTypeFont"/>
        /// program contains the “cmap” subtable
        /// with provided platform ID and encoding ID.
        /// </summary>
        /// <param name="platformID">platform ID</param>
        /// <param name="encodingID">encoding ID</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if “cmap” subtable with provided platform ID and encoding ID is present in the font program,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCmapPresent(int platformID, int encodingID) {
            OpenTypeParser.CmapTable cmaps = fontParser.GetCmapTable();
            if (cmaps == null) {
                return false;
            }
            return cmaps.cmapEncodings.Contains(new Tuple2<int, int>(platformID, encodingID));
        }

        /// <summary>
        /// Gets the number of the “cmap” subtables for the current
        /// <see cref="TrueTypeFont"/>
        /// program.
        /// </summary>
        /// <returns>the number of the “cmap” subtables</returns>
        public virtual int GetNumberOfCmaps() {
            OpenTypeParser.CmapTable cmaps = fontParser.GetCmapTable();
            if (cmaps == null) {
                return 0;
            }
            return cmaps.cmapEncodings.Count;
        }

        protected internal virtual void ReadGdefTable() {
            int[] gdef = fontParser.tables.Get("GDEF");
            if (gdef != null) {
                gdefTable = new OpenTypeGdefTableReader(fontParser.raf, gdef[0]);
            }
            else {
                gdefTable = new OpenTypeGdefTableReader(fontParser.raf, 0);
            }
            gdefTable.ReadTable();
        }

        protected internal virtual void ReadGsubTable() {
            int[] gsub = fontParser.tables.Get("GSUB");
            if (gsub != null) {
                gsubTable = new GlyphSubstitutionTableReader(fontParser.raf, gsub[0], gdefTable, codeToGlyph, fontMetrics.
                    GetUnitsPerEm());
            }
        }

        protected internal virtual void ReadGposTable() {
            int[] gpos = fontParser.tables.Get("GPOS");
            if (gpos != null) {
                gposTable = new GlyphPositioningTableReader(fontParser.raf, gpos[0], gdefTable, codeToGlyph, fontMetrics.GetUnitsPerEm
                    ());
            }
        }

        private void InitializeFontProperties() {
            // initialize sfnt tables
            OpenTypeParser.HeaderTable head = fontParser.GetHeadTable();
            OpenTypeParser.HorizontalHeader hhea = fontParser.GetHheaTable();
            OpenTypeParser.WindowsMetrics os_2 = fontParser.GetOs_2Table();
            OpenTypeParser.PostTable post = fontParser.GetPostTable();
            isFontSpecific = fontParser.GetCmapTable().fontSpecific;
            kerning = fontParser.ReadKerning(head.unitsPerEm);
            bBoxes = fontParser.ReadBbox(head.unitsPerEm);
            // font names group
            fontNames = fontParser.GetFontNames();
            // font metrics group
            fontMetrics.SetUnitsPerEm(head.unitsPerEm);
            fontMetrics.UpdateBbox(head.xMin, head.yMin, head.xMax, head.yMax);
            fontMetrics.SetNumberOfGlyphs(fontParser.ReadNumGlyphs());
            fontMetrics.SetGlyphWidths(fontParser.GetGlyphWidthsByIndex());
            fontMetrics.SetTypoAscender(os_2.sTypoAscender);
            fontMetrics.SetTypoDescender(os_2.sTypoDescender);
            fontMetrics.SetCapHeight(os_2.sCapHeight);
            fontMetrics.SetXHeight(os_2.sxHeight);
            fontMetrics.SetItalicAngle(post.italicAngle);
            fontMetrics.SetAscender(hhea.Ascender);
            fontMetrics.SetDescender(hhea.Descender);
            fontMetrics.SetLineGap(hhea.LineGap);
            fontMetrics.SetWinAscender(os_2.usWinAscent);
            fontMetrics.SetWinDescender(os_2.usWinDescent);
            fontMetrics.SetAdvanceWidthMax(hhea.advanceWidthMax);
            fontMetrics.SetUnderlinePosition((post.underlinePosition - post.underlineThickness) / 2);
            fontMetrics.SetUnderlineThickness(post.underlineThickness);
            fontMetrics.SetStrikeoutPosition(os_2.yStrikeoutPosition);
            fontMetrics.SetStrikeoutSize(os_2.yStrikeoutSize);
            fontMetrics.SetSubscriptOffset(-os_2.ySubscriptYOffset);
            fontMetrics.SetSubscriptSize(os_2.ySubscriptYSize);
            fontMetrics.SetSuperscriptOffset(os_2.ySuperscriptYOffset);
            fontMetrics.SetSuperscriptSize(os_2.ySuperscriptYSize);
            fontMetrics.SetIsFixedPitch(post.isFixedPitch);
            // font identification group
            String[][] ttfVersion = fontNames.GetNames(5);
            if (ttfVersion != null) {
                fontIdentification.SetTtfVersion(ttfVersion[0][3]);
            }
            String[][] ttfUniqueId = fontNames.GetNames(3);
            if (ttfUniqueId != null) {
                fontIdentification.SetTtfVersion(ttfUniqueId[0][3]);
            }
            byte[] pdfPanose = new byte[12];
            pdfPanose[1] = (byte)(os_2.sFamilyClass);
            pdfPanose[0] = (byte)(os_2.sFamilyClass >> 8);
            Array.Copy(os_2.panose, 0, pdfPanose, 2, 10);
            fontIdentification.SetPanose(pdfPanose);
            IDictionary<int, int[]> cmap = GetActiveCmap();
            int[] glyphWidths = fontParser.GetGlyphWidthsByIndex();
            int numOfGlyphs = fontMetrics.GetNumberOfGlyphs();
            unicodeToGlyph = new LinkedDictionary<int, Glyph>(cmap.Count);
            codeToGlyph = new LinkedDictionary<int, Glyph>(numOfGlyphs);
            avgWidth = 0;
            CFFFontSubset cffFontSubset = null;
            if (IsCff()) {
                cffFontSubset = new CFFFontSubset(GetFontStreamBytes());
            }
            foreach (int charCode in cmap.Keys) {
                int index = cmap.Get(charCode)[0];
                if (index >= numOfGlyphs) {
                    // It seems to be a valid case. If the font is subsetted but cmap table is not, it's a valid case
                    continue;
                }
                int cid;
                Glyph glyph;
                int[] glyphBBox = (bBoxes != null && index < bBoxes.Length) ? bBoxes[index] : null;
                if (cffFontSubset != null && cffFontSubset.IsCID()) {
                    cid = cffFontSubset.GetCidForGlyphId(index);
                    GidAwareGlyph cffGlyph = new GidAwareGlyph(cid, glyphWidths[index], charCode, glyphBBox);
                    cffGlyph.SetGid(index);
                    glyph = cffGlyph;
                }
                else {
                    cid = index;
                    glyph = new Glyph(cid, glyphWidths[index], charCode, glyphBBox);
                }
                unicodeToGlyph.Put(charCode, glyph);
                // This is done on purpose to keep the mapping to glyphs with smaller unicode values, in contrast with
                // larger values which often represent different forms of other characters.
                if (!codeToGlyph.ContainsKey(cid)) {
                    codeToGlyph.Put(cid, glyph);
                }
                avgWidth += glyph.GetWidth();
            }
            FixSpaceIssue();
            for (int index = 0; index < glyphWidths.Length; index++) {
                if (codeToGlyph.ContainsKey(index)) {
                    continue;
                }
                Glyph glyph = new Glyph(index, glyphWidths[index], -1);
                codeToGlyph.Put(index, glyph);
                avgWidth += glyph.GetWidth();
            }
            if (!codeToGlyph.IsEmpty()) {
                avgWidth /= codeToGlyph.Count;
            }
            ReadGdefTable();
            ReadGsubTable();
            ReadGposTable();
            isVertical = false;
        }

        /// <summary>Gets the code pages supported by the font.</summary>
        /// <returns>the code pages supported by the font</returns>
        public virtual String[] GetCodePagesSupported() {
            long cp = ((long)fontParser.GetOs_2Table().ulCodePageRange2 << 32) + (fontParser.GetOs_2Table().ulCodePageRange1
                 & unchecked((long)(0xffffffffL)));
            int count = 0;
            long bit = 1;
            for (int k = 0; k < 64; ++k) {
                if ((cp & bit) != 0 && TrueTypeCodePages.Get(k) != null) {
                    ++count;
                }
                bit <<= 1;
            }
            String[] ret = new String[count];
            count = 0;
            bit = 1;
            for (int k = 0; k < 64; ++k) {
                if ((cp & bit) != 0 && TrueTypeCodePages.Get(k) != null) {
                    ret[count++] = TrueTypeCodePages.Get(k);
                }
                bit <<= 1;
            }
            return ret;
        }

        public override bool IsBuiltWith(String fontProgram) {
            return Object.Equals(fontParser.fileName, fontProgram);
        }

        public virtual void Close() {
            if (fontParser != null) {
                fontParser.Close();
            }
            fontParser = null;
        }

        /// <summary>The method will update usedGlyphs with additional range or with all glyphs if there is no subset.
        ///     </summary>
        /// <remarks>
        /// The method will update usedGlyphs with additional range or with all glyphs if there is no subset.
        /// This set of used glyphs can be used for building width array and ToUnicode CMAP.
        /// </remarks>
        /// <param name="usedGlyphs">
        /// a set of integers, which are glyph ids that denote used glyphs.
        /// This set is updated inside the method if needed.
        /// </param>
        /// <param name="subset">subset status</param>
        /// <param name="subsetRanges">additional subset ranges</param>
        public virtual void UpdateUsedGlyphs(SortedSet<int> usedGlyphs, bool subset, IList<int[]> subsetRanges) {
            int[] compactRange = ToCompactRange(subsetRanges, subset);
            ICollection<int> missingGlyphs = GetMissingGlyphs(compactRange);
            foreach (int? glyphId in missingGlyphs) {
                if (GetGlyphByCode(glyphId.Value) != null) {
                    usedGlyphs.Add(glyphId.Value);
                }
            }
        }

        /// <summary>The method will update usedGlyphs with additional range or with all glyphs if there is no subset.
        ///     </summary>
        /// <remarks>
        /// The method will update usedGlyphs with additional range or with all glyphs if there is no subset.
        /// This map of used glyphs can be used for building width array and ToUnicode CMAP.
        /// </remarks>
        /// <param name="usedGlyphs">a map of glyph ids to glyphs. This map is updated inside the method if needed</param>
        /// <param name="subset">subset status</param>
        /// <param name="subsetRanges">additional subset ranges</param>
        public virtual void UpdateUsedGlyphs(IDictionary<int, Glyph> usedGlyphs, bool subset, IList<int[]> subsetRanges
            ) {
            int[] compactRange = ToCompactRange(subsetRanges, subset);
            ICollection<int> missingGlyphs = GetMissingGlyphs(compactRange);
            foreach (int? glyphId in missingGlyphs) {
                Glyph glyph = GetGlyphByCode(glyphId.Value);
                if (glyph != null && !usedGlyphs.ContainsKey(glyphId.Value)) {
                    usedGlyphs.Put(glyphId.Value, glyph);
                }
            }
        }

        /// <summary>
        /// Normalizes given ranges by making sure that first values in pairs are lower than second values and merges overlapping
        /// ranges in one.
        /// </summary>
        /// <param name="ranges">
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// of integer arrays, which are constituted by pairs of ints that denote
        /// each range limits. Each integer array size shall be a multiple of two
        /// </param>
        /// <param name="subset">
        /// 
        /// <see langword="true"/>
        /// if a font subset is required. Used only if
        /// <paramref name="ranges"/>
        /// is
        /// <see langword="null"/>
        /// </param>
        /// <returns>single merged array consisting of pairs of integers, each of them denoting a range</returns>
        private static int[] ToCompactRange(IList<int[]> ranges, bool subset) {
            if (ranges == null) {
                if (subset) {
                    return new int[] {  };
                }
                else {
                    return new int[] { 0, 0xFFFF };
                }
            }
            // Ranges are requested
            IList<int[]> simp = new List<int[]>();
            foreach (int[] range in ranges) {
                for (int j = 0; j < range.Length; j += 2) {
                    simp.Add(new int[] { Math.Max(0, Math.Min(range[j], range[j + 1])), Math.Min(0xffff, Math.Max(range[j], range
                        [j + 1])) });
                }
            }
            for (int k1 = 0; k1 < simp.Count - 1; ++k1) {
                for (int k2 = k1 + 1; k2 < simp.Count; ++k2) {
                    int[] r1 = simp[k1];
                    int[] r2 = simp[k2];
                    if (r1[0] >= r2[0] && r1[0] <= r2[1] || r1[1] >= r2[0] && r1[0] <= r2[1]) {
                        r1[0] = Math.Min(r1[0], r2[0]);
                        r1[1] = Math.Max(r1[1], r2[1]);
                        simp.JRemoveAt(k2);
                        --k2;
                    }
                }
            }
            int[] s = new int[simp.Count * 2];
            for (int k = 0; k < simp.Count; ++k) {
                int[] r = simp[k];
                s[k * 2] = r[0];
                s[k * 2 + 1] = r[1];
            }
            return s;
        }

        private static ICollection<int> GetMissingGlyphs(int[] compactRange) {
            ICollection<int> missingGlyphs = new HashSet<int>();
            for (int k = 0; k < compactRange.Length; k += 2) {
                int from = compactRange[k];
                int to = compactRange[k + 1];
                for (int glyphId = from; glyphId <= to; glyphId++) {
                    missingGlyphs.Add(glyphId);
                }
            }
            return missingGlyphs;
        }
    }
}
