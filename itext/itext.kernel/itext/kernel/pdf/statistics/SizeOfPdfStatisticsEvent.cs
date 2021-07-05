using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Actions;
using iText.Kernel.Actions.Data;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Class which represents event related to size of the PDF document.</summary>
    /// <remarks>Class which represents event related to size of the PDF document. Only for internal usage.</remarks>
    public class SizeOfPdfStatisticsEvent : AbstractStatisticsEvent {
        private const String PDF_SIZE_STATISTICS = "pdfSize";

        private readonly long amountOfBytes;

        /// <summary>
        /// Creates an instance of this class based on the
        /// <see cref="iText.Kernel.Actions.Data.ProductData"/>
        /// and the size of the document.
        /// </summary>
        /// <param name="amountOfBytes">the number of bytes in the PDF document during the processing of which the event was sent
        ///     </param>
        /// <param name="productData">is a description of the product which has generated an event</param>
        public SizeOfPdfStatisticsEvent(long amountOfBytes, ProductData productData)
            : base(productData) {
            if (amountOfBytes < 0) {
                throw new ArgumentException(PdfException.AmountOfBytesLessThanZero);
            }
            this.amountOfBytes = amountOfBytes;
        }

        /// <summary><inheritDoc/></summary>
        public override AbstractStatisticsAggregator CreateStatisticsAggregatorFromName(String statisticsName) {
            if (PDF_SIZE_STATISTICS.Equals(statisticsName)) {
                return new SizeOfPdfStatisticsAggregator();
            }
            return base.CreateStatisticsAggregatorFromName(statisticsName);
        }

        /// <summary><inheritDoc/></summary>
        public override IList<String> GetStatisticsNames() {
            return JavaCollectionsUtil.SingletonList(PDF_SIZE_STATISTICS);
        }

        /// <summary>Gets number of bytes in the PDF document during the processing of which the event was sent.</summary>
        /// <returns>the number of pages</returns>
        public virtual long GetAmountOfBytes() {
            return amountOfBytes;
        }
    }
}
