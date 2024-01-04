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
using System.Collections.Generic;
using iText.Commons.Actions.Data;
using iText.Commons.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class AbstractStatisticsEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            AbstractStatisticsEventTest.DummyStatisticsEvent dummyEvent = new AbstractStatisticsEventTest.DummyStatisticsEvent
                (new ProductData("public name", "product name", "version", 15, 3000));
            ProductData data = dummyEvent.GetProductData();
            NUnit.Framework.Assert.AreEqual("public name", data.GetPublicProductName());
            NUnit.Framework.Assert.AreEqual("product name", data.GetProductName());
            NUnit.Framework.Assert.AreEqual("version", data.GetVersion());
            NUnit.Framework.Assert.AreEqual(15, data.GetSinceCopyrightYear());
            NUnit.Framework.Assert.AreEqual(3000, data.GetToCopyrightYear());
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.INVALID_STATISTICS_NAME)]
        public virtual void CreateStatisticsAggregatorFromNameTest() {
            AbstractStatisticsEventTest.DummyStatisticsEvent dummyEvent = new AbstractStatisticsEventTest.DummyStatisticsEvent
                (new ProductData("public name", "product name", "version", 15, 3000));
            NUnit.Framework.Assert.IsNull(dummyEvent.CreateStatisticsAggregatorFromName("statisticsName"));
        }

        internal class DummyStatisticsEvent : AbstractStatisticsEvent {
            internal DummyStatisticsEvent(ProductData data)
                : base(data) {
            }

            public override IList<String> GetStatisticsNames() {
                return null;
            }
        }
    }
}
