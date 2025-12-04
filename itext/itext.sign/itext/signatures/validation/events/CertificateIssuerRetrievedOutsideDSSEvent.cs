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
    /// <summary>
    /// This event is triggered when a certificate issuer is retrieved from a document resource outside
    /// of the last DSS.
    /// </summary>
    public class CertificateIssuerRetrievedOutsideDSSEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance,</summary>
        /// <param name="certificate">the certificate for which the issuer was retrieved externally</param>
        public CertificateIssuerRetrievedOutsideDSSEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.CERTIFICATE_ISSUER_OTHER_INTERNAL_SOURCE_USED;
        }
    }
}
