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
    /// <summary>A parent for all events issued during certificate chain validation</summary>
    public abstract class AbstractCertificateChainEvent : IValidationEvent {
        private readonly IX509Certificate certificate;

        /// <summary>Create a new instance.</summary>
        /// <param name="certificate">the certificate that is being validated</param>
        protected internal AbstractCertificateChainEvent(IX509Certificate certificate) {
            this.certificate = certificate;
        }

        /// <summary>Returns the certificate for which the event was fired.</summary>
        /// <returns>the certificate for which the event was fired</returns>
        public virtual IX509Certificate GetCertificate() {
            return certificate;
        }

        public abstract EventType GetEventType();
    }
}
