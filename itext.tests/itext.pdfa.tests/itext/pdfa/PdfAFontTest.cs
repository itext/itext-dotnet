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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Pdfa.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
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
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
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
            GenerateAndValidatePdfA2WithCidFont("FreeSans.ttf", outPdf);
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest2() {
            String outPdf = outputDir + "pdfA2b_cidFontCheckTest2.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest2.pdf";
            String expectedVeraPdfWarning = "The following warnings and errors were logged during validation:\n" + "WARNING: The Top DICT does not begin with ROS operator";
            GenerateAndValidatePdfA2WithCidFont("Puritan2.otf", outPdf, expectedVeraPdfWarning);
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void CidFontCheckTest3() {
            String outPdf = outputDir + "pdfA2b_cidFontCheckTest3.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFontTest/cmp_pdfA2b_cidFontCheckTest3.pdf";
            GenerateAndValidatePdfA2WithCidFont("NotoSansCJKtc-Light.otf", outPdf);
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
        public virtual void SymbolicTtfCharEncodingsPdfA1Test04() {
            // emulate behaviour with default WinAnsi, which was present in 7.1
            CreateDocumentWithFont("symbolicTtfCharEncodingsPdfA1Test04.pdf", "Symbols1.ttf", PdfEncodings.WINANSI, PdfAConformanceLevel
                .PDF_A_1B);
        }

        [NUnit.Framework.Test]
        public virtual void SymbolicTtfCharEncodingsPdfA1Test05() {
            // Identity-H behaviour should be the same as the default one, starting from 7.2
            CreateDocumentWithFont("symbolicTtfCharEncodingsPdfA1Test05.pdf", "Symbols1.ttf", PdfEncodings.IDENTITY_H, 
                PdfAConformanceLevel.PDF_A_1B);
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

        private void GenerateAndValidatePdfA2WithCidFont(String fontFile, String outPdf) {
            GenerateAndValidatePdfA2WithCidFont(fontFile, outPdf, null);
        }

        private void GenerateAndValidatePdfA2WithCidFont(String fontFile, String outPdf, String expectedVeraPdfWarning
            ) {
            using (PdfWriter writer = new PdfWriter(outPdf)) {
                using (Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                    )) {
                    using (PdfDocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom"
                        , "", "http://www.color.org", "sRGB IEC61966-2.1", @is))) {
                        PdfPage page = doc.AddNewPage();
                        // Identity-H must be embedded
                        PdfFont font = PdfFontFactory.CreateFont(sourceFolder + fontFile, "Identity-H", PdfFontFactory.EmbeddingStrategy
                            .FORCE_EMBEDDED);
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(font, 12).ShowText("Hello World").EndText(
                            ).RestoreState();
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(expectedVeraPdfWarning, new VeraPdfValidator().Validate(outPdf));
        }
    }
}
