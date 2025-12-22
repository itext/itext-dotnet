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
using System.IO;
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Events {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CRLValidatorEventTriggeringTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/CRLValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] KEY_PASSWORD = "testpassphrase".ToCharArray();

        private MockChainValidator mockChainValidator;

        private IX509Certificate crlIssuerCert;

        private IPrivateKey crlIssuerKey;

        private IssuingCertificateRetriever certificateRetriever;

        private ValidatorChainBuilder validatorChainBuilder;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpOnce() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            certificateRetriever = new IssuingCertificateRetriever();
            SignatureValidationProperties parameters = new SignatureValidationProperties();
            mockChainValidator = new MockChainValidator();
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => certificateRetriever
                ).WithSignatureValidationProperties(parameters).WithCertificateChainValidatorFactory(() => mockChainValidator
                );
        }

        [NUnit.Framework.Test]
        public virtual void AlgorithmReportingTest() {
            RetrieveTestResources("happyPath");
            byte[] crl = CreateCrl(crlIssuerCert, crlIssuerKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-5), TimeTestUtil.
                TEST_DATE_TIME.AddDays(+5));
            MockEventListener testEventListener = new MockEventListener();
            validatorChainBuilder.GetEventManager().Register(testEventListener);
            PerformValidation("happyPath", TimeTestUtil.TEST_DATE_TIME, crl);
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => e is AlgorithmUsageEvent));
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => "CRL response check.".Equals(((AlgorithmUsageEvent
                )e).GetUsageLocation())));
        }

        private void RetrieveTestResources(String path) {
            String resourcePath = SOURCE_FOLDER + path + "/";
            crlIssuerCert = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "crl-issuer.cert.pem")[0];
            crlIssuerKey = PemFileHelper.ReadFirstKey(SOURCE_FOLDER + "keys/crl-key.pem", KEY_PASSWORD);
        }

        private byte[] CreateCrl(IX509Certificate issuerCert, IPrivateKey issuerKey, DateTime issueDate, DateTime 
            nextUpdate) {
            return CreateCrl(issuerCert, issuerKey, issueDate, nextUpdate, null, (DateTime)TimestampConstants.UNDEFINED_TIMESTAMP_DATE
                , 0);
        }

        private byte[] CreateCrl(IX509Certificate issuerCert, IPrivateKey issuerKey, DateTime issueDate, DateTime 
            nextUpdate, IX509Certificate revokedCert, DateTime revocationDate, int reason) {
            TestCrlBuilder builder = new TestCrlBuilder(issuerCert, issuerKey, issueDate);
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
            String missingCertsFileName = resourcePath + "chain.pem";
            IX509Certificate[] knownCerts = PemFileHelper.ReadFirstChain(missingCertsFileName);
            certificateRetriever.AddKnownCertificates(JavaUtil.ArraysAsList(knownCerts));
            IX509Certificate certificateUnderTest = (IX509Certificate)PemFileHelper.ReadFirstChain(resourcePath + "sign.cert.pem"
                )[0];
            ValidationReport result = new ValidationReport();
            ValidationContext context = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR, CertificateSource
                .SIGNER_CERT, TimeBasedContext.PRESENT);
            validatorChainBuilder.BuildCRLValidator().Validate(result, context, certificateUnderTest, (IX509Crl)CertificateUtil
                .ParseCrlFromStream(new MemoryStream(encodedCrl)), testDate, testDate);
            return result;
        }
    }
}
