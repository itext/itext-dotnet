/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Extensions;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Validator class, which is expected to be used for certificates chain validation.</summary>
    public class CertificateChainValidator {
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
        internal const String EXTENSION_MISSING = "Required extension {0} is missing or incorrect.";
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

        private readonly SignatureValidationProperties properties;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly RevocationDataValidator revocationDataValidator;

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
        }

        /// <summary>
        /// Add
        /// <see cref="iText.Signatures.ICrlClient"/>
        /// to be used for CRL responses receiving.
        /// </summary>
        /// <param name="crlClient">
        /// 
        /// <see cref="iText.Signatures.ICrlClient"/>
        /// to be used for CRL responses receiving
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.V1.CertificateChainValidator AddCrlClient(ICrlClient crlClient) {
            revocationDataValidator.AddCrlClient(crlClient);
            return this;
        }

        /// <summary>
        /// Add
        /// <see cref="iText.Signatures.IOcspClient"/>
        /// to be used for OCSP responses receiving.
        /// </summary>
        /// <param name="ocpsClient">
        /// 
        /// <see cref="iText.Signatures.IOcspClient"/>
        /// to be used for OCSP responses receiving
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.V1.CertificateChainValidator AddOcspClient(IOcspClient ocpsClient
            ) {
            revocationDataValidator.AddOcspClient(ocpsClient);
            return this;
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
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
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
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
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
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains both provided and new validation results
        /// </returns>
        public virtual ValidationReport Validate(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime validationDate) {
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR);
            ValidateValidityPeriod(result, certificate, validationDate);
            ValidateRequiredExtensions(result, localContext, certificate);
            if (StopValidation(result, localContext)) {
                return result;
            }
            if (SafeCalling.OnExceptionLog(() => CheckIfCertIsTrusted(result, localContext, certificate), false, result
                , (e) => new CertificateReportItem(certificate, CERTIFICATE_CHECK, TRUSTSTORE_RETRIEVAL_FAILED, e, ReportItem.ReportItemStatus
                .INFO))) {
                return result;
            }
            ValidateRevocationData(result, localContext, certificate, validationDate);
            if (StopValidation(result, localContext)) {
                return result;
            }
            ValidateChain(result, localContext, certificate, validationDate);
            return result;
        }

        private bool CheckIfCertIsTrusted(ValidationReport result, ValidationContext context, IX509Certificate certificate
            ) {
            if (CertificateSource.TRUSTED == context.GetCertificateSource()) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                return true;
            }
            TrustedCertificatesStore store = certificateRetriever.GetTrustedCertificatesStore();
            if (store.IsCertificateGenerallyTrusted(certificate)) {
                // Certificate is trusted for everything.
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                return true;
            }
            if (store.IsCertificateTrustedForCA(certificate)) {
                // Certificate is trusted to be CA, we need to make sure it wasn't used to directly sign anything else.
                if (CertificateSource.CERT_ISSUER == context.GetCertificateSource()) {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                        , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                    return true;
                }
                // Certificate is trusted to be CA, but is not used in CA context.
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT
                    , certificate.GetSubjectDN(), "certificates generation"), ReportItem.ReportItemStatus.INFO));
            }
            if (store.IsCertificateTrustedForTimestamp(certificate)) {
                // Certificate is trusted for timestamp signing,
                // we need to make sure this chain is responsible for timestamping.
                if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.TIMESTAMP)) {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                        , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                    return true;
                }
                // Certificate is trusted for timestamps generation, but is not used in timestamp generation context.
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT
                    , certificate.GetSubjectDN(), "timestamp generation"), ReportItem.ReportItemStatus.INFO));
            }
            if (store.IsCertificateTrustedForOcsp(certificate)) {
                // Certificate is trusted for OCSP response signing,
                // we need to make sure this chain is responsible for OCSP response generation.
                if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.OCSP_ISSUER)
                    ) {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                        , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                    return true;
                }
                // Certificate is trusted for OCSP response generation, but is not used in OCSP response generation context.
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT
                    , certificate.GetSubjectDN(), "OCSP response generation"), ReportItem.ReportItemStatus.INFO));
            }
            if (store.IsCertificateTrustedForCrl(certificate)) {
                // Certificate is trusted for CRL signing,
                // we need to make sure this chain is responsible for CRL generation.
                if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, CertificateSource.CRL_ISSUER)) {
                    result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                        , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                    return true;
                }
                // Certificate is trusted for CRL generation, but is not used in CRL generation context.
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT
                    , certificate.GetSubjectDN(), "CRL generation"), ReportItem.ReportItemStatus.INFO));
            }
            return false;
        }

        private bool StopValidation(ValidationReport result, ValidationContext context) {
            return !properties.GetContinueAfterFailure(context) && result.GetValidationResult() == ValidationReport.ValidationResult
                .INVALID;
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
             certificate) {
            IList<CertificateExtension> requiredExtensions = properties.GetRequiredExtensions(context);
            if (requiredExtensions != null) {
                foreach (CertificateExtension requiredExtension in requiredExtensions) {
                    if (!requiredExtension.ExistsInCertificate(certificate)) {
                        result.AddReportItem(new CertificateReportItem(certificate, EXTENSIONS_CHECK, MessageFormatUtil.Format(EXTENSION_MISSING
                            , requiredExtension.GetExtensionOid()), ReportItem.ReportItemStatus.INVALID));
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
            , DateTime validationDate) {
            IX509Certificate issuerCertificate = null;
            try {
                issuerCertificate = (IX509Certificate)certificateRetriever.RetrieveIssuerCertificate(certificate);
            }
            catch (Exception e) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, ISSUER_RETRIEVAL_FAILED, e, 
                    ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            if (issuerCertificate == null) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(ISSUER_MISSING
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            try {
                certificate.Verify(issuerCertificate.GetPublicKey());
            }
            catch (AbstractGeneralSecurityException e) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(ISSUER_CANNOT_BE_VERIFIED
                    , issuerCertificate.GetSubjectDN(), certificate.GetSubjectDN()), e, ReportItem.ReportItemStatus.INVALID
                    ));
                return;
            }
            catch (Exception e) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(ISSUER_VERIFICATION_FAILED
                    , issuerCertificate.GetSubjectDN(), certificate.GetSubjectDN()), e, ReportItem.ReportItemStatus.INVALID
                    ));
                return;
            }
            this.Validate(result, context.SetCertificateSource(CertificateSource.CERT_ISSUER), issuerCertificate, validationDate
                );
        }
    }
}
