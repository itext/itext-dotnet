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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;

namespace iText.Kernel.Pdf.Annot {
    public class PdfScreenAnnotation : PdfAnnotation {
        public PdfScreenAnnotation(Rectangle rect)
            : base(rect) {
        }

        /// <summary>
        /// Instantiates a new
        /// <see cref="PdfScreenAnnotation"/>
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
        protected internal PdfScreenAnnotation(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        public override PdfName GetSubtype() {
            return PdfName.Screen;
        }

        /// <summary>
        /// An
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to perform, such as launching an application, playing a sound,
        /// changing an annotation’s appearance state etc, when the annotation is activated.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which defines the characteristics and behaviour of an action.
        /// </returns>
        public virtual PdfDictionary GetAction() {
            return GetPdfObject().GetAsDictionary(PdfName.A);
        }

        /// <summary>
        /// Sets a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed when the annotation is activated.
        /// </summary>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAction(PdfAction action) {
            return (iText.Kernel.Pdf.Annot.PdfScreenAnnotation)Put(PdfName.A, action.GetPdfObject());
        }

        /// <summary>An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        ///     </summary>
        /// <remarks>
        /// An additional actions dictionary that extends the set of events that can trigger the execution of an action.
        /// See ISO-320001 12.6.3 Trigger Events.
        /// </remarks>
        /// <returns>
        /// an additional actions
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </returns>
        /// <seealso cref="GetAction()"/>
        public virtual PdfDictionary GetAdditionalAction() {
            return GetPdfObject().GetAsDictionary(PdfName.AA);
        }

        /// <summary>
        /// Sets an additional
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed in response to
        /// the specific trigger event defined by
        /// <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// Sets an additional
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to this annotation which will be performed in response to
        /// the specific trigger event defined by
        /// <paramref name="key"/>
        /// . See ISO-320001 12.6.3, "Trigger Events".
        /// </remarks>
        /// <param name="key">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that denotes a type of the additional action to set.
        /// </param>
        /// <param name="action">
        /// 
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// to set as additional to this annotation.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAdditionalAction(PdfName key, PdfAction action
            ) {
            PdfAction.SetAdditionalAction(this, key, action);
            return this;
        }

        /// <summary>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// An appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// </remarks>
        /// <returns>an appearance characteristics dictionary or null if it isn't specified.</returns>
        public virtual PdfDictionary GetAppearanceCharacteristics() {
            return GetPdfObject().GetAsDictionary(PdfName.MK);
        }

        /// <summary>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream.
        /// </summary>
        /// <remarks>
        /// Sets an appearance characteristics dictionary containing additional information for constructing the
        /// annotation’s appearance stream. See ISO-320001, Table 189.
        /// </remarks>
        /// <param name="characteristics">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// with additional information for appearance stream.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfScreenAnnotation"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Annot.PdfScreenAnnotation SetAppearanceCharacteristics(PdfDictionary characteristics
            ) {
            return (iText.Kernel.Pdf.Annot.PdfScreenAnnotation)Put(PdfName.MK, characteristics);
        }
    }
}
