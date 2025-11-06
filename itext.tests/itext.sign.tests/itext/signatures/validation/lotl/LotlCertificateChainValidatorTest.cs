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
using System.Linq;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class LotlCertificateChainValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOULDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl" + "/LotlCertificateChainValidatorTest/";

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/lotl/LotlState2025_08_08/";

        [NUnit.Framework.Test]
        public virtual void LotlTrustedStoreTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = SOURCE_FOULDER + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LotlTrustedStore lotlTrustedStore = new LotlTrustedStore(validatorChainBuilder);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            validatorChainBuilder.WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() =>
                 mockRevocationDataValidator).WithLotlTrustedStoreFactory(() => lotlTrustedStore);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), rootCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(LotlTrustedStore
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void LotlTrustedStoreChainTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = SOURCE_FOULDER + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddServiceChronologicalInfo(new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LotlTrustedStore lotlTrustedStore = new LotlTrustedStore(validatorChainBuilder);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            validatorChainBuilder.WithKnownCertificates(JavaCollectionsUtil.SingletonList(intermediateCert)).WithSignatureValidationProperties
                (properties).WithRevocationDataValidatorFactory(() => mockRevocationDataValidator).WithLotlTrustedStoreFactory
                (() => lotlTrustedStore);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), signingCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(LotlTrustedStore
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void LotlTrustedStoreExtensionTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = SOURCE_FOULDER + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            info.AddServiceExtension(new AdditionalServiceInformationExtension("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication"
                ));
            context.AddServiceChronologicalInfo(info);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LotlTrustedStore lotlTrustedStore = new LotlTrustedStore(validatorChainBuilder);
            validatorChainBuilder.WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() =>
                 mockRevocationDataValidator).WithLotlTrustedStoreFactory(() => lotlTrustedStore);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), rootCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.INVALID).HasNumberOfFailures
                (2).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName(LotlTrustedStore.EXTENSIONS_CHECK).WithMessage
                (LotlTrustedStore.SCOPE_SPECIFIED_WITH_INVALID_TYPES, (i) => rootCert.GetSubjectDN(), (k) => "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication"
                ).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void LotlOriginTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = SOURCE_FOULDER + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            context.AddServiceChronologicalInfo(info);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LotlTrustedStore lotlTrustedStore = new LotlTrustedStore(validatorChainBuilder);
            validatorChainBuilder.WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() =>
                 mockRevocationDataValidator).WithLotlTrustedStoreFactory(() => lotlTrustedStore);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), rootCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage("Trusted Certificate is taken from European Union List of Trusted Certificates."
                ).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void LotlReportItemsTest() {
            LotlService service = new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()));
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
            service.WithLotlValidator(() => new LotlValidator(service));
            service.InitializeCache();
            IssuingCertificateRetriever retriever = new IssuingCertificateRetriever();
            SignatureValidationProperties param = new SignatureValidationProperties();
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties(param).
                WithIssuingCertificateRetrieverFactory(() => retriever).WithLotlService(() => service).TrustEuropeanLotl
                (true);
            ValidationReport report;
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOULDER + "docWithMultipleSignatures.pdf"
                ))) {
                SignatureValidator signatureValidator = chainBuilder.BuildSignatureValidator(document);
                report = signatureValidator.ValidateSignatures();
            }
            //It is expected to have only 32 lotl validation messages as there 32 countries in lotl.
            long logs = report.GetLogs().Where((reportItem) => XmlSignatureValidator.XML_SIGNATURE_VERIFICATION.Equals
                (reportItem.GetCheckName())).Count();
            NUnit.Framework.Assert.IsTrue(logs <= 32);
        }

        [NUnit.Framework.Test]
        public virtual void CustomOriginTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = SOURCE_FOULDER + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            ServiceChronologicalInfo info = new ServiceChronologicalInfo(ServiceChronologicalInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0));
            context.AddServiceChronologicalInfo(info);
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LotlTrustedStore lotlTrustedStore = new LotlCertificateChainValidatorTest.CustomTrustedStore(this, validatorChainBuilder
                );
            validatorChainBuilder.WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() =>
                 mockRevocationDataValidator).WithLotlTrustedStoreFactory(() => lotlTrustedStore);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList(context));
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), rootCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(2).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage("Trusted Certificate is taken from {0}."
                , (k) => typeof(LotlCertificateChainValidatorTest.CustomTrustedStore).FullName).WithCertificate(rootCert
                )));
        }

        private class CustomTrustedStore : LotlTrustedStore {
            public CustomTrustedStore(LotlCertificateChainValidatorTest _enclosing, ValidatorChainBuilder builder)
                : base(builder) {
                this._enclosing = _enclosing;
            }

            private readonly LotlCertificateChainValidatorTest _enclosing;
        }
    }
}
