/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Codec;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    public class GetImageBytesTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/GetImageBytesTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/GetImageBytesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultiStageFilters() {
            // TODO DEVSIX-2940: extracted image is blank
            TestFile("multistagefilter1.pdf", "Obj13", "jpg");
        }

        [NUnit.Framework.Test]
        public virtual void TestAscii85Filters() {
            TestFile("ASCII85_RunLengthDecode.pdf", "Im9", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestCcittFilters() {
            TestFile("ccittfaxdecode.pdf", "background0", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateDecodeFilters() {
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
            TestFile("img_rgb_icc.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateRgb() {
            TestFile("img_rgb.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestFlateCalRgb() {
            TestFile("img_calrgb.pdf", "Im1", "png");
        }

        [NUnit.Framework.Test]
        public virtual void TestJPXDecode() {
            TestFile("JPXDecode.pdf", "Im1", "jp2");
        }

        [NUnit.Framework.Test]
        public virtual void ExtractByteAlignedG4TiffImageTest() {
            String inFileName = sourceFolder + "extractByteAlignedG4TiffImage.pdf";
            String outImageFileName = destinationFolder + "extractedByteAlignedImage.png";
            String cmpImageFileName = sourceFolder + "cmp_extractByteAlignedG4TiffImage.png";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            GetImageBytesTest.ImageExtractor listener = new GetImageBytesTest.ImageExtractor(this);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            IList<byte[]> images = listener.GetImages();
            NUnit.Framework.Assert.AreEqual(1, images.Count);
            using (FileStream fos = new FileStream(outImageFileName, FileMode.Create)) {
                fos.Write(images[0], 0, images.Count);
            }
            // expected and actual are swapped here for simplicity
            int expectedLen = images[0].Length;
            byte[] buf = new byte[expectedLen];
            using (FileStream @is = new FileStream(cmpImageFileName, FileMode.Open, FileAccess.Read)) {
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
            String inFileName = sourceFolder + "expectedByteAlignedTiffImageExtraction.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            GetImageBytesTest.ImageExtractor listener = new GetImageBytesTest.ImageExtractor(this);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.IOException), () => processor.ProcessPageContent
                (pdfDocument.GetPage(1)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(iText.IO.IOException.ExpectedTrailingZeroBitsForByteAlignedLines
                ), e.Message);
        }

        private class ImageExtractor : IEventListener {
            private IList<byte[]> images = new List<byte[]>();

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

        private void TestFile(String filename, String objectid, String expectedImageFormat) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + filename));
            try {
                PdfResources resources = pdfDocument.GetPage(1).GetResources();
                PdfDictionary xobjects = resources.GetResource(PdfName.XObject);
                PdfObject obj = xobjects.Get(new PdfName(objectid));
                if (obj == null) {
                    throw new ArgumentException("Reference " + objectid + " not found - Available keys are " + xobjects.KeySet
                        ());
                }
                PdfImageXObject img = new PdfImageXObject((PdfStream)(obj.IsIndirectReference() ? ((PdfIndirectReference)obj
                    ).GetRefersTo() : obj));
                NUnit.Framework.Assert.AreEqual(expectedImageFormat, img.IdentifyImageFileExtension());
                byte[] result = img.GetImageBytes(true);
                byte[] cmpBytes = File.ReadAllBytes(System.IO.Path.Combine(sourceFolder, filename.JSubstring(0, filename.Length
                     - 4) + "." + expectedImageFormat));
                if (img.IdentifyImageFileExtension().Equals("tif")) {
                    CompareTiffImages(cmpBytes, result);
                }
                else {
                    NUnit.Framework.Assert.AreEqual(cmpBytes, result);
                }
            }
            finally {
                pdfDocument.Close();
            }
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
                    if (tag == TIFFConstants.TIFFTAG_SOFTWARE) {
                        CompareSoftwareVersion(cmpField, resultField);
                    }
                    else {
                        CompareFields(cmpField, resultField);
                    }
                }
                CompareImageData(cmpDir, resultDir, cmpBytes, resultBytes);
            }
        }

        private void CompareSoftwareVersion(TIFFField cmpField, TIFFField resultField) {
            byte[] versionBytes = resultField.GetAsString(0).GetBytes(System.Text.Encoding.ASCII);
            //drop last always zero byte
            byte[] versionToCompare = SubArray(versionBytes, 0, versionBytes.Length - 2);
            NUnit.Framework.Assert.AreEqual(iText.Kernel.Version.GetInstance().GetVersion().GetBytes(System.Text.Encoding
                .ASCII), versionToCompare);
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
                                            NUnit.Framework.Assert.AreEqual(cmpField.GetAsDoubles(), resultField.GetAsDoubles());
                                        }
                                        else {
                                            if (cmpField.GetFieldType() == TIFFField.TIFF_FLOAT) {
                                                NUnit.Framework.Assert.AreEqual(cmpField.GetAsFloats(), resultField.GetAsFloats());
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
    }
}
