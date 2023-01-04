/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Cmap;
using iText.IO.Util;
using iText.Kernel.Pdf;

namespace iText.Kernel.Font {
    /// <summary>This class allow to parse document font's encoding.</summary>
    internal class DocFontEncoding : FontEncoding {
        protected internal DocFontEncoding() {
        }

        public static FontEncoding CreateDocFontEncoding(PdfObject encoding, CMapToUnicode toUnicode) {
            if (encoding != null) {
                if (encoding.IsName()) {
                    return FontEncoding.CreateFontEncoding(((PdfName)encoding).GetValue());
                }
                else {
                    if (encoding.IsDictionary()) {
                        iText.Kernel.Font.DocFontEncoding fontEncoding = new iText.Kernel.Font.DocFontEncoding();
                        fontEncoding.differences = new String[256];
                        FillBaseEncoding(fontEncoding, ((PdfDictionary)encoding).GetAsName(PdfName.BaseEncoding));
                        FillDifferences(fontEncoding, ((PdfDictionary)encoding).GetAsArray(PdfName.Differences), toUnicode);
                        return fontEncoding;
                    }
                }
            }
            if (toUnicode != null) {
                iText.Kernel.Font.DocFontEncoding fontEncoding = new iText.Kernel.Font.DocFontEncoding();
                fontEncoding.differences = new String[256];
                FillDifferences(fontEncoding, toUnicode);
                return fontEncoding;
            }
            else {
                return FontEncoding.CreateFontSpecificEncoding();
            }
        }

        private static void FillBaseEncoding(iText.Kernel.Font.DocFontEncoding fontEncoding, PdfName baseEncodingName
            ) {
            if (baseEncodingName != null) {
                fontEncoding.baseEncoding = baseEncodingName.GetValue();
            }
            if (PdfName.MacRomanEncoding.Equals(baseEncodingName) || PdfName.WinAnsiEncoding.Equals(baseEncodingName) 
                || PdfName.Symbol.Equals(baseEncodingName) || PdfName.ZapfDingbats.Equals(baseEncodingName)) {
                String enc = PdfEncodings.WINANSI;
                if (PdfName.MacRomanEncoding.Equals(baseEncodingName)) {
                    enc = PdfEncodings.MACROMAN;
                }
                else {
                    if (PdfName.Symbol.Equals(baseEncodingName)) {
                        enc = PdfEncodings.SYMBOL;
                    }
                    else {
                        if (PdfName.ZapfDingbats.Equals(baseEncodingName)) {
                            enc = PdfEncodings.ZAPFDINGBATS;
                        }
                    }
                }
                fontEncoding.baseEncoding = enc;
                fontEncoding.FillNamedEncoding();
            }
            else {
                // Actually, font's built in encoding should be used if font file is embedded
                // and standard encoding should be used otherwise
                fontEncoding.FillStandardEncoding();
            }
        }

        private static void FillDifferences(iText.Kernel.Font.DocFontEncoding fontEncoding, PdfArray diffs, CMapToUnicode
             toUnicode) {
            IntHashtable byte2uni = toUnicode != null ? toUnicode.CreateDirectMapping() : new IntHashtable();
            if (diffs != null) {
                int currentNumber = 0;
                for (int k = 0; k < diffs.Size(); ++k) {
                    PdfObject obj = diffs.Get(k);
                    if (obj.IsNumber()) {
                        currentNumber = ((PdfNumber)obj).IntValue();
                    }
                    else {
                        if (currentNumber > 255) {
                            ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Font.DocFontEncoding));
                            LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.DOCFONT_HAS_ILLEGAL_DIFFERENCES
                                , ((PdfName)obj).GetValue()));
                        }
                        else {
                            /* don't return or break, because differences subarrays may
                            * be in any order:
                            * e.g. [255 /space /one 250 /two /three]
                            * /one is invalid but all others should be parsed
                            */
                            String glyphName = ((PdfName)obj).GetValue();
                            int unicode = AdobeGlyphList.NameToUnicode(glyphName);
                            if (unicode != -1) {
                                fontEncoding.codeToUnicode[currentNumber] = (int)unicode;
                                fontEncoding.unicodeToCode.Put((int)unicode, currentNumber);
                                fontEncoding.differences[currentNumber] = glyphName;
                                fontEncoding.unicodeDifferences.Put((int)unicode, (int)unicode);
                            }
                            else {
                                if (byte2uni.ContainsKey(currentNumber)) {
                                    unicode = byte2uni.Get(currentNumber);
                                    fontEncoding.codeToUnicode[currentNumber] = (int)unicode;
                                    fontEncoding.unicodeToCode.Put((int)unicode, currentNumber);
                                    fontEncoding.differences[currentNumber] = glyphName;
                                    fontEncoding.unicodeDifferences.Put((int)unicode, (int)unicode);
                                }
                            }
                            currentNumber++;
                        }
                    }
                }
            }
        }

        private static void FillDifferences(iText.Kernel.Font.DocFontEncoding fontEncoding, CMapToUnicode toUnicode
            ) {
            IntHashtable byte2uni = toUnicode.CreateDirectMapping();
            foreach (int? code in byte2uni.GetKeys()) {
                int unicode = byte2uni.Get((int)code);
                String glyphName = AdobeGlyphList.UnicodeToName(unicode);
                fontEncoding.codeToUnicode[(int)code] = unicode;
                fontEncoding.unicodeToCode.Put(unicode, (int)code);
                fontEncoding.differences[(int)code] = glyphName;
                fontEncoding.unicodeDifferences.Put(unicode, unicode);
            }
        }
    }
}
