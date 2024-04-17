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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Signatures.Validation.V1.Context;
using iText.Signatures.Validation.V1.Report;
using iText.Test;

namespace iText.Signatures.Validation.V1 {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class RevocationDataValidatorTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/v1/RevocationDataValidatorTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private const long MILLISECONDS_PER_DAY = 86_400_000L;

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private ValidationContext baseContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR, CertificateSource
            .SIGNER_CERT, TimeBasedContext.PRESENT);

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
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetriever(certificateRetriever).
                WithSignatureValidationProperties(parameters);
        }

        [NUnit.Framework.Test]
        public virtual void BasicValidationWithOcspClientTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetProducedAt(checkDate.AddDays(5));
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(ocspClient);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.TRUSTED_OCSP_RESPONDER, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void BasicValidationWithCrlClientTest() {
            // TODO what is being tested here?
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = checkDate.AddDays(-1);
            TestCrlBuilder builder = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder.SetNextUpdate(checkDate.AddDays(10));
            builder.AddCrlEntry(checkCert, revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(builder);
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (0));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                );
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(3, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[2]);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(caCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual("Using crl nextUpdate date as validation date", item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(caCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[2];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.CERTIFICATE_REVOKED, caCert.GetSubjectDN
                (), revocationDate), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void UseFreshCrlResponseTest() {
            // Add client with indeterminate CRL, then with CRL which contains revoked checkCert.
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            DateTime revocationDate = checkDate.AddDays(-1);
            TestCrlBuilder builder1 = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            builder1.SetNextUpdate(checkDate.AddDays(2));
            builder1.AddCrlEntry(checkCert, revocationDate, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlClient crlClient1 = new TestCrlClient().AddBuilderForCertIssuer(builder1);
            DateTime thisUpdate2 = checkDate.AddDays(-2);
            TestCrlBuilder builder2 = new TestCrlBuilder(caCert, caPrivateKey, thisUpdate2);
            builder2.SetNextUpdate(checkDate);
            TestCrlClient crlClient2 = new TestCrlClient().AddBuilderForCertIssuer(builder2);
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (0));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient1
                ).AddCrlClient(crlClient2);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(3, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(report.GetFailures()[0], report.GetLogs()[2]);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(caCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual("Using crl nextUpdate date as validation date", item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(caCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[2];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.CERTIFICATE_REVOKED, caCert.GetSubjectDN
                (), revocationDate), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void UseFreshOcspResponseTest() {
            // Add client with indeterminate OCSP, then with valid OCSP.
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            TestOcspResponseBuilder builder1 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder1.SetProducedAt(checkDate);
            builder1.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            builder1.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder1.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClient ocspClient1 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder1);
            TestOcspResponseBuilder builder2 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder2.SetProducedAt(checkDate.AddDays(5));
            builder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            builder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder2);
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddOcspClient(ocspClient1
                ).AddOcspClient(ocspClient2);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.TRUSTED_OCSP_RESPONDER, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void ValidityAssuredTest() {
            String checkCertFileName = SOURCE_FOLDER + "validityAssuredSigningCert.pem";
            IX509Certificate certificate = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, certificate, checkDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(certificate, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.VALIDITY_ASSURED, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void NoRevocationDataTest() {
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.NO_REVOCATION_DATA, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TryFetchRevocationDataOnlineTest() {
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(1, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.NO_REVOCATION_DATA, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CrlEncodingErrorTest() {
            byte[] crl = new TestCrlBuilder(caCert, caPrivateKey).MakeCrl();
            crl[5] = 0;
            ValidationReport report = new ValidationReport();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.All()
                , CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-2));
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (2));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddCrlClient(new _ICrlClient_355(crl)).Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME
                );
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.CRL_PARSING_ERROR, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.NO_REVOCATION_DATA, item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        private sealed class _ICrlClient_355 : ICrlClient {
            public _ICrlClient_355(byte[] crl) {
                this.crl = crl;
            }

            public ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
                return JavaCollectionsUtil.SingletonList(crl);
            }

            private readonly byte[] crl;
        }

        [NUnit.Framework.Test]
        public virtual void SortResponsesTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            // The oldest one, but the only one valid.
            TestOcspResponseBuilder ocspBuilder1 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder1.SetProducedAt(checkDate);
            ocspBuilder1.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate));
            ocspBuilder1.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(3)));
            TestOcspClient ocspClient1 = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder1);
            TestOcspResponseBuilder ocspBuilder2 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder2.SetProducedAt(checkDate.AddDays(3));
            ocspBuilder2.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(3)));
            ocspBuilder2.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            ocspBuilder2.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClient ocspClient2 = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder2);
            TestOcspResponseBuilder ocspBuilder3 = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder3.SetProducedAt(checkDate.AddDays(5));
            ocspBuilder3.SetThisUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(5)));
            ocspBuilder3.SetNextUpdate(DateTimeUtil.GetCalendar(checkDate.AddDays(10)));
            ocspBuilder3.SetCertificateStatus(FACTORY.CreateUnknownStatus());
            TestOcspClient ocspClient3 = new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder3);
            TestCrlBuilder crlBuilder1 = new TestCrlBuilder(caCert, caPrivateKey, checkDate);
            crlBuilder1.SetNextUpdate(checkDate.AddDays(2));
            TestCrlBuilder crlBuilder2 = new TestCrlBuilder(caCert, caPrivateKey, checkDate.AddDays(2));
            crlBuilder2.SetNextUpdate(checkDate.AddDays(5));
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(crlBuilder1).AddBuilderForCertIssuer
                (crlBuilder2);
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH).SetFreshness(ValidatorContexts.Of(ValidatorContext
                .CRL_VALIDATOR), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays(-5));
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator().AddCrlClient(crlClient
                ).AddOcspClient(ocspClient1).AddOcspClient(ocspClient2).AddOcspClient(ocspClient3);
            validator.Validate(report, baseContext, checkCert, checkDate);
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(6, report.GetLogs().Count);
            CertificateReportItem item = (CertificateReportItem)report.GetLogs()[0];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.CERT_STATUS_IS_UNKNOWN, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[1];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.OCSP_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(OCSPValidator.CERT_STATUS_IS_UNKNOWN, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[2];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.FRESHNESS_CHECK, checkDate.AddDays(2
                ), checkDate, TimeSpan.FromDays(-5)), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[3];
            NUnit.Framework.Assert.AreEqual(checkCert, item.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.FRESHNESS_CHECK, checkDate, checkDate
                , TimeSpan.FromDays(-5)), item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[4];
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.REVOCATION_DATA_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(RevocationDataValidator.TRUSTED_OCSP_RESPONDER, item.GetMessage());
            item = (CertificateReportItem)report.GetLogs()[5];
            NUnit.Framework.Assert.AreEqual(CertificateChainValidator.CERTIFICATE_CHECK, item.GetCheckName());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CertificateChainValidator.CERTIFICATE_TRUSTED, item
                .GetCertificate().GetSubjectDN()), item.GetMessage());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
        }

        [NUnit.Framework.Test]
        public virtual void CrlWithOnlySomeReasonsTest() {
            TestCrlBuilder builder1 = new TestCrlBuilder(caCert, caPrivateKey);
            builder1.AddExtension(FACTORY.CreateExtensions().GetIssuingDistributionPoint(), true, FACTORY.CreateIssuingDistributionPoint
                (null, false, false, FACTORY.CreateReasonFlags(CRLValidator.ALL_REASONS - 31), false, false));
            TestCrlBuilder builder2 = new TestCrlBuilder(caCert, caPrivateKey);
            builder2.AddExtension(FACTORY.CreateExtensions().GetIssuingDistributionPoint(), true, FACTORY.CreateIssuingDistributionPoint
                (null, false, false, FACTORY.CreateReasonFlags(31), false, false));
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(builder1).AddBuilderForCertIssuer(builder2
                );
            TestOcspResponseBuilder ocspBuilder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            ocspBuilder.SetProducedAt(TimeTestUtil.TEST_DATE_TIME.AddDays(-100));
            certificateRetriever.SetTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH);
            validator.AddOcspClient(new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder)).AddCrlClient(crlClient
                );
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(0, report.GetFailures().Count);
            CertificateReportItem reportItem = (CertificateReportItem)report.GetLogs()[2];
            NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INFO, reportItem.GetStatus());
            NUnit.Framework.Assert.AreEqual(checkCert, reportItem.GetCertificate());
            NUnit.Framework.Assert.AreEqual(CRLValidator.ONLY_SOME_REASONS_CHECKED, reportItem.GetMessage());
        }
    }
}
