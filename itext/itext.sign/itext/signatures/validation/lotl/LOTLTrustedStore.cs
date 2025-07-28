/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at http://itextpdf.com/sales.  For AGPL licensing, see below.

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
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Trusted certificates storage class for country specific LOTL trusted certificates.</summary>
    public class LOTLTrustedStore {
//\cond DO_NOT_DOCUMENT
        internal const String REVOKED_CERTIFICATE = "Certificate {0} is revoked.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_CHECK = "Certificate check.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_TRUSTED = "Certificate {0} is trusted, revocation data checks are not required.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT = "Certificate {0} is trusted for {1}, " +
             "but it is not used in this context. Validation will continue as usual.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String CERTIFICATE_SERVICE_TYPE_NOT_RECOGNIZED = "Certificate {0} is trusted, but it's service type {1} is not recognized.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String NOT_YET_VALID_CERTIFICATE = "Certificate {0} is not yet valid.";
//\endcond

        private static readonly IDictionary<String, ICollection<CertificateSource>> serviceTypeIdentifiersScope;

        private readonly ICollection<CountryServiceContext> contexts = new HashSet<CountryServiceContext>();

        private readonly ValidationReport report;

        static LOTLTrustedStore() {
            ICollection<CertificateSource> crlOcspSignScope = new HashSet<CertificateSource>();
            crlOcspSignScope.Add(CertificateSource.CRL_ISSUER);
            crlOcspSignScope.Add(CertificateSource.OCSP_ISSUER);
            crlOcspSignScope.Add(CertificateSource.SIGNER_CERT);
            ICollection<CertificateSource> ocspScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                <CertificateSource>(CertificateSource.OCSP_ISSUER));
            ICollection<CertificateSource> crlScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                <CertificateSource>(CertificateSource.CRL_ISSUER));
            ICollection<CertificateSource> timestampScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                <CertificateSource>(CertificateSource.TIMESTAMP));
            ICollection<CertificateSource> signScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                <CertificateSource>(CertificateSource.SIGNER_CERT));
            IDictionary<String, ICollection<CertificateSource>> tempServiceTypeIdentifiersScope = new Dictionary<String
                , ICollection<CertificateSource>>();
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/CA/QC", crlOcspSignScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/CA/PKC", crlOcspSignScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP/QC", ocspScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL/QC", crlScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TSA/QTST", timestampScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EDS/Q", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EDS/REM/Q", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PSES/Q", timestampScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/QESValidation/Q", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RemoteQSigCDManagement/Q", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RemoteQSealCDManagement/Q", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EAA/Q", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving/Q", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Ledgers/Q", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP", crlScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL", crlScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TS/", timestampScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-QC", timestampScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-AdESQCandQES", timestampScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PSES", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/AdESValidation", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/AdESGeneration", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RemoteSigCDManagemen", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RemoteSealCDManagement", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EAA", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Ledgers", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PKCValidation", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PKCPreservation", timestampScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EAAValidation", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TSTValidation", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EDSValidation", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/EAA/Pub-EAA", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PKCValidation/CertsforOtherTypesOfTS"
                , signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RA", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RA/nothavingPKIid", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/SignaturePolicyAuthority", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Archiv", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Archiv/nothavingPKIid", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/IdV", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/KEscrow", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/KEscrow/nothavingPKIid", signScope
                );
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PPwd", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/PPwd/nothavingPKIid", signScope);
            tempServiceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/TLIssuer", signScope);
            serviceTypeIdentifiersScope = JavaCollectionsUtil.UnmodifiableMap(tempServiceTypeIdentifiersScope);
        }

        /// <summary>
        /// Creates new instance of
        /// <see cref="LOTLTrustedStore"/>.
        /// </summary>
        /// <remarks>
        /// Creates new instance of
        /// <see cref="LOTLTrustedStore"/>
        /// . This constructor shall not be used directly.
        /// Instead, in order to create such instance
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder.GetLOTLTrustedstore()"/>
        /// shall be used.
        /// </remarks>
        /// <param name="builder">
        /// 
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder"/>
        /// which was responsible for creation
        /// </param>
        public LOTLTrustedStore(ValidatorChainBuilder builder) {
            if (builder.GetLotlFetchingProperties() != null) {
                LOTLValidator lotlValidator = builder.GetLotlValidator();
                this.report = lotlValidator.Validate();
                if (report.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    AddCertificatesWithContext(lotlValidator.GetNationalTrustedCertificates());
                }
            }
            else {
                this.report = new ValidationReport();
            }
        }

        /// <summary>Gets all the certificates stored in this trusted store.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// stored
        /// </returns>
        public virtual ICollection<IX509Certificate> GetCertificates() {
            ICollection<IX509Certificate> allCertificates = new HashSet<IX509Certificate>();
            foreach (CountryServiceContext context in contexts) {
                allCertificates.AddAll(context.GetCertificates());
            }
            return allCertificates;
        }

        /// <summary>Checks if given certificate is trusted according to context and time in which it is used.</summary>
        /// <param name="result">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which stores check results
        /// </param>
        /// <param name="context">
        /// 
        /// <see cref="iText.Signatures.Validation.Context.ValidationContext"/>
        /// in which certificate is used
        /// </param>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// certificate to be checked
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// date time in which certificate is validated
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if certificate is trusted,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool CheckIfCertIsTrusted(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime validationDate) {
            ICollection<CountryServiceContext> currentContextSet = GetCertificateContext(certificate);
            result.MergeWithDifferentStatus(report, ReportItem.ReportItemStatus.INFO);
            IList<ReportItem> validationReportItems = new List<ReportItem>();
            foreach (CountryServiceContext currentContext in currentContextSet) {
                if (!IsCertificateValidInTime(validationReportItems, certificate, currentContext, validationDate)) {
                    continue;
                }
                ICollection<CertificateSource> currentScope = serviceTypeIdentifiersScope.Get(currentContext.GetServiceType
                    ());
                if (currentScope == null) {
                    validationReportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format
                        (CERTIFICATE_SERVICE_TYPE_NOT_RECOGNIZED, certificate.GetSubjectDN(), currentContext.GetServiceType())
                        , ReportItem.ReportItemStatus.INFO));
                    continue;
                }
                foreach (CertificateSource source in currentScope) {
                    if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, source)) {
                        result.AddReportItem(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(CERTIFICATE_TRUSTED
                            , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INFO));
                        return true;
                    }
                    else {
                        validationReportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format
                            (CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, certificate.GetSubjectDN(), currentContext.GetServiceType(
                            )), ReportItem.ReportItemStatus.INFO));
                    }
                }
            }
            foreach (ReportItem reportItem in validationReportItems) {
                result.AddReportItem(reportItem);
            }
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal void AddCertificatesWithContext(ICollection<CountryServiceContext> contexts) {
            this.contexts.AddAll(contexts);
        }
//\endcond

        private ICollection<CountryServiceContext> GetCertificateContext(IX509Certificate certificate) {
            ICollection<CountryServiceContext> contextSet = new HashSet<CountryServiceContext>();
            foreach (CountryServiceContext context in contexts) {
                if (context.GetCertificates().Contains(certificate)) {
                    contextSet.Add(context);
                }
            }
            return contextSet;
        }

        private static bool IsCertificateValidInTime(IList<ReportItem> reportItems, IX509Certificate certificate, 
            CountryServiceContext currentContext, DateTime validationDate) {
            String status = currentContext.GetServiceStatusByDate(DateTimeUtil.GetRelativeTime(validationDate));
            if (status == null) {
                reportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(NOT_YET_VALID_CERTIFICATE
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            if (!ServiceStatusInfo.IsStatusValid(status)) {
                reportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(REVOKED_CERTIFICATE
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }
    }
}
