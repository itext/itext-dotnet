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
using System.Collections.Generic;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;
using iText.Test;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractITextConfigurationEventTest : ExtendedITextTest {
        [NUnit.Framework.TearDown]
        public virtual void After() {
            ProductEventHandler.INSTANCE.ClearProcessors();
        }

        [NUnit.Framework.Test]
        public virtual void AddProcessorTest() {
            AbstractITextConfigurationEvent @event = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            ITextProductEventProcessor processor = new AbstractITextConfigurationEventTest.TestITextProductEventProcessor
                ();
            @event.AddProcessor(processor);
            IDictionary<String, ITextProductEventProcessor> processors = ProductEventHandler.INSTANCE.GetProcessors();
            NUnit.Framework.Assert.AreEqual(1, processors.Count);
            NUnit.Framework.Assert.IsTrue(processors.Values.Contains(processor));
        }

        [NUnit.Framework.Test]
        public virtual void GetProcessorsTest() {
            AbstractITextConfigurationEvent @event = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            ITextProductEventProcessor processor = new AbstractITextConfigurationEventTest.TestITextProductEventProcessor
                ();
            @event.AddProcessor(processor);
            NUnit.Framework.Assert.AreEqual(ProductEventHandler.INSTANCE.GetProcessors(), @event.GetProcessors());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveProcessorTest() {
            AbstractITextConfigurationEvent @event = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            ITextProductEventProcessor processor = new AbstractITextConfigurationEventTest.TestITextProductEventProcessor
                ();
            @event.AddProcessor(processor);
            @event.RemoveProcessor(processor.GetProductName());
            IDictionary<String, ITextProductEventProcessor> processors = ProductEventHandler.INSTANCE.GetProcessors();
            NUnit.Framework.Assert.AreEqual(0, processors.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetActiveProcessorTest() {
            AbstractITextConfigurationEvent @event = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            ITextProductEventProcessor processor = new AbstractITextConfigurationEventTest.TestITextProductEventProcessor
                ();
            @event.AddProcessor(processor);
            NUnit.Framework.Assert.AreEqual(processor, @event.GetActiveProcessor(processor.GetProductName()));
        }

        [NUnit.Framework.Test]
        public virtual void AddEventTest() {
            AbstractITextConfigurationEvent configurationEvent = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            AbstractProductProcessITextEvent processEvent = new AbstractITextConfigurationEventTest.TestAbstractProductProcessITextEvent
                ();
            SequenceId id = new SequenceId();
            configurationEvent.AddEvent(id, processEvent);
            IList<AbstractProductProcessITextEvent> events = ProductEventHandler.INSTANCE.GetEvents(id);
            NUnit.Framework.Assert.AreEqual(1, events.Count);
            NUnit.Framework.Assert.AreEqual(processEvent, events[0]);
        }

        [NUnit.Framework.Test]
        public virtual void GetEventsTest() {
            AbstractITextConfigurationEvent configurationEvent = new AbstractITextConfigurationEventTest.TestAbstractITextConfigurationEvent
                ();
            SequenceId id = new SequenceId();
            configurationEvent.AddEvent(id, new AbstractITextConfigurationEventTest.TestAbstractProductProcessITextEvent
                ());
            configurationEvent.AddEvent(id, new AbstractITextConfigurationEventTest.TestAbstractProductProcessITextEvent
                ());
            NUnit.Framework.Assert.AreEqual(ProductEventHandler.INSTANCE.GetEvents(id), configurationEvent.GetEvents(id
                ));
        }

//\cond DO_NOT_DOCUMENT
        internal class TestAbstractITextConfigurationEvent : AbstractITextConfigurationEvent {
            protected internal override void DoAction() {
            }
            // Empty method.
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class TestAbstractProductProcessITextEvent : AbstractProductProcessITextEvent {
            public TestAbstractProductProcessITextEvent()
                : base(new SequenceId(), new ProductData("test public product name", "test product name", "test version", 
                    0, 1), null, EventConfirmationType.ON_DEMAND) {
            }

            public override String GetEventType() {
                return "test event type";
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class TestITextProductEventProcessor : ITextProductEventProcessor {
            public virtual void OnEvent(AbstractProductProcessITextEvent @event) {
            }

            // Empty method.
            public virtual String GetProductName() {
                return "test product";
            }

            public virtual String GetUsageType() {
                return "test usage type";
            }

            public virtual String GetProducer() {
                return "test producer";
            }
        }
//\endcond
    }
}
