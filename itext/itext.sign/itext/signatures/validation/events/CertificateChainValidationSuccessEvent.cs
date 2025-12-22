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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered after certificate chain validation success for the current signature.</summary>
    public class CertificateChainValidationSuccessEvent : IValidationEvent {
        private readonly IX509Certificate certificate;

        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate that was tested</param>
        public CertificateChainValidationSuccessEvent(IX509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>returns the validated certificate.</summary>
        /// <returns>the validated certificate</returns>
        public virtual IX509Certificate GetCertificate() {
            return certificate;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.CERTIFICATE_CHAIN_SUCCESS;
        }
    }
}
