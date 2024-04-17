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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Validator class, which is expected to be used for signatures validation.</summary>
    internal class SignatureValidator {
        internal const String TIMESTAMP_VERIFICATION = "Timestamp verification check.";

        internal const String SIGNATURE_VERIFICATION = "Signature verification check.";

        internal const String CERTS_FROM_DSS = "Certificates from DSS check.";

        internal const String CANNOT_PARSE_CERT_FROM_DSS = "Certificate {0} stored in DSS dictionary cannot be parsed.";

        internal const String CANNOT_VERIFY_SIGNATURE = "Signature {0} cannot be mathematically verified.";

        internal const String DOCUMENT_IS_NOT_COVERED = "Signature {0} doesn't cover entire document.";

        internal const String CANNOT_VERIFY_TIMESTAMP = "Signature timestamp attribute cannot be verified";

        private readonly PdfDocument document;

        private readonly ValidationContext baseValidationContext;

        private readonly CertificateChainValidator certificateChainValidator;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly SignatureValidationProperties properties;

        /// <summary>
        /// Create new instance of
        /// <see cref="SignatureValidator"/>.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        internal SignatureValidator(PdfDocument document, ValidatorChainBuilder builder) {
            this.document = document;
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.certificateChainValidator = builder.GetCertificateChainValidator();
            this.baseValidationContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR, CertificateSource
                .SIGNER_CERT, TimeBasedContext.PRESENT);
        }

        /// <summary>Validate the latest signature in the document.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateLatestSignature() {
            ValidationReport validationReport = new ValidationReport();
            PdfPKCS7 pkcs7 = MathematicallyVerifySignature(validationReport);
            if (StopValidation(validationReport, baseValidationContext)) {
                return validationReport;
            }
            IList<IX509Certificate> certificatesFromDss = GetCertificatesFromDss(validationReport);
            certificateRetriever.AddKnownCertificates(certificatesFromDss);
            if (pkcs7.IsTsp()) {
                return ValidateTimestampChain(validationReport, pkcs7.GetCertificates(), pkcs7.GetSigningCertificate());
            }
            DateTime signingDate = DateTimeUtil.GetCurrentUtcTime();
            if (pkcs7.GetTimeStampTokenInfo() != null) {
                try {
                    if (!pkcs7.VerifyTimestampImprint()) {
                        validationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, ReportItem.ReportItemStatus
                            .INVALID));
                    }
                }
                catch (AbstractGeneralSecurityException e) {
                    validationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, e, ReportItem.ReportItemStatus
                        .INVALID));
                }
                if (StopValidation(validationReport, baseValidationContext)) {
                    return validationReport;
                }
                IX509Certificate[] timestampCertificates = pkcs7.GetTimestampCertificates();
                ValidateTimestampChain(validationReport, timestampCertificates, (IX509Certificate)timestampCertificates[0]
                    );
                if (StopValidation(validationReport, baseValidationContext)) {
                    return validationReport;
                }
                signingDate = pkcs7.GetTimeStampDate().ToUniversalTime();
            }
            IX509Certificate[] certificates = pkcs7.GetCertificates();
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(certificates));
            IX509Certificate signingCertificate = pkcs7.GetSigningCertificate();
            return certificateChainValidator.Validate(validationReport, baseValidationContext, signingCertificate, signingDate
                );
        }

        private PdfPKCS7 MathematicallyVerifySignature(ValidationReport validationReport) {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            IList<String> signatures = signatureUtil.GetSignatureNames();
            String latestSignatureName = signatures[signatures.Count - 1];
            PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData(latestSignatureName);
            if (!signatureUtil.SignatureCoversWholeDocument(latestSignatureName)) {
                validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(DOCUMENT_IS_NOT_COVERED
                    , latestSignatureName), ReportItem.ReportItemStatus.INVALID));
            }
            try {
                if (!pkcs7.VerifySignatureIntegrityAndAuthenticity()) {
                    validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_VERIFY_SIGNATURE
                        , latestSignatureName), ReportItem.ReportItemStatus.INVALID));
                }
            }
            catch (AbstractGeneralSecurityException e) {
                validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_VERIFY_SIGNATURE
                    , latestSignatureName), e, ReportItem.ReportItemStatus.INVALID));
            }
            return pkcs7;
        }

        private ValidationReport ValidateTimestampChain(ValidationReport validationReport, IX509Certificate[] knownCerts
            , IX509Certificate signingCert) {
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts));
            DateTime signingDate = DateTimeUtil.GetCurrentUtcTime();
            return certificateChainValidator.Validate(validationReport, baseValidationContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, signingDate);
        }

        private IList<IX509Certificate> GetCertificatesFromDss(ValidationReport validationReport) {
            PdfDictionary dss = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            IList<IX509Certificate> certificatesFromDss = new List<IX509Certificate>();
            if (dss != null) {
                PdfArray certs = dss.GetAsArray(PdfName.Certs);
                if (certs != null) {
                    for (int i = 0; i < certs.Size(); ++i) {
                        PdfStream certStream = certs.GetAsStream(i);
                        try {
                            certificatesFromDss.Add(CertificateUtil.GenerateCertificate(new MemoryStream(certStream.GetBytes())));
                        }
                        catch (AbstractGeneralSecurityException e) {
                            validationReport.AddReportItem(new ReportItem(CERTS_FROM_DSS, MessageFormatUtil.Format(CANNOT_PARSE_CERT_FROM_DSS
                                , certStream), e, ReportItem.ReportItemStatus.INFO));
                        }
                    }
                }
            }
            return certificatesFromDss;
        }

        private bool StopValidation(ValidationReport result, ValidationContext validationContext) {
            return !properties.GetContinueAfterFailure(validationContext) && result.GetValidationResult() != ValidationReport.ValidationResult
                .VALID;
        }
    }
}
