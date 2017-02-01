using System;

namespace iText.IO.Image {
    public class JpegTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg1() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg2() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_gray.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg3() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_monochrome.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg4() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_negate.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg5() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_year1900.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenJpeg6() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "WP_20140410_001_year1980.jpg");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }
    }
}
