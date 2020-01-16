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
