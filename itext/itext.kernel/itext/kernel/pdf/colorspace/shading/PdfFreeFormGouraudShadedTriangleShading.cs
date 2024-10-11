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
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// ,
    /// <see cref="AbstractPdfShadingMesh"/>
    /// and
    /// <see cref="AbstractPdfShadingMeshWithFlags"/>
    /// classes and is in charge of Shading Dictionary
    /// with free-form Gouraud-shaded triangle mesh type.
    /// </summary>
    /// <remarks>
    /// The class that extends
    /// <see cref="AbstractPdfShading"/>
    /// ,
    /// <see cref="AbstractPdfShadingMesh"/>
    /// and
    /// <see cref="AbstractPdfShadingMeshWithFlags"/>
    /// classes and is in charge of Shading Dictionary
    /// with free-form Gouraud-shaded triangle mesh type.
    /// <para />
    /// The area to be shaded is defined by a path composed entirely of triangles.
    /// The colour at each vertex of the triangles is specified,
    /// and a technique known as Gouraud interpolation is used to colour the interiors.
    /// <para />
    /// The object shall be represented as stream containing a sequence of vertex data.
    /// Each vertex is specified by the following values, in the order shown:
    /// f x y c1 ... cn where:
    /// f -  the vertex's edge flag, that determines the vertex is connected to other vertices of the triangle mesh.
    /// For full description, see ISO-320001 Paragraph 8.7.4.5.5
    /// x, y - vertex's horizontal and vertical coordinates, expressed in the shading's target coordinate space.
    /// c1...cn - vertex's colour components.
    /// <para />
    /// If the shading dictionary includes a Function entry, only a single parametric value, t,
    /// shall be specified for each vertex in place of the colour components c1...cn.
    /// </remarks>
    public class PdfFreeFormGouraudShadedTriangleShading : AbstractPdfShadingMeshWithFlags {
        /// <summary>
        /// Creates the new instance of the class from the existing
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <param name="pdfStream">
        /// from which this
        /// <see cref="PdfFreeFormGouraudShadedTriangleShading"/>
        /// will be created
        /// </param>
        public PdfFreeFormGouraudShadedTriangleShading(PdfStream pdfStream)
            : base(pdfStream) {
        }

        /// <summary>Creates the new instance of the class.</summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object in which colour values shall be expressed.
        /// The special Pattern space isn't excepted.
        /// </param>
        /// <param name="bitsPerCoordinate">
        /// the number of bits used to represent each vertex coordinate.
        /// The value shall be 1, 2, 4, 8, 12, 16, 24, or 32
        /// </param>
        /// <param name="bitsPerComponent">
        /// the number of bits used to represent each colour component.
        /// The value shall be 1, 2, 4, 8, 12, or 16
        /// </param>
        /// <param name="bitsPerFlag">
        /// the number of bits used to represent the edge flag for each vertex.
        /// The value of BitsPerFlag shall be 2, 4, or 8,
        /// but only the least significant 2 bits in each flag value shall be used.
        /// The value for the edge flag shall be 0, 1, or 2
        /// </param>
        /// <param name="decode">
        /// the
        /// <c>int[]</c>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfFreeFormGouraudShadedTriangleShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
            , int bitsPerFlag, float[] decode)
            : this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)) {
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
        /// <param name="bitsPerFlag">
        /// the number of bits used to represent the edge flag for each vertex.
        /// The value of BitsPerFlag shall be 2, 4, or 8,
        /// but only the least significant 2 bits in each flag value shall be used.
        /// The value for the edge flag shall be 0, 1, or 2
        /// </param>
        /// <param name="decode">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers specifying how to map vertex coordinates and colour components
        /// into the appropriate ranges of values. The ranges shall be specified as follows:
        /// [x_min x_max y_min y_max c1_min c1_max … cn_min cn_max].
        /// Only one pair of color values shall be specified if a Function entry is present
        /// </param>
        public PdfFreeFormGouraudShadedTriangleShading(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
            , int bitsPerFlag, PdfArray decode)
            : base(new PdfStream(), ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH, cs) {
            SetBitsPerCoordinate(bitsPerCoordinate);
            SetBitsPerComponent(bitsPerComponent);
            SetBitsPerFlag(bitsPerFlag);
            SetDecode(decode);
        }
    }
}
