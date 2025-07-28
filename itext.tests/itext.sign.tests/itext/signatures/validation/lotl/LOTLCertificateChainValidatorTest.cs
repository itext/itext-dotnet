using System;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Extensions;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class LOTLCertificateChainValidatorTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/LOTLCertificateChainValidatorTest/";

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        [NUnit.Framework.Test]
        public virtual void LotlTrustedStoreTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LOTLTrustedStore lotlTrustedStore = new LOTLTrustedStore(validatorChainBuilder);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList<CountryServiceContext>(context
                ));
            validatorChainBuilder.WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() =>
                 mockRevocationDataValidator).WithLOTLTrustedStoreFactory(() => lotlTrustedStore);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), rootCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(LOTLTrustedStore
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }

        [NUnit.Framework.Test]
        public virtual void LotlTrustedStoreChainTest() {
            MockRevocationDataValidator mockRevocationDataValidator = new MockRevocationDataValidator();
            SignatureValidationProperties properties = new SignatureValidationProperties();
            String chainName = CERTS_SRC + "chain.pem";
            IX509Certificate[] certificateChain = PemFileHelper.ReadFirstChain(chainName);
            IX509Certificate signingCert = (IX509Certificate)certificateChain[0];
            IX509Certificate intermediateCert = (IX509Certificate)certificateChain[1];
            IX509Certificate rootCert = (IX509Certificate)certificateChain[2];
            CountryServiceContext context = new CountryServiceContext();
            context.AddCertificate(rootCert);
            context.SetServiceType("http://uri.etsi.org/TrstSvc/Svctype/CA/QC");
            context.AddNewServiceStatus(new ServiceStatusInfo(ServiceStatusInfo.GRANTED, iText.Commons.Utils.DateTimeUtil.CreateDateTime
                (1900, 1, 1, 0, 0)));
            ValidatorChainBuilder validatorChainBuilder = new ValidatorChainBuilder();
            LOTLTrustedStore lotlTrustedStore = new LOTLTrustedStore(validatorChainBuilder);
            lotlTrustedStore.AddCertificatesWithContext(JavaCollectionsUtil.SingletonList<CountryServiceContext>(context
                ));
            validatorChainBuilder.WithKnownCertificates(JavaCollectionsUtil.SingletonList<IX509Certificate>(intermediateCert
                )).WithSignatureValidationProperties(properties).WithRevocationDataValidatorFactory(() => mockRevocationDataValidator
                ).WithLOTLTrustedStoreFactory(() => lotlTrustedStore);
            CertificateChainValidator validator = validatorChainBuilder.BuildCertificateChainValidator();
            properties.SetRequiredExtensions(CertificateSources.All(), JavaCollectionsUtil.EmptyList<CertificateExtension
                >());
            ValidationReport report1 = validator.ValidateCertificate(baseContext.SetCertificateSource(CertificateSource
                .CRL_ISSUER), signingCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report1, (a) => a.HasStatus(ValidationReport.ValidationResult.VALID).HasNumberOfFailures
                (0).HasNumberOfLogs(1).HasLogItem((l) => l.WithCheckName("Certificate check.").WithMessage(LOTLTrustedStore
                .CERTIFICATE_TRUSTED, (i) => rootCert.GetSubjectDN()).WithCertificate(rootCert)));
        }
    }
}
