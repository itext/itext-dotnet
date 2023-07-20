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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFontTest : ExtendedITextTest {
        public const int PageCount = 1;

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfFontTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfFontTest/";

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithKozminAndDifferentCodespaceRanges() {
            String filename = destinationFolder + "DocumentWithKozminDifferentCodespaceRanges.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozminDifferentCodespaceRanges.pdf";
            String title = "Type 0 test";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont type0Font = PdfFontFactory.CreateFont("KozMinPro-Regular", "83pv-RKSJ-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsTrue(type0Font is PdfType0Font, "Type0Font expected");
            NUnit.Framework.Assert.IsTrue(type0Font.GetFontProgram() is CidFont, "CidFont expected");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(type0Font, 50).ShowText(type0Font.CreateGlyphLine
                ("Hello\u7121\u540dworld\u6b98\u528d")).EndText().RestoreState();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType3Font() {
            String filename = destinationFolder + "DocumentWithType3Font.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithType3Font.pdf";
            // A A A A E E E ~ é
            String testString = "A A A A E E E ~ \u00E9";
            //writing type3 font characters
            String title = "Type3 font iText Document";
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
            NUnit.Framework.Assert.AreEqual(600.0, GetContentWidth(type3, 'A'), 1e-5);
            Type3Glyph space = type3.AddGlyph(' ', 600, 0, 0, 600, 700);
            space.SetLineWidth(10);
            space.ClosePathFillStroke();
            NUnit.Framework.Assert.AreEqual(600.0, GetContentWidth(type3, ' '), 1e-5);
            Type3Glyph e = type3.AddGlyph('E', 600, 0, 0, 600, 700);
            e.SetLineWidth(100);
            e.MoveTo(595, 5);
            e.LineTo(5, 5);
            e.LineTo(300, 350);
            e.LineTo(5, 695);
            e.LineTo(595, 695);
            e.Stroke();
            NUnit.Framework.Assert.AreEqual(600.0, GetContentWidth(type3, 'E'), 1e-5);
            Type3Glyph tilde = type3.AddGlyph('~', 600, 0, 0, 600, 700);
            tilde.SetLineWidth(100);
            tilde.MoveTo(595, 5);
            tilde.LineTo(5, 5);
            tilde.Stroke();
            NUnit.Framework.Assert.AreEqual(600.0, GetContentWidth(type3, '~'), 1e-5);
            Type3Glyph symbol233 = type3.AddGlyph('\u00E9', 600, 0, 0, 600, 700);
            symbol233.SetLineWidth(100);
            symbol233.MoveTo(540, 5);
            symbol233.LineTo(5, 340);
            symbol233.Stroke();
            NUnit.Framework.Assert.AreEqual(600.0, GetContentWidth(type3, '\u00E9'), 1e-5);
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

        [NUnit.Framework.Test]
        public virtual void NotReplaceToUnicodeMappingTest() {
            //TODO DEVSIX-4995 This test should be updated when DEVSIX-4995 is resolved
            String filename = sourceFolder + "toUnicodeAndDifferenceFor32.pdf";
            using (PdfDocument pdf = new PdfDocument(new PdfReader(filename))) {
                PdfDictionary pdfType3FontDict = (PdfDictionary)pdf.GetPdfObject(112);
                PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont(pdfType3FontDict);
                //should be another glyph defined in ToUnicode mapping
                Glyph glyph = pdfType3Font.GetGlyph(32);
                NUnit.Framework.Assert.AreEqual(0, glyph.GetWidth());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateTaggedDocumentWithType3Font() {
            String filename = destinationFolder + "createTaggedDocumentWithType3Font.pdf";
            String cmpFilename = sourceFolder + "cmp_createTaggedDocumentWithType3Font.pdf";
            // A A A A E E E ~ é
            String testString = "A A A A E E E ~ \u00E9";
            //writing type3 font characters
            String title = "Type3 font iText Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties());
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer).SetTagged();
            PdfType3Font type3 = PdfFontFactory.CreateType3Font(pdfDoc, "T3Font", "T3Font", false);
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
                canvas.SaveState().BeginText().SetFontAndSize(type3, 12).MoveText(50, 800).ShowText(testString).EndText().
                    RestoreState();
                page.Flush();
            }
            pdfDoc.Close();
            // reading and comparing text
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

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
            PdfFont pdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaOblique() {
            String filename = destinationFolder + "DocumentWithHelveticaOblique.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithHelveticaOblique.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaBoldOblique() {
            String filename = destinationFolder + "DocumentWithHelveticaBoldOblique.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithHelveticaBoldOblique.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLDOBLIQUE);
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithCourierBold() {
            String filename = destinationFolder + "DocumentWithCourierBold.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCourierBold.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1FontAfm() {
            String filename = destinationFolder + "DocumentWithCMR10Afm.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(fontsFolder + "cmr10.afm"
                , fontsFolder + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                ));
            pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(afm, pfb), FontEncoding.FONT_SPECIFIC
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1FontPfm() {
            String filename = destinationFolder + "DocumentWithCMR10Pfm.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Pfm.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(fontsFolder + "cmr10.pfm"
                , fontsFolder + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeFont1() {
            String filename = destinationFolder + "DocumentWithTrueTypeFont1.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont1.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateFont(ttf, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeFont1NotEmbedded() {
            String filename = destinationFolder + "createDocumentWithTrueTypeFont1NotEmbedded.pdf";
            String cmpFilename = sourceFolder + "cmp_createDocumentWithTrueTypeFont1NotEmbedded.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            page.Flush();
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateFont(ttf, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED
                );
            NUnit.Framework.Assert.IsTrue(pdfTrueTypeFont is PdfTrueTypeFont, "PdfTrueTypeFont expected");
            pdfTrueTypeFont.SetSubset(true);
            page = pdfDoc.AddNewPage();
            new PdfCanvas(page).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText
                ("Hello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill().Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeOtfFont() {
            String filename = destinationFolder + "DocumentWithTrueTypeOtfFont.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeOtfFont.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "Puritan2.otf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
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
            pdfTrueTypeFont = PdfFontFactory.CreateFont(ttf, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeOtfFontPdf20() {
            String filename = destinationFolder + "DocumentWithTrueTypeOtfFontPdf20.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeOtfFontPdf20.pdf";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            String font = fontsFolder + "Puritan2.otf";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateFont(font, PdfEncodings.IDENTITY_H);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            // Assert no CIDSet is written. It is deprecated in PDF 2.0
            PdfDocument generatedDoc = new PdfDocument(new PdfReader(filename));
            PdfFont pdfFont = PdfFontFactory.CreateFont(generatedDoc.GetPage(1).GetResources().GetResource(PdfName.Font
                ).GetAsDictionary(new PdfName("F1")));
            PdfDictionary descriptor = pdfFont.GetPdfObject().GetAsArray(PdfName.DescendantFonts).GetAsDictionary(0).GetAsDictionary
                (PdfName.FontDescriptor);
            NUnit.Framework.Assert.IsFalse(descriptor.ContainsKey(PdfName.CIDSet), "CIDSet is deprecated in PDF 2.0 and should not be written"
                );
            generatedDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType0OtfFont() {
            String filename = destinationFolder + "DocumentWithType0OtfFont.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithType0OtfFont.pdf";
            String title = "Empty iText Document";
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

        [NUnit.Framework.Test]
        public virtual void TestUpdateType3FontBasedExistingFont() {
            String inputFileName = sourceFolder + "type3Font.pdf";
            String outputFileName = destinationFolder + "type3Font_update.pdf";
            String cmpOutputFileName = sourceFolder + "cmp_type3Font_update.pdf";
            String title = "Type3 font iText Document";
            int numberOfGlyphs = 0;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(outputFileName).SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION))) {
                pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
                PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdfDoc.GetPdfObject(5));
                Type3Glyph newGlyph = pdfType3Font.AddGlyph('\u00F6', 600, 0, 0, 600, 700);
                newGlyph.SetLineWidth(100);
                newGlyph.MoveTo(540, 5);
                newGlyph.LineTo(5, 840);
                newGlyph.Stroke();
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().SetFontAndSize(pdfType3Font, 12).MoveText(50, 800)
                                // A A A A A A E E E E ~ é ö
                                .ShowText("A A A A A A E E E E ~ \u00E9 \u00F6").EndText().RestoreState();
                page.Flush();
                numberOfGlyphs = pdfType3Font.GetNumberOfGlyphs();
            }
            NUnit.Framework.Assert.AreEqual(6, numberOfGlyphs);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpOutputFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNewType3FontBasedExistingFont() {
            String inputFileName = sourceFolder + "type3Font.pdf";
            String outputFileName = destinationFolder + "type3Font_new.pdf";
            String cmpOutputFileName = sourceFolder + "cmp_type3Font_new.pdf";
            String title = "Type3 font iText Document";
            int numberOfGlyphs = 0;
            using (PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName))) {
                using (PdfDocument outputPdfDoc = new PdfDocument(new PdfWriter(outputFileName).SetCompressionLevel(CompressionConstants
                    .NO_COMPRESSION))) {
                    outputPdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
                    PdfDictionary pdfType3FontDict = (PdfDictionary)inputPdfDoc.GetPdfObject(5);
                    PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont((PdfDictionary)pdfType3FontDict.CopyTo
                        (outputPdfDoc));
                    Type3Glyph newGlyph = pdfType3Font.AddGlyph('\u00F6', 600, 0, 0, 600, 700);
                    newGlyph.SetLineWidth(100);
                    newGlyph.MoveTo(540, 5);
                    newGlyph.LineTo(5, 840);
                    newGlyph.Stroke();
                    PdfPage page = outputPdfDoc.AddNewPage();
                    PdfCanvas canvas = new PdfCanvas(page);
                    canvas.SaveState().BeginText().SetFontAndSize(pdfType3Font, 12).MoveText(50, 800)
                                        // AAAAAA EEEE ~ é ö
                                        .ShowText("AAAAAA EEEE ~ \u00E9 \u00F6").EndText();
                    page.Flush();
                    numberOfGlyphs = pdfType3Font.GetNumberOfGlyphs();
                }
            }
            NUnit.Framework.Assert.AreEqual(6, numberOfGlyphs);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpOutputFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestAddGlyphToType3FontWithCustomNames() {
            String inputFile = sourceFolder + "type3FontWithCustomNames.pdf";
            int initialGlyphsNumber = 0;
            int finalGlyphsNumber = 0;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(new ByteArrayOutputStream
                ()))) {
                PdfDictionary pdfType3FontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
                PdfType3Font pdfType3Font = (PdfType3Font)PdfFontFactory.CreateFont(pdfType3FontDict);
                initialGlyphsNumber = pdfType3Font.GetNumberOfGlyphs();
                Type3Glyph newGlyph = pdfType3Font.AddGlyph('\u00F6', 600, 0, 0, 600, 700);
                newGlyph.SetLineWidth(100);
                newGlyph.MoveTo(540, 5);
                newGlyph.LineTo(5, 840);
                newGlyph.Stroke();
                PdfPage page = pdfDoc.GetPage(1);
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().SetFontAndSize(pdfType3Font, 12).MoveText(50, 800)
                                // AAAAAA EEEE ~ é ö
                                .ShowText("AAAAAA EEEE ~ \u00E9 \u00F6").EndText();
                page.Flush();
                finalGlyphsNumber = pdfType3Font.GetNumberOfGlyphs();
            }
            NUnit.Framework.Assert.AreEqual(initialGlyphsNumber + 1, finalGlyphsNumber);
        }

        [NUnit.Framework.Test]
        public virtual void TestNewType1FontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithCMR10Afm.pdf";
            String filename = destinationFolder + "DocumentWithCMR10Afm_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithCMR10Afm_new.pdf";
            String title = "Type 1 font iText Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = PdfFontFactory.CreateFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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

        [NUnit.Framework.Test]
        public virtual void TestNewTrueTypeFont2BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeFont2.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeFont2_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeFont2_new.pdf";
            String title = "True Type font iText Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfFont = inputPdfDoc1.GetFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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

        [NUnit.Framework.Test]
        public virtual void TestUpdateCjkFontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithKozmin.pdf";
            String filename = destinationFolder + "DocumentWithKozmin_update.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozmin_update.pdf";
            String title = "Type0 font iText Document";
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

        [NUnit.Framework.Test]
        public virtual void TestNewCjkFontBasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithKozmin.pdf";
            String filename = destinationFolder + "DocumentWithKozmin_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithKozmin_new.pdf";
            String title = "Type0 font iText Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(6);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeAsType0.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeAsType0_new.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeAsType0_new.pdf";
            String title = "Type0 font iText Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(6);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfTrueTypeFont = inputPdfDoc1.GetFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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

        [NUnit.Framework.Test]
        public virtual void CreateUpdatedDocumentWithTrueTypeAsType0BasedExistingFont() {
            String inputFileName1 = sourceFolder + "DocumentWithTrueTypeAsType0.pdf";
            String filename = destinationFolder + "DocumentWithTrueTypeAsType0_update.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTrueTypeAsType0_update.pdf";
            String title = "Type0 font iText Document";
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

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1WithToUnicodeBasedExistingFont() {
            String inputFileName1 = sourceFolder + "fontWithToUnicode.pdf";
            String filename = destinationFolder + "fontWithToUnicode_new.pdf";
            String cmpFilename = sourceFolder + "cmp_fontWithToUnicode_new.pdf";
            String title = "Type1 font iText Document";
            PdfReader reader1 = new PdfReader(inputFileName1);
            PdfDocument inputPdfDoc1 = new PdfDocument(reader1);
            PdfDictionary pdfDictionary = (PdfDictionary)inputPdfDoc1.GetPdfObject(4);
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfFont pdfType1Font = inputPdfDoc1.GetFont((PdfDictionary)pdfDictionary.CopyTo(pdfDoc));
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

        [NUnit.Framework.Test]
        public virtual void CreateWrongAfm1() {
            String message = "";
            try {
                byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                    ));
                FontProgramFactory.CreateType1Font(null, pfb);
            }
            catch (iText.IO.Exceptions.IOException e) {
                message = e.Message;
            }
            NUnit.Framework.Assert.AreEqual("Invalid afm or pfm font file.", message);
        }

        [NUnit.Framework.Test]
        public virtual void CreateWrongAfm2() {
            String message = "";
            String font = fontsFolder + "cmr10.pfb";
            try {
                FontProgramFactory.CreateType1Font(font, null);
            }
            catch (iText.IO.Exceptions.IOException e) {
                message = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.IS_NOT_AN_AFM_OR_PFM_FONT_FILE
                , font), message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.START_MARKER_MISSING_IN_PFB_FILE)]
        public virtual void CreateWrongPfb() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(afm, afm, false), null);
            byte[] streamContent = ((Type1Font)((PdfType1Font)font).GetFontProgram()).GetFontStreamBytes();
            NUnit.Framework.Assert.IsTrue(streamContent == null, "Empty stream content expected");
        }

        [NUnit.Framework.Test]
        public virtual void AutoDetect1() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(afm, false) is Type1Font, "Type1 font expected"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AutoDetect2() {
            byte[] afm = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.afm", FileMode.Open, FileAccess.Read
                ));
            byte[] pfb = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "cmr10.pfb", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateType1Font(afm, pfb) is Type1Font, "Type1 font expected"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AutoDetect3() {
            byte[] otf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "Puritan2.otf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(otf) is TrueTypeFont, "TrueType (OTF) font expected"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AutoDetect4() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "abserif4_5.ttf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(ttf) is TrueTypeFont, "TrueType (TTF) expected"
                );
        }

        [NUnit.Framework.Test]
        public virtual void AutoDetect5() {
            byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(fontsFolder + "abserif4_5.ttf", FileMode.Open, FileAccess.Read
                ));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.CreateFont(ttf) is TrueTypeFont, "TrueType (TTF) expected"
                );
        }

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

        [NUnit.Framework.Test]
        public virtual void TestWriteTTC() {
            String filename = destinationFolder + "DocumentWithTTC.pdf";
            String cmpFilename = sourceFolder + "cmp_DocumentWithTTC.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "uming.ttc";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(font, 0, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED, false);
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            byte[] ttc = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(ttc, 1, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED, false);
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

        [NUnit.Framework.Test]
        public virtual void TestWriteTTCNotEmbedded() {
            String filename = destinationFolder + "testWriteTTCNotEmbedded.pdf";
            String cmpFilename = sourceFolder + "cmp_testWriteTTCNotEmbedded.pdf";
            String title = "Empty iText Document";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            String font = fontsFolder + "uming.ttc";
            PdfFont pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(font, 0, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, false);
            pdfTrueTypeFont.SetSubset(true);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfTrueTypeFont, 72).ShowText("Hello world"
                ).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            page.Flush();
            byte[] ttc = StreamUtil.InputStreamToArray(new FileStream(font, FileMode.Open, FileAccess.Read));
            pdfTrueTypeFont = PdfFontFactory.CreateTtcFont(ttc, 1, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, false);
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

        [NUnit.Framework.Test]
        public virtual void TestNotoFont() {
            String filename = destinationFolder + "testNotoFont.pdf";
            String cmpFilename = sourceFolder + "cmp_testNotoFont.pdf";
            String japanese = "\u713C";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 680).SetFontAndSize(font, 12).ShowText(japanese).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WoffFontTest() {
            String filename = destinationFolder + "testWoffFont.pdf";
            String cmpFilename = sourceFolder + "cmp_testWoffFont.pdf";
            String helloWorld = "Hello world";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "SourceSerif4-Black.woff", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 680).SetFontAndSize(font, 12).ShowText(helloWorld).EndText().RestoreState
                ();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansCJKjpTest() {
            String filename = destinationFolder + "NotoSansCJKjpTest.pdf";
            String cmpFilename = sourceFolder + "cmp_NotoSansCJKjpTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            // font.setSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                .ShowText("1").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansCJKjpTest02() {
            String filename = destinationFolder + "NotoSansCJKjpTest02.pdf";
            String cmpFilename = sourceFolder + "cmp_NotoSansCJKjpTest02.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            // font.setSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                .ShowText("\u3000").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansCJKjpTest03() {
            String filename = destinationFolder + "NotoSansCJKjpTest03.pdf";
            String cmpFilename = sourceFolder + "cmp_NotoSansCJKjpTest03.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            // font.setSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                        // there is no such glyph in provided cff
                        .ShowText("\u0BA4").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SourceHanSansHWTest() {
            String filename = destinationFolder + "SourceHanSansHWTest.pdf";
            String cmpFilename = sourceFolder + "cmp_SourceHanSansHWTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "SourceHanSansHW-Regular.otf", "Identity-H");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                .ShowText("12").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SourceHanSerifKRRegularTest() {
            String filename = destinationFolder + "SourceHanSerifKRRegularTest.pdf";
            String cmpFilename = sourceFolder + "cmp_SourceHanSerifKRRegularTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "SourceHanSerifKR-Regular.otf");
            //font.setSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                .ShowText("\ube48\uc9d1").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder)
                );
        }

        [NUnit.Framework.Test]
        public virtual void SourceHanSerifKRRegularFullTest() {
            String filename = destinationFolder + "SourceHanSerifKRRegularFullTest.pdf";
            String cmpFilename = sourceFolder + "cmp_SourceHanSerifKRRegularFullTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(filename));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "SourceHanSerifKR-Regular.otf");
            font.SetSubset(false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(36, 680).SetFontAndSize(font, 12)
                .ShowText("\ube48\uc9d1").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder)
                );
        }

        [NUnit.Framework.Test]
        public virtual void MmType1ReadTest() {
            String src = sourceFolder + "mmtype1.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src));
            PdfFont font = PdfFontFactory.CreateFont((PdfDictionary)doc.GetPdfObject(335));
            doc.Close();
            NUnit.Framework.Assert.AreEqual(PdfName.MMType1, font.GetPdfObject().GetAsName(PdfName.Subtype));
            NUnit.Framework.Assert.AreEqual(typeof(PdfType1Font), font.GetType());
        }

        [NUnit.Framework.Test]
        public virtual void MmType1WriteTest() {
            String src = sourceFolder + "mmtype1.pdf";
            String filename = destinationFolder + "mmtype1_res.pdf";
            String cmpFilename = sourceFolder + "cmp_mmtype1.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(src), new PdfWriter(filename));
            PdfFont font = PdfFontFactory.CreateFont((PdfDictionary)doc.GetPdfObject(335));
            PdfCanvas canvas = new PdfCanvas(doc.GetPage(1));
            canvas.SaveState().SetFillColor(ColorConstants.RED).BeginText().MoveText(5, 5).SetFontAndSize(font, 6).ShowText
                ("type1 font").EndText().RestoreState();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestFontStyleProcessing() {
            String filename = destinationFolder + "testFontStyleProcessing.pdf";
            String cmpFilename = sourceFolder + "cmp_testFontStyleProcessing.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            PdfFont romanDefault = PdfFontFactory.CreateRegisteredFont("Times-Roman", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            PdfFont romanNormal = PdfFontFactory.CreateRegisteredFont("Times-Roman", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, FontStyles.NORMAL);
            PdfFont romanBold = PdfFontFactory.CreateRegisteredFont("Times-Roman", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, FontStyles.BOLD);
            PdfFont romanItalic = PdfFontFactory.CreateRegisteredFont("Times-Roman", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, FontStyles.ITALIC);
            PdfFont romanBoldItalic = PdfFontFactory.CreateRegisteredFont("Times-Roman", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, FontStyles.BOLDITALIC);
            PdfPage page = pdfDoc.AddNewPage(PageSize.A4.Rotate());
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 400).SetFontAndSize(romanDefault, 72).ShowText("Times-Roman default"
                ).EndText().BeginText().MoveText(36, 350).SetFontAndSize(romanNormal, 72).ShowText("Times-Roman normal"
                ).EndText().BeginText().MoveText(36, 300).SetFontAndSize(romanBold, 72).ShowText("Times-Roman bold").EndText
                ().BeginText().MoveText(36, 250).SetFontAndSize(romanItalic, 72).ShowText("Times-Roman italic").EndText
                ().BeginText().MoveText(36, 200).SetFontAndSize(romanBoldItalic, 72).ShowText("Times-Roman bolditalic"
                ).EndText().RestoreState();
            canvas.Release();
            page.Flush();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckTTCSize() {
            TrueTypeCollection collection = new TrueTypeCollection(fontsFolder + "uming.ttc");
            NUnit.Framework.Assert.IsTrue(collection.GetTTCSize() == 4);
        }

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

        [NUnit.Framework.Test]
        public virtual void FontRegisterTest() {
            FontProgramFactory.RegisterFont(fontsFolder + "NotoSerif-Regular_v1.7.ttf", "notoSerifRegular");
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont pdfFont = PdfFontFactory.CreateRegisteredFont("notoSerifRegular");
            //clear font cache for other tests
            FontProgramFactory.ClearRegisteredFonts();
            NUnit.Framework.Assert.IsTrue(pdfFont is PdfType0Font);
            pdfDoc.AddNewPage();
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestSplitString() {
            PdfFont font = PdfFontFactory.CreateFont();
            IList<String> list1 = font.SplitString("Hello", 12f, 10);
            NUnit.Framework.Assert.IsTrue(list1.Count == 3);
            IList<String> list2 = font.SplitString("Digitally signed by Dmitry Trusevich\nDate: 2015.10.25 14:43:56 MSK\nReason: Test 1\nLocation: Ghent"
                , 12f, 176);
            NUnit.Framework.Assert.IsTrue(list2.Count == 5);
        }

        [NUnit.Framework.Test]
        public virtual void KozminNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor("KozMinPro-Regular");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "KozMinPro-Regular");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "KozMinPro-Regular".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 400);
        }

        [NUnit.Framework.Test]
        public virtual void HelveticaNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor("Helvetica");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "Helvetica");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "Helvetica".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "helvetica");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 500);
        }

        [NUnit.Framework.Test]
        public virtual void OtfByStringNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(fontsFolder + "Puritan2.otf"
                );
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "Puritan2");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "Puritan 2.0 Regular".ToLowerInvariant(
                ));
            NUnit.Framework.Assert.AreEqual(descriptor.GetFamilyNameLowerCase(), "Puritan 2.0".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetStyle(), "Normal");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 400);
        }

        [NUnit.Framework.Test]
        public virtual void OtfByStreamNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(StreamUtil.InputStreamToArray
                (new FileStream(fontsFolder + "Puritan2.otf", FileMode.Open, FileAccess.Read)));
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "Puritan2");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "Puritan 2.0 Regular".ToLowerInvariant(
                ));
            NUnit.Framework.Assert.AreEqual(descriptor.GetFamilyNameLowerCase(), "Puritan 2.0".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetStyle(), "Normal");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 400);
        }

        [NUnit.Framework.Test]
        public virtual void TtfByStringNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(fontsFolder + "abserif4_5.ttf"
                );
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "AboriginalSerif");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "Aboriginal Serif".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetFamilyNameLowerCase(), "Aboriginal Serif".ToLowerInvariant()
                );
            NUnit.Framework.Assert.AreEqual(descriptor.GetStyle(), "Regular");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 400);
        }

        [NUnit.Framework.Test]
        public virtual void TtfByStreamNames() {
            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(StreamUtil.InputStreamToArray
                (new FileStream(fontsFolder + "abserif4_5.ttf", FileMode.Open, FileAccess.Read)));
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontName(), "AboriginalSerif");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFullNameLowerCase(), "Aboriginal Serif".ToLowerInvariant());
            NUnit.Framework.Assert.AreEqual(descriptor.GetFamilyNameLowerCase(), "Aboriginal Serif".ToLowerInvariant()
                );
            NUnit.Framework.Assert.AreEqual(descriptor.GetStyle(), "Regular");
            NUnit.Framework.Assert.AreEqual(descriptor.GetFontWeight(), 400);
        }

        [NUnit.Framework.Test]
        public virtual void TestDefaultFontWithReader() {
            String inputFileName = sourceFolder + "type3Font.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFileName))) {
                NUnit.Framework.Assert.IsNotNull(pdfDoc.GetDefaultFont());
                NUnit.Framework.Assert.IsNull(pdfDoc.GetDefaultFont().GetPdfObject().GetIndirectReference());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MSungLightFontRanges() {
            String filename = destinationFolder + "mSungLightFontRanges.pdf";
            String cmpFilename = sourceFolder + "cmp_mSungLightFontRanges.pdf";
            PdfWriter writer = new PdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont mSungFont = PdfFontFactory.CreateFont("MSung-Light", "UniCNS-UCS2-H");
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(mSungFont, 40).ShowText("\u98db \u6708 \u9577"
                ).EndText().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder)
                );
        }

        private float GetContentWidth(PdfType3Font type3, char glyph) {
            return type3.GetContentWidth(new PdfString(new byte[] { (byte)type3.GetGlyph(glyph).GetCode() }));
        }
    }
}
