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
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;

namespace iText.Forms.Fields {
    /// <summary>An AcroForm field containing signature data.</summary>
    public class PdfSignatureFormField : PdfFormField {
        /// <summary>Indicates if we need to reuse the existing appearance as a background layer.</summary>
        private bool reuseAppearance = false;

        /// <summary>Indicates if we need to ignore page rotation for the signature field annotation.</summary>
        private bool ignorePageRotation = true;

        /// <summary>Background level of the signature appearance.</summary>
        private PdfFormXObject n0;

        /// <summary>Signature appearance layer that contains information about the signature.</summary>
        private PdfFormXObject n2;

        /// <summary>
        /// Creates a minimal
        /// <see cref="PdfSignatureFormField"/>.
        /// </summary>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfSignatureFormField(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        /// <summary>
        /// Creates a signature form field as a parent of a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfWidgetAnnotation"/>.
        /// </summary>
        /// <param name="widget">
        /// The widget which will be a kid of the
        /// <see cref="PdfSignatureFormField"/>.
        /// </param>
        /// <param name="pdfDocument">
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance.
        /// </param>
        protected internal PdfSignatureFormField(PdfWidgetAnnotation widget, PdfDocument pdfDocument)
            : base(widget, pdfDocument) {
        }

        /// <summary>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// </summary>
        /// <remarks>
        /// Creates a signature form field as a wrapper object around a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>.
        /// This
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// must be an indirect object.
        /// </remarks>
        /// <param name="pdfObject">the dictionary to be wrapped, must have an indirect reference.</param>
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

        /// <summary>Sets the background layer that is present when creating the signature field.</summary>
        /// <param name="n0">layer xObject.</param>
        /// <returns>
        /// this same
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Fields.PdfSignatureFormField SetBackgroundLayer(PdfFormXObject n0) {
            this.n0 = n0;
            RegenerateField();
            return this;
        }

        /// <summary>
        /// Sets the signature appearance layer that contains information about the signature, e.g. the line art for the
        /// handwritten signature, the text giving the signer’s name, date, reason, location and so on.
        /// </summary>
        /// <param name="n2">layer xObject.</param>
        /// <returns>
        /// this same
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Fields.PdfSignatureFormField SetSignatureAppearanceLayer(PdfFormXObject n2) {
            this.n2 = n2;
            RegenerateField();
            return this;
        }

        /// <summary>Indicates that the existing appearances needs to be reused as a background.</summary>
        /// <param name="reuseAppearance">is an appearances reusing flag value to set.</param>
        /// <returns>
        /// this same
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Fields.PdfSignatureFormField SetReuseAppearance(bool reuseAppearance) {
            this.reuseAppearance = reuseAppearance;
            return this;
        }

        /// <summary>Sets the boolean value which indicates if page rotation should be ignored for the signature appearance.
        ///     </summary>
        /// <remarks>
        /// Sets the boolean value which indicates if page rotation should be ignored for the signature appearance.
        /// <para />
        /// Default value is
        /// <see langword="true"/>.
        /// </remarks>
        /// <param name="ignore">boolean value to set.</param>
        /// <returns>
        /// this same
        /// <see cref="PdfSignatureFormField"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Fields.PdfSignatureFormField SetIgnorePageRotation(bool ignore) {
            this.ignorePageRotation = ignore;
            return this;
        }

        /// <summary>Gets the background layer that is present when creating the signature field if it was set.</summary>
        /// <returns>n0 layer xObject.</returns>
        internal virtual PdfFormXObject GetBackgroundLayer() {
            return n0;
        }

        /// <summary>Gets the signature appearance layer that contains information about the signature if it was set.</summary>
        /// <returns>n2 layer xObject.</returns>
        internal virtual PdfFormXObject GetSignatureAppearanceLayer() {
            return n2;
        }

        /// <summary>Indicates if the existing appearances needs to be reused as a background.</summary>
        /// <returns>appearances reusing flag value.</returns>
        internal virtual bool IsReuseAppearance() {
            return reuseAppearance;
        }

        /// <summary>Indicates if page rotation should be ignored for the signature appearance.</summary>
        /// <returns>the boolean value which indicates if we need to ignore page rotation for the signature appearance.
        ///     </returns>
        internal virtual bool IsPageRotationIgnored() {
            return this.ignorePageRotation;
        }
    }
}
