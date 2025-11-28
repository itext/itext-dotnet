using iText.Kernel.Pdf;

namespace iText.Signatures.Validation.Events {
    /// <summary>This event is triggered after the most recent DSS is being read.</summary>
    public class DSSProcessedEvent : IValidationEvent {
        private readonly PdfDictionary dss;

        /// <summary>Creates a new instance.</summary>
        /// <param name="dss">the dss that was read</param>
        public DSSProcessedEvent(PdfDictionary dss) {
            this.dss = dss;
        }

        public virtual EventType GetEventType() {
            return EventType.DSS_ENTRY_PROCESSED;
        }

        /// <summary>Returns the DSS.</summary>
        /// <returns>the DSS</returns>
        public virtual PdfDictionary GetDss() {
            return dss;
        }
    }
}
