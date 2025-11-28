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
