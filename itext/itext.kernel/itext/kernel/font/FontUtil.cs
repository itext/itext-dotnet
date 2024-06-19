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

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.FontUtil));

        private const String UNIVERSAL_CMAP_DIR = "ToUnicode.";

        private static readonly ICollection<String> UNIVERSAL_CMAP_ORDERINGS = new HashSet<String>(JavaUtil.ArraysAsList
            ("CNS1", "GB1", "Japan1", "Korea1", "KR"));

        private FontUtil() {
        }

        public static String AddRandomSubsetPrefixForFontName(String fontName) {
            StringBuilder newFontName = GetRandomFontPrefix(6);
            newFontName.Append('+').Append(fontName);
            return newFontName.ToString();
        }

//\cond DO_NOT_DOCUMENT
        internal static CMapToUnicode ProcessToUnicode(PdfObject toUnicode) {
            CMapToUnicode cMapToUnicode = null;
            if (toUnicode is PdfStream) {
                try {
                    byte[] uniBytes = ((PdfStream)toUnicode).GetBytes();
                    ICMapLocation lb = new CMapLocationFromBytes(uniBytes);
                    cMapToUnicode = new CMapToUnicode();
                    CMapParser.ParseCid("", cMapToUnicode, lb);
                }
                catch (Exception e) {
                    LOGGER.LogError(e, iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP);
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
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static CMapToUnicode ParseUniversalToUnicodeCMap(String ordering) {
            if (!UNIVERSAL_CMAP_ORDERINGS.Contains(ordering)) {
                return null;
            }
            String cmapRelPath = UNIVERSAL_CMAP_DIR + "Adobe-" + ordering + "-UCS2";
            CMapToUnicode cMapToUnicode = new CMapToUnicode();
            try {
                CMapParser.ParseCid(cmapRelPath, cMapToUnicode, new CMapLocationResource());
            }
            catch (Exception e) {
                LOGGER.LogError(e, iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP);
                return null;
            }
            return cMapToUnicode;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
                    CMapUniCid uni = CjkResourceLoader.GetUni2CidCmap(uniMap);
                    toUnicode = uni.ExportToUnicode();
                }
                uniMaps.Put(uniMap, toUnicode);
                return toUnicode;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static String CreateRandomFontName() {
            return GetRandomFontPrefix(7).ToString();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

        private static StringBuilder GetRandomFontPrefix(int length) {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] randomByte = new byte[length];
            NUMBER_GENERATOR.GetBytes(randomByte);
            for (int k = 0; k < length; ++k) {
                stringBuilder.Append((char)(Math.Abs(randomByte[k] % 26) + 'A'));
            }
            return stringBuilder;
        }
    }
}
