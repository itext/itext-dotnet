using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    public class LtvVerifierUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/LtvVerifierUnitTest/";

        [NUnit.Framework.Test]
        public virtual void SetVerifierTest() {
            LtvVerifier verifier1 = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            LtvVerifier verifier2 = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier1.SetVerifier(verifier2);
            NUnit.Framework.Assert.AreSame(verifier2, verifier1.verifier);
        }

        [NUnit.Framework.Test]
        public virtual void SetVerifyRootCertificateTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.SetVerifyRootCertificate(true);
            NUnit.Framework.Assert.IsTrue(verifier.verifyRootCertificate);
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNotNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.pkcs7 = null;
            IList<VerificationOK> list = JavaCollectionsUtil.EmptyList<VerificationOK>();
            NUnit.Framework.Assert.AreSame(list, verifier.Verify(list));
        }

        [NUnit.Framework.Test]
        public virtual void GetCRLsFromDSSCRLsNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.dss = new PdfDictionary();
            NUnit.Framework.Assert.AreEqual(new List<Object>(), verifier.GetCRLsFromDSS());
        }

        [NUnit.Framework.Test]
        public virtual void GetOCSPResponsesFromDSSOCSPsNullTest() {
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(new FileStream(SOURCE_FOLDER + "ltvDoc.pdf"
                , FileMode.Open, FileAccess.Read))));
            verifier.dss = new PdfDictionary();
            NUnit.Framework.Assert.AreEqual(new List<Object>(), verifier.GetOCSPResponsesFromDSS());
        }
    }
}
