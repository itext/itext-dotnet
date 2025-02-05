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
using System;
using System.Collections.Generic;
using iText.IO.Font;

namespace iText.Kernel.Pdf {
    public class PdfDocumentInfo {
//\cond DO_NOT_DOCUMENT
        internal static readonly PdfName[] PDF20_DEPRECATED_KEYS = new PdfName[] { PdfName.Title, PdfName.Author, 
            PdfName.Subject, PdfName.Keywords, PdfName.Creator, PdfName.Producer, PdfName.Trapped };
//\endcond

        private readonly PdfDictionary infoDictionary;

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a PdfDocumentInfo based on the passed PdfDictionary.</summary>
        /// <param name="pdfObject">PdfDictionary containing PdfDocumentInfo</param>
        internal PdfDocumentInfo(PdfDictionary pdfObject, PdfDocument pdfDocument) {
            infoDictionary = pdfObject;
            if (pdfDocument.GetWriter() != null) {
                infoDictionary.MakeIndirect(pdfDocument);
            }
        }
//\endcond

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

        /// <summary>Remove creation date from the document info dictionary.</summary>
        /// <returns>this instance.</returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo RemoveCreationDate() {
            infoDictionary.Remove(PdfName.CreationDate);
            return this;
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

//\cond DO_NOT_DOCUMENT
        internal virtual PdfDictionary GetPdfObject() {
            return infoDictionary;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual iText.Kernel.Pdf.PdfDocumentInfo Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            GetPdfObject().SetModified();
            return this;
        }
//\endcond

        private String GetStringValue(PdfName name) {
            PdfString pdfString = infoDictionary.GetAsString(name);
            return pdfString != null ? pdfString.ToUnicodeString() : null;
        }
    }
}
