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
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    [NUnit.Framework.TestFixtureSource("CreateParametersTestFixtureData")]
    public class PdfPadesAdvancedTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

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

        private readonly int? amountOfCrlsForSign;

        private readonly int? amountOfOcspsForSign;

        private readonly int? amountOfCrlsForRoot;

        private readonly int? amountOfOcspsForRoot;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public PdfPadesAdvancedTest(Object signingCertName, Object rootCertName, Object isOcspRevoked, Object cmpFilePostfix
            , Object amountOfCrlsForSign, Object amountOfOcspsForSign, Object amountOfCrlsForRoot, Object amountOfOcspsForRoot
            ) {
            this.signingCertName = (String)signingCertName;
            this.rootCertName = (String)rootCertName;
            this.isOcspRevoked = (bool?)isOcspRevoked;
            this.cmpFilePostfix = (String)cmpFilePostfix;
            this.amountOfCrlsForSign = (int?)amountOfCrlsForSign;
            this.amountOfOcspsForSign = (int?)amountOfOcspsForSign;
            this.amountOfCrlsForRoot = (int?)amountOfCrlsForRoot;
            this.amountOfOcspsForRoot = (int?)amountOfOcspsForRoot;
        }

        public PdfPadesAdvancedTest(Object[] array)
            : this(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]) {
        }

        public static IEnumerable<Object[]> CreateParameters() {
            IList<Object[]> parameters = new List<Object[]>();
            parameters.AddAll(CreateParametersUsingRootName("rootCertNoCrlNoOcsp", 0, 0));
            parameters.AddAll(CreateParametersUsingRootName("rootCertCrlOcsp", 0, 1));
            parameters.AddAll(CreateParametersUsingRootName("rootCertCrlNoOcsp", 1, 0));
            parameters.AddAll(CreateParametersUsingRootName("rootCertOcspNoCrl", 0, 1));
            return parameters;
        }

        public static ICollection<NUnit.Framework.TestFixtureData> CreateParametersTestFixtureData() {
            return CreateParameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        private static IList<Object[]> CreateParametersUsingRootName(String rootCertName, int crlsForRoot, int ocspForRoot
            ) {
            return JavaUtil.ArraysAsList(new Object[] { "signCertCrlOcsp.pem", rootCertName + ".pem", false, "_signCertCrlOcsp_"
                 + rootCertName, 0, 1, crlsForRoot, ocspForRoot }, new Object[] { "signCertCrlOcsp.pem", rootCertName 
                + ".pem", true, "_signCertCrlOcsp_" + rootCertName + "_revoked", 1, 0, crlsForRoot, ocspForRoot }, new 
                Object[] { "signCertOcspNoCrl.pem", rootCertName + ".pem", false, "_signCertOcspNoCrl_" + rootCertName
                , 0, 1, crlsForRoot, ocspForRoot }, new Object[] { "signCertOcspNoCrl.pem", rootCertName + ".pem", true
                , "_signCertOcspNoCrl_" + rootCertName + "_revoked", 0, 0, crlsForRoot, ocspForRoot }, new Object[] { 
                "signCertNoOcspNoCrl.pem", rootCertName + ".pem", false, "_signCertNoOcspNoCrl_" + rootCertName, 0, 0, 
                crlsForRoot, ocspForRoot }, new Object[] { "signCertCrlNoOcsp.pem", rootCertName + ".pem", false, "_signCertCrlNoOcsp_"
                 + rootCertName, 1, 0, crlsForRoot, ocspForRoot });
        }

        [NUnit.Framework.Test]
        public virtual void SignWithAdvancedClientsTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String signCertFileName = CERTS_SRC + signingCertName;
            String rootCertFileName = CERTS_SRC + rootCertName;
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            IX509Certificate signRsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient testOcspClient = new AdvancedTestOcspClient(null);
            TestOcspResponseBuilder ocspBuilderMainCert = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
            if ((bool)isOcspRevoked) {
                ocspBuilderMainCert.SetCertificateStatus(FACTORY.CreateRevokedStatus(TimeTestUtil.TEST_DATE_TIME, FACTORY.
                    CreateCRLReason().GetKeyCompromise()));
            }
            TestOcspResponseBuilder ocspBuilderRootCert = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
            testOcspClient.AddBuilderForCertIssuer(signRsaCert, ocspBuilderMainCert);
            testOcspClient.AddBuilderForCertIssuer(rootCert, ocspBuilderRootCert);
            AdvancedTestCrlClient testCrlClient = new AdvancedTestCrlClient();
            TestCrlBuilder crlBuilderMainCert = new TestCrlBuilder(rootCert, rootPrivateKey);
            crlBuilderMainCert.AddCrlEntry(signRsaCert, FACTORY.CreateCRLReason().GetKeyCompromise());
            crlBuilderMainCert.AddCrlEntry(rootCert, FACTORY.CreateCRLReason().GetKeyCompromise());
            TestCrlBuilder crlBuilderRootCert = new TestCrlBuilder(rootCert, rootPrivateKey);
            crlBuilderRootCert.AddCrlEntry(rootCert, FACTORY.CreateCRLReason().GetKeyCompromise());
            testCrlClient.AddBuilderForCertIssuer(signRsaCert, crlBuilderMainCert);
            testCrlClient.AddBuilderForCertIssuer(rootCert, crlBuilderRootCert);
            SignerProperties signerProperties = CreateSignerProperties();
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                , outputStream);
            padesSigner.SetOcspClient(testOcspClient);
            padesSigner.SetCrlClient(testCrlClient);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] signRsaChain = new IX509Certificate[] { signRsaCert, rootCert };
            if (signCertFileName.Contains("NoOcspNoCrl") || (signCertFileName.Contains("OcspNoCrl") && (bool)isOcspRevoked
                )) {
                try {
                    Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTAProfile
                        (signerProperties, signRsaChain, pks, testTsa));
                    NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.NO_REVOCATION_DATA_FOR_SIGNING_CERTIFICATE, exception
                        .Message);
                }
                finally {
                    outputStream.Dispose();
                }
            }
            else {
                padesSigner.SignWithBaselineLTAProfile(signerProperties, signRsaChain, pks, testTsa);
                TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
                AssertDss(outputStream, rootCert, signRsaCert, (IX509Certificate)tsaChain[0], (IX509Certificate)tsaChain[1
                    ]);
            }
        }

        private void AssertDss(MemoryStream outputStream, IX509Certificate rootCert, IX509Certificate signRsaCert, 
            IX509Certificate tsaCert, IX509Certificate rootTsaCert) {
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            if (amountOfCrlsForRoot + amountOfCrlsForSign != 0) {
                expectedNumberOfCrls.Put(rootCert.GetSubjectDN().ToString(), amountOfCrlsForRoot + amountOfCrlsForSign);
            }
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            if (amountOfOcspsForRoot + amountOfOcspsForSign != 0) {
                expectedNumberOfOcsps.Put(rootCert.GetSubjectDN().ToString(), amountOfOcspsForRoot + amountOfOcspsForSign);
            }
            IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(signRsaCert), GetCertName
                (tsaCert), GetCertName(rootTsaCert));
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                , expectedCerts);
        }

        private String GetCertName(IX509Certificate certificate) {
            return certificate.GetSubjectDN().ToString();
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(signerProperties.GetFieldName()).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }
    }
}
