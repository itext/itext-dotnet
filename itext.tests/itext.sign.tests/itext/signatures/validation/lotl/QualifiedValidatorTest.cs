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
    public class QualifiedValidatorTest : ExtendedITextTest {
        private static readonly String CERTS = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/QualifiedValidatorTest/test_certificates/";

        private static readonly String SOURCE_FOLDER_LOTL_FILES = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/lotl/QualifiedValidatorTest/test_lotl_snapshot/";

        private static readonly ValidationContext SIGN_CONTEXT = new ValidationContext(ValidatorContext.CERTIFICATE_CHAIN_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private static readonly Func<EuropeanTrustedListConfigurationFactory> FACTORY = EuropeanTrustedListConfigurationFactory
            .GetFactory();

        private static readonly DateTime PRESENT_DATE = DateTimeUtil.CreateUtcDateTime(2025, 9, 26, 2, 0, 56);

        private static readonly DateTime PRE_EIDAS_DATE = DateTimeUtil.CreateUtcDateTime(2014, 2, 3, 15, 0, 0);

        private static readonly DateTime PRE_EIDAS_DATE2 = DateTimeUtil.CreateUtcDateTime(2015, 5, 6, 15, 0, 0);

        private const String LOTL_CERT = "-----BEGIN CERTIFICATE-----\n" + "MIIDPDCCAiSgAwIBAgIBATANBgkqhkiG9w0BAQ0FADBQMRQwEgYDVQQDDAtDRVJU\n"
             + "LUxPVEwtMzEYMBYGA1UECgwPRVUgT3JnYW5pemF0aW9uMREwDwYDVQQLDAhQS0kt\n" + "VEVTVDELMAkGA1UEBhMCTFUwHhcNMjQxMDI1MjMwMDAzWhcNMjYxMDI2MDAwMDAz\n"
             + "WjBQMRQwEgYDVQQDDAtDRVJULUxPVEwtMzEYMBYGA1UECgwPRVUgT3JnYW5pemF0\n" + "aW9uMREwDwYDVQQLDAhQS0ktVEVTVDELMAkGA1UEBhMCTFUwggEiMA0GCSqGSIb3\n"
             + "DQEBAQUAA4IBDwAwggEKAoIBAQDeU/iKtAqrfGrHB1N6gFh+d56+W46IxUFEWiS+\n" + "Q+zER1/6hZEKVk0IWhCw2yS5p43Z5h9H3LSMfexTLqSbwhve5+accma+Q6It0vg3\n"
             + "rrBGnMPGOqta7Zc5zZ3kv83jJCQ8EU6FnCp7OqQY2ymiqgIWHwbDWooNUsYnu+wv\n" + "bcYx/AYweMZLdWSogt3iu5Sh1zNhubU4tasn/A5x0pDV97BSGIvs5mmqIndF8uDc\n"
             + "mmxmjn105LGEQqwT6GN1r99kwd2UZewbVztlbvDoI6eTDkZ1ffomDHnNjEIBhcgG\n" + "TlI3zpRmIVcj6Vckh8zGmewTt6FJhGlIb83iqB9ah8ki03NzAgMBAAGjITAfMB0G\n"
             + "A1UdDgQWBBRJ79BepQX9cyVvwvG/Xp1yxwYvnTANBgkqhkiG9w0BAQ0FAAOCAQEA\n" + "PQJNKkMNUGO5gM/CC6D7e4EBvkCBwgjtIhAFoXEzmqij/0Da+dNY1xk6hPMR8jd3\n"
             + "YFpwsBP3h72hSoq8wZhJ3erP0uIo4qmOPDeJsmkpRsKqDFmTg04bE3bGV1pBI06o\n" + "AqwQr5JAoQAIrMFDobxXsTXC1abUKO9BId72rUy5Mxv227aVNx8nWcZoKeg37FVk\n"
             + "bLgd+mjfh8LzxM02i3WIM+Z2wdq/h8SVlupPPkrJr2edBv/CzCf1VFa8L7tDMpxP\n" + "9HdHBJz+nUfTe5mXzqHS0MxogW5sBUk8Rj9KCvNO5wdPZhfg8nGrEnGWXj8gl9Km\n"
             + "MwsoJseoWfQ6GjmQCv0kpQ==\n" + "-----END CERTIFICATE-----";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeAll() {
            // Initialize the LotlService with a default EuropeanResourceFetcher
            LotlService service = new LotlService(new LotlFetchingProperties(new ThrowExceptionOnFailingCountryData())
                );
            service.WithCustomResourceRetriever(new FromDiskResourceRetriever(SOURCE_FOLDER_LOTL_FILES));
            EuropeanTrustedListConfigurationFactory.SetFactory(() => new _EuropeanTrustedListConfigurationFactory_92()
                );
            service.WithLotlValidator(() => new LotlValidator(service));
            LotlService.GLOBAL_SERVICE = service;
            service.InitializeCache();
        }

        private sealed class _EuropeanTrustedListConfigurationFactory_92 : EuropeanTrustedListConfigurationFactory {
            public _EuropeanTrustedListConfigurationFactory_92() {
            }

            public override String GetTrustedListUri() {
                return "https://eidas.ec.europa.eu/efda/api/v2/validation-tests/testcase/tl/LOTL-3.xml";
            }

            public override String GetCurrentlySupportedPublication() {
                return "https://eur-lex.europa.eu/legal-content/EN/TXT/?uri=uriserv:OJ.C_.2019.276.01.0001.01.ENG";
            }

            public override IList<IX509Certificate> GetCertificates() {
                IX509Certificate certificate = CertificateUtil.ReadCertificatesFromPem(new MemoryStream(QualifiedValidatorTest
                    .LOTL_CERT.GetBytes(System.Text.Encoding.UTF8)))[0];
                return JavaCollectionsUtil.SingletonList(certificate);
            }
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterAll() {
            EuropeanTrustedListConfigurationFactory.SetFactory(FACTORY);
            LotlService.GLOBAL_SERVICE.Close();
            LotlService.GLOBAL_SERVICE = null;
        }

        //3. Matching SDI + Sti/aSI + status
        //3.1 matching service
        [NUnit.Framework.Test]
        public virtual void TrustedCertificateNotPresentInTLTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQualifiedServiceTypeTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NoMatchingServiceInformationTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void ServiceWithdrawnTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void SupervisionCeasedServiceStatusBeforeEIDASTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void AccreditationRevokedServiceStatusBeforeEIDASTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.1.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        // 3.2 matching service
        [NUnit.Framework.Test]
        public virtual void StandardCaseESigTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.2.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NoMatchingTSPNameTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.2.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        //3.3.Incoherences in TL
        [NUnit.Framework.Test]
        public virtual void SeveralTLEntriesWithSameResultTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_3.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        //4. QC / notQC
        //4.1.notQC based on sigCert content
        [NUnit.Framework.Test]
        public virtual void SigningCertificateNotDeclaredQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyBeforeEidasQcpPlusTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyBeforeEidasQcpAndQscdTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyBeforeEidasQcpTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyAfterEidasQcpTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            // Sat Jun 06 15:00:00 MSK 2020
            DateTime dateTime = DateTimeUtil.CreateUtcDateTime(2020, 5, 6, 15, 0, 0);
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, dateTime);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyAfterEidasQcpAndQscdTest() {
            // This certificate policy extension contains weird id, which is not recognizable.
            // The expected result is correct, but test name is somewhat confusing.
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            // Sat Jun 06 15:00:00 MSK 2020
            DateTime dateTime = DateTimeUtil.CreateUtcDateTime(2020, 5, 6, 15, 0, 0);
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, dateTime);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyBeforeEidasQcpAndQcpPlusTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.1.7.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        // 4.2.Overrule to notQC by Sie:Q in TL
        [NUnit.Framework.Test]
        public virtual void NotQualifiedOverruleInTLCatchingTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.2.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESEAL, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQualifiedOverruleInTLNotCatchingTypeTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.2.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQualifiedOverruleInTLNotCatchingCriteriaTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.2.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        // 4.3.Overrule to QC by Sie:Q in TL
        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingWithoutTypeTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingWithSIESigAndESealTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingWithAdditionalQcESigTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLNotCatchingTypeTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            // The expected result from their side is "AdESeal". We get NOT_APPLICABLE,
            // because when no CA/QC catches signing cert, we don't say any information about the type.
            // Also, it's kind of a bad test from their side, not because of this,
            // but because it's not only not catching the signing cert because of the SI type, but also because of the criteria.
            // So it will fail even when type catching is ignored.
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLNotCatchingCriteriaTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.3.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESEAL, conclusion);
        }

        // 4.4.Overrule to QC with complex catching logic
        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest1() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest2() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest3() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest4() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest5() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest6() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest7() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.7.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest8() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.8.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest9() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.9.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest10() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.10.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest11() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.11.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest12() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.12.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest13() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.13.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest14() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.14.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest15() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.15.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest16() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.16.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest17() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.17.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest18() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.18.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest19() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.19.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest20() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.20.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QualifiedOverruleInTLCatchingComplexPolicyTest21() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.4.21.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        //4.5.Incoherences in TL
        [NUnit.Framework.Test]
        public virtual void NotQualifiedAndQualifiedInTlTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.5.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQualifiedAndQualifiedInSITest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.5.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQualifiedAndQualifiedInTwoElementsTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.5.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyBeforeEidasStatusSupervisionTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_4.5.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        // 5. Type
        // 5.1.No overrule. Based on sigCert content
        [NUnit.Framework.Test]
        public virtual void StandardCaseEsealTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESEAL_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void WsaTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleSIExtesnionsTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NoTypeInSigCertTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTypesInSigCertQCTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTypesInSigCertNotQCTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.1.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN, conclusion);
        }

        // 5.2.Overrule of type by Sie:Q in TL
        [NUnit.Framework.Test]
        public virtual void StandardOverruleQcTypeIgnoredTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.2.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void TypeOverruleCaughtButNotAppliedBcsNotQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.2.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESEAL, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void TypeOverruleCaughtButNotAppliedBcsOverruledToNotQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.2.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void TypeOverruleCaughtBcsOverruledToQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.2.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        // 5.3.Incoherences in TL
        [NUnit.Framework.Test]
        public virtual void QcForXXNotAllignedWithSITest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void TwoOverruledTypesTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.3.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            // The expected result here is "N/A". No idea why they have N/A here. Seems to be UNKNOWN_QC_AND_QSCD.
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.UNKNOWN_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void TwoConflictingTLValueWithOneOverruleTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_5.3.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_APPLICABLE, conclusion);
        }

        // 6. QSCD / no QSCD
        // 6.1.Certificate policies in sigCert
        [NUnit.Framework.Test]
        public virtual void CertPolicyQcpBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyQcpPlusBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void CertPolicyAfterEidasQCPAndQSCDTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.1.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        // 6.2.Overrule to QSCD by Sie:Q in TL
        [NUnit.Framework.Test]
        public virtual void OverruleToQscdNotCatchingTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.2.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void OverruleToQscdNotApplyingBcsNotQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.2.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void OverruleToQscdNotApplyingBcsOverruleToNotQcTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.2.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void SscdBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.2.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QscdAfterEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.2.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        //6.3.Overrule to no QSCD by Sie:Q in TL
        [NUnit.Framework.Test]
        public virtual void OverruleToNotQscdNotCatchingTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NoQscdBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.3.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NoQscdAfterEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.3.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QscdManagedOnBehalfTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.3.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        //6.4.Incoherences in TL
        [NUnit.Framework.Test]
        public virtual void QscdAndNoQscdTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void SscdAndNoSscdTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QscdStatusAsInCertAndQscdTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.3.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void SscdStatusAsInCertAndSscdTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.4.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QscdBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.5.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void SscdAfterEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.6.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQscdBeforeEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.7.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRE_EIDAS_DATE2);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotSscdAfterEidasTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_6.4.8.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        // 7. Discrepancy betw. time of issuance & time of signing
        // 7.1.QC / notQC
        // In this paragraph our results are different from those posted as expected in the original test suite.
        // Our understanding is that there is something wrong with the test suite.
        [NUnit.Framework.Test]
        public virtual void QcAtIssuingNotQcAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.1.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.NOT_QUALIFIED_ESIG, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void NotQcAtIssuingQcAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.1.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        //7.2.Type
        [NUnit.Framework.Test]
        public virtual void ESigAtIssuingESealAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.2.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESEAL_WITH_QC_AND_QSCD, conclusion);
        }

        //7.3.QSCD / no QSCD
        [NUnit.Framework.Test]
        public virtual void NoQscdAtIssuingQscdAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.3.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }

        [NUnit.Framework.Test]
        public virtual void QscdAtIssuingNoQscdAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.3.2.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC, conclusion);
        }

        //7.4.Before / after eIDAS
        [NUnit.Framework.Test]
        public virtual void BeforeEidasAtIssuingAfterEidasAtSigningTest() {
            ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
            chainBuilder.TrustEuropeanLotl(true);
            QualifiedValidator qualifiedValidator = new QualifiedValidator();
            qualifiedValidator.StartSignatureValidation("signature1");
            chainBuilder.WithQualifiedValidator(qualifiedValidator);
            IX509Certificate[] certChain = PemFileHelper.ReadFirstChain(CERTS + "certificate_7.4.1.pem");
            IX509Certificate signCertificate = (IX509Certificate)certChain[0];
            IX509Certificate trustedCert = (IX509Certificate)certChain[certChain.Length - 1];
            LotlTrustedStore trustedStore = chainBuilder.GetLotlTrustedStore();
            trustedStore.SetPreviousCertificates(JavaCollectionsUtil.SingletonList(signCertificate)).CheckIfCertIsTrusted
                (new ValidationReport(), SIGN_CONTEXT, trustedCert, PRESENT_DATE);
            QualificationConclusion? conclusion = qualifiedValidator.ObtainQualificationValidationResultForSignature("signature1"
                ).GetQualificationConclusion();
            NUnit.Framework.Assert.AreEqual(QualificationConclusion.ESIG_WITH_QC_AND_QSCD, conclusion);
        }
    }
}
