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
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Testutil;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD)]
        public virtual void PrematureCallOfHandleViolatedOrphans() {
            ParagraphOrphansControl orphansControl = new ParagraphOrphansControl(2);
            orphansControl.HandleViolatedOrphans(new ParagraphRenderer(new Paragraph()), "");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD)]
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ORPHANS_CONSTRAINT_VIOLATED)]
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
        public virtual void ViolatedWidowsInabilityToFix() {
            RunMinThreeWidowsTest("violatedWidowsInabilityToFix", 1, false, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
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
            Paragraph widowsParagraph = new Paragraph(OrphansWidowsTestUtil.PARA_TEXT).SetWidowsControl(widowsControl);
            IRenderer paragraphRenderer = widowsParagraph.CreateRendererSubTree().SetParent(document.GetRenderer());
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            float linesHeight = OrphansWidowsTestUtil.CalculateHeightForLinesNum(document, widowsParagraph, effectiveArea
                .GetWidth(), overflowedToNextPageLinesNum, false);
            Rectangle layoutAreaRect = new Rectangle(effectiveArea).SetHeight(linesHeight + OrphansWidowsTestUtil.LINES_SPACE_EPS
                );
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
            ParagraphRenderer firstOverflowRenderer = (ParagraphRenderer)firstLayoutResult.GetOverflowRenderer();
            ParagraphRenderer secondOverflowRenderer = (ParagraphRenderer)secondLayoutResult.GetOverflowRenderer();
            NUnit.Framework.Assert.IsNotNull(firstOverflowRenderer);
            NUnit.Framework.Assert.IsNotNull(secondOverflowRenderer);
            IList<IRenderer> firstOverflowRendererChildren = firstOverflowRenderer.GetChildRenderers();
            IList<IRenderer> secondOverflowRendererChildren = secondOverflowRenderer.GetChildRenderers();
            NUnit.Framework.Assert.IsNotNull(firstOverflowRendererChildren);
            NUnit.Framework.Assert.IsNotNull(secondOverflowRendererChildren);
            NUnit.Framework.Assert.AreEqual(firstOverflowRendererChildren.Count, secondOverflowRendererChildren.Count);
        }

        [NUnit.Framework.Test]
        public virtual void OrphansWidowsAwareAndDirectLayoutProduceSameResult() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            Document document = new Document(pdfDocument);
            int minAllowedWidows = 3;
            int overflowedToNextPageLinesNum = 5;
            Paragraph widowsParagraph = new Paragraph(OrphansWidowsTestUtil.PARA_TEXT);
            IRenderer paragraphRenderer = widowsParagraph.CreateRendererSubTree().SetParent(document.GetRenderer());
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            float linesHeight = OrphansWidowsTestUtil.CalculateHeightForLinesNum(document, widowsParagraph, effectiveArea
                .GetWidth(), overflowedToNextPageLinesNum, false);
            Rectangle layoutAreaRect = new Rectangle(effectiveArea).SetHeight(linesHeight + OrphansWidowsTestUtil.LINES_SPACE_EPS
                );
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

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void MaxHeightLimitCausesOrphans() {
            RunMaxHeightLimit("maxHeightLimitCausesOrphans", true);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void MaxHeightLimitCausesWidows() {
            RunMaxHeightLimit("maxHeightLimitCausesWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void CanvasHeightCausesOrphansViolation() {
            RunCanvasSize("canvasHeightCausesOrphansViolation", true);
        }

        /* NOTE in this test the last possibly fitting line is removed as if to fix widows violation!
        * When the area is limited by highlevel conditions like paragraph's or div's size,
        * there's no attempt to fix orphans or widows. In this test case on the lowlevel canvas limitation
        * there is an attempt of fixing widows.
        */
        [NUnit.Framework.Test]
        public virtual void CanvasHeightCausesWidowsViolation() {
            RunCanvasSize("canvasHeightCausesWidowsViolation", false);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void DivSizeCausesOrphans() {
            RunDivSize("divSizeCausesOrphans", true);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void DivSizeCausesWidows() {
            RunDivSize("divSizeCausesWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherOrphans() {
            RunKeepTogether("keepTogetherOrphans", true, false);
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherWidows() {
            RunKeepTogether("keepTogetherWidows", false, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void KeepTogetherLargeParagraphOrphans() {
            RunKeepTogether("keepTogetherLargeParagraphOrphans", true, true);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void KeepTogetherLargeParagraphWidows() {
            RunKeepTogether("keepTogetherLargeParagraphWidows", false, true);
        }

        [NUnit.Framework.Test]
        public virtual void InlineImageOrphans() {
            RunInlineImage("inlineImageOrphans", true);
        }

        [NUnit.Framework.Test]
        public virtual void InlineImageWidows() {
            RunInlineImage("inlineImageWidows", false);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void HugeInlineImageOrphans() {
            RunHugeInlineImage("hugeInlineImageOrphans", true);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
        public virtual void HugeInlineImageWidows() {
            RunHugeInlineImage("hugeInlineImageWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void CustomParagraphAndRendererOrphans() {
            RunCustomParagraphAndRendererTest("customParagraphAndRendererOrphans", true);
        }

        [NUnit.Framework.Test]
        public virtual void CustomParagraphAndRendererWidows() {
            RunCustomParagraphAndRendererTest("customParagraphAndRendererWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedlyWideNextArea() {
            RunUnexpectedWidthOfNextAreaTest("unexpectedlyWideNextArea", true);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedlyNarrowNextArea() {
            RunUnexpectedWidthOfNextAreaTest("unexpectedlyNarrowNextArea", false);
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlockOrphans() {
            RunInlineBlockTest("inlineBlockOrphans", true);
        }

        [NUnit.Framework.Test]
        public virtual void InlineBlockWidows() {
            RunInlineBlockTest("inlineBlockWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void InlineFloatOrphans() {
            RunInlineFloatTest("inlineFloatOrphans", true);
        }

        [NUnit.Framework.Test]
        public virtual void InlineFloatWidows() {
            RunInlineFloatTest("inlineFloatWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void FloatingDivOrphans() {
            RunFloatingDiv("floatingDivOrphans", true);
        }

        [NUnit.Framework.Test]
        public virtual void FloatingDivWidows() {
            RunFloatingDiv("floatingDivWidows", false);
        }

        [NUnit.Framework.Test]
        public virtual void SingleLineParagraphOrphans() {
            RunOrphansWidowsBiggerThanLinesCount("singleLineParagraphOrphans", true, true);
        }

        [NUnit.Framework.Test]
        public virtual void SingleLineParagraphWidows() {
            RunOrphansWidowsBiggerThanLinesCount("singleLineParagraphWidows", false, true);
        }

        [NUnit.Framework.Test]
        public virtual void TwoLinesParagraphMin3Orphans() {
            RunOrphansWidowsBiggerThanLinesCount("twoLinesParagraphMin3Orphans", true, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED)]
        public virtual void TwoLinesParagraphMin3Widows() {
            RunOrphansWidowsBiggerThanLinesCount("twoLinesParagraphMin3Widows", false, false);
        }

        [NUnit.Framework.Test]
        public virtual void OrphansAndWidowsTest() {
            RunOrphansAndWidows("orphansAndWidowsTest");
        }

        [NUnit.Framework.Test]
        public virtual void WidowsControlOnPagesTest() {
            String testName = "widowsControlOnPagesTest";
            Paragraph testPara = new Paragraph();
            testPara.SetWidowsControl(new ParagraphWidowsControl(3, 1, true));
            RunTestOnPage(testName, testPara, false);
        }

        [NUnit.Framework.Test]
        public virtual void OrphansControlOnPagesTest() {
            String testName = "orphansControlOnPagesTest";
            Paragraph testPara = new Paragraph();
            testPara.SetOrphansControl(new ParagraphOrphansControl(3));
            RunTestOnPage(testName, testPara, true);
        }

        private static void RunMaxHeightLimit(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsAndMaxHeightLimitTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunCanvasSize(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsOnCanvasOfLimitedSizeTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunDivSize(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsWithinDivOfLimitedSizeTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunKeepTogether(String fileName, bool orphans, bool large) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsKeepTogetherTestCase(outPdf, orphans, large);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunInlineImage(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            String imagePath = sourceFolder + "bulb.gif";
            OrphansWidowsTestUtil.ProduceOrphansWidowsInlineImageTestCase(outPdf, imagePath, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunHugeInlineImage(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            String imagePath = sourceFolder + "imageA4.png";
            OrphansWidowsTestUtil.ProduceOrphansWidowsHugeInlineImageTestCase(outPdf, imagePath, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunCustomParagraphAndRendererTest(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTest.CustomParagraph customParagraph = new OrphansWidowsTest.CustomParagraph();
            if (orphans) {
                customParagraph.SetOrphansControl(new ParagraphOrphansControl(3));
            }
            else {
                customParagraph.SetWidowsControl(new ParagraphWidowsControl(3, 1, false));
            }
            OrphansWidowsTestUtil.ProduceOrphansWidowsTestCase(outPdf, 2, orphans, customParagraph, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunUnexpectedWidthOfNextAreaTest(String fileName, bool wide) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsUnexpectedWidthOfNextAreaTestCase(outPdf, wide);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunInlineBlockTest(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsInlineBlockTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunInlineFloatTest(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsInlineFloatTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunFloatingDiv(String fileName, bool orphans) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsFloatingDivTestCase(outPdf, orphans);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunOrphansWidowsBiggerThanLinesCount(String fileName, bool orphans, bool singleLine) {
            String outPdf = destinationFolder + fileName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + fileName + ".pdf";
            OrphansWidowsTestUtil.ProduceOrphansWidowsBiggerThanLinesCountTestCase(outPdf, orphans, singleLine);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
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

        private static void RunOrphansAndWidows(String testName) {
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            Paragraph testPara = new Paragraph();
            testPara.SetOrphansControl(new ParagraphOrphansControl(3)).SetWidowsControl(new ParagraphWidowsControl(3, 
                1, true));
            OrphansWidowsTestUtil.ProduceOrphansAndWidowsTestCase(outPdf, testPara);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
        }

        private static void RunTestOnPage(String testName, Paragraph testPara, bool orphans) {
            String outPdf = destinationFolder + testName + ".pdf";
            String cmpPdf = sourceFolder + "cmp_" + testName + ".pdf";
            int linesLeft = 1;
            OrphansWidowsTestUtil.ProduceOrphansOrWidowsTestCase(outPdf, linesLeft, orphans, testPara);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"
                ));
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

        private class CustomParagraphRenderer : ParagraphRenderer {
            public CustomParagraphRenderer(OrphansWidowsTest.CustomParagraph modelElement)
                : base(modelElement) {
            }

            public override IRenderer GetNextRenderer() {
                return new OrphansWidowsTest.CustomParagraphRenderer((OrphansWidowsTest.CustomParagraph)this.modelElement);
            }
        }

        private class CustomParagraph : Paragraph {
            protected internal override IRenderer MakeNewRenderer() {
                return new OrphansWidowsTest.CustomParagraphRenderer(this);
            }
        }
    }
}
