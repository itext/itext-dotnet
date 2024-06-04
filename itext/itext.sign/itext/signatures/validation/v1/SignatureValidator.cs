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
using iText.Commons.Actions.Contexts;
using iText.Commons.Bouncycastle.Asn1.Tsp;
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
        public const String VALIDATING_SIGNATURE_NAME = "Validating signature {0}";

        internal const String TIMESTAMP_VERIFICATION = "Timestamp verification check.";

        internal const String SIGNATURE_VERIFICATION = "Signature verification check.";

        internal const String CERTS_FROM_DSS = "Certificates from DSS check.";

        internal const String CANNOT_PARSE_CERT_FROM_DSS = "Certificate {0} stored in DSS dictionary cannot be parsed.";

        internal const String CANNOT_VERIFY_SIGNATURE = "Signature {0} cannot be mathematically verified.";

        internal const String DOCUMENT_IS_NOT_COVERED = "Signature {0} doesn't cover entire document.";

        internal const String CANNOT_VERIFY_TIMESTAMP = "Signature timestamp attribute cannot be verified.";

        internal const String REVISIONS_RETRIEVAL_FAILED = "Wasn't possible to retrieve document revisions.";

        private const String TIMESTAMP_EXTRACTION_FAILED = "Unable to extract timestamp from timestamp signature";

        private readonly ValidationContext baseValidationContext;

        private readonly CertificateChainValidator certificateChainValidator;

        private readonly DocumentRevisionsValidator documentRevisionsValidator;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly SignatureValidationProperties properties;

        private DateTime lastKnownPoE = DateTimeUtil.GetCurrentUtcTime();

        private IMetaInfo metaInfo = new ValidationMetaInfo();

        /// <summary>
        /// Creates new instance of
        /// <see cref="SignatureValidator"/>.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        internal SignatureValidator(ValidatorChainBuilder builder) {
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.certificateChainValidator = builder.GetCertificateChainValidator();
            this.documentRevisionsValidator = builder.GetDocumentRevisionsValidator();
            this.baseValidationContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR, CertificateSource
                .SIGNER_CERT, TimeBasedContext.PRESENT);
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// that will be used during new
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// creations.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// the same
        /// <see cref="SignatureValidator"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.V1.SignatureValidator SetEventCountingMetaInfo(IMetaInfo metaInfo
            ) {
            this.metaInfo = metaInfo;
            return this;
        }

        /// <summary>Validate all signatures in the document</summary>
        /// <param name="document">the document to be validated</param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateSignatures(PdfDocument document) {
            ValidationReport report = new ValidationReport();
            documentRevisionsValidator.SetEventCountingMetaInfo(metaInfo);
            ValidationReport revisionsValidationReport = documentRevisionsValidator.ValidateAllDocumentRevisions(baseValidationContext
                , document);
            report.Merge(revisionsValidationReport);
            if (StopValidation(report, baseValidationContext)) {
                return report;
            }
            SignatureUtil util = new SignatureUtil(document);
            IList<String> signatureNames = util.GetSignatureNames();
            JavaCollectionsUtil.Reverse(signatureNames);
            foreach (String fieldName in signatureNames) {
                try {
                    using (PdfDocument doc = new PdfDocument(new PdfReader(util.ExtractRevision(fieldName)), new DocumentProperties
                        ().SetEventCountingMetaInfo(metaInfo))) {
                        ValidationReport subReport = ValidateLatestSignature(doc);
                        report.Merge(subReport);
                        if (StopValidation(report, baseValidationContext)) {
                            return report;
                        }
                    }
                }
                catch (System.IO.IOException e) {
                    report.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, REVISIONS_RETRIEVAL_FAILED, e, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                }
            }
            return report;
        }

        /// <summary>Validate the latest signature in the document.</summary>
        /// <param name="document">the document of which to validate the latest signature</param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateLatestSignature(PdfDocument document) {
            ValidationReport validationReport = new ValidationReport();
            PdfPKCS7 pkcs7 = MathematicallyVerifySignature(validationReport, document);
            if (StopValidation(validationReport, baseValidationContext)) {
                return validationReport;
            }
            IList<IX509Certificate> certificatesFromDss = GetCertificatesFromDss(validationReport, document);
            certificateRetriever.AddKnownCertificates(certificatesFromDss);
            if (pkcs7.IsTsp()) {
                return ValidateTimestampChain(validationReport, pkcs7.GetTimeStampTokenInfo(), pkcs7.GetCertificates(), pkcs7
                    .GetSigningCertificate());
            }
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
                PdfPKCS7 timestampSignatureContainer = pkcs7.GetTimestampSignatureContainer();
                try {
                    if (!timestampSignatureContainer.VerifySignatureIntegrityAndAuthenticity()) {
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
                IX509Certificate[] timestampCertificates = timestampSignatureContainer.GetCertificates();
                ValidateTimestampChain(validationReport, pkcs7.GetTimeStampTokenInfo(), timestampCertificates, timestampSignatureContainer
                    .GetSigningCertificate());
                if (StopValidation(validationReport, baseValidationContext)) {
                    return validationReport;
                }
            }
            IX509Certificate[] certificates = pkcs7.GetCertificates();
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(certificates));
            IX509Certificate signingCertificate = pkcs7.GetSigningCertificate();
            return certificateChainValidator.Validate(validationReport, baseValidationContext, signingCertificate, lastKnownPoE
                );
        }

        private PdfPKCS7 MathematicallyVerifySignature(ValidationReport validationReport, PdfDocument document) {
            SignatureUtil signatureUtil = new SignatureUtil(document);
            IList<String> signatures = signatureUtil.GetSignatureNames();
            String latestSignatureName = signatures[signatures.Count - 1];
            PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData(latestSignatureName);
            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(VALIDATING_SIGNATURE_NAME
                , latestSignatureName), ReportItem.ReportItemStatus.INFO));
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

        private ValidationReport ValidateTimestampChain(ValidationReport validationReport, ITstInfo timeStampTokenInfo
            , IX509Certificate[] knownCerts, IX509Certificate signingCert) {
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts));
            ValidationReport tsValidationReport = new ValidationReport();
            certificateChainValidator.Validate(tsValidationReport, baseValidationContext.SetCertificateSource(CertificateSource
                .TIMESTAMP), signingCert, lastKnownPoE);
            validationReport.Merge(tsValidationReport);
            if (tsValidationReport.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                try {
                    lastKnownPoE = timeStampTokenInfo.GetGenTime();
                }
                catch (Exception e) {
                    validationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, TIMESTAMP_EXTRACTION_FAILED, e, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                }
            }
            return validationReport;
        }

        private IList<IX509Certificate> GetCertificatesFromDss(ValidationReport validationReport, PdfDocument document
            ) {
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
            return !properties.GetContinueAfterFailure(validationContext) && result.GetValidationResult() == ValidationReport.ValidationResult
                .INVALID;
        }
    }
}
