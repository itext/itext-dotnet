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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImagePdfBytesInfoTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject" + "/ImagePdfBytesInfoTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/xobject" + "/ImagePdfBytesInfoTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSpace2BpcTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "indexedCalRGB2bpc.pdf"))) {
                int pngColorType = GetPngColorTypeFromObject(pdfDoc, 1, "Im1");
                NUnit.Framework.Assert.AreEqual(3, pngColorType);
            }
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSpace8BpcTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "indexedCalRGB8bpc.pdf"))) {
                int pngColorType = GetPngColorTypeFromObject(pdfDoc, 1, "Im1");
                NUnit.Framework.Assert.AreEqual(3, pngColorType);
            }
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSpace16BpcTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "indexedCalRGB16bpc.pdf"))) {
                int pngColorType = GetPngColorTypeFromObject(pdfDoc, 1, "Im1");
                NUnit.Framework.Assert.AreEqual(3, pngColorType);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CalRgb16bpcTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "calRgb16bpc.pdf"))) {
                int pngColorType = GetPngColorTypeFromObject(pdfDoc, 1, "Im0");
                NUnit.Framework.Assert.AreEqual(2, pngColorType);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CalGrayTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "calgray.pdf"))) {
                int pngColorType = GetPngColorTypeFromObject(pdfDoc, 1, "Im0");
                NUnit.Framework.Assert.AreEqual(0, pngColorType);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NegativeNTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "negativeN.pdf"))) {
                PdfImageXObject img = GetPdfImageCObject(pdfDoc, 1, "Im1");
                Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => {
                    ImagePdfBytesInfo imagePdfBytesInfo = new ImagePdfBytesInfo(img, PdfImageXObject.ImageBytesRetrievalProperties
                        .GetApplyFiltersOnly());
                    imagePdfBytesInfo.GetProcessedImageData(img.GetImageBytes());
                }
                );
                NUnit.Framework.Assert.AreEqual("N value -1 is not supported.", e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void UndefinedCSArrayTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "undefinedInCSArray.pdf"))) {
                PdfImageXObject img = GetPdfImageCObject(pdfDoc, 1, "Im1");
                Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => {
                    ImagePdfBytesInfo imagePdfBytesInfo = new ImagePdfBytesInfo(img, PdfImageXObject.ImageBytesRetrievalProperties
                        .GetApplyFiltersOnly());
                    imagePdfBytesInfo.GetProcessedImageData(img.GetImageBytes());
                }
                );
                NUnit.Framework.Assert.AreEqual("The color space /Undefined is not supported.", e.Message);
            }
        }

        private int GetPngColorTypeFromObject(PdfDocument pdfDocument, int pageNum, String objectId) {
            PdfImageXObject img = GetPdfImageCObject(pdfDocument, pageNum, objectId);
            ImagePdfBytesInfo imagePdfBytesInfo = new ImagePdfBytesInfo(img, PdfImageXObject.ImageBytesRetrievalProperties
                .GetApplyFiltersOnly());
            return imagePdfBytesInfo.GetPngColorType();
        }

        private PdfImageXObject GetPdfImageCObject(PdfDocument pdfDoc, int pageNum, String objectId) {
            PdfResources resources = pdfDoc.GetPage(pageNum).GetResources();
            PdfDictionary xobjects = resources.GetResource(PdfName.XObject);
            PdfObject obj = xobjects.Get(new PdfName(objectId));
            return new PdfImageXObject((PdfStream)obj);
        }
    }
}
