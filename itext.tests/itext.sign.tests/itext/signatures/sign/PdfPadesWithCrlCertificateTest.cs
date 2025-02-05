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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesWithCrlCertificateTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithCrlCertificateTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithCrlCertificateTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesWithCrlCertificateTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SignCertWithCrlTest() {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String rootCertFileName = certsSrc + "root.pem";
            String signCertFileName = certsSrc + "sign.pem";
            String rootCrlFileName = certsSrc + "crlRoot.crt";
            String crlCertFileName = certsSrc + "crlCert.pem";
            String tsaCertFileName = certsSrc + "tsCert.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate crlCert = (IX509Certificate)PemFileHelper.ReadFirstChain(crlCertFileName)[0];
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            CrlClientOnline testCrlClient = new _CrlClientOnline_94();
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
            padesSigner.SetCrlClient(testCrlClient);
            IIssuingCertificateRetriever issuingCertificateRetriever = new _IssuingCertificateRetriever_107(crlCertFileName
                , rootCrlFileName);
            padesSigner.SetIssuingCertificateRetriever(issuingCertificateRetriever);
            IX509Certificate[] signChain = new IX509Certificate[] { signCert, rootCert };
            padesSigner.SignWithBaselineLTProfile(signerProperties, signChain, signRsaPrivateKey, testTsa);
            outputStream.Dispose();
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IDictionary<String, int?> expectedNumberOfCrls = new Dictionary<String, int?>();
            IDictionary<String, int?> expectedNumberOfOcsps = new Dictionary<String, int?>();
            // It is expected to have two CRL responses, one for signing cert and another for CRL response.
            expectedNumberOfCrls.Put(rootCert.GetSubjectDN().ToString(), 1);
            expectedNumberOfCrls.Put(crlCert.GetSubjectDN().ToString(), 1);
            TestSignUtils.AssertDssDict(new MemoryStream(outputStream.ToArray()), expectedNumberOfCrls, expectedNumberOfOcsps
                );
        }

        private sealed class _CrlClientOnline_94 : CrlClientOnline {
            public _CrlClientOnline_94() {
            }

            protected internal override Stream GetCrlResponse(IX509Certificate cert, Uri urlt) {
                if (urlt.ToString().Contains("cert-crl")) {
                    return FileUtil.GetInputStreamForFile(PdfPadesWithCrlCertificateTest.certsSrc + "crlSignedByCrlCert.crl");
                }
                return FileUtil.GetInputStreamForFile(PdfPadesWithCrlCertificateTest.certsSrc + "crlSignedByCA.crl");
            }
        }

        private sealed class _IssuingCertificateRetriever_107 : IssuingCertificateRetriever {
            public _IssuingCertificateRetriever_107(String crlCertFileName, String rootCrlFileName) {
                this.crlCertFileName = crlCertFileName;
                this.rootCrlFileName = rootCrlFileName;
            }

            protected internal override Stream GetIssuerCertByURI(String uri) {
                if (uri.Contains("crl_cert")) {
                    return FileUtil.GetInputStreamForFile(crlCertFileName);
                }
                return FileUtil.GetInputStreamForFile(rootCrlFileName);
            }

            private readonly String crlCertFileName;

            private readonly String rootCrlFileName;
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
