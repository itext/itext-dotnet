using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

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

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
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
                false);
            signer.SetFieldName("Signature1");
            signer.GetSignatureAppearance().SetPageRect(new Rectangle(50, 650, 200, 100)).SetReason("Test").SetLocation
                ("TestCity").SetLayer2Text("Approval test signature.\nCreated by iText7.");
            TestTsaClient testTsa = new TestTsaClient(iText.IO.Util.JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.SignDetached(pks, signRsaChain, null, null, testTsa, 0, PdfSigner.CryptoStandard.CADES);
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
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
            TestOcspClient ocspClient = new TestOcspClient(caCert, caPrivateKey);
            TestTsaClient testTsa = new TestTsaClient(iText.IO.Util.JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(outFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("Signature1", ocspClient, crlClient, LtvVerification.CertificateOption.SIGNING_CERTIFICATE
                , LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PadesSignatureLevelLTATest01() {
            String outFileName = destinationFolder + "padesSignatureLevelLTATest01.pdf";
            String srcFileName = sourceFolder + "signedPAdES-LT.pdf";
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                true);
            TestTsaClient testTsa = new TestTsaClient(iText.IO.Util.JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.Timestamp(testTsa, "timestampSig1");
        }
    }
}
