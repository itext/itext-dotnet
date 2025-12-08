using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when revocation data is retrieved from anywhere except for latest DSS entry.
    ///     </summary>
    public class RevocationNotFromDssEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance,</summary>
        /// <param name="certificate">the certificate for which there is not revocation data in the DSS</param>
        public RevocationNotFromDssEvent(IX509Certificate certificate)
            : base(certificate) {
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override EventType GetEventType() {
            return EventType.REVOCATION_NOT_FROM_DSS;
        }
    }
}
