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
using iText.Kernel.Pdf.Function;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// class and is in charge of Shading Dictionary
    /// with function-based type, that defines color at every point in the domain by a specified mathematical function.
    /// </summary>
    public class PdfFunctionBasedShading : AbstractPdfShading {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <param name="pdfDictionary">
        /// from which this
        /// <see cref="PdfFunctionBasedShading"/>
        /// will be created
        /// </param>
        public PdfFunctionBasedShading(PdfDictionary pdfDictionary)
            : base(pdfDictionary) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="colorSpace">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed
        /// </param>
        /// <param name="function">
        /// the
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// , that is used to calculate color transitions
        /// </param>
        public PdfFunctionBasedShading(PdfColorSpace colorSpace, IPdfFunction function)
            : this(colorSpace.GetPdfObject(), function) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="colorSpace">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// , that represents color space in which colour values shall be expressed
        /// </param>
        /// <param name="function">
        /// the
        /// <see cref="iText.Kernel.Pdf.Function.IPdfFunction"/>
        /// , that is used to calculate color transitions
        /// </param>
        public PdfFunctionBasedShading(PdfObject colorSpace, IPdfFunction function)
            : base(new PdfDictionary(), ShadingType.FUNCTION_BASED, PdfColorSpace.MakeColorSpace(colorSpace)) {
            SetFunction(function);
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// domain rectangle object that establishes an internal coordinate space
        /// for the shading that is independent of the target coordinate space in which it shall be painted.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// domain rectangle
        /// </returns>
        public virtual PdfArray GetDomain() {
            return GetPdfObject().GetAsArray(PdfName.Domain);
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// domain rectangle object that establishes an internal coordinate space
        /// for the shading that is independent of the target coordinate space in which it shall be painted.
        /// </summary>
        /// <param name="xmin">the Xmin coordinate of rectangle</param>
        /// <param name="xmax">the Xmax coordinate of rectangle</param>
        /// <param name="ymin">the Ymin coordinate of rectangle</param>
        /// <param name="ymax">the Ymax coordinate of rectangle</param>
        public virtual void SetDomain(float xmin, float xmax, float ymin, float ymax) {
            SetDomain(new PdfArray(new float[] { xmin, xmax, ymin, ymax }));
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// domain rectangle object that establishes an internal coordinate space
        /// for the shading that is independent of the target coordinate space in which it shall be painted.
        /// </summary>
        /// <param name="domain">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// domain rectangle object to be set
        /// </param>
        public virtual void SetDomain(PdfArray domain) {
            GetPdfObject().Put(PdfName.Domain, domain);
            SetModified();
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of floats that represents the transformation matrix that maps the domain rectangle
        /// into a corresponding figure in the target coordinate space.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of transformation matrix (identical matrix by default)
        /// </returns>
        public virtual PdfArray GetMatrix() {
            PdfArray matrix = GetPdfObject().GetAsArray(PdfName.Matrix);
            if (matrix == null) {
                matrix = new PdfArray(new float[] { 1, 0, 0, 1, 0, 0 });
                SetMatrix(matrix);
            }
            return matrix;
        }

        /// <summary>
        /// Sets the array of floats that represents the transformation matrix that maps the domain rectangle
        /// into a corresponding figure in the target coordinate space.
        /// </summary>
        /// <param name="matrix">
        /// the
        /// <c>float[]</c>
        /// of transformation matrix to be set
        /// </param>
        public virtual void SetMatrix(float[] matrix) {
            SetMatrix(new PdfArray(matrix));
        }

        /// <summary>
        /// Sets the array of floats that represents the transformation matrix that maps the domain rectangle
        /// into a corresponding figure in the target coordinate space.
        /// </summary>
        /// <param name="matrix">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// transformation matrix object to be set
        /// </param>
        public virtual void SetMatrix(PdfArray matrix) {
            GetPdfObject().Put(PdfName.Matrix, matrix);
            SetModified();
        }
    }
}
