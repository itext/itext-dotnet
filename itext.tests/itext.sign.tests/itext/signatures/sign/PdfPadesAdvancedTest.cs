/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Linq;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [NUnit.Framework.TestFixtureSource("CreateParametersTestFixtureData")]
    public class PdfPadesAdvancedTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly bool FIPS_MODE = "BCFIPS".Equals(FACTORY.GetProviderName());

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesAdvancedTest/certs/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesAdvancedTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesAdvancedTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        private readonly String signingCertName;

        private readonly String rootCertName;

        private readonly bool? isOcspRevoked;

        private readonly String cmpFilePostfix;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public PdfPadesAdvancedTest(Object signingCertName, Object rootCertName, Object isOcspRevoked, Object cmpFilePostfix
            ) {
            this.signingCertName = (String)signingCertName;
            this.rootCertName = (String)rootCertName;
            this.isOcspRevoked = (bool?)isOcspRevoked;
            this.cmpFilePostfix = (String)cmpFilePostfix;
        }

        public PdfPadesAdvancedTest(Object[] array)
            : this(array[0], array[1], array[2], array[3]) {
        }

        public static IEnumerable<Object[]> CreateParameters() {
            IList<Object[]> parameters = new List<Object[]>();
            parameters.AddAll(CreateParametersUsingRootName("rootCertNoCrlNoOcsp"));
            parameters.AddAll(CreateParametersUsingRootName("rootCertCrlOcsp"));
            parameters.AddAll(CreateParametersUsingRootName("rootCertCrlNoOcsp"));
            parameters.AddAll(CreateParametersUsingRootName("rootCertOcspNoCrl"));
            return parameters;
        }

        public static ICollection<NUnit.Framework.TestFixtureData> CreateParametersTestFixtureData() {
            return CreateParameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        private static IList<Object[]> CreateParametersUsingRootName(String rootCertName) {
            return JavaUtil.ArraysAsList(new Object[] { "signCertCrlOcsp.pem", rootCertName + ".pem", false, "_signCertCrlOcsp_"
                 + rootCertName }, new Object[] { "signCertCrlOcsp.pem", rootCertName + ".pem", true, "_signCertCrlOcsp_"
                 + rootCertName + "_revoked" }, new Object[] { "signCertOcspNoCrl.pem", rootCertName + ".pem", false, 
                "_signCertOcspNoCrl_" + rootCertName }, new Object[] { "signCertOcspNoCrl.pem", rootCertName + ".pem", 
                true, "_signCertOcspNoCrl_" + rootCertName + "_revoked" }, new Object[] { "signCertNoOcspNoCrl.pem", rootCertName
                 + ".pem", false, "_signCertNoOcspNoCrl_" + rootCertName }, new Object[] { "signCertCrlNoOcsp.pem", rootCertName
                 + ".pem", false, "_signCertCrlNoOcsp_" + rootCertName });
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_REVOKED, Ignore = true)]
        public virtual void SignWithAdvancedClientsTest() {
            String fileName = "signedWith" + cmpFilePostfix + (FIPS_MODE ? "_FIPS.pdf" : ".pdf");
            String outFileName = DESTINATION_FOLDER + fileName;
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName;
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String signCertFileName = CERTS_SRC + signingCertName;
            String rootCertFileName = CERTS_SRC + rootCertName;
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            IX509Certificate signRsaCert = PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] signRsaChain = new IX509Certificate[2];
            signRsaChain[0] = signRsaCert;
            signRsaChain[1] = rootCert;
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient testOcspClient = new AdvancedTestOcspClient(null);
            TestOcspResponseBuilder ocspBuilderMainCert = new TestOcspResponseBuilder((IX509Certificate)signRsaChain[1
                ], rootPrivateKey);
            if ((bool)isOcspRevoked) {
                ocspBuilderMainCert.SetCertificateStatus(FACTORY.CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME, FACTORY.
                    CreateCRLReason().GetKeyCompromise()));
            }
            TestOcspResponseBuilder ocspBuilderRootCert = new TestOcspResponseBuilder((IX509Certificate)signRsaChain[1
                ], rootPrivateKey);
            testOcspClient.AddBuilderForCertIssuer((IX509Certificate)signRsaChain[0], ocspBuilderMainCert);
            testOcspClient.AddBuilderForCertIssuer((IX509Certificate)signRsaChain[1], ocspBuilderRootCert);
            AdvancedTestCrlClient testCrlClient = new AdvancedTestCrlClient();
            TestCrlBuilder crlBuilderMainCert = new TestCrlBuilder((IX509Certificate)signRsaChain[1], rootPrivateKey);
            crlBuilderMainCert.AddCrlEntry((IX509Certificate)signRsaChain[0], FACTORY.CreateCRLReason().GetKeyCompromise
                ());
            crlBuilderMainCert.AddCrlEntry((IX509Certificate)signRsaChain[1], FACTORY.CreateCRLReason().GetKeyCompromise
                ());
            TestCrlBuilder crlBuilderRootCert = new TestCrlBuilder((IX509Certificate)signRsaChain[1], rootPrivateKey);
            crlBuilderRootCert.AddCrlEntry((IX509Certificate)signRsaChain[1], FACTORY.CreateCRLReason().GetKeyCompromise
                ());
            testCrlClient.AddBuilderForCertIssuer((IX509Certificate)signRsaChain[0], crlBuilderMainCert);
            testCrlClient.AddBuilderForCertIssuer((IX509Certificate)signRsaChain[1], crlBuilderRootCert);
            SignerProperties signerProperties = CreateSignerProperties();
            Stream outputStream = FileUtil.GetFileOutputStream(outFileName);
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName)), 
                outputStream);
            padesSigner.SetOcspClient(testOcspClient);
            padesSigner.SetCrlClient(testCrlClient);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            if (signCertFileName.Contains("NoOcspNoCrl") || (signCertFileName.Contains("OcspNoCrl") && (bool)isOcspRevoked)) {
                try {
                    Exception exception = NUnit.Framework.Assert.Throws<PdfException>(() =>
                        padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, pks, testTsa));
                    NUnit.Framework.Assert.AreEqual(
                        SignExceptionMessageConstant.NO_REVOCATION_DATA_FOR_SIGNING_CERTIFICATE,
                        exception.Message);
                } finally {
                    outputStream.Close();
                }
            } else {
                padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, pks, testTsa);
                PadesSigTest.BasicCheckSignedDoc(outFileName, "Signature1");
                NUnit.Framework.Assert.IsNull(SignaturesCompareTool.CompareSignatures(outFileName, cmpFileName));
            }
        }
        
        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signerProperties.GetFieldName())
                .SetContent("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100))
                .SetSignatureAppearance(appearance);

            return signerProperties;
        }
    }
}
