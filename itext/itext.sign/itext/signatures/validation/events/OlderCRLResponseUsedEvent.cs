using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event triggers for every CRL response from the document that was not in the most recent DSS.
    ///     </summary>
    public class OlderCRLResponseUsedEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate the request CRL response is for</param>
        public OlderCRLResponseUsedEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary><inheritDoc/></summary>
        public override EventType GetEventType() {
            return EventType.CRL_OTHER_INTERNAL_SOURCE_USED;
        }
    }
}
