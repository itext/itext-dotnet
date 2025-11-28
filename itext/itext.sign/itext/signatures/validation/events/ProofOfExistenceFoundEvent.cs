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
