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
using iText.Commons.Utils;
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.IO.Source;
using iText.IO.Util;
using iText.Test;

namespace iText.IO.Image {
    [NUnit.Framework.Category("UnitTest")]
    public class TiffTest : ExtendedITextTest {
        private const double DELTA = 1e-5;

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/image/TiffTest/";

        [NUnit.Framework.Test]
        public virtual void OpenTiff1() {
            byte[] imageBytes = StreamUtil.InputStreamToArray(new FileStream(SOURCE_FOLDER + "WP_20140410_001.tif", FileMode.Open
                , FileAccess.Read));
            // Test a more specific entry point
            ImageData img = ImageDataFactory.CreateTiff(imageBytes, false, 1, false);
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff2() {
            // Test a more specific entry point
            String sourceFile = SOURCE_FOLDER + "WP_20140410_001_gray.tiff";
            CreateTiff(sourceFile, 8, 2592D, 1456D);
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff3() {
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "WP_20140410_001_monochrome.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff4() {
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "WP_20140410_001_negate.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff5() {
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "WP_20140410_001_year1900.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void OpenTiff6() {
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "WP_20140410_001_year1980.tiff");
            NUnit.Framework.Assert.AreEqual(2592, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(1456, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(8, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void GetStringDataFromTiff() {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, "img_cmyk.tif"));
            TIFFDirectory dir = new TIFFDirectory(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                (bytes)), 0);
            String[] stringArray = new String[] { "iText? 7.1.7-SNAPSHOT ?2000-2019 iText Group NV (AGPL-version)\u0000"
                 };
            NUnit.Framework.Assert.AreEqual(stringArray, dir.GetField(305).GetAsStrings());
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionCreateTiffImageTest() {
            String sourceFile = SOURCE_FOLDER + "group3CompressionImage.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionBECreateTiffImageTest() {
            String sourceFile = SOURCE_FOLDER + "group3CompressionImageBE.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void Group3Compression2DCreateTiffImageTest() {
            String sourceFile = SOURCE_FOLDER + "group3CompressionImage2d.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionEolErrorCreateTiffImageTest() {
            String sourceFile = SOURCE_FOLDER + "group3CompressionImageWithEolError.tif";
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => CreateTiff(sourceFile
                , 1, 1024D, 768D));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionCreateImageDataTest() {
            String sourceFile = SOURCE_FOLDER + "group3CompressionImage.tif";
            ImageData img = ImageDataFactory.Create(UrlUtil.ToURL(SOURCE_FOLDER + "group3CompressionImage.tif"));
            NUnit.Framework.Assert.AreEqual(1024, img.GetWidth(), 0);
            NUnit.Framework.Assert.AreEqual(768, img.GetHeight(), 0);
            NUnit.Framework.Assert.AreEqual(1, img.GetBpc());
        }

        [NUnit.Framework.Test]
        public virtual void Group4CompressionTiffImageTest() {
            String sourceFile = SOURCE_FOLDER + "group4CompressionImage.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression1BitMinIsBlackTest() {
            String sourceFile = SOURCE_FOLDER + "adobeDeflateCompression1BitMinIsBlack.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression1BitMinIsWhiteTest() {
            String sourceFile = SOURCE_FOLDER + "adobeDeflateCompression1BitMinIsWhite.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression8BitMinIsBlackTest() {
            String sourceFile = SOURCE_FOLDER + "adobeDeflateCompression8BitMinIsBlack.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression8BitMinIsWhiteTest() {
            String sourceFile = SOURCE_FOLDER + "adobeDeflateCompression8BitMinIsWhite.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression8BitRgbTest() {
            String sourceFile = SOURCE_FOLDER + "adobeDeflateCompression8BitRgb.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateComp16BitMinIsBlackCreateTiffTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .CreateTiff(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitMinIsBlack.tif"), false, 1, false
                ));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateComp16BitMinIsBlackCreateImageTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .Create(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitMinIsBlack.tif")));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateComp16BitMinIsWhiteCreateTiffTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .CreateTiff(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitMinIsWhite.tif"), false, 1, false
                ));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateComp16BitMinIsWhiteCreateImageTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .Create(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitMinIsWhite.tif")));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression16BitRgbCreateTiffTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .CreateTiff(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitRgb.tif"), false, 1, false));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AdobeDeflateCompression16BitRgbCreateImageTest() {
            // TODO: DEVSIX-5791 (update test when support for adobeDeflate compression tiff image will be realized)
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => ImageDataFactory
                .Create(UrlUtil.ToURL(SOURCE_FOLDER + "adobeDeflateCompression16BitRgb.tif")));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.CANNOT_READ_TIFF_IMAGE
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CcittRleCompressionTest() {
            String sourceFile = SOURCE_FOLDER + "ccittRleCompression.tif";
            CreateTiff(sourceFile, 1, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void DeflateCompression8BitRgbTest() {
            String sourceFile = SOURCE_FOLDER + "deflateCompression8BitRgb.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void DeflateCompression8BitPaletteTest() {
            String sourceFile = SOURCE_FOLDER + "deflateCompression8BitPalette.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void JpegCompression8BitYcbcrTest() {
            String sourceFile = SOURCE_FOLDER + "jpegCompression8BitYcbcr.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void OldJpegCompression8BitYcbcrTest() {
            String sourceFile = SOURCE_FOLDER + "oldJpegCompression8BitYcbcr.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void LzwCompression8BitRgbTest() {
            String sourceFile = SOURCE_FOLDER + "lzwCompression8BitRgb.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void LzwCompression8BitPaletteTest() {
            String sourceFile = SOURCE_FOLDER + "lzwCompression8BitPalette.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void PackbitsCompression8BitMinIsBlackTest() {
            String sourceFile = SOURCE_FOLDER + "packbitsCompression8BitMinIsBlack.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        [NUnit.Framework.Test]
        public virtual void PackbitsCompression8BitMinIsWhiteTest() {
            String sourceFile = SOURCE_FOLDER + "packbitsCompression8BitMinIsWhite.tif";
            CreateTiff(sourceFile, 8, 1024D, 768D);
        }

        private static void CreateTiff(String sourceFile, int bpc, double width, double height) {
            ImageData img = ImageDataFactory.CreateTiff(UrlUtil.ToURL(sourceFile), false, 1, false);
            NUnit.Framework.Assert.AreEqual(bpc, img.GetBpc(), DELTA);
            NUnit.Framework.Assert.AreEqual(width, img.GetWidth(), DELTA);
            NUnit.Framework.Assert.AreEqual(height, img.GetHeight(), DELTA);
        }
    }
}
