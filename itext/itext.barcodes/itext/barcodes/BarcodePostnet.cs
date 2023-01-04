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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    public class BarcodePostnet : Barcode1D {
        public static int TYPE_POSTNET = 1;

        public static int TYPE_PLANET = 2;

        /// <summary>The bars for each character.</summary>
        private static readonly byte[][] BARS = new byte[][] { new byte[] { 1, 1, 0, 0, 0 }, new byte[] { 0, 0, 0, 
            1, 1 }, new byte[] { 0, 0, 1, 0, 1 }, new byte[] { 0, 0, 1, 1, 0 }, new byte[] { 0, 1, 0, 0, 1 }, new 
            byte[] { 0, 1, 0, 1, 0 }, new byte[] { 0, 1, 1, 0, 0 }, new byte[] { 1, 0, 0, 0, 1 }, new byte[] { 1, 
            0, 0, 1, 0 }, new byte[] { 1, 0, 1, 0, 0 } };

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
    }
}
