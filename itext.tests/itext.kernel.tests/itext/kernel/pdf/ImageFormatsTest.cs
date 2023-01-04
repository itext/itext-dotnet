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

        [NUnit.Framework.Test]
        public virtual void ImagesWithDifferentDepth() {
            String outFileName = destinationFolder + "transparencyTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_transparencyTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName, new WriterProperties().SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION)));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName, new WriterProperties().SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION)));
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
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName, new WriterProperties().SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION)));
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
