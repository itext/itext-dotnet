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
