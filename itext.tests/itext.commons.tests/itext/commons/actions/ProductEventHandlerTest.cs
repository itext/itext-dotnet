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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class ProductEventHandlerTest : ExtendedITextTest {
        [NUnit.Framework.SetUp]
        public virtual void ClearProcessors() {
            ProductEventHandler.INSTANCE.ClearProcessors();
        }

        [NUnit.Framework.TearDown]
        public virtual void AfterEach() {
            ProductProcessorFactoryKeeper.RestoreDefaultProductProcessorFactory();
        }

        [NUnit.Framework.Test]
        public virtual void UnknownProductTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            AbstractContextBasedITextEvent @event = new ITextTestEvent(new SequenceId(), null, "test-event", "Unknown Product"
                );
            Exception ex = NUnit.Framework.Assert.Catch(typeof(UnknownProductException), () => handler.OnAcceptedEvent
                (@event));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(UnknownProductException.UNKNOWN_PRODUCT, "Unknown Product"
                ), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SequenceIdBasedEventTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            handler.OnAcceptedEvent(new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE)
                );
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            AbstractProductProcessITextEvent @event = handler.GetEvents(sequenceId)[0];
            NUnit.Framework.Assert.AreEqual(sequenceId.GetId(), @event.GetSequenceId().GetId());
            NUnit.Framework.Assert.IsNull(@event.GetMetaInfo());
            NUnit.Framework.Assert.AreEqual("test-event", @event.GetEventType());
            NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
        }

        [NUnit.Framework.Test]
        public virtual void ReportEventSeveralTimesTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            ITextTestEvent @event = new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE);
            EventManager.GetInstance().OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[0]);
            EventManager.GetInstance().OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(2, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[0]);
            NUnit.Framework.Assert.AreEqual(@event, handler.GetEvents(sequenceId)[1]);
        }

        [NUnit.Framework.Test]
        public virtual void ConfirmEventTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            SequenceId sequenceId = new SequenceId();
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
            ITextTestEvent @event = new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE);
            EventManager.GetInstance().OnEvent(@event);
            ConfirmEvent confirmEvent = new ConfirmEvent(sequenceId, @event);
            EventManager.GetInstance().OnEvent(confirmEvent);
            NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
            NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId)[0] is ConfirmedEventWrapper);
            NUnit.Framework.Assert.AreEqual(@event, ((ConfirmedEventWrapper)handler.GetEvents(sequenceId)[0]).GetEvent
                ());
        }

        [NUnit.Framework.Test]
        public virtual void SettingCustomProcessFactoryTest() {
            ProductEventHandlerTest.CustomFactory productProcessorFactory = new ProductEventHandlerTest.CustomFactory(
                );
            productProcessorFactory.CreateProcessor(ProductNameConstant.ITEXT_CORE);
            ProductProcessorFactoryKeeper.SetProductProcessorFactory(productProcessorFactory);
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            ITextProductEventProcessor activeProcessor = handler.GetActiveProcessor(ProductNameConstant.ITEXT_CORE);
            NUnit.Framework.Assert.IsTrue(activeProcessor is ProductEventHandlerTest.TestProductEventProcessor);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatEventHandlingWithFiveExceptionOnProcessingTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            handler.AddProcessor(new ProductEventHandlerTest.RepeatEventProcessor(5));
            AbstractContextBasedITextEvent @event = new ITextTestEvent(new SequenceId(), null, "test", ProductNameConstant
                .ITEXT_CORE);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ProductEventHandlerRepeatException), () => handler.OnAcceptedEvent
                (@event));
            NUnit.Framework.Assert.AreEqual("customMessage5", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatEventHandlingWithFourExceptionOnProcessingTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            handler.AddProcessor(new ProductEventHandlerTest.RepeatEventProcessor(4));
            AbstractContextBasedITextEvent @event = new ITextTestEvent(new SequenceId(), null, "test", ProductNameConstant
                .ITEXT_CORE);
            NUnit.Framework.Assert.DoesNotThrow(() => handler.OnAcceptedEvent(@event));
        }

        [NUnit.Framework.Test]
        public virtual void RepeatEventHandlingWithOneExceptionOnProcessingTest() {
            ProductEventHandler handler = ProductEventHandler.INSTANCE;
            handler.AddProcessor(new ProductEventHandlerTest.RepeatEventProcessor(1));
            AbstractContextBasedITextEvent @event = new ITextTestEvent(new SequenceId(), null, "test", ProductNameConstant
                .ITEXT_CORE);
            NUnit.Framework.Assert.DoesNotThrow(() => handler.OnAcceptedEvent(@event));
        }

        private class CustomFactory : IProductProcessorFactory {
            public virtual ITextProductEventProcessor CreateProcessor(String productName) {
                return new ProductEventHandlerTest.TestProductEventProcessor(productName);
            }
        }

        private class TestProductEventProcessor : AbstractITextProductEventProcessor {
            public TestProductEventProcessor(String productName)
                : base(productName) {
            }

            public override void OnEvent(AbstractProductProcessITextEvent @event) {
            }

            // do nothing
            public override String GetUsageType() {
                return "AGPL";
            }
        }

        private class RepeatEventProcessor : ITextProductEventProcessor {
            private readonly int exceptionsCount;

            private int exceptionCounter = 0;

            public RepeatEventProcessor(int exceptionsCount) {
                this.exceptionsCount = exceptionsCount;
            }

            public virtual void OnEvent(AbstractProductProcessITextEvent @event) {
                if (exceptionCounter < exceptionsCount) {
                    exceptionCounter++;
                    throw new ProductEventHandlerRepeatException("customMessage" + exceptionCounter);
                }
            }

            public virtual String GetProductName() {
                return ProductNameConstant.ITEXT_CORE;
            }

            public virtual String GetUsageType() {
                return "someUsage";
            }

            public virtual String GetProducer() {
                return "someProducer";
            }
        }
    }
}
