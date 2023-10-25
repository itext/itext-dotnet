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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>
    /// POSTNET (Postal Numeric Encoding Technique) is a barcode symbology used by the United States Postal Service to assist
    /// in directing mail.
    /// </summary>
    /// <remarks>
    /// POSTNET (Postal Numeric Encoding Technique) is a barcode symbology used by the United States Postal Service to assist
    /// in directing mail. The ZIP Code or ZIP+4 code is encoded in half- and full-height bars.[1] Most often, the delivery
    /// point is added, usually being the last two digits of the address or PO box number.
    /// </remarks>
    public class BarcodePostnet : Barcode1D {
        public static int TYPE_POSTNET = 1;

        public static int TYPE_PLANET = 2;

        /// <summary>The bars for each character.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 1, 1, 0, 0, 0 }, new byte[] { 0, 0, 0, 
            1, 1 }, new byte[] { 0, 0, 1, 0, 1 }, new byte[] { 0, 0, 1, 1, 0 }, new byte[] { 0, 1, 0, 0, 1 }, new 
            byte[] { 0, 1, 0, 1, 0 }, new byte[] { 0, 1, 1, 0, 0 }, new byte[] { 1, 0, 0, 0, 1 }, new byte[] { 1, 
            0, 0, 1, 0 }, new byte[] { 1, 0, 1, 0, 0 } };

        /// <summary>
        /// Creates new
        /// <see cref="BarcodePostnet"/>
        /// instance.
        /// </summary>
        /// <param name="document">The document</param>
        public BarcodePostnet(PdfDocument document)
            : base(document) {
            // distance between bars
            n = 72f / 22f;
            // bar width
            x = 0.02f * 72f;
            // height of the tall bars
            barHeight = 0.125f * 72f;
            // height of the short bars
            size = 0.05f * 72f;
            // type of code
            codeType = TYPE_POSTNET;
        }

        /// <summary>Creates the bars for Postnet.</summary>
        /// <param name="text">the code to be created without checksum</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsPostnet(String text) {
            int total = 0;
            for (int k = text.Length - 1; k >= 0; --k) {
                int n = text[k] - '0';
                total += n;
            }
            text += (char)(((10 - (total % 10)) % 10) + '0');
            byte[] bars = new byte[text.Length * 5 + 2];
            bars[0] = 1;
            bars[bars.Length - 1] = 1;
            for (int k = 0; k < text.Length; ++k) {
                int c = text[k] - '0';
                Array.Copy(BARS[c], 0, bars, k * 5 + 1, 5);
            }
            return bars;
        }

        public override Rectangle GetBarcodeSize() {
            float width = ((code.Length + 1) * 5 + 1) * n + x;
            return new Rectangle(width, barHeight);
        }

        public override void FitWidth(float width) {
            byte[] bars = GetBarsPostnet(code);
            float currentWidth = GetBarcodeSize().GetWidth();
            x *= width / currentWidth;
            n = (width - x) / (bars.Length - 1);
        }

        public override Rectangle PlaceBarcode(PdfCanvas canvas, Color barColor, Color textColor) {
            if (barColor != null) {
                canvas.SetFillColor(barColor);
            }
            byte[] bars = GetBarsPostnet(code);
            byte flip = 1;
            if (codeType == TYPE_PLANET) {
                flip = 0;
                bars[0] = 0;
                bars[bars.Length - 1] = 0;
            }
            float startX = 0;
            for (int k = 0; k < bars.Length; ++k) {
                canvas.Rectangle(startX, 0, x - inkSpreading, bars[k] == flip ? barHeight : size);
                startX += n;
            }
            canvas.Fill();
            return GetBarcodeSize();
        }
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
    }
}
