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

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateDestinationFolder(outputPath);
        }

        [NUnit.Framework.Test]
        public virtual void HighlightPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229.pdf";
            String cmp = sourceFolder + "cmp_page229.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229_characters.pdf";
            String cmp = sourceFolder + "cmp_page229_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightCharactersHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter_characters.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage2Test() {
            String input = sourceFolder + "reference_page2.pdf";
            String output = outputPath + "reference_page2_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page2_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage832Test() {
            String input = sourceFolder + "reference_page832.pdf";
            String output = outputPath + "reference_page832_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page832_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage604Test() {
            String input = sourceFolder + "reference_page604.pdf";
            String output = outputPath + "reference_page604_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page604_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HighlightNotDefTest() {
            String input = sourceFolder + "notdefWidth.pdf";
            String output = outputPath + "notdefWidth_highlighted.pdf";
            String cmp = sourceFolder + "cmp_notdefWidth.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingType1NoDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingType1NoDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingType1NoDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingType1NoDescriptorTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingTrueTypeFontDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingTrueTypeFontDescriptorTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FillStandardEncodingType1FontDescriptorTest() {
            String input = sourceFolder + "fillStandardEncodingType1FontDescriptorTest.pdf";
            String output = outputPath + "fillStandardEncodingType1FontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_fillStandardEncodingType1FontDescriptorTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectAscentFontDescriptorTest() {
            // As the negative ascent is not covered by pdf specification in details,
            // we work with it as usual (which results with not very beautiful view).
            String input = sourceFolder + "incorrectAscentFontDescriptorTest.pdf";
            String output = outputPath + "incorrectAscentFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_incorrectAscentFontDescriptorTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectDescentFontDescriptorTest() {
            String input = sourceFolder + "incorrectDescentFontDescriptorTest.pdf";
            String output = outputPath + "incorrectDescentFontDescriptorTest.pdf";
            String cmp = sourceFolder + "cmp_incorrectDescentFontDescriptorTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void FontDictWidthArrayMissingWidthTest() {
            String input = sourceFolder + "fontDictWidthArrayMissingWidthTest.pdf";
            String output = outputPath + "fontDictWidthArrayMissingWidthTest.pdf";
            String cmp = sourceFolder + "cmp_fontDictWidthArrayMissingWidthTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeCIDFontWithDWWithoutProperWidthGlyphTest() {
            String input = sourceFolder + "trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            String output = outputPath + "trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            String cmp = sourceFolder + "cmp_trueTypeCIDFontWithDWWithoutProperWidthGlyphTest.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidHighlightTest() {
            //TODO: DEVSIX-4784 (incorrect displaying of highlights)
            String input = sourceFolder + "invalidHighlight.pdf";
            String output = outputPath + "invalidHighlightOutput.pdf";
            String cmp = sourceFolder + "cmp_invalidHighlight.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, outputPath));
        }

        private void ParseAndHighlight(String input, String output, bool singleCharacters) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), new PdfWriter(output));
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
