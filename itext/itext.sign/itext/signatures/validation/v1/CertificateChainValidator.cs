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
        internal const String CERTIFICATE_CHECK = "Certificate check.";

        internal const String VALIDITY_CHECK = "Certificate validity period check.";

        internal const String EXTENSIONS_CHECK = "Required certificate extensions check.";

        internal const String CERTIFICATE_TRUSTED = "Certificate {0} is trusted, revocation data checks are not required.";

        internal const String EXTENSION_MISSING = "Required extension {0} is missing or incorrect.";

        internal const String ISSUER_MISSING = "Certificate {0} isn't trusted and issuer certificate isn't provided.";

        internal const String EXPIRED_CERTIFICATE = "Certificate {0} is expired.";

        internal const String NOT_YET_VALID_CERTIFICATE = "Certificate {0} is not yet valid.";

        internal const String ISSUER_CANNOT_BE_VERIFIED = "Issuer certificate {0} for subject certificate {1} cannot be mathematically verified.";

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
        internal CertificateChainValidator(ValidatorChainBuilder builder) {
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
            ValidateRequiredExtensions(result, context, certificate);
            if (StopValidation(result, context)) {
                return result;
            }
            if (certificateRetriever.IsCertificateTrusted(certificate)) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                return result;
            }
            ValidateRevocationData(result, localContext, certificate, validationDate);
            if (StopValidation(result, localContext)) {
                return result;
            }
            ValidateChain(result, context, certificate, validationDate);
            return result;
        }

        private bool StopValidation(ValidationReport result, ValidationContext context) {
            return !properties.GetContinueAfterFailure(context) && result.GetValidationResult() != ValidationReport.ValidationResult
                .VALID;
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
            revocationDataValidator.Validate(report, context, certificate, validationDate);
        }

        private void ValidateChain(ValidationReport result, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate) {
            IX509Certificate issuerCertificate = (IX509Certificate)certificateRetriever.RetrieveIssuerCertificate(certificate
                );
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
            this.Validate(result, context.SetCertificateSource(CertificateSource.CERT_ISSUER), issuerCertificate, validationDate
                );
        }
    }
}
