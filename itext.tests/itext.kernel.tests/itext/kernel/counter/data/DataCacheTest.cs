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
using System.Collections.Generic;
using iText.IO.Util;
using iText.Test;

namespace iText.Kernel.Counter.Data {
    public class DataCacheTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void QueueTest() {
            TestCache(new EventDataCacheQueueBased<String, DataCacheTest.SimpleData>(), JavaUtil.ArraysAsList(new DataCacheTest.SimpleData
                ("type1", 10), new DataCacheTest.SimpleData("type2", 4), new DataCacheTest.SimpleData("type3", 5), new 
                DataCacheTest.SimpleData("type2", 12)), JavaUtil.ArraysAsList(new DataCacheTest.SimpleData("type1", 10
                ), new DataCacheTest.SimpleData("type2", 16), new DataCacheTest.SimpleData("type3", 5)));
        }

        [NUnit.Framework.Test]
        public virtual void BiggestCountTest() {
            TestCache(new EventDataCacheComparatorBased<String, DataCacheTest.SimpleData>(new EventDataHandlerUtil.BiggerCountComparator
                <String, DataCacheTest.SimpleData>()), JavaUtil.ArraysAsList(new DataCacheTest.SimpleData("type1", 10)
                , new DataCacheTest.SimpleData("type2", 4), new DataCacheTest.SimpleData("type3", 5), new DataCacheTest.SimpleData
                ("type2", 8)), JavaUtil.ArraysAsList(new DataCacheTest.SimpleData("type2", 12), new DataCacheTest.SimpleData
                ("type1", 10), new DataCacheTest.SimpleData("type3", 5)));
        }

        private static void TestCache(IEventDataCache<String, DataCacheTest.SimpleData> cache, IList<DataCacheTest.SimpleData
            > input, IList<DataCacheTest.SimpleData> expectedOutput) {
            foreach (DataCacheTest.SimpleData @event in input) {
                cache.Put(@event);
            }
            foreach (DataCacheTest.SimpleData expected in expectedOutput) {
                DataCacheTest.SimpleData actual = cache.RetrieveNext();
                NUnit.Framework.Assert.AreEqual(expected.GetSignature(), actual.GetSignature());
                NUnit.Framework.Assert.AreEqual(expected.GetCount(), actual.GetCount());
            }
        }

        private class SimpleData : EventData<String> {
            public SimpleData(String signature, long count)
                : base(signature, count) {
            }
        }
    }
}
