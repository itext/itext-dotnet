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
