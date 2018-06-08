using System;
using Common.Logging;
using iText.Kernel.Counter.Event;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Counter {
    public class EventCounterHandlerTest : ExtendedITextTest {
        private const int COUNT = 100;

        [NUnit.Framework.Test]
        [LogMessage("Process event: core-process", Count = COUNT)]
        public virtual void TestCoreEvent() {
            IEventCounterFactory counterFactory = new SimpleEventCounterFactory(new EventCounterHandlerTest.ToLogCounter
                ());
            EventCounterHandler.GetInstance().Register(counterFactory);
            for (int i = 0; i < COUNT; ++i) {
                EventCounterHandler.GetInstance().OnEvent(CoreEvent.PROCESS, GetType());
            }
            EventCounterHandler.GetInstance().Unregister(counterFactory);
        }

        [NUnit.Framework.Test]
        public virtual void TestUnknownEvent() {
            IEventCounterFactory counterFactory = new SimpleEventCounterFactory(new EventCounterHandlerTest.ToLogCounter
                ());
            EventCounterHandler.GetInstance().Register(counterFactory);
            IEvent unknown = new EventCounterHandlerTest.UnknownEvent();
            for (int i = 0; i < COUNT; ++i) {
                EventCounterHandler.GetInstance().OnEvent(unknown, GetType());
            }
            EventCounterHandler.GetInstance().Unregister(counterFactory);
        }

        private class ToLogCounter : EventCounter {
            protected internal override void Process(IEvent @event) {
                LogManager.GetLogger(GetType()).Warn("Process event: " + @event.GetEventType());
            }
        }

        private class UnknownEvent : IEvent {
            public virtual String GetEventType() {
                return "unknown";
            }
        }
    }
}
