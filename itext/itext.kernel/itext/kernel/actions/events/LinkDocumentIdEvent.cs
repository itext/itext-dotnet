/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Actions;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Events {
    /// <summary>
    /// An event allows to associated some
    /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>
    /// with
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
    /// </summary>
    public sealed class LinkDocumentIdEvent : AbstractITextConfigurationEvent {
        private const String LINK_DOCUMENT_ID_EVENT_TYPE = "link-document-id-event";

        private readonly WeakReference document;

        private readonly WeakReference sequenceId;

        private readonly String productName;

        /// <summary>
        /// Creates a new instance of the event associating provided
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// with the
        /// appropriate
        /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <param name="document">is a document</param>
        /// <param name="sequenceId">is a general identifier to be associated with the document</param>
        public LinkDocumentIdEvent(PdfDocument document, SequenceId sequenceId, String productName)
            : base() {
            this.document = new WeakReference(document);
            this.sequenceId = new WeakReference(sequenceId);
            this.productName = productName;
        }

        /// <summary>Returns iText core product identifier.</summary>
        /// <returns>iText core product identifier</returns>
        public override String GetProductName() {
            return productName;
        }

        /// <summary>
        /// Defines an association between
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// and
        /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>.
        /// </summary>
        protected internal override void DoAction() {
            SequenceId storedSequenceId = (SequenceId)sequenceId.Target;
            PdfDocument storedPdfDocument = (PdfDocument)document.Target;
            if (storedSequenceId == null || storedPdfDocument == null) {
                return;
            }
            IList<AbstractITextProductEvent> anonymousEvents = GetEvents(storedSequenceId);
            if (anonymousEvents != null) {
                SequenceId documentId = storedPdfDocument.GetDocumentIdWrapper();
                foreach (AbstractITextProductEvent @event in anonymousEvents) {
                    AddEvent(documentId, @event);
                }
            }
        }

        public override String GetEventType() {
            return LINK_DOCUMENT_ID_EVENT_TYPE;
        }
    }
}
