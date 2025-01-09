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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Source;

namespace iText.IO.Font {
    public class Type1Font : FontProgram {
        private Type1Parser fontParser;

        private String characterSet;

        /// <summary>Represents the section KernPairs in the AFM file.</summary>
        /// <remarks>
        /// Represents the section KernPairs in the AFM file.
        /// Key is uni1 &lt;&lt; 32 + uni2. Value is kerning value.
        /// </remarks>
        private IDictionary<long, int?> kernPairs = new Dictionary<long, int?>();

        /// <summary>Types of records in a PFB file.</summary>
        /// <remarks>Types of records in a PFB file. ASCII is 1 and BINARY is 2. They have to appear in the PFB file in this sequence.
        ///     </remarks>
        private static readonly int[] PFB_TYPES = new int[] { 1, 2, 1 };

        private byte[] fontStreamBytes;

        private int[] fontStreamLengths;

        protected internal static iText.IO.Font.Type1Font CreateStandardFont(String name) {
            if (StandardFonts.IsStandardFont(name)) {
                return new iText.IO.Font.Type1Font(name, null, null, null);
            }
            else {
                throw new iText.IO.Exceptions.IOException("{0} is not a standard type1 font.").SetMessageParams(name);
            }
        }

        protected internal Type1Font() {
            fontNames = new FontNames();
        }

        protected internal Type1Font(String metricsPath, String binaryPath, byte[] afm, byte[] pfb)
            : this() {
            fontParser = new Type1Parser(metricsPath, binaryPath, afm, pfb);
            Process();
        }

        protected internal Type1Font(String baseFont)
            : this() {
            GetFontNames().SetFontName(baseFont);
        }

        /// <summary>
        /// Fills missing character codes in
        /// <c>codeToGlyph</c>
        /// map.
        /// </summary>
        /// <param name="fontEncoding">to be used to map unicode values to character codes.</param>
        public virtual void InitializeGlyphs(FontEncoding fontEncoding) {
            for (int i = 0; i < 256; i++) {
                int unicode = fontEncoding.GetUnicode(i);
                // Original unicodeToGlyph will be the source of widths here
                Glyph fontGlyph = unicodeToGlyph.Get(unicode);
                if (fontGlyph == null) {
                    continue;
                }
                Glyph glyph = new Glyph(i, fontGlyph.GetWidth(), unicode, fontGlyph.GetChars(), false);
                codeToGlyph.Put(i, glyph);
                unicodeToGlyph.Put(glyph.GetUnicode(), glyph);
            }
        }

        public virtual bool IsBuiltInFont() {
            return fontParser != null && fontParser.IsBuiltInFont();
        }

        public override int GetPdfFontFlags() {
            int flags = 0;
            if (fontMetrics.IsFixedPitch()) {
                flags |= 1;
            }
            flags |= IsFontSpecific() ? 4 : 32;
            if (fontMetrics.GetItalicAngle() < 0) {
                flags |= 64;
            }
            if (fontNames.GetFontName().Contains("Caps") || fontNames.GetFontName().EndsWith("SC")) {
                flags |= 131072;
            }
            if (fontNames.IsBold() || fontNames.GetFontWeight() > 500) {
                flags |= 262144;
            }
            return flags;
        }

        public virtual String GetCharacterSet() {
            return characterSet;
        }

        /// <summary>Checks if the font has any kerning pairs.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the font has any kerning pairs.
        /// </returns>
        public override bool HasKernPairs() {
            return kernPairs.Count > 0;
        }

        public override int GetKerning(Glyph first, Glyph second) {
            if (first.HasValidUnicode() && second.HasValidUnicode()) {
                long record = ((long)first.GetUnicode() << 32) + second.GetUnicode();
                if (kernPairs.ContainsKey(record)) {
                    return (int)kernPairs.Get(record);
                }
                else {
                    return 0;
                }
            }
            return 0;
        }

        /// <summary>Sets the kerning between two Unicode chars.</summary>
        /// <param name="first">the first unicode char.</param>
        /// <param name="second">the second unicode char.</param>
        /// <param name="kern">the kerning to apply in normalized 1000 units.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the kerning was applied,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool SetKerning(int first, int second, int kern) {
            long record = ((long)first << 32) + second;
            kernPairs.Put(record, kern);
            return true;
        }

        /// <summary>Find glyph by glyph name.</summary>
        /// <param name="name">Glyph name</param>
        /// <returns>Glyph instance if found, otherwise null.</returns>
        public virtual Glyph GetGlyph(String name) {
            int unicode = AdobeGlyphList.NameToUnicode(name);
            if (unicode != -1) {
                return GetGlyph((int)unicode);
            }
            else {
                return null;
            }
        }

