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
    public class TimestampSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/TimestampSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/TimestampSigTest/";

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
        public virtual void TimestampTest01() {
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String outFileName = destinationFolder + "timestampTest01.pdf";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(outFileName, FileMode.Create), 
                false);
            TestTsaClient testTsa = new TestTsaClient(iText.IO.Util.JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            signer.Timestamp(testTsa, "timestampSig1");
        }
        //        TimeStampToken tsWrong = new TimeStampResponse(Files.readAllBytes(Paths.get("c:\\Users\\yulian\\Desktop\\myTs"))).getTimeStampToken();
        //
        //        JcaSimpleSignerInfoVerifierBuilder sigVerifBuilder = new JcaSimpleSignerInfoVerifierBuilder();
        //        X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.readFirstChain(p12FileName, password)[0];
        //        SignerInformationVerifier signerInfoVerif = sigVerifBuilder.setProvider(BouncyCastleProvider.PROVIDER_NAME).build(caCert.getPublicKey());
        //        boolean signatureValid = tsWrong.isSignatureValid(signerInfoVerif);
        //
    }
}
