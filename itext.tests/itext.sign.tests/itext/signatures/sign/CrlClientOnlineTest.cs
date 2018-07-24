using System;
using iText.Signatures;

namespace iText.Signatures.Sign {
    public class CrlClientOnlineTest {
        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/";

        /// <exception cref="System.UriFormatException"/>
        [NUnit.Framework.Test]
        public virtual void CrlClientOnlineURLConstructorTest() {
            String PROTOCOL = "file://";
            Uri[] urls = new Uri[] { new Uri(PROTOCOL + destinationFolder + "duplicateFolder"), new Uri(PROTOCOL + destinationFolder
                 + "duplicateFolder"), new Uri(PROTOCOL + destinationFolder + "uniqueFolder") };
            CrlClientOnline crlClientOnline = new CrlClientOnline(urls);
            NUnit.Framework.Assert.IsTrue(crlClientOnline.GetUrlsSize() == 2);
        }
    }
}
