using System;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered after signature validation failed for the current signature.</summary>
    public class SignatureValidationFailureEvent : IValidationEvent {
        private readonly bool isInconclusive;

        private readonly String reason;

        /// <summary>Create a new event instance.</summary>
        /// <param name="isInconclusive">
        /// 
        /// <see langword="true"/>
        /// when validation is neither valid nor invalid,
        /// <see langword="false"/>
        /// when it is invalid
        /// </param>
        /// <param name="reason">the failure reason</param>
        public SignatureValidationFailureEvent(bool isInconclusive, String reason) {
            this.isInconclusive = isInconclusive;
            this.reason = reason;
        }

        /// <summary>Returns whether the result was inconclusive.</summary>
        /// <returns>whether the result was inconclusive</returns>
        public virtual bool IsInconclusive() {
            return isInconclusive;
        }

        /// <summary>Returns the reason of the failure.</summary>
        /// <returns>the reason of the failure</returns>
        public virtual String GetReason() {
            return reason;
        }

        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.SIGNATURE_VALIDATION_FAILURE;
        }
    }
}
