/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageDecompressionBombTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/ImageDecompressionBombTest/";

        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/ImageDecompressionBombTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static ICollection<Object[]> BombImagesSource() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "10K.png" }, new Object[] { "10K.jpeg" }, new 
                Object[] { "10K.j2k" }, new Object[] { "10K.tiff" }, new Object[] { "10K.gif" } });
        }

        public static ICollection<Object[]> LargeHeaderSmallDataSource() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "largeHeaderSmallData.jp2" }, new Object[] { 
                "largeHeaderSmallData.jpeg" }, new Object[] { "largeHeaderSmallData.png" } });
        }

        public static ICollection<Object[]> SmallHeaderLargeDataSource() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "smallHeaderLargeData.png" }, new Object[] { 
                "smallHeaderLargeData.jpeg" }, new Object[] { "smallHeaderLargeData.j2k" }, new Object[] { "smallHeaderLargeData.tiff"
                 }, new Object[] { "smallHeaderLargeData.gif" } });
        }

        [NUnit.Framework.TestCaseSource("BombImagesSource")]
        public virtual void BombImagesTest(String fileName) {
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => ProcessImage(fileName));
        }

        [NUnit.Framework.TestCaseSource("LargeHeaderSmallDataSource")]
        public virtual void LargeHeaderSmallDataImagesTest(String fileName) {
            // This is done as a separate test to showcase that we don't have another simple way to catch
            // decompression bombs rather than the one shown in processImage.
            // Images tested here can be read without OOM, but we still throw and can't distinguish them from the ones
            // tested in bombImagesTest.
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => ProcessImage(fileName));
        }

        [NUnit.Framework.Ignore("DEVSIX-9835: OutOfMemoryError when processing PNG images with small reported dimensions but large actual data"
            )]
        [NUnit.Framework.TestCaseSource("SmallHeaderLargeDataSource")]
        public virtual void SmallHeaderLargeDataImagesTest(String fileName) {
            NUnit.Framework.Assert.DoesNotThrow(() => ProcessImage(fileName));
        }

        [NUnit.Framework.Ignore("DEVSIX-9835: OutOfMemoryError when processing PNG images with small reported dimensions but large actual data"
            )]
        [NUnit.Framework.TestCaseSource("BombImagesSource")]
        public virtual void EmbeddedBombImageBytesFromPdfTest(String fileName) {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                String pdfPath = CreatePdfWithImage(fileName);
                byte[] bytes = ReadEmbeddedImageBytes(pdfPath);
                NUnit.Framework.Assert.IsNotNull(bytes);
                NUnit.Framework.Assert.IsTrue(bytes.Length > 0);
            }
            );
        }

        private void ProcessImage(String fileName) {
            ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + fileName);
            PdfImageXObject xObject = new PdfImageXObject(imageData);
            long width = (long)imageData.GetWidth();
            long height = (long)imageData.GetHeight();
            long pixels = width * height;
            if (pixels > 2_000_000L) {
                throw new System.IO.IOException("Image is too large to be processed safely: " + pixels + " pixels");
            }
            // It really fails only for png and tiff
            xObject.GetImageBytes();
        }

        private String CreatePdfWithImage(String fileName) {
            String pdfPath = DESTINATION_FOLDER + fileName + ".pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(pdfPath))) {
                PdfPage page = pdfDocument.AddNewPage();
                ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + fileName);
                PdfImageXObject imageXObject = new PdfImageXObject(imageData);
                new PdfCanvas(page).AddXObject(imageXObject);
            }
            return pdfPath;
        }

        private byte[] ReadEmbeddedImageBytes(String pdfPath) {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath))) {
                PdfDictionary xObjects = pdfDocument.GetFirstPage().GetResources().GetResource(PdfName.XObject);
                NUnit.Framework.Assert.IsNotNull(xObjects, "No XObject resources found in PDF");
                foreach (PdfObject xObject in xObjects.Values()) {
                    if (xObject is PdfStream) {
                        PdfStream stream = (PdfStream)xObject;
                        if (PdfName.Image.Equals(stream.GetAsName(PdfName.Subtype))) {
                            return new PdfImageXObject(stream).GetImageBytes();
                        }
                    }
                }
            }
            NUnit.Framework.Assert.Fail("No image XObject found in PDF");
            return null;
        }
    }
}
