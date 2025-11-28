using iText.Commons.Actions;
using iText.Signatures.Validation.Events;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>This class is for internal usage.</summary>
    /// <remarks>
    /// This class is for internal usage.
    /// It bridges the gap between the new event driven system of collecting validation meta info
    /// and the previous interface driven system.
    /// </remarks>
    public class EventsToAdESReportAggratorConvertor : IEventHandler {
        private readonly AdESReportAggregator target;

        /// <summary>Creates a new instance of the convertor, wrapping an AdESReportAggregator implementation.</summary>
        /// <param name="target">an AdESReportAggregator implementation to be wrapped</param>
        public EventsToAdESReportAggratorConvertor(AdESReportAggregator target) {
            this.target = target;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void OnEvent(IEvent rawEvent) {
            if (rawEvent is IValidationEvent) {
                IValidationEvent @event = (IValidationEvent)rawEvent;
                switch ((@event.GetEventType())) {
                    case EventType.SIGNATURE_VALIDATION_STARTED: {
                        StartSignatureValidationEvent startEvent = (StartSignatureValidationEvent)@event;
                        target.StartSignatureValidation(startEvent.GetPdfSignature().GetContents().GetValueBytes(), startEvent.GetSignatureName
                            (), startEvent.GetSigningDate());
                        break;
                    }

                    case EventType.PROOF_OF_EXISTENCE_FOUND: {
                        ProofOfExistenceFoundEvent peoEvent = (ProofOfExistenceFoundEvent)@event;
                        target.ProofOfExistenceFound(peoEvent.GetTimeStampSignature(), peoEvent.IsDocumentTimestamp());
                        break;
                    }

                    case EventType.SIGNATURE_VALIDATION_SUCCESS: {
                        target.ReportSignatureValidationSuccess();
                        break;
                    }

                    case EventType.SIGNATURE_VALIDATION_FAILURE: {
                        SignatureValidationFailureEvent failureEvent = (SignatureValidationFailureEvent)@event;
                        target.ReportSignatureValidationFailure(failureEvent.IsInconclusive(), failureEvent.GetReason());
                        break;
                    }
                }
            }
        }
    }
}
