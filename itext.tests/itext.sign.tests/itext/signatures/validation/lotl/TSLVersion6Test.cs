using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Lotl {
    // This test suite is taken from https://eidas.ec.europa.eu/efda/validation-tests#/screen/home
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class TSLVersion6Test : ExtendedITextTest {
        private static readonly String CERTS = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/TSLVersion6Test/test_certificates/";

        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/TSLVersion6Test/test_lotl_snapshot/";

        private static readonly ValidationContext SIGN_CONTEXT = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private static readonly Func<EuropeanTrustedListConfigurationFactory> FACTORY = EuropeanTrustedListConfigurationFactory
            .GetFactory();

        private const String LOTL_CERT = "-----BEGIN CERTIFICATE-----\n" + "MIIDPDCCAiSgAwIBAgIBATANBgkqhkiG9w0BAQ0FADBQMRQwEgYDVQQDDAtDRVJU\n"
             + "LUxPVEwtNDEYMBYGA1UECgwPRVUgT3JnYW5pemF0aW9uMREwDwYDVQQLDAhQS0kt\n" + "VEVTVDELMAkGA1UEBhMCTFUwHhcNMjQxMjIxMDAwMDA2WhcNMjYxMjIxMDAwMDA2\n"
             + "WjBQMRQwEgYDVQQDDAtDRVJULUxPVEwtNDEYMBYGA1UECgwPRVUgT3JnYW5pemF0\n" + "aW9uMREwDwYDVQQLDAhQS0ktVEVTVDELMAkGA1UEBhMCTFUwggEiMA0GCSqGSIb3\n"
             + "DQEBAQUAA4IBDwAwggEKAoIBAQCiu8CP3OKq8DMOoJigZH8n1xssQhLtySOJ5tGS\n" + "6KOWTfDaTl3eq+4svjLzhqGNDgSYwk/khYZEoJntO3lrkDN6KsYSOBDFNpjYpxa7\n"
             + "p3EUjuJxDb1clqx27kkuMTIFl5pcs9oNuUORntyZ2SqjqqnqxkjsSpqNPl8nyZc9\n" + "gaAY13XU4D2ACsRmnpURmkFj4ppMucMjTCeqlFesvJELf06jfJcLHIJU/b2Wx8a2\n"
             + "0x3nN564anLIpBfL5Ws40ScRywp9tve2M77DXWXhXKSAaE5D7Fnb7NRb/pPbW5sd\n" + "fjhDoO7EghXsIDKgJPtdJlThikTkGTk59t6IODu9gnZ5x7uvAgMBAAGjITAfMB0G\n"
             + "A1UdDgQWBBROJGXazRDpyPzpJpUN2EBep0AzazANBgkqhkiG9w0BAQ0FAAOCAQEA\n" + "nOzoRYzYEhFdNqXwA89CaHTTEnfubTXqv+fx5t5SpXxr+TEYt4Vrhxepk+nHTfR9\n"
             + "zcwsECYRaZ8c436F7Gk/Fva99njCSJKvh8Awtbmi1+qv8hdkaaNTs2mH+6zR0iva\n" + "anq+G3ozGXQ20L6/HdYXrmrBl6i4JjZ2jbbc5whPx/urgAlngB9oD34YfaedLVUf\n"
             + "Qr8y9OCwFNh0zVvLwYbFyPFdvmt9bgxvxmAH+pD/k8Sxe8HkMbqVqMo2/PgHlEaW\n" + "7CoKoEnAsgGlvmTA4fA2rlUNIUWzZNCr6ee6pdfM6+wZS4v4301L1JezYzyJxJHN\n"
             + "/Ols4IYrBvATPiZl2kxVPQ==\n" + "-----END CERTIFICATE-----";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeAll() {
            // Initialize the LotlService with a default EuropeanResourceFetcher
            LotlService service = new LotlService(new LotlFetchingProperties(new ThrowExceptionOnFailingCountryData())
                );
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
            EuropeanTrustedListConfigurationFactory.SetFactory(() => new _EuropeanTrustedListConfigurationFactory_65()
                );
            service.WithLotlValidator(() => new LotlValidator(service));
            LotlService.GLOBAL_SERVICE = service;
            service.InitializeCache();
        }

        private sealed class _EuropeanTrustedListConfigurationFactory_65 : EuropeanTrustedListConfigurationFactory {
            public _EuropeanTrustedListConfigurationFactory_65() {
            }

            public override String GetTrustedListUri() {
                return "https://eidas.ec.europa.eu/efda/api/v2/validation-tests/testcase/tl/LOTL-4.xml";
            }

            public override String GetCurrentlySupportedPublication() {
                return "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.2019.276.01.0001.01.ENG";
            }

            public override IList<IX509Certificate> GetCertificates() {
                IX509Certificate certificate = CertificateUtil.ReadCertificatesFromPem(new MemoryStream(TSLVersion6Test.LOTL_CERT
                    .GetBytes(System.Text.Encoding.UTF8)))[0];
                return JavaCollectionsUtil.SingletonList(certificate);
            }
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterAll() {
            EuropeanTrustedListConfigurationFactory.SetFactory(FACTORY);
            LotlService.GLOBAL_SERVICE.Close();
            LotlService.GLOBAL_SERVICE = null;
        }

        [NUnit.Framework.Test]
        public virtual void DefaultCaseQESigTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_8.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, DateTimeUtil.GetCurrentUtcTime());
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultCaseAdESigTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_8.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, DateTimeUtil.GetCurrentUtcTime());
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        // Example "2.TSL v6 - Wrong signature format" from a test suite is missing,
        // because wrong signature format results in exception during LOTL cache initialization, which is expected.
        // But because of this we have an exception much earlier than needed for a test.
        [NUnit.Framework.Test]
        public virtual void ServiceWithTLIssuerAndServiceWithQcCaTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_8.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, DateTimeUtil.GetCurrentUtcTime());
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }
    }
}
