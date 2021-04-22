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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using iText.IO.Util;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Exceptions;
using iText.Kernel.Actions.Processors;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Counter.Context;

namespace iText.Kernel.Actions {
    /// <summary>Handles events based oh their origin.</summary>
    internal sealed class ProductEventHandler : AbstractContextBasedEventHandler {
        internal static readonly iText.Kernel.Actions.ProductEventHandler INSTANCE = new iText.Kernel.Actions.ProductEventHandler
            ();

        private static readonly ICollection<String> PRODUCTS_NAMESPACES = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(ProductNameConstant.ITEXT_CORE, ProductNameConstant.PDF_HTML, ProductNameConstant
            .PDF_SWEEP, ProductNameConstant.PDF_OCR, ProductNameConstant.PDF_OCR_TESSERACT4)));

        private readonly ConcurrentDictionary<String, ITextProductEventProcessor> processors = new ConcurrentDictionary
            <String, ITextProductEventProcessor>();

        private readonly ConditionalWeakTable<SequenceId, IList<ITextProductEventWrapper>> events = new ConditionalWeakTable
            <SequenceId, IList<ITextProductEventWrapper>>();

        private ProductEventHandler()
            : base(UnknownContext.PERMISSIVE) {
        }

        /// <summary>
        /// Pass the event to the appropriate
        /// <see cref="iText.Kernel.Actions.Processors.ITextProductEventProcessor"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        protected internal override void OnAcceptedEvent(ITextEvent @event) {
            if (@event is AbstractITextProductEvent) {
                AbstractITextProductEvent iTextEvent = (AbstractITextProductEvent)@event;
                ITextProductEventProcessor productEventProcessor = FindProcessorForProduct(iTextEvent.GetProductName());
                if (iTextEvent.GetSequenceId() != null) {
                    lock (events) {
                        SequenceId id = iTextEvent.GetSequenceId();
                        if (!events.ContainsKey(id)) {
                            events.Put(id, new List<ITextProductEventWrapper>());
                        }
                        // TODO DEVSIX-5053 if event reporting will be done on document closing then
                        //  event wrapping should be done there
                        events.Get(id).Add(new ITextProductEventWrapper(iTextEvent, productEventProcessor.GetUsageType(), productEventProcessor
                            .GetProducer()));
                    }
                }
                productEventProcessor.OnEvent(iTextEvent);
            }
        }

        internal ITextProductEventProcessor AddProcessor(ITextProductEventProcessor processor) {
            return processors.Put(processor.GetProductName(), processor);
        }

        internal ITextProductEventProcessor RemoveProcessor(String productName) {
            return processors.JRemove(productName);
        }

        internal ITextProductEventProcessor GetProcessor(String productName) {
            return processors.Get(productName);
        }

        internal IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return JavaCollectionsUtil.UnmodifiableMap(new Dictionary<String, ITextProductEventProcessor>(processors));
        }

        internal IList<ITextProductEventWrapper> GetEvents(SequenceId id) {
            lock (events) {
                IList<ITextProductEventWrapper> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    return JavaCollectionsUtil.EmptyList<ITextProductEventWrapper>();
                }
                return JavaCollectionsUtil.UnmodifiableList<ITextProductEventWrapper>(new List<ITextProductEventWrapper>(listOfEvents
                    ));
            }
        }

        internal void AddEvent(SequenceId id, ITextProductEventWrapper @event) {
            lock (events) {
                IList<ITextProductEventWrapper> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    listOfEvents = new List<ITextProductEventWrapper>();
                    events.Put(id, listOfEvents);
                }
                listOfEvents.Add(@event);
            }
        }

        private ITextProductEventProcessor FindProcessorForProduct(String productName) {
            ITextProductEventProcessor processor = processors.Get(productName);
            if (processor != null) {
                return processor;
            }
            if (PRODUCTS_NAMESPACES.Contains(productName)) {
                processor = new DefaultITextProductEventProcessor(productName);
                processors.Put(productName, processor);
                return processor;
            }
            else {
                throw new UnknownProductException(MessageFormatUtil.Format(UnknownProductException.UNKNOWN_PRODUCT, productName
                    ));
            }
        }
    }
}
