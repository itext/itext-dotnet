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
using iText.Commons.Actions;
using iText.Commons.Utils;

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

        private readonly IDictionary<String, long?> numberOfDocuments = new LinkedDictionary<String, long?>();

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
                long? documentsOfThisRange = numberOfDocuments.Get(range);
                long? currentValue = documentsOfThisRange == null ? 1L : (documentsOfThisRange.Value + 1L);
                numberOfDocuments.Put(range, currentValue);
            }
        }

        /// <summary>Retrieves Map where keys are ranges of document sizes and values are the amounts of such PDF documents.
        ///     </summary>
        /// <returns>
        /// aggregated
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// </returns>
        public override Object RetrieveAggregation() {
            return JavaCollectionsUtil.UnmodifiableMap(numberOfDocuments);
        }

        /// <summary>Merges data about amounts of ranges of document sizes from the provided aggregator into this aggregator.
        ///     </summary>
        /// <param name="aggregator">
        /// 
        /// <see cref="SizeOfPdfStatisticsAggregator"/>
        /// from which data will be taken.
        /// </param>
        public override void Merge(AbstractStatisticsAggregator aggregator) {
            if (!(aggregator is SizeOfPdfStatisticsAggregator)) {
                return;
            }
            IDictionary<String, long?> amountOfDocuments = ((SizeOfPdfStatisticsAggregator)aggregator).numberOfDocuments;
            lock (Lock) {
                MapUtil.Merge(this.numberOfDocuments, amountOfDocuments, (el1, el2) => {
                    if (el2 == null) {
                        return el1;
                    }
                    else {
                        return el1 + el2;
                    }
                }
                );
            }
        }
    }
}
