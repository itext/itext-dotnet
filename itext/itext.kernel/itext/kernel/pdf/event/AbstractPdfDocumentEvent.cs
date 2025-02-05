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
using iText.Commons.Actions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Event {
    /// <summary>Describes abstract PDF document event of the specified type.</summary>
    /// <remarks>
    /// Describes abstract PDF document event of the specified type.
    /// <para />
    /// Use
    /// <see cref="iText.Kernel.Pdf.PdfDocument.DispatchEvent(AbstractPdfDocumentEvent)"/>
    /// to fire an event
    /// and
    /// <see cref="iText.Kernel.Pdf.PdfDocument.AddEventHandler(System.String, AbstractPdfDocumentEventHandler)"/>
    /// to register
    /// <see cref="AbstractPdfDocumentEventHandler"/>
    /// handler for that type of event.
    /// </remarks>
    public abstract class AbstractPdfDocumentEvent : IEvent {
        /// <summary>A type of event.</summary>
        protected internal String type;

        private PdfDocument document;

        /// <summary>Creates an event of the specified type.</summary>
        /// <param name="type">the type of event</param>
        protected internal AbstractPdfDocumentEvent(String type) {
            this.type = type;
        }

        /// <summary>Returns the type of this event.</summary>
        /// <returns>type of this event</returns>
        public virtual String GetType() {
            return type;
        }

        /// <summary>Retrieves the document associated with this event.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that triggered this event
        /// </returns>
        public virtual PdfDocument GetDocument() {
            return document;
        }

        /// <summary>Sets the document associated with this event.</summary>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that triggered this event
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AbstractPdfDocumentEvent"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Event.AbstractPdfDocumentEvent SetDocument(PdfDocument document) {
            this.document = document;
            return this;
        }
    }
}
