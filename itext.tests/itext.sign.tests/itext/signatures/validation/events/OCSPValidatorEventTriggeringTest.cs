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
using iText.Signatures.Validation;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Mocks;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation.Events {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class OCSPValidatorEventTriggeringTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/OCSPValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.REVOCATION_DATA_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private ValidatorChainBuilder validatorChainBuilder;

        private MockChainValidator mockCertificateChainValidator;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            String rootCertFileName = SOURCE_FOLDER + "rootCert.pem";
            String checkCertFileName = SOURCE_FOLDER + "signCert.pem";
            String ocspResponderCertFileName = SOURCE_FOLDER + "ocspResponderCert.pem";
            caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            checkCert = (IX509Certificate)PemFileHelper.ReadFirstChain(checkCertFileName)[0];
            responderCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspResponderCertFileName)[0];
            ocspRespPrivateKey = PemFileHelper.ReadFirstKey(ocspResponderCertFileName, PASSWORD);
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            certificateRetriever = new IssuingCertificateRetriever();
            parameters = new SignatureValidationProperties();
            mockCertificateChainValidator = new MockChainValidator();
            validatorChainBuilder = new ValidatorChainBuilder().WithSignatureValidationProperties(parameters).WithIssuingCertificateRetrieverFactory
                (() => certificateRetriever).WithCertificateChainValidatorFactory(() => mockCertificateChainValidator);
        }

        [NUnit.Framework.Test]
        public virtual void AlgorithmReportingTest() {
            DateTime checkDate = TimeTestUtil.TEST_DATE_TIME;
            MockEventListener testEventListener = new MockEventListener();
            validatorChainBuilder.GetEventManager().Register(testEventListener);
            ValidateTest(checkDate);
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => e is AlgorithmUsageEvent));
            NUnit.Framework.Assert.IsTrue(testEventListener.GetEvents().Any((e) => "OCSP response check.".Equals(((AlgorithmUsageEvent
                )e).GetUsageLocation())));
        }

        private ValidationReport ValidateTest(DateTime checkDate) {
            return ValidateTest(checkDate, checkDate.AddDays(1), 0);
        }

        private ValidationReport ValidateTest(DateTime checkDate, DateTime thisUpdate, long freshness) {
            TestOcspResponseBuilder builder = new TestOcspResponseBuilder(responderCert, ocspRespPrivateKey);
            builder.SetThisUpdate(DateTimeUtil.GetCalendar(thisUpdate));
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, builder);
            IBasicOcspResponse basicOCSPResp = FACTORY.CreateBasicOCSPResponse(FACTORY.CreateASN1Primitive(ocspClient.
                GetEncoded(checkCert, caCert, null)));
            ValidationReport report = new ValidationReport();
            certificateRetriever.AddTrustedCertificates(JavaCollectionsUtil.SingletonList(caCert));
            OCSPValidator validator = validatorChainBuilder.BuildOCSPValidator();
            parameters.SetFreshness(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts.All(), TimeSpan.FromDays
                (freshness));
            validator.Validate(report, baseContext, checkCert, basicOCSPResp.GetResponses()[0], basicOCSPResp, checkDate
                , checkDate);
            return report;
        }
    }
}
