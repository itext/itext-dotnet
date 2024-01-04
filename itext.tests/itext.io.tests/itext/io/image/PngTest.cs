/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class PngTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/PngTest/";

        [NUnit.Framework.Test]
        public virtual void Grayscale8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "grayscale8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(0, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void Grayscale16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "grayscale16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(0, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void Graya8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "graya8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(4, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageMask());
            NUnit.Framework.Assert.AreEqual(1, img.GetImageMask().GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(8, img.GetImageMask().GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void Graya8BpcDepthWithoutEmbeddedProfileImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "graya8BpcWithoutProfile.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(4, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageMask());
            NUnit.Framework.Assert.AreEqual(1, img.GetImageMask().GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(8, img.GetImageMask().GetBpc());
            NUnit.Framework.Assert.IsNull(img.GetProfile());
        }

        [NUnit.Framework.Test]
        public virtual void Graya8BpcAddColorToAlphaImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "graya8BpcAddColorToAlpha.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
        }

        [NUnit.Framework.Test]
        public virtual void Rgb8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgb8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
        }

        [NUnit.Framework.Test]
        public virtual void Rgb16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgb16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(2, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void RgbWithoutSaveColorProfileImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgbWithoutSaveColorProfile.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(2, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNull(img.GetProfile());
        }

        [NUnit.Framework.Test]
        public virtual void Rgba8BpcDepthImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgba8Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(6, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageMask());
            NUnit.Framework.Assert.AreEqual(1, img.GetImageMask().GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(8, img.GetImageMask().GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void Rgba16BpcDepthImageTest() {
            // iText explicitly processes 16bit images as 8bit
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgba16Bpc.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(6, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageMask());
            NUnit.Framework.Assert.AreEqual(1, img.GetImageMask().GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(8, img.GetImageMask().GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed2BpcImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed2BpcImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(3, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed1BpcImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed1BpcImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(100, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(1, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(3, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void Indexed2BpcWithAlphaChannelTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexed2BpcWithAlphaChannel.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(3, ((PngImageData)img).GetColorType());
        }

        [NUnit.Framework.Test]
        public virtual void GrayscaleSimpleTransparencyImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "grayscaleSimpleTransparencyImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(200, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(200, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(0, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageAttributes());
            NUnit.Framework.Assert.AreEqual("[0 0]", img.GetImageAttributes().Get(PngImageHelperConstants.MASK));
        }

        [NUnit.Framework.Test]
        public virtual void RgbSimpleTransparencyImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "rgbSimpleTransparencyImage.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(600, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(100, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
            NUnit.Framework.Assert.AreEqual(3, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(2, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageAttributes());
            NUnit.Framework.Assert.AreEqual("[255 255 0 0 0 0]", img.GetImageAttributes().Get(PngImageHelperConstants.
                MASK));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedAddColorToAlphaImageTest() {
            ImageData img = ImageDataFactory.Create(sourceFolder + "indexedAddColorToAlpha.png");
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(346, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(49, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(2, img.GetBpc());
            //Indexed colorspace contains one component indeed
            NUnit.Framework.Assert.AreEqual(1, img.GetColorEncodingComponentsNumber());
            NUnit.Framework.Assert.AreEqual(3, ((PngImageData)img).GetColorType());
            NUnit.Framework.Assert.IsNotNull(img.GetImageAttributes());
            NUnit.Framework.Assert.AreEqual(0, ((int[])img.GetImageAttributes().Get(PngImageHelperConstants.MASK))[0]);
            NUnit.Framework.Assert.AreEqual(0, ((int[])img.GetImageAttributes().Get(PngImageHelperConstants.MASK))[1]);
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
            // Test a more specific entry point
            ImageData img = ImageDataFactory.CreatePng(UrlUtil.ToURL(sourceFolder + "size300Px300Dpi.png"));
            NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
            NUnit.Framework.Assert.AreEqual(300, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiX());
            NUnit.Framework.Assert.AreEqual(300, img.GetDpiY());
        }

        [NUnit.Framework.Test]
        public virtual void SRGBImageTest() {
            using (FileStream fis = new FileStream(sourceFolder + "sRGBImage.png", FileMode.Open, FileAccess.Read)) {
                byte[] imageBytes = StreamUtil.InputStreamToArray(fis);
                // Test a more specific entry point
                ImageData img = ImageDataFactory.CreatePng(imageBytes);
                NUnit.Framework.Assert.AreEqual(ImageType.PNG, img.GetOriginalType());
                NUnit.Framework.Assert.AreEqual(50, img.GetWidth(), 0);
                NUnit.Framework.Assert.AreEqual(50, img.GetHeight(), 0);
                NUnit.Framework.Assert.AreEqual(96, img.GetDpiX());
                NUnit.Framework.Assert.AreEqual(96, img.GetDpiY());
                NUnit.Framework.Assert.AreEqual(2.2, ((PngImageData)img).GetGamma(), 0.0001f);
                PngChromaticities pngChromaticities = ((PngImageData)img).GetPngChromaticities();
                NUnit.Framework.Assert.AreEqual(0.3127f, pngChromaticities.GetXW(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.329f, pngChromaticities.GetYW(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.64f, pngChromaticities.GetXR(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.33f, pngChromaticities.GetYR(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.3f, pngChromaticities.GetXG(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.6f, pngChromaticities.GetYG(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.15f, pngChromaticities.GetXB(), 0.0001f);
                NUnit.Framework.Assert.AreEqual(0.06f, pngChromaticities.GetYB(), 0.0001f);
            }
        }
    }
}
