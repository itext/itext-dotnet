/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;
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

        [NUnit.Framework.Test]
        public virtual void ImageResizedParentWithHardMaskTest() {
            String outFileName = destinationFolder + "imageResizedParentWithHardMask.pdf";
            String cmpFileName = sourceFolder + "cmp_imageResizedParentWithHardMask.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
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
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(iText.IO.Exceptions.IOException.ThisImageCanNotBeAnImageMask
                ), e.Message);
        }
    }
}
