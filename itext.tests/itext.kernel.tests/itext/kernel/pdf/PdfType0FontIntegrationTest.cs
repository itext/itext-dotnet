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
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfType0FontIntegrationTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfType0FontIntegrationTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/fonts/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfType0FontIntegrationTest/";

        private static readonly String[] CHINESE = new String[] { "[以下、x。，料子法個資人用要供c及業B之括b對立該如 此蒐主N(需形並經擔予前關取揭手交其處,管:"
            , "同政持易任；行符何司股認意受求與為X稱理務服府或集使表步)情係a必f提循地東合商代風益限列「得保於團作露進已品不就事遵險維建F公機一", "目有僱包院律的]" };

        private const String JAPANESE = "5た うぞせツそぇBぁデぢつっず信えいすてナおドぅだトヅでぉミ(:テかちぜ)じぃあづ";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansJpFontTest() {
            String filename = DESTINATION_FOLDER + "notoSansJpFontTest.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_notoSansJpFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont jpFont = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansJP-Regular.otf");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(jpFont, 8).ShowText(jpFont.CreateGlyphLine
                (JAPANESE)).EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansScFontTest() {
            String filename = DESTINATION_FOLDER + "notoSansScFontTest.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_notoSansScFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont jpFont = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansSC-Regular.otf");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().SetFontAndSize(jpFont, 8).MoveText(36, 700);
            foreach (String s in CHINESE) {
                canvas.ShowText(jpFont.CreateGlyphLine(s)).MoveText(0, -16);
            }
            canvas.EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NotoSansTcFontTest() {
            String filename = DESTINATION_FOLDER + "notoSansTcFontTest.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_notoSansTcFontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont jpFont = PdfFontFactory.CreateFont(FONTS_FOLDER + "NotoSansTC-Regular.otf");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().SetFontAndSize(jpFont, 8).MoveText(36, 700);
            foreach (String s in CHINESE) {
                canvas.ShowText(jpFont.CreateGlyphLine(s)).MoveText(0, -16);
            }
            canvas.EndText().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CmapPlatform0PlatEnc3Format4FontTest() {
            String filename = DESTINATION_FOLDER + "cmapPlatform0PlatEnc3Format4FontTest.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_cmapPlatform0PlatEnc3Format4FontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "glyphs.ttf");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().SetFontAndSize(font, 20).MoveText(36, 700).ShowText("===fff===iii===ﬁ").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CmapPlatform0PlatEnc3Format6FontTest() {
            String filename = DESTINATION_FOLDER + "cmapPlatform0PlatEnc3Format6FontTest.pdf";
            String cmpFilename = SOURCE_FOLDER + "cmp_cmapPlatform0PlatEnc3Format6FontTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filename);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFont font = PdfFontFactory.CreateFont(FONTS_FOLDER + "glyphs-fmt-6.ttf");
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().SetFontAndSize(font, 20).MoveText(36, 700).ShowText("===fff===iii===").EndText
                ().RestoreState();
            canvas.Rectangle(100, 500, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, cmpFilename, DESTINATION_FOLDER
                ));
        }
    }
}
