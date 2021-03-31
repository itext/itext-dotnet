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
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Session;

namespace iText.Kernel.Actions.Processors {
    /// <summary>Interface for product event processors.</summary>
    public interface ITextProductEventProcessor {
        /// <summary>
        /// Handles the
        /// <see cref="iText.Kernel.Actions.Events.AbstractITextProductEvent"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        void OnEvent(AbstractITextProductEvent @event);

        /// <summary>Gets the name of the product to which this processor corresponds.</summary>
        /// <returns>the product name</returns>
        String GetProductName();

        /// <summary>
        /// When document is closing it will search for every
        /// <see cref="ITextProductEventProcessor"/>
        /// associated with the products involved into document processing and may aggregate some data
        /// from them.
        /// </summary>
        /// <remarks>
        /// When document is closing it will search for every
        /// <see cref="ITextProductEventProcessor"/>
        /// associated with the products involved into document processing and may aggregate some data
        /// from them. Aggregation stage is the first stage of closing process. See also the second step:
        /// <see cref="CompletionOnClose(iText.Kernel.Actions.Session.ClosingSession)"/>
        /// </remarks>
        /// <param name="session">is a closing session</param>
        void AggregationOnClose(ClosingSession session);

        /// <summary>
        /// When document is closing it will search for every
        /// <see cref="ITextProductEventProcessor"/>
        /// associated with the products involved into document processing and may aggregate some data
        /// from them.
        /// </summary>
        /// <remarks>
        /// When document is closing it will search for every
        /// <see cref="ITextProductEventProcessor"/>
        /// associated with the products involved into document processing and may aggregate some data
        /// from them. Completion stage is the second stage of closing process. See also the first step:
        /// <see cref="AggregationOnClose(iText.Kernel.Actions.Session.ClosingSession)"/>
        /// </remarks>
        /// <param name="session">is a closing session</param>
        void CompletionOnClose(ClosingSession session);
    }
}
