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
using iText.Kernel.Pdf.Annot.DA;

namespace iText.Kernel.Pdf.Annot {
    public class PdfRedactAnnotation : PdfMarkupAnnotation {
        public PdfRedactAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfRedactAnnotation"/>
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
        protected internal PdfRedactAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Redact;
        }

        /// <summary>The default appearance string that shall be used in formatting the text.</summary>
        /// <remarks>The default appearance string that shall be used in formatting the text. See ISO-32001 12.7.3.3, "Variable Text".
        ///     </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// that specifies the default appearance, or null if default appereance is not specified.
        /// </returns>
        public virtual PdfString GetDefaultAppearance() {
            return GetPdfObject().GetAsString(PdfName.DA);
        }

        /// <summary>The default appearance string that shall be used in formatting the text.</summary>
        /// <remarks>The default appearance string that shall be used in formatting the text. See ISO-32001 12.7.3.3, "Variable Text".
        ///     </remarks>
        /// <param name="appearanceString">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// that specifies the default appearance.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfMarkupAnnotation"/>
        /// instance.+
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetDefaultAppearance(PdfString appearanceString) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.DA, appearanceString);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetDefaultAppearance(AnnotationDefaultAppearance
             da) {
            return SetDefaultAppearance(da.ToPdfString());
        }

        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetOverlayText(PdfString text) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.OverlayText, text);
        }

        public virtual PdfString GetOverlayText() {
            return GetPdfObject().GetAsString(PdfName.OverlayText);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetRedactRolloverAppearance(PdfStream stream) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.RO, stream);
        }

        public virtual PdfStream GetRedactRolloverAppearance() {
            return GetPdfObject().GetAsStream(PdfName.RO);
        }

        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetRepeat(PdfBoolean repeat) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.Repeat, repeat);
        }

        public virtual PdfBoolean GetRepeat() {
            return GetPdfObject().GetAsBoolean(PdfName.Repeat);
        }

        /// <summary>An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.</summary>
        /// <remarks>
        /// An array of 8 × n numbers specifying the coordinates of n quadrilaterals in default user space.
        /// Quadrilaterals are used to define the content region that is intended to be removed for a redaction annotation.
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
        /// Quadrilaterals are used to define the content region that is intended to be removed for a redaction annotation.
        /// </remarks>
        /// <param name="quadPoints">
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of 8 × n numbers specifying the coordinates of n quadrilaterals.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfRedactAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetQuadPoints(PdfArray quadPoints) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.QuadPoints, quadPoints);
        }

        /// <summary>The interior color which is used to fill the redacted region after the affected content has been removed.
        ///     </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// of either
        /// <see cref="iText.Kernel.Colors.DeviceGray"/>
        /// ,
        /// <see cref="iText.Kernel.Colors.DeviceRgb"/>
        /// or
        /// <see cref="iText.Kernel.Colors.DeviceCmyk"/>
        /// type which defines
        /// interior color of the annotation, or null if interior color is not specified.
        /// </returns>
        public virtual Color GetInteriorColor() {
            return InteriorColorUtil.ParseInteriorColor(GetPdfObject().GetAsArray(PdfName.IC));
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color which
        /// is used to fill the redacted region after the affected content has been removed.
        /// </summary>
        /// <param name="interiorColor">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// of numbers in the range 0.0 to 1.0. The number of array elements determines
        /// the colour space in which the colour is defined: 0 - No colour, transparent; 1 - DeviceGray,
        /// 3 - DeviceRGB, 4 - DeviceCMYK. For the
        /// <see cref="PdfRedactAnnotation"/>
        /// number of elements shall be
        /// equal to 3 (which defines DeviceRGB colour space).
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfRedactAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetInteriorColor(PdfArray interiorColor) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.IC, interiorColor);
        }

        /// <summary>
        /// An array of numbers in the range 0.0 to 1.0 specifying the interior color which
        /// is used to fill the redacted region after the affected content has been removed.
        /// </summary>
        /// <param name="interiorColor">an array of floats in the range 0.0 to 1.0.</param>
        /// <returns>
        /// this
        /// <see cref="PdfRedactAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetInteriorColor(float[] interiorColor) {
            return SetInteriorColor(new PdfArray(interiorColor));
        }

        /// <summary>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified.
        /// </summary>
        /// <remarks>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified. Default value: 0 (left-justified).
        /// </remarks>
        /// <returns>a code specifying the form of quadding (justification), returns the default value if not explicitly specified.
        ///     </returns>
        public virtual int GetJustification() {
            PdfNumber q = GetPdfObject().GetAsNumber(PdfName.Q);
            return q == null ? 0 : q.IntValue();
        }

        /// <summary>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified.
        /// </summary>
        /// <remarks>
        /// A code specifying the form of quadding (justification) that is used in displaying the annotation's text:
        /// 0 - Left-justified, 1 - Centered, 2 - Right-justified. Default value: 0 (left-justified).
        /// </remarks>
        /// <param name="justification">a code specifying the form of quadding (justification).</param>
        /// <returns>
        /// this
        /// <see cref="PdfRedactAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfRedactAnnotation SetJustification(int justification) {
            return (iText.Kernel.Pdf.Annot.PdfRedactAnnotation)Put(PdfName.Q, new PdfNumber(justification));
        }
    }
}
