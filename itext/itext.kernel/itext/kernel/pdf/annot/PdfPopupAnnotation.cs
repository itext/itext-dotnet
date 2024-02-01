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
    public class PdfPopupAnnotation : PdfAnnotation {
        protected internal PdfAnnotation parent;

        public PdfPopupAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfPopupAnnotation"/>
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
        protected internal PdfPopupAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Popup;
        }

        public virtual PdfDictionary GetParentObject() {
            return GetPdfObject().GetAsDictionary(PdfName.Parent);
        }

        public virtual PdfAnnotation GetParent() {
            if (parent == null) {
                parent = MakeAnnotation(GetParentObject());
            }
            return parent;
        }

        public virtual iText.Kernel.Pdf.Annot.PdfPopupAnnotation SetParent(PdfAnnotation parent) {
            this.parent = parent;
            return (iText.Kernel.Pdf.Annot.PdfPopupAnnotation)Put(PdfName.Parent, parent.GetPdfObject());
        }

        /// <summary>A flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// A flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <returns>true if annotation is initially open, false - if closed.</returns>
        public virtual bool GetOpen() {
            return PdfBoolean.TRUE.Equals(GetPdfObject().GetAsBoolean(PdfName.Open));
        }

        /// <summary>Sets a flag specifying whether the annotation shall initially be displayed open.</summary>
        /// <remarks>
        /// Sets a flag specifying whether the annotation shall initially be displayed open.
        /// This flag has affect to not all kinds of annotations.
        /// </remarks>
        /// <param name="open">true if annotation shall initially be open, false - if closed.</param>
        /// <returns>
        /// this
        /// <see cref="PdfPopupAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfPopupAnnotation SetOpen(bool open) {
            return (iText.Kernel.Pdf.Annot.PdfPopupAnnotation)Put(PdfName.Open, PdfBoolean.ValueOf(open));
        }
    }
}
