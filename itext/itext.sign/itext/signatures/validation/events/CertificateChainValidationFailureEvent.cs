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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when a certificates chain validation fails.</summary>
    public class CertificateChainValidationFailureEvent : IValidationEvent {
        private readonly IX509Certificate certificate;

        private readonly bool isInconclusive;

        private readonly String reason;

        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the validated certificate</param>
        /// <param name="isInconclusive">whether the validation result was conclusive</param>
        /// <param name="reason">the reason the validation failed</param>
        public CertificateChainValidationFailureEvent(IX509Certificate certificate, bool isInconclusive, String reason
            ) {
            this.certificate = certificate;
            this.isInconclusive = isInconclusive;
            this.reason = reason;
        }

        /// <summary>Returns the validated certificate.</summary>
        /// <returns>the validated certificate</returns>
        public virtual IX509Certificate GetCertificate() {
            return certificate;
        }

        /// <summary>Returns whether the validation result was conclusive.</summary>
        /// <returns>whether the validation result was conclusive</returns>
        public virtual bool IsInconclusive() {
            return isInconclusive;
        }

        /// <summary>Returns the reason the validation failed.</summary>
        /// <returns>the reason the validation failed</returns>
        public virtual String GetReason() {
            return reason;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.CERTIFICATE_CHAIN_FAILURE;
        }
    }
}
