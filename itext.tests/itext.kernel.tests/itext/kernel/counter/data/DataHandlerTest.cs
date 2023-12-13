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
using System.Threading;
using Common.Logging;
using iText.Kernel.Counter.Event;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Counter.Data {
    public class DataHandlerTest : ExtendedITextTest {
        private const int SUCCESS_LIMIT = 3;

        [NUnit.Framework.Test]
        [LogMessage("Process event with signature: type1, and count: 1")]
        [LogMessage("Process event with signature: type1, and count: 2", Count = 2)]
        [LogMessage("Process event with signature: type1, and count: 3")]
        [LogMessage("Process event with signature: type1, and count: 4")]
        [LogMessage("Process event with signature: type2, and count: 2", Count = 2)]
        public virtual void RunTest() {
            DataHandlerTest.TestDataHandler dataHandler = new DataHandlerTest.TestDataHandler();
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"), null);
            Thread.Sleep(200);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"), null);
            Thread.Sleep(200);
            dataHandler.Register(new DataHandlerTest.TestEvent("type2"), null);
            Thread.Sleep(200);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"), null);
            Thread.Sleep(200);
            dataHandler.Register(new DataHandlerTest.TestEvent("type1"), null);
            Thread.Sleep(200);
            dataHandler.Register(new DataHandlerTest.TestEvent("type2"), null);
            Thread.Sleep(200);
            dataHandler.TryProcessRest();
        }

        private class SimpleData : EventData<String> {
            public SimpleData(String signature)
                : base(signature) {
            }
        }

        private class SimpleDataFactory : IEventDataFactory<String, DataHandlerTest.SimpleData> {
            public virtual DataHandlerTest.SimpleData Create(IEvent @event, IMetaInfo metaInfo) {
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
