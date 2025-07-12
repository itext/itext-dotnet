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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Codec;
using iText.IO.Exceptions;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GetImageBytesTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject" + "/GetImageBytesTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/xobject/GetImageBytesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultiStageFilters() {
            // TODO DEVSIX-2940: extracted image is blank
            TestFile("multistagefilter1.pdf", "Obj13", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void TestAscii85Filters() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("ASCII85_RunLengthDecode.pdf", "Im9", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestCcittFilters() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("ccittfaxdecode.pdf", "background0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateDecodeFilters() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            // TODO DEVSIX-2941: extracted indexed devicegray RunLengthDecode gets color inverted
            TestFile("flatedecode_runlengthdecode.pdf", "Im9", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestDctDecodeFilters() {
            // TODO DEVSIX-2940: extracted image is upside down
            TestFile("dctdecode.pdf", "im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Testjbig2Filters() {
            // TODO DEVSIX-2942: extracted jbig2 image is not readable by most popular image viewers
            TestFile("jbig2decode.pdf", "2", "jbig2");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateCmyk() {
            TestFile("img_cmyk.pdf", "Im1", "tif");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateCmykIcc() {
            TestFile("img_cmyk_icc.pdf", "Im1", "tif");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateIndexed() {
            TestFile("img_indexed.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateRgbIcc() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("img_rgb_icc.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateRgb() {
            TestFile("img_rgb.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateCalRgb() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("img_calrgb.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestJPXDecode() {
            TestFile("JPXDecode.pdf", "Im1", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void TestSeparationCSWithICCBasedAsAlternative() {
            TestFile("separationCSWithICCBasedAsAlternative.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestSeparationCSWithDeviceCMYKAsAlternative() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => {
                TestFile("separationCSWithDeviceCMYKAsAlternative.pdf", "Im1", "png");
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestGrayScalePng() {
            TestFile("grayImages.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestSeparationCSWithDeviceRGBAsAlternative() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("separationCSWithDeviceRgbAsAlternative.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestSeparationCSWithDeviceRGBAsAlternative2() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("spotColorImagesSmall.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithJPXDecoderAndFunctionType0() {
            TestFile("RGBJpxF0.pdf", "Im1", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithDCTDecoderAndFunctionType0() {
            TestFile("RGBDctF0.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithFlateDecoderAndFunctionType0() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6445 fix different DeflaterOutputStream behavior)
            TestFile("RGBFlateF0.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestCMYKSeparationCSWithJPXDecoderAndFunctionType2() {
            TestFile("CMYKJpxF2.pdf", "Im1", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithJPXDecoderAndFunctionType2() {
            TestFile("RGBJpxF2.pdf", "Im1", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void TestCMYKSeparationCSWithDCTDecoderAndFunctionType2() {
            TestFile("CMYKDctF2.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithDCTDecoderAndFunctionType2() {
            TestFile("RGBDctF2.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void TestRGBSeparationCSWithFlateDecoderAndFunctionType2() {
            TestFile("RGBFlateF2.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void ExtractByteAlignedG4TiffImageTest() {
            String inFileName = SOURCE_FOLDER + "extractByteAlignedG4TiffImage.pdf";
            String outImageFileName = DESTINATION_FOLDER + "extractedByteAlignedImage.png";
            String cmpImageFileName = SOURCE_FOLDER + "cmp_extractByteAlignedG4TiffImage.png";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            GetImageBytesTest.ImageExtractor listener = new GetImageBytesTest.ImageExtractor(this);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            IList<byte[]> images = listener.GetImages();
            NUnit.Framework.Assert.AreEqual(1, images.Count);
            using (Stream fos = FileUtil.GetFileOutputStream(outImageFileName)) {
                fos.Write(images[0], 0, images.Count);
            }
            // expected and actual are swapped here for simplicity
            int expectedLen = images[0].Length;
            byte[] buf = new byte[expectedLen];
            using (Stream @is = FileUtil.GetInputStreamForFile(cmpImageFileName)) {
                int read = @is.JRead(buf, 0, buf.Length);
                NUnit.Framework.Assert.AreEqual(expectedLen, read);
                read = @is.JRead(buf, 0, buf.Length);
                NUnit.Framework.Assert.IsTrue(read <= 0);
            }
            NUnit.Framework.Assert.AreEqual(images[0], buf);
        }

        [NUnit.Framework.Test]
        public virtual void ExpectedByteAlignedTiffImageExtractionTest() {
            //Byte-aligned image is expected in pdf file, but in fact it's not
            String inFileName = SOURCE_FOLDER + "expectedByteAlignedTiffImageExtraction.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            GetImageBytesTest.ImageExtractor listener = new GetImageBytesTest.ImageExtractor(this);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => processor.ProcessPageContent
                (pdfDocument.GetPage(1)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.EXPECTED_TRAILING_ZERO_BITS_FOR_BYTE_ALIGNED_LINES
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitTest() {
            TestFile("deviceGray8bit.pdf", "fzImg0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitFlateDecodeTest() {
            TestFile("deviceGray8bitFlateDecode.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray1bitFlateDecodeInvertedTest() {
            TestFile("deviceGray1bitFlateDecodeInverted.pdf", "Im0", "png", true);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray4bitFlateDecodeInvertedTest() {
            //TODO DEVSIX-7015 Support decoding images with Decode array and BitsPerComponent more than 1
            TestFile("deviceGray4bitFlateDecodeInverted.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitFlateDecodeWithMaskTest() {
            TestFile("deviceGray8bitFlateDecodeWithMask.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitDctDecodeTest() {
            TestFile("deviceGray8bitDctDecode.pdf", "fzImg0", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitJPXDecodeTest() {
            TestFile("deviceGray8bitJPXDecode.pdf", "fzImg0", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray1bitCCITTFaxDecodeTest() {
            TestFile("deviceGray1bitCCITTFaxDecode.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitFlateDecodeMaskRotatedTest() {
            TestFile("deviceGray8bitFlateDecodeMaskRotated.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitFlateDecodeScaledTest() {
            TestFile("deviceGray8bitFlateDecodeScaled.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceGray8bitFlateCombinedTransformationTest() {
            TestFile("deviceGray8bitFlateCombinedTransformation.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgb1BitDecodeInvertTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb1BitDecodeInvert.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 1), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgb1BitDecodeTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb1BitDecode.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 1), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgb1BitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb1Bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 1), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgb4BitDecodeInvertTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb4BitDecodeInvert.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgb4BitDecodeTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb4BitDecode.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgb4BitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgb4Bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDctDecodeInvertTest() {
            TestFile("dRgbDctDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDctDecodeTest() {
            TestFile("dRgbDctDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDctMaskedTest() {
            TestFile("dRgbDctMasked.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTMaskedDecodeTest() {
            TestFile("dRgbDCTMaskedDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTMaskedInvertTest() {
            TestFile("dRgbDCTMaskedInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTtransformationsDecodeInvertTest() {
            TestFile("dRgbDCTtransformationsDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTtransformationsDecodeTest() {
            TestFile("dRgbDCTtransformationsDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTtransformationsMaskedDecodeInvertTest() {
            TestFile("dRgbDCTtransformationsMaskedDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTtransformationsMaskedDecodeTest() {
            TestFile("dRgbDCTtransformationsMaskedDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTtransformationsTest() {
            TestFile("dRgbDCTtransformations.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyDecodeInvertTest() {
            TestFile("dRgbDCTTransparancyDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyDecodeTest() {
            TestFile("dRgbDCTTransparancyDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyMaskDecodeInvertTest() {
            TestFile("dRgbDCTTransparancyMaskDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyMaskDecodeTest() {
            TestFile("dRgbDCTTransparancyMaskDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyMaskTest() {
            TestFile("dRgbDCTTransparancyMask.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTest() {
            TestFile("dRgbDCTTransparancy.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformDecodeInvertTest() {
            TestFile("dRgbDCTTransparancyTransformDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformDecodeTest() {
            TestFile("dRgbDCTTransparancyTransformDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformMaskDecodeInvertTest() {
            TestFile("dRgbDCTTransparancyTransformMaskDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformMaskDecodeTest() {
            TestFile("dRgbDCTTransparancyTransformMaskDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformMaskTest() {
            TestFile("dRgbDCTTransparancyTransformMask.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbDCTTransparancyTransformTest() {
            TestFile("dRgbDCTTransparancyTransform.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlateTest() {
            TestFile("dRgbFlate.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlateTransparencyTest() {
            TestFile("dRgbFlateTransparency.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlateInvertedTest() {
            TestFile("dRgbFlateInverted.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlateRotatedTest() {
            TestFile("dRgbFlateRotated.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlateRotatedInvertedTest() {
            TestFile("dRgbFlateRotatedInverted.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlate1bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgbFlate1bit.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 1), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DRgbFlate4bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("dRgbFlate4bit.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ICCBasedDctMaskedInvertedTest() {
            TestFile("ICCBasedDctMaskedInverted.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void ICCBasedDCTTransformMaskedDecodeTest() {
            TestFile("ICCBasedDCTTransformMaskedDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void ICCBasedDCTTransformMaskedDecodeInvertTest() {
            TestFile("ICCBasedDCTTransformMaskedDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void ICCBasedFlateTransformMaskedDecodeTest() {
            TestFile("ICCBasedFlateTransformMaskedDecode.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void ICCBasedFlateTransformMaskedDecodeInvertTest() {
            TestFile("ICCBasedFlateTransformMaskedDecodeInvert.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceCMYKTest() {
            TestFile("deviceCMYK.pdf", "Im1", "tif");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceCMYKFlateDecodeInvertedTest() {
            TestFile("deviceCMYKFlateDecodeInverted.pdf", "Im1", "tif");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray8bitTest() {
            TestFile("calGray8bit.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray8bitGamma22Test() {
            TestFile("calGray8bitGamma22.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray8bitGamma18InvertedTest() {
            TestFile("calGray8bitGamma18Inverted.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray1bitTest() {
            TestFile("calGray1bit.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray1bitInvertedTest() {
            TestFile("calGray1bitInverted.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray4bitGamma22Test() {
            TestFile("calGray4bitGamma22.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray4bitGamma10InvertedTest() {
            TestFile("calGray4bitGamma10Inverted.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalGray8bitExtGStateTest() {
            TestFile("calGray8bitExtGStateTest.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitTest() {
            TestFile("calRGB8bit.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitCustomGammaTest() {
            TestFile("calRGB8bitCustomGamma.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitInvertedTest() {
            TestFile("calRGB8bitInverted.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB4bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("calRGB4bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , "4"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitNoFilterTest() {
            TestFile("calRGB8bitNoFilter.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitSMaskTest() {
            TestFile("calRGB8bitSMask.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitExtGStateTest() {
            TestFile("calRGB8bitExtGState.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB8bitCustomWhitePointTest() {
            TestFile("calRGB8bitCustomWhitePoint.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB1bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("calRGB1bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , "1"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CalRGB2bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("calRGB2bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , "2"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Lab8bitTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("lab8bit.pdf"
                , "Im1", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/Lab"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LabDctMaskedTest() {
            TestFile("labDctMasked.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void LabDctTransformTest() {
            TestFile("labDctTransform.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void LabDctTransparancyTest() {
            TestFile("labDctTransparancy.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void LabDctTransparancyMaskTest() {
            TestFile("labDctTransparancyMask.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void LabDctTransparancyTransformTest() {
            TestFile("labDctTransparancyTransform.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void LabDctTransparancyTransformMaskTest() {
            TestFile("labDctTransparancyTransformMask.pdf", "Im1", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed1bitTest() {
            TestFile("indexed1bit.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed2bitTest() {
            TestFile("indexed2bit.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed4bitTest() {
            TestFile("indexed4bit.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed8bitTest() {
            TestFile("indexed8bit.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed8bitGradientTest() {
            TestFile("indexed8bitGradient.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Indexed8bitSMaskTest() {
            TestFile("indexed8bitSMask.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation1bitDeviceCMYKTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => TestFile("separation1bitDeviceCMYK.pdf"
                , "Im0", "png"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceCMYKTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => TestFile("separation8bitDeviceCMYK.pdf"
                , "Im0", "png"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceRGBTest() {
            TestFile("separation8bitDeviceRGB.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitLabTest() {
            TestFile("separation8bitLab.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceCMYKExtGStateTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => TestFile("separation8bitDeviceCMYKExtGState.pdf"
                , "Im0", "png"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.GET_IMAGEBYTES_FOR_SEPARATION_COLOR_ONLY_SUPPORTS_RGB
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceRGBTransparencyTest() {
            TestFile("separation8bitDeviceRGBTransparency.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceRGBDctDecodeTest() {
            TestFile("separation8bitDeviceRGBDctDecode.pdf", "Im0", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Separation8bitDeviceRGBCustomDecodeRangeTest() {
            TestFile("separation8bitDeviceRGBCustomDecodeRange.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation1bitDeviceRGBTest() {
            TestFile("separation1bitDeviceRGB.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation2bitDeviceRGBTest() {
            TestFile("separation2bitDeviceRGB.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void Separation4bitDeviceRGBTest() {
            TestFile("separation4bitDeviceRGB.pdf", "Im0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceCMYKTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceCMYK.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceRGBTransparencyTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceRGBTransparency.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceRGBSpotASpotBTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceRGBSpotASpotB.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN4bitDeviceCMYKTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN4bitDeviceCMYKTest.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_DEPTH_IS_NOT_SUPPORTED
                , "4"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceCMYKTransparencyDCTDecodeTest() {
            TestFile("deviceN8bitDeviceCMYKTransparencyDCTDecode.pdf", "Im0", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bit5ChannelsTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bit5Channels.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceRGBCustomDecodeTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceRGBCustomDecode.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceCMYKFunctionType0Test() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceCMYKFunctionType0.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DeviceN8bitDeviceRGBRotatedTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => TestFile("deviceN8bitDeviceRGBRotated.pdf"
                , "Im0", "tif"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        private void TestFile(String filename, String objectid, String expectedImageFormat) {
            TestFile(filename, objectid, expectedImageFormat, false);
        }

        private void TestFile(String filename, String objectid, String expectedImageFormat, bool saveResult) {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + filename)) {
                using (PdfDocument pdfDocument = new PdfDocument(reader)) {
                    PdfResources resources = pdfDocument.GetPage(1).GetResources();
                    PdfDictionary xobjects = resources.GetResource(PdfName.XObject);
                    PdfImageXObject img = FindImageXObjectByName(xobjects, new PdfName(objectid));
                    if (img == null) {
                        throw new ArgumentException("Image reference " + objectid + " not found - Available keys are " + xobjects.
                            KeySet());
                    }
                    NUnit.Framework.Assert.AreEqual(expectedImageFormat, img.IdentifyImageFileExtension());
                    byte[] result = img.GetImageBytes(true);
                    if (saveResult) {
                        System.IO.File.WriteAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, filename.JSubstring(0, filename.Length 
                            - 4) + ".new." + expectedImageFormat), result);
                    }
                    byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER, filename.JSubstring(0, filename.
                        Length - 4) + "." + expectedImageFormat));
                    if (img.IdentifyImageFileExtension().Equals("tif")) {
                        CompareTiffImages(cmpBytes, result);
                    }
                    else {
                        NUnit.Framework.Assert.AreEqual(cmpBytes, result);
                    }
                }
            }
        }

        private PdfImageXObject FindImageXObjectByName(PdfDictionary xobjects, PdfName targetName) {
            if (xobjects == null) {
                return null;
            }
            foreach (PdfName name in xobjects.KeySet()) {
                PdfObject obj = xobjects.Get(name);
                if (obj == null) {
                    continue;
                }
                if (obj.IsIndirectReference()) {
                    obj = ((PdfIndirectReference)obj).GetRefersTo();
                }
                if (!(obj is PdfStream)) {
                    continue;
                }
                PdfStream stream = (PdfStream)obj;
                PdfName subtype = stream.GetAsName(PdfName.Subtype);
                if (PdfName.Image.Equals(subtype) && name.Equals(targetName)) {
                    return new PdfImageXObject(stream);
                }
                if (PdfName.Form.Equals(subtype)) {
                    PdfDictionary innerResources = stream.GetAsDictionary(PdfName.Resources);
                    if (innerResources != null) {
                        PdfDictionary innerXObjects = innerResources.GetAsDictionary(PdfName.XObject);
                        PdfImageXObject result = FindImageXObjectByName(innerXObjects, targetName);
                        if (result != null) {
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        private void CompareTiffImages(byte[] cmpBytes, byte[] resultBytes) {
            int cmpNumDirectories = TIFFDirectory.GetNumDirectories(new RandomAccessFileOrArray(new RandomAccessSourceFactory
                ().CreateSource(cmpBytes)));
            int resultNumDirectories = TIFFDirectory.GetNumDirectories(new RandomAccessFileOrArray(new RandomAccessSourceFactory
                ().CreateSource(resultBytes)));
            NUnit.Framework.Assert.AreEqual(cmpNumDirectories, resultNumDirectories);
            for (int dirNum = 0; dirNum < cmpNumDirectories; ++dirNum) {
                TIFFDirectory cmpDir = new TIFFDirectory(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                    (cmpBytes)), dirNum);
                TIFFDirectory resultDir = new TIFFDirectory(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource
                    (resultBytes)), dirNum);
                NUnit.Framework.Assert.AreEqual(cmpDir.GetNumEntries(), resultDir.GetNumEntries());
                NUnit.Framework.Assert.AreEqual(cmpDir.GetIFDOffset(), resultDir.GetIFDOffset());
                NUnit.Framework.Assert.AreEqual(cmpDir.GetNextIFDOffset(), resultDir.GetNextIFDOffset());
                NUnit.Framework.Assert.AreEqual(cmpDir.GetTags(), resultDir.GetTags());
                foreach (int tag in cmpDir.GetTags()) {
                    NUnit.Framework.Assert.AreEqual(cmpDir.IsTagPresent(tag), resultDir.IsTagPresent(tag));
                    TIFFField cmpField = cmpDir.GetField(tag);
                    TIFFField resultField = resultDir.GetField(tag);
                    if (tag != TIFFConstants.TIFFTAG_SOFTWARE) {
                        CompareFields(cmpField, resultField);
                    }
                }
                CompareImageData(cmpDir, resultDir, cmpBytes, resultBytes);
            }
        }

        private void CompareFields(TIFFField cmpField, TIFFField resultField) {
            if (cmpField.GetFieldType() == TIFFField.TIFF_LONG) {
                NUnit.Framework.Assert.AreEqual(cmpField.GetAsLongs(), resultField.GetAsLongs());
            }
            else {
                if (cmpField.GetFieldType() == TIFFField.TIFF_BYTE) {
                    NUnit.Framework.Assert.AreEqual(cmpField.GetAsBytes(), resultField.GetAsBytes());
                }
                else {
                    if (cmpField.GetFieldType() == TIFFField.TIFF_SBYTE) {
                        NUnit.Framework.Assert.AreEqual(cmpField.GetAsBytes(), resultField.GetAsBytes());
                    }
                    else {
                        if (cmpField.GetFieldType() == TIFFField.TIFF_SHORT) {
                            NUnit.Framework.Assert.AreEqual(cmpField.GetAsChars(), resultField.GetAsChars());
                        }
                        else {
                            if (cmpField.GetFieldType() == TIFFField.TIFF_SLONG) {
                                NUnit.Framework.Assert.AreEqual(cmpField.GetAsInts(), resultField.GetAsInts());
                            }
                            else {
                                if (cmpField.GetFieldType() == TIFFField.TIFF_SSHORT) {
                                    NUnit.Framework.Assert.AreEqual(cmpField.GetAsChars(), resultField.GetAsChars());
                                }
                                else {
                                    if (cmpField.GetFieldType() == TIFFField.TIFF_UNDEFINED) {
                                        NUnit.Framework.Assert.AreEqual(cmpField.GetAsBytes(), resultField.GetAsBytes());
                                    }
                                    else {
                                        if (cmpField.GetFieldType() == TIFFField.TIFF_DOUBLE) {
                                            iText.Test.TestUtil.AreEqual(cmpField.GetAsDoubles(), resultField.GetAsDoubles(), 0);
                                        }
                                        else {
                                            if (cmpField.GetFieldType() == TIFFField.TIFF_FLOAT) {
                                                iText.Test.TestUtil.AreEqual(cmpField.GetAsFloats(), resultField.GetAsFloats(), 0);
                                            }
                                            else {
                                                if (cmpField.GetFieldType() == TIFFField.TIFF_RATIONAL) {
                                                    NUnit.Framework.Assert.AreEqual(cmpField.GetAsRationals(), resultField.GetAsRationals());
                                                }
                                                else {
                                                    if (cmpField.GetFieldType() == TIFFField.TIFF_SRATIONAL) {
                                                        NUnit.Framework.Assert.AreEqual(cmpField.GetAsSRationals(), resultField.GetAsSRationals());
                                                    }
                                                    else {
                                                        if (cmpField.GetFieldType() == TIFFField.TIFF_ASCII) {
                                                            NUnit.Framework.Assert.AreEqual(cmpField.GetAsStrings(), resultField.GetAsStrings());
                                                        }
                                                        else {
                                                            NUnit.Framework.Assert.AreEqual(cmpField.GetAsBytes(), resultField.GetAsBytes());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CompareImageData(TIFFDirectory cmpDir, TIFFDirectory resultDir, byte[] cmpBytes, byte[] resultBytes
            ) {
            NUnit.Framework.Assert.IsTrue(cmpDir.IsTagPresent(TIFFConstants.TIFFTAG_STRIPOFFSETS));
            NUnit.Framework.Assert.IsTrue(cmpDir.IsTagPresent(TIFFConstants.TIFFTAG_STRIPBYTECOUNTS));
            NUnit.Framework.Assert.IsTrue(resultDir.IsTagPresent(TIFFConstants.TIFFTAG_STRIPOFFSETS));
            NUnit.Framework.Assert.IsTrue(resultDir.IsTagPresent(TIFFConstants.TIFFTAG_STRIPBYTECOUNTS));
            long[] cmpImageOffsets = cmpDir.GetField(TIFFConstants.TIFFTAG_STRIPOFFSETS).GetAsLongs();
            long[] cmpStripByteCountsArray = cmpDir.GetField(TIFFConstants.TIFFTAG_STRIPOFFSETS).GetAsLongs();
            long[] resultImageOffsets = resultDir.GetField(TIFFConstants.TIFFTAG_STRIPOFFSETS).GetAsLongs();
            long[] resultStripByteCountsArray = resultDir.GetField(TIFFConstants.TIFFTAG_STRIPOFFSETS).GetAsLongs();
            NUnit.Framework.Assert.AreEqual(cmpImageOffsets.Length, resultImageOffsets.Length);
            NUnit.Framework.Assert.AreEqual(cmpStripByteCountsArray.Length, resultStripByteCountsArray.Length);
            for (int i = 0; i < cmpImageOffsets.Length; ++i) {
                int cmpOffset = (int)cmpImageOffsets[i];
                int cmpCounts = (int)cmpStripByteCountsArray[i];
                int resultOffset = (int)resultImageOffsets[i];
                int resultCounts = (int)resultStripByteCountsArray[i];
                NUnit.Framework.Assert.AreEqual(SubArray(cmpBytes, cmpOffset, (cmpOffset + cmpCounts - 1)), SubArray(resultBytes
                    , resultOffset, (resultOffset + resultCounts - 1)));
            }
        }

        private byte[] SubArray(byte[] array, int beg, int end) {
            return JavaUtil.ArraysCopyOfRange(array, beg, end + 1);
        }

        private class ImageExtractor : IEventListener {
            private readonly IList<byte[]> images = new List<byte[]>();

            public virtual void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.RENDER_IMAGE: {
                        ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                        byte[] bytes = renderInfo.GetImage().GetImageBytes();
                        this.images.Add(bytes);
                        break;
                    }

                    default: {
                        break;
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }

            public virtual IList<byte[]> GetImages() {
                return this.images;
            }

            internal ImageExtractor(GetImageBytesTest _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly GetImageBytesTest _enclosing;
        }
    }
}
