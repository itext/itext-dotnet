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
    public class PdfCaretAnnotation : PdfMarkupAnnotation {
        public PdfCaretAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfCaretAnnotation"/>
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
        protected internal PdfCaretAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Caret;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfCaretAnnotation SetSymbol(PdfString symbol) {
            return (iText.Kernel.Pdf.Annot.PdfCaretAnnotation)Put(PdfName.Sy, symbol);
        }

        public virtual PdfString GetSymbol() {
            return GetPdfObject().GetAsString(PdfName.Sy);
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the actual boundaries of the underlying caret.
        /// </summary>
        /// <returns>
        /// null if not specified, otherwise a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// with four numbers which correspond to the
        /// differences in default user space between the left, top, right, and bottom coordinates of Rect and those
        /// of the inner rectangle, respectively.
        /// </returns>
        public virtual PdfArray GetRectangleDifferences() {
            return GetPdfObject().GetAsArray(PdfName.RD);
        }

        /// <summary>
        /// A set of four numbers describing the numerical differences between two rectangles:
        /// the Rect entry of the annotation and the actual boundaries of the underlying caret.
        /// </summary>
        /// <param name="rect">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// with four numbers which correspond to the differences in default user space between
        /// the left, top, right, and bottom coordinates of Rect and those of the inner rectangle, respectively.
        /// Each value shall be greater than or equal to 0. The sum of the top and bottom differences shall be
        /// less than the height of Rect, and the sum of the left and right differences shall be less than
        /// the width of Rect.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfCaretAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfCaretAnnotation SetRectangleDifferences(PdfArray rect) {
            return (iText.Kernel.Pdf.Annot.PdfCaretAnnotation)Put(PdfName.RD, rect);
        }
    }
}
