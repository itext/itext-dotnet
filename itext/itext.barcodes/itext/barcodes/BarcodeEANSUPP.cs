/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
    /// <summary>
    /// Implements the most commonly used EAN standard is the thirteen-digit EAN-13, a superset of the original 12-digit
    /// Universal Product Code (UPC-A)
    /// </summary>
    public class BarcodeEANSUPP : Barcode1D {
        /// <summary>The barcode with the EAN/UPC.</summary>
        protected internal Barcode1D ean;

        /// <summary>The barcode with the supplemental.</summary>
        protected internal Barcode1D supp;

        /// <summary>Creates new combined barcode.</summary>
        /// <param name="ean">the EAN/UPC barcode</param>
        /// <param name="supp">the supplemental barcode</param>
        public BarcodeEANSUPP(Barcode1D ean, Barcode1D supp)
            : base(ean.document) {
            // horizontal distance between the two barcodes
            n = 8;
            this.ean = ean;
            this.supp = supp;
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
            Rectangle rect = ean.GetBarcodeSize();
            rect.SetWidth(rect.GetWidth() + supp.GetBarcodeSize().GetWidth() + n);
            return rect;
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
            if (supp.GetFont() == null) {
                supp.SetBarHeight(ean.GetBarHeight());
            }
            else {
                float sizeCoefficient = FontProgram.ConvertTextSpaceToGlyphSpace(supp.GetSize());
                supp.SetBarHeight(ean.GetBarHeight() + supp.GetBaseline() - supp.GetFont().GetFontProgram().GetFontMetrics
                    ().GetCapHeight() * sizeCoefficient);
            }
            Rectangle eanR = ean.GetBarcodeSize();
            canvas.SaveState();
            ean.PlaceBarcode(canvas, barColor, textColor);
            canvas.RestoreState();
            canvas.SaveState();
            canvas.ConcatMatrix(1, 0, 0, 1, eanR.GetWidth() + n, eanR.GetHeight() - ean.GetBarHeight());
            supp.PlaceBarcode(canvas, barColor, textColor);
            canvas.RestoreState();
            return GetBarcodeSize();
        }
        // Android-Conversion-Skip-Block-Start (java.awt library isn't available on Android)
        // Android-Conversion-Skip-Block-End
    }
}
