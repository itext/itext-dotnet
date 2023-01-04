/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
    public class PdfDocumentInfo {
        internal static readonly PdfName[] PDF20_DEPRECATED_KEYS = new PdfName[] { PdfName.Title, PdfName.Author, 
            PdfName.Subject, PdfName.Keywords, PdfName.Creator, PdfName.Producer, PdfName.Trapped };

        private PdfDictionary infoDictionary;

        /// <summary>Create a PdfDocumentInfo based on the passed PdfDictionary.</summary>
        /// <param name="pdfObject">PdfDictionary containing PdfDocumentInfo</param>
        internal PdfDocumentInfo(PdfDictionary pdfObject, PdfDocument pdfDocument) {
            infoDictionary = pdfObject;
            if (pdfDocument.GetWriter() != null) {
                infoDictionary.MakeIndirect(pdfDocument);
            }
        }

        /// <summary>Create a default, empty PdfDocumentInfo and link it to the passed PdfDocument</summary>
        /// <param name="pdfDocument">document the info will belong to</param>
        internal PdfDocumentInfo(PdfDocument pdfDocument)
            : this(new PdfDictionary(), pdfDocument) {
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetTitle(String title) {
            return Put(PdfName.Title, new PdfString(title, PdfEncodings.UNICODE_BIG));
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetAuthor(String author) {
            return Put(PdfName.Author, new PdfString(author, PdfEncodings.UNICODE_BIG));
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetSubject(String subject) {
            return Put(PdfName.Subject, new PdfString(subject, PdfEncodings.UNICODE_BIG));
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetKeywords(String keywords) {
            return Put(PdfName.Keywords, new PdfString(keywords, PdfEncodings.UNICODE_BIG));
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetCreator(String creator) {
            return Put(PdfName.Creator, new PdfString(creator, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Sets a producer line for the
        /// <see cref="PdfDocument"/>
        /// described by this instance.
        /// </summary>
        /// <param name="producer">is a new producer line to set</param>
        /// <returns>this instance</returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetProducer(String producer) {
            GetPdfObject().Put(PdfName.Producer, new PdfString(producer, PdfEncodings.UNICODE_BIG));
            return this;
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetTrapped(PdfName trapped) {
            return Put(PdfName.Trapped, trapped);
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

        public virtual PdfName GetTrapped() {
            return infoDictionary.GetAsName(PdfName.Trapped);
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddCreationDate() {
            return Put(PdfName.CreationDate, new PdfDate().GetPdfObject());
        }

        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddModDate() {
            return Put(PdfName.ModDate, new PdfDate().GetPdfObject());
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
                infoDictionary.Remove(keyName);
                infoDictionary.SetModified();
            }
            else {
                Put(keyName, new PdfString(value, PdfEncodings.UNICODE_BIG));
            }
        }

        public virtual String GetMoreInfo(String key) {
            return GetStringValue(new PdfName(key));
        }

        internal virtual PdfDictionary GetPdfObject() {
            return infoDictionary;
        }

        internal virtual iText.Kernel.Pdf.PdfDocumentInfo Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            GetPdfObject().SetModified();
            return this;
        }

        private String GetStringValue(PdfName name) {
            PdfString pdfString = infoDictionary.GetAsString(name);
            return pdfString != null ? pdfString.ToUnicodeString() : null;
        }
    }
}
