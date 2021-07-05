using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Actions;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Statistics aggregator which aggregates number of pages in PDF documents.</summary>
    public class NumberOfPagesStatisticsAggregator : AbstractStatisticsAggregator {
        private const int ONE = 1;

        private const int TEN = 10;

        private const int HUNDRED = 100;

        private const int THOUSAND = 1000;

        private const String STRING_FOR_ONE_PAGE = "1";

        private const String STRING_FOR_TEN_PAGES = "2-10";

        private const String STRING_FOR_HUNDRED_PAGES = "11-100";

        private const String STRING_FOR_THOUSAND_PAGES = "101-1000";

        private const String STRING_FOR_INF = "1001+";

        private static readonly IDictionary<int, String> NUMBERS_OF_PAGES;

        // This List must be sorted.
        private static readonly IList<int> SORTED_UPPER_BOUNDS_OF_PAGES = JavaUtil.ArraysAsList(ONE, TEN, HUNDRED, 
            THOUSAND);

        static NumberOfPagesStatisticsAggregator() {
            IDictionary<int, String> temp = new Dictionary<int, String>();
            temp.Put(ONE, STRING_FOR_ONE_PAGE);
            temp.Put(TEN, STRING_FOR_TEN_PAGES);
            temp.Put(HUNDRED, STRING_FOR_HUNDRED_PAGES);
            temp.Put(THOUSAND, STRING_FOR_THOUSAND_PAGES);
            NUMBERS_OF_PAGES = JavaCollectionsUtil.UnmodifiableMap(temp);
        }

        private readonly Object Lock = new Object();

        private readonly IDictionary<String, AtomicLong> numberOfDocuments = new ConcurrentDictionary<String, AtomicLong
            >();

        /// <summary>Aggregates number of pages from the provided event.</summary>
        /// <param name="event">
        /// 
        /// <see cref="NumberOfPagesStatisticsEvent"/>
        /// instance
        /// </param>
        public override void Aggregate(AbstractStatisticsEvent @event) {
            if (!(@event is NumberOfPagesStatisticsEvent)) {
                return;
            }
            int numberOfPages = ((NumberOfPagesStatisticsEvent)@event).GetNumberOfPages();
            String range = STRING_FOR_INF;
            foreach (int upperBound in SORTED_UPPER_BOUNDS_OF_PAGES) {
                if (numberOfPages <= upperBound) {
                    range = NUMBERS_OF_PAGES.Get(upperBound);
                    break;
                }
            }
            lock (Lock) {
                AtomicLong documentsOfThisRange = numberOfDocuments.Get(range);
                if (documentsOfThisRange == null) {
                    numberOfDocuments.Put(range, new AtomicLong(1));
                }
                else {
                    documentsOfThisRange.IncrementAndGet();
                }
            }
        }

        /// <summary>Retrieves Map where keys are ranges of pages and values are the amounts of such PDF documents.</summary>
        /// <returns>
        /// aggregated
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// </returns>
        public override Object RetrieveAggregation() {
            return numberOfDocuments;
        }
    }
}
