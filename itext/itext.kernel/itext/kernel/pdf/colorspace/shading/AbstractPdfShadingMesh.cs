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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The PdfShadingMesh class which extends
    /// <see cref="AbstractPdfShading"/>
    /// and represents shadings which are based on a mesh,
    /// with BitsPerCoordinate, BitsPerComponent and Decode fields in the PDF object.
    /// </summary>
    public abstract class AbstractPdfShadingMesh : AbstractPdfShading {
        /// <summary>Gets the number of bits used to represent each vertex coordinate.</summary>
        /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, 16, 24, or 32</returns>
        public virtual int GetBitsPerCoordinate() {
            return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
        }

        /// <summary>Sets the number of bits used to represent each vertex coordinate.</summary>
        /// <param name="bitsPerCoordinate">the number of bits to be set. Shall be 1, 2, 4, 8, 12, 16, 24, or 32</param>
        public void SetBitsPerCoordinate(int bitsPerCoordinate) {
            GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
            SetModified();
        }

        /// <summary>Gets the number of bits used to represent each colour component.</summary>
        /// <returns>the number of bits. Can be 1, 2, 4, 8, 12, or 16</returns>
        public virtual int GetBitsPerComponent() {
            return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
        }

        /// <summary>Sets the number of bits used to represent each colour component.</summary>
        /// <param name="bitsPerComponent">the number of bits to be set. Shall be 1, 2, 4, 8, 12, or 16</param>
        public void SetBitsPerComponent(int bitsPerComponent) {
            GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
            SetModified();
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values.
        /// </summary>
        /// <remarks>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present.
        /// </remarks>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// Decode object
        /// </returns>
        public virtual PdfArray GetDecode() {
            return GetPdfObject().GetAsArray(PdfName.Decode);
        }

        /// <summary>
        /// Sets the
        /// <c>float[]</c>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values.
        /// </summary>
        /// <remarks>
        /// Sets the
        /// <c>float[]</c>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present.
        /// </remarks>
        /// <param name="decode">
        /// the
        /// <c>float[]</c>
        /// of Decode object to set
        /// </param>
        public void SetDecode(float[] decode) {
            SetDecode(new PdfArray(decode));
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values.
        /// </summary>
        /// <remarks>
        /// Sets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present.
        /// </remarks>
        /// <param name="decode">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// Decode object to set
        /// </param>
        public void SetDecode(PdfArray decode) {
            GetPdfObject().Put(PdfName.Decode, decode);
            SetModified();
        }

        /// <summary>Constructor for PdfShadingBlend object using a PdfDictionary.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        protected internal AbstractPdfShadingMesh(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Constructor for PdfShadingBlend object using a PdfDictionary, shading type and color space.</summary>
        /// <param name="pdfObject">input PdfDictionary</param>
        /// <param name="type">shading type</param>
        /// <param name="colorSpace">color space</param>
        protected internal AbstractPdfShadingMesh(PdfDictionary pdfObject, int type, PdfColorSpace colorSpace)
            : base(pdfObject, type, colorSpace) {
        }
    }
}
