/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAFontTest : ExtendedITextTest {
//\cond DO_NOT_DOCUMENT
        internal static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFontTest/";
//\endcond

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_01() {
            String outPdf = DESTINATION_FOLDER + "pdfA1b_fontCheckPdfA1_01.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_02() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "FreeSans"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_03() {
            String outPdf = DESTINATION_FOLDER + "pdfA1b_fontCheckPdfA1_03.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_04() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont("Helvetica", "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_05() {
            String outPdf = DESTINATION_FOLDER + "pdfA1b_fontCheckPdfA1_05.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_05.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSansCJKtc-Light.otf", "Identity-H");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA2_01() {
            String outPdf = DESTINATION_FOLDER + "pdfA2b_fontCheckPdfA2_01.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA2b_fontCheckPdfA2_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-2B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA3_01() {
            String outPdf = DESTINATION_FOLDER + "pdfA3b_fontCheckPdfA3_01.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA3b_fontCheckPdfA3_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-3B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest1() {
            String outPdf = DESTINATION_FOLDER + "pdfA2b_cidFontCheckTest1.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest1.pdf";
            GeneratePdfA2WithCidFont("FreeSans.ttf", outPdf);
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest2() {
            String outPdf = DESTINATION_FOLDER + "pdfA2b_cidFontCheckTest2.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest2.pdf";
            String expectedVeraPdfWarning = "The following warnings and errors were logged during validation:\n" + "WARNING: The Top DICT does not begin with ROS operator";
            GeneratePdfA2WithCidFont("Puritan2.otf", outPdf);
            CompareResult(outPdf, cmpPdf, expectedVeraPdfWarning);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest3() {
            String outPdf = DESTINATION_FOLDER + "pdfA2b_cidFontCheckTest3.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest3.pdf";
            GeneratePdfA2WithCidFont("NotoSansCJKtc-Light.otf", outPdf);
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test01() {
            // encoding must not be specified
            // Here we produced valid pdfa files in the past by silently removing not valid symbols
            // But right now we check for used glyphs which don't exist in the font and throw exception
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test01.pdf", "Symbols1.ttf", "", PdfAConformanceLevel.PDF_A_1B));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test02() {
            // if you specify encoding, symbolic font is treated as non-symbolic
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test02.pdf", "Symbols1.ttf", PdfEncodings.MACROMAN, PdfAConformanceLevel
                .PDF_A_1B));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test03() {
            // if you specify encoding, symbolic font is treated as non-symbolic
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test03.pdf", "Symbols1.ttf", "ISO-8859-1", PdfAConformanceLevel.PDF_A_1B
                ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test04() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test04.pdf", "Symbols1.ttf", PdfEncodings.WINANSI, PdfAConformanceLevel.
                PDF_A_1B));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test05() {
            // Identity-H behaviour should be the same as the default one, starting from 7.2
            // Here we produced valid pdfa files in the past by silently removing not valid symbols
            // But right now we check for used glyphs which don't exist in the font and throw exception
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test05.pdf", "Symbols1.ttf", PdfEncodings.IDENTITY_H, PdfAConformanceLevel
                .PDF_A_1B));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonSymbolicTtfCharEncodingsPdfA1Test01() {
            // encoding must be either winansi or macroman, by default winansi is used
            CreateDocumentWithFont("nonSymbolicTtfCharEncodingsPdfA1Test01.pdf", "FreeSans.ttf", PdfEncodings.WINANSI, 
                PdfAConformanceLevel.PDF_A_1B);
        }

        [NUnit.Framework.Test]
        public virtual void NonSymbolicTtfCharEncodingsPdfA1Test02() {
            // encoding must be either winansi or macroman, by default winansi is used
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "nonSymbolicTtfCharEncodingsPdfA1Test02.pdf", "FreeSans.ttf", "ISO-8859-1", PdfAConformanceLevel.PDF_A_2B
                ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.ALL_NON_SYMBOLIC_TRUE_TYPE_FONT_SHALL_SPECIFY_MAC_ROMAN_ENCODING_OR_WIN_ANSI_ENCODING
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotdefInTrueTypeFontTest() {
            String outPdf = DESTINATION_FOLDER + "notdefInTrueTypeFont.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "# simple 32 0020 00C5 1987", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.ShowText("\u00C5 \u1987"
                ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotdefFontTest2() {
            String outPdf = DESTINATION_FOLDER + "notdefFontTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSans-Regular.ttf", "", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.ShowText("\u898B\u7A4D\u3082\u308A"
                ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GlyphLineWithUndefinedGlyphsTest() {
            String outPdf = DESTINATION_FOLDER + "glyphLineWithUndefinedGlyphs.pdf";
            Stream icm = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            Document document = new Document(new PdfADocument(new PdfWriter(outPdf, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)), PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB ICC preference", icm)));
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSans-Regular.ttf", "", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            Paragraph p = new Paragraph("\u898B\u7A4D\u3082\u308A");
            p.SetFont(font);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => document.Add(p));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfArrayWithUndefinedGlyphsTest() {
            String outPdf = DESTINATION_FOLDER + "pdfArrayWithUndefinedGlyphs.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSans-Regular.ttf", "", PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 786).SetFontAndSize(font, 36);
            PdfArray pdfArray = new PdfArray();
            pdfArray.Add(new PdfString("ABC"));
            pdfArray.Add(new PdfNumber(1));
            pdfArray.Add(new PdfString("\u898B\u7A4D\u3082\u308A"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => canvas.ShowText(pdfArray
                ));
            NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.EMBEDDED_FONTS_SHALL_DEFINE_ALL_REFERENCED_GLYPHS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocumentWithType1FontAfmTest() {
            String outPdf = DESTINATION_FOLDER + "DocumentWithCMR10Afm.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_DocumentWithCMR10Afm.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", 
                "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfFont pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(SOURCE_FOLDER + "cmr10.afm"
                , SOURCE_FOLDER + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            byte[] afm = StreamUtil.InputStreamToArray(FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "cmr10.afm"));
            byte[] pfb = StreamUtil.InputStreamToArray(FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "cmr10.pfb"));
            pdfType1Font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(afm, pfb), FontEncoding.FONT_SPECIFIC
                , PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsTrue(pdfType1Font is PdfType1Font, "PdfType1Font expected");
            new PdfCanvas(pdfDoc.AddNewPage()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize(pdfType1Font, 
                72).ShowText("\u0000\u0001\u007cHello world").EndText().RestoreState().Rectangle(100, 500, 100, 100).Fill
                ();
            pdfDoc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4FreeSansForceEmbeddedTest() {
            String outPdf = DESTINATION_FOLDER + "PdfA4FreeSansForceEmbeddedTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_PdfA4FreeSansForceEmbeddedTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4FreeSansForceNotEmbeddedTest() {
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(new MemoryStream(), writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "FreeSans"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4FreeSansPreferNotEmbeddedTest() {
            String outPdf = DESTINATION_FOLDER + "PdfA4FreeSansPreferNotEmbeddedTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_PdfA4FreeSansPreferNotEmbeddedTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4HelveticaPreferEmbeddedTest() {
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(new MemoryStream(), writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont("Helvetica", "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfaExceptionMessageConstant.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4NotoSansCJKtcLightTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4NotoSansCJKtcLightTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA4NotoSansCJKtcLightTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSansCJKtc-Light.otf", "Identity-H");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("你好世界! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4Puritan2Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4Puritan2Test.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA4Puritan2Test.pdf";
            String expectedVeraPdfWarning = "The following warnings and errors were logged during validation:\n" + "WARNING: The Top DICT does not begin with ROS operator";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "Puritan2.otf", "Identity-H");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, expectedVeraPdfWarning);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4Type3Test() {
            String outPdf = DESTINATION_FOLDER + "pdfA4Type3Test.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA4Type3Test.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            // A A A A E E E ~ é
            String testString = "A A A A E E E ~ \u00E9";
            //writing type3 font characters
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfType3Font type3 = PdfFontFactory.CreateType3Font(doc, false);
            PdfDictionary charProcs = new PdfDictionary();
            type3.GetPdfObject().Put(PdfName.CharProcs, charProcs);
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
            PdfPage page = doc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().SetFontAndSize(type3, 12).MoveText(50, 800).ShowText(testString).EndText();
            page.Flush(true);
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4UmingTtcTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4UmingTtcTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA4UmingTtcTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateTtcFont(SOURCE_FOLDER + "uming.ttc", 0, "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED, false);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4WoffTest() {
            String outPdf = DESTINATION_FOLDER + "pdfA4WoffTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_pdfA4WoffTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "SourceSerif4-Black.woff", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-4").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfA4SurrogatePairTest() {
            String outPdf = DESTINATION_FOLDER + "PdfA4SurrogatePairTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_PdfA4SurrogatePairTest.pdf";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(outPdf, writerProperties);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent("Custom", "", 
                "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoEmoji-Regular.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("\uD83D\uDC7B \uD83D\uDE09").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        private void CreateDocumentWithFont(String outFileName, String fontFileName, String encoding, PdfAConformanceLevel
             conformanceLevel) {
            String outPdf = DESTINATION_FOLDER + outFileName;
            String cmpPdf = SOURCE_FOLDER + "cmp/PdfAFontTest/cmp_" + outFileName;
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfDocument doc = new PdfADocument(writer, conformanceLevel, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + fontFileName, encoding, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                ).RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf, null);
        }

        private void CompareResult(String outPdf, String cmpPdf, String expectedVeraPdfWarning) {
            NUnit.Framework.Assert.AreEqual(expectedVeraPdfWarning, new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        private void GeneratePdfA2WithCidFont(String fontFile, String outPdf) {
            using (PdfWriter writer = new PdfWriter(outPdf)) {
                using (Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")) {
                    using (PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom"
                        , "", "http://www.color.org", "sRGB IEC61966-2.1", @is))) {
                        PdfPage page = doc.AddNewPage();
                        // Identity-H must be embedded
                        PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + fontFile, "Identity-H", PdfFontFactory.EmbeddingStrategy
                            .FORCE_EMBEDDED);
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                            ).RestoreState();
                    }
                }
            }
        }
    }
}
