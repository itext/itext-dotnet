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
using iText.Signatures.Cms;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PadesTwoPhaseSigningTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesTwoPhaseSigningTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesTwoPhaseSigningTest/";

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DifferentDigestAlgorithmsTest() {
            String fileName = "differentDigestAlgorithmsTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String rootCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, PASSWORD);
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(rootCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTSAClient(testTsa).SetTimestampSignatureName
                ("timestampSig1");
            using (MemoryStream preparedDoc = new MemoryStream()) {
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, DigestAlgorithms
                    .SHA256, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => twoPhaseSigningHelper.SignCMSContainerWithBaselineLTAProfile
                    (new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA512), new PdfReader(new MemoryStream(preparedDoc
                    .ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.DIGEST_ALGORITHMS_ARE_NOT_SAME
                    , "SHA256", "SHA512"), exception.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MissingTimestampClientTest() {
            String fileName = "missingTimestampClientTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String rootCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IX509Certificate[] certChain = new IX509Certificate[] { signCert, rootCert };
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(rootCertFileName, PASSWORD);
            PadesTwoPhaseSigningHelper twoPhaseSigningHelper = new PadesTwoPhaseSigningHelper();
            TestCrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(rootCert, caPrivateKey);
            crlClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(rootCert, caPrivateKey);
            ocspClient.AddBuilderForCertIssuer(rootCert, caPrivateKey);
            twoPhaseSigningHelper.SetCrlClient(crlClient).SetOcspClient(ocspClient).SetTimestampSignatureName("timestampSig1"
                );
            using (MemoryStream preparedDoc = new MemoryStream()) {
                CMSContainer container = twoPhaseSigningHelper.CreateCMSContainerWithoutSignature(certChain, DigestAlgorithms
                    .SHA256, new PdfReader(srcFileName), preparedDoc, CreateSignerProperties());
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => twoPhaseSigningHelper.SignCMSContainerWithBaselineLTAProfile
                    (new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256), new PdfReader(new MemoryStream(preparedDoc
                    .ToArray())), FileUtil.GetFileOutputStream(outFileName), "Signature1", container));
                NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.TSA_CLIENT_IS_MISSING, exception.Message);
            }
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
