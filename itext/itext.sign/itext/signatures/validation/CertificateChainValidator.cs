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
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Validator class, which is expected to be used for certificates chain validation.</summary>
    public class CertificateChainValidator {
        internal const String CERTIFICATE_CHECK = "Certificate check.";

        internal const String VALIDITY_CHECK = "Certificate validity period check.";

        internal const String EXTENSIONS_CHECK = "Required certificate extensions check.";

        internal const String CERTIFICATE_TRUSTED = "Certificate {0} is trusted, revocation data checks are not required.";

        internal const String EXTENSION_MISSING = "Required extension {0} is missing or incorrect.";

        internal const String GLOBAL_EXTENSION_MISSING = "Globally required extension {0} is missing or incorrect.";

        internal const String ISSUER_MISSING = "Certificate {0} isn't trusted and issuer certificate isn't provided.";

        internal const String EXPIRED_CERTIFICATE = "Certificate {0} is expired.";

        internal const String NOT_YET_VALID_CERTIFICATE = "Certificate {0} is not yet valid.";

        internal const String ISSUER_CANNOT_BE_VERIFIED = "Issuer certificate {0} for subject certificate {1} cannot be mathematically verified.";

        private IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();

        private RevocationDataValidator revocationDataValidator;

        private iText.Signatures.Validation.CertificateChainValidator nextCertificateChainValidator;

        private IList<CertificateExtension> globalRequiredExtensions = new List<CertificateExtension>();

        private bool proceedValidationAfterFail = true;

        /// <summary>
        /// Create new instance of
        /// <see cref="CertificateChainValidator"/>.
        /// </summary>
        public CertificateChainValidator() {
            nextCertificateChainValidator = this;
        }

        // Empty constructor.
        /// <summary>
        /// Set
        /// <see cref="RevocationDataValidator"/>
        /// to be used for OCSP and CRL responses validation.
        /// </summary>
        /// <param name="revocationDataValidator">
        /// 
        /// <see cref="RevocationDataValidator"/>
        /// to be used for OCSP and CRL responses validation
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetRevocationDataValidator(RevocationDataValidator
             revocationDataValidator) {
            this.revocationDataValidator = revocationDataValidator;
            return this;
        }

        /// <summary>
        /// Get
        /// <see cref="RevocationDataValidator"/>
        /// to be used for OCSP and CRL responses validation.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="RevocationDataValidator"/>
        /// to be used for OCSP and CRL responses validation.
        /// </returns>
        public virtual RevocationDataValidator GetRevocationDataValidator() {
            if (this.revocationDataValidator == null) {
                this.revocationDataValidator = new RevocationDataValidator();
            }
            return this.revocationDataValidator;
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
        public virtual iText.Signatures.Validation.CertificateChainValidator AddCrlClient(ICrlClient crlClient) {
            GetRevocationDataValidator().AddCrlClient(crlClient);
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
        public virtual iText.Signatures.Validation.CertificateChainValidator AddOcspClient(IOcspClient ocpsClient) {
            GetRevocationDataValidator().AddOcspClient(ocpsClient);
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// to be used for certificate chain building.
        /// </summary>
        /// <param name="certificateRetriever">
        /// 
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// to be used for certificate chain building
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetIssuingCertificateRetriever(IssuingCertificateRetriever
             certificateRetriever) {
            this.certificateRetriever = certificateRetriever;
            return this;
        }

        /// <summary>
        /// Set certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be used as trusted roots.
        /// </summary>
        /// <param name="trustedCertificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be used as trusted roots
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetTrustedCertificates(ICollection<IX509Certificate
            > trustedCertificates) {
            certificateRetriever.AddTrustedCertificates(trustedCertificates);
            return this;
        }

        /// <summary>
        /// Set certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be used as possible certificates for chain building.
        /// </summary>
        /// <param name="knownCertificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be used as possible certificates for chain building
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetKnownCertificates(ICollection<IX509Certificate
            > knownCertificates) {
            certificateRetriever.AddKnownCertificates(knownCertificates);
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="CertificateChainValidator"/>
        /// to be used as a validator for the next certificate in the chain.
        /// </summary>
        /// <param name="nextValidator">
        /// 
        /// <see cref="CertificateChainValidator"/>
        /// to be a validator for the next certificate in the chain
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetNextCertificateChainValidator(iText.Signatures.Validation.CertificateChainValidator
             nextValidator) {
            this.nextCertificateChainValidator = nextValidator;
            return this;
        }

        /// <summary>
        /// Set certificate extension
        /// <see cref="System.Collections.IList{E}"/>
        /// , which are globally required for each descending certificate in the chain.
        /// </summary>
        /// <param name="globalRequiredExtensions">list of globally required extensions</param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator SetGlobalRequiredExtensions(IList<CertificateExtension
            > globalRequiredExtensions) {
            this.globalRequiredExtensions = globalRequiredExtensions;
            return this;
        }

        /// <summary>
        /// Set
        /// <c>boolean</c>
        /// value, which determines whether to proceed or abort validation in case of failure.
        /// </summary>
        /// <param name="proceedValidationAfterFail">
        /// 
        /// <see langword="true"/>
        /// to proceed validation in case of failure,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CertificateChainValidator"/>
        /// </returns>
        public virtual iText.Signatures.Validation.CertificateChainValidator ProceedValidationAfterFail(bool proceedValidationAfterFail
            ) {
            this.proceedValidationAfterFail = proceedValidationAfterFail;
            return this;
        }

        /// <summary>Validate given certificate using provided validation date and required extensions.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// against which certificate is expected to be validated. Usually signing date
        /// </param>
        /// <param name="requiredExtensions">
        /// certificate extension
        /// <see cref="System.Collections.IList{E}"/>
        /// , which are required for the provided certificate
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateCertificate(IX509Certificate certificate, DateTime validationDate, 
            IList<CertificateExtension> requiredExtensions) {
            ValidationReport result = new ValidationReport();
            return Validate(result, certificate, validationDate, requiredExtensions);
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
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// against which certificate is expected to be validated. Usually signing date
        /// </param>
        /// <param name="requiredExtensions">
        /// certificate extension
        /// <see cref="System.Collections.IList{E}"/>
        /// , which are required for the provided certificate
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which contains both provided and new validation results
        /// </returns>
        public virtual ValidationReport Validate(ValidationReport result, IX509Certificate certificate, DateTime validationDate
            , IList<CertificateExtension> requiredExtensions) {
            ValidateValidityPeriod(result, certificate, validationDate);
            ValidateRequiredExtensions(result, certificate, requiredExtensions);
            if (StopValidation(result)) {
                return result;
            }
            if (certificateRetriever.IsCertificateTrusted(certificate)) {
                result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                return result;
            }
            ValidateRevocationData(result, certificate, validationDate);
            if (StopValidation(result)) {
                return result;
            }
            ValidateChain(result, certificate, validationDate);
            return result;
        }

        private bool StopValidation(ValidationReport result) {
            return !proceedValidationAfterFail && result.GetValidationResult() != ValidationReport.ValidationResult.VALID;
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

        private void ValidateRequiredExtensions(ValidationReport result, IX509Certificate certificate, IList<CertificateExtension
            > requiredExtensions) {
            if (requiredExtensions != null) {
                foreach (CertificateExtension requiredExtension in requiredExtensions) {
                    if (!requiredExtension.ExistsInCertificate(certificate)) {
                        result.AddReportItem(new CertificateReportItem(certificate, EXTENSIONS_CHECK, MessageFormatUtil.Format(EXTENSION_MISSING
                            , requiredExtension.GetExtensionOid()), ReportItem.ReportItemStatus.INVALID));
                    }
                }
            }
            if (globalRequiredExtensions != null) {
                foreach (CertificateExtension requiredExtension in globalRequiredExtensions) {
                    if (!requiredExtension.ExistsInCertificate(certificate)) {
                        result.AddReportItem(new CertificateReportItem(certificate, EXTENSIONS_CHECK, MessageFormatUtil.Format(GLOBAL_EXTENSION_MISSING
                            , requiredExtension.GetExtensionOid()), ReportItem.ReportItemStatus.INVALID));
                    }
                }
            }
        }

        private void ValidateRevocationData(ValidationReport report, IX509Certificate certificate, DateTime validationDate
            ) {
            GetRevocationDataValidator().SetIssuingCertificateRetriever(certificateRetriever).Validate(report, certificate
                , validationDate);
        }

        private void ValidateChain(ValidationReport result, IX509Certificate certificate, DateTime validationDate) {
            IList<CertificateExtension> requiredCertificateExtensions = new List<CertificateExtension>();
            requiredCertificateExtensions.Add(new KeyUsageExtension(KeyUsage.KEY_CERT_SIGN));
            requiredCertificateExtensions.Add(new BasicConstraintsExtension(true));
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
            nextCertificateChainValidator.Validate(result, issuerCertificate, validationDate, requiredCertificateExtensions
                );
        }
    }
}
