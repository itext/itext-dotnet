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
using iText.Signatures.Cms;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>Use this implementation when an xml report has to be created</summary>
    public class DefaultAdESReportAggregator : AdESReportAggregator {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.Validation.Report.Xml.DefaultAdESReportAggregator
            ));

        private readonly ValidationObjects validationObjects = new ValidationObjects();

        private readonly PadesValidationReport report;

        private readonly Stack<SubValidationReport> validationReportStack = new Stack<SubValidationReport>();

        /// <summary>Instantiates a new DefaultAdESReportAggregator instance</summary>
        public DefaultAdESReportAggregator() {
            report = new PadesValidationReport(validationObjects);
        }

        // Declaring default constructor explicitly to avoid removing it unintentionally
        public virtual void StartSignatureValidation(byte[] signature, String name, DateTime signingDate) {
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

        public virtual void ProofOfExistenceFound(byte[] timeStampSignature, bool document) {
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

        public virtual void ReportSignatureValidationSuccess() {
            SignatureValidationStatus status = new SignatureValidationStatus();
            status.SetMainIndication(SignatureValidationStatus.MainIndication.TOTAL_PASSED);
            SubValidationReport currentValidationReport = validationReportStack.Pop();
            currentValidationReport.SetSignatureValidationStatus(status);
        }

        public virtual void ReportSignatureValidationFailure(bool isInconclusive, String reason) {
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

        //  code for future use commented out for code coverage
        //    @Override
        //    public void reportCertificateChainValidationSuccess(X509Certificate certificate) {
        //        //will be completed later
        //    }
        //
        //    @Override
        //    public void reportCertificateChainValidationFailure(X509Certificate certificate, boolean isInconclusive,
        //            String reason) {
        //        //will be completed later
        //    }
        //
        //    @Override
        //    public void reportCRLValidationSuccess(X509Certificate certificate, CRL crl) {
        //        //will be completed later
        //    }
        //
        //    @Override
        //    public void reportCRLValidationFailure(X509Certificate certificate, CRL crl, boolean isInconclusive,
        //            String reason) {
        //        //will be completed later
        //    }
        //
        //    @Override
        //    public void reportOCSPValidationSuccess(X509Certificate certificate, IBasicOCSPResp ocsp) {
        //        //will be completed later
        //    }
        //
        //    @Override
        //    public void reportOCSPValidationFailure(X509Certificate certificate, IBasicOCSPResp ocsp, boolean isInconclusive,
        //            String reason) {
        //        //will be completed later
        //    }
        public virtual PadesValidationReport GetReport() {
            return report;
        }
    }
}
