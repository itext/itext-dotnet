using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;

namespace iText.Signatures.Verify {
    public class LtvVerifierTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/verify/LtvVerifierTest/";

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void After() {
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void ValidLtvDocTest01() {
            String ltvTsFileName = sourceFolder + "ltvDoc.pdf";
            LtvVerifier verifier = new LtvVerifier(new PdfDocument(new PdfReader(ltvTsFileName)));
            verifier.SetCertificateOption(LtvVerification.CertificateOption.WHOLE_CHAIN);
            verifier.SetRootStore(Pkcs12FileHelper.InitStore(certsSrc + "rootStore.p12", password));
            IList<VerificationOK> verificationMessages = verifier.Verify(null);
            NUnit.Framework.Assert.AreEqual(7, verificationMessages.Count);
        }
    }
}
