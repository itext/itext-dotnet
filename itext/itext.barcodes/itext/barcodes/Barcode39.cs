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
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    public class Barcode39 : Barcode1D {
        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 0, 0, 0, 1, 1, 0, 1, 0, 0 }, new byte[
            ] { 1, 0, 0, 1, 0, 0, 0, 0, 1 }, new byte[] { 0, 0, 1, 1, 0, 0, 0, 0, 1 }, new byte[] { 1, 0, 1, 1, 0, 
            0, 0, 0, 0 }, new byte[] { 0, 0, 0, 1, 1, 0, 0, 0, 1 }, new byte[] { 1, 0, 0, 1, 1, 0, 0, 0, 0 }, new 
            byte[] { 0, 0, 1, 1, 1, 0, 0, 0, 0 }, new byte[] { 0, 0, 0, 1, 0, 0, 1, 0, 1 }, new byte[] { 1, 0, 0, 
            1, 0, 0, 1, 0, 0 }, new byte[] { 0, 0, 1, 1, 0, 0, 1, 0, 0 }, new byte[] { 1, 0, 0, 0, 0, 1, 0, 0, 1 }
            , new byte[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 }, new byte[] { 1, 0, 1, 0, 0, 1, 0, 0, 0 }, new byte[] { 0, 
            0, 0, 0, 1, 1, 0, 0, 1 }, new byte[] { 1, 0, 0, 0, 1, 1, 0, 0, 0 }, new byte[] { 0, 0, 1, 0, 1, 1, 0, 
            0, 0 }, new byte[] { 0, 0, 0, 0, 0, 1, 1, 0, 1 }, new byte[] { 1, 0, 0, 0, 0, 1, 1, 0, 0 }, new byte[]
             { 0, 0, 1, 0, 0, 1, 1, 0, 0 }, new byte[] { 0, 0, 0, 0, 1, 1, 1, 0, 0 }, new byte[] { 1, 0, 0, 0, 0, 
            0, 0, 1, 1 }, new byte[] { 0, 0, 1, 0, 0, 0, 0, 1, 1 }, new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0 }, new 
            byte[] { 0, 0, 0, 0, 1, 0, 0, 1, 1 }, new byte[] { 1, 0, 0, 0, 1, 0, 0, 1, 0 }, new byte[] { 0, 0, 1, 
            0, 1, 0, 0, 1, 0 }, new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 1 }, new byte[] { 1, 0, 0, 0, 0, 0, 1, 1, 0 }
            , new byte[] { 0, 0, 1, 0, 0, 0, 1, 1, 0 }, new byte[] { 0, 0, 0, 0, 1, 0, 1, 1, 0 }, new byte[] { 1, 
            1, 0, 0, 0, 0, 0, 0, 1 }, new byte[] { 0, 1, 1, 0, 0, 0, 0, 0, 1 }, new byte[] { 1, 1, 1, 0, 0, 0, 0, 
            0, 0 }, new byte[] { 0, 1, 0, 0, 1, 0, 0, 0, 1 }, new byte[] { 1, 1, 0, 0, 1, 0, 0, 0, 0 }, new byte[]
             { 0, 1, 1, 0, 1, 0, 0, 0, 0 }, new byte[] { 0, 1, 0, 0, 0, 0, 1, 0, 1 }, new byte[] { 1, 1, 0, 0, 0, 
            0, 1, 0, 0 }, new byte[] { 0, 1, 1, 0, 0, 0, 1, 0, 0 }, new byte[] { 0, 1, 0, 1, 0, 1, 0, 0, 0 }, new 
            byte[] { 0, 1, 0, 1, 0, 0, 0, 1, 0 }, new byte[] { 0, 1, 0, 0, 0, 1, 0, 1, 0 }, new byte[] { 0, 0, 0, 
            1, 0, 1, 0, 1, 0 }, new byte[] { 0, 1, 0, 0, 1, 0, 1, 0, 0 } };

        /// <summary>
        /// The index chars to <c>BARS</c>, symbol * use only start and stop  characters,
        /// the * character will not appear in the input data.
        /// </summary>
        private const String CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

        /// <summary>The character combinations to make the code 39 extended.</summary>
        private const String EXTENDED = "%U" + "$A$B$C$D$E$F$G$H$I$J$K$L$M$N$O$P$Q$R$S$T$U$V$W$X$Y$Z" + "%A%B%C%D%E  /A/B/C/D/E/F/G/H/I/J/K/L - ./O"
             + " 0 1 2 3 4 5 6 7 8 9/Z%F%G%H%I%J%V" + " A B C D E F G H I J K L M N O P Q R S T U V W X Y Z" + "%K%L%M%N%O%W"
             + "+A+B+C+D+E+F+G+H+I+J+K+L+M+N+O+P+Q+R+S+T+U+V+W+X+Y+Z" + "%P%Q%R%S%T";

        /// <summary>Creates a new Barcode39.</summary>
        /// <remarks>
        /// Creates a new Barcode39.
        /// To generate the font the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.GetDefaultFont()"/>
        /// will be implicitly called.
        /// If you want to use this barcode in PDF/A documents, please consider using
        /// <see cref="Barcode39(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Font.PdfFont)"/>.
        /// </remarks>
        /// <param name="document">The document to which the barcode will be added</param>
        public Barcode39(PdfDocument document)
            : this(document, document.GetDefaultFont()) {
        }

        /// <summary>Creates a new Barcode39.</summary>
        /// <param name="document">The document to which the barcode will be added</param>
        /// <param name="font">The font to use</param>
        public Barcode39(PdfDocument document, PdfFont font)
            : base(document) {
            this.x = 0.8f;
            this.n = 2;
            this.font = font;
            this.size = 8;
            this.baseline = size;
            this.barHeight = size * 3;
            this.generateChecksum = false;
            this.checksumText = false;
            this.startStopText = true;
            this.extended = false;
        }

        /// <summary>Creates the bars.</summary>
        /// <param name="text">
        /// the text to create the bars. This text does not include the start and
        /// stop characters
        /// </param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsCode39(String text) {
            text = "*" + text + "*";
            byte[] bars = new byte[text.Length * 10 - 1];
            for (int k = 0; k < text.Length; ++k) {
                char ch = text[k];
                int idx = CHARS.IndexOf(ch);
                if (ch == '*' && k != 0 && k != (text.Length - 1)) {
                    throw new ArgumentException("The character " + ch + " is illegal in code 39");
                }
                if (idx < 0) {
                    throw new ArgumentException("The character " + text[k] + " is illegal in code 39");
                }
                Array.Copy(BARS[idx], 0, bars, k * 10, 9);
            }
            return bars;
        }

        /// <summary>
        /// Converts the extended text into a normal, escaped text,
        /// ready to generate bars.
        /// </summary>
        /// <param name="text">the extended text</param>
        /// <returns>the escaped text</returns>
        public static String GetCode39Ex(String text) {
            StringBuilder @out = new StringBuilder("");
            for (int k = 0; k < text.Length; ++k) {
                char c = text[k];
                if (c > 127) {
                    throw new ArgumentException("The character " + c + " is illegal in code 39");
                }
                char c1 = EXTENDED[c * 2];
                char c2 = EXTENDED[c * 2 + 1];
                if (c1 != ' ') {
                    @out.Append(c1);
                }
                @out.Append(c2);
            }
            return @out.ToString();
        }

        /// <summary>Calculates the checksum.</summary>
        /// <param name="text">the text</param>
        /// <returns>the checksum</returns>
        internal static char GetChecksum(String text) {
            int chk = 0;
            for (int k = 0; k < text.Length; ++k) {
                int idx = CHARS.IndexOf(text[k]);
                char ch = text[k];
                if (ch == '*' && k != 0 && k != (text.Length - 1)) {
                    throw new ArgumentException("The character " + ch + " is illegal in code 39");
                }
                if (idx < 0) {
                    throw new ArgumentException("The character " + text[k] + " is illegal in code 39");
                }
                chk += idx;
            }
            return CHARS[chk % 43];
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
            String fCode = code;
            if (extended) {
                fCode = GetCode39Ex(code);
            }
            if (font != null) {
                if (baseline > 0) {
                    fontY = baseline - GetDescender();
                }
                else {
                    fontY = -baseline + size;
                }
                String fullCode = code;
                if (generateChecksum && checksumText) {
                    fullCode += GetChecksum(fCode);
                }
                if (startStopText) {
                    fullCode = "*" + fullCode + "*";
                }
                fontX = font.GetWidth(altText != null ? altText : fullCode, size);
            }
            int len = fCode.Length + 2;
            if (generateChecksum) {
                ++len;
            }
            float fullWidth = len * (6 * x + 3 * x * n) + (len - 1) * x;
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
            String bCode = code;
            if (extended) {
                bCode = GetCode39Ex(code);
            }
            if (font != null) {
                if (generateChecksum && checksumText) {
                    fullCode += GetChecksum(bCode);
                }
                if (startStopText) {
                    fullCode = "*" + fullCode + "*";
                }
                fontX = font.GetWidth(fullCode = altText != null ? altText : fullCode, size);
            }
            if (generateChecksum) {
                bCode += GetChecksum(bCode);
            }
            int len = bCode.Length + 2;
            float fullWidth = len * (6 * x + 3 * x * n) + (len - 1) * x;
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
            byte[] bars = GetBarsCode39(bCode);
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
                canvas.BeginText().SetFontAndSize(font, size).SetTextMatrix(textStartX, textStartY).ShowText(fullCode).EndText
                    ();
            }
            return GetBarcodeSize();
        }
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
    }
}
