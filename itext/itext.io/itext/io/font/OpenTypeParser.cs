/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font {
    internal class OpenTypeParser : IDisposable {
        private const int HEAD_LOCA_FORMAT_OFFSET = 51;

        /// <summary>The components of table 'head'.</summary>
        internal class HeaderTable {
            internal int flags;

            internal int unitsPerEm;

            internal short xMin;

            internal short yMin;

            internal short xMax;

            internal short yMax;

            internal int macStyle;
        }

        /// <summary>The components of table 'hhea'.</summary>
        internal class HorizontalHeader {
            internal short Ascender;

            internal short Descender;

            internal short LineGap;

            internal int advanceWidthMax;

            internal short minLeftSideBearing;

            internal short minRightSideBearing;

            internal short xMaxExtent;

            internal short caretSlopeRise;

            internal short caretSlopeRun;

            internal int numberOfHMetrics;
        }

        /// <summary>The components of table 'OS/2'.</summary>
        internal class WindowsMetrics {
            internal short xAvgCharWidth;

            internal int usWeightClass;

            internal int usWidthClass;

            internal short fsType;

            internal short ySubscriptXSize;

            internal short ySubscriptYSize;

            internal short ySubscriptXOffset;

            internal short ySubscriptYOffset;

            internal short ySuperscriptXSize;

            internal short ySuperscriptYSize;

            internal short ySuperscriptXOffset;

            internal short ySuperscriptYOffset;

            internal short yStrikeoutSize;

            internal short yStrikeoutPosition;

            internal short sFamilyClass;

            internal byte[] panose = new byte[10];

            internal byte[] achVendID = new byte[4];

            internal int fsSelection;

            internal int usFirstCharIndex;

            internal int usLastCharIndex;

            internal short sTypoAscender;

            internal short sTypoDescender;

            internal short sTypoLineGap;

            internal int usWinAscent;

            internal int usWinDescent;

            internal int ulCodePageRange1;

            internal int ulCodePageRange2;

            internal int sxHeight;

            internal int sCapHeight;
        }

        internal class PostTable {
            /// <summary>The italic angle.</summary>
            /// <remarks>
            /// The italic angle. It is usually extracted from the 'post' table or in it's
            /// absence with the code:
            /// <pre>
            /// <c>-Math.atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI</c>
            /// </pre>
            /// </remarks>
            internal float italicAngle;

            internal int underlinePosition;

            internal int underlineThickness;

            /// <summary><c>true</c> if all the glyphs have the same width.</summary>
            internal bool isFixedPitch;
        }

        internal class CmapTable {
            /// <summary>The map containing the code information for the table 'cmap', encoding 1.0.</summary>
            /// <remarks>
            /// The map containing the code information for the table 'cmap', encoding 1.0.
            /// The key is the code and the value is an
            /// <c>int[2]</c>
            /// where position 0
            /// is the glyph number and position 1 is the glyph width normalized to 1000 units.
            /// </remarks>
            /// <seealso cref="FontProgram.UNITS_NORMALIZATION"/>
            internal IDictionary<int, int[]> cmap10;

            /// <summary>The map containing the code information for the table 'cmap', encoding 3.1 in Unicode.</summary>
            /// <remarks>
            /// The map containing the code information for the table 'cmap', encoding 3.1 in Unicode.
            /// The key is the code and the value is an
            /// <c>int[2]</c>
            /// where position 0
            /// is the glyph number and position 1 is the glyph width normalized to 1000 units.
            /// </remarks>
            /// <seealso cref="FontProgram.UNITS_NORMALIZATION"/>
            internal IDictionary<int, int[]> cmap31;

            internal IDictionary<int, int[]> cmapExt;

            internal bool fontSpecific = false;
        }

        /// <summary>The file name.</summary>
        protected internal String fileName;

        /// <summary>The file in use.</summary>
        protected internal RandomAccessFileOrArray raf;

        /// <summary>The index for the TTC font.</summary>
        /// <remarks>
        /// The index for the TTC font. It is -1
        /// <c>int</c>
        /// for a TTF file.
        /// </remarks>
        protected internal int ttcIndex = -1;

        /// <summary>The offset from the start of the file to the table directory.</summary>
        /// <remarks>
        /// The offset from the start of the file to the table directory.
        /// It is 0 for TTF and may vary for TTC depending on the chosen font.
        /// </remarks>
        protected internal int directoryOffset;

        /// <summary>The font name.</summary>
        /// <remarks>The font name. This name is usually extracted from the table 'name' with the 'Name ID' 6.</remarks>
        protected internal String fontName;

        /// <summary>All the names of the Names-Table.</summary>
        protected internal IDictionary<int, IList<String[]>> allNameEntries;

        /// <summary>Indicate, that the font contains 'CFF ' table.</summary>
        protected internal bool cff = false;

        /// <summary>Offset to 'CFF ' table.</summary>
        protected internal int cffOffset;

        /// <summary>Length of 'CFF ' table.</summary>
        protected internal int cffLength;

        private int[] glyphWidthsByIndex;

        protected internal OpenTypeParser.HeaderTable head;

        protected internal OpenTypeParser.HorizontalHeader hhea;

        protected internal OpenTypeParser.WindowsMetrics os_2;

        protected internal OpenTypeParser.PostTable post;

        protected internal OpenTypeParser.CmapTable cmaps;

        /// <summary>Contains the location of the several tables.</summary>
        /// <remarks>
        /// Contains the location of the several tables. The key is the name of
        /// the table and the value is an <c>int[2]</c> where position 0
        /// is the offset from the start of the file and position 1 is the length
        /// of the table.
        /// </remarks>
        protected internal IDictionary<String, int[]> tables;

        public OpenTypeParser(byte[] ttf) {
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(ttf));
            InitializeSfntTables();
        }

        public OpenTypeParser(byte[] ttc, int ttcIndex) {
            this.ttcIndex = ttcIndex;
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(ttc));
            InitializeSfntTables();
        }

        public OpenTypeParser(String ttcPath, int ttcIndex) {
            this.ttcIndex = ttcIndex;
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(ttcPath));
            InitializeSfntTables();
        }

        public OpenTypeParser(String name) {
            String ttcName = GetTTCName(name);
            this.fileName = ttcName;
            if (ttcName.Length < name.Length) {
                ttcIndex = Convert.ToInt32(name.Substring(ttcName.Length + 1), System.Globalization.CultureInfo.InvariantCulture
                    );
            }
            raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(fileName));
            InitializeSfntTables();
        }

        /// <summary>Gets the Postscript font name.</summary>
        public virtual String GetPsFontName() {
            if (fontName == null) {
                IList<String[]> names = allNameEntries.Get(6);
                if (names != null && names.Count > 0) {
                    fontName = names[0][3];
                }
                else {
                    fontName = new FileInfo(fileName).Name.Replace(' ', '-');
                }
            }
            return fontName;
        }

        public virtual IDictionary<int, IList<String[]>> GetAllNameEntries() {
            return allNameEntries;
        }

        public virtual OpenTypeParser.PostTable GetPostTable() {
            return post;
        }

        public virtual OpenTypeParser.WindowsMetrics GetOs_2Table() {
            return os_2;
        }

        public virtual OpenTypeParser.HorizontalHeader GetHheaTable() {
            return hhea;
        }

        public virtual OpenTypeParser.HeaderTable GetHeadTable() {
            return head;
        }

        public virtual OpenTypeParser.CmapTable GetCmapTable() {
            return cmaps;
        }

        public virtual int[] GetGlyphWidthsByIndex() {
            return glyphWidthsByIndex;
        }

        public virtual FontNames GetFontNames() {
            FontNames fontNames = new FontNames();
            fontNames.SetAllNames(GetAllNameEntries());
            fontNames.SetFontName(GetPsFontName());
            fontNames.SetFullName(fontNames.GetNames(4));
            String[][] otfFamilyName = fontNames.GetNames(16);
            String[][] familyName = fontNames.GetNames(1);
            fontNames.SetFamilyName2(familyName);
            if (otfFamilyName != null) {
                fontNames.SetFamilyName(otfFamilyName);
            }
            else {
                fontNames.SetFamilyName(familyName);
            }
            String[][] subfamily = fontNames.GetNames(2);
            if (subfamily != null) {
                fontNames.SetStyle(subfamily[0][3]);
            }
            String[][] otfSubFamily = fontNames.GetNames(17);
            if (otfFamilyName != null) {
                fontNames.SetSubfamily(otfSubFamily);
            }
            else {
                fontNames.SetSubfamily(subfamily);
            }
            String[][] cidName = fontNames.GetNames(20);
            if (cidName != null) {
                fontNames.SetCidFontName(cidName[0][3]);
            }
            fontNames.SetFontWeight(os_2.usWeightClass);
            fontNames.SetFontStretch(FontStretches.FromOpenTypeWidthClass(os_2.usWidthClass));
            fontNames.SetMacStyle(head.macStyle);
            fontNames.SetAllowEmbedding(os_2.fsType != 2);
            return fontNames;
        }

        public virtual bool IsCff() {
            return cff;
        }

        public virtual byte[] GetFullFont() {
            RandomAccessFileOrArray rf2 = null;
            try {
                rf2 = raf.CreateView();
                byte[] b = new byte[(int)rf2.Length()];
                rf2.ReadFully(b);
                return b;
            }
            finally {
                try {
                    if (rf2 != null) {
                        rf2.Close();
                    }
                }
                catch (Exception) {
                }
            }
        }

        /// <summary>
        /// If this font file is using the Compact Font File Format, then this method
        /// will return the raw bytes needed for the font stream.
        /// </summary>
        /// <remarks>
        /// If this font file is using the Compact Font File Format, then this method
        /// will return the raw bytes needed for the font stream. If this method is
        /// ever made public: make sure to add a test if (cff == true).
        /// </remarks>
        /// <returns>a byte array</returns>
        public virtual byte[] ReadCffFont() {
            if (!IsCff()) {
                return null;
            }
            RandomAccessFileOrArray rf2 = null;
            try {
                rf2 = raf.CreateView();
                rf2.Seek(cffOffset);
                byte[] cff = new byte[cffLength];
                rf2.ReadFully(cff);
                return cff;
            }
            finally {
                try {
                    if (rf2 != null) {
                        rf2.Close();
                    }
                }
                catch (Exception) {
                }
            }
        }

        internal virtual byte[] GetSubset(ICollection<int> glyphs, bool subset) {
            TrueTypeFontSubset sb = new TrueTypeFontSubset(fileName, raf.CreateView(), glyphs, directoryOffset, subset
                );
            return sb.Process();
        }

        public virtual void Close() {
            if (raf != null) {
                raf.Close();
            }
            raf = null;
        }

        private void InitializeSfntTables() {
            tables = new LinkedDictionary<String, int[]>();
            if (ttcIndex >= 0) {
                int dirIdx = ttcIndex;
                if (dirIdx < 0) {
                    if (fileName != null) {
                        throw new iText.IO.Exceptions.IOException("The font index for {0} must be positive.").SetMessageParams(fileName
                            );
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException("The font index must be positive.");
                    }
                }
                String mainTag = ReadStandardString(4);
                if (!mainTag.Equals("ttcf")) {
                    if (fileName != null) {
                        throw new iText.IO.Exceptions.IOException("{0} is not a valid ttc file.").SetMessageParams(fileName);
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException("Not a valid ttc file.");
                    }
                }
                raf.SkipBytes(4);
                int dirCount = raf.ReadInt();
                if (dirIdx >= dirCount) {
                    if (fileName != null) {
                        throw new iText.IO.Exceptions.IOException("The font index for {0} must be between 0 and {1}. It is {2}.").
                            SetMessageParams(fileName, dirCount - 1, dirIdx);
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException("The font index must be between 0 and {0}. It is {1}.").SetMessageParams
                            (dirCount - 1, dirIdx);
                    }
                }
                raf.SkipBytes(dirIdx * 4);
                directoryOffset = raf.ReadInt();
            }
            raf.Seek(directoryOffset);
            int ttId = raf.ReadInt();
            if (ttId != 0x00010000 && ttId != 0x4F54544F) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException("{0} is not a valid ttf or otf file.").SetMessageParams(fileName
                        );
                }
                else {
                    throw new iText.IO.Exceptions.IOException("Not a valid ttf or otf file.");
                }
            }
            int num_tables = raf.ReadUnsignedShort();
            raf.SkipBytes(6);
            for (int k = 0; k < num_tables; ++k) {
                String tag = ReadStandardString(4);
                raf.SkipBytes(4);
                int[] table_location = new int[2];
                table_location[0] = raf.ReadInt();
                table_location[1] = raf.ReadInt();
                tables.Put(tag, table_location);
            }
        }

        /// <summary>Reads the font data.</summary>
        /// <param name="all">if true, all tables will be read, otherwise only 'head', 'name', and 'os/2'.</param>
        protected internal virtual void LoadTables(bool all) {
            ReadNameTable();
            ReadHeadTable();
            ReadOs_2Table();
            ReadPostTable();
            if (all) {
                CheckCff();
                ReadHheaTable();
                ReadGlyphWidths();
                ReadCmapTable();
            }
        }

        /// <summary>Gets the name from a composed TTC file name.</summary>
        /// <remarks>
        /// Gets the name from a composed TTC file name.
        /// If I have for input "myfont.ttc,2" the return will
        /// be "myfont.ttc".
        /// </remarks>
        /// <param name="name">the full name</param>
        /// <returns>the simple file name</returns>
        protected internal static String GetTTCName(String name) {
            if (name == null) {
                return null;
            }
            int idx = name.ToLowerInvariant().IndexOf(".ttc,", StringComparison.Ordinal);
            if (idx < 0) {
                return name;
            }
            else {
                return name.JSubstring(0, idx + 4);
            }
        }

        protected internal virtual void CheckCff() {
            int[] table_location;
            table_location = tables.Get("CFF ");
            if (table_location != null) {
                cff = true;
                cffOffset = table_location[0];
                cffLength = table_location[1];
            }
        }

        /// <summary>Reads the glyphs widths.</summary>
        /// <remarks>
        /// Reads the glyphs widths. The widths are extracted from the table 'hmtx'.
        /// The glyphs are normalized to 1000 units (TrueTypeFont.UNITS_NORMALIZATION).
        /// Depends on
        /// <see cref="HorizontalHeader.numberOfHMetrics"/>
        /// and
        /// <see cref="HeaderTable.unitsPerEm"/>.
        /// </remarks>
        protected internal virtual void ReadGlyphWidths() {
            int numberOfHMetrics = hhea.numberOfHMetrics;
            int unitsPerEm = head.unitsPerEm;
            int[] table_location;
            table_location = tables.Get("hmtx");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("hmtx", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("hmtx");
                }
            }
            glyphWidthsByIndex = new int[ReadNumGlyphs()];
            raf.Seek(table_location[0]);
            for (int k = 0; k < numberOfHMetrics; ++k) {
                glyphWidthsByIndex[k] = FontProgram.ConvertGlyphSpaceToTextSpace(raf.ReadUnsignedShort()) / unitsPerEm;
                int leftSideBearing = FontProgram.ConvertGlyphSpaceToTextSpace(raf.ReadShort()) / unitsPerEm;
            }
            // If the font is monospaced, only one entry need be in the array, but that entry is required.
            // The last entry applies to all subsequent glyphs.
            if (numberOfHMetrics > 0) {
                for (int k = numberOfHMetrics; k < glyphWidthsByIndex.Length; k++) {
                    glyphWidthsByIndex[k] = glyphWidthsByIndex[numberOfHMetrics - 1];
                }
            }
        }

        /// <summary>Reads the kerning information from the 'kern' table.</summary>
        /// <param name="unitsPerEm">
        /// 
        /// <see cref="HeaderTable.unitsPerEm"/>.
        /// </param>
        protected internal virtual IntHashtable ReadKerning(int unitsPerEm) {
            int[] table_location;
            table_location = tables.Get("kern");
            IntHashtable kerning = new IntHashtable();
            if (table_location == null) {
                return kerning;
            }
            raf.Seek(table_location[0] + 2);
            int nTables = raf.ReadUnsignedShort();
            int checkpoint = table_location[0] + 4;
            int length = 0;
            for (int k = 0; k < nTables; k++) {
                checkpoint += length;
                raf.Seek(checkpoint);
                raf.SkipBytes(2);
                length = raf.ReadUnsignedShort();
                int coverage = raf.ReadUnsignedShort();
                if ((coverage & 0xfff7) == 0x0001) {
                    int nPairs = raf.ReadUnsignedShort();
                    raf.SkipBytes(6);
                    for (int j = 0; j < nPairs; ++j) {
                        int pair = raf.ReadInt();
                        int value = FontProgram.ConvertGlyphSpaceToTextSpace(raf.ReadShort()) / unitsPerEm;
                        kerning.Put(pair, value);
                    }
                }
            }
            return kerning;
        }

        /// <summary>Read the glyf bboxes from 'glyf' table.</summary>
        /// <param name="unitsPerEm">
        /// 
        /// <see cref="HeaderTable.unitsPerEm"/>
        /// </param>
        protected internal virtual int[][] ReadBbox(int unitsPerEm) {
            int[] tableLocation;
            tableLocation = tables.Get("head");
            if (tableLocation == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("head", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("head");
                }
            }
            raf.Seek(tableLocation[0] + HEAD_LOCA_FORMAT_OFFSET);
            bool locaShortTable = raf.ReadUnsignedShort() == 0;
            tableLocation = tables.Get("loca");
            if (tableLocation == null) {
                return null;
            }
            raf.Seek(tableLocation[0]);
            int[] locaTable;
            if (locaShortTable) {
                int entries = tableLocation[1] / 2;
                locaTable = new int[entries];
                for (int k = 0; k < entries; ++k) {
                    locaTable[k] = raf.ReadUnsignedShort() * 2;
                }
            }
            else {
                int entries = tableLocation[1] / 4;
                locaTable = new int[entries];
                for (int k = 0; k < entries; ++k) {
                    locaTable[k] = raf.ReadInt();
                }
            }
            tableLocation = tables.Get("glyf");
            if (tableLocation == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("glyf", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("glyf");
                }
            }
            int tableGlyphOffset = tableLocation[0];
            int[][] bboxes = new int[locaTable.Length - 1][];
            for (int glyph = 0; glyph < locaTable.Length - 1; ++glyph) {
                int start = locaTable[glyph];
                if (start != locaTable[glyph + 1]) {
                    raf.Seek(tableGlyphOffset + start + 2);
                    bboxes[glyph] = new int[] { FontProgram.ConvertGlyphSpaceToTextSpace(raf.ReadShort()) / unitsPerEm, FontProgram
                        .ConvertGlyphSpaceToTextSpace(raf.ReadShort()) / unitsPerEm, FontProgram.ConvertGlyphSpaceToTextSpace(
                        raf.ReadShort()) / unitsPerEm, FontProgram.ConvertGlyphSpaceToTextSpace(raf.ReadShort()) / unitsPerEm };
                }
            }
            return bboxes;
        }

        protected internal virtual int ReadNumGlyphs() {
            int[] table_location = tables.Get("maxp");
            if (table_location == null) {
                return 65536;
            }
            else {
                raf.Seek(table_location[0] + 4);
                return raf.ReadUnsignedShort();
            }
        }

        /// <summary>Extracts the names of the font in all the languages available.</summary>
        private void ReadNameTable() {
            int[] table_location = tables.Get("name");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("name", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("name");
                }
            }
            allNameEntries = new LinkedDictionary<int, IList<String[]>>();
            raf.Seek(table_location[0] + 2);
            int numRecords = raf.ReadUnsignedShort();
            int startOfStorage = raf.ReadUnsignedShort();
            for (int k = 0; k < numRecords; ++k) {
                int platformID = raf.ReadUnsignedShort();
                int platformEncodingID = raf.ReadUnsignedShort();
                int languageID = raf.ReadUnsignedShort();
                int nameID = raf.ReadUnsignedShort();
                int length = raf.ReadUnsignedShort();
                int offset = raf.ReadUnsignedShort();
                IList<String[]> names;
                if (allNameEntries.ContainsKey(nameID)) {
                    names = allNameEntries.Get(nameID);
                }
                else {
                    allNameEntries.Put(nameID, names = new List<String[]>());
                }
                int pos = (int)raf.GetPosition();
                raf.Seek(table_location[0] + startOfStorage + offset);
                String name;
                if (platformID == 0 || platformID == 3 || platformID == 2 && platformEncodingID == 1) {
                    name = ReadUnicodeString(length);
                }
                else {
                    name = ReadStandardString(length);
                }
                names.Add(new String[] { JavaUtil.IntegerToString(platformID), JavaUtil.IntegerToString(platformEncodingID
                    ), JavaUtil.IntegerToString(languageID), name });
                raf.Seek(pos);
            }
        }

        /// <summary>Read horizontal header, table 'hhea'.</summary>
        private void ReadHheaTable() {
            int[] table_location = tables.Get("hhea");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("hhea", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("hhea");
                }
            }
            raf.Seek(table_location[0] + 4);
            hhea = new OpenTypeParser.HorizontalHeader();
            hhea.Ascender = raf.ReadShort();
            hhea.Descender = raf.ReadShort();
            hhea.LineGap = raf.ReadShort();
            hhea.advanceWidthMax = raf.ReadUnsignedShort();
            hhea.minLeftSideBearing = raf.ReadShort();
            hhea.minRightSideBearing = raf.ReadShort();
            hhea.xMaxExtent = raf.ReadShort();
            hhea.caretSlopeRise = raf.ReadShort();
            hhea.caretSlopeRun = raf.ReadShort();
            raf.SkipBytes(12);
            hhea.numberOfHMetrics = raf.ReadUnsignedShort();
        }

        /// <summary>Read font header, table 'head'.</summary>
        private void ReadHeadTable() {
            int[] table_location = tables.Get("head");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("head", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("head");
                }
            }
            raf.Seek(table_location[0] + 16);
            head = new OpenTypeParser.HeaderTable();
            head.flags = raf.ReadUnsignedShort();
            head.unitsPerEm = raf.ReadUnsignedShort();
            raf.SkipBytes(16);
            head.xMin = raf.ReadShort();
            head.yMin = raf.ReadShort();
            head.xMax = raf.ReadShort();
            head.yMax = raf.ReadShort();
            head.macStyle = raf.ReadUnsignedShort();
        }

        /// <summary>Reads the windows metrics table.</summary>
        /// <remarks>
        /// Reads the windows metrics table. The metrics are extracted from the table 'OS/2'.
        /// Depends on
        /// <see cref="HeaderTable.unitsPerEm"/>
        /// property.
        /// </remarks>
        private void ReadOs_2Table() {
            int[] table_location = tables.Get("OS/2");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("os/2", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("os/2");
                }
            }
            os_2 = new OpenTypeParser.WindowsMetrics();
            raf.Seek(table_location[0]);
            int version = raf.ReadUnsignedShort();
            os_2.xAvgCharWidth = raf.ReadShort();
            os_2.usWeightClass = raf.ReadUnsignedShort();
            os_2.usWidthClass = raf.ReadUnsignedShort();
            os_2.fsType = raf.ReadShort();
            os_2.ySubscriptXSize = raf.ReadShort();
            os_2.ySubscriptYSize = raf.ReadShort();
            os_2.ySubscriptXOffset = raf.ReadShort();
            os_2.ySubscriptYOffset = raf.ReadShort();
            os_2.ySuperscriptXSize = raf.ReadShort();
            os_2.ySuperscriptYSize = raf.ReadShort();
            os_2.ySuperscriptXOffset = raf.ReadShort();
            os_2.ySuperscriptYOffset = raf.ReadShort();
            os_2.yStrikeoutSize = raf.ReadShort();
            os_2.yStrikeoutPosition = raf.ReadShort();
            os_2.sFamilyClass = raf.ReadShort();
            raf.ReadFully(os_2.panose);
            raf.SkipBytes(16);
            raf.ReadFully(os_2.achVendID);
            os_2.fsSelection = raf.ReadUnsignedShort();
            os_2.usFirstCharIndex = raf.ReadUnsignedShort();
            os_2.usLastCharIndex = raf.ReadUnsignedShort();
            os_2.sTypoAscender = raf.ReadShort();
            os_2.sTypoDescender = raf.ReadShort();
            if (os_2.sTypoDescender > 0) {
                os_2.sTypoDescender = (short)-os_2.sTypoDescender;
            }
            os_2.sTypoLineGap = raf.ReadShort();
            os_2.usWinAscent = raf.ReadUnsignedShort();
            os_2.usWinDescent = raf.ReadUnsignedShort();
            if (os_2.usWinDescent > 0) {
                os_2.usWinDescent = (short)-os_2.usWinDescent;
            }
            os_2.ulCodePageRange1 = 0;
            os_2.ulCodePageRange2 = 0;
            if (version > 0) {
                os_2.ulCodePageRange1 = raf.ReadInt();
                os_2.ulCodePageRange2 = raf.ReadInt();
            }
            if (version > 1) {
                raf.SkipBytes(2);
                os_2.sCapHeight = raf.ReadShort();
            }
            else {
                os_2.sCapHeight = (int)(0.7 * head.unitsPerEm);
            }
        }

        private void ReadPostTable() {
            int[] table_location = tables.Get("post");
            if (table_location != null) {
                raf.Seek(table_location[0] + 4);
                short mantissa = raf.ReadShort();
                int fraction = raf.ReadUnsignedShort();
                post = new OpenTypeParser.PostTable();
                post.italicAngle = (float)(mantissa + fraction / 16384.0d);
                post.underlinePosition = raf.ReadShort();
                post.underlineThickness = raf.ReadShort();
                post.isFixedPitch = raf.ReadInt() != 0;
            }
            else {
                post = new OpenTypeParser.PostTable();
                post.italicAngle = (float)(-Math.Atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI);
            }
        }

        /// <summary>Reads the several maps from the table 'cmap'.</summary>
        /// <remarks>
        /// Reads the several maps from the table 'cmap'. The maps of interest are 1.0 for symbolic
        /// fonts and 3.1 for all others. A symbolic font is defined as having the map 3.0.
        /// Depends from
        /// <c>readGlyphWidths()</c>.
        /// </remarks>
        private void ReadCmapTable() {
            int[] table_location = tables.Get("cmap");
            if (table_location == null) {
                if (fileName != null) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXISTS_IN).SetMessageParams
                        ("cmap", fileName);
                }
                else {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.TABLE_DOES_NOT_EXIST).SetMessageParams
                        ("cmap");
                }
            }
            raf.Seek(table_location[0]);
            raf.SkipBytes(2);
            int num_tables = raf.ReadUnsignedShort();
            int map10 = 0;
            int map31 = 0;
            int map30 = 0;
            int mapExt = 0;
            cmaps = new OpenTypeParser.CmapTable();
            for (int k = 0; k < num_tables; ++k) {
                int platId = raf.ReadUnsignedShort();
                int platSpecId = raf.ReadUnsignedShort();
                int offset = raf.ReadInt();
                if (platId == 3 && platSpecId == 0) {
                    cmaps.fontSpecific = true;
                    map30 = offset;
                }
                else {
                    if (platId == 3 && platSpecId == 1) {
                        map31 = offset;
                    }
                    else {
                        if (platId == 3 && platSpecId == 10) {
                            mapExt = offset;
                        }
                        else {
                            if (platId == 1 && platSpecId == 0) {
                                map10 = offset;
                            }
                        }
                    }
                }
            }
            if (map10 > 0) {
                raf.Seek(table_location[0] + map10);
                int format = raf.ReadUnsignedShort();
                switch (format) {
                    case 0: {
                        cmaps.cmap10 = ReadFormat0();
                        break;
                    }

                    case 4: {
                        cmaps.cmap10 = ReadFormat4(false);
                        break;
                    }

                    case 6: {
                        cmaps.cmap10 = ReadFormat6();
                        break;
                    }
                }
            }
            if (map31 > 0) {
                raf.Seek(table_location[0] + map31);
                int format = raf.ReadUnsignedShort();
                if (format == 4) {
                    cmaps.cmap31 = ReadFormat4(false);
                }
            }
            if (map30 > 0) {
                raf.Seek(table_location[0] + map30);
                int format = raf.ReadUnsignedShort();
                if (format == 4) {
                    cmaps.cmap10 = ReadFormat4(cmaps.fontSpecific);
                }
                else {
                    cmaps.fontSpecific = false;
                }
            }
            if (mapExt > 0) {
                raf.Seek(table_location[0] + mapExt);
                int format = raf.ReadUnsignedShort();
                switch (format) {
                    case 0: {
                        cmaps.cmapExt = ReadFormat0();
                        break;
                    }

                    case 4: {
                        cmaps.cmapExt = ReadFormat4(false);
                        break;
                    }

                    case 6: {
                        cmaps.cmapExt = ReadFormat6();
                        break;
                    }

                    case 12: {
                        cmaps.cmapExt = ReadFormat12();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads a <c>String</c> from the font file as bytes using the Cp1252
        /// encoding.
        /// </summary>
        /// <param name="length">the length of bytes to read</param>
        /// <returns>the <c>String</c> read</returns>
        private String ReadStandardString(int length) {
            return raf.ReadString(length, PdfEncodings.WINANSI);
        }

        /// <summary>Reads a Unicode <c>String</c> from the font file.</summary>
        /// <remarks>Reads a Unicode <c>String</c> from the font file. Each character is represented by two bytes.</remarks>
        /// <param name="length">the length of bytes to read. The <c>String</c> will have <c>length</c>/2 characters.</param>
        /// <returns>the <c>String</c> read.</returns>
        private String ReadUnicodeString(int length) {
            StringBuilder buf = new StringBuilder();
            length /= 2;
            for (int k = 0; k < length; ++k) {
                buf.Append(raf.ReadChar());
            }
            return buf.ToString();
        }

        /// <summary>Gets a glyph width.</summary>
        /// <param name="glyph">the glyph to get the width of</param>
        /// <returns>the width of the glyph in normalized 1000 units (TrueTypeFont.UNITS_NORMALIZATION)</returns>
        protected internal virtual int GetGlyphWidth(int glyph) {
            if (glyph >= glyphWidthsByIndex.Length) {
                glyph = glyphWidthsByIndex.Length - 1;
            }
            return glyphWidthsByIndex[glyph];
        }

        /// <summary>The information in the maps of the table 'cmap' is coded in several formats.</summary>
        /// <remarks>
        /// The information in the maps of the table 'cmap' is coded in several formats.
        /// Format 0 is the Apple standard character to glyph index mapping table.
        /// </remarks>
        /// <returns>a <c>HashMap</c> representing this map</returns>
        private IDictionary<int, int[]> ReadFormat0() {
            IDictionary<int, int[]> h = new LinkedDictionary<int, int[]>();
            raf.SkipBytes(4);
            for (int k = 0; k < 256; ++k) {
                int[] r = new int[2];
                r[0] = raf.ReadUnsignedByte();
                r[1] = GetGlyphWidth(r[0]);
                h.Put(k, r);
            }
            return h;
        }

        /// <summary>The information in the maps of the table 'cmap' is coded in several formats.</summary>
        /// <remarks>
        /// The information in the maps of the table 'cmap' is coded in several formats.
        /// Format 4 is the Microsoft standard character to glyph index mapping table.
        /// </remarks>
        /// <returns>a <c>HashMap</c> representing this map</returns>
        private IDictionary<int, int[]> ReadFormat4(bool fontSpecific) {
            IDictionary<int, int[]> h = new LinkedDictionary<int, int[]>();
            int table_lenght = raf.ReadUnsignedShort();
            raf.SkipBytes(2);
            int segCount = raf.ReadUnsignedShort() / 2;
            raf.SkipBytes(6);
            int[] endCount = new int[segCount];
            for (int k = 0; k < segCount; ++k) {
                endCount[k] = raf.ReadUnsignedShort();
            }
            raf.SkipBytes(2);
            int[] startCount = new int[segCount];
            for (int k = 0; k < segCount; ++k) {
                startCount[k] = raf.ReadUnsignedShort();
            }
            int[] idDelta = new int[segCount];
            for (int k = 0; k < segCount; ++k) {
                idDelta[k] = raf.ReadUnsignedShort();
            }
            int[] idRO = new int[segCount];
            for (int k = 0; k < segCount; ++k) {
                idRO[k] = raf.ReadUnsignedShort();
            }
            int[] glyphId = new int[table_lenght / 2 - 8 - segCount * 4];
            for (int k = 0; k < glyphId.Length; ++k) {
                glyphId[k] = raf.ReadUnsignedShort();
            }
            for (int k = 0; k < segCount; ++k) {
                int glyph;
                for (int j = startCount[k]; j <= endCount[k] && j != 0xFFFF; ++j) {
                    if (idRO[k] == 0) {
                        glyph = j + idDelta[k] & 0xFFFF;
                    }
                    else {
                        int idx = k + idRO[k] / 2 - segCount + j - startCount[k];
                        if (idx >= glyphId.Length) {
                            continue;
                        }
                        glyph = glyphId[idx] + idDelta[k] & 0xFFFF;
                    }
                    int[] r = new int[2];
                    r[0] = glyph;
                    r[1] = GetGlyphWidth(r[0]);
                    // (j & 0xff00) == 0xf000) means, that it is private area of unicode
                    // So, in case symbol font (cmap 3/0) we add both char codes:
                    // j & 0xff and j. It will simplify unicode conversion in TrueTypeFont
                    if (fontSpecific && ((j & 0xff00) == 0xf000)) {
                        h.Put(j & 0xff, r);
                    }
                    h.Put(j, r);
                }
            }
            return h;
        }

        /// <summary>The information in the maps of the table 'cmap' is coded in several formats.</summary>
        /// <remarks>
        /// The information in the maps of the table 'cmap' is coded in several formats.
        /// Format 6 is a trimmed table mapping. It is similar to format 0 but can have
        /// less than 256 entries.
        /// </remarks>
        /// <returns>a <c>HashMap</c> representing this map</returns>
        private IDictionary<int, int[]> ReadFormat6() {
            IDictionary<int, int[]> h = new LinkedDictionary<int, int[]>();
            raf.SkipBytes(4);
            int start_code = raf.ReadUnsignedShort();
            int code_count = raf.ReadUnsignedShort();
            for (int k = 0; k < code_count; ++k) {
                int[] r = new int[2];
                r[0] = raf.ReadUnsignedShort();
                r[1] = GetGlyphWidth(r[0]);
                h.Put(k + start_code, r);
            }
            return h;
        }

        private IDictionary<int, int[]> ReadFormat12() {
            IDictionary<int, int[]> h = new LinkedDictionary<int, int[]>();
            raf.SkipBytes(2);
            int table_length = raf.ReadInt();
            raf.SkipBytes(4);
            int nGroups = raf.ReadInt();
            for (int k = 0; k < nGroups; k++) {
                int startCharCode = raf.ReadInt();
                int endCharCode = raf.ReadInt();
                int startGlyphID = raf.ReadInt();
                for (int i = startCharCode; i <= endCharCode; i++) {
                    int[] r = new int[2];
                    r[0] = startGlyphID;
                    r[1] = GetGlyphWidth(r[0]);
                    h.Put(i, r);
                    startGlyphID++;
                }
            }
            return h;
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
