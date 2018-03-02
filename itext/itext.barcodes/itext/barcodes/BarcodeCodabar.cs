/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    public class BarcodeCodabar : Barcode1D {
        /// <summary>The index chars to <CODE>BARS</CODE>.</summary>
        private const String CHARS = "0123456789-$:/.+ABCD";

        private const int START_STOP_IDX = 16;

        /// <summary>The bars to generate the code.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 0, 0, 0, 0, 0, 1, 1 }, new byte[] { 0, 
            0, 0, 0, 1, 1, 0 }, new byte[] { 0, 0, 0, 1, 0, 0, 1 }, new byte[] { 1, 1, 0, 0, 0, 0, 0 }, new byte[]
             { 0, 0, 1, 0, 0, 1, 0 }, new byte[] { 1, 0, 0, 0, 0, 1, 0 }, new byte[] { 0, 1, 0, 0, 0, 0, 1 }, new 
            byte[] { 0, 1, 0, 0, 1, 0, 0 }, new byte[] { 0, 1, 1, 0, 0, 0, 0 }, new byte[] { 1, 0, 0, 1, 0, 0, 0 }
            , new byte[] { 0, 0, 0, 1, 1, 0, 0 }, new byte[] { 0, 0, 1, 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0, 1, 
            0, 1 }, new byte[] { 1, 0, 1, 0, 0, 0, 1 }, new byte[] { 1, 0, 1, 0, 1, 0, 0 }, new byte[] { 0, 0, 1, 
            0, 1, 0, 1 }, new byte[] { 0, 0, 1, 1, 0, 1, 0 }, new byte[] { 0, 1, 0, 1, 0, 0, 1 }, new byte[] { 0, 
            0, 0, 1, 0, 1, 1 }, new byte[] { 0, 0, 0, 1, 1, 1, 0 } };

        /// <summary>Creates a new BarcodeCodabar.</summary>
        /// <param name="document">The document</param>
        public BarcodeCodabar(PdfDocument document)
            : base(document) {
            // 0
            // 1
            // 2
            // 3
            // 4
            // 5
            // 6
            // 7
            // 8
            // 9
            // -
            // $
            // :
            // /
            // .
            // +
            // a
            // b
            // c
            // d
            x = 0.8f;
            n = 2;
            font = document.GetDefaultFont();
            size = 8;
            baseline = size;
            barHeight = size * 3;
            textAlignment = ALIGN_CENTER;
            generateChecksum = false;
            checksumText = false;
            startStopText = false;
        }

        /// <summary>Creates the bars.</summary>
        /// <param name="text">the text to create the bars</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsCodabar(String text) {
            text = text.ToUpperInvariant();
            int len = text.Length;
            if (len < 2) {
                throw new ArgumentException(PdfException.CodabarMustHaveAtLeastStartAndStopCharacter);
            }
            if (CHARS.IndexOf(text[0]) < START_STOP_IDX || CHARS.IndexOf(text[len - 1]) < START_STOP_IDX) {
                throw new ArgumentException(PdfException.CodabarMustHaveOneAbcdAsStartStopCharacter);
            }
            byte[] bars = new byte[text.Length * 8 - 1];
            for (int k = 0; k < len; ++k) {
                int idx = CHARS.IndexOf(text[k]);
                if (idx >= START_STOP_IDX && k > 0 && k < len - 1) {
                    throw new ArgumentException(PdfException.InCodabarStartStopCharactersAreOnlyAllowedAtTheExtremes);
                }
                if (idx < 0) {
                    throw new ArgumentException(PdfException.IllegalCharacterInCodabarBarcode);
                }
                Array.Copy(BARS[idx], 0, bars, k * 8, 7);
            }
            return bars;
        }

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

        /// <summary>Places the barcode in a <CODE>PdfCanvas</CODE>.</summary>
        /// <remarks>
        /// Places the barcode in a <CODE>PdfCanvas</CODE>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.<p>
        /// The bars and text are written in the following colors:
        /// <br />
        /// <TABLE BORDER=1 SUMMARY="barcode properties">
        /// <TR>
        /// <TH><P><CODE>barColor</CODE></TH>
        /// <TH><P><CODE>textColor</CODE></TH>
        /// <TH><P>Result</TH>
        /// </TR>
        /// <TR>
        /// <TD><P><CODE>null</CODE></TD>
        /// <TD><P><CODE>null</CODE></TD>
        /// <TD><P>bars and text painted with current fill color</TD>
        /// </TR>
        /// <TR>
        /// <TD><P><CODE>barColor</CODE></TD>
        /// <TD><P><CODE>null</CODE></TD>
        /// <TD><P>bars and text painted with <CODE>barColor</CODE></TD>
        /// </TR>
        /// <TR>
        /// <TD><P><CODE>null</CODE></TD>
        /// <TD><P><CODE>textColor</CODE></TD>
        /// <TD><P>bars painted with current color<br />text painted with <CODE>textColor</CODE></TD>
        /// </TR>
        /// <TR>
        /// <TD><P><CODE>barColor</CODE></TD>
        /// <TD><P><CODE>textColor</CODE></TD>
        /// <TD><P>bars painted with <CODE>barColor</CODE><br />text painted with <CODE>textColor</CODE></TD>
        /// </TR>
        /// </TABLE>
        /// </remarks>
        /// <param name="canvas">the <CODE>PdfCanvas</CODE> where the barcode will be placed</param>
        /// <param name="barColor">the color of the bars. It can be <CODE>null</CODE></param>
        /// <param name="textColor">the color of the text. It can be <CODE>null</CODE></param>
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
        // AWT related methods (remove this if you port to Android / GAE)
    }
}
