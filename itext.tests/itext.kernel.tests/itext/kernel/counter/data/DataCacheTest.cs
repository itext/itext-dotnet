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
