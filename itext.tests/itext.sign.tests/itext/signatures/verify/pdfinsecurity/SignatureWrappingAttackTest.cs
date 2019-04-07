using System;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Verify.Pdfinsecurity {
    public class SignatureWrappingAttackTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/pdfinsecurity/SignatureWrappingAttackTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void TestSWA01() {
            String filePath = sourceFolder + "siwa.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.ReadSignatureData(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.VerifySignatureIntegrityAndAuthenticity());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            document.Close();
        }
    }
}
