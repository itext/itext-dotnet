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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;

namespace iText.Commons.Actions {
    /// <summary>Base class to wrap events.</summary>
    public abstract class AbstractEventWrapper : AbstractProductProcessITextEvent {
        private readonly AbstractProductProcessITextEvent @event;

        /// <summary>Creates a wrapper for the event.</summary>
        /// <param name="event">
        /// is a
        /// <see cref="AbstractProductProcessITextEvent"/>
        /// to wrap
        /// </param>
        /// <param name="confirmationType">event confirmation type</param>
        protected internal AbstractEventWrapper(AbstractProductProcessITextEvent @event, EventConfirmationType confirmationType
            )
            : base(@event.GetSequenceId(), @event.GetProductData(), @event.GetMetaInfo(), confirmationType) {
            this.@event = @event;
        }

        /// <summary>
        /// Creates a wrapper of event associated with
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>.
        /// </summary>
        /// <param name="updatedSequenceId">
        /// is a
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>
        /// for the document. May be different with
        /// sequence id of original event
        /// </param>
        /// <param name="event">
        /// is a
        /// <see cref="AbstractProductProcessITextEvent"/>
        /// to wrap
        /// </param>
        /// <param name="confirmationType">event confirmation type</param>
        protected internal AbstractEventWrapper(SequenceId updatedSequenceId, AbstractProductProcessITextEvent @event
            , EventConfirmationType confirmationType)
            : base(updatedSequenceId, @event.GetProductData(), @event.GetMetaInfo(), confirmationType) {
            this.@event = @event;
        }

        /// <summary>Obtains the wrapped event.</summary>
        /// <returns>wrapped event</returns>
        public virtual AbstractProductProcessITextEvent GetEvent() {
            return @event;
        }

        public override Type GetClassFromContext() {
            return GetEvent().GetClassFromContext();
        }

        public override String GetEventType() {
            return GetEvent().GetEventType();
        }
    }
}
