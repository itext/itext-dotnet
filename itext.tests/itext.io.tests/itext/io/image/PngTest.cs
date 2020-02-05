using System;
using iText.Test;

namespace iText.IO.Image {
    public class PngTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/PngTest/";

        [NUnit.Framework.Test]
        public virtual void Grayscale8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "grayscale8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Grayscale16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "grayscale16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Graya8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "graya8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Graya8BpcAddColorToAlphaImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "graya8BpcAddColorToAlpha.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Rgb8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgb8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Rgb16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgb16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void RgbWithoutSaveColorProfileImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgbWithoutSaveColorProfile.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Rgba8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgba8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Rgba16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgba16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void RgbaAddColorToAlphaImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgbaAddColorToAlpha.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed2BpcImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed2BpcImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed1BpcImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed1BpcImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(1, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed2BpcWithAlphaChannelTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed2BpcWithAlphaChannel.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void IndexedAddColorToAlphaImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexedAddColorToAlpha.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorSpace());
        }

        [NUnit.Framework.Test]
        public virtual void Size50Px30DpiImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "size50Px30Dpi.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(50, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(50, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(30, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(30, img.GetDpiY());
        }

        [NUnit.Framework.Test]
        public virtual void Size50Px300DpiImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "size50Px300Dpi.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(50, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(50, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiY());
        }

        [NUnit.Framework.Test]
        public virtual void Size150Px72DpiImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "size150Px72Dpi.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(150, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(150, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(72, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(72, img.GetDpiY());
        }

        [NUnit.Framework.Test]
        public virtual void Size300Px72DpiImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "size300Px72Dpi.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(300, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(72, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(72, img.GetDpiY());
        }

        [NUnit.Framework.Test]
        public virtual void Size300Px300DpiImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "size300Px300Dpi.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(300, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiY());
        }
    }
}
