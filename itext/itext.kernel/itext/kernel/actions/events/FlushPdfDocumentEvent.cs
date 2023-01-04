/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Producer;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;

namespace iText.Kernel.Actions.Events {
    /// <summary>
    /// Class represents events notifying that
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// was flushed.
    /// </summary>
    public sealed class FlushPdfDocumentEvent : AbstractITextConfigurationEvent {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Actions.Events.FlushPdfDocumentEvent
            ));

        private readonly WeakReference document;

        /// <summary>Creates a new instance of the flushing event.</summary>
        /// <param name="document">is a document to be flushed</param>
        public FlushPdfDocumentEvent(PdfDocument document)
            : base() {
            this.document = new WeakReference(document);
        }

        /// <summary>Prepares document for flushing.</summary>
        protected internal override void DoAction() {
            PdfDocument pdfDocument = (PdfDocument)document.Target;
            if (pdfDocument == null) {
                return;
            }
            IList<AbstractProductProcessITextEvent> events = GetEvents(pdfDocument.GetDocumentIdWrapper());
            if (events == null || events.IsEmpty()) {
                ProductData productData = ITextCoreProductData.GetInstance();
                String noEventProducer = "iText\u00ae \u00a9" + productData.GetSinceCopyrightYear() + "-" + productData.GetToCopyrightYear
                    () + " iText Group NV (no registered products)";
                pdfDocument.GetDocumentInfo().SetProducer(noEventProducer);
                return;
            }
            ICollection<String> products = new HashSet<String>();
            foreach (AbstractProductProcessITextEvent @event in events) {
                pdfDocument.GetFingerPrint().RegisterProduct(@event.GetProductData());
                if (@event.GetConfirmationType() == EventConfirmationType.ON_CLOSE) {
                    EventManager.GetInstance().OnEvent(new ConfirmEvent(pdfDocument.GetDocumentIdWrapper(), @event));
                }
                products.Add(@event.GetProductName());
            }
            foreach (String product in products) {
                ITextProductEventProcessor processor = GetActiveProcessor(product);
                if (processor == null && LOGGER.IsEnabled(LogLevel.Warning)) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.UNKNOWN_PRODUCT_INVOLVED, product));
                }
            }
            String oldProducer = pdfDocument.GetDocumentInfo().GetProducer();
            String newProducer = ProducerBuilder.ModifyProducer(GetConfirmedEvents(pdfDocument.GetDocumentIdWrapper())
                , oldProducer);
            pdfDocument.GetDocumentInfo().SetProducer(newProducer);
        }

        private IList<ConfirmedEventWrapper> GetConfirmedEvents(SequenceId sequenceId) {
            IList<AbstractProductProcessITextEvent> events = GetEvents(sequenceId);
            IList<ConfirmedEventWrapper> confirmedEvents = new List<ConfirmedEventWrapper>();
            foreach (AbstractProductProcessITextEvent @event in events) {
                if (@event is ConfirmedEventWrapper) {
                    confirmedEvents.Add((ConfirmedEventWrapper)@event);
                }
                else {
                    LOGGER.LogWarning(MessageFormatUtil.Format(KernelLogMessageConstant.UNCONFIRMED_EVENT, @event.GetProductName
                        (), @event.GetEventType()));
                }
            }
            return confirmedEvents;
        }
    }
}
