using System;

namespace iText.Kernel.Actions {
    /// <summary>Abstract class for statistics aggregation.</summary>
    /// <remarks>Abstract class for statistics aggregation. Note that aggregator class must be thread safe.</remarks>
    public abstract class AbstractStatisticsAggregator {
        /// <summary>Aggregates data from the provided event.</summary>
        /// <param name="event">
        /// 
        /// <see cref="AbstractStatisticsEvent"/>
        /// instance
        /// </param>
        public abstract void Aggregate(AbstractStatisticsEvent @event);

        /// <summary>Retrieves aggregated data.</summary>
        /// <returns>
        /// aggregated data as
        /// <see cref="System.Object"/>
        /// </returns>
        public abstract Object RetrieveAggregation();
    }
}
