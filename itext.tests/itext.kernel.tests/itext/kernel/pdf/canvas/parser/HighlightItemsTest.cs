using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser {
    public class HighlightItemsTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/HighlightItemsTest/";

        private static readonly String outputPath = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/parser/HighlightItemsTest/";

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            CreateDestinationFolder(outputPath);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229.pdf";
            String cmp = sourceFolder + "cmp_page229.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightCharactersPage229() {
            String input = sourceFolder + "page229.pdf";
            String output = outputPath + "page229_characters.pdf";
            String cmp = sourceFolder + "cmp_page229_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightCharactersIsoTc171() {
            String input = sourceFolder + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda.pdf";
            String output = outputPath + "ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            String cmp = sourceFolder + "cmp_ISO-TC171-SC2_N0896_SC2WG5_Edinburgh_Agenda_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightCharactersHeaderFooter() {
            String input = sourceFolder + "HeaderFooter.pdf";
            String output = outputPath + "HeaderFooter_characters.pdf";
            String cmp = sourceFolder + "cmp_HeaderFooter_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage2Test() {
            String input = sourceFolder + "reference_page2.pdf";
            String output = outputPath + "reference_page2_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page2_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage832Test() {
            String input = sourceFolder + "reference_page832.pdf";
            String output = outputPath + "reference_page832_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page832_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightReferencePage604Test() {
            String input = sourceFolder + "reference_page604.pdf";
            String output = outputPath + "reference_page604_characters.pdf";
            String cmp = sourceFolder + "cmp_reference_page604_characters.pdf";
            ParseAndHighlight(input, output, true);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void HighlightNotDefTest() {
            String input = sourceFolder + "notdefWidth.pdf";
            String output = outputPath + "notdefWidth_highlighted.pdf";
            String cmp = sourceFolder + "cmp_notdefWidth.pdf";
            ParseAndHighlight(input, output, false);
            NUnit.Framework.Assert.AreEqual(null, new CompareTool().CompareByContent(output, cmp, outputPath, "diff"));
        }

        /// <exception cref="System.IO.IOException"/>
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
                canvas.SetStrokeColor(Color.RED);
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
