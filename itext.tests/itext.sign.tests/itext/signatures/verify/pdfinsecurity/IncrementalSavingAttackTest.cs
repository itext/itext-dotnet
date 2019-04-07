using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Signatures;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Verify.Pdfinsecurity {
    public class IncrementalSavingAttackTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/pdfinsecurity/IncrementalSavingAttackTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR)]
        public virtual void TestISA03() {
            String filePath = sourceFolder + "isa-3.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.VerifySignature(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.Verify());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            document.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void TestISAValidPdf() {
            String filePath = sourceFolder + "isaValidPdf.pdf";
            String signatureName = "Signature1";
            PdfDocument document = new PdfDocument(new PdfReader(filePath));
            SignatureUtil sigUtil = new SignatureUtil(document);
            PdfPKCS7 pdfPKCS7 = sigUtil.VerifySignature(signatureName);
            NUnit.Framework.Assert.IsTrue(pdfPKCS7.Verify());
            NUnit.Framework.Assert.IsFalse(sigUtil.SignatureCoversWholeDocument(signatureName));
            String textFromPage = PdfTextExtractor.GetTextFromPage(document.GetPage(1));
            // We are working with the latest revision of the document, that's why we should get amended page text.
            // However Signature shall be marked as not covering the complete document, indicating its invalidity
            // for the current revision.
            NUnit.Framework.Assert.AreEqual("This is manipulated malicious text, ha-ha!", textFromPage);
            NUnit.Framework.Assert.AreEqual(2, sigUtil.GetTotalRevisions());
            NUnit.Framework.Assert.AreEqual(1, sigUtil.GetRevision(signatureName));
            Stream sigInputStream = sigUtil.ExtractRevision(signatureName);
            PdfDocument sigRevDocument = new PdfDocument(new PdfReader(sigInputStream));
            SignatureUtil sigRevUtil = new SignatureUtil(sigRevDocument);
            PdfPKCS7 sigRevSignatureData = sigRevUtil.VerifySignature(signatureName);
            NUnit.Framework.Assert.IsTrue(sigRevSignatureData.Verify());
            NUnit.Framework.Assert.IsTrue(sigRevUtil.SignatureCoversWholeDocument(signatureName));
            sigRevDocument.Close();
            document.Close();
        }
    }
}
