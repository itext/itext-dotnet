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
using iText.Signatures;

namespace iText.Signatures.Validation.Events {
    /// <summary>
    /// This event is triggered at the start of a signature validation,
    /// after successfully parsing the signature.
    /// </summary>
    public class StartSignatureValidationEvent : IValidationEvent {
        private readonly PdfSignature sig;

        private readonly String signatureName;

        private readonly DateTime signingDate;

        /// <summary>Creates a new event instance.</summary>
        /// <param name="sig">the PdfSignature containing the signature</param>
        /// <param name="signatureName">signature name</param>
        /// <param name="signingDate">the signing date</param>
        public StartSignatureValidationEvent(PdfSignature sig, String signatureName, DateTime signingDate) {
            this.sig = sig;
            this.signatureName = signatureName;
            this.signingDate = signingDate;
        }

        /// <summary>Returns the PdfSignature containing the signature.</summary>
        /// <returns>the PdfSignature containing the signature</returns>
        public virtual PdfSignature GetPdfSignature() {
            return sig;
        }

        /// <summary>Returns the signature name.</summary>
        /// <returns>the signature name</returns>
        public virtual String GetSignatureName() {
            return signatureName;
        }

        /// <summary>Returns the claimed signing date.</summary>
        /// <returns>the claimed signing date</returns>
        public virtual DateTime GetSigningDate() {
            return signingDate;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.SIGNATURE_VALIDATION_STARTED;
        }
    }
}
