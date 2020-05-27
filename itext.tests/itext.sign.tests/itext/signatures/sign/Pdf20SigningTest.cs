using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class Pdf20SigningTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/Pdf20SigningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/Pdf20SigningTest/";

        public static readonly String keystorePath = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/signCertRsa01.p12";

        public static readonly char[] password = "testpass".ToCharArray();

        private X509Certificate[] chain;

        private ICipherParameters pk;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void Init() {
            pk = Pkcs12FileHelper.ReadFirstKey(keystorePath, password, password);
            chain = Pkcs12FileHelper.ReadFirstChain(keystorePath, password);
        }

        [NUnit.Framework.Test]
        public virtual void SignExistingFieldWhenDirectAcroformAndNoSigFlagTest() {
            String src = sourceFolder + "signExistingFieldWhenDirectAcroformAndNoSigFlag.pdf";
            String dest = destinationFolder + "signExistingFieldWhenDirectAcroformAndNoSigFlag.pdf";
            String fieldName = "Signature1";
            Sign(src, fieldName, dest, chain, pk, DigestAlgorithms.SHA256, PdfSigner.CryptoStandard.CADES, PdfSigner.NOT_CERTIFIED
                );
            PdfDocument doc = new PdfDocument(new PdfReader(dest));
            PdfNumber sigFlag = doc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm).GetAsNumber(PdfName.
                SigFlags);
            NUnit.Framework.Assert.AreEqual(new PdfNumber(3).IntValue(), sigFlag.IntValue());
        }

        protected internal virtual void Sign(String src, String name, String dest, X509Certificate[] chain, ICipherParameters
             pk, String digestAlgorithm, PdfSigner.CryptoStandard subfilter, int certificationLevel) {
            PdfReader reader = new PdfReader(src);
            StampingProperties properties = new StampingProperties();
            properties.UseAppendMode();
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), properties);
            signer.SetCertificationLevel(certificationLevel);
            signer.SetFieldName(name);
            // Creating the signature
            IExternalSignature pks = new PrivateKeySignature(pk, digestAlgorithm);
            signer.SignDetached(pks, chain, null, null, null, 0, subfilter);
        }
    }
}
