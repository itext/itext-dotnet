using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event triggered when revocation data from a timestamped DSS is not enough to perform signature validation.
    ///     </summary>
    public class DssNotTimestampedEvent : AbstractCertificateChainEvent {
        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the certificate for which there is no timestamped DSS</param>
        public DssNotTimestampedEvent(IX509Certificate certificate)
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
            return EventType.DSS_NOT_TIMESTAMPED;
        }
    }
}
