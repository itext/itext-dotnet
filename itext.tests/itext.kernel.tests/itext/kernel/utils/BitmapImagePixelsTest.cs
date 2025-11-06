/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class BitmapImagePixelsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/BitmapImagePixelsTest/";

        [NUnit.Framework.Test]
        public virtual void ConstructorWithImageByteArrayParameterTest() {
            byte[] imageBytes = new byte[] { 1, 2, 3, 4, 5, 6 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(1, 3, 8, 2, imageBytes);
            NUnit.Framework.Assert.AreEqual(1, imagePixels.GetWidth());
            NUnit.Framework.Assert.AreEqual(3, imagePixels.GetHeight());
            NUnit.Framework.Assert.AreEqual(8, imagePixels.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(2, imagePixels.GetNumberOfComponents());
            NUnit.Framework.Assert.AreEqual(imageBytes, imagePixels.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithImageByteArrayParameterInvalidParamsTest() {
            byte[] imageBytes = new byte[] { 1, 2, 3, 4, 5 };
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new BitmapImagePixels(
                30, 40, 8, 3, imageBytes));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_DATA_LENGTH
                , 28800, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithParametersTest() {
            BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
            NUnit.Framework.Assert.AreEqual(30, imagePixels.GetWidth());
            NUnit.Framework.Assert.AreEqual(40, imagePixels.GetHeight());
            NUnit.Framework.Assert.AreEqual(8, imagePixels.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(3, imagePixels.GetNumberOfComponents());
            byte[] expectedArray = new byte[30 * 40 * 8 * 3 / 8];
            NUnit.Framework.Assert.AreEqual(expectedArray, imagePixels.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithParametersWithTrashBitsOnEahRowTest() {
            // Tests that each row ends on the byte border
            BitmapImagePixels imagePixels = new BitmapImagePixels(15, 15, 4, 3);
            NUnit.Framework.Assert.AreEqual(15, imagePixels.GetWidth());
            NUnit.Framework.Assert.AreEqual(15, imagePixels.GetHeight());
            NUnit.Framework.Assert.AreEqual(4, imagePixels.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(3, imagePixels.GetNumberOfComponents());
            NUnit.Framework.Assert.AreEqual(15, imagePixels.GetMaxComponentValue());
            byte[] expectedArray = new byte[15 * (15 * 4 * 3 / 8 + 1)];
            NUnit.Framework.Assert.AreEqual(expectedArray, imagePixels.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithPdfXObjectTest() {
            String sourceImage = SOURCE_FOLDER + "png-example.png";
            PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(sourceImage));
            BitmapImagePixels imagePixels = new BitmapImagePixels(image);
            NUnit.Framework.Assert.AreEqual(200, imagePixels.GetWidth());
            NUnit.Framework.Assert.AreEqual(200, imagePixels.GetHeight());
            NUnit.Framework.Assert.AreEqual(8, imagePixels.GetBitsPerComponent());
            NUnit.Framework.Assert.AreEqual(3, imagePixels.GetNumberOfComponents());
            NUnit.Framework.Assert.AreEqual(255, imagePixels.GetMaxComponentValue());
            byte[] expectedArray = image.GetPdfObject().GetBytes();
            NUnit.Framework.Assert.AreEqual(expectedArray, imagePixels.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void ConstructorWithPdfXObjectWithoutBpcTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                String sourceImage = SOURCE_FOLDER + "png-example.png";
                PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(sourceImage));
                image.GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(1));
                new BitmapImagePixels(image);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_DATA_LENGTH
                , 120000, 960000), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs1bit1channelTest() {
            byte[] imageBytes = new byte[] { (byte)0b10001000, 0b00100000, 0b01000100, (byte)0b10000000, 0b00100000, (
                byte)0b11100000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(11, 3, 1, 1, imageBytes);
            for (int x = 0; x < 11; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 4 || y == 0 && x == 10 || y == 1 && x == 1 || y == 1 && x == 5 || y
                         == 1 && x == 8 || y == 2 && x == 2 || y == 2 && x == 8 || y == 2 && x == 9 || y == 2 && x == 10) {
                        iText.Test.AssertUtil.AreEqual(1, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 1 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs1bit3channelsTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00011122           23334445           55666777           888 
                /* 0 */ 0b11100000, (byte)0b00001110, (byte)0b00111000, (byte)0b11100000, (byte)/* 1 */ 0b00011100, (byte
                )0b01110000, (byte)0b00000111, (byte)0b00000000, (byte)/* 2 */ 0b00000011, (byte)0b10000001, (byte)0b11000000
                , (byte)0b00000000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(9, 3, 1, 3, imageBytes);
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 4 || y == 0 && x == 6 || y == 0 && x == 8 || y == 1 && x == 1 || y 
                        == 1 && x == 3 || y == 1 && x == 7 || y == 2 && x == 2 || y == 2 && x == 5) {
                        iText.Test.AssertUtil.AreEqual(1, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 1 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs2bit1channelTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00112233           44556677           889900 /* 0 */ 
                0b11000000, (byte)0b00001100, (byte)0b00110000, (byte)/* 1 */ 0b00001100, (byte)0b00110000, (byte)0b00000000
                , (byte)/* 2 */ 0b00000011, (byte)0b11000000, (byte)0b11000000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(11, 3, 2, 1, imageBytes);
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 6 || y == 0 && x == 9 || y == 1 && x == 2 || y == 1 && x == 5 || y 
                        == 2 && x == 3 || y == 2 && x == 4 || y == 2 && x == 8) {
                        iText.Test.AssertUtil.AreEqual(3, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 3 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs2bit3channelTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00000011           11112222           22333333           444444 
                /* 0 */ 0b11111100, (byte)0b00001111, (byte)0b11000000, (byte)0b11111100, (byte)/* 1 */ 0b00000011, (byte
                )0b11110000, (byte)0b00111111, (byte)0b00000000, (byte)/* 2 */ 0b00000000, (byte)0b00001111, (byte)0b11000000
                , (byte)0b00000000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(5, 3, 2, 3, imageBytes);
            for (int x = 0; x < 5; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 2 || y == 0 && x == 4 || y == 1 && x == 1 || y == 1 && x == 3 || y 
                        == 2 && x == 2) {
                        iText.Test.AssertUtil.AreEqual(3, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 1 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(3, imagePixels.GetPixelAsLongs(x, y)[1], () => String.Format("Expected a value of 1 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(3, imagePixels.GetPixelAsLongs(x, y)[2], () => String.Format("Expected a value of 1 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[1], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[2], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs4bit1channelTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00001111           22223333           44445555           6666 
                /* 0 */ 0b11110000, (byte)0b00001111, (byte)0b11110000, (byte)0b11110000, (byte)/* 1 */ 0b00001111, (byte
                )0b11110000, (byte)0b00001111, (byte)0b00000000, (byte)/* 2 */ 0b00000000, (byte)0b00001111, (byte)0b00000000
                , (byte)0b11110000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(7, 3, 4, 1, imageBytes);
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 3 || y == 0 && x == 4 || y == 0 && x == 6 || y == 1 && x == 1 || y 
                        == 1 && x == 2 || y == 1 && x == 5 || y == 2 && x == 3 || y == 2 && x == 6) {
                        iText.Test.AssertUtil.AreEqual(15, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 15 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs4bit3channelTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00000000           00001111           11111111           22222222           22220000 
                /* 0 */ 0b11111111, (byte)0b11110000, (byte)0b00000000, (byte)0b11111111, (byte)0b11110000, (byte)/* 1 */ 
                0b00000000, (byte)0b00001111, (byte)0b11111111, (byte)0b00000000, (byte)0b00000000, (byte)/* 2 */ 0b00000000
                , (byte)0b00001111, (byte)0b11111111, (byte)0b11111111, (byte)0b11110000 };
            BitmapImagePixels imagePixels = new BitmapImagePixels(3, 3, 4, 3, imageBytes);
            for (int x = 0; x < 3; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    if (y == 0 && x == 0 || y == 0 && x == 2 || y == 1 && x == 1 || y == 2 && x == 1 || y == 2 && x == 2) {
                        iText.Test.AssertUtil.AreEqual(15, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 15 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(15, imagePixels.GetPixelAsLongs(x, y)[1], () => String.Format("Expected a value of 15 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(15, imagePixels.GetPixelAsLongs(x, y)[2], () => String.Format("Expected a value of 15 for pixel %d, %d"
                            , my, mx));
                    }
                    else {
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[1], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                        iText.Test.AssertUtil.AreEqual(0, imagePixels.GetPixelAsLongs(x, y)[2], () => String.Format("Expected a value of 0 for pixel %d, %d"
                            , my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs16bit1channelTest() {
            byte[] imageBytes = new byte[] { (byte)/*              00000000           00000000           11111111           11111111 
                /* 0 */ 0b00000010, (byte)0b00000010, (byte)0b00000011, (byte)0b00000011, (byte)/*514, 771*/ /* 1 */ 0b00000100
                , (byte)0b00000100, (byte)0b00000101, (byte)0b00000101, (byte)/* 1028, 1258 */ /* 2 */ 0b00001000, (byte
                )0b00001000, (byte)0b00001001, (byte)0b00001001 };
            /* 2056, 2313 */
            BitmapImagePixels imagePixels = new BitmapImagePixels(2, 3, 16, 1, imageBytes);
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    int expected = (2 << y) + x;
                    expected <<= 8;
                    expected += (2 << y) + x;
                    int mexpected = expected;
                    iText.Test.AssertUtil.AreEqual(expected, imagePixels.GetPixelAsLongs(x, y)[0], () => String.Format("Expected a value of %d for pixel %d, %d"
                        , mexpected, my, mx));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongs16bit3channelTest() {
            int[] testData = new int[] { /*       1r 1g 1b 2r 2g 2b 3r 3g 3b */ /* 0 */ 1, 2, 3, 4, 5, 6, 7, 8, 9, /* 1 */ 
                10, 11, 12, 13, 14, 15, 16, 17, 18, /* 2 */ 19, 20, 21, 22, 23, 24, 25, 26, 27 };
            byte[] imageBytes = new byte[testData.Length * 2];
            for (int i = 0; i < testData.Length; i++) {
                imageBytes[i * 2] = 0;
                imageBytes[i * 2 + 1] = (byte)testData[i];
            }
            BitmapImagePixels imagePixels = new BitmapImagePixels(3, 3, 16, 3, imageBytes);
            for (int x = 0; x < 2; x++) {
                for (int y = 0; y < 3; y++) {
                    int mx = x;
                    int my = y;
                    for (int c = 0; c < 3; c++) {
                        int mc = c;
                        iText.Test.AssertUtil.AreEqual(testData[x * 3 + c + y * 9], imagePixels.GetPixelAsLongs(x, y)[c], () => String
                            .Format("Expected a value of %d for pixel %d, %d", testData[mx * 3 + mc + my * 9], my, mx));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsAsLongsTest() {
            String sourceImage = SOURCE_FOLDER + "png-example.png";
            PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(sourceImage));
            BitmapImagePixels imagePixels = new BitmapImagePixels(image);
            long[] greyColor = new long[] { 195, 195, 195 };
            long[] redColor = new long[] { 237, 28, 36 };
            NUnit.Framework.Assert.AreEqual(greyColor, imagePixels.GetPixelAsLongs(0, 0));
            NUnit.Framework.Assert.AreEqual(redColor, imagePixels.GetPixelAsLongs(100, 50));
        }

        [NUnit.Framework.Test]
        public virtual void GetPixelsTest() {
            String sourceImage = SOURCE_FOLDER + "png-example.png";
            PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(sourceImage));
            BitmapImagePixels imagePixels = new BitmapImagePixels(image);
            double[] greyColor = new double[] { (double)195 / 255, (double)195 / 255, (double)195 / 255 };
            double[] redColor = new double[] { (double)237 / 255, (double)28 / 255, (double)36 / 255 };
            iText.Test.TestUtil.AreEqual(greyColor, imagePixels.GetPixel(0, 0), 0.0001);
            iText.Test.TestUtil.AreEqual(redColor, imagePixels.GetPixel(100, 50), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void SetPixelsTest() {
            String sourceImage = SOURCE_FOLDER + "png-example.png";
            String cmpImage = SOURCE_FOLDER + "png-example-modified.png";
            PdfImageXObject image = new PdfImageXObject(ImageDataFactory.Create(sourceImage));
            BitmapImagePixels imagePixels = new BitmapImagePixels(image);
            double[] orangeColor = new double[] { (double)255 / 255, (double)170 / 255, (double)0 };
            for (int i = 0; i < imagePixels.GetWidth(); i++) {
                imagePixels.SetPixel(i, i, orangeColor);
            }
            PdfImageXObject cmpImageObject = new PdfImageXObject(ImageDataFactory.Create(cmpImage));
            NUnit.Framework.Assert.AreEqual(cmpImageObject.GetPdfObject().GetBytes(), imagePixels.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void XCoordinateCannotBeNegativeGetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                imagePixels.GetPixel(-1, 0);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , -1, 0, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void XCoordinateCannotBeNegativeSetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                double[] orangePixel = new double[] { 1.0, (double)170 / 255, 0.0 };
                imagePixels.SetPixel(-1, 0, orangePixel);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , -1, 0, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void YCoordinateCannotBeNegativeGetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                imagePixels.GetPixel(0, -1);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 0, -1, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void YCoordinateCannotBeNegativeSetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                double[] orangePixel = new double[] { 1.0, (double)170 / 255, 0.0 };
                imagePixels.SetPixel(0, -1, orangePixel);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 0, -1, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void XCoordinateOutOfPictureGetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                imagePixels.GetPixel(31, 0);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 31, 0, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void XCoordinateOutOfPictureSetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                double[] orangePixel = new double[] { 1.0, (double)170 / 255, 0.0 };
                imagePixels.SetPixel(31, 0, orangePixel);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 31, 0, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void YCoordinateOutOfPictureGetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                imagePixels.GetPixel(0, 41);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 0, 41, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void YCoordinateOutOfPictureSetterTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                double[] orangePixel = new double[] { 1.0, (double)170 / 255, 0.0 };
                imagePixels.SetPixel(0, 41, orangePixel);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PIXEL_OUT_OF_BORDERS
                , 0, 41, 30, 40), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PixelArrayShouldMatchColorSpaceTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
                double[] blackPixel = new double[] { 0.0, 0.0, 0.0, 0.0 };
                imagePixels.SetPixel(0, 0, blackPixel);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.LENGTH_OF_ARRAY_SHOULD_MATCH_NUMBER_OF_COMPONENTS
                , 4, 3), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PixelArrayNormalizationTest() {
            BitmapImagePixels imagePixels = new BitmapImagePixels(30, 40, 8, 3);
            double[] greenPixel = new double[] { -10.0, 10.0, -10.0 };
            imagePixels.SetPixel(0, 0, greenPixel);
            double[] expectedPixel = new double[] { 0.0, 1.0, 0.0 };
            iText.Test.TestUtil.AreEqual(expectedPixel, imagePixels.GetPixel(0, 0), 0.0001);
        }
    }
}
