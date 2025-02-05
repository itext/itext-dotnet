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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Event {
    /// <summary>Event dispatched by PdfDocument.</summary>
    public class PdfDocumentEvent : AbstractPdfDocumentEvent {
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

        /// <summary>Dispatched before writer is closed.</summary>
        public const String START_WRITER_CLOSING = "StartWriterClosing";

        /// <summary>Dispatched after writer is flushed to a document.</summary>
        public const String START_DOCUMENT_CLOSING = "StartDocumentClosing";

        /// <summary>The PdfPage associated with this event.</summary>
        protected internal PdfPage page;

        /// <summary>Creates a PdfDocumentEvent.</summary>
        /// <param name="type">type of the event that fired this event</param>
        public PdfDocumentEvent(String type)
            : base(type) {
        }

        /// <summary>Creates a PdfDocumentEvent.</summary>
        /// <param name="type">type of the event that fired this event</param>
        /// <param name="page">page that fired this event</param>
        public PdfDocumentEvent(String type, PdfPage page)
            : base(type) {
            this.page = page;
        }

        /// <summary>Returns the PdfPage associated with this event.</summary>
        /// <remarks>Returns the PdfPage associated with this event. Warning: this can be null.</remarks>
        /// <returns>the PdfPage associated with this event</returns>
        public virtual PdfPage GetPage() {
            return page;
        }
    }
}
