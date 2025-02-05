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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf.Colorspace.Shading {
    /// <summary>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingMesh"/>
    /// classes
    /// and is in charge of Shading Dictionary with lattice-form Gouraud-shaded triangle mesh type.
    /// </summary>
    /// <remarks>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// and
    /// <see cref="AbstractPdfShadingMesh"/>
    /// classes
    /// and is in charge of Shading Dictionary with lattice-form Gouraud-shaded triangle mesh type.
    /// <para />
    /// This type is similar to
    /// <see cref="PdfFreeFormGouraudShadedTriangleShading"/>
    /// but instead of using free-form geometry,
    /// the vertices are arranged in a pseudorectangular lattice,
    /// which is topologically equivalent to a rectangular grid.
    /// The vertices are organized into rows, which need not be geometrically linear.
    /// <para />
    /// The verticals data in stream is similar to
    /// <see cref="PdfFreeFormGouraudShadedTriangleShading"/>
    /// ,
    /// except there is no edge flag.
    /// </remarks>
    public class PdfLatticeFormGouraudShadedTriangleShading : AbstractPdfShadingMesh {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <param name="pdfStream">
        /// from which this
        /// <see cref="PdfLatticeFormGouraudShadedTriangleShading"/>
        /// will be created
        /// </param>
        public PdfLatticeFormGouraudShadedTriangleShading(PdfStream pdfStream)
            : base(pdfStream) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="bitsPerCoordinate">
        /// the number of bits used to represent each vertex coordinate.
        /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32
        /// </param>
        /// <param name="bitsPerComponent">
        /// the number of bits used to represent each colour component.
        /// The value shall be 1, 2, 4, 8, 12, or 16
        /// </param>
        /// <param name="verticesPerRow">
        /// the number of vertices in each row of the lattice (shall be &gt; 1).
        /// The number of rows need not be specified
        /// </param>
        /// <param name="decode">
        /// the
        /// <c>int[]</c>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfLatticeFormGouraudShadedTriangleShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
            , int verticesPerRow, float[] decode)
            : this(cs, bitsPerCoordinate, bitsPerComponent, verticesPerRow, new PdfArray(decode)) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted
        /// </param>
        /// <param name="bitsPerCoordinate">
        /// the number of bits used to represent each vertex coordinate.
        /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32
        /// </param>
        /// <param name="bitsPerComponent">
        /// the number of bits used to represent each colour component.
        /// The value shall be 1, 2, 4, 8, 12, or 16
        /// </param>
        /// <param name="verticesPerRow">
        /// the number of vertices in each row of the lattice (shall be &gt; 1).
        /// The number of rows need not be specified
        /// </param>
        /// <param name="decode">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfLatticeFormGouraudShadedTriangleShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
            , int verticesPerRow, PdfArray decode)
            : base(new PdfStream(), ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs) {
            SetBitsPerCoordinate(bitsPerCoordinate);
            SetBitsPerComponent(bitsPerComponent);
            SetVerticesPerRow(verticesPerRow);
            SetDecode(decode);
        }

        /// <summary>Gets the number of vertices in each row of the lattice.</summary>
        /// <returns>the number of vertices. Can only be greater than 1</returns>
        public virtual int GetVerticesPerRow() {
            return (int)GetPdfObject().GetAsInt(PdfName.VerticesPerRow);
        }

        /// <summary>Sets the number of vertices in each row of the lattice.</summary>
        /// <remarks>
        /// Sets the number of vertices in each row of the lattice.
        /// The number of rows need not be specified.
        /// </remarks>
        /// <param name="verticesPerRow">the number of vertices to be set. Shall be greater than 1</param>
        public void SetVerticesPerRow(int verticesPerRow) {
            GetPdfObject().Put(PdfName.VerticesPerRow, new PdfNumber(verticesPerRow));
            SetModified();
        }
    }
}
