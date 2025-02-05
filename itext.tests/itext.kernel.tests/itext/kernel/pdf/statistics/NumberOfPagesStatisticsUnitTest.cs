/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Kernel.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Statistics {
    [NUnit.Framework.Category("UnitTest")]
    public class NumberOfPagesStatisticsUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultEventTest() {
            NumberOfPagesStatisticsEvent @event = new NumberOfPagesStatisticsEvent(1, ITextCoreProductData.GetInstance
                ());
            NUnit.Framework.Assert.AreEqual(1, @event.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("numberOfPages"), @event.GetStatisticsNames
                ());
            NUnit.Framework.Assert.AreEqual(typeof(NumberOfPagesStatisticsAggregator), @event.CreateStatisticsAggregatorFromName
                ("numberOfPages").GetType());
        }

        [NUnit.Framework.Test]
        public virtual void InvalidArgumentEventTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => new NumberOfPagesStatisticsEvent
                (-1, ITextCoreProductData.GetInstance()));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NUMBER_OF_PAGES_CAN_NOT_BE_NEGATIVE, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void ZeroNumberOfPagesTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => new NumberOfPagesStatisticsEvent(0, ITextCoreProductData.GetInstance
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void AggregateZeroPageEventTest() {
            NumberOfPagesStatisticsAggregator aggregator = new NumberOfPagesStatisticsAggregator();
            aggregator.Aggregate(new NumberOfPagesStatisticsEvent(0, ITextCoreProductData.GetInstance()));
            Object aggregation = aggregator.RetrieveAggregation();
            IDictionary<String, long?> castedAggregation = (IDictionary<String, long?>)aggregation;
            NUnit.Framework.Assert.AreEqual(1, castedAggregation.Count);
            long? numberOfPages = castedAggregation.Get("1");
            NUnit.Framework.Assert.AreEqual(1L, numberOfPages);
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.INVALID_STATISTICS_NAME)]
        public virtual void InvalidStatisticsNameEventTest() {
            NumberOfPagesStatisticsEvent @event = new NumberOfPagesStatisticsEvent(5, ITextCoreProductData.GetInstance
                ());
            NUnit.Framework.Assert.IsNull(@event.CreateStatisticsAggregatorFromName("invalid name"));
        }

        [NUnit.Framework.Test]
        public virtual void AggregateEventTest() {
            NumberOfPagesStatisticsAggregator aggregator = new NumberOfPagesStatisticsAggregator();
            NumberOfPagesStatisticsEvent @event = new NumberOfPagesStatisticsEvent(5, ITextCoreProductData.GetInstance
                ());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(7, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(10, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(2, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(1000, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(500, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(100000000, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(1, ITextCoreProductData.GetInstance());
            aggregator.Aggregate(@event);
            Object aggregation = aggregator.RetrieveAggregation();
            IDictionary<String, long?> castedAggregation = (IDictionary<String, long?>)aggregation;
            NUnit.Framework.Assert.AreEqual(4, castedAggregation.Count);
            long? numberOfPages = castedAggregation.Get("1");
            NUnit.Framework.Assert.AreEqual(1L, numberOfPages);
            numberOfPages = castedAggregation.Get("2-10");
            NUnit.Framework.Assert.AreEqual(4L, numberOfPages);
            NUnit.Framework.Assert.IsNull(castedAggregation.Get("11-100"));
            numberOfPages = castedAggregation.Get("101-1000");
            NUnit.Framework.Assert.AreEqual(2L, numberOfPages);
            numberOfPages = castedAggregation.Get("1001+");
            NUnit.Framework.Assert.AreEqual(1L, numberOfPages);
        }

        [NUnit.Framework.Test]
        public virtual void NothingAggregatedTest() {
            NumberOfPagesStatisticsAggregator aggregator = new NumberOfPagesStatisticsAggregator();
            Object aggregation = aggregator.RetrieveAggregation();
            IDictionary<String, long?> castedAggregation = (IDictionary<String, long?>)aggregation;
            NUnit.Framework.Assert.IsTrue(castedAggregation.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void AggregateWrongEventTest() {
            NumberOfPagesStatisticsAggregator aggregator = new NumberOfPagesStatisticsAggregator();
            aggregator.Aggregate(new SizeOfPdfStatisticsEvent(200, ITextCoreProductData.GetInstance()));
            Object aggregation = aggregator.RetrieveAggregation();
            IDictionary<String, long?> castedAggregation = (IDictionary<String, long?>)aggregation;
            NUnit.Framework.Assert.IsTrue(castedAggregation.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void MergeTest() {
            NumberOfPagesStatisticsAggregator aggregator1 = new NumberOfPagesStatisticsAggregator();
            NumberOfPagesStatisticsAggregator aggregator2 = new NumberOfPagesStatisticsAggregator();
            NumberOfPagesStatisticsEvent @event = new NumberOfPagesStatisticsEvent(5, ITextCoreProductData.GetInstance
                ());
            aggregator1.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(1, ITextCoreProductData.GetInstance());
            aggregator1.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(7, ITextCoreProductData.GetInstance());
            aggregator1.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(10, ITextCoreProductData.GetInstance());
            aggregator1.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(1000, ITextCoreProductData.GetInstance());
            aggregator1.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(500, ITextCoreProductData.GetInstance());
            aggregator2.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(100000000, ITextCoreProductData.GetInstance());
            aggregator2.Aggregate(@event);
            @event = new NumberOfPagesStatisticsEvent(2, ITextCoreProductData.GetInstance());
            aggregator2.Aggregate(@event);
            aggregator1.Merge(aggregator2);
            Object aggregation = aggregator1.RetrieveAggregation();
            IDictionary<String, long?> castedAggregation = (IDictionary<String, long?>)aggregation;
            NUnit.Framework.Assert.AreEqual(4, castedAggregation.Count);
            long? numberOfPages = castedAggregation.Get("1");
            NUnit.Framework.Assert.AreEqual(1L, numberOfPages);
            numberOfPages = castedAggregation.Get("2-10");
            NUnit.Framework.Assert.AreEqual(4L, numberOfPages);
            NUnit.Framework.Assert.IsNull(castedAggregation.Get("11-100"));
            numberOfPages = castedAggregation.Get("101-1000");
            NUnit.Framework.Assert.AreEqual(2L, numberOfPages);
            numberOfPages = castedAggregation.Get("1001+");
            NUnit.Framework.Assert.AreEqual(1L, numberOfPages);
        }
    }
}
