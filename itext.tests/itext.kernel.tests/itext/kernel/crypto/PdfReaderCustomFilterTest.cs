using System;
using iText.IO.Util;
using iText.Kernel.Crypto.Securityhandler;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Crypto {
    public class PdfReaderCustomFilterTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfReaderCustomFilterTest/";

        [NUnit.Framework.Test]
        public virtual void EncryptedDocumentCustomFilterStandartTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "customSecurityHandler.pdf"));
                doc.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<UnsupportedSecurityHandlerException>().With.Message.EqualTo(MessageFormatUtil.Format(UnsupportedSecurityHandlerException.UnsupportedSecurityHandler, "/Standart")))
;
        }
    }
}
