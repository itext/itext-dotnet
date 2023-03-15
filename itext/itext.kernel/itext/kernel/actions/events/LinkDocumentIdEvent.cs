/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Actions.Sequence;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Events {
    /// <summary>
    /// An event allows to associated some
    /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>
    /// with
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
    /// </summary>
    public sealed class LinkDocumentIdEvent : AbstractITextConfigurationEvent {
        private readonly WeakReference document;

        private readonly WeakReference sequenceId;

        /// <summary>
        /// Creates a new instance of the event associating provided
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// with the
        /// appropriate
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <param name="document">is a document</param>
        /// <param name="sequenceId">is a general identifier to be associated with the document</param>
        public LinkDocumentIdEvent(PdfDocument document, SequenceId sequenceId)
            : base() {
            this.document = new WeakReference(document);
            this.sequenceId = new WeakReference(sequenceId);
        }

        /// <summary>
        /// Creates a new instance of the event associating provided
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// with the
        /// appropriate
        /// <see cref="iText.Commons.Actions.Sequence.AbstractIdentifiableElement"/>.
        /// </summary>
        /// <param name="document">is a document</param>
        /// <param name="identifiableElement">is an identifiable element to be associated with the document</param>
        public LinkDocumentIdEvent(PdfDocument document, AbstractIdentifiableElement identifiableElement)
            : this(document, identifiableElement == null ? null : SequenceIdManager.GetSequenceId(identifiableElement)
                ) {
        }

        /// <summary>
        /// Defines an association between
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// and
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>.
        /// </summary>
        protected internal override void DoAction() {
            SequenceId storedSequenceId = (SequenceId)sequenceId.Target;
            PdfDocument storedPdfDocument = (PdfDocument)document.Target;
            if (storedSequenceId == null || storedPdfDocument == null) {
                return;
            }
            IList<AbstractProductProcessITextEvent> anonymousEvents = GetEvents(storedSequenceId);
            if (anonymousEvents != null) {
                SequenceId documentId = storedPdfDocument.GetDocumentIdWrapper();
                foreach (AbstractProductProcessITextEvent @event in anonymousEvents) {
                    IList<AbstractProductProcessITextEvent> storedEvents = GetEvents(documentId);
                    if (!storedEvents.Contains(@event)) {
                        AddEvent(documentId, @event);
                    }
                }
            }
        }
    }
}
