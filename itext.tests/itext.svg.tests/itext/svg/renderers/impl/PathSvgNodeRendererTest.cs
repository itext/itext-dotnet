/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PathSvgNodeRendererTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PathSvgNodeRendererTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/PathSvgNodeRendererTest/";

        private ISvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest() {
            String filename = "pathNodeRendererMoveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M 100,100, L300,100,L200,300,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest1() {
            String filename = "pathNodeRendererMoveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M 100 100 l300 100 L200 300 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest() {
            String filename = "pathNodeRendererCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M100,200 C100,100 250,100 250,200 S400,300 400,200,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest1() {
            String filename = "pathNodeRendererCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M100 200 C100 300 250 300 250 200 S400 100 400 200 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest() {
            String filename = "pathNodeRendererQCurveToCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M200,300 Q400,50 600,300,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest1() {
            String filename = "pathNodeRendererQCurveToCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M200 300 Q400 50 600 300 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            String result = new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename, DESTINATION_FOLDER, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest1() {
            String filename = "smoothCurveTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest1.svg";
            Stream xmlStream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + svgFilename);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IBranchSvgNodeRenderer root = (IBranchSvgNodeRenderer)processor.Process(rootTag, null).GetRootRenderer();
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest2() {
            String filename = "smoothCurveTest2.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest2.svg";
            Stream xmlStream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + svgFilename);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IBranchSvgNodeRenderer root = (IBranchSvgNodeRenderer)processor.Process(rootTag, null).GetRootRenderer();
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest3() {
            String filename = "smoothCurveTest3.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest3.svg";
            Stream xmlStream = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + svgFilename);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            IBranchSvgNodeRenderer root = (IBranchSvgNodeRenderer)processor.Process(rootTag, null).GetRootRenderer();
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveComplexTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "curves");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorMultipleZTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathZOperatorMultipleZTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorSingleZTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathZOperatorSingleZTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorSingleZInstructionsAfterTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathZOperatorSingleZInstructionsAfterTest");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidZOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "invalidZOperatorTest01"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "invalidOperatorTest01"));
        }

        [NUnit.Framework.Test]
        public virtual void PathLOperatorMultipleCoordinates() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathLOperatorMultipleCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void PathVOperatorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathVOperatorTest01");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorContinuePathingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathZOperatorContinuePathingTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathVOperatorMultipleArgumentsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathVOperatorMultipleArgumentsTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorSimpleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHOperatorSimpleTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHandVOperatorTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHandVOperatorTest");
        }

        [NUnit.Framework.Test]
        public virtual void CurveToContinuePathingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "curveToContinuePathingTest");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeHorizontalLineToTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeHorizontalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeVerticalLineToTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "relativeVerticalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void CombinedRelativeVerticalLineToAndRelativeHorizontalLineToTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "combinedRelativeVerticalLineToAndRelativeHorizontalLineTo"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRelativeHorizontalLineToTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleRelativeHorizontalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRelativeVerticalLineToTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multipleRelativeVerticalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void MoveToRelativeMultipleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "moveToRelativeMultiple");
        }

        [NUnit.Framework.Test]
        public virtual void MoveToAbsoluteMultipleTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "moveToAbsoluteMultiple");
        }

        [NUnit.Framework.Test]
        public virtual void ITextLogoTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "iTextLogo");
        }

        [NUnit.Framework.Test]
        public virtual void EofillUnsuportedPathTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "eofillUnsuportedPathTest"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiplePairsAfterMoveToRelativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multiplePairsAfterMoveToRelative");
        }

        [NUnit.Framework.Test]
        public virtual void MultiplePairsAfterMoveToAbsoluteTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "multiplePairsAfterMoveToAbsolute");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorAbsoluteAfterMultiplePairsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHOperatorAbsoluteAfterMultiplePairs");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorRelativeAfterMultiplePairsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHOperatorRelativeAfterMultiplePairs");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHref");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternContentUnits1Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefPatternContentUnits1");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternContentUnits2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefPatternContentUnits2");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefPatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPreserveAR1Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefPreserveAR1", properties);
        }

        [NUnit.Framework.Test]
        public virtual void PatternXlinkHrefPreserveAR2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefPreserveAR2", properties);
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePatternUnitsTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePatternUnits");
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePCUTopLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePCUTopLayer");
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePCUBottomLayerTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePCUBottomLayer");
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePCU2Test() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePCU2");
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePresAR1Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePresAR1", properties);
        }

        [NUnit.Framework.Test]
        public virtual void PatternHrefTransitivePresAR2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "patternHrefTransitivePresAR2", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClosedPathIsCutTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "closedPathIsCutTest", properties);
        }
    }
}
