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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OCSPValidatorIntegrationTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/OCSPValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private ValidatorChainBuilder validatorChainBuilder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            String rootCertFileName = SOURCE_FOLDER + "rootCert.pem";
            String checkCertFileName = SOURCE_FOLDER + "signCert.pem";
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCert.pem";
            caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)[0];
            ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            certificateRetriever = new IssuingCertificateRetriever();
            parameters = new SignatureValidationProperties();
            validatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties(parameters).WithIssuingCertificateRetrieverFactory
                (() => certificateRetriever);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateResponderOcspNoCheckTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = ValidateTest(checkDate);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasNumberOfLogs(2).HasLogItem((al
                ) => al.WithCheckName(RevocationDataValidator.REVOCATION_DATA_CHECK).WithMessage(RevocationDataValidator
                .TRUSTED_OCSP_RESPONDER)).HasLogItem((al) => al.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK
                ).WithMessage(CertificateChainValidator.CERTIFICATE_TRUSTED, (l) => ((CertificateReportItem)l).GetCertificate
                ().GetSubjectDN())).HasStatus(ValidationReport.ValidationResult.VALID));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspTest() {
            ValidationReport report = VerifyResponderWithOcsp(false);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasNumberOfLogs(2).HasLogItems(2
                , (al) => al.WithCheckName(CertificateChainValidator.CERTIFICATE_CHECK).WithMessage(CertificateChainValidator
                .CERTIFICATE_TRUSTED, (l) => ((CertificateReportItem)l).GetCertificate().GetSubjectDN())).HasStatus(ValidationReport.ValidationResult
                .VALID));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderWithOcspRevokedTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            certificateRetriever.AddKnownCertificates(JavaCollectionsUtil.Singleton(responderCert));
            ValidationReport report = VerifyResponderWithOcsp(true);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(1).HasNumberOfLogs(1).HasLogItem((al
                ) => al.WithCheckName(OCSPValidator.OCSP_CHECK).WithMessage(OCSPValidator.CERT_IS_REVOKED).WithStatus(
                ReportItem.ReportItemStatus.INDETERMINATE)));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateAuthorizedOCSPResponderFromTheTrustedStoreTest() {
            ValidationReport report = ValidateOcspWithoutCertsTest();
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void TrustedOcspResponderDoesNotHaveOcspSigningExtensionTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse caBasicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient
                .GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            // Configure OCSP signing authority for the certificate in question
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, caBasicOCSPResp.GetResponses()[0], caBasicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasStatus(ValidationReport.ValidationResult
                .VALID));
        }

        [NUnit.Framework.Test]
        public virtual void AuthorizedOcspResponderDoesNotHaveOcspSigningExtensionTest() {
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertWithoutOcspSigning.pem";
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(TimeTestUtil.TEST_DATE_TIME.AddDays(1)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(1).HasLogItem((al) => al.WithCheckName
                (CertificateChainValidator.EXTENSIONS_CHECK).WithMessage(CertificateChainValidator.EXTENSION_MISSING, 
                (l) => OID.X509Extensions.EXTENDED_KEY_USAGE)).HasStatus(ValidationReport.ValidationResult.INDETERMINATE
                ));
        }

        private ValidationReport ValidateTest(DateTime checkDate) {
            DateTime thisUpdate = checkDate.AddDays(1);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(thisUpdate));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (0));
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                , checkDate);
            return report;
        }

        private ValidationReport ValidateOcspWithoutCertsTest() {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetOcspCertsChain(new IX509Certificate[0]);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaUtil.ArraysAsList(caCert, responderCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, TimeTestUtil
                .TEST_DATE_TIME, TimeTestUtil.TEST_DATE_TIME);
            return report;
        }

        private ValidationReport VerifyResponderWithOcsp(bool revokedOcsp) {
            String rootCertFileName = SOURCE_FOLDER + "rootCertForOcspTest.pem";
            String checkCertFileName = SOURCE_FOLDER + "signCertForOcspTest.pem";
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCertForOcspTest.pem";
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            IX509Certificate responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)
                [0];
            IPrivateKey ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(-5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            TestOcspResponseBuilder builder2 = revokedOcsp ? new TestOcspResponseBuilder(caCert, caPrivateKey, FACTORY
                .CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME.AddDays(-5), FACTORY.CreateCRLReason().GetKeyCompromise
                ())) : new TestOcspResponseBuilder(caCert, caPrivateKey);
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(20)));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(30)));
            TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(5));
            if (revokedOcsp) {
                parameters.SetContinueAfterFailure(ValidatorContexts.All(), CertificateSources.All(), false);
            }
            RevocationDataValidator revocationDataValidator = validatorChainBuilder.GetRevocationDataValidator();
            revocationDataValidator.AddOcspClient(ocspClient);
            revocationDataValidator.AddOcspClient(ocspClient2);
            validatorChainBuilder.WithRevocationDataValidatorFactory(() => revocationDataValidator);
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                , checkDate);
            return report;
        }
    }
}
