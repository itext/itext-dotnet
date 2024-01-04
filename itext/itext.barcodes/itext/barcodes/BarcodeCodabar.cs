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
using iText.Barcodes.Exceptions;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>Implementation of the Codabar barcode.</summary>
    /// <remarks>
    /// Implementation of the Codabar barcode.
    /// <para />
    /// Codabar was designed to be accurately read even when printed on dot-matrix printers for multi-part forms such as
    /// FedEx airbills and blood bank forms, where variants are still in use as of 2007. Although newer symbologies hold more
    /// information in a smaller space.
    /// </remarks>
    public class BarcodeCodabar : Barcode1D {
        /// <summary>The index chars to <c>BARS</c>.</summary>
        private const String CHARS = "0123456789-$:/.+ABCD";

        private const int START_STOP_IDX = 16;

        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { 
                // 0
                new byte[] { 0, 0, 0, 0, 0, 1, 1 }, 
                // 1
                new byte[] { 0, 0, 0, 0, 1, 1, 0 }, 
                // 2
                new byte[] { 0, 0, 0, 1, 0, 0, 1 }, 
                // 3
                new byte[] { 1, 1, 0, 0, 0, 0, 0 }, 
                // 4
                new byte[] { 0, 0, 1, 0, 0, 1, 0 }, 
                // 5
                new byte[] { 1, 0, 0, 0, 0, 1, 0 }, 
                // 6
                new byte[] { 0, 1, 0, 0, 0, 0, 1 }, 
                // 7
                new byte[] { 0, 1, 0, 0, 1, 0, 0 }, 
                // 8
                new byte[] { 0, 1, 1, 0, 0, 0, 0 }, 
                // 9
                new byte[] { 1, 0, 0, 1, 0, 0, 0 }, 
                // -
                new byte[] { 0, 0, 0, 1, 1, 0, 0 }, 
                // $
                new byte[] { 0, 0, 1, 1, 0, 0, 0 }, 
                // :
                new byte[] { 1, 0, 0, 0, 1, 0, 1 }, 
                // /
                new byte[] { 1, 0, 1, 0, 0, 0, 1 }, 
                // .
                new byte[] { 1, 0, 1, 0, 1, 0, 0 }, 
                // +
                new byte[] { 0, 0, 1, 0, 1, 0, 1 }, 
                // a
                new byte[] { 0, 0, 1, 1, 0, 1, 0 }, 
                // b
                new byte[] { 0, 1, 0, 1, 0, 0, 1 }, 
                // c
                new byte[] { 0, 0, 0, 1, 0, 1, 1 }, 
                // d
                new byte[] { 0, 0, 0, 1, 1, 1, 0 } };

        /// <summary>Creates a new BarcodeCodabar.</summary>
        /// <remarks>
        /// Creates a new BarcodeCodabar.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="BarcodeCodabar(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public BarcodeCodabar(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates a new BarcodeCodabar.</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public BarcodeCodabar(PdfDocument document, PdfFont font)
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
            this.startStopText = false;
        }

        /// <summary>Creates the bars.</summary>
        /// <param name="text">the text to create the bars</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsCodabar(String text) {
            text = text.ToUpperInvariant();
            int len = text.Length;
            if (len < 2) {
                throw new ArgumentException(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_AT_LEAST_START_AND_STOP_CHARACTER
                    );
            }
            if (CHARS.IndexOf(text[0]) < START_STOP_IDX || CHARS.IndexOf(text[len - 1]) < START_STOP_IDX) {
                throw new ArgumentException(BarcodesExceptionMessageConstant.CODABAR_MUST_HAVE_ONE_ABCD_AS_START_STOP_CHARACTER
                    );
            }
            byte[] bars = new byte[text.Length * 8 - 1];
            for (int k = 0; k < len; ++k) {
                int idx = CHARS.IndexOf(text[k]);
                if (idx >= START_STOP_IDX && k > 0 && k < len - 1) {
                    throw new ArgumentException(BarcodesExceptionMessageConstant.IN_CODABAR_START_STOP_CHARACTERS_ARE_ONLY_ALLOWED_AT_THE_EXTREMES
                        );
                }
                if (idx < 0) {
                    throw new ArgumentException(BarcodesExceptionMessageConstant.ILLEGAL_CHARACTER_IN_CODABAR_BARCODE);
                }
                Array.Copy(BARS[idx], 0, bars, k * 8, 7);
            }
            return bars;
        }

        /// <summary>Calculates the checksum.</summary>
        /// <param name="code">the value to calculate the checksum for</param>
        /// <returns>the checksum for the given value</returns>
        public static String CalculateChecksum(String code) {
            if (code.Length < 2) {
                return code;
            }
            String text = code.ToUpperInvariant();
            int sum = 0;
            int len = text.Length;
            for (int k = 0; k < len; ++k) {
                sum += CHARS.IndexOf(text[k]);
            }
            sum = (sum + 15) / 16 * 16 - sum;
            return code.JSubstring(0, len - 1) + CHARS[sum] + code.Substring(len - 1);
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
            String text = code;
            if (generateChecksum && checksumText) {
                text = CalculateChecksum(code);
            }
            if (!startStopText) {
                text = text.JSubstring(1, text.Length - 1);
            }
            if (font != null) {
                if (baseline > 0) {
                    fontY = baseline - GetDescender();
                }
                else {
                    fontY = -baseline + size;
                }
                fontX = font.GetWidth(altText != null ? altText : text, size);
            }
            text = code;
            if (generateChecksum) {
                text = CalculateChecksum(code);
            }
            byte[] bars = GetBarsCodabar(text);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k) {
                wide += bars[k];
            }
            int narrow = bars.Length - wide;
            float fullWidth = x * (narrow + wide * n);
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
            if (generateChecksum && checksumText) {
                fullCode = CalculateChecksum(code);
            }
            if (!startStopText) {
                fullCode = fullCode.JSubstring(1, fullCode.Length - 1);
            }
            float fontX = 0;
            if (font != null) {
                fontX = font.GetWidth(fullCode = altText != null ? altText : fullCode, size);
            }
            byte[] bars = GetBarsCodabar(generateChecksum ? CalculateChecksum(code) : code);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k) {
                wide += bars[k];
            }
            int narrow = bars.Length - wide;
            float fullWidth = x * (narrow + wide * n);
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
