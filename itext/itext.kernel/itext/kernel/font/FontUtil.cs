/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Cmap;
using iText.IO.Util;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    public class FontUtil {
        private static readonly RNGCryptoServiceProvider NUMBER_GENERATOR = new RNGCryptoServiceProvider();

        private static readonly Dictionary<String, CMapToUnicode> uniMaps = new Dictionary<String, CMapToUnicode>(
            );

        private FontUtil() {
        }

        public static String AddRandomSubsetPrefixForFontName(String fontName) {
            StringBuilder newFontName = new StringBuilder(fontName.Length + 7);
            byte[] randomByte = new byte[1];
            for (int k = 0; k < 6; ++k) {
                NUMBER_GENERATOR.GetBytes(randomByte);
                newFontName.Append((char)(Math.Abs(randomByte[0] % 26) + 'A'));
            }
            newFontName.Append('+').Append(fontName);
            return newFontName.ToString();
        }

        internal static CMapToUnicode ProcessToUnicode(PdfObject toUnicode) {
            CMapToUnicode cMapToUnicode = null;
            if (toUnicode is PdfStream) {
                try {
                    byte[] uniBytes = ((PdfStream)toUnicode).GetBytes();
                    ICMapLocation lb = new CMapLocationFromBytes(uniBytes);
                    cMapToUnicode = new CMapToUnicode();
                    CMapParser.ParseCid("", cMapToUnicode, lb);
                }
                catch (Exception) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(CMapToUnicode));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP);
                    cMapToUnicode = CMapToUnicode.EmptyCMapToUnicodeMap;
                }
            }
            else {
                if (PdfName.IdentityH.Equals(toUnicode)) {
                    cMapToUnicode = CMapToUnicode.GetIdentity();
                }
            }
            return cMapToUnicode;
        }

        internal static CMapToUnicode GetToUnicodeFromUniMap(String uniMap) {
            if (uniMap == null) {
                return null;
            }
            lock (uniMaps) {
                if (uniMaps.Contains(uniMap)) {
                    return uniMaps.Get(uniMap);
                }
                CMapToUnicode toUnicode;
                if (PdfEncodings.IDENTITY_H.Equals(uniMap)) {
                    toUnicode = CMapToUnicode.GetIdentity();
                }
                else {
                    CMapUniCid uni = FontCache.GetUni2CidCmap(uniMap);
                    if (uni == null) {
                        return null;
                    }
                    toUnicode = uni.ExportToUnicode();
                }
                uniMaps.Put(uniMap, toUnicode);
                return toUnicode;
            }
        }

        internal static String CreateRandomFontName() {
            StringBuilder s = new StringBuilder("");
            for (int k = 0; k < 7; ++k) {
                s.Append((char)(JavaUtil.Random() * 26 + 'A'));
            }
            return s.ToString();
        }

        internal static int[] ConvertSimpleWidthsArray(PdfArray widthsArray, int first, int missingWidth) {
            int[] res = new int[256];
            JavaUtil.Fill(res, missingWidth);
            if (widthsArray == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.FontUtil));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.FONT_DICTIONARY_WITH_NO_WIDTHS);
                return res;
            }
            for (int i = 0; i < widthsArray.Size() && first + i < 256; i++) {
                PdfNumber number = widthsArray.GetAsNumber(i);
                res[first + i] = number != null ? number.IntValue() : missingWidth;
            }
            return res;
        }

        internal static IntHashtable ConvertCompositeWidthsArray(PdfArray widthsArray) {
            IntHashtable res = new IntHashtable();
            if (widthsArray == null) {
                return res;
            }
            for (int k = 0; k < widthsArray.Size(); ++k) {
                int c1 = widthsArray.GetAsNumber(k).IntValue();
                PdfObject obj = widthsArray.Get(++k);
                if (obj.IsArray()) {
                    PdfArray subWidths = (PdfArray)obj;
                    for (int j = 0; j < subWidths.Size(); ++j) {
                        int c2 = subWidths.GetAsNumber(j).IntValue();
                        res.Put(c1++, c2);
                    }
                }
                else {
                    int c2 = ((PdfNumber)obj).IntValue();
                    int w = widthsArray.GetAsNumber(++k).IntValue();
                    for (; c1 <= c2; ++c1) {
                        res.Put(c1, w);
                    }
                }
            }
            return res;
        }
    }
}
