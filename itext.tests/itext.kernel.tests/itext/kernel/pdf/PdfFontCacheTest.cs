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
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFontCacheTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfFontCacheTest/";

        private static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfFontCacheTest/";

//\cond DO_NOT_DOCUMENT
        internal const String pangramme = "Amazingly few discotheques provide jukeboxes " + "but it now while sayingly ABEFGHJKNOPQRSTUWYZ?";
//\endcond

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        private static readonly String[] TextSetHelloWorld = new String[] { "Hello World" };

        private static readonly String[] TextSetWithABC = new String[] { "Hello World", "ABC", "XYZ" };

        private static readonly String[] TextSetInternational = new String[] { "Hello World", "Привет, мир", "你好，世界"
            , "안녕 세상" };

        private static readonly String[] TextSetChinese = new String[] { "Hello World", "你好", "世界" };

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithKozmin() {
            String testName = "DocumentWithKozmin";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            AddPagesWithFonts(pdfDoc, "KozMinPro-Regular", "UniJIS-UCS2-H", TextSetChinese);
            AddPagesWithFonts(pdfDoc, "KozMinPro-Regular", "Adobe-Japan1-0", TextSetChinese);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaMixEncodings() {
            String testName = "DocumentWithHelveticaMixEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.HELVETICA;
            String encoding = null;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "MacRoman", TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelvetica() {
            String testName = "DocumentWithHelvetica";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.HELVETICA;
            String encoding = null;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaFlushed() {
            String testName = "DocumentWithHelveticaFlushed";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.HELVETICA;
            String encoding = null;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            //Flushed fonts cannot be reused.
            NUnit.Framework.Assert.AreEqual(3, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTimesAndCustomEncoding() {
            String testName = "DocumentTimesAndCustomEncoding";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.TIMES_ROMAN;
            String encoding = "# full 'A' Aring 0041 'E' Egrave 0045 32 space 0020";
            String[] AE = new String[] { "A E" };
            AddPagesWithFonts(pdfDoc, font, encoding, AE);
            AddPagesWithFonts(pdfDoc, font, encoding, AE);
            AddPagesWithFonts(pdfDoc, font, encoding, AE);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithCourierAndWinAnsiEncodings() {
            String testName = "DocumentCourierAndWinAnsiEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.COURIER;
            PdfFontFactory.EmbeddingStrategy embeddingStrategy = PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED;
            //All those encodings actually the same winansi.
            AddPagesWithFonts(pdfDoc, font, null, embeddingStrategy, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "", embeddingStrategy, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "WinAnsi", embeddingStrategy, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "WinAnsiEncoding", embeddingStrategy, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithAbserifAndIdentityHEncodings() {
            String testName = "DocumentWithAbserifAndIdentityHEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            //All those encodings actually the same Identity-H.
            AddPagesWithFonts(pdfDoc, font, null, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "", TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, PdfEncodings.IDENTITY_H, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithEmbeddedAbserifFirstWinAnsiThenIdentityHEncodings() {
            String testName = "DocumentWithEmbeddedAbserifFirstWinAnsiThenIdentityHEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            AddPagesWithFonts(pdfDoc, font, PdfEncodings.WINANSI, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "", TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithEmbeddedAbserifFirstIdentityHThenWinAnsiEncodings() {
            String testName = "DocumentWithEmbeddedAbserifFirstIdentityHThenWinAnsiEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            AddPagesWithFonts(pdfDoc, font, "", TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, PdfEncodings.WINANSI, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithNotEmbeddedAbserifFirstWinAnsiThenIdentityHEncodings() {
            String testName = "DocumentWithNotEmbeddedAbserifFirstWinAnsiThenIdentityHEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFontFactory.EmbeddingStrategy embeddingStrategy = PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED;
            AddPagesWithFonts(pdfDoc, font, PdfEncodings.WINANSI, embeddingStrategy, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "", embeddingStrategy, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithNotEmbeddedAbserifFirstIdentityHThenWinAnsiEncodings() {
            String testName = "DocumentWithNotEmbeddedAbserifFirstIdentityHThenWinAnsiEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            PdfFontFactory.EmbeddingStrategy embeddingStrategy = PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED;
            AddPagesWithFonts(pdfDoc, font, "", embeddingStrategy, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, PdfEncodings.WINANSI, embeddingStrategy, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTimesBoldAndMacRomanEncodings() {
            String testName = "DocumentTimesBoldAndMacRomanEncodings";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = StandardFonts.TIMES_BOLD;
            //All those encodings actually the same MacRoman.
            AddPagesWithFonts(pdfDoc, font, "MacRoman", TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "MacRomanEncoding", TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0DefaultEncoding() {
            String testName = "DocumentWithTrueTypeAsType0DefaultEncoding";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            String encoding = null;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsTrueType() {
            String testName = "DocumentWithTrueType";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            String encoding = PdfEncodings.WINANSI;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeFlushed() {
            String testName = "DocumentWithTrueTypeFlushed";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            String encoding = null;
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            //Flushed fonts cannot be reused.
            //For some reason Acrobat shows only one font in Properties.
            //RUPS shows 3 instances of the same font.
            NUnit.Framework.Assert.AreEqual(3, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0() {
            String testName = "DocumentWithTrueTypeAsType0";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            String encoding = "Identity-H";
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAsType0Flushed() {
            String testName = "DocumentWithTrueTypeAsType0Flushed";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "abserif4_5.ttf";
            String encoding = "Identity-H";
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetWithABC);
            pdfDoc.Close();
            //Flushed fonts cannot be reused.
            NUnit.Framework.Assert.AreEqual(3, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithOpenTypeAsType0() {
            String testName = "DocumentWithOpenTypeAsType0";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "NotoSansCJKjp-Bold.otf";
            String encoding = "Identity-H";
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithOpenTypeAsType0Flushed() {
            String testName = "DocumentWithOpenTypeAsType0Flushed";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            String font = fontsFolder + "NotoSansCJKjp-Bold.otf";
            String encoding = "Identity-H";
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            pdfDoc.FlushFonts();
            AddPagesWithFonts(pdfDoc, font, encoding, TextSetInternational);
            pdfDoc.Close();
            //Flushed fonts cannot be reused.
            NUnit.Framework.Assert.AreEqual(3, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaFromDocument() {
            String testName = "DocumentWithHelveticaFromDocument";
            String input = sourceFolder + "DocumentWithHelvetica.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String font = StandardFonts.HELVETICA;
            String encoding = "WinAnsiEncoding";
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, font, "WinAnsi", TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, null, TextSetWithABC);
            pdfDoc.Close();
            //We cannot rely on font name for a document font, so we treat them as two different fonts.
            //However we're trying to detect standard fonts in this case, so it will work.
            NUnit.Framework.Assert.AreEqual(1, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithHelveticaFromDocumentWithWrongEncoding() {
            String testName = "DocumentWithHelveticaFromDocumentWithWrongEncoding";
            String input = sourceFolder + "DocumentWithHelvetica.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String font = StandardFonts.HELVETICA;
            String encoding = "WinAnsiEncoding";
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, font, null, TextSetWithABC);
            AddPagesWithFonts(pdfDoc, font, "MacRoman", TextSetWithABC);
            pdfDoc.Close();
            //Two different encodings were used -> two fonts are expected.
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithTrueTypeAboriginalFromDocument() {
            String testName = "DocumentWithTrueTypeAboriginalFromDocument";
            String input = sourceFolder + "DocumentWithTrueTypeAboriginalSerif.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String font = "AboriginalSerif";
            String encoding = "WinAnsiEncoding";
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, fontsFolder + "abserif4_5.ttf", "WinAnsi", TextSetWithABC);
            pdfDoc.Close();
            //We cannot rely on font name for a document font, so we treat them as two different fonts.
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1NotoFromDocument() {
            String testName = "DocumentWithType1NotoFromDocument";
            String input = sourceFolder + "DocumentWithType1Noto.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String font = "NotoSansCJKjp-Bold";
            String encoding = "WinAnsiEncoding";
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, fontsFolder + "NotoSansCJKjp-Bold.otf", encoding, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED, TextSetWithABC);
            pdfDoc.Close();
            //We cannot rely on font name for a document font, so we treat them as two different fonts.
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType0AboriginalFromDocument1() {
            String testName = "DocumentWithType0AboriginalFromDocument";
            String input = sourceFolder + "DocumentWithType0AboriginalSerif.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String font = "AboriginalSerif";
            String encoding = "Identity-H";
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, fontsFolder + "abserif4_5.ttf", encoding, TextSetWithABC);
            pdfDoc.Close();
            //We cannot rely on font name for a document font, so we treat them as two different fonts.
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType0NotoFromDocument1() {
            String testName = "DocumentWithType0NotoFromDocument";
            String input = sourceFolder + "DocumentWithType0Noto.pdf";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            String encoding = "Identity-H";
            String font = "NotoSansCJKjp-Bold-" + encoding;
            PdfDictionary fontDict = (PdfDictionary)pdfDoc.GetPdfObject(6);
            NUnit.Framework.Assert.AreEqual(font, fontDict.GetAsName(PdfName.BaseFont).GetValue());
            NUnit.Framework.Assert.AreEqual(encoding, fontDict.GetAsName(PdfName.Encoding).GetValue());
            PdfFont documentFont = PdfFontFactory.CreateFont(fontDict);
            //Add it to PdfDocument#documentFonts via PdfCanvas.
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginText().MoveText(36, 700).SetFontAndSize(documentFont, 20).ShowText(pangramme.JSubstring(0, pangramme
                .Length / 2)).EndText().BeginText().SetFontAndSize(documentFont, 20).MoveText(36, 670).ShowText(pangramme
                .Substring(pangramme.Length / 2)).EndText().Release();
            //There is only one just loaded and used document font.
            NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetDocumentFonts().Count);
            AddPagesWithFonts(pdfDoc, fontsFolder + "NotoSansCJKjp-Bold.otf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED, TextSetWithABC);
            pdfDoc.Close();
            //We cannot rely on font name for a document font, so we treat them as two different fonts.
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType3Font() {
            String testName = "DocumentWithType3Font";
            String filename = destinationFolder + testName + ".pdf";
            String cmpFilename = sourceFolder + "cmp_" + testName + ".pdf";
            PdfDocument pdfDoc = CreateDocument(filename);
            PdfType3Font type3Font = PdfFontFactory.CreateType3Font(pdfDoc, false);
            Type3Glyph type3Glyph = type3Font.AddGlyph('A', 600, 0, 0, 600, 700);
            type3Glyph.SetLineWidth(100);
            type3Glyph.MoveTo(5, 5);
            type3Glyph.LineTo(300, 695);
            type3Glyph.LineTo(595, 5);
            type3Glyph.ClosePathFillStroke();
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().SetFontAndSize(type3Font, 36).MoveText(50, 700).ShowText("AA").EndText();
            type3Font = PdfFontFactory.CreateType3Font(pdfDoc, false);
            type3Glyph = type3Font.AddGlyph('A', 600, 0, 0, 600, 700);
            type3Glyph.SetLineWidth(100);
            type3Glyph.MoveTo(5, 5);
            type3Glyph.LineTo(300, 695);
            type3Glyph.LineTo(595, 5);
            type3Glyph.ClosePathFillStroke();
            canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().SetFontAndSize(type3Font, 36).MoveText(50, 650).ShowText("AAA").EndText();
            pdfDoc.Close();
            //PdfType3Font comparing returns false;
            NUnit.Framework.Assert.AreEqual(2, CountPdfFonts(filename));
            // reading and comparing text
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, destinationFolder, 
                "diff_"));
        }

        private void AddPagesWithFonts(PdfDocument pdfDoc, String fontProgram, String fontEncoding, String[] text) {
            AddPagesWithFonts(pdfDoc, fontProgram, fontEncoding, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED, text
                );
        }

        private void AddPagesWithFonts(PdfDocument pdfDoc, String fontProgram, String fontEncoding, PdfFontFactory.EmbeddingStrategy
             embeddingStrategy, String[] text) {
            int top = 700;
            foreach (String t in text) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, top).SetFontAndSize(PdfFontFactory.CreateFont(fontProgram, fontEncoding
                    , embeddingStrategy, pdfDoc), 72).ShowText(t).EndText().RestoreState();
                canvas.Release();
                page.Flush();
            }
        }

        private int CountPdfFonts(String filename) {
            PdfReader reader = CompareTool.CreateOutputReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            ICollection<PdfIndirectReference> fonts = new HashSet<PdfIndirectReference>();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++) {
                PdfPage page = pdfDoc.GetPage(i);
                foreach (PdfObject value in page.GetResources().GetResource(PdfName.Font).Values()) {
                    fonts.Add(value.GetIndirectReference());
                }
            }
            return fonts.Count;
        }

        private PdfDocument CreateDocument(String filename) {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                );
            return new PdfDocument(writer);
        }
    }
}
