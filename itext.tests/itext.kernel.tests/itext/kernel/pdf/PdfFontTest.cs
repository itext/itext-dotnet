using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfFontTest : ExtendedITextTest {
        public const int PageCount = 1;

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfFontTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfFontTest/";

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 6";

        internal const String pangramme = "Amazingly few discothegues provide jukeboxes" + "but it now while sayingly ABEFGHJKNOPQRSTUWYZ?";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithKozmin() {
            String filename = destinationFolder + "DocumentWithKozmin.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozmin.pdf";
            String title = "Type 0 test";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont type0Font = PdfFontFactory.CreateFont("KozMinPro-Regular", "UniJIS-UCS2-H");
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "Type0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is CidFont, "CidFont expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithStSongUni() {
            String filename = destinationFolder + "DocumentWithStSongUni.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithStSongUni.pdf";
            String title = "Type0 test";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                ));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont type0Font = PdfFontFactory.CreateFont("STSong-Light", "UniGB-UTF16-H");
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "Type0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is CidFont, "CidFont expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithStSong() {
            String filename = destinationFolder + "DocumentWithStSong.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithStSong.pdf";
            String title = "Type0 test";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                ));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont type0Font = PdfFontFactory.CreateFont("STSong-Light", "Adobe-GB1-4");
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "Type0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is CidFont, "CidFont expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0() {
            String filename = destinationFolder + "DocumentWithTrueTypeAsType0.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeAsType0.pdf";
            String title = "Type0 test";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFont type0Font = PdfFontFactory.CreateFont(font, "Identity-H");
            //        type0Font.setSubset(false);
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "PdfType0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is TrueTypeFont, "TrueType expected");
            PdfPage page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 72).ShowText("Hello World"
                ).EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            //        new PdfCanvas(page)
            //                .saveState()
            //                .beginText()
            //                .moveText(36, 650)
            //                .setFontAndSize(type0Font, 12)
            //                .showText(pangramme)
            //                .endText()
            //                .restoreState()
            //                .release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            type0Font = PdfFontFactory.CreateFont(ttf, "Identity-H");
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "PdfType0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is TrueTypeFont, "TrueType expected");
            page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 72).ShowText("Hello World"
                ).EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType3Font() {
            String filename = destinationFolder + "DocumentWithType3Font.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithType3Font.pdf";
            String testString = "A A A A E E E ~ \u00E9";
            // A A A A E E E ~ é
            //writing type3 font characters
            String title = "Type3 font iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfType3Font type3 = PdfFontFactory.CreateType3Font(pdfDoc, false);
            Type3Glyph a = type3.AddGlyph('A', 600, 0, 0, 600, 700);
            a.SetLineWidth(100);
            a.MoveTo(5, 5);
            a.LineTo(300, 695);
            a.LineTo(595, 5);
            a.ClosePathFillStroke();
            Type3Glyph space = type3.AddGlyph(' ', 600, 0, 0, 600, 700);
            space.SetLineWidth(10);
            space.ClosePathFillStroke();
            Type3Glyph e = type3.AddGlyph('E', 600, 0, 0, 600, 700);
            e.SetLineWidth(100);
            e.MoveTo(595, 5);
            e.LineTo(5, 5);
            e.LineTo(300, 350);
            e.LineTo(5, 695);
            e.LineTo(595, 695);
            e.Stroke();
            Type3Glyph tilde = type3.AddGlyph('~', 600, 0, 0, 600, 700);
            tilde.SetLineWidth(100);
            tilde.MoveTo(595, 5);
            tilde.LineTo(5, 5);
            tilde.Stroke();
            Type3Glyph symbol233 = type3.AddGlyph('\u00E9', 600, 0, 0, 600, 700);
            symbol233.SetLineWidth(100);
            symbol233.MoveTo(540, 5);
            symbol233.LineTo(5, 340);
            symbol233.Stroke();
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().SetFontAndSize(type3, 12).MoveText(50, 800).ShowText(testString).EndText();
                page.Flush();
            }
            pdfDoc.Close();
            // reading and comparing text
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelvetica() {
            String filename = destinationFolder + "DocumentWithHelvetica.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithHelvetica.pdf";
            String title = "Type3 test";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            PdfFont pdfFont = PdfFontFactory.CreateFont(FontConstants.HELVETICA);
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType1Font, "PdfType1Font expected");
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaOblique() {
            String filename = destinationFolder + "DocumentWithHelveticaOblique.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithHelveticaOblique.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(FontConstants.HELVETICA_OBLIQUE);
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType1Font, "PdfType1Font expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaBoldOblique() {
            String filename = destinationFolder + "DocumentWithHelveticaBoldOblique.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithHelveticaBoldOblique.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(FontConstants.HELVETICA_BOLDOBLIQUE);
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType1Font, "PdfType1Font expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithCourierBold() {
            String filename = destinationFolder + "DocumentWithCourierBold.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCourierBold.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(FontConstants.COURIER_BOLD);
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType1Font, "PdfType1Font expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello World").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1FontAfm() {
            String filename = destinationFolder + "DocumentWithCMR10Afm.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(fontsFolder + "cmr10.afm"
                , fontsFolder + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, true);
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                ));
            pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(afm, pfb), FontEncoding.FONT_SPECIFIC
                , true);
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1FontPfm() {
            String filename = destinationFolder + "DocumentWithCMR10Pfm.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Pfm.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(fontsFolder + "cmr10.pfm"
                , fontsFolder + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 72).ShowText("Hello world").
                EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeFont1() {
            String filename = destinationFolder + "DocumentWithTrueTypeFont1.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont1.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, true);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateFont(ttf, true);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeOtfFont() {
            String filename = destinationFolder + "DocumentWithTrueTypeOtfFont.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeOtfFont.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "Puritan2.otf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, true);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateFont(ttf, true);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType0OtfFont() {
            String filename = destinationFolder + "DocumentWithType0OtfFont.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithType0OtfFont.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "Puritan2.otf";
            PdfFont pdfFont = PdfFontFactory.CreateFont(font, "Identity-H");
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType0Font, "PdfType0Font expected");
            pdfFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello world").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfFont = PdfFontFactory.CreateFont(ttf, "Identity-H");
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType0Font, "PdfTrueTypeFont expected");
            pdfFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("Hello world").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestUpdateType3FontBasedExistingFont() {
            String inputFileName = sourceFolder + "type3Font.pdf";
            String outputFileName = destinationFolder + "type3Font_update.pdf";
            String cmpOutputFileName = sourceFolder + "cmp_type3Font_update.pdf";
            String title = "Type3 font iText 6 Document";
            PdfReader reader = new PdfReader(inputFileName);
            PdfWriter writer = new PdfWriter(outputFileName);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdfDoc.GetPdfObject(4));
            Type3Glyph newGlyph = pdfType3Font.AddGlyph('\u00F6', 600, 0, 0, 600, 700);
            newGlyph.SetLineWidth(100);
            newGlyph.MoveTo(540, 5);
            newGlyph.LineTo(5, 840);
            newGlyph.Stroke();
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().SetFontAndSize(pdfType3Font, 12).MoveText(50, 800).ShowText("AAAAAA EEEE ~ \u00E9 \u00F6"
                ).EndText();
            // é ö
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(6, ((Type3FontProgram)pdfType3Font.GetFontProgram()).GetGlyphsCount());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpOutputFileName, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNewType3FontBasedExistingFont() {
            String inputFileName = sourceFolder + "type3Font.pdf";
            String outputFileName = destinationFolder + "type3Font_new.pdf";
            String cmpOutputFileName = sourceFolder + "cmp_type3Font_new.pdf";
            String title = "Type3 font iText 6 Document";
            PdfReader reader = new PdfReader(inputFileName);
            PdfWriter pdfWriter = new PdfWriter(outputFileName);
            pdfWriter.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument inputPdfDoc = new PdfDocument(reader);
            PdfDocument outputPdfDoc = new PdfDocument(pdfWriter);
            outputPdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfDictionary pdfType3FontDict = (PdfDictionary)inputPdfDoc.GetPdfObject(4);
            PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont(((PdfDictionary)pdfType3FontDict.CopyTo
                (outputPdfDoc)));
            Type3Glyph newGlyph = pdfType3Font.AddGlyph('\u00F6', 600, 0, 0, 600, 700);
            newGlyph.SetLineWidth(100);
            newGlyph.MoveTo(540, 5);
            newGlyph.LineTo(5, 840);
            newGlyph.Stroke();
            PdfPage page = outputPdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().SetFontAndSize(pdfType3Font, 12).MoveText(50, 800).ShowText("AAAAAA EEEE ~ \u00E9 \u00F6"
                ).EndText();
            // é ö
            page.Flush();
            outputPdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(6, ((Type3FontProgram)pdfType3Font.GetFontProgram()).GetGlyphsCount());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpOutputFileName, destinationFolder
                , "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNewType1FontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithCMR10Afm.pdf";
            String filename = destinationFolder + "DocumentWithCMR10Afm_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm_new.pdf";
            String title = "Type 1 font iText 6 Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNewTrueTypeFont1BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeFont1.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeFont1_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont1_new.pdf";
            String title = "testNewTrueTypeFont1BasedExistingFont";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNewTrueTypeFont2BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeFont2.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeFont2_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont2_new.pdf";
            String title = "True Type font iText 6 Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = inputPdfDoc1.GetFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("New Hello world").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestTrueTypeFont1BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeFont1.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeFont1_updated.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont1_updated.pdf";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader1, writer);
            PdfDictionary pdfDictionary = (PdfDictionary)pdfDoc.GetPdfObject(4);
            PdfFont pdfFont = PdfFontFactory.CreateFont(pdfDictionary);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfFont, 72).ShowText("New Hello world").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestUpdateCjkFontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithKozmin.pdf";
            String filename = destinationFolder + "DocumentWithKozmin_update.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozmin_update.pdf";
            String title = "Type0 font iText 6 Document";
            PdfReader reader = new PdfReader(inputFileName1);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfDictionary pdfDictionary = (PdfDictionary)pdfDoc.GetPdfObject(6);
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(pdfDictionary);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNewCjkFontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithKozmin.pdf";
            String filename = destinationFolder + "DocumentWithKozmin_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozmin_new.pdf";
            String title = "Type0 font iText 6 Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(6);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeAsType0.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeAsType0_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeAsType0_new.pdf";
            String title = "Type0 font iText 6 Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(6);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("New Hello World"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateUpdatedDocumentWithTrueTypeAsType0BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeAsType0.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeAsType0_update.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeAsType0_update.pdf";
            String title = "Type0 font iText 6 Document";
            PdfReader reader = new PdfReader(inputFileName1);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfTrueTypeFont = pdfDoc.GetFont((PdfDictionary)pdfDoc.GetPdfObject(6));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("New Hello World"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1WithToUnicodeBasedExistingFont() {
            String inputFileName1 = sourceFolder + "fontWithToUnicode.pdf";
            String filename = destinationFolder + "fontWithToUnicode_new.pdf";
            String cmpFilename = sourceFolder + "cmp_fontWithToUnicode_new.pdf";
            String title = "Type1 font iText 6 Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = inputPdfDoc1.GetFont(((PdfDictionary)pdfDictionary.CopyTo(pdfDoc)));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 756).SetFontAndSize(pdfType1Font, 10).ShowText("New MyriadPro-Bold font."
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestType1FontUpdateContent() {
            String inputFileName1 = sourceFolder + "DocumentWithCMR10Afm.pdf";
            String filename = destinationFolder + "DocumentWithCMR10Afm_updated.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm_updated.pdf";
            PdfReader reader = new PdfReader(inputFileName1);
            PdfWriter writer = new PdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfDictionary pdfDictionary = (PdfDictionary)pdfDoc.GetPdfObject(4);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(pdfDictionary);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestType1FontUpdateContent2() {
            String inputFileName1 = sourceFolder + "DocumentWithCMR10Afm.pdf";
            String filename = destinationFolder + "DocumentWithCMR10Afm2_updated.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm2_updated.pdf";
            PdfReader reader = new PdfReader(inputFileName1);
            PdfWriter writer = new PdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfDictionary pdfDictionary = (PdfDictionary)pdfDoc.GetPdfObject(4);
            PdfFont pdfType1Font = pdfDoc.GetFont(pdfDictionary);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 72).ShowText("New Hello world"
                ).EndText().RestoreState();
            PdfFont pdfType1Font2 = pdfDoc.GetFont(pdfDictionary);
            NUnit.Framework.Assert.AreEqual(pdfType1Font, pdfType1Font2);
            canvas.SaveState().BeginText().MoveText(36, 620).SetFontAndSize(pdfType1Font2, 72).ShowText("New Hello world2"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateWrongAfm1() {
            String message = "";
            try {
                byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                    ));
                FontProgramFactory.CreateType1Font(null, pfb);
            }
            catch (iText.IO.IOException e) {
                message = e.Message;
            }
            NUnit.Framework.Assert.AreEqual("invalid.afm.or.pfm.font.file", message);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateWrongAfm2() {
            String message = "";
            String font = fontsFolder + "cmr10.pfb";
            try {
                FontProgramFactory.CreateType1Font(font, null);
            }
            catch (iText.IO.IOException e) {
                message = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(String.Format(iText.IO.IOException._1IsNotAnAfmOrPfmFontFile, font), message
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.START_MARKER_MISSING_IN_PFB_FILE)]
        public virtual void CreateWrongPfb() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(afm, afm, false), null);
            byte[] streamContent = ((Type1Font)((PdfType1Font)font).GetFontProgram()).GetFontStreamBytes();
            NUnit.Framework.Assert.IsTrue(streamContent == null, "Empty stream content expected");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoDetect1() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(afm, false) is Type1Font, "Type1 font expected"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoDetect2() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateType1Font(afm, pfb) is Type1Font, "Type1 font expected"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoDetect3() {
            byte[] otf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "Puritan2.otf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(otf) is TrueTypeFont, "TrueType (OTF) font expected"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoDetect4() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "abserif4_5.ttf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(ttf) is TrueTypeFont, "TrueType (TTF) expected"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AutoDetect5() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "abserif4_5.ttf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(ttf) is TrueTypeFont, "TrueType (TTF) expected"
                );
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestPdfFontFactoryTtc() {
            String filename = destinationFolder + "testPdfFontFactoryTtc.pdf";
            String cmpFilename = sourceFolder + "cmp_testPdfFontFactoryTtc.pdf";
            String txt = "The quick brown fox";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "uming.ttc,1");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 680).SetFontAndSize(font, 12).ShowText(txt).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestWriteTTC() {
            String filename = destinationFolder + "DocumentWithTTC.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTTC.pdf";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "uming.ttc";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(font, 0, PdfEncodings.WINANSI, true, false);
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            byte[] ttc = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(ttc, 1, PdfEncodings.WINANSI, true, false);
            pdfTrueTypeFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestNotoFont() {
            String filename = destinationFolder + "testNotoFont.pdf";
            String cmpFilename = sourceFolder + "cmp_testNotoFont.pdf";
            String japanese = "\u713C";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H", true);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 680).SetFontAndSize(font, 12).ShowText(japanese).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("Invalid subset")]
        public virtual void NotoSansCJKjpTest() {
            String filename = destinationFolder + "NotoSansCJKjpTest.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTTC.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            // font.setSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            //        canvas.saveState()
            //                .setFillColor(DeviceRgb.GREEN)
            //                .beginText()
            //                .moveText(36, 700)
            //                .setFontAndSize(font, 12)
            //                .showText(pangramme)
            //                .endText()
            //                .restoreState();
            canvas.SaveState().SetFillColor(DeviceRgb.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12).ShowText
                ("1").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestCheckTTCSize() {
            TrueTypeCollection collection = new TrueTypeCollection(fontsFolder + "uming.ttc");
            NUnit.Framework.Assert.IsTrue(collection.GetTTCSize() == 4);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestFontDirectoryRegister() {
            PdfFontFactory.RegisterDirectory(sourceFolder);
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            foreach (String name in PdfFontFactory.GetRegisteredFonts()) {
                PdfFont pdfFont = PdfFontFactory.CreateRegisteredFont(name);
                if (pdfFont == null) {
                    NUnit.Framework.Assert.IsTrue(false, "Font {" + name + "} can't be empty");
                }
            }
            pdfDoc.AddNewPage();
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestFontRegister() {
            FontProgramFactory.RegisterFont(fontsFolder + "Aller_Rg.ttf", "aller");
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont pdfFont = PdfFontFactory.CreateRegisteredFont("aller");
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfTrueTypeFont);
            pdfDoc.AddNewPage();
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestSplitString() {
            PdfFont font = PdfFontFactory.CreateFont();
            IList<String> list1 = font.SplitString("Hello", 12f, 10);
            NUnit.Framework.Assert.IsTrue(list1.Count == 2);
            IList<String> list2 = font.SplitString("Digitally signed by Dmitry Trusevich\nDate: 2015.10.25 14:43:56 MSK\nReason: Test 1\nLocation: Ghent"
                , 12f, 176);
            NUnit.Framework.Assert.IsTrue(list2.Count == 5);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void KozminNames() {
            FontNames names = FontNamesFactory.FetchFontNames("KozMinPro-Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "KozMinPro-Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "KozMinPro-Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "kozminpro-regular");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 400);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), false);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HelveticaNames() {
            FontNames names = FontNamesFactory.FetchFontNames("Helvetica");
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "Helvetica");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "Helvetica");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "helvetica");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 500);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), false);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void OtfByStringNames() {
            FontNames names = FontNamesFactory.FetchFontNames(fontsFolder + "Puritan2.otf");
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "Puritan2");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "Puritan 2.0 Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFamilyName()[0][3], "Puritan 2.0");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "puritan 2.0 regular");
            NUnit.Framework.Assert.AreEqual(names.GetStyle(), "Normal");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 400);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), true);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void OtfByStreamNames() {
            FontNames names = FontNamesFactory.FetchFontNames(StreamUtil.InputStreamToArray(new FileStream(fontsFolder
                 + "Puritan2.otf", FileMode.Open, FileAccess.Read)));
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "Puritan2");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "Puritan 2.0 Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFamilyName()[0][3], "Puritan 2.0");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "puritan 2.0 regular");
            NUnit.Framework.Assert.AreEqual(names.GetStyle(), "Normal");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 400);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), true);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TtfByStringNames() {
            FontNames names = FontNamesFactory.FetchFontNames(fontsFolder + "abserif4_5.ttf");
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "AboriginalSerif");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "Aboriginal Serif");
            NUnit.Framework.Assert.AreEqual(names.GetFamilyName()[0][3], "Aboriginal Serif");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "aboriginal serif");
            NUnit.Framework.Assert.AreEqual(names.GetStyle(), "Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 400);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), true);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TtfByStreamNames() {
            FontNames names = FontNamesFactory.FetchFontNames(StreamUtil.InputStreamToArray(new FileStream(fontsFolder
                 + "abserif4_5.ttf", FileMode.Open, FileAccess.Read)));
            NUnit.Framework.Assert.AreEqual(names.GetFontName(), "AboriginalSerif");
            NUnit.Framework.Assert.AreEqual(names.GetFullName()[0][3], "Aboriginal Serif");
            NUnit.Framework.Assert.AreEqual(names.GetFamilyName()[0][3], "Aboriginal Serif");
            NUnit.Framework.Assert.AreEqual(names.GetFullNameLowerCase(), "aboriginal serif");
            NUnit.Framework.Assert.AreEqual(names.GetStyle(), "Regular");
            NUnit.Framework.Assert.AreEqual(names.GetFontWeight(), 400);
            NUnit.Framework.Assert.AreEqual(names.GetFontWidth(), 5);
            NUnit.Framework.Assert.AreEqual(names.AllowEmbedding(), true);
        }
    }
}
