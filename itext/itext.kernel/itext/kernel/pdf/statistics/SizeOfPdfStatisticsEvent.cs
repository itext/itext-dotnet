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
using iText.Commons.Actions;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Class which represents event related to size of the PDF document.</summary>
    /// <remarks>Class which represents event related to size of the PDF document. Only for internal usage.</remarks>
    public class SizeOfPdfStatisticsEvent : AbstractStatisticsEvent {
        private const String PDF_SIZE_STATISTICS = "pdfSize";

        private readonly long amountOfBytes;

        /// <summary>
        /// Creates an instance of this class based on the
        /// <see cref="iText.Commons.Actions.Data.ProductData"/>
        /// and the size of the document.
        /// </summary>
        /// <param name="amountOfBytes">the number of bytes in the PDF document during the processing of which the event was sent
        ///     </param>
        /// <param name="productData">is a description of the product which has generated an event</param>
        public SizeOfPdfStatisticsEvent(long amountOfBytes, ProductData productData)
            : base(productData) {
            if (amountOfBytes < 0) {
                throw new ArgumentException(KernelExceptionMessageConstant.AMOUNT_OF_BYTES_LESS_THAN_ZERO);
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
