using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    public class OrphansWidowsTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/OrphansWidowsTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/OrphansWidowsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD)]
        public virtual void PrematureCallOfHandleViolatedOrphans() {
            ParagraphOrphansControl orphansControl = new ParagraphOrphansControl(2);
            orphansControl.HandleViolatedOrphans(new ParagraphRenderer(new Paragraph()), "");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD)]
        public virtual void PrematureCallOfHandleViolatedWidows() {
            ParagraphWidowsControl widowsControl = new ParagraphWidowsControl(2, 1, false);
            widowsControl.HandleViolatedWidows(new ParagraphRenderer(new Paragraph()), "");
        }

        [NUnit.Framework.Test]
        public virtual void Min3OrphansTest01LeftLines1() {
            RunMinThreeOrphansTest("min3OrphansTest01LeftLines1", 1);
        }

        [NUnit.Framework.Test]
        public virtual void Min3OrphansTest01LeftLines2() {
            RunMinThreeOrphansTest("min3OrphansTest01LeftLines2", 2);
        }

        [NUnit.Framework.Test]
        public virtual void Min3OrphansTest01LeftLines3() {
            RunMinThreeOrphansTest("min3OrphansTest01LeftLines3", 3);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.ORPHANS_CONSTRAINT_VIOLATED)]
        public virtual void ViolatedOrphans() {
            RunMinThreeOrphansTest("violatedOrphans", 1, true, false);
        }

        [NUnit.Framework.Test]
        public virtual void Min3WidowsTest01LeftLines1() {
            RunMinThreeWidowsTest("min3WidowsTest01LeftLines1", 1);
        }

        [NUnit.Framework.Test]
        public virtual void Min3WidowsTest01LeftLines2() {
            RunMinThreeWidowsTest("min3WidowsTest01LeftLines2", 2);
        }

        [NUnit.Framework.Test]
        public virtual void Min3WidowsTest01LeftLines3() {
            RunMinThreeWidowsTest("min3WidowsTest01LeftLines3", 3);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
        public virtual void ViolatedWidowsInabilityToFix() {
            RunMinThreeWidowsTest("violatedWidowsInabilityToFix", 1, false, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
        public virtual void ViolatedWidowsForcedPlacement() {
            RunMinThreeWidowsTest("violatedWidowsForcedPlacement", 1, true, true);
        }

        [NUnit.Framework.Test]
        public virtual void MarginCollapseAndOrphansRestriction() {
            RunMinThreeOrphansTest("marginCollapseAndOrphansRestriction", 3, false, true);
        }

        [NUnit.Framework.Test]
        public virtual void MultipleLayoutCallsProduceSameLayoutResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            int minAllowedWidows = 5;
            int overflowedToNextPageLinesNum = 2;
            ParagraphWidowsControl widowsControl = new ParagraphWidowsControl(minAllowedWidows, minAllowedWidows - overflowedToNextPageLinesNum
                , false);
            Paragraph widowsParagraph = new Paragraph(OrphansWidowsTestUtil.paraText).SetWidowsControl(widowsControl);
            IRenderer paragraphRenderer = widowsParagraph.CreateRendererSubTree().SetParent(document.GetRenderer());
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            float linesHeight = OrphansWidowsTestUtil.CalculateHeightForLinesNum(document, widowsParagraph, effectiveArea
                .GetWidth(), overflowedToNextPageLinesNum, false);
            Rectangle layoutAreaRect = new Rectangle(effectiveArea).SetHeight(linesHeight + 5);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, layoutAreaRect));
            LayoutResult firstLayoutResult = paragraphRenderer.Layout(layoutContext);
            LayoutResult secondLayoutResult = paragraphRenderer.Layout(layoutContext);
            // toString() comparison is used since it contains report on status, areaBreak and occupiedArea
            NUnit.Framework.Assert.AreEqual(firstLayoutResult.ToString(), secondLayoutResult.ToString());
            ParagraphRenderer firstSplitRenderer = (ParagraphRenderer)firstLayoutResult.GetSplitRenderer();
            ParagraphRenderer secondSplitRenderer = (ParagraphRenderer)secondLayoutResult.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(firstSplitRenderer);
            NUnit.Framework.Assert.IsNotNull(secondSplitRenderer);
            NUnit.Framework.Assert.AreEqual(firstSplitRenderer.ToString(), secondSplitRenderer.ToString());
            NUnit.Framework.Assert.IsNotNull(firstLayoutResult.GetOverflowRenderer());
            NUnit.Framework.Assert.IsNotNull(secondLayoutResult.GetOverflowRenderer());
        }

        [NUnit.Framework.Test]
        public virtual void OrphansWidowsAwareAndDirectLayoutProduceSameResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            int minAllowedWidows = 3;
            int overflowedToNextPageLinesNum = 5;
            Paragraph widowsParagraph = new Paragraph(OrphansWidowsTestUtil.paraText);
            IRenderer paragraphRenderer = widowsParagraph.CreateRendererSubTree().SetParent(document.GetRenderer());
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            float linesHeight = OrphansWidowsTestUtil.CalculateHeightForLinesNum(document, widowsParagraph, effectiveArea
                .GetWidth(), overflowedToNextPageLinesNum, false);
            Rectangle layoutAreaRect = new Rectangle(effectiveArea).SetHeight(linesHeight + 5);
            LayoutContext layoutContext = new LayoutContext(new LayoutArea(1, layoutAreaRect));
            LayoutResult noWidowsControlLayoutResult = paragraphRenderer.Layout(layoutContext);
            ParagraphWidowsControl widowsControl = new ParagraphWidowsControl(minAllowedWidows, 1, false);
            widowsParagraph.SetWidowsControl(widowsControl);
            LayoutResult widowsControlLayoutResult = paragraphRenderer.Layout(layoutContext);
            // toString() comparison is used since it contains report on status, areaBreak and occupiedArea
            NUnit.Framework.Assert.AreEqual(noWidowsControlLayoutResult.ToString(), widowsControlLayoutResult.ToString
                ());
            ParagraphRenderer firstSplitRenderer = (ParagraphRenderer)noWidowsControlLayoutResult.GetSplitRenderer();
            ParagraphRenderer secondSplitRenderer = (ParagraphRenderer)widowsControlLayoutResult.GetSplitRenderer();
            NUnit.Framework.Assert.IsNotNull(firstSplitRenderer);
            NUnit.Framework.Assert.IsNotNull(secondSplitRenderer);
            NUnit.Framework.Assert.AreEqual(firstSplitRenderer.ToString(), secondSplitRenderer.ToString());
            NUnit.Framework.Assert.IsNotNull(noWidowsControlLayoutResult.GetOverflowRenderer());
            NUnit.Framework.Assert.IsNotNull(widowsControlLayoutResult.GetOverflowRenderer());
        }

        private static void RunMinThreeOrphansTest(String testName, int linesLeft) {
            RunMinThreeOrphansTest(testName, linesLeft, false, false);
        }

        private static void RunMinThreeOrphansTest(String testName, int linesLeft, bool forcedPlacement, bool marginCollapseTestCase
            ) {
            Paragraph testPara = new Paragraph();
            testPara.SetOrphansControl(new ParagraphOrphansControl(3));
            if (forcedPlacement) {
                testPara.SetProperty(Property.FORCED_PLACEMENT, true);
            }
            RunTest(testName, linesLeft, true, testPara, marginCollapseTestCase);
        }

        private static void RunMinThreeWidowsTest(String testName, int linesLeft) {
            RunMinThreeWidowsTest(testName, linesLeft, false, true);
        }

        private static void RunMinThreeWidowsTest(String testName, int linesLeft, bool forcedPlacement, bool overflowParagraphOnViolation
            ) {
            Paragraph testPara = new Paragraph();
            testPara.SetWidowsControl(new ParagraphWidowsControl(3, 1, overflowParagraphOnViolation));
            if (forcedPlacement) {
                testPara.SetProperty(Property.FORCED_PLACEMENT, true);
            }
            RunTest(testName, linesLeft, false, testPara);
        }

        private static void RunTest(String testName, int linesLeft, bool orphans, Paragraph testPara) {
            RunTest(testName, linesLeft, orphans, testPara, false);
        }

        private static void RunTest(String testName, int linesLeft, bool orphans, Paragraph testPara, bool marginCollapseTestCase
            ) {
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsTestCase(outPdf, linesLeft, orphans, testPara, marginCollapseTestCase
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }
    }
}
