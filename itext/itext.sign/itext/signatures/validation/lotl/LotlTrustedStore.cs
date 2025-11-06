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
using iText.Signatures.Exceptions;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Trusted certificates storage class for country specific Lotl trusted certificates.</summary>
    public class LotlTrustedStore {
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

//\cond DO_NOT_DOCUMENT
        internal const String SCOPE_SPECIFIED_WITH_INVALID_TYPES = "Certificate {0} is trusted for {1}, " + "which is incorrect scope for pdf validation.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String EXTENSIONS_CHECK = "Certificate extensions check.";
//\endcond

        private static readonly IDictionary<String, ICollection<CertificateSource>> serviceTypeIdentifiersScope;

        private readonly ICollection<CountryServiceContext> contexts = new HashSet<CountryServiceContext>();

        private readonly QualifiedValidator qualifiedValidator;

        private readonly ValidationReport report;

        private IList<IX509Certificate> previousCertificates = new List<IX509Certificate>();

        static LotlTrustedStore() {
            ICollection<CertificateSource> crlOcspSignScope = new HashSet<CertificateSource>();
            crlOcspSignScope.Add(CertificateSource.CRL_ISSUER);
            crlOcspSignScope.Add(CertificateSource.OCSP_ISSUER);
            crlOcspSignScope.Add(CertificateSource.SIGNER_CERT);
            ICollection<CertificateSource> ocspScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                (CertificateSource.OCSP_ISSUER));
            ICollection<CertificateSource> crlScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                (CertificateSource.CRL_ISSUER));
            ICollection<CertificateSource> timestampScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                (CertificateSource.TIMESTAMP));
            ICollection<CertificateSource> signScope = new HashSet<CertificateSource>(JavaCollectionsUtil.SingletonList
                (CertificateSource.SIGNER_CERT));
            IDictionary<String, ICollection<CertificateSource>> tempServiceTypeIdentifiersScope = new Dictionary<String
                , ICollection<CertificateSource>>(ServiceTypeIdentifiersConstants.GetAllValues().Count);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.CA_QC, crlOcspSignScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.CA_PKC, crlOcspSignScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.OCSP_QC, ocspScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.CRL_QC, crlScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TSA_QTST, timestampScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EDS_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.REM_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PSES_Q, timestampScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.QES_VALIDATION_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.REMOTE_Q_SIG_CD_MANAGEMENT_Q, signScope
                );
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.REMOTE_Q_SEAL_CD_MANAGEMENT_Q, signScope
                );
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EAA_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ELECTRONIC_ARCHIVING_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.LEDGERS_Q, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.OCSP, crlScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.CRL, crlScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TS, timestampScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TSA_TSS_QC, timestampScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TSA_TSS_ADES_Q_CAND_QES, timestampScope
                );
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PSES, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ADES_VALIDATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ADES_GENERATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.REMOTE_SIG_CD_MANAGEMENT, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.REMOTE_SEAL_CD_MANAGEMENT, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EAA, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ELECTRONIC_ARCHIVING, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.LEDGERS, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PKC_VALIDATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PKC_PRESERVATION, timestampScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EAA_VALIDATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TST_VALIDATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EDS_VALIDATION, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.EAA_PUB_EAA, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.CERTS_FOR_OTHER_TYPES_OF_TS, signScope
                );
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.RA, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.RA_NOT_HAVING_PKI_ID, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.SIGNATURE_POLICY_AUTHORITY, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ARCHIV, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ARCHIV_NOT_HAVING_PKI_ID, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.ID_V, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.K_ESCROW, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.K_ESCROW_NOT_HAVING_PKI_ID, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PP_WD, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.PP_WD_NOT_HAVING_PKI_ID, signScope);
            tempServiceTypeIdentifiersScope.Put(ServiceTypeIdentifiersConstants.TL_ISSUER, signScope);
            serviceTypeIdentifiersScope = JavaCollectionsUtil.UnmodifiableMap(tempServiceTypeIdentifiersScope);
        }

        /// <summary>
        /// Creates new instance of
        /// <see cref="LotlTrustedStore"/>.
        /// </summary>
        /// <remarks>
        /// Creates new instance of
        /// <see cref="LotlTrustedStore"/>
        /// . This constructor shall not be used directly.
        /// Instead, in order to create such instance
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder.GetLotlTrustedStore()"/>
        /// shall be used.
        /// </remarks>
        /// <param name="builder">
        /// 
        /// <see cref="iText.Signatures.Validation.ValidatorChainBuilder"/>
        /// which was responsible for creation
        /// </param>
        public LotlTrustedStore(ValidatorChainBuilder builder) {
            if (builder.IsEuropeanLotlTrusted()) {
                LotlService lotlService = builder.GetLotlService();
                if (lotlService == null || !lotlService.IsCacheInitialized()) {
                    throw new SafeCallingAvoidantException(SignExceptionMessageConstant.CACHE_NOT_INITIALIZED);
                }
                LotlValidator lotlValidator = builder.GetLotlService().GetLotlValidator();
                this.report = lotlValidator.Validate();
                if (report.GetValidationResult() == ValidationReport.ValidationResult.VALID) {
                    AddCertificatesWithContext(MapIServiceContextToCountry(lotlValidator.GetNationalTrustedCertificates()));
                }
            }
            else {
                this.report = new ValidationReport();
            }
            qualifiedValidator = builder.GetQualifiedValidator();
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

        /// <summary>Sets the certificate chain, corresponding to the certificate we are about to check.</summary>
        /// <param name="previousCertificates">
        /// list of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// certificates
        /// </param>
        /// <returns>
        /// same instance of
        /// <see cref="LotlTrustedStore"/>
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlTrustedStore SetPreviousCertificates(IList<IX509Certificate
            > previousCertificates) {
            this.previousCertificates = JavaCollectionsUtil.UnmodifiableList(previousCertificates);
            return this;
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
            CheckQualification(context, certificate, validationDate, currentContextSet);
            return CheckTrustworthiness(result, context, certificate, validationDate, currentContextSet);
        }

        private void CheckQualification(ValidationContext context, IX509Certificate certificate, DateTime validationDate
            , ICollection<CountryServiceContext> currentContextSet) {
            foreach (CountryServiceContext currentContext in currentContextSet) {
                qualifiedValidator.CheckSignatureQualification(previousCertificates, currentContext, certificate, validationDate
                    , context);
            }
        }

        private bool CheckTrustworthiness(ValidationReport result, ValidationContext context, IX509Certificate certificate
            , DateTime validationDate, ICollection<CountryServiceContext> currentContextSet) {
            IList<ReportItem> validationReportItems = new List<ReportItem>();
            foreach (CountryServiceContext currentContext in currentContextSet) {
                ServiceChronologicalInfo chronologicalInfo = GetCertificateChronologicalInfoByTime(validationReportItems, 
                    certificate, currentContext, validationDate);
                if (chronologicalInfo == null || !IsScopeCorrectlySpecified(validationReportItems, certificate, chronologicalInfo
                    .GetServiceExtensions())) {
                    continue;
                }
                ICollection<CertificateSource> currentScope = GetCertificateSourceBasedOnServiceType(currentContext.GetServiceType
                    ());
                if (currentScope == null) {
                    validationReportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format
                        (CERTIFICATE_SERVICE_TYPE_NOT_RECOGNIZED, certificate.GetSubjectDN(), currentContext.GetServiceType())
                        , ReportItem.ReportItemStatus.INFO));
                }
                else {
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
            }
            foreach (ReportItem reportItem in validationReportItems) {
                result.AddReportItem(reportItem);
            }
            return false;
        }

        /// <summary>Gets lotl validation report.</summary>
        /// <returns>validation report regarding trusted lists accessibility.</returns>
        public virtual ValidationReport GetLotlValidationReport() {
            return new ValidationReport(report);
        }

        /// <summary>
        /// Gets set of
        /// <see cref="iText.Signatures.Validation.Context.CertificateSource"/>
        /// items based on service type identifier of a given certificate in LOTL file.
        /// </summary>
        /// <remarks>
        /// Gets set of
        /// <see cref="iText.Signatures.Validation.Context.CertificateSource"/>
        /// items based on service type identifier of a given certificate in LOTL file.
        /// <para />
        /// Certificate source defines in which context this certificate is supposed to be trusted.
        /// </remarks>
        /// <param name="serviceType">
        /// 
        /// <see cref="System.String"/>
        /// representing service type identifier field in LOTL file.
        /// </param>
        /// <returns>
        /// set of
        /// <see cref="iText.Signatures.Validation.Context.CertificateSource"/>
        /// representing contexts, in which certificate is supposed to be trusted.
        /// </returns>
        protected internal virtual ICollection<CertificateSource> GetCertificateSourceBasedOnServiceType(String serviceType
            ) {
            return serviceTypeIdentifiersScope.Get(serviceType);
        }

        /// <summary>Checks if scope specified by extensions contains valid types.</summary>
        /// <param name="reportItems">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which is populated with detailed validation results
        /// </param>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="extensions">
        /// 
        /// <see cref="AdditionalServiceInformationExtension"/>
        /// that specify scope
        /// </param>
        /// <returns>false if extensions specify scope only with invalid types.</returns>
        protected internal virtual bool IsScopeCorrectlySpecified(IList<ReportItem> reportItems, IX509Certificate 
            certificate, IList<AdditionalServiceInformationExtension> extensions) {
            IList<ReportItem> currentReportItems = new List<ReportItem>();
            foreach (AdditionalServiceInformationExtension extension in extensions) {
                if (extension.IsScopeValid()) {
                    return true;
                }
                else {
                    currentReportItems.Add(new CertificateReportItem(certificate, EXTENSIONS_CHECK, MessageFormatUtil.Format(SCOPE_SPECIFIED_WITH_INVALID_TYPES
                        , certificate.GetSubjectDN(), extension.GetUri()), ReportItem.ReportItemStatus.INVALID));
                }
            }
            if (currentReportItems.IsEmpty()) {
                return true;
            }
            else {
                reportItems.AddAll(currentReportItems);
                return false;
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static IList<CountryServiceContext> MapIServiceContextToCountry(IList<IServiceContext> serviceContexts
            ) {
            IList<CountryServiceContext> list = new List<CountryServiceContext>();
            foreach (IServiceContext serviceContext in serviceContexts) {
                CountryServiceContext countryServiceContext = serviceContext is CountryServiceContext ? (CountryServiceContext
                    )serviceContext : null;
                if (countryServiceContext != null) {
                    list.Add(countryServiceContext);
                }
            }
            return list;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal void AddCertificatesWithContext(ICollection<CountryServiceContext> contexts) {
            this.contexts.AddAll(contexts);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Find
        /// <see cref="ServiceChronologicalInfo"/>
        /// corresponding to provided date.
        /// </summary>
        /// <remarks>
        /// Find
        /// <see cref="ServiceChronologicalInfo"/>
        /// corresponding to provided date. If Service wasn't operating at that date
        /// report item will be added and null will be returned.
        /// </remarks>
        /// <param name="reportItems">
        /// 
        /// <see cref="iText.Signatures.Validation.Report.ValidationReport"/>
        /// which is populated with detailed validation results
        /// </param>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be validated
        /// </param>
        /// <param name="currentContext">
        /// 
        /// <see cref="CountryServiceContext"/>
        /// which contains statuses and their starting time
        /// </param>
        /// <param name="validationDate">
        /// 
        /// <see cref="System.DateTime"/>
        /// against which certificate is expected to be validated. Usually signing
        /// date
        /// </param>
        /// <returns>
        /// 
        /// <see cref="ServiceChronologicalInfo"/>
        /// which contains time specific service information.
        /// </returns>
        internal virtual ServiceChronologicalInfo GetCertificateChronologicalInfoByTime(IList<ReportItem> reportItems
            , IX509Certificate certificate, CountryServiceContext currentContext, DateTime validationDate) {
            ServiceChronologicalInfo status = currentContext.GetServiceChronologicalInfoByDate(DateTimeUtil.GetRelativeTime
                (validationDate));
            if (status == null) {
                reportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(NOT_YET_VALID_CERTIFICATE
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
                return null;
            }
            if (!ServiceChronologicalInfo.IsStatusValid(status.GetServiceStatus())) {
                reportItems.Add(new CertificateReportItem(certificate, CERTIFICATE_CHECK, MessageFormatUtil.Format(REVOKED_CERTIFICATE
                    , certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
                return null;
            }
            return status;
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
    }
}
