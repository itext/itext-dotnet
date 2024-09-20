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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Class that allows you to validate a certificate against a Certificate Revocation List (CRL) Response.
    ///     </summary>
    public class CRLValidator {
//\cond DO_NOT_DOCUMENT
        internal const String CRL_CHECK = "CRL response check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ATTRIBUTE_CERTS_ASSERTED = "The onlyContainsAttributeCerts is asserted. Conforming CRLs "
             + "issuers MUST set the onlyContainsAttributeCerts boolean to FALSE.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_IS_EXPIRED = "Certificate is expired on {0} and could have been removed from the CRL.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_IS_UNREVOKED = "The certificate was unrevoked.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_IS_NOT_IN_THE_CRL_SCOPE = "Certificate isn't in the current CRL scope.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_REVOKED = "Certificate was revoked by {0} on {1}.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CRL_ISSUER_NOT_FOUND = "Unable to validate CRL response: no issuer certificate found.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CRL_ISSUER_REQUEST_FAILED = "Unable to validate CRL response: Unexpected exception occurred retrieving issuer certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CRL_ISSUER_CHAIN_FAILED = "Unable to validate CRL response: Unexpected exception occurred validating issuer certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CRL_ISSUER_NO_COMMON_ROOT = "The CRL issuer does not share the root of the inspected certificate.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CRL_INVALID = "CRL response is invalid.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FRESHNESS_CHECK = "CRL response is not fresh enough: " + "this update: {0}, validation date: {1}, freshness: {2}.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ONLY_SOME_REASONS_CHECKED = "Revocation status cannot be determined since " + "not all reason codes are covered by the current CRL.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SAME_REASONS_CHECK = "CRLs that cover the same reason codes were already verified.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UPDATE_DATE_BEFORE_CHECK_DATE = "nextUpdate: {0} of CRLResponse is before validation date {1}.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_IN_ISSUER_CHAIN = "Unable to validate CRL response: validated certificate is"
             + " part of issuer certificate chain.";
//\endcond

//\cond DO_NOT_DOCUMENT
        // All reasons without unspecified.
        internal const int ALL_REASONS = 32895;
//\endcond

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IDictionary<IX509Certificate, int?> checkedReasonsMask = new Dictionary<IX509Certificate, 
            int?>();

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly SignatureValidationProperties properties;

        private readonly ValidatorChainBuilder builder;

        /// <summary>
        /// Creates new
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal CRLValidator(ValidatorChainBuilder builder) {
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.builder = builder;
        }

        /// <summary>Validates a certificate against Certificate Revocation List (CRL) Responses.</summary>
        /// <param name="report">to store all the chain verification results</param>
        /// <param name="context">the context in which to perform the validation</param>
        /// <param name="certificate">the certificate to check against CRL response</param>
        /// <param name="crl">the crl response to be validated</param>
        /// <param name="validationDate">validation date to check for</param>
        /// <param name="responseGenerationDate">trusted date at which response is generated</param>
        public virtual void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , IX509Crl crl, DateTime validationDate, DateTime responseGenerationDate) {
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.CRL_VALIDATOR);
            if (CertificateUtil.IsSelfSigned(certificate)) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, RevocationDataValidator.SELF_SIGNED_CERTIFICATE
                    , ReportItem.ReportItemStatus.INFO));
                return;
            }
            TimeSpan freshness = properties.GetFreshness(localContext);
            // Check that thisUpdate + freshness < validation.
            if (DateTimeUtil.AddMillisToDate(crl.GetThisUpdate(), (long)freshness.TotalMilliseconds).Before(validationDate
                )) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(FRESHNESS_CHECK
                    , crl.GetThisUpdate(), validationDate, freshness), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            // Check that the validation date is before the nextUpdate.
            if (crl.GetNextUpdate() != TimestampConstants.UNDEFINED_TIMESTAMP_DATE && validationDate.After(crl.GetNextUpdate
                ())) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(UPDATE_DATE_BEFORE_CHECK_DATE
                    , crl.GetNextUpdate(), validationDate), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            // Check expiredCertOnCrl extension in case the certificate is expired.
            if (certificate.GetNotAfter().Before(crl.GetThisUpdate())) {
                DateTime startExpirationDate = GetExpiredCertsOnCRLExtensionDate(crl);
                if (TimestampConstants.UNDEFINED_TIMESTAMP_DATE == startExpirationDate || certificate.GetNotAfter().Before
                    (startExpirationDate)) {
                    report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(CERTIFICATE_IS_EXPIRED
                        , certificate.GetNotAfter()), ReportItem.ReportItemStatus.INDETERMINATE));
                    return;
                }
            }
            IIssuingDistributionPoint issuingDistPoint = GetIssuingDistributionPointExtension(crl);
            IDistributionPoint distributionPoint = null;
            if (!issuingDistPoint.IsNull()) {
                // Verify that certificate is in the CRL scope using IDP extension.
                bool basicConstraintsCaAsserted = new DynamicBasicConstraintsExtension().WithCertificateChainSize(1).ExistsInCertificate
                    (certificate);
                if ((issuingDistPoint.OnlyContainsUserCerts() && basicConstraintsCaAsserted) || (issuingDistPoint.OnlyContainsCACerts
                    () && !basicConstraintsCaAsserted)) {
                    report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CERTIFICATE_IS_NOT_IN_THE_CRL_SCOPE
                        , ReportItem.ReportItemStatus.INDETERMINATE));
                    return;
                }
                if (issuingDistPoint.OnlyContainsAttributeCerts()) {
                    report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, ATTRIBUTE_CERTS_ASSERTED, ReportItem.ReportItemStatus
                        .INDETERMINATE));
                    return;
                }
                // Try to retrieve corresponding DP from the certificate by name specified in the IDP.
                if (!issuingDistPoint.GetDistributionPoint().IsNull()) {
                    distributionPoint = CertificateUtil.GetDistributionPointByName(certificate, issuingDistPoint.GetDistributionPoint
                        ());
                }
            }
            int interimReasonsMask = ComputeInterimReasonsMask(issuingDistPoint, distributionPoint);
            int? reasonsMask = checkedReasonsMask.Get(certificate);
            if (reasonsMask != null) {
                interimReasonsMask |= (int)reasonsMask;
            }
            // Verify the CRL issuer.
            VerifyCrlIntegrity(report, localContext, certificate, crl, responseGenerationDate);
            // Check the status of the certificate.
            VerifyRevocation(report, certificate, validationDate, crl);
            if (report.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                checkedReasonsMask.Put(certificate, interimReasonsMask);
            }
            // If ((reasons_mask is all-reasons) OR (cert_status is not UNREVOKED)),
            // then the revocation status has been determined.
            if (interimReasonsMask != ALL_REASONS) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, ONLY_SOME_REASONS_CHECKED, ReportItem.ReportItemStatus
                    .INDETERMINATE));
            }
        }

        private static void VerifyRevocation(ValidationReport report, IX509Certificate certificate, DateTime verificationDate
            , IX509Crl crl) {
            IX509CrlEntry revocation = crl.GetRevokedCertificate(certificate.GetSerialNumber());
            if (revocation != null) {
                DateTime revocationDate = revocation.GetRevocationDate();
                if (verificationDate.Before(revocationDate)) {
                    report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(SignLogMessageConstant
                        .VALID_CERTIFICATE_IS_REVOKED, revocationDate), ReportItem.ReportItemStatus.INFO));
                }
                else {
                    if (CRLReason.REMOVE_FROM_CRL == revocation.GetReason()) {
                        report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(CERTIFICATE_IS_UNREVOKED
                            , revocationDate), ReportItem.ReportItemStatus.INFO));
                    }
                    else {
                        report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(CERTIFICATE_REVOKED
                            , crl.GetIssuerDN(), revocation.GetRevocationDate()), ReportItem.ReportItemStatus.INVALID));
                    }
                }
            }
        }

        private static IIssuingDistributionPoint GetIssuingDistributionPointExtension(IX509Crl crl) {
            IAsn1Object issuingDistPointExtension = null;
            try {
                issuingDistPointExtension = CertificateUtil.GetExtensionValue(crl, FACTORY.CreateExtensions().GetIssuingDistributionPoint
                    ().GetId());
            }
            catch (System.IO.IOException) {
            }
            catch (Exception) {
            }
            // Ignore exception.
            return FACTORY.CreateIssuingDistributionPoint(issuingDistPointExtension);
        }

        private static DateTime GetExpiredCertsOnCRLExtensionDate(IX509Crl crl) {
            IAsn1Encodable expiredCertsOnCRL = null;
            try {
                // The scope of a CRL containing this extension is extended to include the revocation status of the
                // certificates that expired after the date specified in ExpiredCertsOnCRL or at that date.
                expiredCertsOnCRL = CertificateUtil.GetExtensionValue(crl, FACTORY.CreateExtensions().GetExpiredCertsOnCRL
                    ().GetId());
            }
            catch (System.IO.IOException) {
            }
            catch (Exception) {
            }
            // Ignore exception.
            if (expiredCertsOnCRL != null) {
                try {
                    return FACTORY.CreateASN1GeneralizedTime(expiredCertsOnCRL).GetDate();
                }
                catch (Exception) {
                }
            }
            // Ignore exception.
            return (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE;
        }

        private static int ComputeInterimReasonsMask(IIssuingDistributionPoint issuingDistPoint, IDistributionPoint
             distributionPoint) {
            int interimReasonsMask = ALL_REASONS;
            if (!issuingDistPoint.IsNull()) {
                IReasonFlags onlySomeReasons = issuingDistPoint.GetOnlySomeReasons();
                if (!onlySomeReasons.IsNull()) {
                    interimReasonsMask &= onlySomeReasons.IntValue();
                }
            }
            if (distributionPoint != null) {
                IReasonFlags reasons = distributionPoint.GetReasons();
                if (!reasons.IsNull()) {
                    interimReasonsMask &= reasons.IntValue();
                }
            }
            return interimReasonsMask;
        }

        private void VerifyCrlIntegrity(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , IX509Crl crl, DateTime responseGenerationDate) {
            IX509Certificate[] certs = null;
            try {
                certs = certificateRetriever.GetCrlIssuerCertificates(crl);
            }
            catch (Exception e) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_REQUEST_FAILED, e, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            if (certs == null || certs.Length == 0) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_NOT_FOUND, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            if (JavaUtil.ArraysToEnumerable(certs).Any((c) => c.Equals(certificate))) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CERTIFICATE_IN_ISSUER_CHAIN, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            IX509Certificate crlIssuer = certs[0];
            IX509Certificate crlIssuerRoot = GetRoot(crlIssuer);
            IX509Certificate subjectRoot = GetRoot(certificate);
            if (!crlIssuerRoot.Equals(subjectRoot)) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_NO_COMMON_ROOT, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            SafeCalling.OnExceptionLog(() => crl.Verify(crlIssuer.GetPublicKey()), report, (e) => new CertificateReportItem
                (certificate, CRL_CHECK, CRL_INVALID, e, ReportItem.ReportItemStatus.INDETERMINATE));
            ValidationReport responderReport = new ValidationReport();
            SafeCalling.OnExceptionLog(() => builder.GetCertificateChainValidator().Validate(responderReport, context.
                SetCertificateSource(CertificateSource.CRL_ISSUER), (IX509Certificate)crlIssuer, responseGenerationDate
                ), report, (e) => new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_CHAIN_FAILED, e, ReportItem.ReportItemStatus
                .INDETERMINATE));
            AddResponderValidationReport(report, responderReport);
        }

        private IX509Certificate GetRoot(IX509Certificate cert) {
            IX509Certificate[] chain = certificateRetriever.RetrieveMissingCertificates(new IX509Certificate[] { cert }
                );
            return chain[chain.Length - 1];
        }

        private static void AddResponderValidationReport(ValidationReport report, ValidationReport responderReport
            ) {
            foreach (ReportItem reportItem in responderReport.GetLogs()) {
                report.AddReportItem(ReportItem.ReportItemStatus.INVALID == reportItem.GetStatus() ? reportItem.SetStatus(
                    ReportItem.ReportItemStatus.INDETERMINATE) : reportItem);
            }
        }
    }
}
