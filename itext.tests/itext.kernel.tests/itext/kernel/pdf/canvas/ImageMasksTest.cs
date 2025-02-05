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
using iText.IO.Exceptions;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageMasksTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/ImageMasksTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/ImageMasksTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImageResizedParentWithHardMaskTest() {
            String outFileName = destinationFolder + "imageResizedParentWithHardMask.pdf";
            String cmpFileName = sourceFolder + "cmp_imageResizedParentWithHardMask.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            ImageData mask = ImageDataFactory.Create(sourceFolder + "hardMask.png");
            mask.MakeMask();
            ImageData img1 = ImageDataFactory.Create(sourceFolder + "sRGBImageBig.png");
            img1.SetImageMask(mask);
            ImageData img2 = ImageDataFactory.Create(sourceFolder + "sRGBImage.png");
            img2.SetImageMask(mask);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddImageAt(img1, 30, 500, false);
            canvas.AddImageAt(img2, 430, 500, false);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DiffMasksOnSameImageXObjectTest() {
            // TODO: DEVSIX-4992
            String outFileName = destinationFolder + "diffMasksOnSameImageXObject.pdf";
            String cmpFileName = sourceFolder + "cmp_diffMasksOnSameImageXObject.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            ImageData sMask = ImageDataFactory.Create(sourceFolder + "SMask3px.jpg");
            sMask.MakeMask();
            ImageData hardMask = ImageDataFactory.Create(sourceFolder + "hardMask.png");
            hardMask.MakeMask();
            PdfImageXObject hardMaskXObject = new PdfImageXObject(hardMask);
            ImageData img = ImageDataFactory.Create(sourceFolder + "sRGBImageBig.png");
            img.SetImageMask(sMask);
            PdfImageXObject hardXObject = new PdfImageXObject(img, hardMaskXObject);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectAt(hardXObject, 300, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ImageResizedParentWithSoftMaskTest() {
            String outFileName = destinationFolder + "imageResizedParentWithSoftMask.pdf";
            String cmpFileName = sourceFolder + "cmp_imageResizedParentWithSoftMask.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            ImageData mask = ImageDataFactory.Create(sourceFolder + "SMask3px.jpg");
            mask.MakeMask();
            ImageData img1 = ImageDataFactory.Create(sourceFolder + "sRGBImageBig.png");
            img1.SetImageMask(mask);
            ImageData img2 = ImageDataFactory.Create(sourceFolder + "sRGBImage.png");
            img2.SetImageMask(mask);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddImageAt(img1, 30, 500, false);
            canvas.AddImageAt(img2, 430, 500, false);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithSoftMaskMatteTest() {
            String outFileName = destinationFolder + "imageWithSoftMaskMatte.pdf";
            String cmpFileName = sourceFolder + "cmp_imageWithSoftMaskMatte.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            ImageData mask = ImageDataFactory.Create(sourceFolder + "matteMask.jpg");
            mask.MakeMask();
            PdfImageXObject maskXObject = new PdfImageXObject(mask);
            maskXObject.Put(new PdfName("Matte"), new PdfArray(new float[] { 1, 1, 1 }));
            ImageData img1 = ImageDataFactory.Create(sourceFolder + "imageForMatteMask.jpg");
            PdfImageXObject xObject = new PdfImageXObject(img1, maskXObject);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage(PageSize.A4));
            canvas.AddXObjectAt(xObject, 50, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SMaskMatteDifferentSizeOfImgTest() {
            // TODO: DEVSIX-4991
            String outFileName = destinationFolder + "sMaskMatteDifferentSizeOfImg.pdf";
            String cmpFileName = sourceFolder + "cmp_sMaskMatteDifferentSizeOfImg.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName));
            ImageData mask = ImageDataFactory.Create(sourceFolder + "matteMask.jpg");
            mask.MakeMask();
            PdfImageXObject maskXObject = new PdfImageXObject(mask);
            maskXObject.Put(new PdfName("Matte"), new PdfArray(new float[] { 1, 1, 1 }));
            ImageData img1 = ImageDataFactory.Create(sourceFolder + "resizedImageForMatteMask.jpg");
            PdfImageXObject xObject = new PdfImageXObject(img1, maskXObject);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage(PageSize.A4));
            canvas.AddXObjectAt(xObject, 50, 500);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithInvalidMaskTest() {
            ImageData mask = ImageDataFactory.Create(sourceFolder + "mask.png");
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => mask.MakeMask());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.THIS_IMAGE_CAN_NOT_BE_AN_IMAGE_MASK
                ), e.Message);
        }
    }
}
