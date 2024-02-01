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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;

namespace iText.Barcodes {
    /// <summary>This is a class that is used to implement the logic common to all 2D barcodes.</summary>
    /// <remarks>
    /// This is a class that is used to implement the logic common to all 2D barcodes.
    /// A 2D barcode is a barcode with two dimensions; this means that
    /// data can be encoded vertically and horizontally.
    /// </remarks>
    public abstract class Barcode2D {
        protected internal const float DEFAULT_MODULE_SIZE = 1;

        /// <summary>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy.
        /// </summary>
        /// <remarks>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy. The lower left corner is always (0, 0).
        /// </remarks>
        /// <returns>the size the barcode occupies.</returns>
        public abstract Rectangle GetBarcodeSize();

        /// <summary>Places the barcode in a <c>PdfCanvas</c>.</summary>
        /// <remarks>
        /// Places the barcode in a <c>PdfCanvas</c>. The
        /// barcode is always placed at coordinates (0, 0). Use the
        /// translation matrix to move it elsewhere.
        /// </remarks>
        /// <param name="canvas">the <c>PdfCanvas</c> where the barcode will be placed</param>
        /// <param name="foreground">the foreground color. It can be <c>null</c></param>
        /// <returns>the dimensions the barcode occupies</returns>
        public abstract Rectangle PlaceBarcode(PdfCanvas canvas, Color foreground);

        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <remarks>
        /// Creates a PdfFormXObject with the barcode.
        /// Default foreground color will be used.
        /// </remarks>
        /// <param name="document">The document</param>
        /// <returns>the XObject.</returns>
        public virtual PdfFormXObject CreateFormXObject(PdfDocument document) {
            return CreateFormXObject(null, document);
        }

        /// <summary>Creates a PdfFormXObject with the barcode.</summary>
        /// <param name="foreground">The color of the pixels. It can be <c>null</c></param>
        /// <param name="document">The document</param>
        /// <returns>the XObject.</returns>
        public abstract PdfFormXObject CreateFormXObject(Color foreground, PdfDocument document);
    }
}
