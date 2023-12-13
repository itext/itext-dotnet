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
using iText.Kernel.Counter.Event;
using iText.Test;

namespace iText.Kernel.Counter {
    public class EventCounterHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCoreEvent() {
            int EVENTS_COUNT = 100;
            IEventCounterFactory counterFactory = new SimpleEventCounterFactory(new EventCounterHandlerTest.ToLogCounter
                ());
            EventCounterHandler.GetInstance().Register(counterFactory);
            EventCounterHandlerTest.MetaInfoCounter counter = new EventCounterHandlerTest.MetaInfoCounter();
            for (int i = 0; i < EVENTS_COUNT; ++i) {
                EventCounterHandler.GetInstance().OnEvent(CoreEvent.PROCESS, counter, GetType());
            }
            EventCounterHandler.GetInstance().Unregister(counterFactory);
            NUnit.Framework.Assert.AreEqual(counter.events_count, EVENTS_COUNT);
        }

        private class ToLogCounter : EventCounter {
            protected internal override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
                ((EventCounterHandlerTest.MetaInfoCounter)metaInfo).events_count++;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestDefaultCoreEvent() {
            int EVENTS_COUNT = 10001;
            IEventCounterFactory counterFactory = new SimpleEventCounterFactory(new EventCounterHandlerTest.ToLogDefaultCounter
                ());
            EventCounterHandler.GetInstance().Register(counterFactory);
            EventCounterHandlerTest.MetaInfoCounter counter = new EventCounterHandlerTest.MetaInfoCounter();
            for (int i = 0; i < EVENTS_COUNT; ++i) {
                EventCounterHandler.GetInstance().OnEvent(CoreEvent.PROCESS, counter, GetType());
            }
            EventCounterHandler.GetInstance().Unregister(counterFactory);
            NUnit.Framework.Assert.AreEqual(counter.events_count, EVENTS_COUNT);
        }

        private class MetaInfoCounter : IMetaInfo {
            internal int events_count = 0;
        }

        private class ToLogDefaultCounter : DefaultEventCounter {
            protected internal override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
                base.OnEvent(@event, metaInfo);
                ((EventCounterHandlerTest.MetaInfoCounter)metaInfo).events_count++;
            }
        }
    }
}
