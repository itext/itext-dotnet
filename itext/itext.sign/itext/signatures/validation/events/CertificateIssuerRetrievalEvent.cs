using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when a certificate issues was retrieved from the internet.</summary>
    public class CertificateIssuerRetrievalEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate for which the issuer was retrieved externally</param>
        public CertificateIssuerRetrievalEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.CERTIFICATE_ISSUER_EXTERNAL_RETRIEVAL;
        }
    }
}
