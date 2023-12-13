/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfImageXObjectTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddFlushedImageXObjectToCanvas() {
            String filename = destinationFolder + "addFlushedImageXObjectToCanvas.pdf";
            String cmpfile = sourceFolder + "cmp_addFlushedImageXObjectToCanvas.pdf";
            String image = sourceFolder + "image.png";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(image));
            // flushing pdf object directly
            imageXObject.GetPdfObject().MakeIndirect(pdfDoc).Flush();
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObject(imageXObject, 50, 500, 200);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpfile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "indexed.pdf", sourceFolder + "cmp_indexed.pdf", sourceFolder + "indexed.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "indexedSimpleTransparency.pdf", sourceFolder + "cmp_indexedSimpleTransparency.pdf"
                , sourceFolder + "indexedSimpleTransparency.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "grayscale16Bpc.pdf", sourceFolder + "cmp_grayscale16Bpc.pdf", sourceFolder
                 + "grayscale16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "graya8Bpc.pdf", sourceFolder + "cmp_graya8Bpc.pdf", sourceFolder + 
                "graya8Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngWithoutEmbeddedProfileImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "graya8BpcWithoutProfile.pdf", sourceFolder + "cmp_graya8BpcWithoutProfile.pdf"
                , sourceFolder + "graya8BpcWithoutProfile.png");
        }

        [NUnit.Framework.Test]
        public virtual void GraySimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "grayscaleSimpleTransparencyImage.pdf", sourceFolder + "cmp_grayscaleSimpleTransparencyImage.pdf"
                , sourceFolder + "grayscaleSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgb16Bpc.pdf", sourceFolder + "cmp_rgb16Bpc.pdf", sourceFolder + "rgb16Bpc.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RgbAlphaPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgba16Bpc.pdf", sourceFolder + "cmp_rgba16Bpc.pdf", sourceFolder + 
                "rgba16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(destinationFolder + "rgbSimpleTransparencyImage.pdf", sourceFolder + "cmp_rgbSimpleTransparencyImage.pdf"
                , sourceFolder + "rgbSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void SRgbImageTest() {
            ConvertAndCompare(destinationFolder + "sRGBImage.pdf", sourceFolder + "cmp_sRGBImage.pdf", sourceFolder + 
                "sRGBImage.png");
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, String imageFilename) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFilename));
            PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFilename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(imageFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObject(imageXObject, 50, 500, 346);
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(new PdfReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }
    }
}
