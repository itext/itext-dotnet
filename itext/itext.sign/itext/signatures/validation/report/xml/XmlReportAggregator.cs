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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Signatures.Cms;
using iText.Signatures.Validation.Events;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>Use this implementation when an xml report has to be created.</summary>
    public class XmlReportAggregator : IEventHandler {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Report.Xml.XmlReportAggregator
            ));

        private readonly ValidationObjects validationObjects = new ValidationObjects();

        private readonly PadesValidationReport report;

        private readonly Stack<SubValidationReport> validationReportStack = new Stack<SubValidationReport>();

        /// <summary>Instantiates a new AdESReportEventListener instance.</summary>
        public XmlReportAggregator() {
            report = new PadesValidationReport(validationObjects);
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally
        /// <summary>Returns the generated PadesValidationReport.</summary>
        /// <returns>the generated PadesValidationReport</returns>
        public virtual PadesValidationReport GetReport() {
            return report;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void OnEvent(IEvent rawEvent) {
            if (rawEvent is IValidationEvent) {
                IValidationEvent @event = (IValidationEvent)rawEvent;
                if (@event.GetEventType() != null) {
                    switch (@event.GetEventType()) {
                        case EventType.PROOF_OF_EXISTENCE_FOUND: {
                            ProofOfExistenceFoundEvent poe = (ProofOfExistenceFoundEvent)@event;
                            this.ProofOfExistenceFound(poe.GetTimeStampSignature(), poe.IsDocumentTimestamp());
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_STARTED: {
                            StartSignatureValidationEvent start = (StartSignatureValidationEvent)@event;
                            this.StartSignatureValidation(start.GetPdfSignature().GetContents().GetValueBytes(), start.GetSignatureName
                                (), start.GetSigningDate());
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_SUCCESS: {
                            this.ReportSignatureValidationSuccess();
                            break;
                        }

                        case EventType.SIGNATURE_VALIDATION_FAILURE: {
                            SignatureValidationFailureEvent failure = (SignatureValidationFailureEvent)@event;
                            this.ReportSignatureValidationFailure(failure.IsInconclusive(), failure.GetReason());
                            break;
                        }

                        case EventType.CERTIFICATE_CHAIN_SUCCESS: {
                            break;
                        }

                        case EventType.CERTIFICATE_CHAIN_FAILURE: {
                            break;
                        }

                        case EventType.CRL_VALIDATION_SUCCESS: {
                            break;
                        }

                        case EventType.CRL_VALIDATION_FAILURE: {
                            break;
                        }

                        case EventType.OCSP_VALIDATION_SUCCESS: {
                            break;
                        }

                        case EventType.OCSP_VALIDATION_FAILURE: {
                            break;
                        }
                    }
                }
            }
        }

        private void StartSignatureValidation(byte[] signature, String name, DateTime signingDate) {
            try {
                SignatureValidationReport currentSignatureValidationReport = new SignatureValidationReport(validationObjects
                    , new CMSContainer(signature), name, signingDate);
                validationReportStack.Push(currentSignatureValidationReport);
                report.AddSignatureValidationReport(currentSignatureValidationReport);
            }
            catch (Exception e) {
                // catching generic Exception here for portability
                LOGGER.LogError(e, "Unable to parse signature container.");
                throw new ArgumentException("Signature is not parsable", e);
            }
        }

        private void ProofOfExistenceFound(byte[] timeStampSignature, bool document) {
            try {
                POEValidationReport currentValidationReport = new POEValidationReport(validationObjects, new CMSContainer(
                    timeStampSignature), document);
                validationReportStack.Push(currentValidationReport);
                validationObjects.AddObject(currentValidationReport);
            }
            catch (Exception e) {
                // catching generic Exception here for portability
                LOGGER.LogError(e, "Unable to parse timestamp signature container.");
                throw new ArgumentException("Timestamp signature is not parsable", e);
            }
        }

        private void ReportSignatureValidationSuccess() {
            SignatureValidationStatus status = new SignatureValidationStatus();
            status.SetMainIndication(SignatureValidationStatus.MainIndication.TOTAL_PASSED);
            SubValidationReport currentValidationReport = validationReportStack.Pop();
            currentValidationReport.SetSignatureValidationStatus(status);
        }

        private void ReportSignatureValidationFailure(bool isInconclusive, String reason) {
            SignatureValidationStatus status = new SignatureValidationStatus();
            if (isInconclusive) {
                status.SetMainIndication(SignatureValidationStatus.MainIndication.INDETERMINATE);
            }
            else {
                status.SetMainIndication(SignatureValidationStatus.MainIndication.TOTAL_FAILED);
            }
            status.AddMessage(reason, SignatureValidationStatus.MessageType.ERROR);
            SubValidationReport currentValidationReport = validationReportStack.Pop();
            currentValidationReport.SetSignatureValidationStatus(status);
        }
    }
}
