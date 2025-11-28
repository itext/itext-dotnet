using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>
    /// This event is triggered when an OCSP response is used from another
    /// document source than the latest DSS.
    /// </summary>
    public class OlderOCSPResponseUsedEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate for which the OCSP response was used</param>
        public OlderOCSPResponseUsedEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.OCSP_OTHER_INTERNAL_SOURCE_USED;
        }
    }
}
