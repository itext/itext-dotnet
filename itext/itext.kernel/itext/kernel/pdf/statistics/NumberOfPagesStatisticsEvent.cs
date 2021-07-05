using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Actions;
using iText.Kernel.Actions.Data;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Class which represents event for counting the number of pages in a PDF document.</summary>
    /// <remarks>Class which represents event for counting the number of pages in a PDF document. Only for internal usage.
    ///     </remarks>
    public class NumberOfPagesStatisticsEvent : AbstractStatisticsEvent {
        private const String NUMBER_OF_PAGES_STATISTICS = "numberOfPages";

        private readonly int numberOfPages;

        /// <summary>
        /// Creates an instance of this class based on the
        /// <see cref="iText.Kernel.Actions.Data.ProductData"/>
        /// and the number of pages.
        /// </summary>
        /// <param name="numberOfPages">the number of pages in the PDF document during the processing of which the event was sent
        ///     </param>
        /// <param name="productData">is a description of the product which has generated an event</param>
        public NumberOfPagesStatisticsEvent(int numberOfPages, ProductData productData)
            : base(productData) {
            if (numberOfPages <= 0) {
                throw new PdfException(PdfException.DocumentHasNoPages);
            }
            this.numberOfPages = numberOfPages;
        }

        /// <summary><inheritDoc/></summary>
        public override AbstractStatisticsAggregator CreateStatisticsAggregatorFromName(String statisticsName) {
            if (NUMBER_OF_PAGES_STATISTICS.Equals(statisticsName)) {
                return new NumberOfPagesStatisticsAggregator();
            }
            return base.CreateStatisticsAggregatorFromName(statisticsName);
        }

        /// <summary><inheritDoc/></summary>
        public override IList<String> GetStatisticsNames() {
            return JavaCollectionsUtil.SingletonList(NUMBER_OF_PAGES_STATISTICS);
        }

        /// <summary>Gets number of pages in the PDF document during the processing of which the event was sent.</summary>
        /// <returns>the number of pages</returns>
        public virtual int GetNumberOfPages() {
            return numberOfPages;
        }
    }
}
