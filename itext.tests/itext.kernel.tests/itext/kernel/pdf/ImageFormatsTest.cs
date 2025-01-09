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
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ImageFormatsTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/ImageFormatsTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/ImageFormatsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ImagesWithDifferentDepth() {
            String outFileName = destinationFolder + "transparencyTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_transparencyTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties
                ().SetCompressionLevel(CompressionConstants.NO_COMPRESSION)));
            PdfPage page = pdfDocument.AddNewPage(PageSize.A3);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY).Fill();
            canvas.Rectangle(80, 0, 700, 1200).Fill();
            canvas.SaveState().BeginText().MoveText(116, 1150).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                HELVETICA), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("8 bit depth PNG").EndText().RestoreState
                ();
            ImageData img = ImageDataFactory.Create(sourceFolder + "manualTransparency_8bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 780, 200, 292.59f), false);
            canvas.SaveState().BeginText().MoveText(316, 1150).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                HELVETICA), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("24 bit depth PNG").EndText().RestoreState
                ();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_24bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(300, 780, 200, 292.59f), false);
            canvas.SaveState().BeginText().MoveText(516, 1150).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                HELVETICA), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("32 bit depth PNG").EndText().RestoreState
                ();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_32bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(500, 780, 200, 292.59f), false);
            canvas.SaveState().BeginText().MoveText(116, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).SetFillColor(ColorConstants.MAGENTA).ShowText("GIF image ").EndText().RestoreState();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_gif.gif");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 300, 200, 292.59f), false);
            canvas.SaveState().BeginText().MoveText(316, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).SetFillColor(ColorConstants.MAGENTA).ShowText("TIF image ").EndText().RestoreState();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_tif.tif");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(300, 300, 200, 292.59f), false);
            canvas.Release();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Png_imageTransparency_8bitDepthImage() {
            String outFileName = destinationFolder + "png_imageTransparancy_8bitDepthImage.pdf";
            String cmpFileName = sourceFolder + "cmp_png_imageTransparancy_8bitDepthImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties
                ().SetCompressionLevel(CompressionConstants.NO_COMPRESSION)));
            PdfPage page = pdfDocument.AddNewPage(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY).Fill();
            canvas.Rectangle(80, 0, PageSize.A4.GetWidth() - 80, PageSize.A4.GetHeight()).Fill();
            canvas.SaveState().BeginText().MoveText(116, 800).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("8 bit depth PNG").MoveText(0, -20).ShowText("This image should not have a black rectangle as background"
                ).EndText().RestoreState();
            ImageData img = ImageDataFactory.Create(sourceFolder + "manualTransparency_8bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 450, 200, 292.59f), false);
            canvas.Release();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Png_imageTransparency_24bitDepthImage() {
            String outFileName = destinationFolder + "png_imageTransparancy_24bitDepthImage.pdf";
            String cmpFileName = sourceFolder + "cmp_png_imageTransparancy_24bitDepthImage.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties
                ().SetCompressionLevel(CompressionConstants.NO_COMPRESSION)));
            PdfPage page = pdfDocument.AddNewPage(PageSize.A4);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY).Fill();
            canvas.Rectangle(80, 0, PageSize.A4.GetWidth() - 80, PageSize.A4.GetHeight()).Fill();
            canvas.SaveState().BeginText().MoveText(116, 800).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("24 bit depth PNG").MoveText(0, -20).ShowText("This image should not have a white rectangle as background"
                ).EndText().RestoreState();
            ImageData img = ImageDataFactory.Create(sourceFolder + "manualTransparency_24bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(100, 450, 200, 292.59f), false);
            canvas.SaveState().BeginText().MoveText(116, 400).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("32 bit depth PNG").EndText().RestoreState();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_32bit.png");
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(116, 100, 200, 292.59f), false);
            canvas.Release();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }
    }
}
