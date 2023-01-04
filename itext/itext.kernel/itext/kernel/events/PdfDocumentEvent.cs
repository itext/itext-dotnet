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
using iText.Kernel.Pdf;

namespace iText.Kernel.Events {
    /// <summary>Event dispatched by PdfDocument.</summary>
    public class PdfDocumentEvent : Event {
        /// <summary>Dispatched after page is created.</summary>
        public const String START_PAGE = "StartPdfPage";

        /// <summary>Dispatched after page is inserted/added into a document.</summary>
        public const String INSERT_PAGE = "InsertPdfPage";

        /// <summary>Dispatched after page is removed from a document.</summary>
        public const String REMOVE_PAGE = "RemovePdfPage";

        /// <summary>Dispatched before page is flushed to a document.</summary>
        /// <remarks>
        /// Dispatched before page is flushed to a document.
        /// This event isn't necessarily dispatched when a successive page has been created.
        /// Keep it in mind when using with highlevel iText API.
        /// </remarks>
        public const String END_PAGE = "EndPdfPage";

        /// <summary>The PdfPage associated with this event.</summary>
        protected internal PdfPage page;

        /// <summary>The PdfDocument associated with this event.</summary>
        private PdfDocument document;

        /// <summary>Creates a PdfDocumentEvent.</summary>
        /// <param name="type">type of the event that fired this event</param>
        /// <param name="document">document that fired this event</param>
        public PdfDocumentEvent(String type, PdfDocument document)
            : base(type) {
            this.document = document;
        }

        /// <summary>Creates a PdfDocumentEvent.</summary>
        /// <param name="type">type of the event that fired this event</param>
        /// <param name="page">page that fired this event</param>
        public PdfDocumentEvent(String type, PdfPage page)
            : base(type) {
            this.page = page;
            this.document = page.GetDocument();
        }

        /// <summary>Returns the PdfDocument associated with this event.</summary>
        /// <returns>the PdfDocument associated with this event</returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Returns the PdfPage associated with this event.</summary>
        /// <remarks>Returns the PdfPage associated with this event. Warning: this can be null.</remarks>
        /// <returns>the PdfPage associated with this event</returns>
        public virtual PdfPage GetPage() {
            return page;
        }
    }
}
