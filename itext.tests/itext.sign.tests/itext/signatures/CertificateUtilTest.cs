using System;
using Org.BouncyCastle.X509;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures {
    public class CertificateUtilTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly char[] PASSWORD = "testpass".ToCharArray();

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCert.p12"
                , PASSWORD)[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.AreEqual("https://itextpdf.com/en", url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLUsualTimestampCertificateTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "tsCertRsa.p12", PASSWORD
                )[0];
            String url = CertificateUtil.GetTSAURL(tsaCert);
            NUnit.Framework.Assert.IsNull(url);
        }

        [NUnit.Framework.Test]
        public virtual void GetTSAURLAdobeExtensionNotTaggedTest() {
            X509Certificate tsaCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(CERTS_SRC + "adobeExtensionCertWithoutTag.p12"
                , PASSWORD)[0];
            NUnit.Framework.Assert.Catch(typeof(InvalidCastException), () => CertificateUtil.GetTSAURL(tsaCert));
        }
    }
}
