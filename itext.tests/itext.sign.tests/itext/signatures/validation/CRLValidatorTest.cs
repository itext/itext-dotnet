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
using iText.Signatures.Logs;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Validation.Extensions;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CRLValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/CRLValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] KEY_PASSWORD = "testpassphrase".ToCharArray();

        private CRLValidator validator;

        /// <summary>This field is used to configure the file to use for the missing certificates.</summary>
        private String missingCertsFileName;

        /// <summary>This field is used to configure the CRL response to return.</summary>
        private IssuingCertificateRetriever mockCertificateRetriever;

        private CRLValidatorTest.MockChainValidator mockChainValidator;

        private IX509Certificate crlIssuerCert;

        private IX509Certificate signCert;

        private IPrivateKey crlIssuerKey;

        private IPrivateKey intermediateKey;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            validator = new CRLValidator();
            mockChainValidator = new CRLValidatorTest.MockChainValidator();
            validator.SetCertificateChainValidator(mockChainValidator);
        }

        [NUnit.Framework.Test]
        public virtual void HappyPathTest() {
            RetrieveTestResources("happyPath");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            ValidationReport report = PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.IsTrue(report.GetFailures().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void NextUpdateBeforeValidationTest() {
            RetrieveTestResources("happyPath");
            DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(-5);
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-15), nextUpdate);
            ValidationReport report = PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.UPDATE_DATE_BEFORE_CHECKDATE, nextUpdate
                , TimeTestUtil.TEST_DATE_TIME), report.GetFailures()[0].GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void ChainValidatorUsageTest() {
            RetrieveTestResources("happyPath");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            ValidationReport report = PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.IsTrue(report.GetFailures().IsEmpty());
            NUnit.Framework.Assert.AreEqual(1, mockChainValidator.verificationCalls.Count);
            NUnit.Framework.Assert.AreEqual(1, mockChainValidator.verificationCalls[0].requiredExtensions.Count);
            NUnit.Framework.Assert.AreEqual(new KeyUsageExtension(KeyUsage.CRL_SIGN), mockChainValidator.verificationCalls
                [0].requiredExtensions[0]);
            NUnit.Framework.Assert.AreEqual(crlIssuerCert, mockChainValidator.verificationCalls[0].certificate);
        }

        [NUnit.Framework.Test]
        public virtual void IssuerCertificateIsNotFoundTest() {
            RetrieveTestResources("missingIssuer");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            ValidationReport report = PerformValidation("missingIssuer", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_ISSUER_NOT_FOUND, report.GetFailures()[0].GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CrlEncodingErrorTest() {
            RetrieveTestResources("crlEncodingError");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            crl[5] = 0;
            String resourcePath = SOURCE_FOLDER + "crlEncodingError/";
            this.missingCertsFileName = resourcePath + "chain.pem";
            IX509Certificate certificateUnderTest = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "sign.cert.pem"
                )[0];
            ValidationReport report = new ValidationReport();
            new CRLValidator().SetIssuingCertificateRetriever(mockCertificateRetriever).Validate(report, certificateUnderTest
                , crl, TimeTestUtil.TEST_DATE_TIME);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CrlIssuerAndSignCertHaveNoSharedRootTest() {
            RetrieveTestResources("crlIssuerAndSignCertHaveNoSharedRoot");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            ValidationReport report = PerformValidation("crlIssuerAndSignCertHaveNoSharedRoot", TimeTestUtil.TEST_DATE_TIME
                , crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_ISSUER_NO_COMMON_ROOT, report.GetFailures()[0].GetMessage
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CrlIssuerRevokedBeforeSigningDate() {
            // CRL has the certificate revoked before signing date
            RetrieveTestResources("crlIssuerRevokedBeforeSigningDate");
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(-2);
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5), signCert, revocationDate, 1);
            ValidationReport report = PerformValidation("crlIssuerRevokedBeforeSigningDate", TimeTestUtil.TEST_DATE_TIME
                , crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INVALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CRLValidator.CERTIFICATE_REVOKED, crlIssuerCert.GetSubjectDN
                (), revocationDate), report.GetFailures()[0].GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CrlRevokedAfterSigningDate() {
            // CRL has the certificate revoked after signing date
            RetrieveTestResources("happyPath");
            DateTime revocationDate = TimeTestUtil.TEST_DATE_TIME.AddDays(+20);
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(+18), TimeTestUtil
                .TEST_DATE_TIME.AddDays(+23), signCert, revocationDate, 1);
            ValidationReport report = PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.VALID, report.GetValidationResult());
            NUnit.Framework.Assert.AreEqual(2, report.GetLogs().Count);
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignLogMessageConstant.VALID_CERTIFICATE_IS_REVOKED
                , revocationDate), report.GetLogs()[1].GetMessage());
        }

        [NUnit.Framework.Test]
        public virtual void CrlSignatureMismatch() {
            //CRL response is invalid (signature not matching)
            RetrieveTestResources("happyPath");
            byte[] crl = CreateCrl(crlIssuerCert, intermediateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(+18), TimeTestUtil
                .TEST_DATE_TIME.AddDays(+23), signCert, TimeTestUtil.TEST_DATE_TIME.AddDays(+20), 1);
            ValidationReport report = PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.AreEqual(ValidationReport.ValidationResult.INDETERMINATE, report.GetValidationResult
                ());
            NUnit.Framework.Assert.AreEqual(1, report.GetFailures().Count);
            NUnit.Framework.Assert.AreEqual(CRLValidator.CRL_INVALID, report.GetFailures()[0].GetMessage());
        }

        private void RetrieveTestResources(String path) {
            String resourcePath = SOURCE_FOLDER + path + "/";
            crlIssuerCert = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "crl-issuer.cert.pem")[0];
            signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "sign.cert.pem")[0];
            crlIssuerKey = PemFileHelper.ReadFirstKey(SOURCE_FOLDER + "keys/crl-key.pem", KEY_PASSWORD);
            intermediateKey = PemFileHelper.ReadFirstKey(SOURCE_FOLDER + "keys/im_key.pem", KEY_PASSWORD);
        }

        private byte[] CreateCrl(IX509Certificate issuerCert, IPrivateKey issuerKey, DateTime issueDate, DateTime 
            nextUpdate) {
            return CreateCrl(issuerCert, issuerKey, issueDate, nextUpdate, null, (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE
                , 0);
        }

        private byte[] CreateCrl(IX509Certificate issuerCert, IPrivateKey issuerKey, DateTime issueDate, DateTime 
            nextUpdate, IX509Certificate revokedCert, DateTime revocationDate, int reason) {
            TestCrlBuilder builder = new TestCrlBuilder(issuerCert, issuerKey);
            if (nextUpdate != null) {
                builder.SetNextUpdate(nextUpdate);
            }
            if (revocationDate != TimestampConstants.UNDEFINED_TIMESTAMP_DATE && revokedCert != null) {
                builder.AddCrlEntry(revokedCert, revocationDate, reason);
            }
            return builder.MakeCrl();
        }

        public virtual ValidationReport PerformValidation(String testName, DateTime testDate, byte[] encodedCrl) {
            String resourcePath = SOURCE_FOLDER + testName + '/';
            this.missingCertsFileName = resourcePath + "chain.pem";
            IX509Certificate[] knownCerts = PemFileHelper.ReadFirstChain(missingCertsFileName);
            mockCertificateRetriever = new IssuingCertificateRetriever();
            mockCertificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts));
            validator.SetIssuingCertificateRetriever(mockCertificateRetriever);
            IX509Certificate certificateUnderTest = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "sign.cert.pem"
                )[0];
            ValidationReport result = new ValidationReport();
            validator.Validate(result, certificateUnderTest, encodedCrl, testDate);
            return result;
        }

        private class MockChainValidator : CertificateChainValidator {
            public IList<CRLValidatorTest.ValidationCallBack> verificationCalls = new List<CRLValidatorTest.ValidationCallBack
                >();

            public override ValidationReport Validate(ValidationReport result, IX509Certificate certificate, DateTime 
                verificationDate, IList<CertificateExtension> requiredExtensions) {
                verificationCalls.Add(new CRLValidatorTest.ValidationCallBack(certificate, verificationDate, requiredExtensions
                    ));
                return result;
            }
        }

        private class ValidationCallBack {
            public IX509Certificate certificate;

            public DateTime checkDate;

            public IList<CertificateExtension> requiredExtensions;

            public ValidationCallBack(IX509Certificate certificate, DateTime checkDate, IList<CertificateExtension> requiredExtensions
                ) {
                this.certificate = certificate;
                this.checkDate = checkDate;
                this.requiredExtensions = requiredExtensions;
            }
        }
    }
}
