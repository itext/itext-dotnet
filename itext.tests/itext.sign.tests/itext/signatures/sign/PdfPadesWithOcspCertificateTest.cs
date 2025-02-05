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
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesWithOcspCertificateTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithOcspCertificateTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithOcspCertificateTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesWithOcspCertificateTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SignCertWithOcspTest() {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signRsaWithOcsp.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String rootCertFileName = certsSrc + "rootRsa2.pem";
            String ocspCertFileName = certsSrc + "ocspCert.pem";
            IX509Certificate signRsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate ocspCert = (IX509Certificate)PemFileHelper.ReadFirstChain(ocspCertFileName)[0];
            IPrivateKey ocspPrivateKey = PemFileHelper.ReadFirstKey(ocspCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient ocspClient = new AdvancedTestOcspClient();
            ocspClient.AddBuilderForCertIssuer(signRsaCert, ocspCert, ocspPrivateKey);
            ocspClient.AddBuilderForCertIssuer(ocspCert, ocspCert, ocspPrivateKey);
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
            padesSigner.SetOcspClient(ocspClient);
            IX509Certificate[] signRsaChain = new IX509Certificate[] { signRsaCert, rootCert };
            padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(signRsaCert), GetCertName
                ((IX509Certificate)tsaChain[0]), GetCertName((IX509Certificate)tsaChain[1]), GetCertName(ocspCert));
            // It is expected to have two OCSP responses, one for signing cert and another for OCSP response.
            expectedNumberOfOcsps.Put(ocspCert.GetSubjectDN().ToString(), 2);
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                , expectedCerts);
        }

        [NUnit.Framework.Test]
        public virtual void SignCertWithoutOcspTest() {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signRsaWithoutOcsp.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String rootCertFileName = certsSrc + "rootRsa.pem";
            String ocspCertFileName = certsSrc + "ocspCert.pem";
            IX509Certificate signRsaCert = PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] signRsaChain = new IX509Certificate[2];
            signRsaChain[0] = signRsaCert;
            signRsaChain[1] = rootCert;
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate ocspCert = PemFileHelper.ReadFirstChain(ocspCertFileName)[0];
            IPrivateKey ocspPrivateKey = PemFileHelper.ReadFirstKey(ocspCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient ocspClient = new AdvancedTestOcspClient();
            ocspClient.AddBuilderForCertIssuer((IX509Certificate)signRsaCert, (IX509Certificate)ocspCert, ocspPrivateKey
                );
            ocspClient.AddBuilderForCertIssuer((IX509Certificate)ocspCert, (IX509Certificate)ocspCert, ocspPrivateKey);
            using (Stream outputStream = new MemoryStream()) {
                PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
                padesSigner.SetOcspClient(ocspClient);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTProfile
                    (signerProperties, signRsaChain, signRsaPrivateKey, testTsa));
                NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.NO_REVOCATION_DATA_FOR_SIGNING_CERTIFICATE, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignCertWithOcspOcspCertSameAsSignCertTest() {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signRsaWithOcsp.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String rootCertFileName = certsSrc + "rootRsa2.pem";
            IX509Certificate signRsaCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient ocspClient = new AdvancedTestOcspClient();
            ocspClient.AddBuilderForCertIssuer(signRsaCert, signRsaCert, signRsaPrivateKey);
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
            padesSigner.SetOcspClient(ocspClient);
            IX509Certificate[] signRsaChain = new IX509Certificate[] { signRsaCert, rootCert };
            padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(rootCert), GetCertName(signRsaCert), GetCertName
                ((IX509Certificate)tsaChain[0]), GetCertName((IX509Certificate)tsaChain[1]));
            // It is expected to have one OCSP response, only for signing cert.
            expectedNumberOfOcsps.Put(signRsaCert.GetSubjectDN().ToString(), 1);
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                , expectedCerts);
        }

        private String GetCertName(IX509Certificate certificate) {
            return certificate.GetSubjectDN().ToString();
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }

        private PdfPadesSigner CreatePdfPadesSigner(String srcFileName, Stream outputStream) {
            return new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName)), outputStream);
        }
    }
}
