/*
This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
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
