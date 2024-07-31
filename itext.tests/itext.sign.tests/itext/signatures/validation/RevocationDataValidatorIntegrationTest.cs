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
    public class RevocationDataValidatorIntegrationTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/RevocationDataValidatorTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private static IX509Certificate caCert;

        private static IPrivateKey caPrivateKey;

        private static IX509Certificate checkCert;

        private static IX509Certificate responderCert;

        private static IPrivateKey ocspRespPrivateKey;

        private IssuingCertificateRetriever certificateRetriever;

        private SignatureValidationProperties parameters;

        private ValidatorChainBuilder validatorChainBuilder;

        private readonly ValidationContext baseContext = new ValidationContext(ValidatorContext.SIGNATURE_VALIDATOR
            , CertificateSource.SIGNER_CERT, TimeBasedContext.PRESENT);

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
            validatorChainBuilder = new ValidatorChainBuilder().WithIssuingCertificateRetrieverFactory(() => certificateRetriever
                ).WithSignatureValidationProperties(parameters);
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
            parameters.SetRevocationOnlineFetching(ValidatorContexts.All(), CertificateSources.All(), TimeBasedContexts
                .All(), SignatureValidationProperties.OnlineFetching.NEVER_FETCH);
            ValidationReport report = new ValidationReport();
            RevocationDataValidator validator = validatorChainBuilder.BuildRevocationDataValidator();
            validator.AddOcspClient(new TestOcspClient().AddBuilderForCertIssuer(caCert, ocspBuilder)).AddCrlClient(crlClient
                );
            validator.Validate(report, baseContext, checkCert, TimeTestUtil.TEST_DATE_TIME);
            AssertValidationReport.AssertThat(report, (a) => a.HasNumberOfFailures(0).HasLogItem((la) => la.WithCertificate
                (checkCert).WithStatus(ReportItem.ReportItemStatus.INFO).WithMessage(CRLValidator.ONLY_SOME_REASONS_CHECKED
                )));
        }
    }
}
