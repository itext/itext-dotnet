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
using System.Text;
using iText.Barcodes.Exceptions;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>Implements the code interleaved 2 of 5.</summary>
    /// <remarks>
    /// Implements the code interleaved 2 of 5. The text can include
    /// non numeric characters that are printed but do not generate bars.
    /// The default parameters are:
    /// <pre>
    /// x = 0.8f;
    /// n = 2;
    /// font = new PdfType1Font(document, new TYPE_1_FONT(StandardFonts.HELVETICA, PdfEncodings.WINANSI));
    /// size = 8;
    /// baseline = size;
    /// barHeight = size * 3;
    /// textAlignment = ALIGN_CENTER;
    /// generateChecksum = false;
    /// checksumText = false;
    /// </pre>
    /// </remarks>
    public class BarcodeInter25 : Barcode1D {
        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 0, 0, 1, 1, 0 }, new byte[] { 1, 0, 0, 
            0, 1 }, new byte[] { 0, 1, 0, 0, 1 }, new byte[] { 1, 1, 0, 0, 0 }, new byte[] { 0, 0, 1, 0, 1 }, new 
            byte[] { 1, 0, 1, 0, 0 }, new byte[] { 0, 1, 1, 0, 0 }, new byte[] { 0, 0, 0, 1, 1 }, new byte[] { 1, 
            0, 0, 1, 0 }, new byte[] { 0, 1, 0, 1, 0 } };

        /// <summary>Creates new BarcodeInter25.</summary>
        /// <remarks>
        /// Creates new BarcodeInter25.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="BarcodeInter25(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public BarcodeInter25(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates new BarcodeInter25</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public BarcodeInter25(PdfDocument document, PdfFont font)
            : base(document) {
            this.x = 0.8f;
            this.n = 2;
            this.font = font;
            this.size = 8;
            this.baseline = size;
            this.barHeight = size * 3;
            this.textAlignment = ALIGN_CENTER;
            this.generateChecksum = false;
            this.checksumText = false;
        }

        /// <summary>Deletes all the non numeric characters from <c>text</c>.</summary>
        /// <param name="text">the text</param>
        /// <returns>a <c>String</c> with only numeric characters</returns>
        public static String KeepNumbers(String text) {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < text.Length; ++k) {
                char c = text[k];
                if (c >= '0' && c <= '9') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>Calculates the checksum.</summary>
        /// <param name="text">the numeric text</param>
        /// <returns>the checksum</returns>
        public static char GetChecksum(String text) {
            int mul = 3;
            int total = 0;
            for (int k = text.Length - 1; k >= 0; --k) {
                int n = text[k] - '0';
                total += mul * n;
                mul ^= 2;
            }
            return (char)(((10 - (total % 10)) % 10) + '0');
        }

        /// <summary>Creates the bars for the barcode.</summary>
        /// <param name="text">the text. It can contain non numeric characters</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsInter25(String text) {
            text = KeepNumbers(text);
            if ((text.Length & 1) != 0) {
                throw new PdfException(BarcodeExceptionMessageConstant.TEXT_MUST_BE_EVEN);
            }
            byte[] bars = new byte[text.Length * 5 + 7];
            int pb = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            int len = text.Length / 2;
            for (int k = 0; k < len; ++k) {
                int c1 = text[k * 2] - '0';
                int c2 = text[k * 2 + 1] - '0';
                byte[] b1 = BARS[c1];
                byte[] b2 = BARS[c2];
                for (int j = 0; j < 5; ++j) {
                    bars[pb++] = b1[j];
                    bars[pb++] = b2[j];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 0;
            bars[pb++] = 0;
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
            if (font != null) {
                if (baseline > 0) {
                    fontY = baseline - GetDescender();
                }
                else {
                    fontY = -baseline + size;
                }
                String fullCode = code;
                if (generateChecksum && checksumText) {
                    fullCode += GetChecksum(fullCode);
                }
                fontX = font.GetWidth(altText != null ? altText : fullCode, size);
            }
            String fullCode_1 = KeepNumbers(code);
            int len = fullCode_1.Length;
            if (generateChecksum) {
                ++len;
            }
            float fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
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
            String fullCode = code;
            float fontX = 0;
            if (font != null) {
                if (generateChecksum && checksumText) {
                    fullCode += GetChecksum(fullCode);
                }
                fontX = font.GetWidth(fullCode = altText != null ? altText : fullCode, size);
            }
            String bCode = KeepNumbers(code);
            if (generateChecksum) {
                bCode += GetChecksum(bCode);
            }
            int len = bCode.Length;
            float fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
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
            byte[] bars = GetBarsInter25(bCode);
            bool print = true;
            if (barColor != null) {
                canvas.SetFillColor(barColor);
            }
            for (int k = 0; k < bars.Length; ++k) {
                float w = (bars[k] == 0 ? x : x * n);
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
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
    }
}
