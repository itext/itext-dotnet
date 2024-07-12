/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;
using iText.Commons.Exceptions;
using iText.Commons.Logs;
using iText.Commons.Utils;

namespace iText.Commons.Actions {
//\cond DO_NOT_DOCUMENT
    /// <summary>Handles events based oh their origin.</summary>
    internal sealed class ProductEventHandler : AbstractContextBasedEventHandler {
//\cond DO_NOT_DOCUMENT
        internal static readonly iText.Commons.Actions.ProductEventHandler INSTANCE = new iText.Commons.Actions.ProductEventHandler
            ();
//\endcond

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Commons.Actions.ProductEventHandler
            ));

        // The constant has the following value for two reasons. First, to avoid the infinite loop.
        // Second, to retry event processing several times for technical reasons.
        private const int MAX_EVENT_RETRY_COUNT = 4;

        private readonly ConcurrentDictionary<String, ITextProductEventProcessor> processors = new ConcurrentDictionary
            <String, ITextProductEventProcessor>();

        private readonly ConditionalWeakTable<SequenceId, IList<AbstractProductProcessITextEvent>> events = new ConditionalWeakTable
            <SequenceId, IList<AbstractProductProcessITextEvent>>();

        private ProductEventHandler()
            : base(UnknownContext.PERMISSIVE) {
        }

        /// <summary>
        /// Pass the event to the appropriate
        /// <see cref="iText.Commons.Actions.Processors.ITextProductEventProcessor"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        protected internal override void OnAcceptedEvent(AbstractContextBasedITextEvent @event) {
            for (int i = 0; i < MAX_EVENT_RETRY_COUNT; i++) {
                try {
                    TryProcessEvent(@event);
                    // process succeeded
                    return;
                }
                catch (ProductEventHandlerRepeatException) {
                }
            }
            // ignore this exception to retry the processing
            // the final processing retry
            TryProcessEvent(@event);
        }

//\cond DO_NOT_DOCUMENT
        internal ITextProductEventProcessor AddProcessor(ITextProductEventProcessor processor) {
            return processors.Put(processor.GetProductName(), processor);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal ITextProductEventProcessor RemoveProcessor(String productName) {
            return processors.JRemove(productName);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal ITextProductEventProcessor GetActiveProcessor(String productName) {
            ITextProductEventProcessor processor = processors.Get(productName);
            if (processor != null) {
                return processor;
            }
            if (ProductNameConstant.PRODUCT_NAMES.Contains(productName)) {
                processor = ProductProcessorFactoryKeeper.GetProductProcessorFactory().CreateProcessor(productName);
                processors.Put(productName, processor);
                return processor;
            }
            else {
                return null;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return JavaCollectionsUtil.UnmodifiableMap(new Dictionary<String, ITextProductEventProcessor>(processors));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal void ClearProcessors() {
            processors.Clear();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal IList<AbstractProductProcessITextEvent> GetEvents(SequenceId id) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    return JavaCollectionsUtil.EmptyList<AbstractProductProcessITextEvent>();
                }
                return JavaCollectionsUtil.UnmodifiableList<AbstractProductProcessITextEvent>(new List<AbstractProductProcessITextEvent
                    >(listOfEvents));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal void AddEvent(SequenceId id, AbstractProductProcessITextEvent @event) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    listOfEvents = new List<AbstractProductProcessITextEvent>();
                    events.Put(id, listOfEvents);
                }
                listOfEvents.Add(@event);
            }
        }
//\endcond

        private void TryProcessEvent(AbstractContextBasedITextEvent @event) {
            if (!(@event is AbstractProductProcessITextEvent)) {
                return;
            }
            AbstractProductProcessITextEvent productEvent = (AbstractProductProcessITextEvent)@event;
            String productName = productEvent.GetProductName();
            ITextProductEventProcessor productEventProcessor = GetActiveProcessor(productName);
            if (productEventProcessor == null) {
                throw new UnknownProductException(MessageFormatUtil.Format(UnknownProductException.UNKNOWN_PRODUCT, productName
                    ));
            }
            productEventProcessor.OnEvent(productEvent);
            if (productEvent.GetSequenceId() != null) {
                if (productEvent is ConfirmEvent) {
                    WrapConfirmedEvent((ConfirmEvent)productEvent, productEventProcessor);
                }
                else {
                    AddEvent(productEvent.GetSequenceId(), productEvent);
                }
            }
        }

        private void WrapConfirmedEvent(ConfirmEvent @event, ITextProductEventProcessor productEventProcessor) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> eventsList = events.Get(@event.GetSequenceId());
                AbstractProductProcessITextEvent confirmedEvent = @event.GetConfirmedEvent();
                int indexOfReportedEvent = eventsList.IndexOf(confirmedEvent);
                if (indexOfReportedEvent >= 0) {
                    eventsList[indexOfReportedEvent] = new ConfirmedEventWrapper(confirmedEvent, productEventProcessor.GetUsageType
                        (), productEventProcessor.GetProducer());
                }
                else {
                    LOGGER.LogWarning(MessageFormatUtil.Format(CommonsLogMessageConstant.UNREPORTED_EVENT, confirmedEvent.GetProductName
                        (), confirmedEvent.GetEventType()));
                }
            }
        }
    }
//\endcond
}
