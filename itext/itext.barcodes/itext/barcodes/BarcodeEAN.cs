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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>The implementation of the barcode EAN.</summary>
    /// <remarks>
    /// The implementation of the barcode EAN.
    /// <para />
    /// The International Article Number (also known as European Article Number or EAN) is a standard describing a barcode
    /// symbology and numbering system used in global trade to identify a specific retail product type, in a specific
    /// packaging configuration, from a specific manufacturer
    /// </remarks>
    public class BarcodeEAN : Barcode1D {
        /// <summary>A type of barcode</summary>
        public const int EAN13 = 1;

        /// <summary>A type of barcode</summary>
        public const int EAN8 = 2;

        /// <summary>A type of barcode</summary>
        public const int UPCA = 3;

        /// <summary>A type of barcode</summary>
        public const int UPCE = 4;

        /// <summary>A type of barcode</summary>
        public const int SUPP2 = 5;

        /// <summary>A type of barcode</summary>
        public const int SUPP5 = 6;

        /// <summary>The bar positions that are guard bars.</summary>
        private static readonly int[] GUARD_EMPTY = new int[] {  };

        /// <summary>The bar positions that are guard bars.</summary>
        private static readonly int[] GUARD_UPCA = new int[] { 0, 2, 4, 6, 28, 30, 52, 54, 56, 58 };

        /// <summary>The bar positions that are guard bars.</summary>
        private static readonly int[] GUARD_EAN13 = new int[] { 0, 2, 28, 30, 56, 58 };

        /// <summary>The bar positions that are guard bars.</summary>
        private static readonly int[] GUARD_EAN8 = new int[] { 0, 2, 20, 22, 40, 42 };

        /// <summary>The bar positions that are guard bars.</summary>
        private static readonly int[] GUARD_UPCE = new int[] { 0, 2, 28, 30, 32 };

        /// <summary>The x coordinates to place the text.</summary>
        private static readonly float[] TEXTPOS_EAN13 = new float[] { 6.5f, 13.5f, 20.5f, 27.5f, 34.5f, 41.5f, 53.5f
            , 60.5f, 67.5f, 74.5f, 81.5f, 88.5f };

        /// <summary>The x coordinates to place the text.</summary>
        private static readonly float[] TEXTPOS_EAN8 = new float[] { 6.5f, 13.5f, 20.5f, 27.5f, 39.5f, 46.5f, 53.5f
            , 60.5f };

        /// <summary>The basic bar widths.</summary>
        private static readonly byte[][] BARS = new byte[][] { 
                // 0
                new byte[] { 3, 2, 1, 1 }, 
                // 1
                new byte[] { 2, 2, 2, 1 }, 
                // 2
                new byte[] { 2, 1, 2, 2 }, 
                // 3
                new byte[] { 1, 4, 1, 1 }, 
                // 4
                new byte[] { 1, 1, 3, 2 }, 
                // 5
                new byte[] { 1, 2, 3, 1 }, 
                // 6
                new byte[] { 1, 1, 1, 4 }, 
                // 7
                new byte[] { 1, 3, 1, 2 }, 
                // 8
                new byte[] { 1, 2, 1, 3 }, 
                // 9
                new byte[] { 3, 1, 1, 2 } };

        /// <summary>The total number of bars for EAN13.</summary>
        private const int TOTALBARS_EAN13 = 11 + 12 * 4;

        /// <summary>The total number of bars for EAN8.</summary>
        private const int TOTALBARS_EAN8 = 11 + 8 * 4;

        /// <summary>The total number of bars for UPCE.</summary>
        private const int TOTALBARS_UPCE = 9 + 6 * 4;

        /// <summary>The total number of bars for supplemental 2.</summary>
        private const int TOTALBARS_SUPP2 = 13;

        /// <summary>The total number of bars for supplemental 5.</summary>
        private const int TOTALBARS_SUPP5 = 31;

        /// <summary>Marker for odd parity.</summary>
        private const int ODD = 0;

        /// <summary>Marker for even parity.</summary>
        private const int EVEN = 1;

        /// <summary>Sequence of parities to be used with EAN13.</summary>
        private static readonly byte[][] PARITY13 = new byte[][] { 
                // 0
                new byte[] { ODD, ODD, ODD, ODD, ODD, ODD }, 
                // 1
                new byte[] { ODD, ODD, EVEN, ODD, EVEN, EVEN }, 
                // 2
                new byte[] { ODD, ODD, EVEN, EVEN, ODD, EVEN }, 
                // 3
                new byte[] { ODD, ODD, EVEN, EVEN, EVEN, ODD }, 
                // 4
                new byte[] { ODD, EVEN, ODD, ODD, EVEN, EVEN }, 
                // 5
                new byte[] { ODD, EVEN, EVEN, ODD, ODD, EVEN }, 
                // 6
                new byte[] { ODD, EVEN, EVEN, EVEN, ODD, ODD }, 
                // 7
                new byte[] { ODD, EVEN, ODD, EVEN, ODD, EVEN }, 
                // 8
                new byte[] { ODD, EVEN, ODD, EVEN, EVEN, ODD }, 
                // 9
                new byte[] { ODD, EVEN, EVEN, ODD, EVEN, ODD } };

        /// <summary>Sequence of parities to be used with supplemental 2.</summary>
        private static readonly byte[][] PARITY2 = new byte[][] { 
                // 0
                new byte[] { ODD, ODD }, 
                // 1
                new byte[] { ODD, EVEN }, 
                // 2
                new byte[] { EVEN, ODD }, 
                // 3
                new byte[] { EVEN, EVEN } };

        /// <summary>Sequence of parities to be used with supplemental 2.</summary>
        private static readonly byte[][] PARITY5 = new byte[][] { 
                // 0
                new byte[] { EVEN, EVEN, ODD, ODD, ODD }, 
                // 1
                new byte[] { EVEN, ODD, EVEN, ODD, ODD }, 
                // 2
                new byte[] { EVEN, ODD, ODD, EVEN, ODD }, 
                // 3
                new byte[] { EVEN, ODD, ODD, ODD, EVEN }, 
                // 4
                new byte[] { ODD, EVEN, EVEN, ODD, ODD }, 
                // 5
                new byte[] { ODD, ODD, EVEN, EVEN, ODD }, 
                // 6
                new byte[] { ODD, ODD, ODD, EVEN, EVEN }, 
                // 7
                new byte[] { ODD, EVEN, ODD, EVEN, ODD }, 
                // 8
                new byte[] { ODD, EVEN, ODD, ODD, EVEN }, 
                // 9
                new byte[] { ODD, ODD, EVEN, ODD, EVEN } };

        /// <summary>Sequence of parities to be used with UPCE.</summary>
        private static readonly byte[][] PARITYE = new byte[][] { 
                // 0
                new byte[] { EVEN, EVEN, EVEN, ODD, ODD, ODD }, 
                // 1
                new byte[] { EVEN, EVEN, ODD, EVEN, ODD, ODD }, 
                // 2
                new byte[] { EVEN, EVEN, ODD, ODD, EVEN, ODD }, 
                // 3
                new byte[] { EVEN, EVEN, ODD, ODD, ODD, EVEN }, 
                // 4
                new byte[] { EVEN, ODD, EVEN, EVEN, ODD, ODD }, 
                // 5
                new byte[] { EVEN, ODD, ODD, EVEN, EVEN, ODD }, 
                // 6
                new byte[] { EVEN, ODD, ODD, ODD, EVEN, EVEN }, 
                // 7
                new byte[] { EVEN, ODD, EVEN, ODD, EVEN, ODD }, 
                // 8
                new byte[] { EVEN, ODD, EVEN, ODD, ODD, EVEN }, 
                // 9
                new byte[] { EVEN, ODD, ODD, EVEN, ODD, EVEN } };

        /// <summary>Creates new BarcodeEAN.</summary>
        /// <remarks>
        /// Creates new BarcodeEAN.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="BarcodeEAN(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public BarcodeEAN(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates new BarcodeEAN</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public BarcodeEAN(PdfDocument document, PdfFont font)
            : base(document) {
            this.x = 0.8f;
            this.font = font;
            this.size = 8;
            this.baseline = size;
            this.barHeight = size * 3;
            this.guardBars = true;
            this.codeType = EAN13;
            this.code = "";
        }

        /// <summary>Calculates the EAN parity character.</summary>
        /// <param name="code">the code</param>
        /// <returns>the parity character</returns>
        public static int CalculateEANParity(String code) {
            int mul = 3;
            int total = 0;
            for (int k = code.Length - 1; k >= 0; --k) {
                int n = code[k] - '0';
                total += mul * n;
                mul ^= 2;
            }
            return (10 - (total % 10)) % 10;
        }

        /// <summary>Converts an UPCA code into an UPCE code.</summary>
        /// <remarks>
        /// Converts an UPCA code into an UPCE code. If the code can not
        /// be converted a <c>null</c> is returned.
        /// </remarks>
        /// <param name="text">the code to convert. It must have 12 numeric characters</param>
        /// <returns>
        /// the 8 converted digits or <c>null</c> if the
        /// code could not be converted
        /// </returns>
        public static String ConvertUPCAtoUPCE(String text) {
            if (text.Length != 12 || !(text.StartsWith("0") || text.StartsWith("1"))) {
                return null;
            }
            if (text.JSubstring(3, 6).Equals("000") || text.JSubstring(3, 6).Equals("100") || text.JSubstring(3, 6).Equals
                ("200")) {
                if (text.JSubstring(6, 8).Equals("00")) {
                    return text.JSubstring(0, 1) + text.JSubstring(1, 3) + text.JSubstring(8, 11) + text.JSubstring(3, 4) + text
                        .Substring(11);
                }
            }
            else {
                if (text.JSubstring(4, 6).Equals("00")) {
                    if (text.JSubstring(6, 9).Equals("000")) {
                        return text.JSubstring(0, 1) + text.JSubstring(1, 4) + text.JSubstring(9, 11) + "3" + text.Substring(11);
                    }
                }
                else {
                    if (text.JSubstring(5, 6).Equals("0")) {
                        if (text.JSubstring(6, 10).Equals("0000")) {
                            return text.JSubstring(0, 1) + text.JSubstring(1, 5) + text.JSubstring(10, 11) + "4" + text.Substring(11);
                        }
                    }
                    else {
                        if (text[10] >= '5') {
                            if (text.JSubstring(6, 10).Equals("0000")) {
                                return text.JSubstring(0, 1) + text.JSubstring(1, 6) + text.JSubstring(10, 11) + text.Substring(11);
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Creates the bars for the barcode EAN13 and UPCA.</summary>
        /// <param name="_code">the text with 13 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsEAN13(String _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k) {
                code[k] = _code[k] - '0';
            }
            byte[] bars = new byte[TOTALBARS_EAN13];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = PARITY13[code[0]];
            for (int k = 0; k < sequence.Length; ++k) {
                int c = code[k + 1];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 7; k < 13; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /// <summary>Creates the bars for the barcode EAN8.</summary>
        /// <param name="_code">the text with 8 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsEAN8(String _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k) {
                code[k] = _code[k] - '0';
            }
            byte[] bars = new byte[TOTALBARS_EAN8];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 0; k < 4; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 4; k < 8; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /// <summary>Creates the bars for the barcode UPCE.</summary>
        /// <param name="_code">the text with 8 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsUPCE(String _code) {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k) {
                code[k] = _code[k] - '0';
            }
            byte[] bars = new byte[TOTALBARS_UPCE];
            bool flip = (code[0] != 0);
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = PARITYE[code[code.Length - 1]];
            for (int k = 1; k < code.Length - 1; ++k) {
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k - 1] == (flip ? EVEN : ODD)) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /// <summary>Creates the bars for the barcode supplemental 2.</summary>
        /// <param name="_code">the text with 2 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsSupplemental2(String _code) {
            int[] code = new int[2];
            for (int k = 0; k < code.Length; ++k) {
                code[k] = _code[k] - '0';
            }
            byte[] bars = new byte[TOTALBARS_SUPP2];
            int pb = 0;
            int parity = (code[0] * 10 + code[1]) % 4;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = PARITY2[parity];
            for (int k = 0; k < sequence.Length; ++k) {
                if (k == 1) {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            return bars;
        }

        /// <summary>Creates the bars for the barcode supplemental 5.</summary>
        /// <param name="_code">the text with 5 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsSupplemental5(String _code) {
            int[] code = new int[5];
            for (int k = 0; k < code.Length; ++k) {
                code[k] = _code[k] - '0';
            }
            byte[] bars = new byte[TOTALBARS_SUPP5];
            int pb = 0;
            int parity = (((code[0] + code[2] + code[4]) * 3) + ((code[1] + code[3]) * 9)) % 10;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = PARITY5[parity];
            for (int k = 0; k < sequence.Length; ++k) {
                if (k != 0) {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = BARS[c];
                if (sequence[k] == ODD) {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
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
            float width;
            float height = barHeight;
            if (font != null) {
                if (baseline <= 0) {
                    height += -baseline + size;
                }
                else {
                    height += baseline - GetDescender();
                }
            }
            switch (codeType) {
                case EAN13: {
                    width = x * (11 + 12 * 7);
                    if (font != null) {
                        width += font.GetWidth(code[0], size);
                    }
                    break;
                }

                case EAN8: {
                    width = x * (11 + 8 * 7);
                    break;
                }

                case UPCA: {
                    width = x * (11 + 12 * 7);
                    if (font != null) {
                        width += font.GetWidth(code[0], size) + font.GetWidth(code[11], size);
                    }
                    break;
                }

                case UPCE: {
                    width = x * (9 + 6 * 7);
                    if (font != null) {
                        width += font.GetWidth(code[0], size) + font.GetWidth(code[7], size);
                    }
                    break;
                }

                case SUPP2: {
                    width = x * (6 + 2 * 7);
                    break;
                }

                case SUPP5: {
                    width = x * (4 + 5 * 7 + 4 * 2);
                    break;
                }

                default: {
                    throw new PdfException("Invalid code type");
                }
            }
            return new Rectangle(width, height);
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
            Rectangle rect = GetBarcodeSize();
            float barStartX = 0;
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
            switch (codeType) {
                case EAN13:
                case UPCA:
                case UPCE: {
                    if (font != null) {
                        barStartX += font.GetWidth(code[0], size);
                    }
                    break;
                }
            }
            byte[] bars;
            int[] guard = GUARD_EMPTY;
            switch (codeType) {
                case EAN13: {
                    bars = GetBarsEAN13(code);
                    guard = GUARD_EAN13;
                    break;
                }

                case EAN8: {
                    bars = GetBarsEAN8(code);
                    guard = GUARD_EAN8;
                    break;
                }

                case UPCA: {
                    bars = GetBarsEAN13("0" + code);
                    guard = GUARD_UPCA;
                    break;
                }

                case UPCE: {
                    bars = GetBarsUPCE(code);
                    guard = GUARD_UPCE;
                    break;
                }

                case SUPP2: {
                    bars = GetBarsSupplemental2(code);
                    break;
                }

                case SUPP5: {
                    bars = GetBarsSupplemental5(code);
                    break;
                }

                default: {
                    throw new PdfException("Invalid code type");
                }
            }
            float keepBarX = barStartX;
            bool print = true;
            float gd = 0;
            if (font != null && baseline > 0 && guardBars) {
                gd = baseline / 2;
            }
            if (barColor != null) {
                canvas.SetFillColor(barColor);
            }
            for (int k = 0; k < bars.Length; ++k) {
                float w = bars[k] * x;
                if (print) {
                    if (JavaUtil.ArraysBinarySearch(guard, k) >= 0) {
                        canvas.Rectangle(barStartX, barStartY - gd, w - inkSpreading, barHeight + gd);
                    }
                    else {
                        canvas.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                    }
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
                switch (codeType) {
                    case EAN13: {
                        canvas.SetTextMatrix(0, textStartY);
                        canvas.ShowText(code.JSubstring(0, 1));
                        for (int k = 1; k < 13; ++k) {
                            String c = code.JSubstring(k, k + 1);
                            float len = font.GetWidth(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k - 1] * x - len / 2;
                            canvas.SetTextMatrix(pX, textStartY);
                            canvas.ShowText(c);
                        }
                        break;
                    }

                    case EAN8: {
                        for (int k = 0; k < 8; ++k) {
                            String c = code.JSubstring(k, k + 1);
                            float len = font.GetWidth(c, size);
                            float pX = TEXTPOS_EAN8[k] * x - len / 2;
                            canvas.SetTextMatrix(pX, textStartY);
                            canvas.ShowText(c);
                        }
                        break;
                    }

                    case UPCA: {
                        canvas.SetTextMatrix(0, textStartY);
                        canvas.ShowText(code.JSubstring(0, 1));
                        for (int k = 1; k < 11; ++k) {
                            String c = code.JSubstring(k, k + 1);
                            float len = font.GetWidth(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k] * x - len / 2;
                            canvas.SetTextMatrix(pX, textStartY);
                            canvas.ShowText(c);
                        }
                        canvas.SetTextMatrix(keepBarX + x * (11 + 12 * 7), textStartY);
                        canvas.ShowText(code.JSubstring(11, 12));
                        break;
                    }

                    case UPCE: {
                        canvas.SetTextMatrix(0, textStartY);
                        canvas.ShowText(code.JSubstring(0, 1));
                        for (int k = 1; k < 7; ++k) {
                            String c = code.JSubstring(k, k + 1);
                            float len = font.GetWidth(c, size);
                            float pX = keepBarX + TEXTPOS_EAN13[k - 1] * x - len / 2;
                            canvas.SetTextMatrix(pX, textStartY);
                            canvas.ShowText(c);
                        }
                        canvas.SetTextMatrix(keepBarX + x * (9 + 6 * 7), textStartY);
                        canvas.ShowText(code.JSubstring(7, 8));
                        break;
                    }

                    case SUPP2:
                    case SUPP5: {
                        for (int k = 0; k < code.Length; ++k) {
                            String c = code.JSubstring(k, k + 1);
                            float len = font.GetWidth(c, size);
                            float pX = (7.5f + (9 * k)) * x - len / 2;
                            canvas.SetTextMatrix(pX, textStartY);
                            canvas.ShowText(c);
                        }
                        break;
                    }
                }
                canvas.EndText();
            }
            return rect;
        }
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
    }
}
