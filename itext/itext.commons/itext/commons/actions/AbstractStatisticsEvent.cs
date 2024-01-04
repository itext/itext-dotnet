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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Data;
using iText.Commons.Logs;
using iText.Commons.Utils;

namespace iText.Commons.Actions {
    /// <summary>Abstract class which defines statistics event.</summary>
    /// <remarks>Abstract class which defines statistics event. Only for internal usage.</remarks>
    public abstract class AbstractStatisticsEvent : AbstractProductITextEvent {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Commons.Actions.AbstractStatisticsEvent
            ));

        /// <summary>Creates instance of abstract statistics iText event based on passed product data.</summary>
        /// <remarks>Creates instance of abstract statistics iText event based on passed product data. Only for internal usage.
        ///     </remarks>
        /// <param name="productData">is a description of the product which has generated an event</param>
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
        /// <see cref="GetStatisticsNames()"/>
        /// </param>
        /// <returns>
        /// new instance of
        /// <see cref="AbstractStatisticsAggregator"/>
        /// </returns>
        public virtual AbstractStatisticsAggregator CreateStatisticsAggregatorFromName(String statisticsName) {
            LOGGER.LogWarning(MessageFormatUtil.Format(CommonsLogMessageConstant.INVALID_STATISTICS_NAME, statisticsName
                ));
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
