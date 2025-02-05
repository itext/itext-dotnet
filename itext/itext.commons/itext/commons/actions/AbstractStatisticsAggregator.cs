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

namespace iText.Commons.Actions {
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

        /// <summary>Merges data from the provided aggregator into this aggregator.</summary>
        /// <param name="aggregator">from which data will be taken.</param>
        public abstract void Merge(AbstractStatisticsAggregator aggregator);
    }
}
