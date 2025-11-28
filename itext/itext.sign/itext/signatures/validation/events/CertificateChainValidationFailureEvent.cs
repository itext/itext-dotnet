using System;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered when a certificates chain validation fails.</summary>
    public class CertificateChainValidationFailureEvent : IValidationEvent {
        private readonly IX509Certificate certificate;

        private readonly bool isInconclusive;

        private readonly String reason;

        /// <summary>Creates a new event instance.</summary>
        /// <param name="certificate">the validated certificate</param>
        /// <param name="isInconclusive">whether the validation result was conclusive</param>
        /// <param name="reason">the reason the validation failed</param>
        public CertificateChainValidationFailureEvent(IX509Certificate certificate, bool isInconclusive, String reason
            ) {
            this.certificate = certificate;
            this.isInconclusive = isInconclusive;
            this.reason = reason;
        }

        /// <summary>Returns the validated certificate.</summary>
        /// <returns>the validated certificate</returns>
        public virtual IX509Certificate GetCertificate() {
            return certificate;
        }

        /// <summary>Returns whether the validation result was conclusive.</summary>
        /// <returns>whether the validation result was conclusive</returns>
        public virtual bool IsInconclusive() {
            return isInconclusive;
        }

        /// <summary>Returns the reason the validation failed.</summary>
        /// <returns>the reason the validation failed</returns>
        public virtual String GetReason() {
            return reason;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.CERTIFICATE_CHAIN_FAILURE;
        }
    }
}
