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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Class that allows you to validate a single OCSP response.</summary>
    public class OCSPValidator {
//\cond DO_NOT_DOCUMENT
        internal const String CERT_IS_EXPIRED = "Certificate is expired on {0}. Its revocation status could have been "
             + "removed from the database, so the OCSP response status could be falsely valid.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERT_IS_REVOKED = "Certificate status is revoked.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERT_STATUS_IS_UNKNOWN = "Certificate status is unknown.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String INVALID_OCSP = "OCSP response is invalid.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUERS_DO_NOT_MATCH = "OCSP: Issuers don't match.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ISSUER_MISSING = "Issuer certificate wasn't found.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FRESHNESS_CHECK = "OCSP response is not fresh enough: " + "this update: {0}, validation date: {1}, freshness: {2}.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_COULD_NOT_BE_VERIFIED = "OCSP response could not be verified: " + "it does not contain responder in the certificate chain and response is not signed "
             + "by issuer certificate or any from the trusted store.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_NOT_RETRIEVED = "OCSP response could not be verified: \" +\n" + "            \"Unexpected exception occurred retrieving responder.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_NOT_VERIFIED = "OCSP response could not be verified: \" +\n" + "            \" Unexpected exception occurred while validating responder certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_DID_NOT_SIGN = "OCSP response could not be verified against this responder.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_TRUST_NOT_RETRIEVED = "OCSP response could not be verified: \" +\n" +
             "            \"responder trust state could not be retrieved.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_TRUSTED = "Responder certificate is a trusted certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_RESPONDER_IS_CA = "Responder certificate is the CA certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_IS_NO_LONGER_VALID = "OCSP is no longer valid: {0} after {1}";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SERIAL_NUMBERS_DO_NOT_MATCH = "OCSP: Serial numbers don't match.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_CHECK_IF_ISSUERS_MATCH = "OCSP response could not be verified: Unexpected exception"
             + " occurred checking if issuers match.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_RETRIEVE_ISSUER = "OCSP response could not be verified: Unexpected exception "
             + "occurred while retrieving issuer";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String OCSP_CHECK = "OCSP response check.";
//\endcond

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly SignatureValidationProperties properties;

        private readonly ValidatorChainBuilder builder;

        /// <summary>
        /// Creates new
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal OCSPValidator(ValidatorChainBuilder builder) {
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.builder = builder;
        }

        /// <summary>Validates a certificate against single OCSP Response.</summary>
        /// <param name="report">to store all the chain verification results</param>
        /// <param name="context">the context in which to perform the validation</param>
        /// <param name="certificate">the certificate to check for</param>
        /// <param name="singleResp">single response to check</param>
        /// <param name="ocspResp">basic OCSP response which contains single response to check</param>
        /// <param name="validationDate">validation date to check for</param>
        /// <param name="responseGenerationDate">trusted date at which response is generated</param>
        public virtual void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , ISingleResponse singleResp, IBasicOcspResponse ocspResp, DateTime validationDate, DateTime responseGenerationDate
            ) {
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.OCSP_VALIDATOR);
            if (CertificateUtil.IsSelfSigned(certificate)) {
                report.AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, RevocationDataValidator.SELF_SIGNED_CERTIFICATE
                    , ReportItem.ReportItemStatus.INFO));
                return;
            }
            // SingleResp contains the basic information of the status of the certificate identified by the certID.
            // Check if the serial numbers of the signCert and certID corresponds:
            if (!certificate.GetSerialNumber().Equals(singleResp.GetCertID().GetSerialNumber())) {
                report.AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, SERIAL_NUMBERS_DO_NOT_MATCH, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            IList<IX509Certificate> issuerCerts;
            try {
                issuerCerts = certificateRetriever.RetrieveIssuerCertificate(certificate);
            }
            catch (Exception e) {
                report.AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, UNABLE_TO_RETRIEVE_ISSUER, e, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            if (issuerCerts.IsEmpty()) {
                report.AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, MessageFormatUtil.Format(ISSUER_MISSING
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            ValidationReport[] candidateReports = new ValidationReport[issuerCerts.Count];
            for (int i = 0; i < issuerCerts.Count; i++) {
                candidateReports[i] = new ValidationReport();
                // Check if the issuer of the certID and signCert matches, i.e. check that issuerNameHash and issuerKeyHash
                // fields of the certID is the hash of the issuer's name and public key:
                try {
                    if (!CertificateUtil.CheckIfIssuersMatch(singleResp.GetCertID(), issuerCerts[i])) {
                        candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, ISSUERS_DO_NOT_MATCH, 
                            ReportItem.ReportItemStatus.INDETERMINATE));
                        continue;
                    }
                }
                catch (Exception e) {
                    candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, UNABLE_TO_CHECK_IF_ISSUERS_MATCH
                        , e, ReportItem.ReportItemStatus.INDETERMINATE));
                    continue;
                }
                // So, since the issuer name and serial number identify a unique certificate, we found the single response
                // for the provided certificate.
                TimeSpan freshness = properties.GetFreshness(localContext);
                // Check that thisUpdate + freshness < validation.
                if (DateTimeUtil.AddMillisToDate(singleResp.GetThisUpdate(), (long)freshness.TotalMilliseconds).Before(validationDate
                    )) {
                    candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, MessageFormatUtil.Format
                        (FRESHNESS_CHECK, singleResp.GetThisUpdate(), validationDate, freshness), ReportItem.ReportItemStatus.
                        INDETERMINATE));
                    continue;
                }
                // If nextUpdate is not set, the responder is indicating that newer revocation information
                // is available all the time.
                if (singleResp.GetNextUpdate() != TimestampConstants.UNDEFINED_TIMESTAMP_DATE && validationDate.After(singleResp
                    .GetNextUpdate())) {
                    candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, MessageFormatUtil.Format
                        (OCSP_IS_NO_LONGER_VALID, validationDate, singleResp.GetNextUpdate()), ReportItem.ReportItemStatus.INDETERMINATE
                        ));
                    continue;
                }
                // Check the status of the certificate:
                ICertStatus status = singleResp.GetCertStatus();
                IRevokedCertStatus revokedStatus = BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(status);
                bool isStatusGood = BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood().Equals(status);
                // Check OCSP Archive Cutoff extension in case OCSP response was generated after the certificate is expired.
                if (isStatusGood && certificate.GetNotAfter().Before(ocspResp.GetProducedAt())) {
                    DateTime startExpirationDate = GetArchiveCutoffExtension(ocspResp);
                    if (TimestampConstants.UNDEFINED_TIMESTAMP_DATE == startExpirationDate || certificate.GetNotAfter().Before
                        (startExpirationDate)) {
                        candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, MessageFormatUtil.Format
                            (CERT_IS_EXPIRED, certificate.GetNotAfter()), ReportItem.ReportItemStatus.INDETERMINATE));
                        continue;
                    }
                }
                if (isStatusGood || (revokedStatus != null && validationDate.Before(revokedStatus.GetRevocationTime()))) {
                    // Check if the OCSP response is genuine.
                    VerifyOcspResponder(candidateReports[i], localContext, ocspResp, issuerCerts[i], responseGenerationDate);
                    if (!isStatusGood) {
                        candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, MessageFormatUtil.Format
                            (SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED, revokedStatus.GetRevocationTime()), ReportItem.ReportItemStatus
                            .INFO));
                    }
                }
                else {
                    if (revokedStatus != null) {
                        candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, CERT_IS_REVOKED, ReportItem.ReportItemStatus
                            .INVALID));
                    }
                    else {
                        candidateReports[i].AddReportItem(new CertificateReportItem(certificate, OCSP_CHECK, CERT_STATUS_IS_UNKNOWN
                            , ReportItem.ReportItemStatus.INDETERMINATE));
                    }
                }
                if (candidateReports[i].GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    // We found valid issuer, no need to try other ones.
                    report.Merge(candidateReports[i]);
                    return;
                }
            }
            // Valid issuer wasn't found, add all the reports
            foreach (ValidationReport candidateReport in candidateReports) {
                report.Merge(candidateReport);
            }
        }

        /// <summary>Verifies if an OCSP response is genuine.</summary>
        /// <remarks>
        /// Verifies if an OCSP response is genuine.
        /// If it doesn't verify against the issuer certificate and response's certificates, it may verify
        /// using a trusted anchor or cert.
        /// </remarks>
        /// <param name="report">to store all the chain verification results</param>
        /// <param name="context">the context in which to perform the validation</param>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="issuerCert">the issuer of the certificate for which the OCSP is checked</param>
        private void VerifyOcspResponder(ValidationReport report, ValidationContext context, IBasicOcspResponse ocspResp
            , IX509Certificate issuerCert, DateTime responseGenerationDate) {
            ValidationContext localContext = context.SetCertificateSource(CertificateSource.OCSP_ISSUER);
            // OCSP response might be signed by the issuer certificate or
            // the Authorized OCSP responder certificate containing the id-kp-OCSPSigning extended key usage extension.
            // First check if the issuer certificate signed the response since it is expected to be the most common case:
            // the CA will already be validated by the chain validator
            if (CertificateUtil.IsSignatureValid(ocspResp, issuerCert)) {
                report.AddReportItem(new CertificateReportItem(issuerCert, OCSP_CHECK, OCSP_RESPONDER_IS_CA, ReportItem.ReportItemStatus
                    .INFO));
                return;
            }
            // If the issuer certificate didn't sign the ocsp response, look for authorized ocsp responses
            // from the properties or from the certificate chain received with response.
            ICollection<IX509Certificate> candidates = SafeCalling.OnRuntimeExceptionLog(() => certificateRetriever.RetrieveOCSPResponderByNameCertificate
                (ocspResp), JavaCollectionsUtil.EmptySet<IX509Certificate>(), report, (e) => new CertificateReportItem
                (issuerCert, OCSP_CHECK, OCSP_RESPONDER_NOT_RETRIEVED, e, ReportItem.ReportItemStatus.INDETERMINATE));
            if (candidates.IsEmpty()) {
                report.AddReportItem(new CertificateReportItem(issuerCert, OCSP_CHECK, OCSP_COULD_NOT_BE_VERIFIED, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            ValidationReport[] candidateReports = new ValidationReport[candidates.Count];
            int reportIndex = 0;
            foreach (IX509Certificate cert in candidates) {
                IX509Certificate responderCert = (IX509Certificate)cert;
                ValidationReport candidateReport = new ValidationReport();
                candidateReports[reportIndex++] = candidateReport;
                // if the response was not signed by this candidate we can stop further processing
                if (!CertificateUtil.IsSignatureValid(ocspResp, responderCert)) {
                    candidateReport.AddReportItem(new CertificateReportItem(responderCert, OCSP_CHECK, OCSP_RESPONDER_DID_NOT_SIGN
                        , ReportItem.ReportItemStatus.INDETERMINATE));
                    continue;
                }
                // if the responder is trusted validation is successful
                try {
                    if (certificateRetriever.GetTrustedCertificatesStore().IsCertificateTrustedForOcsp(responderCert) || certificateRetriever
                        .GetTrustedCertificatesStore().IsCertificateGenerallyTrusted(responderCert)) {
                        candidateReport.AddReportItem(new CertificateReportItem(responderCert, OCSP_CHECK, OCSP_RESPONDER_TRUSTED, 
                            ReportItem.ReportItemStatus.INFO));
                        report.Merge(candidateReport);
                        return;
                    }
                }
                catch (Exception e) {
                    report.AddReportItem(new CertificateReportItem(responderCert, OCSP_CHECK, OCSP_RESPONDER_TRUST_NOT_RETRIEVED
                        , e, ReportItem.ReportItemStatus.INDETERMINATE));
                    continue;
                }
                // RFC 6960 4.2.2.2. Authorized Responders:
                // "Systems relying on OCSP responses MUST recognize a delegation certificate as being issued
                // by the CA that issued the certificate in question only if the delegation certificate and the
                // certificate being checked for revocation were signed by the same key."
                // and "This certificate MUST be issued directly by the CA that is identified in the request".
                try {
                    responderCert.Verify(issuerCert.GetPublicKey());
                }
                catch (Exception e) {
                    candidateReport.AddReportItem(new CertificateReportItem(responderCert, OCSP_CHECK, INVALID_OCSP, e, ReportItem.ReportItemStatus
                        .INVALID));
                    continue;
                }
                // Validating of the ocsp signer's certificate (responderCert) described in the
                // RFC6960 4.2.2.2.1. Revocation Checking of an Authorized Responder.
                ValidationReport responderReport = new ValidationReport();
                try {
                    builder.GetCertificateChainValidator().Validate(responderReport, localContext, responderCert, responseGenerationDate
                        );
                }
                catch (Exception e) {
                    candidateReport.AddReportItem(new CertificateReportItem(responderCert, OCSP_CHECK, OCSP_RESPONDER_NOT_VERIFIED
                        , e, ReportItem.ReportItemStatus.INDETERMINATE));
                    continue;
                }
                AddResponderValidationReport(candidateReport, responderReport);
                if (candidateReport.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    AddResponderValidationReport(report, candidateReport);
                    return;
                }
            }
            //if we get here, none of the candidates were successful
            foreach (ValidationReport subReport in candidateReports) {
                report.Merge(subReport);
            }
        }

        private static void AddResponderValidationReport(ValidationReport report, ValidationReport responderReport
            ) {
            foreach (ReportItem reportItem in responderReport.GetLogs()) {
                report.AddReportItem(ReportItem.ReportItemStatus.INVALID == reportItem.GetStatus() ? reportItem.SetStatus(
                    ReportItem.ReportItemStatus.INDETERMINATE) : reportItem);
            }
        }

        private DateTime GetArchiveCutoffExtension(IBasicOcspResponse ocspResp) {
            // OCSP containing this extension specifies the reliable revocation status of the certificate
            // that expired after the date specified in the Archive Cutoff extension or at that date.
            IAsn1Encodable archiveCutoff = ocspResp.GetExtensionParsedValue(BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers
                ().GetIdPkixOcspArchiveCutoff());
            if (!archiveCutoff.IsNull()) {
                try {
                    return BOUNCY_CASTLE_FACTORY.CreateASN1GeneralizedTime(archiveCutoff).GetDate();
                }
                catch (Exception) {
                }
            }
            // Ignore exception.
            return (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE;
        }
    }
}
