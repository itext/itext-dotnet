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

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered after signature validation failed for the current signature.</summary>
    public class SignatureValidationFailureEvent : IValidationEvent {
        private readonly bool isInconclusive;

        private readonly String reason;

        /// <summary>Create a new event instance.</summary>
        /// <param name="isInconclusive">
        /// 
        /// <see langword="true"/>
        /// when validation is neither valid nor invalid,
        /// <see langword="false"/>
        /// when it is invalid
        /// </param>
        /// <param name="reason">the failure reason</param>
        public SignatureValidationFailureEvent(bool isInconclusive, String reason) {
            this.isInconclusive = isInconclusive;
            this.reason = reason;
        }

        /// <summary>Returns whether the result was inconclusive.</summary>
        /// <returns>whether the result was inconclusive</returns>
        public virtual bool IsInconclusive() {
            return isInconclusive;
        }

        /// <summary>Returns the reason of the failure.</summary>
        /// <returns>the reason of the failure</returns>
        public virtual String GetReason() {
            return reason;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.SIGNATURE_VALIDATION_FAILURE;
        }
    }
}
