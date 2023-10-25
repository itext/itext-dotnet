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
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfImageXObjectTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/xobject/PdfImageXObjectTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AddFlushedImageXObjectToCanvas() {
            String filename = DESTINATION_FOLDER + "addFlushedImageXObjectToCanvas.pdf";
            String cmpfile = SOURCE_FOLDER + "cmp_addFlushedImageXObjectToCanvas.pdf";
            String image = SOURCE_FOLDER + "image.png";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(image));
            // flushing pdf object directly
            imageXObject.GetPdfObject().MakeIndirect(pdfDoc).Flush();
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 200, 200));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpfile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "indexed.pdf", SOURCE_FOLDER + "cmp_indexed.pdf", SOURCE_FOLDER + "indexed.png"
                );
        }

        [NUnit.Framework.Test]
        public virtual void IndexedColorSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "indexedSimpleTransparency.pdf", SOURCE_FOLDER + "cmp_indexedSimpleTransparency.pdf"
                , SOURCE_FOLDER + "indexedSimpleTransparency.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "grayscale16Bpc.pdf", SOURCE_FOLDER + "cmp_grayscale16Bpc.pdf", SOURCE_FOLDER
                 + "grayscale16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "graya8Bpc.pdf", SOURCE_FOLDER + "cmp_graya8Bpc.pdf", SOURCE_FOLDER
                 + "graya8Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void GrayAlphaPngWithoutEmbeddedProfileImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "graya8BpcWithoutProfile.pdf", SOURCE_FOLDER + "cmp_graya8BpcWithoutProfile.pdf"
                , SOURCE_FOLDER + "graya8BpcWithoutProfile.png");
        }

        [NUnit.Framework.Test]
        public virtual void GraySimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "grayscaleSimpleTransparencyImage.pdf", SOURCE_FOLDER + "cmp_grayscaleSimpleTransparencyImage.pdf"
                , SOURCE_FOLDER + "grayscaleSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgb16Bpc.pdf", SOURCE_FOLDER + "cmp_rgb16Bpc.pdf", SOURCE_FOLDER +
                 "rgb16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbAlphaPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgba16Bpc.pdf", SOURCE_FOLDER + "cmp_rgba16Bpc.pdf", SOURCE_FOLDER
                 + "rgba16Bpc.png");
        }

        [NUnit.Framework.Test]
        public virtual void RgbSimpleTransparencyPngImageXObjectTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "rgbSimpleTransparencyImage.pdf", SOURCE_FOLDER + "cmp_rgbSimpleTransparencyImage.pdf"
                , SOURCE_FOLDER + "rgbSimpleTransparencyImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void SRgbImageTest() {
            ConvertAndCompare(DESTINATION_FOLDER + "sRGBImage.pdf", SOURCE_FOLDER + "cmp_sRGBImage.pdf", SOURCE_FOLDER
                 + "sRGBImage.png");
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompressionTiffImageTest() {
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            ConvertAndCompare(DESTINATION_FOLDER + "group3CompressionTiffImage.pdf", SOURCE_FOLDER + "cmp_group3CompressionTiffImage.pdf"
                , new PdfImageXObject(ImageDataFactory.Create(UrlUtil.ToURL(image))));
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompTiffImgRecoverErrorAndDirectTest() {
            String filename = DESTINATION_FOLDER + "group3CompTiffImgRecoverErrorAndDirect.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_group3CompTiffImgRecoverErrorAndDirect.pdf";
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            using (PdfWriter writer = CompareTool.CreateTestPdfWriter(filename)) {
                using (PdfDocument pdfDoc = new PdfDocument(writer)) {
                    PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.CreateTiff(UrlUtil.ToURL(image), true, 
                        1, true));
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                    canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 200, 200));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void Group3CompTiffImgNoRecoverErrorAndNotDirectTest() {
            String image = SOURCE_FOLDER + "group3CompressionImage.tif";
            ConvertAndCompare(DESTINATION_FOLDER + "group3CompTiffImgNoRecoverErrorAndNotDirect.pdf", SOURCE_FOLDER + 
                "cmp_group3CompTiffImgNoRecoverErrorAndNotDirect.pdf", new PdfImageXObject(ImageDataFactory.CreateTiff
                (UrlUtil.ToURL(image), false, 1, false)));
        }

        [NUnit.Framework.Test]
        public virtual void RedundantDecodeParmsTest() {
            String srcFilename = SOURCE_FOLDER + "redundantDecodeParms.pdf";
            String destFilename = DESTINATION_FOLDER + "redundantDecodeParms.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_redundantDecodeParms.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcFilename), CompareTool.CreateTestPdfWriter(destFilename
                ), new StampingProperties())) {
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, String imageFilename) {
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFilename));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpFilename) + "\n");
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFilename));
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(imageFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(50, 500, 346, imageXObject.GetHeight()));
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(CompareTool.CreateOutputReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfDocument cmpDoc = new PdfDocument(CompareTool.CreateOutputReader(cmpFilename));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }

        private void ConvertAndCompare(String outFilename, String cmpFilename, PdfImageXObject imageXObject) {
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outFilename));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpFilename) + "\n");
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFilename));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(10, 20, 575, 802));
            pdfDoc.Close();
            PdfDocument outDoc = new PdfDocument(CompareTool.CreateOutputReader(outFilename));
            PdfStream outStream = outDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            PdfDocument cmpDoc = new PdfDocument(CompareTool.CreateOutputReader(cmpFilename));
            PdfStream cmpStream = cmpDoc.GetFirstPage().GetResources().GetResource(PdfName.XObject).GetAsStream(new PdfName
                ("Im1"));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareStreamsStructure(outStream, cmpStream));
            cmpDoc.Close();
            outDoc.Close();
        }
    }
}
