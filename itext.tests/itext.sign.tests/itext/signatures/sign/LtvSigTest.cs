using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Client;
using iText.Test;

namespace iText.Signatures.Sign {
    public class LtvSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/LtvSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/LtvSigTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="Org.BouncyCastle.Tsp.TSPException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        [NUnit.Framework.Test]
        public virtual void LtvEnabledTest01() {
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String caCertFileName = certsSrc + "rootRsa.p12";
            String srcFileName = sourceFolder + "signedDoc.pdf";
            String ltvFileName = destinationFolder + "ltvEnabledTest01.pdf";
            String ltvTsFileName = destinationFolder + "ltvEnabledTsTest01.pdf";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestTsaClient testTsa = new TestTsaClient(iText.IO.Util.JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient(caCert, caPrivateKey);
            TestCrlClient testCrlClient = new TestCrlClient(caCert, caPrivateKey);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(ltvFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("Signature1", testOcspClient, testCrlClient, LtvVerification.CertificateOption
                .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), new FileStream(ltvTsFileName, FileMode.Create
                ), true);
            signer.Timestamp(testTsa, "timestampSig1");
        }
    }
}
