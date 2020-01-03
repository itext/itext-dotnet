/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
