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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Exceptions;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPadesSignerTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesSignerTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesSignerTest/";

        private static readonly char[] password = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DirectoryPathIsNotADirectoryTest() {
            String fileName = "directoryPathIsNotADirectoryTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            PdfSigner signer = CreatePdfSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = new PdfPadesSigner();
            padesSigner.SetTemporaryDirectoryPath(destinationFolder + "newPdf.pdf");
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTProfile
                (signer, signRsaChain, pks, testTsa));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(SignExceptionMessageConstant.PATH_IS_NOT_DIRECTORY
                , destinationFolder + "newPdf.pdf"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoSignaturesToProlongTest() {
            String fileName = "noSignaturesToProlongTest.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            String caCertFileName = certsSrc + "rootRsa.pem";
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            IX509Certificate caCert = (IX509Certificate)PemFileHelper.ReadFirstChain(caCertFileName)[0];
            IPrivateKey caPrivateKey = PemFileHelper.ReadFirstKey(caCertFileName, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            PdfPadesSigner padesSigner = new PdfPadesSigner();
            padesSigner.SetOcspClient(ocspClient).SetCrlClient(crlClient);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.ProlongSignatures
                (new PdfReader(srcFileName), FileUtil.GetFileOutputStream(outFileName), testTsa));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.NO_SIGNATURES_TO_PROLONG, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultClientsCannotBeCreated() {
            String fileName = "defaultClientsCannotBeCreated.pdf";
            String outFileName = destinationFolder + fileName;
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.pem";
            String tsaCertFileName = certsSrc + "tsCertRsa.pem";
            IX509Certificate[] signRsaChain = PemFileHelper.ReadFirstChain(signCertFileName);
            IPrivateKey signRsaPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            IX509Certificate[] tsaChain = PemFileHelper.ReadFirstChain(tsaCertFileName);
            IPrivateKey tsaPrivateKey = PemFileHelper.ReadFirstKey(tsaCertFileName, password);
            PdfSigner signer = CreatePdfSigner(srcFileName, outFileName);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            PdfPadesSigner padesSigner = new PdfPadesSigner();
            padesSigner.SetTemporaryDirectoryPath(destinationFolder);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => padesSigner.SignWithBaselineLTProfile
                (signer, signRsaChain, pks, testTsa));
            NUnit.Framework.Assert.AreEqual(SignExceptionMessageConstant.DEFAULT_CLIENTS_CANNOT_BE_CREATED, exception.
                Message);
        }

        private PdfSigner CreatePdfSigner(String srcFileName, String outFileName) {
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), FileUtil.GetFileOutputStream(outFileName), new 
                StampingProperties());
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText.");
            return signer;
        }
    }
}
