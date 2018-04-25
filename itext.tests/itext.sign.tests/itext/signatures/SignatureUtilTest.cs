using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    public class SignatureUtilTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/SignatureUtilTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void GetSignaturesTest01() {
            String inPdf = sourceFolder + "simpleSignature.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            IList<String> signatureNames = signatureUtil.GetSignatureNames();
            NUnit.Framework.Assert.AreEqual(1, signatureNames.Count);
            NUnit.Framework.Assert.AreEqual("Signature1", signatureNames[0]);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void GetSignaturesTest02() {
            String inPdf = sourceFolder + "simpleDocument.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inPdf));
            SignatureUtil signatureUtil = new SignatureUtil(pdfDocument);
            IList<String> signatureNames = signatureUtil.GetSignatureNames();
            NUnit.Framework.Assert.AreEqual(0, signatureNames.Count);
        }
    }
}
