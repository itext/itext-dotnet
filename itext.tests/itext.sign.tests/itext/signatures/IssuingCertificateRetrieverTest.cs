using System;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Testutils;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Mocks;
using iText.Test;

namespace iText.Signatures {
//\cond DO_NOT_DOCUMENT
    internal class IssuingCertificateRetrieverTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.Test]
        public virtual void TestResourceRetrieverUsage() {
            IX509Certificate[] cert = PemFileHelper.ReadFirstChain(CERTS_SRC + "intermediate.pem");
            IList<Uri> urlsCalled = new List<Uri>();
            MockResourceRetriever mockRetriever = new MockResourceRetriever();
            mockRetriever.OnGetInputStreamByUrl((u) => {
                urlsCalled.Add(u);
                try {
                    return FileUtil.GetInputStreamForFile(CERTS_SRC + "root.pem");
                }
                catch (System.IO.IOException e) {
                    throw new Exception("Error reading certificate.", e);
                }
            }
            );
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithResourceRetriever(() => mockRetriever);
            builder.GetCertificateRetriever().RetrieveIssuerCertificate(cert[0]);
            NUnit.Framework.Assert.AreEqual(1, urlsCalled.Count);
            NUnit.Framework.Assert.AreEqual("http://test.example.com/example-ca/certs/ca/ca.crt", urlsCalled[0].ToString
                ());
        }
    }
//\endcond
}
