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
using iText.Kernel.Actions.Sequence;

namespace iText.Kernel.Actions.Events {
    /// <summary>
    /// Used to confirm that process associated with some
    /// <see cref="AbstractProductProcessITextEvent"/>
    /// ended successfully.
    /// </summary>
    public class ConfirmEvent : AbstractProductProcessITextEvent {
        private readonly AbstractProductProcessITextEvent confirmedEvent;

        /// <summary>Creates an instance of confirmation event.</summary>
        /// <param name="updatedSequenceId">
        /// is a
        /// <see cref="iText.Kernel.Actions.Sequence.SequenceId"/>
        /// for the document. May be different with
        /// sequence id of original event if
        /// <see cref="LinkDocumentIdEvent"/>
        /// was generated before to link some events with another document
        /// </param>
        /// <param name="confirmedEvent">is an event to confirm</param>
        public ConfirmEvent(SequenceId updatedSequenceId, AbstractProductProcessITextEvent confirmedEvent)
            : base(updatedSequenceId, confirmedEvent.GetProductData(), confirmedEvent.GetMetaInfo(), EventConfirmationType
                .UNCONFIRMABLE) {
            this.confirmedEvent = confirmedEvent;
        }

        /// <summary>Creates an instance of confirmation event.</summary>
        /// <param name="confirmedEvent">is an event to confirm</param>
        public ConfirmEvent(AbstractProductProcessITextEvent confirmedEvent)
            : this(confirmedEvent.GetSequenceId(), confirmedEvent) {
        }

        public override String GetEventType() {
            return confirmedEvent.GetEventType();
        }

        public override String GetProductName() {
            return confirmedEvent.GetProductName();
        }

        /// <summary>
        /// Returns the
        /// <see cref="AbstractProductProcessITextEvent"/>
        /// associated with confirmed process.
        /// </summary>
        /// <returns>confirmed event</returns>
        public virtual AbstractProductProcessITextEvent GetConfirmedEvent() {
            if (confirmedEvent is iText.Kernel.Actions.Events.ConfirmEvent) {
                return ((iText.Kernel.Actions.Events.ConfirmEvent)confirmedEvent).GetConfirmedEvent();
            }
            return confirmedEvent;
        }

        public override Type GetClassFromContext() {
            return confirmedEvent.GetClassFromContext();
        }
    }
}
