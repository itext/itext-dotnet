/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Fields {
    /// <summary>An AcroForm field containing signature data.</summary>
    public class PdfSignatureFormField : PdfFormField {
        protected internal PdfSignatureFormField(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        protected internal PdfSignatureFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : base(widget, pdfDocument) {
        }

        protected internal PdfSignatureFormField(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>Returns <c>Sig</c>, the form type for signature form fields.</summary>
        /// <returns>
        /// the form type, as a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// </returns>
        public override PdfName GetFormType() {
            return PdfName.Sig;
        }

        /// <summary>Adds the signature to the signature field.</summary>
        /// <param name="value">the signature to be contained in the signature field, or an indirect reference to it</param>
        /// <returns>the edited field</returns>
        public virtual iText.Forms.Fields.PdfSignatureFormField SetValue(PdfObject value) {
            Put(PdfName.V, value);
            return this;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Forms.PdfSigFieldLock"/>
        /// , which contains fields that
        /// must be locked if the document is signed.
        /// </summary>
        /// <returns>a dictionary containing locked fields.</returns>
        /// <seealso cref="iText.Forms.PdfSigFieldLock"/>
        public virtual PdfSigFieldLock GetSigFieldLockDictionary() {
            PdfDictionary sigLockDict = (PdfDictionary)GetPdfObject().Get(PdfName.Lock);
            return sigLockDict == null ? null : new PdfSigFieldLock(sigLockDict);
        }
    }
}
