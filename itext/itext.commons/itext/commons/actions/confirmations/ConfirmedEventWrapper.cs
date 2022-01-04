/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Commons.Actions;

namespace iText.Commons.Actions.Confirmations {
    /// <summary>
    /// A wrapper for a
    /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>
    /// storing additional data about the event.
    /// </summary>
    /// <remarks>
    /// A wrapper for a
    /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>
    /// storing additional data about the event.
    /// If wrapped event is immutable then the instance of the wrapper is immutable too.
    /// </remarks>
    public class ConfirmedEventWrapper : AbstractEventWrapper {
        private readonly String productUsageType;

        private readonly String producerLine;

        /// <summary>Creates a wrapper for the event with additional data about the event.</summary>
        /// <param name="event">
        /// is a
        /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>
        /// to wrap
        /// </param>
        /// <param name="productUsageType">is a product usage marker</param>
        /// <param name="producerLine">
        /// is a producer line defined by the
        /// <see cref="iText.Commons.Actions.Processors.ITextProductEventProcessor"/>
        /// which registered the event
        /// </param>
        public ConfirmedEventWrapper(AbstractProductProcessITextEvent @event, String productUsageType, String producerLine
            )
            : base(@event, EventConfirmationType.UNCONFIRMABLE) {
            this.productUsageType = productUsageType;
            this.producerLine = producerLine;
        }

        /// <summary>Obtains the license type for the product which generated the event.</summary>
        /// <returns>product usage type</returns>
        public virtual String GetProductUsageType() {
            return productUsageType;
        }

        /// <summary>
        /// Gets producer line defined by the
        /// <see cref="iText.Commons.Actions.Processors.ITextProductEventProcessor"/>
        /// which registered the
        /// event.
        /// </summary>
        /// <returns>producer line</returns>
        public virtual String GetProducerLine() {
            return producerLine;
        }
    }
}
