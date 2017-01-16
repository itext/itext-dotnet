using System;

namespace iText.IO.Image {
    public class Jpeg2000Test {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg2000_1() {
            try {
                ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.JP2");
                Jpeg2000ImageHelper.ProcessImage(img);
            }
            catch (iText.IO.IOException e) {
                NUnit.Framework.Assert.AreEqual(iText.IO.IOException.UnsupportedBoxSizeEqEq0, e.Message);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg2000_2() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.JPC");
            Jpeg2000ImageHelper.ProcessImage(img);
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }
    }
}
