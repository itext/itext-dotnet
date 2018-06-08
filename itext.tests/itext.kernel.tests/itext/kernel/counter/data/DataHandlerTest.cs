using System;
using System.Threading;
using Common.Logging;
using iText.Kernel.Counter.Event;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Counter.Data {
    public class DataHandlerTest : ExtendedITextTest {
        private const int SUCCESS_LIMIT = 3;

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage("Process event with signature: type1, and count: 1")]
        [LogMessage("Process event with signature: type1, and count: 2", Count = 2)]
        [LogMessage("Process event with signature: type1, and count: 3")]
        [LogMessage("Process event with signature: type1, and count: 4")]
        [LogMessage("Process event with signature: type2, and count: 2", Count = 2)]
        public virtual void RunTest() {
            DataHandlerTest.TestDataHandler dataHandler = new DataHandlerTest.TestDataHandler();
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"));
            Thread.Sleep(100);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"));
            Thread.Sleep(100);
            dataHandler.Register(new DataHandlerTest.TestEvent("type2"));
            Thread.Sleep(100);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"));
            Thread.Sleep(100);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"));
            Thread.Sleep(100);
            dataHandler.Register(new DataHandlerTest.TestEvent("type2"));
            Thread.Sleep(100);
            dataHandler.TryProcessRest();
        }

        private class SimpleData : EventData<String> {
            public SimpleData(String signature)
                : base(signature) {
            }
        }

        private class SimpleDataFactory : IEventDataFactory<String, DataHandlerTest.SimpleData> {
            public virtual DataHandlerTest.SimpleData Create(IEvent @event) {
                return new DataHandlerTest.SimpleData(@event.GetEventType());
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

        private class TestDataHandler : EventDataHandler<String, DataHandlerTest.SimpleData> {
            public TestDataHandler()
                : base(new EventDataCacheComparatorBased<String, DataHandlerTest.SimpleData>(new EventDataHandlerUtil.BiggerCountComparator
                    <String, DataHandlerTest.SimpleData>()), new DataHandlerTest.SimpleDataFactory(), 0, 0) {
            }

            protected internal override bool Process(DataHandlerTest.SimpleData data) {
                LogManager.GetLogger(GetType()).Warn("Process event with signature: " + data.GetSignature() + ", and count: "
                     + data.GetCount());
                return data.GetCount() > SUCCESS_LIMIT;
            }
        }
    }
}
