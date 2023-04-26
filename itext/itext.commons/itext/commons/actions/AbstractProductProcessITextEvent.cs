/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;

namespace iText.Commons.Actions {
    /// <summary>Abstract class which defines product process event.</summary>
    /// <remarks>Abstract class which defines product process event. Only for internal usage.</remarks>
    public abstract class AbstractProductProcessITextEvent : AbstractContextBasedITextEvent {
        private readonly WeakReference sequenceId;

        private readonly EventConfirmationType confirmationType;

        /// <summary>
        /// Creates an event associated with
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <remarks>
        /// Creates an event associated with
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>
        /// . It may contain auxiliary meta data.
        /// </remarks>
        /// <param name="sequenceId">is a general identifier for the event</param>
        /// <param name="productData">is a description of the product which has generated an event</param>
        /// <param name="metaInfo">is an auxiliary meta info</param>
        /// <param name="confirmationType">
        /// defines when the event should be confirmed to notify that the
        /// associated process has finished successfully
        /// </param>
        protected internal AbstractProductProcessITextEvent(SequenceId sequenceId, ProductData productData, IMetaInfo
             metaInfo, EventConfirmationType confirmationType)
            : base(productData, metaInfo) {
            this.sequenceId = new WeakReference(sequenceId);
            this.confirmationType = confirmationType;
        }

        /// <summary>Creates an event which is not associated with any object.</summary>
        /// <remarks>Creates an event which is not associated with any object. It may contain auxiliary meta data.</remarks>
        /// <param name="productData">is a description of the product which has generated an event</param>
        /// <param name="metaInfo">is an auxiliary meta info</param>
        /// <param name="confirmationType">
        /// defines when the event should be confirmed to notify that the
        /// associated process has finished successfully
        /// </param>
        protected internal AbstractProductProcessITextEvent(ProductData productData, IMetaInfo metaInfo, EventConfirmationType
             confirmationType)
            : this(null, productData, metaInfo, confirmationType) {
        }

        /// <summary>Retrieves an identifier of event source.</summary>
        /// <returns>an identifier of event source</returns>
        public virtual SequenceId GetSequenceId() {
            return (SequenceId)sequenceId.Target;
        }

        /// <summary>Returns an event type.</summary>
        /// <returns>event type</returns>
        public abstract String GetEventType();

        /// <summary>
        /// Retrieves an
        /// <see cref="iText.Commons.Actions.Confirmations.EventConfirmationType">event confirmation type</see>.
        /// </summary>
        /// <returns>
        /// value of event confirmation type which defines when the event should be confirmed
        /// to notify that the associated process has finished successfully
        /// </returns>
        public virtual EventConfirmationType GetConfirmationType() {
            return confirmationType;
        }
    }
}
