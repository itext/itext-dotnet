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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class PadesSignatureLevelTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PadesSignatureLevelTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PadesSignatureLevelTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelTTest01() {
            String outFileName = destinationFolder + "padesSignatureLevelTTest01.pdf";
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "signCertRsa01.p12";
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            X509Certificate[] signRsaChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signRsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            IExternalSignature pks = new PrivateKeySignature(signRsaPrivateKey, DigestAlgorithms.SHA256);
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties());
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.SignDetached(pks, signRsaChain, null, null, testTsa, 0, PdfSigner.CryptoStandard.CADES);
            PadesSigTest.BasicCheckSignedDoc(destinationFolder + "padesSignatureLevelTTest01.pdf", "Signature1");
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelLTTest01() {
            String outFileName = destinationFolder + "padesSignatureLevelLTTest01.pdf";
            String srcFileName = sourceFolder + "signedPAdES-T.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String caCertFileName = certsSrc + "rootRsa.p12";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            ICrlClient crlClient = new TestCrlClient(caCert, caPrivateKey);
            TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(outFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("Signature1", ocspClient, crlClient, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            BasicCheckDssDict("padesSignatureLevelLTTest01.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelLTATest01() {
            String outFileName = destinationFolder + "padesSignatureLevelLTATest01.pdf";
            String srcFileName = sourceFolder + "signedPAdES-LT.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                new StampingProperties().UseAppendMode());
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.Timestamp(testTsa, "timestampSig1");
            PadesSigTest.BasicCheckSignedDoc(destinationFolder + "padesSignatureLevelLTATest01.pdf", "timestampSig1");
        }

        private void BasicCheckDssDict(String fileName) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(destinationFolder + fileName));
            PdfDictionary dssDict = outDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            NUnit.Framework.Assert.IsNotNull(dssDict);
            NUnit.Framework.Assert.AreEqual(4, dssDict.Size());
        }
    }
}
