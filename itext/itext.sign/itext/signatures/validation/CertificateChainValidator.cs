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
using System.Linq;
using iText.Commons.Actions;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.Signatures;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Dataorigin;
using iText.Signatures.Validation.Events;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Lotl;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Validator class, which is expected to be used for certificates chain validation.</summary>
    public class CertificateChainValidator {
        private readonly SignatureValidationProperties properties;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly RevocationDataValidator revocationDataValidator;

        private readonly LotlTrustedStore lotlTrustedStore;

        private readonly EventManager eventManager;

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_CHECK = "Certificate check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String VALIDITY_CHECK = "Certificate validity period check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSIONS_CHECK = "Required certificate extensions check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_TRUSTED = "Certificate {0} is trusted, revocation data checks are not required.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT = "Certificate {0} is trusted for {1}, " +
             "but it is not used in this context. Validation will continue as usual.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSION_MISSING = "Required extension validation failed: {0}";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_MISSING = "Certificate {0} isn't trusted and issuer certificate isn't provided.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXPIRED_CERTIFICATE = "Certificate {0} is expired.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_YET_VALID_CERTIFICATE = "Certificate {0} is not yet valid.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_CANNOT_BE_VERIFIED = "Issuer certificate {0} for subject certificate {1} cannot be mathematically verified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_VERIFICATION_FAILED = "Unexpected exception occurred while verifying issuer certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_RETRIEVAL_FAILED = "Unexpected exception occurred while retrieving certificate issuer from IssuingCertificateRetriever.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TRUSTSTORE_RETRIEVAL_FAILED = "Unexpected exception occurred while retrieving trust store from IssuingCertificateRetriever.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVOCATION_VALIDATION_FAILED = "Unexpected exception occurred while validating certificate revocation.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String VALIDITY_PERIOD_CHECK_FAILED = "Unexpected exception occurred while validating certificate validity period.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_RETRIEVER_ORIGIN = "Trusted Certificate is taken from manually configured Trust List.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_LOTL_ORIGIN = "Trusted Certificate is taken from European Union List of Trusted Certificates.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_CUSTOM_ORIGIN = "Trusted Certificate is taken from {0}.";
//\endcond

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateChainValidator"/>.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal CertificateChainValidator(ValidatorChainBuilder builder) {
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.revocationDataValidator = builder.GetRevocationDataValidator();
            this.lotlTrustedStore = builder.GetLotlTrustedStore();
            this.eventManager = builder.GetEventManager();
        }

        /// <summary>Validate given certificate using provided validation date and required extensions.</summary>
        /// <param name="context">the validation context in which to validate the certificate chain</param>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// against which certificate is expected to be validated. Usually signing
        /// date
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains detailed validation results.
        /// </returns>
        public virtual ValidationReport ValidateCertificate(ValidationContext context, IX509Certificate certificate
            , DateTime validationDate) {
            ValidationReport result = new ValidationReport();
            return Validate(result, context, certificate, validationDate);
        }

        /// <summary>Validate given certificate using provided validation date and required extensions.</summary>
        /// <remarks>
        /// Validate given certificate using provided validation date and required extensions.
        /// Result is added into provided report.
        /// </remarks>
        /// <param name="result">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which is populated with detailed validation results
        /// </param>
        /// <param name="context">the context in which to perform the validation</param>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// against which certificate is expected to be validated. Usually signing
        /// date
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains both provided and new validation results.
        /// </returns>
        public virtual ValidationReport Validate(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime validationDate) {
            return Validate(result, context, certificate, validationDate, new List<IX509Certificate>());
        }

        private ValidationReport Validate(ValidationReport result, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate, IList<IX509Certificate> previousCertificates) {
            ReportAlgorithmUsage(certificate);
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR);
            ValidateRequiredExtensions(result, localContext, certificate, previousCertificates.Count);
            if (StopValidation(result, localContext)) {
                return result;
            }
            if (SafeCalling.OnExceptionLog(() => CheckIfCertIsTrusted(result, localContext, certificate, validationDate
                , previousCertificates), false, result, (e) => new CertificateReportItem(certificate, CERTIFICATE_CHECK
                , TRUSTSTORE_RETRIEVAL_FAILED, e, ReportItem.ReportItemStatus.INFO))) {
                return result;
            }
            HandlePadesEvents(certificate);
            ValidateValidityPeriod(result, certificate, validationDate);
            ValidateRevocationData(result, localContext, certificate, validationDate);
            if (StopValidation(result, localContext)) {
                return result;
            }
            ValidateChain(result, localContext, certificate, validationDate, previousCertificates);
            return result;
        }

        private void ReportAlgorithmUsage(IX509Certificate certificate) {
            eventManager.OnEvent(new AlgorithmUsageEvent(certificate.GetSigAlgName(), certificate.GetSigAlgOID(), CERTIFICATE_CHECK
                ));
        }

        private void HandlePadesEvents(IX509Certificate certificate) {
            CertificateOrigin? certificateOrigin = certificateRetriever.GetCertificateOrigin(certificate);
            if (certificateOrigin == CertificateOrigin.OTHER) {
                eventManager.OnEvent(new CertificateIssuerExternalRetrievalEvent(certificate));
            }
            else {
                if (certificateOrigin != CertificateOrigin.LATEST_DSS) {
                    eventManager.OnEvent(new CertificateIssuerRetrievedOutsideDSSEvent(certificate));
                }
            }
        }

        private bool CheckIfCertIsTrusted(ValidationReport result, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate, IList<IX509Certificate> previousCertificates) {
            if (certificateRetriever.GetTrustedCertificatesStore().CheckIfCertIsTrusted(result, context, certificate)) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, CERTIFICATE_RETRIEVER_ORIGIN
                    , ReportItem.ReportItemStatus.INFO));
                return true;
            }
            if (lotlTrustedStore == null) {
                return false;
            }
            if (lotlTrustedStore.SetPreviousCertificates(previousCertificates).CheckIfCertIsTrusted(result, context, certificate
                , validationDate)) {
                if (lotlTrustedStore.GetType() == typeof(LotlTrustedStore)) {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, CERTIFICATE_LOTL_ORIGIN, ReportItem.ReportItemStatus
                        .INFO));
                }
                else {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_CUSTOM_ORIGIN
                        , lotlTrustedStore.GetType().FullName), ReportItem.ReportItemStatus.INFO));
                }
                return true;
            }
            return false;
        }

        private bool StopValidation(ValidationReport result, ValidationContext context) {
            return result.GetValidationResult() == ValidationReport.ValidationResult.INVALID && !properties.GetContinueAfterFailure
                (context);
        }

        private void ValidateValidityPeriod(ValidationReport result, IX509Certificate certificate, DateTime validationDate
            ) {
            try {
                certificate.CheckValidity(validationDate);
            }
            catch (AbstractCertificateExpiredException e) {
                result.AddReportItem(new CertificateReportItem(certificate, VALIDITY_CHECK, MessageFormatUtil.Format(EXPIRED_CERTIFICATE
                    , certificate.GetSubjectDN()), e, ReportItem.ReportItemStatus.INVALID));
            }
            catch (AbstractCertificateNotYetValidException e) {
                result.AddReportItem(new CertificateReportItem(certificate, VALIDITY_CHECK, MessageFormatUtil.Format(NOT_YET_VALID_CERTIFICATE
                    , certificate.GetSubjectDN()), e, ReportItem.ReportItemStatus.INVALID));
            }
            catch (Exception e) {
                result.AddReportItem(new CertificateReportItem(certificate, VALIDITY_CHECK, MessageFormatUtil.Format(VALIDITY_PERIOD_CHECK_FAILED
                    , certificate.GetSubjectDN()), e, ReportItem.ReportItemStatus.INVALID));
            }
        }

        private void ValidateRequiredExtensions(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, int certificateChainSize) {
            IList<CertificateExtension> requiredExtensions = properties.GetRequiredExtensions(context);
            if (requiredExtensions != null) {
                foreach (CertificateExtension requiredExtension in requiredExtensions) {
                    if (requiredExtension is DynamicCertificateExtension) {
                        ((DynamicCertificateExtension)requiredExtension).WithCertificateChainSize(certificateChainSize);
                    }
                    if (!requiredExtension.ExistsInCertificate(certificate)) {
                        result.AddReportItem(new CertificateReportItem(certificate, EXTENSIONS_CHECK, MessageFormatUtil.Format(EXTENSION_MISSING
                            , requiredExtension.GetMessage()), ReportItem.ReportItemStatus.INVALID));
                    }
                }
            }
        }

        private void ValidateRevocationData(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate) {
            SafeCalling.OnRuntimeExceptionLog(() => revocationDataValidator.Validate(report, context, certificate, validationDate
                ), report, (e) => new CertificateReportItem(certificate, CERTIFICATE_CHECK, REVOCATION_VALIDATION_FAILED
                , e, ReportItem.ReportItemStatus.INDETERMINATE));
        }

        private void ValidateChain(ValidationReport result, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate, IList<IX509Certificate> previousCertificates) {
            IList<IX509Certificate> issuerCertificates;
            try {
                issuerCertificates = certificateRetriever.RetrieveIssuerCertificate(certificate);
            }
            catch (Exception e) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, ISSUER_RETRIEVAL_FAILED, e, 
                    ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            if (issuerCertificates.IsEmpty()) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(ISSUER_MISSING
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            // We need to sort certificates to process them starting from those, better suited for PAdES validation.
            issuerCertificates = issuerCertificates.Sorted((issuer1, issuer2) => JavaUtil.IntegerCompare((int)(certificateRetriever
                .GetCertificateOrigin(issuer1)), (int)(certificateRetriever.GetCertificateOrigin(issuer2)))).ToList();
            ValidationReport[] candidateReports = new ValidationReport[issuerCertificates.Count];
            for (int i = 0; i < issuerCertificates.Count; i++) {
                candidateReports[i] = new ValidationReport();
                try {
                    certificate.Verify(issuerCertificates[i].GetPublicKey());
                }
                catch (AbstractGeneralSecurityException e) {
                    candidateReports[i].AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil
                        .Format(ISSUER_CANNOT_BE_VERIFIED, issuerCertificates[i].GetSubjectDN(), certificate.GetSubjectDN()), 
                        e, ReportItem.ReportItemStatus.INVALID));
                    continue;
                }
                catch (Exception e) {
                    candidateReports[i].AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil
                        .Format(ISSUER_VERIFICATION_FAILED, issuerCertificates[i].GetSubjectDN(), certificate.GetSubjectDN()), 
                        e, ReportItem.ReportItemStatus.INVALID));
                    continue;
                }
                previousCertificates.Add(certificate);
                this.Validate(candidateReports[i], context.SetCertificateSource(CertificateSource.CERT_ISSUER), issuerCertificates
                    [i], validationDate, previousCertificates);
                previousCertificates.JRemoveAt(previousCertificates.Count - 1);
                if (candidateReports[i].GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    // We found valid issuer, no need to try other ones.
                    result.Merge(candidateReports[i]);
                    return;
                }
            }
            // Valid issuer wasn't found, add all the reports
            foreach (ValidationReport candidateReport in candidateReports) {
                result.Merge(candidateReport);
            }
        }
    }
}
