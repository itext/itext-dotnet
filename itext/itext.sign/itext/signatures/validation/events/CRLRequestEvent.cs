using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when a CRL request is executed instead of using a local resource.</summary>
    public class CRLRequestEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate for which the CRL request was performed</param>
        public CRLRequestEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.CRL_REQUEST;
        }
    }
}
