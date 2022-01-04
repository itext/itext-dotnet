/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            return (iText.Forms.Fields.PdfSignatureFormField)Put(PdfName.V, value);
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
