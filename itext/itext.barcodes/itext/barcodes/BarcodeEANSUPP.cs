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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;

namespace iText.Barcodes {
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
