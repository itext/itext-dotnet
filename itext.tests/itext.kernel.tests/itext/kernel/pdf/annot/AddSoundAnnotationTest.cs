using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddSoundAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddSoundAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/AddSoundAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SoundTestAif() {
            String filename = destinationFolder + "soundAnnotation02.pdf";
            String audioFile = sourceFolder + "sample.aif";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String @string = "";
            for (int i = 0; i < 4; i++) {
                @string = @string + (char)@is.Read();
            }
            if (@string.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream sound1 = new PdfStream(pdfDoc, @is);
            sound1.Put(PdfName.R, new PdfNumber(32117));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), sound1);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SoundTestAiff() {
            String filename = destinationFolder + "soundAnnotation03.pdf";
            String audioFile = sourceFolder + "sample.aiff";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String @string = "";
            for (int i = 0; i < 4; i++) {
                @string = @string + (char)@is.Read();
            }
            if (@string.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream sound1 = new PdfStream(pdfDoc, @is);
            sound1.Put(PdfName.R, new PdfNumber(44100));
            sound1.Put(PdfName.E, PdfName.Signed);
            sound1.Put(PdfName.B, new PdfNumber(16));
            sound1.Put(PdfName.C, new PdfNumber(1));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), sound1);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation03.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SoundTestSnd() {
            String filename = destinationFolder + "soundAnnotation04.pdf";
            String audioFile = sourceFolder + "sample.snd";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            PdfSoundAnnotation sound = new PdfSoundAnnotation(pdfDoc, new Rectangle(100, 100, 100, 100), @is, 44100, PdfName
                .Signed, 2, 16);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation04.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SoundTestWav() {
            String filename = destinationFolder + "soundAnnotation01.pdf";
            String audioFile = sourceFolder + "sample.wav";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            PdfSoundAnnotation sound = new PdfSoundAnnotation(pdfDoc, new Rectangle(100, 100, 100, 100), @is, 48000, PdfName
                .Signed, 2, 16);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SoundTestWav01() {
            String filename = destinationFolder + "soundAnnotation05.pdf";
            String audioFile = sourceFolder + "sample.wav";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfPage page1 = pdfDoc.AddNewPage();
            Stream @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            String header = "";
            for (int i = 0; i < 4; i++) {
                header = header + (char)@is.Read();
            }
            if (header.Equals("RIFF")) {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
                @is.Read();
            }
            else {
                @is = new FileStream(audioFile, FileMode.Open, FileAccess.Read);
            }
            PdfStream soundStream = new PdfStream(pdfDoc, @is);
            soundStream.Put(PdfName.R, new PdfNumber(48000));
            soundStream.Put(PdfName.E, PdfName.Signed);
            soundStream.Put(PdfName.B, new PdfNumber(16));
            soundStream.Put(PdfName.C, new PdfNumber(2));
            PdfSoundAnnotation sound = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), soundStream);
            page1.AddAnnotation(sound);
            page1.Flush();
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(filename, sourceFolder + "cmp_soundAnnotation05.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
