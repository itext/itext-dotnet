using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Actions;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Statistics aggregator which aggregates size of PDF documents.</summary>
    public class SizeOfPdfStatisticsAggregator : AbstractStatisticsAggregator {
        private const long MEASURE_COEFFICIENT = 1024;

        private const long SIZE_128KB = 128 * MEASURE_COEFFICIENT;

        private const long SIZE_1MB = MEASURE_COEFFICIENT * MEASURE_COEFFICIENT;

        private const long SIZE_16MB = 16 * MEASURE_COEFFICIENT * MEASURE_COEFFICIENT;

        private const long SIZE_128MB = 128 * MEASURE_COEFFICIENT * MEASURE_COEFFICIENT;

        private const String STRING_FOR_128KB = "<128kb";

        private const String STRING_FOR_1MB = "128kb-1mb";

        private const String STRING_FOR_16MB = "1mb-16mb";

        private const String STRING_FOR_128MB = "16mb-128mb";

        private const String STRING_FOR_INF = "128mb+";

        private static readonly IDictionary<long, String> DOCUMENT_SIZES;

        // This List must be sorted.
        private static readonly IList<long> SORTED_UPPER_BOUNDS_OF_SIZES = JavaUtil.ArraysAsList(SIZE_128KB, SIZE_1MB
            , SIZE_16MB, SIZE_128MB);

        static SizeOfPdfStatisticsAggregator() {
            IDictionary<long, String> temp = new Dictionary<long, String>();
            temp.Put(SIZE_128KB, STRING_FOR_128KB);
            temp.Put(SIZE_1MB, STRING_FOR_1MB);
            temp.Put(SIZE_16MB, STRING_FOR_16MB);
            temp.Put(SIZE_128MB, STRING_FOR_128MB);
            DOCUMENT_SIZES = JavaCollectionsUtil.UnmodifiableMap(temp);
        }

        private readonly Object Lock = new Object();

        private readonly IDictionary<String, AtomicLong> numberOfDocuments = new ConcurrentDictionary<String, AtomicLong
            >();

        /// <summary>Aggregates size of the PDF document from the provided event.</summary>
        /// <param name="event">
        /// 
        /// <see cref="SizeOfPdfStatisticsEvent"/>
        /// instance
        /// </param>
        public override void Aggregate(AbstractStatisticsEvent @event) {
            if (!(@event is SizeOfPdfStatisticsEvent)) {
                return;
            }
            long sizeOfPdf = ((SizeOfPdfStatisticsEvent)@event).GetAmountOfBytes();
            String range = STRING_FOR_INF;
            foreach (long upperBound in SORTED_UPPER_BOUNDS_OF_SIZES) {
                if (sizeOfPdf <= upperBound) {
                    range = DOCUMENT_SIZES.Get(upperBound);
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
