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
using System.Collections.Generic;
using System.Text;
using iText.Barcodes.Exceptions;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    public class Barcode128 : Barcode1D {
        /// <summary>A type of barcode</summary>
        public const int CODE128 = 1;

        /// <summary>A type of barcode</summary>
        public const int CODE128_UCC = 2;

        /// <summary>A type of barcode</summary>
        public const int CODE128_RAW = 3;

        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 2, 1, 2, 2, 2, 2 }, new byte[] { 2, 2, 
            2, 1, 2, 2 }, new byte[] { 2, 2, 2, 2, 2, 1 }, new byte[] { 1, 2, 1, 2, 2, 3 }, new byte[] { 1, 2, 1, 
            3, 2, 2 }, new byte[] { 1, 3, 1, 2, 2, 2 }, new byte[] { 1, 2, 2, 2, 1, 3 }, new byte[] { 1, 2, 2, 3, 
            1, 2 }, new byte[] { 1, 3, 2, 2, 1, 2 }, new byte[] { 2, 2, 1, 2, 1, 3 }, new byte[] { 2, 2, 1, 3, 1, 
            2 }, new byte[] { 2, 3, 1, 2, 1, 2 }, new byte[] { 1, 1, 2, 2, 3, 2 }, new byte[] { 1, 2, 2, 1, 3, 2 }
            , new byte[] { 1, 2, 2, 2, 3, 1 }, new byte[] { 1, 1, 3, 2, 2, 2 }, new byte[] { 1, 2, 3, 1, 2, 2 }, new 
            byte[] { 1, 2, 3, 2, 2, 1 }, new byte[] { 2, 2, 3, 2, 1, 1 }, new byte[] { 2, 2, 1, 1, 3, 2 }, new byte
            [] { 2, 2, 1, 2, 3, 1 }, new byte[] { 2, 1, 3, 2, 1, 2 }, new byte[] { 2, 2, 3, 1, 1, 2 }, new byte[] 
            { 3, 1, 2, 1, 3, 1 }, new byte[] { 3, 1, 1, 2, 2, 2 }, new byte[] { 3, 2, 1, 1, 2, 2 }, new byte[] { 3
            , 2, 1, 2, 2, 1 }, new byte[] { 3, 1, 2, 2, 1, 2 }, new byte[] { 3, 2, 2, 1, 1, 2 }, new byte[] { 3, 2
            , 2, 2, 1, 1 }, new byte[] { 2, 1, 2, 1, 2, 3 }, new byte[] { 2, 1, 2, 3, 2, 1 }, new byte[] { 2, 3, 2
            , 1, 2, 1 }, new byte[] { 1, 1, 1, 3, 2, 3 }, new byte[] { 1, 3, 1, 1, 2, 3 }, new byte[] { 1, 3, 1, 3
            , 2, 1 }, new byte[] { 1, 1, 2, 3, 1, 3 }, new byte[] { 1, 3, 2, 1, 1, 3 }, new byte[] { 1, 3, 2, 3, 1
            , 1 }, new byte[] { 2, 1, 1, 3, 1, 3 }, new byte[] { 2, 3, 1, 1, 1, 3 }, new byte[] { 2, 3, 1, 3, 1, 1
             }, new byte[] { 1, 1, 2, 1, 3, 3 }, new byte[] { 1, 1, 2, 3, 3, 1 }, new byte[] { 1, 3, 2, 1, 3, 1 }, 
            new byte[] { 1, 1, 3, 1, 2, 3 }, new byte[] { 1, 1, 3, 3, 2, 1 }, new byte[] { 1, 3, 3, 1, 2, 1 }, new 
            byte[] { 3, 1, 3, 1, 2, 1 }, new byte[] { 2, 1, 1, 3, 3, 1 }, new byte[] { 2, 3, 1, 1, 3, 1 }, new byte
            [] { 2, 1, 3, 1, 1, 3 }, new byte[] { 2, 1, 3, 3, 1, 1 }, new byte[] { 2, 1, 3, 1, 3, 1 }, new byte[] 
            { 3, 1, 1, 1, 2, 3 }, new byte[] { 3, 1, 1, 3, 2, 1 }, new byte[] { 3, 3, 1, 1, 2, 1 }, new byte[] { 3
            , 1, 2, 1, 1, 3 }, new byte[] { 3, 1, 2, 3, 1, 1 }, new byte[] { 3, 3, 2, 1, 1, 1 }, new byte[] { 3, 1
            , 4, 1, 1, 1 }, new byte[] { 2, 2, 1, 4, 1, 1 }, new byte[] { 4, 3, 1, 1, 1, 1 }, new byte[] { 1, 1, 1
            , 2, 2, 4 }, new byte[] { 1, 1, 1, 4, 2, 2 }, new byte[] { 1, 2, 1, 1, 2, 4 }, new byte[] { 1, 2, 1, 4
            , 2, 1 }, new byte[] { 1, 4, 1, 1, 2, 2 }, new byte[] { 1, 4, 1, 2, 2, 1 }, new byte[] { 1, 1, 2, 2, 1
            , 4 }, new byte[] { 1, 1, 2, 4, 1, 2 }, new byte[] { 1, 2, 2, 1, 1, 4 }, new byte[] { 1, 2, 2, 4, 1, 1
             }, new byte[] { 1, 4, 2, 1, 1, 2 }, new byte[] { 1, 4, 2, 2, 1, 1 }, new byte[] { 2, 4, 1, 2, 1, 1 }, 
            new byte[] { 2, 2, 1, 1, 1, 4 }, new byte[] { 4, 1, 3, 1, 1, 1 }, new byte[] { 2, 4, 1, 1, 1, 2 }, new 
            byte[] { 1, 3, 4, 1, 1, 1 }, new byte[] { 1, 1, 1, 2, 4, 2 }, new byte[] { 1, 2, 1, 1, 4, 2 }, new byte
            [] { 1, 2, 1, 2, 4, 1 }, new byte[] { 1, 1, 4, 2, 1, 2 }, new byte[] { 1, 2, 4, 1, 1, 2 }, new byte[] 
            { 1, 2, 4, 2, 1, 1 }, new byte[] { 4, 1, 1, 2, 1, 2 }, new byte[] { 4, 2, 1, 1, 1, 2 }, new byte[] { 4
            , 2, 1, 2, 1, 1 }, new byte[] { 2, 1, 2, 1, 4, 1 }, new byte[] { 2, 1, 4, 1, 2, 1 }, new byte[] { 4, 1
            , 2, 1, 2, 1 }, new byte[] { 1, 1, 1, 1, 4, 3 }, new byte[] { 1, 1, 1, 3, 4, 1 }, new byte[] { 1, 3, 1
            , 1, 4, 1 }, new byte[] { 1, 1, 4, 1, 1, 3 }, new byte[] { 1, 1, 4, 3, 1, 1 }, new byte[] { 4, 1, 1, 1
            , 1, 3 }, new byte[] { 4, 1, 1, 3, 1, 1 }, new byte[] { 1, 1, 3, 1, 4, 1 }, new byte[] { 1, 1, 4, 1, 3
            , 1 }, new byte[] { 3, 1, 1, 1, 4, 1 }, new byte[] { 4, 1, 1, 1, 3, 1 }, new byte[] { 2, 1, 1, 4, 1, 2
             }, new byte[] { 2, 1, 1, 2, 1, 4 }, new byte[] { 2, 1, 1, 2, 3, 2 } };

        /// <summary>The stop bars.</summary>
        private static readonly byte[] BARS_STOP = new byte[] { 2, 3, 3, 1, 1, 1, 2 };

        /// <summary>The charset code change.</summary>
        public const char CODE_AB_TO_C = (char)99;

        /// <summary>The charset code change.</summary>
        public const char CODE_AC_TO_B = (char)100;

        /// <summary>The charset code change.</summary>
        public const char CODE_BC_TO_A = (char)101;

        /// <summary>The code for UCC/EAN-128.</summary>
        public const char FNC1_INDEX = (char)102;

        /// <summary>The start code.</summary>
        public const char START_A = (char)103;

        /// <summary>The start code.</summary>
        public const char START_B = (char)104;

        /// <summary>The start code.</summary>
        public const char START_C = (char)105;

        public const char FNC1 = '\u00ca';

        public const char DEL = '\u00c3';

        public const char FNC3 = '\u00c4';

        public const char FNC2 = '\u00c5';

        public const char SHIFT = '\u00c6';

        public const char CODE_C = '\u00c7';

        public const char CODE_A = '\u00c8';

        public const char FNC4 = '\u00c8';

        public const char STARTA = '\u00cb';

        public const char STARTB = '\u00cc';

        public const char STARTC = '\u00cd';

        private static IDictionary<int, int?> ais = new Dictionary<int, int?>();

        /// <summary>Creates new Barcode128.</summary>
        /// <remarks>
        /// Creates new Barcode128.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="Barcode128(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public Barcode128(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates new Barcode128, which will use the provided font</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public Barcode128(PdfDocument document, PdfFont font)
            : base(document) {
            this.x = 0.8f;
            this.font = font;
            this.size = 8;
            this.baseline = size;
            this.barHeight = size * 3;
            this.textAlignment = ALIGN_CENTER;
            this.codeType = CODE128;
        }

        public enum Barcode128CodeSet {
            A,
            B,
            C,
            AUTO
        }

        public virtual void SetCodeSet(Barcode128.Barcode128CodeSet codeSet) {
            this.codeSet = codeSet;
        }

        public virtual Barcode128.Barcode128CodeSet GetCodeSet() {
            return this.codeSet;
        }

        private Barcode128.Barcode128CodeSet codeSet = Barcode128.Barcode128CodeSet.AUTO;

        /// <summary>Removes the FNC1 codes in the text.</summary>
        /// <param name="code">The text to clean</param>
        /// <returns>The cleaned text</returns>
        public static String RemoveFNC1(String code) {
            int len = code.Length;
            StringBuilder buf = new StringBuilder(len);
            for (int k = 0; k < len; ++k) {
                char c = code[k];
                if (c >= 32 && c <= 126) {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }

        /// <summary>Gets the human readable text of a sequence of AI.</summary>
        /// <param name="code">the text</param>
        /// <returns>the human readable text</returns>
        public static String GetHumanReadableUCCEAN(String code) {
            StringBuilder buf = new StringBuilder();
            String fnc1 = new String(new char[] { FNC1 });
            while (true) {
                if (code.StartsWith(fnc1)) {
                    code = code.Substring(1);
                    continue;
                }
                int n = 0;
                int idlen = 0;
                for (int k = 2; k < 5; ++k) {
                    if (code.Length < k) {
                        break;
                    }
                    int subcode = Convert.ToInt32(code.JSubstring(0, k), System.Globalization.CultureInfo.InvariantCulture);
                    n = ais.ContainsKey(subcode) ? (int)ais.Get(subcode) : 0;
                    if (n != 0) {
                        idlen = k;
                        break;
                    }
                }
                if (idlen == 0) {
                    break;
                }
                buf.Append('(').Append(code.JSubstring(0, idlen)).Append(')');
                code = code.Substring(idlen);
                if (n > 0) {
                    n -= idlen;
                    if (code.Length <= n) {
                        break;
                    }
                    buf.Append(RemoveFNC1(code.JSubstring(0, n)));
                    code = code.Substring(n);
                }
                else {
                    int idx = code.IndexOf(FNC1);
                    if (idx < 0) {
                        break;
                    }
                    buf.Append(code.JSubstring(0, idx));
                    code = code.Substring(idx + 1);
                }
            }
            buf.Append(RemoveFNC1(code));
            return buf.ToString();
        }

        /// <summary>
        /// Converts the human readable text to the characters needed to
        /// create a barcode using the specified code set.
        /// </summary>
        /// <param name="text">the text to convert</param>
        /// <param name="ucc">
        /// <c>true</c> if it is an UCC/EAN-128. In this case
        /// the character FNC1 is added
        /// </param>
        /// <param name="codeSet">forced code set, or AUTO for optimized barcode.</param>
        /// <returns>the code ready to be fed to getBarsCode128Raw()</returns>
        public static String GetRawText(String text, bool ucc, Barcode128.Barcode128CodeSet codeSet) {
            String @out = "";
            int tLen = text.Length;
            if (tLen == 0) {
                @out += GetStartSymbol(codeSet);
                if (ucc) {
                    @out += FNC1_INDEX;
                }
                return @out;
            }
            int c;
            for (int k = 0; k < tLen; ++k) {
                c = text[k];
                if (c > 127 && c != FNC1) {
                    throw new PdfException(BarcodeExceptionMessageConstant.THERE_ARE_ILLEGAL_CHARACTERS_FOR_BARCODE_128);
                }
            }
            c = text[0];
            char currentCode = GetStartSymbol(codeSet);
            int index = 0;
            if ((codeSet == Barcode128.Barcode128CodeSet.AUTO || codeSet == Barcode128.Barcode128CodeSet.C) && IsNextDigits
                (text, index, 2)) {
                currentCode = START_C;
                @out += currentCode;
                if (ucc) {
                    @out += FNC1_INDEX;
                }
                String out2 = GetPackedRawDigits(text, index, 2);
                index += out2[0];
                @out += out2.Substring(1);
            }
            else {
                if (c < ' ') {
                    currentCode = START_A;
                    @out += currentCode;
                    if (ucc) {
                        @out += FNC1_INDEX;
                    }
                    @out += (char)(c + 64);
                    ++index;
                }
                else {
                    @out += currentCode;
                    if (ucc) {
                        @out += FNC1_INDEX;
                    }
                    if (c == FNC1) {
                        @out += FNC1_INDEX;
                    }
                    else {
                        @out += (char)(c - ' ');
                    }
                    ++index;
                }
            }
            if (codeSet != Barcode128.Barcode128CodeSet.AUTO && currentCode != GetStartSymbol(codeSet)) {
                throw new PdfException(BarcodeExceptionMessageConstant.THERE_ARE_ILLEGAL_CHARACTERS_FOR_BARCODE_128);
            }
            while (index < tLen) {
                switch (currentCode) {
                    case START_A: {
                        if (codeSet == Barcode128.Barcode128CodeSet.AUTO && IsNextDigits(text, index, 4)) {
                            currentCode = START_C;
                            @out += CODE_AB_TO_C;
                            String out2 = GetPackedRawDigits(text, index, 4);
                            index += out2[0];
                            @out += out2.Substring(1);
                        }
                        else {
                            c = text[index++];
                            if (c == FNC1) {
                                @out += FNC1_INDEX;
                            }
                            else {
                                if (c > '_') {
                                    currentCode = START_B;
                                    @out += CODE_AC_TO_B;
                                    @out += (char)(c - ' ');
                                }
                                else {
                                    if (c < ' ') {
                                        @out += (char)(c + 64);
                                    }
                                    else {
                                        @out += (char)(c - ' ');
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case START_B: {
                        if (codeSet == Barcode128.Barcode128CodeSet.AUTO && IsNextDigits(text, index, 4)) {
                            currentCode = START_C;
                            @out += CODE_AB_TO_C;
                            String out2 = GetPackedRawDigits(text, index, 4);
                            index += out2[0];
                            @out += out2.Substring(1);
                        }
                        else {
                            c = text[index++];
                            if (c == FNC1) {
                                @out += FNC1_INDEX;
                            }
                            else {
                                if (c < ' ') {
                                    currentCode = START_A;
                                    @out += CODE_BC_TO_A;
                                    @out += (char)(c + 64);
                                }
                                else {
                                    @out += (char)(c - ' ');
                                }
                            }
                        }
                        break;
                    }

                    case START_C: {
                        if (IsNextDigits(text, index, 2)) {
                            String out2 = GetPackedRawDigits(text, index, 2);
                            index += out2[0];
                            @out += out2.Substring(1);
                        }
                        else {
                            c = text[index++];
                            if (c == FNC1) {
                                @out += FNC1_INDEX;
                            }
                            else {
                                if (c < ' ') {
                                    currentCode = START_A;
                                    @out += CODE_BC_TO_A;
                                    @out += (char)(c + 64);
                                }
                                else {
                                    currentCode = START_B;
                                    @out += CODE_AC_TO_B;
                                    @out += (char)(c - ' ');
                                }
                            }
                        }
                        break;
                    }
                }
                if (codeSet != Barcode128.Barcode128CodeSet.AUTO && currentCode != GetStartSymbol(codeSet)) {
                    throw new PdfException(BarcodeExceptionMessageConstant.THERE_ARE_ILLEGAL_CHARACTERS_FOR_BARCODE_128);
                }
            }
            return @out;
        }

        /// <summary>
        /// Converts the human readable text to the characters needed to
        /// create a barcode.
        /// </summary>
        /// <remarks>
        /// Converts the human readable text to the characters needed to
        /// create a barcode. Some optimization is done to get the shortest code.
        /// </remarks>
        /// <param name="text">the text to convert</param>
        /// <param name="ucc">
        /// <c>true</c> if it is an UCC/EAN-128. In this case
        /// the character FNC1 is added
        /// </param>
        /// <returns>the code ready to be fed to getBarsCode128Raw()</returns>
        public static String GetRawText(String text, bool ucc) {
            return GetRawText(text, ucc, Barcode128.Barcode128CodeSet.AUTO);
        }

        /// <summary>Generates the bars.</summary>
        /// <remarks>
        /// Generates the bars. The input has the actual barcodes, not
        /// the human readable text.
        /// </remarks>
        /// <param name="text">the barcode</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsCode128Raw(String text) {
            int idx = text.IndexOf('\uffff');
            if (idx >= 0) {
                text = text.JSubstring(0, idx);
            }
            int chk = text[0];
            for (int k = 1; k < text.Length; ++k) {
                chk += k * text[k];
            }
            chk = chk % 103;
            text += (char)chk;
            byte[] bars = new byte[(text.Length + 1) * 6 + 7];
            int k_1;
            for (k_1 = 0; k_1 < text.Length; ++k_1) {
                Array.Copy(BARS[text[k_1]], 0, bars, k_1 * 6, 6);
            }
            Array.Copy(BARS_STOP, 0, bars, k_1 * 6, 7);
            return bars;
        }

        /// <summary>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy.
        /// </summary>
        /// <remarks>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy. The lower left corner is always (0, 0).
        /// </remarks>
        /// <returns>the size the barcode occupies.</returns>
        public override Rectangle GetBarcodeSize() {
            float fontX = 0;
            float fontY = 0;
            String fullCode;
            if (font != null) {
                if (baseline > 0) {
                    fontY = baseline - GetDescender();
                }
                else {
                    fontY = -baseline + size;
                }
                if (codeType == CODE128_RAW) {
                    int idx = code.IndexOf('\uffff');
                    if (idx < 0) {
                        fullCode = "";
                    }
                    else {
                        fullCode = code.Substring(idx + 1);
                    }
                }
                else {
                    if (codeType == CODE128_UCC) {
                        fullCode = GetHumanReadableUCCEAN(code);
                    }
                    else {
                        fullCode = RemoveFNC1(code);
                    }
                }
                fontX = font.GetWidth(altText != null ? altText : fullCode, size);
            }
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx >= 0) {
                    fullCode = code.JSubstring(0, idx);
                }
                else {
                    fullCode = code;
                }
            }
            else {
                fullCode = GetRawText(code, codeType == CODE128_UCC, codeSet);
            }
            int len = fullCode.Length;
            float fullWidth = (len + 2) * 11 * x + 2 * x;
            fullWidth = Math.Max(fullWidth, fontX);
            float fullHeight = barHeight + fontY;
            return new Rectangle(fullWidth, fullHeight);
        }

        /// <summary>Places the barcode in a <c>PdfCanvas</c>.</summary>
        /// <remarks>
        /// Places the barcode in a <c>PdfCanvas</c>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.<para />
        /// The bars and text are written in the following colors:
        /// <br />
        /// <table border="1" summary="barcode properties">
        /// <tr>
        /// <th><c>barColor</c></th>
        /// <th><c>textColor</c></th>
        /// <th>Result</th>
        /// </tr>
        /// <tr>
        /// <td><c>null</c></td>
        /// <td><c>null</c></td>
        /// <td>bars and text painted with current fill color</td>
        /// </tr>
        /// <tr>
        /// <td><c>barColor</c></td>
        /// <td><c>null</c></td>
        /// <td>bars and text painted with <c>barColor</c></td>
        /// </tr>
        /// <tr>
        /// <td><c>null</c></td>
        /// <td><c>textColor</c></td>
        /// <td>bars painted with current color<br />text painted with <c>textColor</c></td>
        /// </tr>
        /// <tr>
        /// <td><c>barColor</c></td>
        /// <td><c>textColor</c></td>
        /// <td>bars painted with <c>barColor</c><br />text painted with <c>textColor</c></td>
        /// </tr>
        /// </table>
        /// </remarks>
        /// <param name="canvas">the <c>PdfCanvas</c> where the barcode will be placed</param>
        /// <param name="barColor">the color of the bars. It can be <c>null</c></param>
        /// <param name="textColor">the color of the text. It can be <c>null</c></param>
        /// <returns>the dimensions the barcode occupies</returns>
        public override Rectangle PlaceBarcode(PdfCanvas canvas, Color barColor, Color textColor) {
            String fullCode;
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx < 0) {
                    fullCode = "";
                }
                else {
                    fullCode = code.Substring(idx + 1);
                }
            }
            else {
                if (codeType == CODE128_UCC) {
                    fullCode = GetHumanReadableUCCEAN(code);
                }
                else {
                    fullCode = RemoveFNC1(code);
                }
            }
            float fontX = 0;
            if (font != null) {
                fontX = font.GetWidth(fullCode = altText != null ? altText : fullCode, size);
            }
            String bCode;
            if (codeType == CODE128_RAW) {
                int idx = code.IndexOf('\uffff');
                if (idx >= 0) {
                    bCode = code.JSubstring(0, idx);
                }
                else {
                    bCode = code;
                }
            }
            else {
                bCode = GetRawText(code, codeType == CODE128_UCC, codeSet);
            }
            int len = bCode.Length;
            float fullWidth = (len + 2) * 11 * x + 2 * x;
            float barStartX = 0;
            float textStartX = 0;
            switch (textAlignment) {
                case ALIGN_LEFT: {
                    break;
                }

                case ALIGN_RIGHT: {
                    if (fontX > fullWidth) {
                        barStartX = fontX - fullWidth;
                    }
                    else {
                        textStartX = fullWidth - fontX;
                    }
                    break;
                }

                default: {
                    if (fontX > fullWidth) {
                        barStartX = (fontX - fullWidth) / 2;
                    }
                    else {
                        textStartX = (fullWidth - fontX) / 2;
                    }
                    break;
                }
            }
            float barStartY = 0;
            float textStartY = 0;
            if (font != null) {
                if (baseline <= 0) {
                    textStartY = barHeight - baseline;
                }
                else {
                    textStartY = -GetDescender();
                    barStartY = textStartY + baseline;
                }
            }
            byte[] bars = GetBarsCode128Raw(bCode);
            bool print = true;
            if (barColor != null) {
                canvas.SetFillColor(barColor);
            }
            for (int k = 0; k < bars.Length; ++k) {
                float w = bars[k] * x;
                if (print) {
                    canvas.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                }
                print = !print;
                barStartX += w;
            }
            canvas.Fill();
            if (font != null) {
                if (textColor != null) {
                    canvas.SetFillColor(textColor);
                }
                canvas.BeginText();
                canvas.SetFontAndSize(font, size);
                canvas.SetTextMatrix(textStartX, textStartY);
                canvas.ShowText(fullCode);
                canvas.EndText();
            }
            return GetBarcodeSize();
        }

        /// <summary>Sets the code to generate.</summary>
        /// <remarks>
        /// Sets the code to generate. If it's an UCC code and starts with '(' it will
        /// be split by the AI. This code in UCC mode is valid:
        /// <br />
        /// <c>(01)00000090311314(10)ABC123(15)060916</c>
        /// </remarks>
        /// <param name="code">the code to generate</param>
        public override void SetCode(String code) {
            if (GetCodeType() == Barcode128.CODE128_UCC && code.StartsWith("(")) {
                int idx = 0;
                StringBuilder ret = new StringBuilder("");
                while (idx >= 0) {
                    int end = code.IndexOf(')', idx);
                    if (end < 0) {
                        throw new ArgumentException("Badly formed ucc string");
                    }
                    String sai = code.JSubstring(idx + 1, end);
                    if (sai.Length < 2) {
                        throw new ArgumentException("AI is too short");
                    }
                    int ai = Convert.ToInt32(sai, System.Globalization.CultureInfo.InvariantCulture);
                    int len = (int)ais.Get(ai);
                    if (len == 0) {
                        throw new ArgumentException("AI not found");
                    }
                    sai = JavaUtil.IntegerToString(Convert.ToInt32(ai));
                    if (sai.Length == 1) {
                        sai = "0" + sai;
                    }
                    idx = code.IndexOf('(', end);
                    int next = (idx < 0 ? code.Length : idx);
                    ret.Append(sai).Append(code.JSubstring(end + 1, next));
                    if (len < 0) {
                        if (idx >= 0) {
                            ret.Append(FNC1);
                        }
                    }
                    else {
                        if (next - end - 1 + sai.Length != len) {
                            throw new ArgumentException("Invalid AI length");
                        }
                    }
                }
                base.SetCode(ret.ToString());
            }
            else {
                base.SetCode(code);
            }
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        private static char GetStartSymbol(Barcode128.Barcode128CodeSet codeSet) {
            switch (codeSet) {
                case Barcode128.Barcode128CodeSet.A: {
                    return START_A;
                }

                case Barcode128.Barcode128CodeSet.B: {
                    return START_B;
                }

                case Barcode128.Barcode128CodeSet.C: {
                    return START_C;
                }

                default: {
                    return START_B;
                }
            }
        }

        static Barcode128() {
            ais.Put(0, 20);
            ais.Put(1, 16);
            ais.Put(2, 16);
            ais.Put(10, -1);
            ais.Put(11, 9);
            ais.Put(12, 8);
            ais.Put(13, 8);
            ais.Put(15, 8);
            ais.Put(17, 8);
            ais.Put(20, 4);
            ais.Put(21, -1);
            ais.Put(22, -1);
            ais.Put(23, -1);
            ais.Put(240, -1);
            ais.Put(241, -1);
            ais.Put(250, -1);
            ais.Put(251, -1);
            ais.Put(252, -1);
            ais.Put(30, -1);
            for (int k = 3100; k < 3700; ++k) {
                ais.Put(k, 10);
            }
            ais.Put(37, -1);
            for (int k = 3900; k < 3940; ++k) {
                ais.Put(k, -1);
            }
            ais.Put(400, -1);
            ais.Put(401, -1);
            ais.Put(402, 20);
            ais.Put(403, -1);
            for (int k = 410; k < 416; ++k) {
                ais.Put(k, 16);
            }
            ais.Put(420, -1);
            ais.Put(421, -1);
            ais.Put(422, 6);
            ais.Put(423, -1);
            ais.Put(424, 6);
            ais.Put(425, 6);
            ais.Put(426, 6);
            ais.Put(7001, 17);
            ais.Put(7002, -1);
            for (int k = 7030; k < 7040; ++k) {
                ais.Put(k, -1);
            }
            ais.Put(8001, 18);
            ais.Put(8002, -1);
            ais.Put(8003, -1);
            ais.Put(8004, -1);
            ais.Put(8005, 10);
            ais.Put(8006, 22);
            ais.Put(8007, -1);
            ais.Put(8008, -1);
            ais.Put(8018, 22);
            ais.Put(8020, -1);
            ais.Put(8100, 10);
            ais.Put(8101, 14);
            ais.Put(8102, 6);
            for (int k = 90; k < 100; ++k) {
                ais.Put(k, -1);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the next <c>numDigits</c>
        /// starting from index <c>textIndex</c> are numeric skipping any FNC1.
        /// </summary>
        /// <param name="text">the text to check</param>
        /// <param name="textIndex">where to check from</param>
        /// <param name="numDigits">the number of digits to check</param>
        /// <returns>the check result</returns>
        internal static bool IsNextDigits(String text, int textIndex, int numDigits) {
            int len = text.Length;
            while (textIndex < len && numDigits > 0) {
                if (text[textIndex] == FNC1) {
                    ++textIndex;
                    continue;
                }
                int n = Math.Min(2, numDigits);
                if (textIndex + n > len) {
                    return false;
                }
                while (n-- > 0) {
                    char c = text[textIndex++];
                    if (c < '0' || c > '9') {
                        return false;
                    }
                    --numDigits;
                }
            }
            return numDigits == 0;
        }

        /// <summary>Packs the digits for charset C also considering FNC1.</summary>
        /// <remarks>
        /// Packs the digits for charset C also considering FNC1. It assumes that all the parameters
        /// are valid.
        /// </remarks>
        /// <param name="text">the text to pack</param>
        /// <param name="textIndex">where to pack from</param>
        /// <param name="numDigits">the number of digits to pack. It is always an even number</param>
        /// <returns>the packed digits, two digits per character</returns>
        internal static String GetPackedRawDigits(String text, int textIndex, int numDigits) {
            StringBuilder @out = new StringBuilder("");
            int start = textIndex;
            while (numDigits > 0) {
                if (text[textIndex] == FNC1) {
                    @out.Append(FNC1_INDEX);
                    ++textIndex;
                    continue;
                }
                numDigits -= 2;
                int c1 = text[textIndex++] - '0';
                int c2 = text[textIndex++] - '0';
                @out.Append((char)(c1 * 10 + c2));
            }
            return (char)(textIndex - start) + @out.ToString();
        }
    }
}
