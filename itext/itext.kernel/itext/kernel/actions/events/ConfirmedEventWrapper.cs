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

namespace iText.Kernel.Actions.Events {
    /// <summary>
    /// A wrapper for a
    /// <see cref="AbstractProductProcessITextEvent"/>
    /// storing additional data about the event.
    /// </summary>
    /// <remarks>
    /// A wrapper for a
    /// <see cref="AbstractProductProcessITextEvent"/>
    /// storing additional data about the event.
    /// If wrapped event is immutable then the instance of the wrapper is immutable too.
    /// </remarks>
    public class ConfirmedEventWrapper : AbstractProductProcessITextEvent {
        private readonly AbstractProductProcessITextEvent @event;

        private readonly String productUsageType;

        private readonly String producerLine;

        /// <summary>Creates a wrapper for the event with additional data about the event.</summary>
        /// <param name="event">
        /// is a
        /// <see cref="AbstractProductProcessITextEvent"/>
        /// to wrap
        /// </param>
        /// <param name="productUsageType">is a product usage marker</param>
        /// <param name="producerLine">
        /// is a producer line defined by the
        /// <see cref="iText.Kernel.Actions.Processors.ITextProductEventProcessor"/>
        /// which registered the event
        /// </param>
        public ConfirmedEventWrapper(AbstractProductProcessITextEvent @event, String productUsageType, String producerLine
            )
            : base(@event.GetSequenceId(), @event.GetProductData(), @event.GetMetaInfo(), EventConfirmationType.UNCONFIRMABLE
                ) {
            this.@event = @event;
            this.productUsageType = productUsageType;
            this.producerLine = producerLine;
        }

        /// <summary>Obtains the wrapped event.</summary>
        /// <returns>wrapped event</returns>
        public virtual AbstractProductProcessITextEvent GetEvent() {
            return @event;
        }

        /// <summary>Obtains the license type for the product which generated the event.</summary>
        /// <returns>product usage type</returns>
        public virtual String GetProductUsageType() {
            return productUsageType;
        }

        /// <summary>
        /// Gets producer line defined by the
        /// <see cref="iText.Kernel.Actions.Processors.ITextProductEventProcessor"/>
        /// which registered the
        /// event.
        /// </summary>
        /// <returns>producer line</returns>
        public virtual String GetProducerLine() {
            return producerLine;
        }

        public override String GetEventType() {
            return @event.GetEventType();
        }

        public override String GetProductName() {
            return @event.GetProductName();
        }
    }
}
