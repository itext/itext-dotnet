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
using iText.Bouncycastleconnector;
using iText.Commons.Actions.Contexts;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Validator class, which is expected to be used for signatures validation.</summary>
    public class SignatureValidator {
        public const String VALIDATING_SIGNATURE_NAME = "Validating signature {0}";

//\cond DO_NOT_DOCUMENT
        internal const String TIMESTAMP_VERIFICATION = "Timestamp verification check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNATURE_VERIFICATION = "Signature verification check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CANNOT_PARSE_CERT_FROM_DSS = "Certificate {0} stored in DSS dictionary cannot be parsed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CANNOT_PARSE_OCSP_FROM_DSS = "OCSP response {0} stored in DSS dictionary cannot be parsed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CANNOT_PARSE_CRL_FROM_DSS = "CRL {0} stored in DSS dictionary cannot be parsed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CANNOT_VERIFY_SIGNATURE = "Signature {0} cannot be mathematically verified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String DOCUMENT_IS_NOT_COVERED = "Signature {0} doesn't cover entire document.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CANNOT_VERIFY_TIMESTAMP = "Signature timestamp attribute cannot be verified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TIMESTAMP_VERIFICATION_FAILED = "Unexpected exception occurred during mathematical verification of time stamp signature.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVISIONS_RETRIEVAL_FAILED = "Unexpected exception occurred during document revisions retrieval.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String TIMESTAMP_EXTRACTION_FAILED = "Unexpected exception occurred retrieving prove of existence from timestamp signature";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CHAIN_VALIDATION_FAILED = "Unexpected exception occurred during certificate chain validation.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String REVISIONS_VALIDATION_FAILED = "Unexpected exception occurred during revisions validation.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ADD_KNOWN_CERTIFICATES_FAILED = "Unexpected exception occurred adding known certificates to certificate retriever.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SIGNATURE_NOT_FOUND = "Document doesn't contain signature field {0}.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String VALIDATION_PERFORMED = "Validation has already been performed. " + "You should create new SignatureValidator instance for each validation call.";
//\endcond

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private ValidationContext validationContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR, 
            CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private readonly CertificateChainValidator certificateChainValidator;

        private readonly DocumentRevisionsValidator documentRevisionsValidator;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly SignatureValidationProperties properties;

        private DateTime lastKnownPoE = DateTimeUtil.GetCurrentUtcTime();

        private IMetaInfo metaInfo = new ValidationMetaInfo();

        private readonly PdfDocument originalDocument;

        private ValidationOcspClient validationOcspClient;

        private ValidationCrlClient validationCrlClient;

        private bool validationPerformed = false;

        /// <summary>
        /// Creates new instance of
        /// <see cref="SignatureValidator"/>.
        /// </summary>
        /// <param name="originalDocument">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance which will be validated
        /// </param>
        /// <param name="builder">
        /// see
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal SignatureValidator(PdfDocument originalDocument, ValidatorChainBuilder builder) {
            this.originalDocument = originalDocument;
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.certificateChainValidator = builder.GetCertificateChainValidator();
            this.documentRevisionsValidator = builder.GetDocumentRevisionsValidator();
            FindValidationClients();
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

        /// <summary>Validate all signatures in the document.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results
        /// </returns>
        public virtual ValidationReport ValidateSignatures() {
            if (validationPerformed) {
                throw new PdfException(VALIDATION_PERFORMED);
            }
            validationPerformed = true;
            ValidationReport report = new ValidationReport();
            SafeCalling.OnRuntimeExceptionLog(() => {
                documentRevisionsValidator.SetEventCountingMetaInfo(metaInfo);
                ValidationReport revisionsValidationReport = documentRevisionsValidator.ValidateAllDocumentRevisions(validationContext
                    , originalDocument);
                report.Merge(revisionsValidationReport);
            }
            , report, (e) => new ReportItem(SIGNATURE_VERIFICATION, REVISIONS_VALIDATION_FAILED, e, ReportItem.ReportItemStatus
                .INDETERMINATE));
            if (StopValidation(report, validationContext)) {
                return report;
            }
            return report.Merge(Validate(null));
        }

        /// <summary>Validate single signature in the document.</summary>
        /// <param name="signatureName">name of the signature to validate</param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Report.ValidationReport"/>
        /// which contains detailed validation results.
        /// </returns>
        public virtual ValidationReport ValidateSignature(String signatureName) {
            if (validationPerformed) {
                throw new PdfException(VALIDATION_PERFORMED);
            }
            validationPerformed = true;
            ValidationReport report = new ValidationReport();
            SafeCalling.OnRuntimeExceptionLog(() => {
                documentRevisionsValidator.SetEventCountingMetaInfo(metaInfo);
                ValidationReport revisionsValidationReport = documentRevisionsValidator.ValidateAllDocumentRevisions(validationContext
                    , originalDocument, signatureName);
                report.Merge(revisionsValidationReport);
            }
            , report, (e) => new ReportItem(SIGNATURE_VERIFICATION, REVISIONS_VALIDATION_FAILED, e, ReportItem.ReportItemStatus
                .INDETERMINATE));
            if (StopValidation(report, validationContext)) {
                return report;
            }
            return report.Merge(Validate(signatureName));
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ValidationReport ValidateLatestSignature(PdfDocument document) {
            ValidationReport validationReport = new ValidationReport();
            PdfPKCS7 pkcs7 = MathematicallyVerifySignature(validationReport, document);
            UpdateValidationClients(pkcs7, validationReport, validationContext, document);
            // We only retrieve not signed revocation data at the very beginning of signature processing.
            RetrieveNotSignedRevocationInfoFromSignatureContainer(pkcs7, validationContext);
            if (StopValidation(validationReport, validationContext)) {
                return validationReport;
            }
            IList<IX509Certificate> certificatesFromDss = GetCertificatesFromDss(validationReport, document);
            SafeCalling.OnRuntimeExceptionLog(() => certificateRetriever.AddKnownCertificates(certificatesFromDss), validationReport
                , (e) => new ReportItem(SIGNATURE_VERIFICATION, ADD_KNOWN_CERTIFICATES_FAILED, e, ReportItem.ReportItemStatus
                .INFO));
            if (pkcs7.IsTsp()) {
                ValidateTimestampChain(validationReport, pkcs7.GetCertificates(), pkcs7.GetSigningCertificate());
                if (UpdateLastKnownPoE(validationReport, pkcs7.GetTimeStampTokenInfo())) {
                    UpdateValidationClients(pkcs7, validationReport, validationContext, document);
                }
                return validationReport;
            }
            bool isPoEUpdated = false;
            DateTime previousLastKnowPoE = lastKnownPoE;
            ValidationContext previousValidationContext = validationContext;
            if (pkcs7.GetTimeStampTokenInfo() != null) {
                ValidationReport tsValidationReport = ValidateEmbeddedTimestamp(pkcs7);
                isPoEUpdated = UpdateLastKnownPoE(tsValidationReport, pkcs7.GetTimeStampTokenInfo());
                if (isPoEUpdated) {
                    PdfPKCS7 timestampSignatureContainer = pkcs7.GetTimestampSignatureContainer();
                    RetrieveSignedRevocationInfoFromSignatureContainer(timestampSignatureContainer, validationContext);
                    UpdateValidationClients(pkcs7, tsValidationReport, validationContext, document);
                }
                validationReport.Merge(tsValidationReport);
                if (StopValidation(tsValidationReport, validationContext)) {
                    return validationReport;
                }
            }
            IX509Certificate[] certificates = pkcs7.GetCertificates();
            SafeCalling.OnRuntimeExceptionLog(() => certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(certificates
                )), validationReport, (e) => new ReportItem(SIGNATURE_VERIFICATION, ADD_KNOWN_CERTIFICATES_FAILED, e, 
                ReportItem.ReportItemStatus.INFO));
            IX509Certificate signingCertificate = pkcs7.GetSigningCertificate();
            ValidationReport signatureReport = new ValidationReport();
            SafeCalling.OnExceptionLog(() => certificateChainValidator.Validate(signatureReport, validationContext, signingCertificate
                , lastKnownPoE), validationReport, (e) => new CertificateReportItem(signingCertificate, SIGNATURE_VERIFICATION
                , CHAIN_VALIDATION_FAILED, e, ReportItem.ReportItemStatus.INDETERMINATE));
            if (isPoEUpdated && signatureReport.GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                // We can only use PoE retrieved from timestamp attribute in case main signature validation is successful.
                // That's why if the result is not valid, we set back lastKnownPoE value, validation context and rev data.
                lastKnownPoE = previousLastKnowPoE;
                validationContext = previousValidationContext;
                PdfPKCS7 timestampSignatureContainer = pkcs7.GetTimestampSignatureContainer();
                RetrieveSignedRevocationInfoFromSignatureContainer(timestampSignatureContainer, validationContext);
                UpdateValidationClients(pkcs7, validationReport, validationContext, document);
            }
            return validationReport.Merge(signatureReport);
        }
//\endcond

        private ValidationReport Validate(String signatureName) {
            ValidationReport validationReport = new ValidationReport();
            bool validateSingleSignature = signatureName != null;
            SignatureUtil util = new SignatureUtil(originalDocument);
            IList<String> signatureNames = util.GetSignatureNames();
            JavaCollectionsUtil.Reverse(signatureNames);
            foreach (String fieldName in signatureNames) {
                ValidationReport subReport = new ValidationReport();
                try {
                    using (PdfDocument doc = new PdfDocument(new PdfReader(util.ExtractRevision(fieldName)), new DocumentProperties
                        ().SetEventCountingMetaInfo(metaInfo))) {
                        subReport.Merge(ValidateLatestSignature(doc));
                    }
                }
                catch (System.IO.IOException e) {
                    subReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, REVISIONS_RETRIEVAL_FAILED, e, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                }
                catch (Exception e) {
                    subReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, REVISIONS_RETRIEVAL_FAILED, e, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                }
                if (!validateSingleSignature) {
                    validationReport.Merge(subReport);
                    if (StopValidation(subReport, validationContext)) {
                        return validationReport;
                    }
                }
                else {
                    if (fieldName.Equals(signatureName)) {
                        return subReport;
                    }
                }
            }
            if (validateSingleSignature) {
                validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(SIGNATURE_NOT_FOUND
                    , signatureName), ReportItem.ReportItemStatus.INDETERMINATE));
            }
            return validationReport;
        }

        private void FindValidationClients() {
            foreach (IOcspClient ocspClient in this.properties.GetOcspClients()) {
                if (ocspClient.GetType() == typeof(ValidationOcspClient)) {
                    validationOcspClient = (ValidationOcspClient)ocspClient;
                    break;
                }
            }
            foreach (ICrlClient crlClient in this.properties.GetCrlClients()) {
                if (crlClient.GetType() == typeof(ValidationCrlClient)) {
                    validationCrlClient = (ValidationCrlClient)crlClient;
                    break;
                }
            }
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
            catch (Exception e) {
                validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_VERIFY_SIGNATURE
                    , latestSignatureName), e, ReportItem.ReportItemStatus.INVALID));
            }
            return pkcs7;
        }

        private ValidationReport ValidateEmbeddedTimestamp(PdfPKCS7 pkcs7) {
            ValidationReport tsValidationReport = new ValidationReport();
            try {
                if (!pkcs7.VerifyTimestampImprint()) {
                    tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, ReportItem.ReportItemStatus
                        .INVALID));
                }
            }
            catch (AbstractGeneralSecurityException e) {
                tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, e, ReportItem.ReportItemStatus
                    .INVALID));
            }
            catch (Exception e) {
                tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION_FAILED, TIMESTAMP_VERIFICATION_FAILED
                    , e, ReportItem.ReportItemStatus.INVALID));
            }
            if (StopValidation(tsValidationReport, validationContext)) {
                return tsValidationReport;
            }
            PdfPKCS7 timestampSignatureContainer = pkcs7.GetTimestampSignatureContainer();
            RetrieveSignedRevocationInfoFromSignatureContainer(timestampSignatureContainer, validationContext);
            try {
                if (!timestampSignatureContainer.VerifySignatureIntegrityAndAuthenticity()) {
                    tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, ReportItem.ReportItemStatus
                        .INVALID));
                }
            }
            catch (AbstractGeneralSecurityException e) {
                tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, CANNOT_VERIFY_TIMESTAMP, e, ReportItem.ReportItemStatus
                    .INVALID));
            }
            catch (Exception e) {
                tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION_FAILED, TIMESTAMP_VERIFICATION_FAILED
                    , e, ReportItem.ReportItemStatus.INVALID));
            }
            if (StopValidation(tsValidationReport, validationContext)) {
                return tsValidationReport;
            }
            IX509Certificate[] timestampCertificates = timestampSignatureContainer.GetCertificates();
            ValidateTimestampChain(tsValidationReport, timestampCertificates, timestampSignatureContainer.GetSigningCertificate
                ());
            return tsValidationReport;
        }

        private void ValidateTimestampChain(ValidationReport validationReport, IX509Certificate[] knownCerts, IX509Certificate
             signingCert) {
            SafeCalling.OnExceptionLog(() => certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts
                )), validationReport, (e) => new ReportItem(SIGNATURE_VERIFICATION, ADD_KNOWN_CERTIFICATES_FAILED, e, 
                ReportItem.ReportItemStatus.INFO));
            try {
                certificateChainValidator.Validate(validationReport, validationContext.SetCertificateSource(CertificateSource
                    .TIMESTAMP), signingCert, lastKnownPoE);
            }
            catch (Exception e) {
                validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, CHAIN_VALIDATION_FAILED, e, ReportItem.ReportItemStatus
                    .INFO));
            }
        }

        private bool UpdateLastKnownPoE(ValidationReport tsValidationReport, ITstInfo timeStampTokenInfo) {
            if (tsValidationReport.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                try {
                    lastKnownPoE = timeStampTokenInfo.GetGenTime();
                    if (validationContext.GetTimeBasedContext() == TimeBasedContext.PRESENT) {
                        validationContext = validationContext.SetTimeBasedContext(TimeBasedContext.HISTORICAL);
                    }
                    return true;
                }
                catch (Exception e) {
                    tsValidationReport.AddReportItem(new ReportItem(TIMESTAMP_VERIFICATION, TIMESTAMP_EXTRACTION_FAILED, e, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                }
            }
            return false;
        }

        private void UpdateValidationClients(PdfPKCS7 pkcs7, ValidationReport validationReport, ValidationContext 
            validationContext, PdfDocument document) {
            RetrieveOcspResponsesFromDss(validationReport, validationContext, document);
            RetrieveCrlResponsesFromDss(validationReport, validationContext, document);
            RetrieveSignedRevocationInfoFromSignatureContainer(pkcs7, validationContext);
        }

        private void RetrieveSignedRevocationInfoFromSignatureContainer(PdfPKCS7 pkcs7, ValidationContext validationContext
            ) {
            if (pkcs7.GetCRLs() != null) {
                foreach (IX509Crl crl in pkcs7.GetCRLs()) {
                    validationCrlClient.AddCrl((IX509Crl)crl, lastKnownPoE, validationContext.GetTimeBasedContext());
                }
            }
            if (pkcs7.GetOcsp() != null) {
                validationOcspClient.AddResponse(pkcs7.GetOcsp(), lastKnownPoE, validationContext.GetTimeBasedContext());
            }
        }

        private void RetrieveNotSignedRevocationInfoFromSignatureContainer(PdfPKCS7 pkcs7, ValidationContext validationContext
            ) {
            foreach (IX509Crl crl in pkcs7.GetSignedDataCRLs()) {
                validationCrlClient.AddCrl((IX509Crl)crl, lastKnownPoE, validationContext.GetTimeBasedContext());
            }
            foreach (IBasicOcspResponse oscp in pkcs7.GetSignedDataOcsps()) {
                validationOcspClient.AddResponse(oscp, lastKnownPoE, validationContext.GetTimeBasedContext());
            }
        }

        private void RetrieveOcspResponsesFromDss(ValidationReport validationReport, ValidationContext context, PdfDocument
             document) {
            PdfDictionary dss = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            if (dss != null) {
                PdfArray ocsps = dss.GetAsArray(PdfName.OCSPs);
                if (ocsps != null) {
                    for (int i = 0; i < ocsps.Size(); ++i) {
                        PdfStream ocspStream = ocsps.GetAsStream(i);
                        try {
                            validationOcspClient.AddResponse(BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateOCSPResponse
                                (ocspStream.GetBytes()).GetResponseObject()), lastKnownPoE, context.GetTimeBasedContext());
                        }
                        catch (System.IO.IOException e) {
                            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_PARSE_OCSP_FROM_DSS
                                , ocspStream), e, ReportItem.ReportItemStatus.INFO));
                        }
                        catch (AbstractOcspException e) {
                            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_PARSE_OCSP_FROM_DSS
                                , ocspStream), e, ReportItem.ReportItemStatus.INFO));
                        }
                        catch (Exception e) {
                            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_PARSE_OCSP_FROM_DSS
                                , ocspStream), e, ReportItem.ReportItemStatus.INFO));
                        }
                    }
                }
            }
        }

        private void RetrieveCrlResponsesFromDss(ValidationReport validationReport, ValidationContext context, PdfDocument
             document) {
            PdfDictionary dss = document.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            if (dss != null) {
                PdfArray crls = dss.GetAsArray(PdfName.CRLs);
                if (crls != null) {
                    for (int i = 0; i < crls.Size(); ++i) {
                        PdfStream crlStream = crls.GetAsStream(i);
                        SafeCalling.OnExceptionLog(() => validationCrlClient.AddCrl((IX509Crl)CertificateUtil.ParseCrlFromBytes(crlStream
                            .GetBytes()), lastKnownPoE, context.GetTimeBasedContext()), validationReport, (e) => new ReportItem(SIGNATURE_VERIFICATION
                            , MessageFormatUtil.Format(CANNOT_PARSE_CRL_FROM_DSS, crlStream), e, ReportItem.ReportItemStatus.INFO)
                            );
                    }
                }
            }
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
                            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_PARSE_CERT_FROM_DSS
                                , certStream), e, ReportItem.ReportItemStatus.INFO));
                        }
                        catch (Exception e) {
                            validationReport.AddReportItem(new ReportItem(SIGNATURE_VERIFICATION, MessageFormatUtil.Format(CANNOT_PARSE_CERT_FROM_DSS
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
