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
