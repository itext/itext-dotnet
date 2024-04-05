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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Class that allows you to validate a certificate against a Certificate Revocation List (CRL) Response.
    ///     </summary>
    public class CRLValidator {
        internal const String CRL_CHECK = "CRL response check.";

        internal const String ATTRIBUTE_CERTS_ASSERTED = "The onlyContainsAttributeCerts is asserted. Conforming CRLs "
             + "issuers MUST set the onlyContainsAttributeCerts boolean to FALSE.";

        internal const String CERTIFICATE_IS_UNREVOKED = "The certificate was unrevoked.";

        internal const String CERTIFICATE_IS_NOT_IN_THE_CRL_SCOPE = "Certificate isn't in the current CRL scope.";

        internal const String CERTIFICATE_REVOKED = "Certificate was revoked by {0} on {1}.";

        internal const String CRL_ISSUER_NOT_FOUND = "Unable to validate CRL response: no issuer certificate found.";

        internal const String CRL_ISSUER_NO_COMMON_ROOT = "The CRL issuer does not share the root of the inspected certificate.";

        internal const String CRL_INVALID = "CRL response is invalid.";

        internal const String FRESHNESS_CHECK = "CRL response is not fresh enough: " + "this update: {0}, validation date: {1}, freshness: {2}.";

        internal const String ONLY_SOME_REASONS_CHECKED = "Revocation status cannot be determined since " + "not all reason codes are covered by the current CRL.";

        internal const String SAME_REASONS_CHECK = "CRLs that cover the same reason codes were already verified.";

        internal const String UPDATE_DATE_BEFORE_CHECK_DATE = "nextUpdate: {0} of CRLResponse is before validation date {1}.";

        internal static readonly CertificateExtension KEY_USAGE_EXTENSION = new KeyUsageExtension(KeyUsage.CRL_SIGN
            );

        // All reasons without unspecified.
        internal const int ALL_REASONS = 32895;

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IDictionary<IX509Certificate, int?> checkedReasonsMask = new Dictionary<IX509Certificate, 
            int?>();

        private IssuingCertificateRetriever certificateRetriever = new IssuingCertificateRetriever();

        private CertificateChainValidator certificateChainValidator = new CertificateChainValidator();

        private long freshness = 3024000000L;

        // 35 days
        /// <summary>Instantiated a CRLValidator instance.</summary>
        /// <remarks>
        /// Instantiated a CRLValidator instance.
        /// This class allows you to validate a certificate against a Certificate Revocation List (CRL) Response.
        /// </remarks>
        public CRLValidator() {
        }

        // Empty constructor.
        /// <summary>
        /// Sets the revocation freshness value which is a time interval in milliseconds indicating that the validation
        /// accepts revocation data responses that were emitted at a point in time after the validation time minus the
        /// freshness: thisUpdate not before (validationTime - freshness).
        /// </summary>
        /// <remarks>
        /// Sets the revocation freshness value which is a time interval in milliseconds indicating that the validation
        /// accepts revocation data responses that were emitted at a point in time after the validation time minus the
        /// freshness: thisUpdate not before (validationTime - freshness).
        /// <para />
        /// If the revocation freshness constraint is respected by a revocation data then it can be used.
        /// </remarks>
        /// <param name="freshness">time interval in milliseconds identifying revocation freshness constraint</param>
        /// <returns>
        /// this same
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </returns>
        public virtual iText.Signatures.Validation.CRLValidator SetFreshness(long freshness) {
            this.freshness = freshness;
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
        /// to restore certificates chain that can be used
        /// to verify the signature on the CRL response
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CRLValidator"/>.
        /// </returns>
        public virtual iText.Signatures.Validation.CRLValidator SetIssuingCertificateRetriever(IssuingCertificateRetriever
             certificateRetriever) {
            this.certificateRetriever = certificateRetriever;
            this.certificateChainValidator.SetIssuingCertificateRetriever(certificateRetriever);
            return this;
        }

        /// <summary>
        /// Set
        /// <see cref="CertificateChainValidator"/>
        /// for the CRL issuer certificate.
        /// </summary>
        /// <param name="validator">
        /// 
        /// <see cref="CertificateChainValidator"/>
        /// to be a validator for the CRL issuer certificate
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="CRLValidator"/>.
        /// </returns>
        public virtual iText.Signatures.Validation.CRLValidator SetCertificateChainValidator(CertificateChainValidator
             validator) {
            this.certificateChainValidator = validator;
            return this;
        }

        /// <summary>Validates a certificate against Certificate Revocation List (CRL) Responses.</summary>
        /// <param name="report">to store all the chain verification results</param>
        /// <param name="certificate">the certificate to check against CRL response</param>
        /// <param name="crl">the crl response to be validated</param>
        /// <param name="validationDate">validation date to check for</param>
        public virtual void Validate(ValidationReport report, IX509Certificate certificate, IX509Crl crl, DateTime
             validationDate) {
            if (CertificateUtil.IsSelfSigned(certificate)) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, RevocationDataValidator.SELF_SIGNED_CERTIFICATE
                    , ReportItem.ReportItemStatus.INFO));
                return;
            }
            // Check that thisUpdate >= (validationDate - freshness).
            if (crl.GetThisUpdate().Before(DateTimeUtil.AddMillisToDate(validationDate, -freshness))) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(FRESHNESS_CHECK
                    , crl.GetThisUpdate(), validationDate, freshness), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            // Check that the validation date is before the nextUpdate.
            if (crl.GetNextUpdate() != TimestampConstants.UNDEFINED_TIMESTAMP_DATE && validationDate.After(crl.GetNextUpdate())) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, MessageFormatUtil.Format(UPDATE_DATE_BEFORE_CHECK_DATE
                    , crl.GetNextUpdate(), validationDate), ReportItem.ReportItemStatus.INDETERMINATE));
                return;
            }
            IIssuingDistributionPoint issuingDistPoint = GetIssuingDistributionPointExtension(crl);
            IDistributionPoint distributionPoint = null;
            if (!issuingDistPoint.IsNull()) {
                // Verify that certificate is in the CRL scope using IDP extension.
                bool basicConstraintsCaAsserted = new BasicConstraintsExtension(true).ExistsInCertificate(certificate);
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
                interimReasonsMask |= (int) reasonsMask;
                // Verify that interim_reasons_mask includes one or more reasons that are not included in the reasons_mask.
                if (interimReasonsMask == reasonsMask) {
                    report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, SAME_REASONS_CHECK, ReportItem.ReportItemStatus
                        .INFO));
                }
            }
            // Verify the CRL issuer.
            VerifyCrlIntegrity(report, certificate, crl);
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
            // Ignore exception.
            return FACTORY.CreateIssuingDistributionPoint(issuingDistPointExtension);
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

        private void VerifyCrlIntegrity(ValidationReport report, IX509Certificate certificate, IX509Crl crl) {
            IX509Certificate[] certs = certificateRetriever.GetCrlIssuerCertificates(crl);
            if (certs.Length == 0) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_NOT_FOUND, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            IX509Certificate crlIssuer = certs[0];
            IX509Certificate crlIssuerRoot = GetRoot(crlIssuer);
            IX509Certificate subjectRoot = GetRoot(certificate);
            if (!crlIssuerRoot.Equals(subjectRoot)) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_ISSUER_NO_COMMON_ROOT, ReportItem.ReportItemStatus
                    .INDETERMINATE));
            }
            try {
                crl.Verify(crlIssuer.GetPublicKey());
            }
            catch (Exception e) {
                report.AddReportItem(new CertificateReportItem(certificate, CRL_CHECK, CRL_INVALID, e, ReportItem.ReportItemStatus
                    .INDETERMINATE));
                return;
            }
            // Ideally this date should be the date this response was retrieved from the server.
            DateTime crlIssuerDate;
            if (TimestampConstants.UNDEFINED_TIMESTAMP_DATE != crl.GetNextUpdate()) {
                crlIssuerDate = crl.GetNextUpdate();
                report.AddReportItem(new CertificateReportItem((IX509Certificate)crlIssuer, CRL_CHECK, "Using crl nextUpdate date as validation date"
                    , ReportItem.ReportItemStatus.INFO));
            }
            else {
                crlIssuerDate = crl.GetThisUpdate();
                report.AddReportItem(new CertificateReportItem((IX509Certificate)crlIssuer, CRL_CHECK, "Using crl thisUpdate date as validation date"
                    , ReportItem.ReportItemStatus.INFO));
            }
            certificateChainValidator.Validate(report, (IX509Certificate)crlIssuer, crlIssuerDate, JavaCollectionsUtil
                .SingletonList(KEY_USAGE_EXTENSION));
        }

        private IX509Certificate GetRoot(IX509Certificate cert) {
            IX509Certificate[] chain = certificateRetriever.RetrieveMissingCertificates(new IX509Certificate[] { cert }
                );
            return chain[chain.Length - 1];
        }
    }
}
