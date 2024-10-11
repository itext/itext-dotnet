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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesWithTimestampCertificateTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithTimestampCertificateTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithTimestampCertificateTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesWithTimestampCertificateTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentTimestampCertTest() {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertWithCrl.pem";
            String tsaCertFileName = certsSrc + "tsCertWithOcspCrl.pem";
            String caCertFileName = certsSrc + "rootCert.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            AdvancedTestOcspClient ocspClient = new AdvancedTestOcspClient();
            ocspClient.AddBuilderForCertIssuer((IX509Certificate)tsaChain[0], caCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer((IX509Certificate)tsaChain[1], caCert, caPrivateKey);
            padesSigner.SetOcspClient(ocspClient);
            AdvancedTestCrlClient crlClient = new AdvancedTestCrlClient();
            crlClient.AddBuilderForCertIssuer((IX509Certificate)signRsaChain[0], caCert, caPrivateKey);
            padesSigner.SetCrlClient(crlClient);
            padesSigner.SignWithBaselineLTProfile(signerProperties, signRsaChain, signRsaPrivateKey, testTsa);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            IList<String> expectedCerts = JavaUtil.ArraysAsList(GetCertName(caCert), GetCertName((IX509Certificate)signRsaChain
                [0]), GetCertName((IX509Certificate)tsaChain[0]), GetCertName((IX509Certificate)tsaChain[1]));
            // It is expected to have two OCSP responses, one for timestamp cert and another timestamp root cert.
            expectedNumberOfOcsps.Put(caCert.GetSubjectDN().ToString(), 2);
            expectedNumberOfCrls.Put(caCert.GetSubjectDN().ToString(), 1);
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
