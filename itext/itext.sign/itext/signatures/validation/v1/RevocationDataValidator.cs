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
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.Signatures;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
    /// <summary>Class that allows you to fetch and validate revocation data for the certificate.</summary>
    public class RevocationDataValidator {
        internal const String REVOCATION_DATA_CHECK = "Revocation data check.";

        internal const String NO_REVOCATION_DATA = "Certificate revocation status cannot be checked: " + "no revocation data available or the status cannot be determined.";

        internal const String SELF_SIGNED_CERTIFICATE = "Certificate is self-signed. Revocation data check will be skipped.";

        internal const String TRUSTED_OCSP_RESPONDER = "Authorized OCSP Responder certificate has id-pkix-ocsp-nocheck "
             + "extension so it is trusted by the definition and no revocation checking is performed.";

        internal const String VALIDITY_ASSURED = "Certificate is trusted due to validity assured - short term extension.";

        internal const String CANNOT_PARSE_OCSP = "OCSP response from \"{0}\" OCSP client cannot be parsed.";

        internal const String CANNOT_PARSE_CRL = "CRL response from \"{0}\" CRL client cannot be parsed.";

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IList<IOcspClient> ocspClients = new List<IOcspClient>();

        private readonly IList<ICrlClient> crlClients = new List<ICrlClient>();

        private readonly SignatureValidationProperties properties;

        private readonly IssuingCertificateRetriever certificateRetriever;

        private readonly OCSPValidator ocspValidator;

        private readonly CRLValidator crlValidator;

        /// <summary>
        /// Creates new
        /// <see cref="RevocationDataValidator"/>
        /// instance to validate certificate revocation data.
        /// </summary>
        /// <param name="builder">
        /// See
        /// <see cref="ValidatorChainBuilder"/>
        /// </param>
        protected internal RevocationDataValidator(ValidatorChainBuilder builder) {
            this.certificateRetriever = builder.GetCertificateRetriever();
            this.properties = builder.GetProperties();
            this.ocspValidator = builder.GetOCSPValidator();
            this.crlValidator = builder.GetCRLValidator();
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
        /// <see cref="RevocationDataValidator"/>.
        /// </returns>
        public virtual iText.Signatures.Validation.V1.RevocationDataValidator AddCrlClient(ICrlClient crlClient) {
            this.crlClients.Add(crlClient);
            return this;
        }

        /// <summary>
        /// Add
        /// <see cref="iText.Signatures.IOcspClient"/>
        /// to be used for OCSP responses receiving.
        /// </summary>
        /// <param name="ocspClient">
        /// 
        /// <see cref="iText.Signatures.IOcspClient"/>
        /// to be used for OCSP responses receiving
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="RevocationDataValidator"/>.
        /// </returns>
        public virtual iText.Signatures.Validation.V1.RevocationDataValidator AddOcspClient(IOcspClient ocspClient
            ) {
            this.ocspClients.Add(ocspClient);
            return this;
        }

        /// <summary>Validates revocation data (Certificate Revocation List (CRL) Responses and OCSP Responses) of the certificate.
        ///     </summary>
        /// <param name="report">to store all the verification results</param>
        /// <param name="context">
        /// 
        /// <see cref="iText.Signatures.Validation.V1.Context.ValidationContext"/>
        /// the context
        /// </param>
        /// <param name="certificate">the certificate to check revocation data for</param>
        /// <param name="validationDate">validation date to check for</param>
        public virtual void Validate(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate) {
            ValidationContext localContext = context.SetValidatorContext(ValidatorContext.REVOCATION_DATA_VALIDATOR);
            if (CertificateUtil.IsSelfSigned(certificate)) {
                report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, SELF_SIGNED_CERTIFICATE
                    , ReportItem.ReportItemStatus.INFO));
                return;
            }
            // Check Validity Assured - Short Term extension which indicates that the validity of the certificate is assured
            // because the certificate is a "short-term certificate".
            if (CertificateUtil.GetExtensionValueByOid(certificate, OID.X509Extensions.VALIDITY_ASSURED_SHORT_TERM) !=
                 null) {
                report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, VALIDITY_ASSURED, ReportItem.ReportItemStatus
                    .INFO));
                return;
            }
            if (CertificateSource.OCSP_ISSUER == localContext.GetCertificateSource()) {
                // Check if Authorised OCSP Responder certificate has id-pkix-ocsp-nocheck extension, in which case we
                // do not perform revocation check for it.
                if (CertificateUtil.GetExtensionValueByOid(certificate, BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers(
                    ).GetIdPkixOcspNoCheck().GetId()) != null) {
                    report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, TRUSTED_OCSP_RESPONDER, 
                        ReportItem.ReportItemStatus.INFO));
                    return;
                }
            }
            // Collect revocation data.
            IList<RevocationDataValidator.OcspResponseValidationInfo> ocspResponses = RetrieveAllOCSPResponses(report, 
                localContext, certificate);
            ocspResponses = ocspResponses.Sorted((o1, o2) => o2.singleResp.GetThisUpdate().CompareTo(o1.singleResp.GetThisUpdate
                ())).ToList();
            IList<RevocationDataValidator.CrlValidationInfo> crlResponses = RetrieveAllCRLResponses(report, localContext
                , certificate);
            // Try to check responderCert for revocation using provided responder OCSP/CRL clients or
            // Authority Information Access for OCSP responses and CRL Distribution Points for CRL responses
            // using default clients.
            ValidateRevocationData(report, localContext, certificate, validationDate, ocspResponses, crlResponses);
        }

        private void ValidateRevocationData(ValidationReport report, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate, IList<RevocationDataValidator.OcspResponseValidationInfo> ocspResponses, IList
            <RevocationDataValidator.CrlValidationInfo> crlResponses) {
            int i = 0;
            int j = 0;
            while (i < ocspResponses.Count || j < crlResponses.Count) {
                ValidationReport revDataValidationReport = new ValidationReport();
                if (i < ocspResponses.Count && (j >= crlResponses.Count || ocspResponses[i].singleResp.GetThisUpdate().After
                    (crlResponses[j].crl.GetThisUpdate()))) {
                    RevocationDataValidator.OcspResponseValidationInfo validationInfo = ocspResponses[i];
                    ocspValidator.Validate(revDataValidationReport, context.SetTimeBasedContext(validationInfo.timeBasedContext
                        ), certificate, validationInfo.singleResp, validationInfo.basicOCSPResp, validationDate, validationInfo
                        .trustedGenerationDate);
                    i++;
                }
                else {
                    RevocationDataValidator.CrlValidationInfo validationInfo = crlResponses[j];
                    crlValidator.Validate(revDataValidationReport, context.SetTimeBasedContext(validationInfo.timeBasedContext
                        ), certificate, validationInfo.crl, validationDate, validationInfo.trustedGenerationDate);
                    j++;
                }
                if (ValidationReport.ValidationResult.INDETERMINATE == revDataValidationReport.GetValidationResult()) {
                    foreach (ReportItem reportItem in revDataValidationReport.GetLogs()) {
                        // These messages are useless for the user, we don't want them to be in the resulting report.
                        if (!OCSPValidator.SERIAL_NUMBERS_DO_NOT_MATCH.Equals(reportItem.GetMessage()) && !CRLValidator.CRL_ISSUER_NO_COMMON_ROOT
                            .Equals(reportItem.GetMessage())) {
                            report.AddReportItem(reportItem.SetStatus(ReportItem.ReportItemStatus.INFO));
                        }
                    }
                }
                else {
                    report.Merge(revDataValidationReport);
                    return;
                }
            }
            report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, NO_REVOCATION_DATA, ReportItem.ReportItemStatus
                .INDETERMINATE));
        }

        private IList<RevocationDataValidator.OcspResponseValidationInfo> RetrieveAllOCSPResponses(ValidationReport
             report, ValidationContext context, IX509Certificate certificate) {
            IList<RevocationDataValidator.OcspResponseValidationInfo> ocspResponses = new List<RevocationDataValidator.OcspResponseValidationInfo
                >();
            foreach (IOcspClient ocspClient in ocspClients) {
                if (ocspClient is ValidationOcspClient) {
                    ValidationOcspClient validationOcspClient = (ValidationOcspClient)ocspClient;
                    foreach (KeyValuePair<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo> response in 
                        validationOcspClient.GetResponses()) {
                        FillOcspResponses(ocspResponses, response.Key, response.Value.trustedGenerationDate, response.Value.timeBasedContext
                            );
                    }
                }
                else {
                    byte[] basicOcspRespBytes = ocspClient.GetEncoded(certificate, (IX509Certificate)certificateRetriever.RetrieveIssuerCertificate
                        (certificate), null);
                    if (basicOcspRespBytes != null) {
                        try {
                            IBasicOcspResponse basicOCSPResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1Primitive
                                (basicOcspRespBytes));
                            FillOcspResponses(ocspResponses, basicOCSPResp, DateTimeUtil.GetCurrentUtcTime(), TimeBasedContext.PRESENT
                                );
                        }
                        catch (System.IO.IOException e) {
                            report.AddReportItem(new ReportItem(REVOCATION_DATA_CHECK, MessageFormatUtil.Format(CANNOT_PARSE_OCSP, ocspClient
                                ), e, ReportItem.ReportItemStatus.INFO));
                        }
                    }
                }
            }
            SignatureValidationProperties.OnlineFetching onlineFetching = properties.GetRevocationOnlineFetching(context
                .SetValidatorContext(ValidatorContext.OCSP_VALIDATOR));
            if (SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH == onlineFetching || (SignatureValidationProperties.OnlineFetching
                .FETCH_IF_NO_OTHER_DATA_AVAILABLE == onlineFetching && ocspResponses.IsEmpty())) {
                IBasicOcspResponse basicOCSPResp = new OcspClientBouncyCastle(null).GetBasicOCSPResp(certificate, (IX509Certificate
                    )certificateRetriever.RetrieveIssuerCertificate(certificate), null);
                FillOcspResponses(ocspResponses, basicOCSPResp, DateTimeUtil.GetCurrentUtcTime(), TimeBasedContext.PRESENT
                    );
            }
            return ocspResponses;
        }

        private IList<RevocationDataValidator.CrlValidationInfo> RetrieveAllCRLResponses(ValidationReport report, 
            ValidationContext context, IX509Certificate certificate) {
            IList<RevocationDataValidator.CrlValidationInfo> crlResponses = new List<RevocationDataValidator.CrlValidationInfo
                >();
            foreach (ICrlClient crlClient in crlClients) {
                crlResponses.AddAll(RetrieveAllCRLResponsesUsingClient(report, certificate, crlClient));
            }
            SignatureValidationProperties.OnlineFetching onLineFetching = properties.GetRevocationOnlineFetching(context
                .SetValidatorContext(ValidatorContext.CRL_VALIDATOR));
            if (SignatureValidationProperties.OnlineFetching.ALWAYS_FETCH == onLineFetching || (SignatureValidationProperties.OnlineFetching
                .FETCH_IF_NO_OTHER_DATA_AVAILABLE == onLineFetching && crlResponses.IsEmpty())) {
                crlResponses.AddAll(RetrieveAllCRLResponsesUsingClient(report, certificate, new CrlClientOnline()));
            }
            // Sort all the CRL responses available based on the most recent revocation data.
            return crlResponses.Sorted((o1, o2) => o2.crl.GetThisUpdate().CompareTo(o1.crl.GetThisUpdate())).ToList();
        }

        private static void FillOcspResponses(IList<RevocationDataValidator.OcspResponseValidationInfo> ocspResponses
            , IBasicOcspResponse basicOCSPResp, DateTime generationDate, TimeBasedContext timeBasedContext) {
            if (basicOCSPResp != null) {
                // Getting the responses.
                ISingleResponse[] singleResponses = basicOCSPResp.GetResponses();
                foreach (ISingleResponse singleResponse in singleResponses) {
                    ocspResponses.Add(new RevocationDataValidator.OcspResponseValidationInfo(singleResponse, basicOCSPResp, generationDate
                        , timeBasedContext));
                }
            }
        }

        private static IList<RevocationDataValidator.CrlValidationInfo> RetrieveAllCRLResponsesUsingClient(ValidationReport
             report, IX509Certificate certificate, ICrlClient crlClient) {
            IList<RevocationDataValidator.CrlValidationInfo> crlResponses = new List<RevocationDataValidator.CrlValidationInfo
                >();
            if (crlClient is ValidationCrlClient) {
                ValidationCrlClient validationCrlClient = (ValidationCrlClient)crlClient;
                crlResponses.AddAll(validationCrlClient.GetCrls().Values);
            }
            else {
                try {
                    ICollection<byte[]> crlBytesCollection = crlClient.GetEncoded(certificate, null);
                    foreach (byte[] crlBytes in crlBytesCollection) {
                        try {
                            crlResponses.Add(new RevocationDataValidator.CrlValidationInfo((IX509Crl)CertificateUtil.ParseCrlFromBytes
                                (crlBytes), DateTimeUtil.GetCurrentUtcTime(), TimeBasedContext.PRESENT));
                        }
                        catch (Exception) {
                            report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, MessageFormatUtil.Format
                                (CANNOT_PARSE_CRL, crlClient), ReportItem.ReportItemStatus.INFO));
                        }
                    }
                }
                catch (AbstractGeneralSecurityException) {
                    report.AddReportItem(new CertificateReportItem(certificate, REVOCATION_DATA_CHECK, MessageFormatUtil.Format
                        (CANNOT_PARSE_CRL, crlClient), ReportItem.ReportItemStatus.INFO));
                }
            }
            return crlResponses;
        }

        /// <summary>Class which contains validation related information about single OCSP response.</summary>
        public class OcspResponseValidationInfo {
            internal readonly ISingleResponse singleResp;

            internal readonly IBasicOcspResponse basicOCSPResp;

            internal readonly DateTime trustedGenerationDate;

            internal readonly TimeBasedContext timeBasedContext;

            /// <summary>Creates validation related information about single OCSP response.</summary>
            /// <param name="singleResp">
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Cert.Ocsp.ISingleResponse"/>
            /// single response to be validated
            /// </param>
            /// <param name="basicOCSPResp">
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
            /// basic OCSP response which contains this single response
            /// </param>
            /// <param name="trustedGenerationDate">
            /// 
            /// <see cref="System.DateTime"/>
            /// trusted date at which response was generated
            /// </param>
            /// <param name="timeBasedContext">
            /// 
            /// <see cref="iText.Signatures.Validation.V1.Context.TimeBasedContext"/>
            /// time based context which corresponds to generation date
            /// </param>
            public OcspResponseValidationInfo(ISingleResponse singleResp, IBasicOcspResponse basicOCSPResp, DateTime trustedGenerationDate
                , TimeBasedContext timeBasedContext) {
                this.singleResp = singleResp;
                this.basicOCSPResp = basicOCSPResp;
                this.trustedGenerationDate = trustedGenerationDate;
                this.timeBasedContext = timeBasedContext;
            }
        }

        /// <summary>Class which contains validation related information about CRL response.</summary>
        public class CrlValidationInfo {
            internal readonly IX509Crl crl;

            internal readonly DateTime trustedGenerationDate;

            internal readonly TimeBasedContext timeBasedContext;

            /// <summary>Creates validation related information about CRL response.</summary>
            /// <param name="crl">
            /// 
            /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Crl"/>
            /// CRL to be validated
            /// </param>
            /// <param name="trustedGenerationDate">
            /// 
            /// <see cref="System.DateTime"/>
            /// trusted date at which response was generated
            /// </param>
            /// <param name="timeBasedContext">
            /// 
            /// <see cref="iText.Signatures.Validation.V1.Context.TimeBasedContext"/>
            /// time based context which corresponds to generation date
            /// </param>
            public CrlValidationInfo(IX509Crl crl, DateTime trustedGenerationDate, TimeBasedContext timeBasedContext) {
                this.crl = crl;
                this.trustedGenerationDate = trustedGenerationDate;
                this.timeBasedContext = timeBasedContext;
            }
        }
    }
}
