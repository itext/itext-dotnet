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
    /// <summary>This event is triggered when a timestamp signature is encountered.</summary>
    public class ProofOfExistenceFoundEvent : IValidationEvent {
        private readonly byte[] timeStampSignature;

        private readonly PdfSignature sig;

        private readonly bool document;

        /// <summary>Creates a new event instance for a document timestamp.</summary>
        /// <param name="sig">
        /// the PdfSignature containing the timestamp signature,
        /// only applicable for document signatures
        /// </param>
        /// <param name="signatureName">signature name, only applicable for document signatures</param>
        public ProofOfExistenceFoundEvent(PdfSignature sig, String signatureName) {
            this.sig = sig;
            this.timeStampSignature = sig.GetContents().GetValueBytes();
            this.document = true;
        }

        /// <summary>Creates a new event instance for a signature timestamp.</summary>
        /// <param name="timeStampSignature">timestamp container as a byte[]</param>
        public ProofOfExistenceFoundEvent(byte[] timeStampSignature) {
            this.timeStampSignature = timeStampSignature;
            this.sig = null;
            this.document = false;
        }

        /// <summary>Returns the encoded timestamp signature.</summary>
        /// <returns>the encoded timestamp signature</returns>
        public virtual byte[] GetTimeStampSignature() {
            return timeStampSignature;
        }

        /// <summary>Returns whether this is a document timestamp.</summary>
        /// <returns>whether this is a document timestamp</returns>
        public virtual bool IsDocumentTimestamp() {
            return document;
        }

        /// <summary>Returns the PdfSignature containing the timestamp signature.</summary>
        /// <returns>the PdfSignature containing the timestamp signature</returns>
        public virtual PdfSignature GetPdfSignature() {
            return sig;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.PROOF_OF_EXISTENCE_FOUND;
        }
    }
}
