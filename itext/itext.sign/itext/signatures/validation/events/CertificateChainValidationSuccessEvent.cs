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
