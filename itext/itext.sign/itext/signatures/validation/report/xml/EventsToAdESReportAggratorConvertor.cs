/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using iText.Commons.Actions;
using iText.Signatures.Validation.Events;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>This class is for internal usage.</summary>
    /// <remarks>
    /// This class is for internal usage.
    /// <para />
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
                if (@event.GetEventType() != null) {
                    switch (@event.GetEventType()) {
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
}
