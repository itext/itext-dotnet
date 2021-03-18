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
using System.Threading;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Counter.Data;
using iText.Kernel.Counter.Event;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Counter {
    public class DataHandlerCounterTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DisableHooksTest() {
            int betweenChecksSleepTime = 500;
            int handlerSleepTime = 100;
            DataHandlerCounterTest.TestDataHandler dataHandler = new DataHandlerCounterTest.TestDataHandler(handlerSleepTime
                );
            DataHandlerCounterTest.TestDataHandlerCounter counter = new DataHandlerCounterTest.TestDataHandlerCounter(
                dataHandler);
            // check the initial process count
            NUnit.Framework.Assert.AreEqual(0, dataHandler.GetProcessCount());
            long count = dataHandler.GetProcessCount();
            Thread.Sleep(betweenChecksSleepTime);
            // check that process count has been updated
            NUnit.Framework.Assert.AreNotEqual(count, dataHandler.GetProcessCount());
            counter.Close();
            // ensure that last process on disable would be finished
            Thread.Sleep(betweenChecksSleepTime);
            long totalCount = dataHandler.GetProcessCount();
            Thread.Sleep(betweenChecksSleepTime);
            // ensure that after disabling there are no new processes has been invoked
            NUnit.Framework.Assert.AreEqual(totalCount, dataHandler.GetProcessCount());
        }

        [NUnit.Framework.Test]
        public virtual void OnEventAfterDisableTest() {
            DataHandlerCounterTest.TestDataHandlerCounter counter = new DataHandlerCounterTest.TestDataHandlerCounter(
                new DataHandlerCounterTest.TestDataHandler(100));
            DataHandlerCounterTest.TestEvent testEvent = new DataHandlerCounterTest.TestEvent("test");
            NUnit.Framework.Assert.DoesNotThrow(() => counter.OnEvent(testEvent, null));
            counter.Close();
            NUnit.Framework.Assert.That(() =>  {
                counter.OnEvent(testEvent, null);
            }
            , NUnit.Framework.Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo(PdfException.DataHandlerCounterHasBeenDisabled))
;
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRegisterHooksTest() {
            DataHandlerCounterTest.TestDataHandler dataHandler = new DataHandlerCounterTest.TestDataHandler(200);
            DataHandlerCounterTest.TestDataHandlerCounter counter = new DataHandlerCounterTest.TestDataHandlerCounter(
                dataHandler);
            DataHandlerCounterTest.TestDataHandlerCounter secondCounter = new DataHandlerCounterTest.TestDataHandlerCounter
                (dataHandler);
            NUnit.Framework.Assert.DoesNotThrow(() => counter.Close());
            NUnit.Framework.Assert.DoesNotThrow(() => secondCounter.Close());
        }

        [NUnit.Framework.Test]
        // count set explicitly as it is required that the test should log this message only once
        [LogMessage(iText.IO.LogMessageConstant.UNEXPECTED_EVENT_HANDLER_SERVICE_THREAD_EXCEPTION, Count = 1, LogLevel
             = LogLevelConstants.ERROR)]
        public virtual void TimedProcessWithExceptionTest() {
            int betweenChecksSleepTime = 500;
            int handlerSleepTime = 100;
            DataHandlerCounterTest.TestDataHandlerWithException dataHandler = new DataHandlerCounterTest.TestDataHandlerWithException
                (handlerSleepTime);
            DataHandlerCounterTest.TestDataHandlerCounter counter = new DataHandlerCounterTest.TestDataHandlerCounter(
                dataHandler);
            // check the initial process count
            NUnit.Framework.Assert.AreEqual(0, dataHandler.GetProcessCount());
            Thread.Sleep(betweenChecksSleepTime);
            // check that process count has not been updated
            NUnit.Framework.Assert.AreEqual(0, dataHandler.GetProcessCount());
            NUnit.Framework.Assert.DoesNotThrow(() => counter.Close());
        }

        private class TestDataHandlerCounter : DataHandlerCounter<String, DataHandlerCounterTest.SimpleData> {
            public TestDataHandlerCounter(DataHandlerCounterTest.TestDataHandler dataHandler)
                : base(dataHandler) {
            }
        }

        private class SimpleData : EventData<String> {
            public SimpleData(String signature)
                : base(signature) {
            }
        }

        private class SimpleDataFactory : IEventDataFactory<String, DataHandlerCounterTest.SimpleData> {
            public virtual DataHandlerCounterTest.SimpleData Create(IEvent @event, IMetaInfo metaInfo) {
                return new DataHandlerCounterTest.SimpleData(@event.GetEventType());
            }
        }

        private class TestDataHandler : EventDataHandler<String, DataHandlerCounterTest.SimpleData> {
            private readonly AtomicLong processCount = new AtomicLong(0);

            public TestDataHandler(long sleepTime)
                : base(new EventDataCacheComparatorBased<String, DataHandlerCounterTest.SimpleData>(new EventDataHandlerUtil.BiggerCountComparator
                    <String, DataHandlerCounterTest.SimpleData>()), new DataHandlerCounterTest.SimpleDataFactory(), sleepTime
                    , sleepTime) {
            }

            public override void TryProcessNext() {
                processCount.IncrementAndGet();
                base.TryProcessNext();
            }

            public override void TryProcessRest() {
                processCount.IncrementAndGet();
                base.TryProcessRest();
            }

            protected internal override bool Process(DataHandlerCounterTest.SimpleData data) {
                return true;
            }

            public virtual long GetProcessCount() {
                return processCount.Get();
            }
        }

        private class TestDataHandlerWithException : DataHandlerCounterTest.TestDataHandler {
            public TestDataHandlerWithException(long sleepTime)
                : base(sleepTime) {
            }

            public override void TryProcessNextAsync(bool? daemon) {
                throw new PdfException("Some exception message");
            }
        }

        private class TestEvent : IEvent {
            private readonly String type;

            public TestEvent(String type) {
                this.type = type;
            }

            public virtual String GetEventType() {
                return type;
            }
        }
    }
}
