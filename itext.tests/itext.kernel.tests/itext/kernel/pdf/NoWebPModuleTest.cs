using System;
using iText.IO.Image;
using iText.IO.Util;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NoWebPModuleTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/NoWebpModuleTest/";

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void IsWebPSupportedTest() {
            NUnit.Framework.Assert.IsFalse(ImageDataFactory.IsSupportedType(ImageType.WEBP));
        }

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void ReadWebPBytesTest() {
            byte[] webpImageDummy = new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0x00, 0x00, 0x00, 0x00, (
                byte)'W', (byte)'E', (byte)'B', (byte)'P', 0, 0, 0 };
            ImageData imageData = ImageDataFactory.CreateWebP(webpImageDummy);
            NUnit.Framework.Assert.IsNull(imageData);
        }

        [NUnit.Framework.Test]
        [LogMessage(WebPLogMessageConstant.WEBP_NOT_FOUND)]
        public virtual void ReadWebPUrlTest() {
            ImageData imageData = ImageDataFactory.CreateWebP(UrlUtil.ToURL(SOURCE_FOLDER + "webpImage.webp"));
            NUnit.Framework.Assert.IsNull(imageData);
        }
    }
}
