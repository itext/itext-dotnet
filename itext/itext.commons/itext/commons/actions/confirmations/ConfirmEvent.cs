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
using iText.Commons.Actions;
using iText.Commons.Actions.Sequence;

namespace iText.Commons.Actions.Confirmations {
    /// <summary>
    /// Used to confirm that process associated with some
    /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>
    /// ended successfully.
    /// </summary>
    public class ConfirmEvent : AbstractEventWrapper {
        /// <summary>Creates an instance of confirmation event.</summary>
        /// <param name="updatedSequenceId">
        /// is a
        /// <see cref="iText.Commons.Actions.Sequence.SequenceId"/>
        /// for the document. May be different with
        /// sequence id of original event
        /// </param>
        /// <param name="confirmedEvent">is an event to confirm</param>
        public ConfirmEvent(SequenceId updatedSequenceId, AbstractProductProcessITextEvent confirmedEvent)
            : base(updatedSequenceId, confirmedEvent, EventConfirmationType.UNCONFIRMABLE) {
        }

        /// <summary>Creates an instance of confirmation event.</summary>
        /// <param name="confirmedEvent">is an event to confirm</param>
        public ConfirmEvent(AbstractProductProcessITextEvent confirmedEvent)
            : this(confirmedEvent.GetSequenceId(), confirmedEvent) {
        }

        /// <summary>
        /// Returns the
        /// <see cref="iText.Commons.Actions.AbstractProductProcessITextEvent"/>
        /// associated with confirmed process.
        /// </summary>
        /// <returns>confirmed event</returns>
        public virtual AbstractProductProcessITextEvent GetConfirmedEvent() {
            AbstractProductProcessITextEvent @event = GetEvent();
            if (@event is iText.Commons.Actions.Confirmations.ConfirmEvent) {
                return ((iText.Commons.Actions.Confirmations.ConfirmEvent)@event).GetConfirmedEvent();
            }
            return @event;
        }
    }
}
