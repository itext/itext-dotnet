/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Actions.Data;

namespace iText.Kernel.Actions {
    /// <summary>Abstract class which defines statistics event.</summary>
    /// <remarks>Abstract class which defines statistics event. Only for internal usage.</remarks>
    public abstract class AbstractStatisticsEvent : AbstractProductITextEvent {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Kernel.Actions.AbstractStatisticsEvent
            ));

        /// <seealso cref="AbstractProductITextEvent.AbstractProductITextEvent(iText.Kernel.Actions.Data.ProductData)"
        ///     />
        protected internal AbstractStatisticsEvent(ProductData productData)
            : base(productData) {
        }

        /// <summary>Creates statistics aggregator based on provided statistics name.</summary>
        /// <remarks>
        /// Creates statistics aggregator based on provided statistics name.
        /// By default prints log warning and returns <c>null</c>.
        /// </remarks>
        /// <param name="statisticsName">
        /// name of statistics based on which aggregator will be created.
        /// Shall be one of those returned from
        /// <see>this#getStatisticsNames()</see>
        /// </param>
        /// <returns>
        /// new instance of
        /// <see cref="AbstractStatisticsAggregator"/>
        /// </returns>
        public virtual AbstractStatisticsAggregator CreateStatisticsAggregatorFromName(String statisticsName) {
            LOGGER.Warn(MessageFormatUtil.Format(KernelLogMessageConstant.INVALID_STATISTICS_NAME, statisticsName));
            return null;
        }

        /// <summary>Gets all statistics names related to this event.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of statistics names
        /// </returns>
        public abstract IList<String> GetStatisticsNames();
    }
}