        public virtual byte[] GetFontStreamBytes() {
            if (fontParser.IsBuiltInFont()) {
                return null;
            }
            if (fontStreamBytes != null) {
                return fontStreamBytes;
            }
            RandomAccessFileOrArray raf = null;
            try {
                raf = fontParser.GetPostscriptBinary();
                int fileLength = (int)raf.Length();
                fontStreamBytes = new byte[fileLength - 18];
                fontStreamLengths = new int[3];
                int bytePtr = 0;
                for (int k = 0; k < 3; ++k) {
                    if (raf.Read() != 0x80) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Type1Font));
                        logger.LogError(iText.IO.Logs.IoLogMessageConstant.START_MARKER_MISSING_IN_PFB_FILE);
                        return null;
                    }
                    if (raf.Read() != PFB_TYPES[k]) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Type1Font));
                        logger.LogError("incorrect.segment.type.in.pfb.file");
                        return null;
                    }
                    int size = raf.Read();
                    size += raf.Read() << 8;
                    size += raf.Read() << 16;
                    size += raf.Read() << 24;
                    fontStreamLengths[k] = size;
                    while (size != 0) {
                        int got = raf.Read(fontStreamBytes, bytePtr, size);
                        if (got < 0) {
                            ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Type1Font));
                            logger.LogError("premature.end.in.pfb.file");
                            return null;
                        }
                        bytePtr += got;
                        size -= got;
                    }
                }
                return fontStreamBytes;
            }
            catch (Exception) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.IO.Font.Type1Font));
                logger.LogError("type1.font.file.exception");
                return null;
            }
            finally {
                if (raf != null) {
                    try {
                        raf.Close();
                    }
                    catch (Exception) {
                    }
                }
            }
        }

        public virtual int[] GetFontStreamLengths() {
            return fontStreamLengths;
        }

        public override bool IsBuiltWith(String fontProgram) {
            return Object.Equals(fontParser.GetAfmPath(), fontProgram);
        }

        protected internal virtual void Process() {
            RandomAccessFileOrArray raf = fontParser.GetMetricsFile();
            String line;
            bool startKernPairs = false;
            while (!startKernPairs && (line = raf.ReadLine()) != null) {
                StringTokenizer tok = new StringTokenizer(line, " ,\n\r\t\f");
                if (!tok.HasMoreTokens()) {
                    continue;
                }
                String ident = tok.NextToken();
                switch (ident) {
                    case "FontName": {
                        fontNames.SetFontName(tok.NextToken("\u00ff").Substring(1));
                        break;
                    }

                    case "FullName": {
                        String fullName = tok.NextToken("\u00ff").Substring(1);
                        fontNames.SetFullName(new String[][] { new String[] { "", "", "", fullName } });
                        break;
                    }

                    case "FamilyName": {
                        String familyName = tok.NextToken("\u00ff").Substring(1);
                        fontNames.SetFamilyName(new String[][] { new String[] { "", "", "", familyName } });
                        break;
                    }

                    case "Weight": {
                        fontNames.SetFontWeight(FontWeights.FromType1FontWeight(tok.NextToken("\u00ff").Substring(1)));
                        break;
                    }

                    case "ItalicAngle": {
                        fontMetrics.SetItalicAngle(float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture)
                            );
                        break;
                    }

                    case "IsFixedPitch": {
                        fontMetrics.SetIsFixedPitch(tok.NextToken().Equals("true"));
                        break;
                    }

                    case "CharacterSet": {
                        characterSet = tok.NextToken("\u00ff").Substring(1);
                        break;
                    }

                    case "FontBBox": {
                        int llx = (int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                        int lly = (int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                        int urx = (int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                        int ury = (int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                        fontMetrics.SetBbox(llx, lly, urx, ury);
                        break;
                    }

                    case "UnderlinePosition": {
                        fontMetrics.SetUnderlinePosition((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "UnderlineThickness": {
                        fontMetrics.SetUnderlineThickness((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "EncodingScheme": {
                        encodingScheme = tok.NextToken("\u00ff").Substring(1).Trim();
                        break;
                    }

                    case "CapHeight": {
                        fontMetrics.SetCapHeight((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "XHeight": {
                        fontMetrics.SetXHeight((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "Ascender": {
                        fontMetrics.SetTypoAscender((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "Descender": {
                        fontMetrics.SetTypoDescender((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case "StdHW": {
                        fontMetrics.SetStemH((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }

                    case "StdVW": {
                        fontMetrics.SetStemV((int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }

                    case "StartCharMetrics": {
                        startKernPairs = true;
                        break;
                    }
                }
            }
            if (!startKernPairs) {
                String metricsPath = fontParser.GetAfmPath();
                if (metricsPath != null) {
                    throw new iText.IO.Exceptions.IOException("startcharmetrics is missing in {0}.").SetMessageParams(metricsPath
                        );
                }
                else {
                    throw new iText.IO.Exceptions.IOException("startcharmetrics is missing in the metrics file.");
                }
            }
            avgWidth = 0;
            int widthCount = 0;
            while ((line = raf.ReadLine()) != null) {
                StringTokenizer tok = new StringTokenizer(line);
                if (!tok.HasMoreTokens()) {
                    continue;
                }
                String ident = tok.NextToken();
                if (ident.Equals("EndCharMetrics")) {
                    startKernPairs = false;
                    break;
                }
                int C = -1;
                int WX = 250;
                String N = "";
                int[] B = null;
                tok = new StringTokenizer(line, ";");
                while (tok.HasMoreTokens()) {
                    StringTokenizer tokc = new StringTokenizer(tok.NextToken());
                    if (!tokc.HasMoreTokens()) {
                        continue;
                    }
                    ident = tokc.NextToken();
                    switch (ident) {
                        case "C": {
                            C = Convert.ToInt32(tokc.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }

                        case "WX": {
                            WX = (int)float.Parse(tokc.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }

                        case "N": {
                            N = tokc.NextToken();
                            break;
                        }

                        case "B": {
                            B = new int[] { Convert.ToInt32(tokc.NextToken(), System.Globalization.CultureInfo.InvariantCulture), Convert.ToInt32
                                (tokc.NextToken(), System.Globalization.CultureInfo.InvariantCulture), Convert.ToInt32(tokc.NextToken(
                                ), System.Globalization.CultureInfo.InvariantCulture), Convert.ToInt32(tokc.NextToken(), System.Globalization.CultureInfo.InvariantCulture
                                ) };
                            break;
                        }
                    }
                }
                int unicode = AdobeGlyphList.NameToUnicode(N);
                Glyph glyph = new Glyph(C, WX, unicode, B);
                if (C >= 0) {
                    codeToGlyph.Put(C, glyph);
                }
                if (unicode != -1) {
                    unicodeToGlyph.Put(unicode, glyph);
                }
                avgWidth += WX;
                widthCount++;
            }
            if (widthCount != 0) {
                avgWidth /= widthCount;
            }
            if (startKernPairs) {
                String metricsPath = fontParser.GetAfmPath();
                if (metricsPath != null) {
                    throw new iText.IO.Exceptions.IOException("endcharmetrics is missing in {0}.").SetMessageParams(metricsPath
                        );
                }
                else {
                    throw new iText.IO.Exceptions.IOException("endcharmetrics is missing in the metrics file.");
                }
            }
            // From AdobeGlyphList:
            // nonbreakingspace;00A0
            // space;0020
            if (!unicodeToGlyph.ContainsKey(0x00A0)) {
                Glyph space = unicodeToGlyph.Get(0x0020);
                if (space != null) {
                    unicodeToGlyph.Put(0x00A0, new Glyph(space.GetCode(), space.GetWidth(), 0x00A0, space.GetBbox()));
                }
            }
            bool endOfMetrics = false;
            while ((line = raf.ReadLine()) != null) {
                StringTokenizer tok = new StringTokenizer(line);
                if (!tok.HasMoreTokens()) {
                    continue;
                }
                String ident = tok.NextToken();
                if (ident.Equals("EndFontMetrics")) {
                    endOfMetrics = true;
                    break;
                }
                else {
                    if (ident.Equals("StartKernPairs")) {
                        startKernPairs = true;
                        break;
                    }
                }
            }
            if (startKernPairs) {
                while ((line = raf.ReadLine()) != null) {
                    StringTokenizer tok = new StringTokenizer(line);
                    if (!tok.HasMoreTokens()) {
                        continue;
                    }
                    String ident = tok.NextToken();
                    if (ident.Equals("KPX")) {
                        String first = tok.NextToken();
                        String second = tok.NextToken();
                        int? width = (int)float.Parse(tok.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                        int firstUni = AdobeGlyphList.NameToUnicode(first);
                        int secondUni = AdobeGlyphList.NameToUnicode(second);
                        if (firstUni != -1 && secondUni != -1) {
                            long record = ((long)firstUni << 32) + secondUni;
                            kernPairs.Put(record, width);
                        }
                    }
                    else {
                        if (ident.Equals("EndKernPairs")) {
                            startKernPairs = false;
                            break;
                        }
                    }
                }
            }
            else {
                if (!endOfMetrics) {
                    String metricsPath = fontParser.GetAfmPath();
                    if (metricsPath != null) {
                        throw new iText.IO.Exceptions.IOException("endfontmetrics is missing in {0}.").SetMessageParams(metricsPath
                            );
                    }
                    else {
                        throw new iText.IO.Exceptions.IOException("endfontmetrics is missing in the metrics file.");
                    }
                }
            }
            if (startKernPairs) {
                String metricsPath = fontParser.GetAfmPath();
                if (metricsPath != null) {
                    throw new iText.IO.Exceptions.IOException("endkernpairs is missing in {0}.").SetMessageParams(metricsPath);
                }
                else {
                    throw new iText.IO.Exceptions.IOException("endkernpairs is missing in the metrics file.");
                }
            }
            raf.Close();
            isFontSpecific = !(encodingScheme.Equals("AdobeStandardEncoding") || encodingScheme.Equals("StandardEncoding"
                ));
        }
    }
}
