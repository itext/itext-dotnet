using System;
using iText.IO.Util;

namespace iText.IO.Font {
    public sealed class FontNamesFactory {
        private static bool FETCH_CACHED_FIRST = true;

        public static FontNames FetchFontNames(String fontName, byte[] fontProgram) {
            String baseName = FontProgram.GetBaseName(fontName);
            //yes, we trying to find built-in standard font with original name, not baseName.
            bool isBuiltinFonts14 = FontConstants.BUILTIN_FONTS_14.Contains(fontName);
            bool isCidFont = !isBuiltinFonts14 && FontCache.IsPredefinedCidFont(baseName);
            FontNames fontNames = null;
            if (FETCH_CACHED_FIRST) {
                fontNames = FetchCachedFontNames(fontName, fontProgram);
                if (fontNames != null) {
                    return fontNames;
                }
            }
            if (fontName == null) {
                if (fontProgram != null) {
                    try {
                        fontNames = FetchTrueTypeNames(null, fontProgram);
                    }
                    catch (Exception) {
                    }
                    if (fontNames == null) {
                        try {
                            fontNames = FetchType1Names(null, fontProgram);
                        }
                        catch (Exception) {
                        }
                    }
                }
            }
            else {
                try {
                    if (isBuiltinFonts14 || fontName.ToLowerInvariant().EndsWith(".afm") || fontName.ToLowerInvariant().EndsWith
                        (".pfm")) {
                        fontNames = FetchType1Names(fontName, null);
                    }
                    else {
                        if (isCidFont) {
                            fontNames = FetchCidFontNames(fontName);
                        }
                        else {
                            if (baseName.ToLowerInvariant().EndsWith(".ttf") || baseName.ToLowerInvariant().EndsWith(".otf")) {
                                fontNames = FetchTrueTypeNames(fontName, fontProgram);
                            }
                            else {
                                fontNames = FetchTTCNames(baseName);
                            }
                        }
                    }
                }
                catch (Exception) {
                    fontNames = null;
                }
            }
            return fontNames;
        }

        private static FontNames FetchCachedFontNames(String fontName, byte[] fontProgram) {
            String fontKey;
            if (fontName != null) {
                fontKey = fontName;
            }
            else {
                fontKey = iText.IO.Util.JavaUtil.IntegerToString(ArrayUtil.HashCode(fontProgram));
            }
            FontProgram fontFound = FontCache.GetFont(fontKey);
            return fontFound != null ? fontFound.GetFontNames() : null;
        }

        /// <exception cref="System.IO.IOException"/>
        private static FontNames FetchTTCNames(String baseName) {
            int ttcSplit = baseName.ToLowerInvariant().IndexOf(".ttc,", StringComparison.Ordinal);
            if (ttcSplit > 0) {
                String ttcName;
                int ttcIndex;
                try {
                    ttcName = baseName.JSubstring(0, ttcSplit + 4);
                    //count(.ttc) = 4
                    ttcIndex = System.Convert.ToInt32(baseName.Substring(ttcSplit + 5));
                }
                catch (FormatException nfe) {
                    //count(.ttc,) = 5)
                    throw new iText.IO.IOException(nfe.Message, nfe);
                }
                OpenTypeParser parser = new OpenTypeParser(ttcName, ttcIndex);
                FontNames names = FetchOpenTypeNames(parser);
                parser.Close();
                return names;
            }
            else {
                return null;
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static FontNames FetchTrueTypeNames(String fontName, byte[] fontProgram) {
            OpenTypeParser parser;
            if (fontName != null) {
                parser = new OpenTypeParser(fontName);
            }
            else {
                parser = new OpenTypeParser(fontProgram);
            }
            FontNames names = FetchOpenTypeNames(parser);
            parser.Close();
            return names;
        }

        /// <exception cref="System.IO.IOException"/>
        private static FontNames FetchOpenTypeNames(OpenTypeParser fontParser) {
            fontParser.LoadTables(false);
            return fontParser.GetFontNames();
        }

        /// <exception cref="System.IO.IOException"/>
        private static FontNames FetchType1Names(String fontName, byte[] afm) {
            Type1Font fp = new Type1Font(fontName, null, afm, null);
            return fp.GetFontNames();
        }

        private static FontNames FetchCidFontNames(String fontName) {
            CidFont font = new CidFont(fontName, null);
            return font.GetFontNames();
        }
    }
}
