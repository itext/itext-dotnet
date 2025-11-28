using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when an OCSP request is made instead of using a local resource.</summary>
    public class OCSPResponseRetrievalEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate for which the OCSP request was performed</param>
        public OCSPResponseRetrievalEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.OCSP_REQUEST;
        }
    }
}
