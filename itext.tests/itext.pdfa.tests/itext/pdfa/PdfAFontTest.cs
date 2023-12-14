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
using System.IO;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfAFontTest : ExtendedITextTest {
        internal static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        internal static readonly String outputDir = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/pdfa/PdfAFontTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(outputDir);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_01() {
            String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_02() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "FreeSans"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_03() {
            String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_03.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_04() {
            PdfWriter writer = new PdfWriter(new MemoryStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont("Helvetica", "WinAnsi", PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.ALL_THE_FONTS_MUST_BE_EMBEDDED_THIS_ONE_IS_NOT_0
                , "Helvetica"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA1_05() {
            String outPdf = outputDir + "pdfA1b_fontCheckPdfA1_05.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA1b_fontCheckPdfA1_05.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf", "Identity-H");
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-1B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA2_01() {
            String outPdf = outputDir + "pdfA2b_fontCheckPdfA2_01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_fontCheckPdfA2_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-2B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FontCheckPdfA3_01() {
            String outPdf = outputDir + "pdfA3b_fontCheckPdfA3_01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA3b_fontCheckPdfA3_01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().SetFillColor(ColorConstants.GREEN).BeginText().MoveText(36, 700).SetFontAndSize(font, 36
                ).ShowText("Hello World! Pdf/A-3B").EndText().RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest1() {
            String outPdf = outputDir + "pdfA2b_cidFontCheckTest1.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest1.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                ).RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest2() {
            String outPdf = outputDir + "pdfA2b_cidFontCheckTest2.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest2.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "Puritan2.otf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                ).RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest3() {
            String outPdf = outputDir + "pdfA2b_cidFontCheckTest3.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            // Identity-H must be embedded
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKtc-Light.otf", "Identity-H", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                ).RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test01() {
            // encoding must not be specified
            CreateDocumentWithFont("symbolicTtfCharEncodingsPdfA1Test01.pdf", "Symbols1.ttf", "", PdfAConformanceLevel
                .PDF_A_1B);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test02() {
            // if you specify encoding, symbolic font is treated as non-symbolic
            CreateDocumentWithFont("symbolicTtfCharEncodingsPdfA1Test02.pdf", "Symbols1.ttf", PdfEncodings.MACROMAN, PdfAConformanceLevel
                .PDF_A_1B);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test03() {
            // if you specify encoding, symbolic font is treated as non-symbolic
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => CreateDocumentWithFont(
                "symbolicTtfCharEncodingsPdfA1Test03.pdf", "Symbols1.ttf", "ISO-8859-1", PdfAConformanceLevel.PDF_A_1B
                ));
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ALL_NON_SYMBOLIC_TRUE_TYPE_FONT_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING_AS_THE_ENCODING_ENTRY
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
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.ALL_NON_SYMBOLIC_TRUE_TYPE_FONT_SHALL_SPECIFY_MAC_ROMAN_ENCODING_OR_WIN_ANSI_ENCODING
                , e.Message);
        }

        private void CreateDocumentWithFont(String outFileName, String fontFileName, String encoding, PdfAConformanceLevel
             conformanceLevel) {
            String outPdf = outputDir + outFileName;
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_" + outFileName;
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfDocument doc = new PdfADocument(writer, conformanceLevel, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + fontFileName, encoding, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                ).RestoreState();
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, outputDir, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
