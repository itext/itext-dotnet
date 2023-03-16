using System;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddScreenAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddScreenAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/AddScreenAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ScreenTestExternalWavFile() {
            String filename = destinationFolder + "screenAnnotation01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            FileStream fos = new FileStream(destinationFolder + "sample.wav", FileMode.Create);
            FileStream fis = new FileStream(sourceFolder + "sample.wav", FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024];
            int length;
            while ((length = fis.Read(buffer)) > 0) {
                fos.Write(buffer, 0, length);
            }
            fos.Dispose();
            fis.Dispose();
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
            PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_screenAnnotation01.pdf", 
                destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile01() {
            String filename = destinationFolder + "screenAnnotation02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                , null, null);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }

        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation02.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile02() {
            String filename = destinationFolder + "screenAnnotation03.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, new FileStream(sourceFolder + "sample.wav", 
                FileMode.Open, FileAccess.Read), null, "sample.wav", null, null);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }

        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation03.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
        [NUnit.Framework.Test]
        public virtual void ScreenTestEmbeddedWavFile03() {
            String filename = destinationFolder + "screenAnnotation04.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            Stream @is = new FileStream(sourceFolder + "sample.wav", FileMode.Open, FileAccess.Read);
            MemoryStream baos = new MemoryStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, baos.ToArray(), null, "sample.wav", null, null
                , null);
            PdfAction action = PdfAction.CreateRendition(sourceFolder + "sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
            pdfDoc.Close();
        }
        //        CompareTool compareTool = new CompareTool();
        //        String errorMessage = compareTool.compareByContent(filename, sourceFolder + "cmp_screenAnnotation04.pdf", destinationFolder, "diff_");
        //        if (errorMessage != null) {
        //            Assert.fail(errorMessage);
        //        }
    }
}
