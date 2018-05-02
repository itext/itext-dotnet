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
    public class ImageFormatsTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/ImageFormatsTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/ImageFormatsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ImagesWithDifferentDepth() {
            //TODO: update after DEVSIX-1934 ticket will be fixed
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
            canvas.AddImage(img, 100, 780, 200, false);
            canvas.SaveState().BeginText().MoveText(316, 1150).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                HELVETICA), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("24 bit depth PNG").EndText().RestoreState
                ();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_24bit.png");
            canvas.AddImage(img, 300, 780, 200, false);
            canvas.SaveState().BeginText().MoveText(516, 1150).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.
                HELVETICA), 14).SetFillColor(ColorConstants.MAGENTA).ShowText("32 bit depth PNG").EndText().RestoreState
                ();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_32bit.png");
            canvas.AddImage(img, 500, 780, 200, false);
            canvas.SaveState().BeginText().MoveText(116, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).SetFillColor(ColorConstants.MAGENTA).ShowText("GIF image ").EndText().RestoreState();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_gif.gif");
            canvas.AddImage(img, 100, 300, 200, false);
            canvas.SaveState().BeginText().MoveText(316, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).SetFillColor(ColorConstants.MAGENTA).ShowText("TIF image ").EndText().RestoreState();
            img = ImageDataFactory.Create(sourceFolder + "manualTransparency_tif.tif");
            canvas.AddImage(img, 300, 300, 200, false);
            canvas.Release();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }
    }
}
