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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesSignerLtvExtensionsTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesSignerLtvExtensionsTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesSignerLtvExtensionsTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void OcspNoCheckExtensionTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String signCertFileName = CERTS_SRC + "signCertRsa01.pem";
            String rootCertFileName = CERTS_SRC + "rootRsa.pem";
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            String ocspCertFileName = CERTS_SRC + "ocspCert.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate ocspCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspCertFileName)[0];
            IPrivateKey ocspPrivateKey = PemFileHelper.ReadFirstKey(ocspCertFileName, PASSWORD);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient();
            TestOcspResponseBuilder ocspBuilderRootCert = new TestOcspResponseBuilder(ocspCert, ocspPrivateKey);
            TestOcspResponseBuilder ocspBuilderOcspCert = new TestOcspResponseBuilder(rootCert, rootPrivateKey);
            testOcspClient.AddBuilderForCertIssuer(rootCert, ocspBuilderRootCert);
            testOcspClient.AddBuilderForCertIssuer(ocspCert, ocspBuilderOcspCert);
            SignerProperties signerProperties = CreateSignerProperties();
            using (MemoryStream outputStream = new MemoryStream()) {
                PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                    , outputStream);
                padesSigner.SetOcspClient(testOcspClient);
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                IX509Certificate[] signRsaChain = new IX509Certificate[] { signCert, rootCert };
                padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, pks, testTsa);
                TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
                IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
                expectedNumberOfOcsps.Put(ocspCert.GetSubjectDN().ToString(), 3);
                IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(signCert), GetCertName
                    ((IX509Certificate)tsaChain[0]), GetCertName(ocspCert));
                TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), new Dictionary<String, int?>(), expectedNumberOfOcsps
                    , expectedCerts);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(SignLogMessageConstant.REVOCATION_DATA_NOT_ADDED_VALIDITY_ASSURED, LogLevel = LogLevelConstants
            .INFO)]
        public virtual void ValidityAssuredExtensionTest() {
            String srcFileName = SOURCE_FOLDER + "helloWorldDoc.pdf";
            String signCertFileName = CERTS_SRC + "validityAssuredSigningCert.pem";
            String rootCertFileName = CERTS_SRC + "rootRsa.pem";
            String tsaCertFileName = CERTS_SRC + "tsCertRsa.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey rootPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient();
            testOcspClient.AddBuilderForCertIssuer(rootCert, rootPrivateKey);
            SignerProperties signerProperties = CreateSignerProperties();
            using (MemoryStream outputStream = new MemoryStream()) {
                PdfPadesSigner padesSigner = new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName))
                    , outputStream);
                padesSigner.SetOcspClient(testOcspClient);
                IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
                IX509Certificate[] signRsaChain = new IX509Certificate[] { signCert, rootCert };
                padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, pks, testTsa);
                TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
                IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
                expectedNumberOfOcsps.Put(rootCert.GetSubjectDN().ToString(), 2);
                IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(signCert), GetCertName
                    ((IX509Certificate)tsaChain[0]));
                TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), new Dictionary<String, int?>(), expectedNumberOfOcsps
                    , expectedCerts);
            }
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
