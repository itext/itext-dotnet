/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>Implements the MSI Barcode.</summary>
    /// <remarks>
    /// Implements the MSI Barcode.
    /// The <c>code</c> may only contain numeric characters.
    /// The
    /// <see cref="GetChecksum(System.String)">getChecksum</see>
    /// method returns the mod 10 checksum digit which is the most widely used for MSI barcodes.
    /// </remarks>
    public class BarcodeMSI : Barcode1D {
        /// <summary>The index chars to <c>BARS</c> representing valid characters in the <c>code</c></summary>
        private const String CHARS = "0123456789";

        /// <summary>The sequence prepended to the start of all MSI Barcodes.</summary>
        private static readonly byte[] BARS_START = new byte[] { 1, 1, 0 };

        /// <summary>The sequence appended to the end of all MSI Barcodes.</summary>
        private static readonly byte[] BARS_END = new byte[] { 1, 0, 0, 1 };

        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { 
                // 0
                new byte[] { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0 }, 
                // 1
                new byte[] { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0 }, 
                // 2
                new byte[] { 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0 }, 
                // 3
                new byte[] { 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0 }, 
                // 4
                new byte[] { 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0 }, 
                // 5
                new byte[] { 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0 }, 
                // 6
                new byte[] { 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0 }, 
                // 7
                new byte[] { 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0 }, 
                // 8
                new byte[] { 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0 }, 
                // 9
                new byte[] { 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0 } };

        /// <summary>The number of individual bars either drawn or not drawn per character of the <c>code</c></summary>
        private const int BARS_PER_CHARACTER = 12;

        /// <summary>The number of individual bars either drawn or not drawn for the start character in the BarcodeMSI.
        ///     </summary>
        private const int BARS_FOR_START = 3;

        /// <summary>The number of individual bars either drawn or not drawn for the stop character in the BarcodeMSI.
        ///     </summary>
        private const int BARS_FOR_STOP = 4;

        /// <summary>Creates a new BarcodeMSI.</summary>
        /// <remarks>
        /// Creates a new BarcodeMSI.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="BarcodeMSI(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public BarcodeMSI(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates a new BarcodeMSI</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public BarcodeMSI(PdfDocument document, PdfFont font)
            : base(document) {
            this.x = 0.8f;
            this.n = 2.0f;
            this.font = font;
            this.size = 8.0f;
            this.baseline = this.size;
            this.barHeight = this.size * 3.0f;
            this.generateChecksum = false;
            this.checksumText = false;
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
            float fontX = 0.0f;
            float fontY = 0.0f;
            String fCode = this.code;
            if (this.font != null) {
                if (this.baseline > 0.0f) {
                    fontY = this.baseline - this.GetDescender();
                }
                else {
                    fontY = -this.baseline + this.size;
                }
                String fullCode = this.code;
                fontX = this.font.GetWidth(this.altText != null ? this.altText : fullCode, this.size);
            }
            int len = fCode.Length;
            if (this.generateChecksum) {
                ++len;
            }
            float fullWidth = (len * BARS_PER_CHARACTER + BARS_FOR_START + BARS_FOR_STOP) * x;
            fullWidth = Math.Max(fullWidth, fontX);
            float fullHeight = this.barHeight + fontY;
            return new Rectangle(fullWidth, fullHeight);
        }

        /// <summary>Places the barcode in a <c>PdfCanvas</c>.</summary>
        /// <remarks>
        /// Places the barcode in a <c>PdfCanvas</c>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.
        /// <para />
        /// The bars and text are written in the following colors:
        /// <table border="1" summary="">
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
            String fullCode = this.code;
            if (this.checksumText) {
                fullCode = fullCode + JavaUtil.IntegerToString(GetChecksum(this.code));
            }
            float fontX = 0.0f;
            if (this.font != null) {
                String var10001 = this.altText != null ? this.altText : fullCode;
                fullCode = this.altText != null ? this.altText : fullCode;
                fontX = this.font.GetWidth(var10001, this.size);
            }
            String bCode = this.code;
            if (this.generateChecksum) {
                bCode += GetChecksum(bCode);
            }
            int idx;
            idx = bCode.Length;
            float fullWidth = (idx * BARS_PER_CHARACTER + BARS_FOR_START + BARS_FOR_STOP) * this.x;
            float barStartX = 0.0f;
            float textStartX = 0.0f;
            switch (this.textAlignment) {
                case 1: {
                    break;
                }

                case 2: {
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
                        barStartX = (fontX - fullWidth) / 2.0f;
                    }
                    else {
                        textStartX = (fullWidth - fontX) / 2.0f;
                    }
                    break;
                }
            }
            float barStartY = 0.0f;
            float textStartY = 0.0f;
            if (this.font != null) {
                if (this.baseline <= 0.0f) {
                    textStartY = this.barHeight - this.baseline;
                }
                else {
                    textStartY = -this.GetDescender();
                    barStartY = textStartY + this.baseline;
                }
            }
            byte[] bars = GetBarsMSI(bCode);
            if (barColor != null) {
                canvas.SetFillColor(barColor);
            }
            for (int k = 0; k < bars.Length; ++k) {
                float w = (float)bars[k] * this.x;
                if (bars[k] == 1) {
                    canvas.Rectangle((double)barStartX, (double)barStartY, (double)(w - this.inkSpreading), (double)this.barHeight
                        );
                }
                barStartX += this.x;
            }
            canvas.Fill();
            if (this.font != null) {
                if (textColor != null) {
                    canvas.SetFillColor(textColor);
                }
                canvas.BeginText();
                canvas.SetFontAndSize(this.font, this.size);
                canvas.SetTextMatrix(textStartX, textStartY);
                canvas.ShowText(fullCode);
                canvas.EndText();
            }
            return this.GetBarcodeSize();
        }

        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
        /// <summary>Creates the bars.</summary>
        /// <param name="text">the text to create the bars.</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsMSI(String text) {
            if (text == null) {
                throw new ArgumentException("Valid code required to generate MSI barcode.");
            }
            byte[] bars = new byte[((text.Length) * BARS_PER_CHARACTER) + 7];
            Array.Copy(BARS_START, 0, bars, 0, 3);
            for (int x = 0; x < text.Length; x++) {
                char ch = text[x];
                int idx = CHARS.IndexOf(ch);
                if (idx < 0) {
                    throw new ArgumentException("The character " + text[x] + " is illegal in MSI bar codes.");
                }
                Array.Copy(BARS[idx], 0, bars, 3 + x * 12, 12);
            }
            Array.Copy(BARS_END, 0, bars, bars.Length - 4, 4);
            return bars;
        }

        /// <summary>Calculates the mod 10 checksum digit using the Luhn algorithm.</summary>
        /// <param name="text">the barcode data</param>
        /// <returns>the checksum digit</returns>
        public static int GetChecksum(String text) {
            if (text == null) {
                throw new ArgumentException("Valid code required to generate checksum for MSI barcode");
            }
            int[] digits = new int[text.Length];
            for (int x = 0; x < text.Length; x++) {
                digits[x] = (int)(text[x] - '0');
                if (digits[x] < 0 || digits[x] > 9) {
                    throw new ArgumentException("The character " + text[x] + " is illegal in MSI bar codes.");
                }
            }
            int sum = 0;
            int length = digits.Length;
            for (int i = 0; i < length; i++) {
                int digit = digits[length - i - 1];
                if (i % 2 == 0) {
                    digit *= 2;
                }
                sum += digit > 9 ? digit - 9 : digit;
            }
            return (sum * 9) % 10;
        }
    }
}
