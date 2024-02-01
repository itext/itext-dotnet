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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Annot {
    public class PdfTextMarkupAnnotation : PdfMarkupAnnotation {
        /// <summary>Subtypes</summary>
        public static readonly PdfName MarkupHighlight = PdfName.Highlight;

        public static readonly PdfName MarkupUnderline = PdfName.Underline;

        public static readonly PdfName MarkupStrikeout = PdfName.StrikeOut;

        public static readonly PdfName MarkupSquiggly = PdfName.Squiggly;

        public PdfTextMarkupAnnotation(Rectangle rect, PdfName subtype, float[] quadPoints)
            : base(rect) {
            Put(PdfName.Subtype, subtype);
            SetQuadPoints(new PdfArray(quadPoints));
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// instance based on
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// instance, that represents existing annotation object in the document.
        /// </summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// representing annotation object
        /// </param>
        /// <seealso cref="PdfAnnotation.MakeAnnotation(iText.Kernel.Pdf.PdfObject)"/>
        protected internal PdfTextMarkupAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Creates a text markup annotation of highlight style subtype.</summary>
        /// <remarks>
        /// Creates a text markup annotation of highlight style subtype.
        /// <para />
        /// IMPORTANT NOTE on <b>quadPoints</b> argument:
        /// According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units.
        /// </param>
        /// <param name="quadPoints">
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Each quadrilateral shall encompasses a word or group of contiguous words in the text underlying
        /// the annotation. The text is oriented with respect to the edge connecting first two vertices.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// of Highlight type.
        /// </returns>
        public static iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation CreateHighLight(Rectangle rect, float[] quadPoints
            ) {
            return new iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation(rect, MarkupHighlight, quadPoints);
        }

        /// <summary>Creates a text markup annotation of underline style subtype.</summary>
        /// <remarks>
        /// Creates a text markup annotation of underline style subtype.
        /// <para />
        /// IMPORTANT NOTE on <b>quadPoints</b> argument:
        /// According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units.
        /// </param>
        /// <param name="quadPoints">
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Each quadrilateral shall encompasses a word or group of contiguous words in the text underlying
        /// the annotation. The text is oriented with respect to the edge connecting first two vertices.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// of Underline type.
        /// </returns>
        public static iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation CreateUnderline(Rectangle rect, float[] quadPoints
            ) {
            return new iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation(rect, MarkupUnderline, quadPoints);
        }

        /// <summary>Creates a text markup annotation of strikeout style subtype.</summary>
        /// <remarks>
        /// Creates a text markup annotation of strikeout style subtype.
        /// <para />
        /// IMPORTANT NOTE on <b>quadPoints</b> argument:
        /// According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units.
        /// </param>
        /// <param name="quadPoints">
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Each quadrilateral shall encompasses a word or group of contiguous words in the text underlying
        /// the annotation. The text is oriented with respect to the edge connecting first two vertices.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// of Strikeout type.
        /// </returns>
        public static iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation CreateStrikeout(Rectangle rect, float[] quadPoints
            ) {
            return new iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation(rect, MarkupStrikeout, quadPoints);
        }

        /// <summary>Creates a text markup annotation of squiggly-underline type.</summary>
        /// <remarks>
        /// Creates a text markup annotation of squiggly-underline type.
        /// <para />
        /// IMPORTANT NOTE on <b>quadPoints</b> argument:
        /// According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <param name="rect">
        /// the annotation rectangle, defining the location of the annotation on the page
        /// in default user space units.
        /// </param>
        /// <param name="quadPoints">
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Each quadrilateral shall encompasses a word or group of contiguous words in the text underlying
        /// the annotation. The text is oriented with respect to the edge connecting first two vertices.
        /// </param>
        /// <returns>
        /// created
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// of squiggly-underline type.
        /// </returns>
        public static iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation CreateSquiggly(Rectangle rect, float[] quadPoints
            ) {
            return new iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation(rect, MarkupSquiggly, quadPoints);
        }

        public override PdfName GetSubtype() {
            PdfName subType = GetPdfObject().GetAsName(PdfName.Subtype);
            if (subType == null) {
                subType = MarkupUnderline;
            }
            return subType;
        }

        /// <summary>An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.</summary>
        /// <remarks>
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Quadrilaterals are used to define a word or group of contiguous words in the text
        /// underlying the text markup annotation.
        /// <para />
        /// IMPORTANT NOTE: According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </returns>
        public virtual PdfArray GetQuadPoints() {
            return GetPdfObject().GetAsArray(PdfName.QuadPoints);
        }

        /// <summary>
        /// Sets n quadrilaterals in default user space by passing an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers.
        /// </summary>
        /// <remarks>
        /// Sets n quadrilaterals in default user space by passing an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers.
        /// Quadrilaterals are used to define a word or group of contiguous words in the text
        /// underlying the text markup annotation.
        /// <para />
        /// IMPORTANT NOTE: According to Table 179 in ISO 32000-1, the QuadPoints array lists the vertices in counterclockwise
        /// order and the text orientation is defined by the first and second vertex. This basically means QuadPoints is
        /// specified as lower-left, lower-right, top-right, top-left. HOWEVER, Adobe's interpretation
        /// (tested at least with Acrobat 10, Acrobat 11, Reader 11) is top-left, top-right, lower-left, lower-right (Z-shaped order).
        /// This means that if the QuadPoints array is specified according to the standard, the rendering is not as expected.
        /// Other viewers seem to follow Adobe's interpretation. Hence we recommend to use and expect QuadPoints array in Z-order,
        /// just as Acrobat and probably most other viewers expect.
        /// </remarks>
        /// <param name="quadPoints">
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfTextMarkupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation SetQuadPoints(PdfArray quadPoints) {
            return (iText.Kernel.Pdf.Annot.PdfTextMarkupAnnotation)Put(PdfName.QuadPoints, quadPoints);
        }
    }
}
