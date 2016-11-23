/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System;
using System.Collections.Generic;
using iText.IO.Font;

namespace iText.Kernel.Pdf {
    public class PdfDocumentInfo : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Create a PdfDocumentInfo based on the passed PdfDictionary and linked to the passed PdfDocument.</summary>
        /// <param name="pdfObject">PdfDictionary containing PdfDocumentInfo</param>
        /// <param name="pdfDocument">PdfDocument the PdfDocumentInfo corresponds to.</param>
        public PdfDocumentInfo(PdfDictionary pdfObject, PdfDocument pdfDocument)
            : base(pdfObject == null ? new PdfDictionary() : pdfObject) {
            if (pdfDocument.GetWriter() != null) {
                this.GetPdfObject().MakeIndirect(pdfDocument);
            }
            SetForbidRelease();
        }

        public PdfDocumentInfo(PdfDictionary pdfObject)
            : this(pdfObject, null) {
        }

        /// <summary>Create a default, empty PdfDocumentInfo and link it to the passed PdfDocument</summary>
        /// <param name="pdfDocument"/>
        public PdfDocumentInfo(PdfDocument pdfDocument)
            : this(new PdfDictionary(), pdfDocument) {
        }

        //Samuel: Wouldn't this raise a nullpointer exception?
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetTitle(String title) {
            GetPdfObject().Put(PdfName.Title, new PdfString(title, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetAuthor(String author) {
            GetPdfObject().Put(PdfName.Author, new PdfString(author, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetSubject(String subject) {
            GetPdfObject().Put(PdfName.Subject, new PdfString(subject, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetKeywords(String keywords) {
            GetPdfObject().Put(PdfName.Keywords, new PdfString(keywords, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetCreator(String creator) {
            GetPdfObject().Put(PdfName.Creator, new PdfString(creator, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual String GetTitle() {
            return GetStringValue(PdfName.Title);
        }

        public virtual String GetAuthor() {
            return GetStringValue(PdfName.Author);
        }

        public virtual String GetSubject() {
            return GetStringValue(PdfName.Subject);
        }

        public virtual String GetKeywords() {
            return GetStringValue(PdfName.Keywords);
        }

        public virtual String GetCreator() {
            return GetStringValue(PdfName.Creator);
        }

        public virtual String GetProducer() {
            return GetStringValue(PdfName.Producer);
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddCreationDate() {
            this.GetPdfObject().Put(PdfName.CreationDate, new PdfDate().GetPdfObject());
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddModDate() {
            this.GetPdfObject().Put(PdfName.ModDate, new PdfDate().GetPdfObject());
            return this;
        }

        public virtual void SetMoreInfo(IDictionary<String, String> moreInfo) {
            if (moreInfo != null) {
                foreach (KeyValuePair<String, String> entry in moreInfo) {
                    String key = entry.Key;
                    String value = entry.Value;
                    SetMoreInfo(key, value);
                }
            }
        }

        public virtual void SetMoreInfo(String key, String value) {
            PdfName keyName = new PdfName(key);
            if (value == null) {
                GetPdfObject().Remove(keyName);
            }
            else {
                GetPdfObject().Put(keyName, new PdfString(value, PdfEncodings.UNICODE_BIG));
            }
        }

        public override void Flush() {
            GetPdfObject().Flush(false);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private String GetStringValue(PdfName name) {
            PdfString pdfString = GetPdfObject().GetAsString(name);
            return pdfString != null ? pdfString.ToUnicodeString() : null;
        }
    }
}
