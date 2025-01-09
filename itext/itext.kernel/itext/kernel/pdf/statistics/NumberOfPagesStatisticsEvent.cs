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
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Statistics {
    /// <summary>Class which represents event for counting the number of pages in a PDF document.</summary>
    /// <remarks>Class which represents event for counting the number of pages in a PDF document. Only for internal usage.
    ///     </remarks>
    public class NumberOfPagesStatisticsEvent : AbstractStatisticsEvent {
        private const String NUMBER_OF_PAGES_STATISTICS = "numberOfPages";

        private readonly int numberOfPages;

        /// <summary>
        /// Creates an instance of this class based on the
        /// <see cref="iText.Commons.Actions.Data.ProductData"/>
        /// and the number of pages.
        /// </summary>
        /// <param name="numberOfPages">the number of pages in the PDF document during the processing of which the event was sent
        ///     </param>
        /// <param name="productData">is a description of the product which has generated an event</param>
        public NumberOfPagesStatisticsEvent(int numberOfPages, ProductData productData)
            : base(productData) {
            if (numberOfPages < 0) {
                throw new InvalidOperationException(KernelExceptionMessageConstant.NUMBER_OF_PAGES_CAN_NOT_BE_NEGATIVE);
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
