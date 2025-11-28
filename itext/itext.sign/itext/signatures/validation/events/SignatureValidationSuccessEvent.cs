namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered after a successful validation of the current signature.</summary>
    public class SignatureValidationSuccessEvent : IValidationEvent {
        public SignatureValidationSuccessEvent() {
        }

        // empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual EventType GetEventType() {
            return EventType.SIGNATURE_VALIDATION_SUCCESS;
        }
    }
}
