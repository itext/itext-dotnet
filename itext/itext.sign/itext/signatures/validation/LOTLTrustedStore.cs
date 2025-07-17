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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    /// <summary>Trusted certificates storage class for LOTL trusted certificates.</summary>
    internal class LOTLTrustedStore {
        private readonly ICollection<CountryServiceContext> contexts = new HashSet<CountryServiceContext>();

        private static readonly IDictionary<String, ICollection<CertificateSource>> serviceTypeIdentifiersScope = 
            new Dictionary<String, ICollection<CertificateSource>>();

//\cond DO_NOT_DOCUMENT
        internal const String REVOKED_CERTIFICATE = "Certificate {0} is revoked.";
//\endcond

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
            serviceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/CA/QC", crlOcspSignScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/CA/PKC/", crlOcspSignScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP/QC/", ocspScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL/QC/", crlScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TSA/QTST/", timestampScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EDS/Q/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EDS/REM/Q/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PSES/Q/", timestampScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/QESValidation/Q/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/RemoteQSigCDManagement/Q", signScope
                );
            serviceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/RemoteQSealCDManagement/Q", signScope
                );
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EAA/Q", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving/Q", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Ledgers/Q", signScope);
            serviceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/OCSP", crlScope);
            serviceTypeIdentifiersScope.Put("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL", crlScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TSA/", timestampScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-QC/", timestampScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TSA/TSS-AdESQCandQES/", timestampScope
                );
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PSES/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/AdESValidation/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/AdESGeneration/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/RemoteSigCDManagemen", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/RemoteSealCDManagement", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EAA", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/ElectronicArchiving", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Ledgers", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PKCValidation", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PKCPreservation", timestampScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EAAValidation", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TSTValidation", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EDSValidation", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/EAA/Pub-EAA", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PKCValidation/CertsforOtherTypesOfTS"
                , signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/RA/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/RA/nothavingPKIid/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/SignaturePolicyAuthority/", signScope
                );
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Archiv/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/Archiv/nothavingPKIid/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/IdV/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/KEscrow/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/KEscrow/nothavingPKIid", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PPwd/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/PPwd/nothavingPKIid/", signScope);
            serviceTypeIdentifiersScope.Put("https://uri.etsi.org/TrstSvc/Svctype/TLIssuer/", signScope);
        }

//\cond DO_NOT_DOCUMENT
        internal LOTLTrustedStore() {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        //empty constructor
        internal virtual ICollection<IX509Certificate> GetCertificates() {
            ICollection<IX509Certificate> allCertificates = new HashSet<IX509Certificate>();
            foreach (CountryServiceContext context in contexts) {
                allCertificates.AddAll(context.GetCertificates());
            }
            return allCertificates;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddCertificatesWithContext(ICollection<CountryServiceContext> contexts) {
            this.contexts.AddAll(contexts);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool CheckIfCertIsTrusted(ValidationReport result, ValidationContext context, IX509Certificate
             certificate, DateTime validationDate) {
            ICollection<CountryServiceContext> currentContextSet = GetCertificateContext(certificate);
            if (currentContextSet.IsEmpty()) {
                return false;
            }
            IList<ReportItem> validationReportItems = new List<ReportItem>();
            foreach (CountryServiceContext currentContext in currentContextSet) {
                if (!IsCertificateValidInTime(validationReportItems, certificate, currentContext, validationDate)) {
                    continue;
                }
                ICollection<CertificateSource> currentScope = serviceTypeIdentifiersScope.Get(currentContext.GetServiceType
                    ());
                foreach (CertificateSource source in currentScope) {
                    if (ValidationContext.CheckIfContextChainContainsCertificateSource(context, source)) {
                        result.AddReportItem(new CertificateReportItem(certificate, CertificateChainValidator.CERTIFICATE_CHECK, MessageFormatUtil
                            .Format(CertificateChainValidator.CERTIFICATE_TRUSTED, certificate.GetSubjectDN()), ReportItem.ReportItemStatus
                            .INFO));
                        return true;
                    }
                    else {
                        validationReportItems.Add(new CertificateReportItem(certificate, CertificateChainValidator.CERTIFICATE_CHECK
                            , MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, certificate
                            .GetSubjectDN(), currentContext.GetServiceType()), ReportItem.ReportItemStatus.INFO));
                    }
                }
            }
            foreach (ReportItem reportItem in validationReportItems) {
                result.AddReportItem(reportItem);
            }
            return false;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsCertificateValidInTime(IList<ReportItem> reportItems, IX509Certificate certificate
            , CountryServiceContext currentContext, DateTime validationDate) {
            String status = currentContext.GetServiceStatusByDate(DateTimeUtil.GetRelativeTime(validationDate));
            if (status == null) {
                reportItems.Add(new CertificateReportItem(certificate, CertificateChainValidator.VALIDITY_CHECK, MessageFormatUtil
                    .Format(CertificateChainValidator.NOT_YET_VALID_CERTIFICATE, certificate.GetSubjectDN()), ReportItem.ReportItemStatus
                    .INVALID));
                return false;
            }
            if (!ServiceStatusInfo.IsStatusValid(status)) {
                reportItems.Add(new CertificateReportItem(certificate, CertificateChainValidator.VALIDITY_CHECK, MessageFormatUtil
                    .Format(REVOKED_CERTIFICATE, certificate.GetSubjectDN()), ReportItem.ReportItemStatus.INVALID));
                return false;
            }
            return true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<CountryServiceContext> GetCertificateContext(IX509Certificate certificate) {
            ICollection<CountryServiceContext> contextSet = new HashSet<CountryServiceContext>();
            foreach (CountryServiceContext context in contexts) {
                if (context.GetCertificates().Contains(certificate)) {
                    contextSet.Add(context);
                }
            }
            return contextSet;
        }
//\endcond
    }
//\endcond
}
