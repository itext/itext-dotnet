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
using iText.IO.Font.Constants;
using iText.IO.Font.Woff2;

namespace iText.IO.Font {
    public sealed class FontProgramDescriptorFactory {
        private static bool FETCH_CACHED_FIRST = true;

        public static FontProgramDescriptor FetchDescriptor(String fontName) {
            if (fontName == null || fontName.Length == 0) {
                return null;
            }
            String baseName = FontProgram.TrimFontStyle(fontName);
            //yes, we trying to find built-in standard font with original name, not baseName.
            bool isBuiltinFonts14 = StandardFonts.IsStandardFont(fontName);
            bool isCidFont = !isBuiltinFonts14 && CjkResourceLoader.IsPredefinedCidFont(baseName);
            FontProgramDescriptor fontDescriptor = null;
            if (FETCH_CACHED_FIRST) {
                fontDescriptor = FetchCachedDescriptor(fontName, null);
                if (fontDescriptor != null) {
                    return fontDescriptor;
                }
            }
            try {
                String fontNameLowerCase = baseName.ToLowerInvariant();
                if (isBuiltinFonts14 || fontNameLowerCase.EndsWith(".afm") || fontNameLowerCase.EndsWith(".pfm")) {
                    fontDescriptor = FetchType1FontDescriptor(fontName, null);
                }
                else {
                    if (isCidFont) {
                        fontDescriptor = FetchCidFontDescriptor(fontName);
                    }
                    else {
                        if (fontNameLowerCase.EndsWith(".ttf") || fontNameLowerCase.EndsWith(".otf")) {
                            fontDescriptor = FetchTrueTypeFontDescriptor(fontName);
                        }
                        else {
                            if (fontNameLowerCase.EndsWith(".woff") || fontNameLowerCase.EndsWith(".woff2")) {
                                byte[] fontProgram;
                                if (fontNameLowerCase.EndsWith(".woff")) {
                                    fontProgram = WoffConverter.Convert(FontProgramFactory.ReadFontBytesFromPath(baseName));
                                }
                                else {
                                    fontProgram = Woff2Converter.Convert(FontProgramFactory.ReadFontBytesFromPath(baseName));
                                }
                                fontDescriptor = FetchTrueTypeFontDescriptor(fontProgram);
                            }
                            else {
                                fontDescriptor = FetchTTCDescriptor(baseName);
                            }
                        }
                    }
                }
            }
            catch (Exception) {
                fontDescriptor = null;
            }
            return fontDescriptor;
        }

        public static FontProgramDescriptor FetchDescriptor(byte[] fontProgram) {
            if (fontProgram == null || fontProgram.Length == 0) {
                return null;
            }
            FontProgramDescriptor fontDescriptor = null;
            if (FETCH_CACHED_FIRST) {
                fontDescriptor = FetchCachedDescriptor(null, fontProgram);
                if (fontDescriptor != null) {
                    return fontDescriptor;
                }
            }
            try {
                fontDescriptor = FetchTrueTypeFontDescriptor(fontProgram);
            }
            catch (Exception) {
            }
            if (fontDescriptor == null) {
                try {
                    fontDescriptor = FetchType1FontDescriptor(null, fontProgram);
                }
                catch (Exception) {
                }
            }
            return fontDescriptor;
        }

        public static FontProgramDescriptor FetchDescriptor(FontProgram fontProgram) {
            return FetchDescriptorFromFontProgram(fontProgram);
        }

        private static FontProgramDescriptor FetchCachedDescriptor(String fontName, byte[] fontProgram) {
            FontProgram fontFound;
            FontCacheKey key;
            if (fontName != null) {
                key = FontCacheKey.Create(fontName);
            }
            else {
                key = FontCacheKey.Create(fontProgram);
            }
            fontFound = FontCache.GetFont(key);
            return fontFound != null ? FetchDescriptorFromFontProgram(fontFound) : null;
        }

        private static FontProgramDescriptor FetchTTCDescriptor(String baseName) {
            int ttcSplit = baseName.ToLowerInvariant().IndexOf(".ttc,", StringComparison.Ordinal);
            if (ttcSplit > 0) {
                String ttcName;
                int ttcIndex;
                try {
                    // count(.ttc) = 4
                    ttcName = baseName.JSubstring(0, ttcSplit + 4);
                    // count(.ttc,) = 5)
                    ttcIndex = Convert.ToInt32(baseName.Substring(ttcSplit + 5), System.Globalization.CultureInfo.InvariantCulture
                        );
                }
                catch (FormatException nfe) {
                    throw new iText.IO.Exceptions.IOException(nfe.Message, nfe);
                }
                OpenTypeParser parser = new OpenTypeParser(ttcName, ttcIndex);
                FontProgramDescriptor descriptor = FetchOpenTypeFontDescriptor(parser);
                parser.Close();
                return descriptor;
            }
            else {
                return null;
            }
        }

        private static FontProgramDescriptor FetchTrueTypeFontDescriptor(String fontName) {
            using (OpenTypeParser parser = new OpenTypeParser(fontName)) {
                return FetchOpenTypeFontDescriptor(parser);
            }
        }

        private static FontProgramDescriptor FetchTrueTypeFontDescriptor(byte[] fontProgram) {
            using (OpenTypeParser parser = new OpenTypeParser(fontProgram)) {
                return FetchOpenTypeFontDescriptor(parser);
            }
        }

        private static FontProgramDescriptor FetchOpenTypeFontDescriptor(OpenTypeParser fontParser) {
            fontParser.LoadTables(false);
            return new FontProgramDescriptor(fontParser.GetFontNames(), fontParser.GetPostTable().italicAngle, fontParser
                .GetPostTable().isFixedPitch);
        }

        private static FontProgramDescriptor FetchType1FontDescriptor(String fontName, byte[] afm) {
            Type1Font fp = new Type1Font(fontName, null, afm, null);
            return new FontProgramDescriptor(fp.GetFontNames(), fp.GetFontMetrics());
        }

        private static FontProgramDescriptor FetchCidFontDescriptor(String fontName) {
            CidFont font = new CidFont(fontName, null, null);
            return new FontProgramDescriptor(font.GetFontNames(), font.GetFontMetrics());
        }

        private static FontProgramDescriptor FetchDescriptorFromFontProgram(FontProgram fontProgram) {
            return new FontProgramDescriptor(fontProgram.GetFontNames(), fontProgram.GetFontMetrics());
        }
    }
}
