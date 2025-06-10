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
    /// <summary>
    /// The class is a wrapper around
    /// <c>Info</c>
    /// dictionary from
    /// <c>PdfDocument</c>
    /// root which provides utility methods to work with the
    /// <c>Info</c>
    /// dictionary.
    /// </summary>
    /// <remarks>
    /// The class is a wrapper around
    /// <c>Info</c>
    /// dictionary from
    /// <c>PdfDocument</c>
    /// root which provides utility methods to work with the
    /// <c>Info</c>
    /// dictionary.
    /// <para />
    /// For more information about each of the PDF document info key, see ISO 32000-2 Table 349.
    /// </remarks>
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

        /// <summary>
        /// Sets title of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="title">the title to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetTitle(String title) {
            return Put(PdfName.Title, new PdfString(title, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Sets the author of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="author">the author to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetAuthor(String author) {
            return Put(PdfName.Author, new PdfString(author, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Sets the subject of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="subject">the subject to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetSubject(String subject) {
            return Put(PdfName.Subject, new PdfString(subject, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Sets the keywords of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="keywords">the keywords to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetKeywords(String keywords) {
            return Put(PdfName.Keywords, new PdfString(keywords, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>
        /// Sets the creator of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="creator">the creator to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
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

        /// <summary>
        /// Sets the trapped of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <remarks>
        /// Sets the trapped of the
        /// <c>PdfDocument</c>.
        /// <para />
        /// The value indicates whether the document has been modified to include trapping information or not.
        /// </remarks>
        /// <param name="trapped">trapped to set</param>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo SetTrapped(PdfName trapped) {
            return Put(PdfName.Trapped, trapped);
        }

        /// <summary>
        /// Gets the title of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the title</returns>
        public virtual String GetTitle() {
            return GetStringValue(PdfName.Title);
        }

        /// <summary>
        /// Gets the author of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the author</returns>
        public virtual String GetAuthor() {
            return GetStringValue(PdfName.Author);
        }

        /// <summary>
        /// Gets the subject of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the subject</returns>
        public virtual String GetSubject() {
            return GetStringValue(PdfName.Subject);
        }

        /// <summary>
        /// Gets the keywords of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the keywords</returns>
        public virtual String GetKeywords() {
            return GetStringValue(PdfName.Keywords);
        }

        /// <summary>
        /// Gets the creator of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the creator</returns>
        public virtual String GetCreator() {
            return GetStringValue(PdfName.Creator);
        }

        /// <summary>
        /// Gets the producer of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>the producer</returns>
        public virtual String GetProducer() {
            return GetStringValue(PdfName.Producer);
        }

        /// <summary>
        /// Gets the trapped of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <remarks>
        /// Gets the trapped of the
        /// <c>PdfDocument</c>.
        /// <para />
        /// The value indicates whether the document has been modified to include trapping information or not.
        /// </remarks>
        /// <returns>the trapped</returns>
        public virtual PdfName GetTrapped() {
            return infoDictionary.GetAsName(PdfName.Trapped);
        }

        /// <summary>
        /// Adds the creation date of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddCreationDate() {
            return Put(PdfName.CreationDate, new PdfDate().GetPdfObject());
        }

        /// <summary>Remove creation date from the document info dictionary.</summary>
        /// <returns>this instance.</returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo RemoveCreationDate() {
            infoDictionary.Remove(PdfName.CreationDate);
            return this;
        }

        /// <summary>
        /// Adds modification date of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <returns>
        /// the current
        /// <c>PdfDocumentInfo</c>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfDocumentInfo AddModDate() {
            return Put(PdfName.ModDate, new PdfDate().GetPdfObject());
        }

        /// <summary>
        /// Sets custom keys and values into
        /// <c>Info</c>
        /// dictionary of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="moreInfo">the map of keys and values to be set</param>
        public virtual void SetMoreInfo(IDictionary<String, String> moreInfo) {
            if (moreInfo != null) {
                foreach (KeyValuePair<String, String> entry in moreInfo) {
                    String key = entry.Key;
                    String value = entry.Value;
                    SetMoreInfo(key, value);
                }
            }
        }

        /// <summary>
        /// Sets custom key and value into
        /// <c>Info</c>
        /// dictionary of the
        /// <c>PdfDocument</c>.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
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

        /// <summary>
        /// Gets the value of additional key of
        /// <c>Info</c>
        /// dictionary.
        /// </summary>
        /// <param name="key">the key to get value for</param>
        /// <returns>
        /// the value or
        /// <see langword="null"/>
        /// if there is no value for such a key
        /// </returns>
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
