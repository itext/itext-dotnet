using iText.Commons.Actions;

namespace iText.Signatures.Validation.Events {
    /// <summary>This interface represents events registered during signature validation.</summary>
    public interface IValidationEvent : IEvent {
        /// <summary>Returns the event type of the event, this fields is used to avoid instanceof usage.</summary>
        /// <returns>the event type of the event</returns>
        EventType GetEventType();
    }
}
