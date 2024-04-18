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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HighlightItemsTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/parser/HighlightItemsTest/";

        private static readonly String outputPath = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/parser/HighlightItemsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(outputPath);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(outputPath);
        }

        [NUnit.Framework.Test]
        public virtual void HighlightPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229.pdf";
            String cmp = sourceFolder + "cmp_page229.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229_characters.pdf";
            String cmp = sourceFolder + "cmp_page229_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter_characters.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage2Test() {
            String input = sourceFolder + "reference_page2.pdf";
            String output = outputPath + "reference_page2_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page2_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage832Test() {
            String input = sourceFolder + "reference_page832.pdf";
            String output = outputPath + "reference_page832_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page832_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage604Test() {
            String input = sourceFolder + "reference_page604.pdf";
            String output = outputPath + "reference_page604_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page604_characters.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightNotDefTest() {
            String input = sourceFolder + "notdefWidth.pdf";
            String output = outputPath + "notdefWidth_highlighted.pdf";
            String cmp = sourceFolder + "cmp_notdefWidth.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingType1NoDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingType1NoDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingType1NoDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingType1NoDescriptorTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingTrueTypeFontDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingType1FontDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingType1FontDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingType1FontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingType1FontDescriptorTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectAscentFontDescriptorTest() {
            // As the negative ascent is not covered by pdf specification in details,
            // we work with it as usual (which results with not very beautiful view).
            String input = sourceFolder + "incorrectAscentFontDescriptorTest.pdf";
            String output = outputPath + "incorrectAscentFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_incorrectAscentFontDescriptorTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectDescentFontDescriptorTest() {
            String input = sourceFolder + "incorrectDescentFontDescriptorTest.pdf";
            String output = outputPath + "incorrectDescentFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_incorrectDescentFontDescriptorTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FontDictWidthArrayMissingWidthTest() {
            String input = sourceFolder + "fontDictWidthArrayMissingWidthTest.pdf";
            String output = outputPath + "fontDictWidthArrayMissingWidthTest.pdf";
            String cmp = sourceFolder + "cmp_fontDictWidthArrayMissingWidthTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeCIDFontWithDWWithoutProperWidthGlyphTest() {
            String input = sourceFolder + "trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            String output = outputPath + "trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            String cmp = sourceFolder + "cmp_trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void DoubleMappingSimpleFontTest() {
            String input = sourceFolder + "doubleMappingSimpleFont.pdf";
            String output = outputPath + "doubleMappingSimpleFont.pdf";
            String cmp = sourceFolder + "cmp_doubleMappingSimpleFont.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void DoubleMappingSimpleFontTest2() {
            String input = sourceFolder + "doubleMappingSimpleFont2.pdf";
            String output = outputPath + "doubleMappingSimpleFont2.pdf";
            String cmp = sourceFolder + "cmp_doubleMappingSimpleFont2.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(output);
            ParseAndHighlight(input, writer, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        private void ParseAndHighlight(String input, PdfWriter writer, bool singleCharacters) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), writer);
            HighlightItemsTest.MyEventListener myEventListener = singleCharacters ? new HighlightItemsTest.MyCharacterEventListener
                () : new HighlightItemsTest.MyEventListener();
            PdfDocumentContentParser parser = new PdfDocumentContentParser(pdfDocument);
            for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++) {
                parser.ProcessContent(pageNum, myEventListener);
                IList<Rectangle> rectangles = myEventListener.GetRectangles();
                PdfCanvas canvas = new PdfCanvas(pdfDocument.GetPage(pageNum));
                canvas.SetLineWidth(0.5f);
                canvas.SetStrokeColor(ColorConstants.RED);
                foreach (Rectangle rectangle in rectangles) {
                    canvas.Rectangle(rectangle);
                    canvas.Stroke();
                }
                myEventListener.Clear();
            }
            pdfDocument.Close();
        }

        internal class MyEventListener : IEventListener {
            private IList<Rectangle> rectangles = new List<Rectangle>();

            public virtual void EventOccurred(IEventData data, EventType type) {
                if (type == EventType.RENDER_TEXT) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    Vector startPoint = renderInfo.GetDescentLine().GetStartPoint();
                    Vector endPoint = renderInfo.GetAscentLine().GetEndPoint();
                    float x1 = Math.Min(startPoint.Get(0), endPoint.Get(0));
                    float x2 = Math.Max(startPoint.Get(0), endPoint.Get(0));
                    float y1 = Math.Min(startPoint.Get(1), endPoint.Get(1));
                    float y2 = Math.Max(startPoint.Get(1), endPoint.Get(1));
                    rectangles.Add(new Rectangle(x1, y1, x2 - x1, y2 - y1));
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return new LinkedHashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_TEXT));
            }

            public virtual IList<Rectangle> GetRectangles() {
                return rectangles;
            }

            public virtual void Clear() {
                rectangles.Clear();
            }
        }

        internal class MyCharacterEventListener : HighlightItemsTest.MyEventListener {
            public override void EventOccurred(IEventData data, EventType type) {
                if (type == EventType.RENDER_TEXT) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    foreach (TextRenderInfo tri in renderInfo.GetCharacterRenderInfos()) {
                        base.EventOccurred(tri, type);
                    }
                }
            }
        }
    }
}
