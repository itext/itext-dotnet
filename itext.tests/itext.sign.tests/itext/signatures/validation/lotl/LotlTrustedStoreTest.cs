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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    internal class LotlTrustedStoreTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        private static IX509Certificate crlRootCert;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
            crlRootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(SOURCE_FOLDER + "crlRoot.pem")[0];
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert.GetSubjectDN
                ()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateWithValidationContextChainTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/Certstatus/CRL");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            ValidationContext previousValidationContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT);
            ValidationContext validationContext = previousValidationContext.SetCertificateSource(CertificateSource.OCSP_ISSUER
                );
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, validationContext, crlRootCert, TimeTestUtil
                .TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert.GetSubjectDN
                ()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectContextTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/TSA/QTST");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                (LotlTrustedStore.CERTIFICATE_TRUSTED_FOR_DIFFERENT_CONTEXT, (l) => crlRootCert.GetSubjectDN(), (l) =>
                 "http://uri.etsi.org/TrstSvc/Svctype/TSA/QTST").WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void ServiceTypeNotRecognizedTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("invalid service type");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                (LotlTrustedStore.CERTIFICATE_SERVICE_TYPE_NOT_RECOGNIZED, (l) => crlRootCert.GetSubjectDN(), (l) => "invalid service type"
                ).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectTimeBeforeValidTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (2025, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is not yet valid.", (l) => crlRootCert.GetSubjectDN()).WithCertificate(crlRootCert))
                );
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectTimeAfterValidTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo("http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/withdrawn"
                , iText.Commons.Utils.DateTimeUtil.CreateDateTime(1900, 1, 1, 0, 0)));
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.CRL_ISSUER, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is revoked.", (l) => crlRootCert.GetSubjectDN()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateWithCorrectExtensionScopeTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            info.AddServiceExtension(new AdditionalServiceInformationExtension("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForeSignatures"
                ));
            context.AddServiceChronologicalInfo(info);
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert.GetSubjectDN
                ()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateWithCorrectExtensionScope2Test() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            info.AddServiceExtension(new AdditionalServiceInformationExtension("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForeSignatures"
                ));
            info.AddServiceExtension(new AdditionalServiceInformationExtension("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication"
                ));
            context.AddServiceChronologicalInfo(info);
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsTrue(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.CERTIFICATE_CHECK).WithMessage
                ("Certificate {0} is trusted, revocation data checks are not required.", (l) => crlRootCert.GetSubjectDN
                ()).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CheckCertificateWithIncorrectExtensionScopeTest() {
            LotlTrustedStore store = new ValidatorChainBuilder().GetLotlTrustedStore();
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(crlRootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            info.AddServiceExtension(new AdditionalServiceInformationExtension("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication"
                ));
            context.AddServiceChronologicalInfo(info);
            store.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            ValidationReport report = new ValidationReport();
            NUnit.Framework.Assert.IsFalse(store.CheckIfCertIsTrusted(report, new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
                , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT), crlRootCert, TimeTestUtil.TEST_DATE_TIME));
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (1).HasNumberOfLogs(1).HasLogItem((la) => la.WithCheckName(LotlTrustedStore.EXTENSIONS_CHECK).WithMessage
                (LotlTrustedStore.SCOPE_SPECIFIED_WITH_INVALID_TYPES, (l) => crlRootCert.GetSubjectDN(), (k) => "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication"
                ).WithCertificate(crlRootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void CreateTrustedStoreWithValidLotl() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            LotlFetchingProperties fetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(fetchingProperties)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                chainBuilder.WithLotlService(() => lotlService);
                lotlService.InitializeCache();
            }
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            NUnit.Framework.Assert.IsFalse(trustedStore.GetCertificates().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void CreateTrustedStoreWithInvalidLotl() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            LotlFetchingProperties fetchingProperties = new LotlFetchingProperties(new RemoveOnFailingCountryData());
            using (LotlService lotlService = new LotlService(fetchingProperties)) {
                lotlService.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
                chainBuilder.WithLotlService(() => lotlService);
                lotlService.InitializeCache();
                LotlValidator invalidValidator = new _LotlValidator_341(lotlService);
                lotlService.WithLotlValidator(() => invalidValidator);
            }
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            NUnit.Framework.Assert.IsTrue(trustedStore.GetCertificates().IsEmpty());
        }

        private sealed class _LotlValidator_341 : LotlValidator {
            public _LotlValidator_341(LotlService baseArg1)
                : base(baseArg1) {
            }

            public override ValidationReport Validate() {
                ValidationReport report = new ValidationReport();
                report.AddReportItem(new ReportItem("check", "test invalid", ReportItem.ReportItemStatus.INVALID));
                return report;
            }
        }
    }
//\endcond
}
