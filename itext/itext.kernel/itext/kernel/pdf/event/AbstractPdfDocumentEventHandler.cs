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
using iText.Commons.Actions;

namespace iText.Kernel.Pdf.Event {
    /// <summary>Base class for PDF document events handling based on the event type.</summary>
    /// <remarks>
    /// Base class for PDF document events handling based on the event type.
    /// <para />
    /// Handles
    /// <see cref="AbstractPdfDocumentEvent"/>
    /// event fired by
    /// <see cref="iText.Kernel.Pdf.PdfDocument.DispatchEvent(AbstractPdfDocumentEvent)"/>.
    /// Use
    /// <see cref="iText.Kernel.Pdf.PdfDocument.AddEventHandler(System.String, AbstractPdfDocumentEventHandler)"/>
    /// to register this handler for
    /// specific type of event.
    /// </remarks>
    public abstract class AbstractPdfDocumentEventHandler : IEventHandler {
        private readonly ICollection<String> types = new HashSet<String>();

        /// <summary>
        /// Creates new
        /// <see cref="AbstractPdfDocumentEventHandler"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="AbstractPdfDocumentEventHandler"/>
        /// instance.
        /// <para />
        /// By default, this instance handles all types of the
        /// <see cref="AbstractPdfDocumentEvent"/>
        /// events. For specific types
        /// handling, use
        /// <see cref="AddType(System.String)"/>
        /// method.
        /// </remarks>
        protected internal AbstractPdfDocumentEventHandler() {
        }

        /// <summary>
        /// Adds new event type to handle by this
        /// <see cref="AbstractPdfDocumentEventHandler"/>
        /// instance.
        /// </summary>
        /// <param name="type">
        /// the
        /// <see cref="AbstractPdfDocumentEvent"/>
        /// type to handle
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AbstractPdfDocumentEventHandler"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.Event.AbstractPdfDocumentEventHandler AddType(String type) {
            this.types.Add(type);
            return this;
        }

        public virtual void OnEvent(IEvent @event) {
            if (!(@event is AbstractPdfDocumentEvent)) {
                return;
            }
            AbstractPdfDocumentEvent iTextEvent = (AbstractPdfDocumentEvent)@event;
            if (types.IsEmpty() || types.Contains(iTextEvent.GetType())) {
                OnAcceptedEvent(iTextEvent);
            }
        }

        /// <summary>Handles the accepted event.</summary>
        /// <param name="event">
        /// 
        /// <see cref="AbstractPdfDocumentEvent"/>
        /// to handle
        /// </param>
        protected internal abstract void OnAcceptedEvent(AbstractPdfDocumentEvent @event);
    }
}
