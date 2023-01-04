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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Exceptions;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PathSvgNodeRendererTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PathSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PathSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest() {
            String filename = "pathNodeRendererMoveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest1() {
            String filename = "pathNodeRendererMoveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest() {
            String filename = "pathNodeRendererCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest1() {
            String filename = "pathNodeRendererCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest() {
            String filename = "pathNodeRendererQCurveToCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest1() {
            String filename = "pathNodeRendererQCurveToCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
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
            String result = new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            if (result != null && !result.Contains("No visual differences")) {
                NUnit.Framework.Assert.Fail(result);
            }
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest1() {
            String filename = "smoothCurveTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest1.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
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
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest2.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
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
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest3.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
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
            ConvertAndCompare(sourceFolder, destinationFolder, "curves");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorMultipleZTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathZOperatorMultipleZTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorSingleZTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathZOperatorSingleZTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorSingleZInstructionsAfterTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathZOperatorSingleZInstructionsAfterTest");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidZOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(sourceFolder, destinationFolder
                , "invalidZOperatorTest01"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(sourceFolder, destinationFolder
                , "invalidOperatorTest01"));
        }

        //TODO DEVSIX-2242. This test should fail when the ticket is resolved
        [NUnit.Framework.Test]
        public virtual void PathLOperatorMultipleCoordinates() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathLOperatorMultipleCoordinates");
        }

        [NUnit.Framework.Test]
        public virtual void PathVOperatorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathVOperatorTest01");
        }

        [NUnit.Framework.Test]
        public virtual void PathZOperatorContinuePathingTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathZOperatorContinuePathingTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathVOperatorMultipleArgumentsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathVOperatorMultipleArgumentsTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorSimpleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHOperatorSimpleTest");
        }

        [NUnit.Framework.Test]
        public virtual void PathHandVOperatorTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHandVOperatorTest");
        }

        [NUnit.Framework.Test]
        public virtual void CurveToContinuePathingTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "curveToContinuePathingTest");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeHorizontalLineToTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "relativeHorizontalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void RelativeVerticalLineToTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "relativeVerticalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void CombinedRelativeVerticalLineToAndRelativeHorizontalLineToTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "combinedRelativeVerticalLineToAndRelativeHorizontalLineTo"
                );
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRelativeHorizontalLineToTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "multipleRelativeHorizontalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRelativeVerticalLineToTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "multipleRelativeVerticalLineTo");
        }

        [NUnit.Framework.Test]
        public virtual void MoveToRelativeMultipleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "moveToRelativeMultiple");
        }

        [NUnit.Framework.Test]
        public virtual void MoveToAbsoluteMultipleTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "moveToAbsoluteMultiple");
        }

        [NUnit.Framework.Test]
        public virtual void ITextLogoTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "iTextLogo");
        }

        [NUnit.Framework.Test]
        public virtual void EofillUnsuportedPathTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(sourceFolder, destinationFolder
                , "eofillUnsuportedPathTest"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiplePairsAfterMoveToRelativeTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "multiplePairsAfterMoveToRelative");
        }

        [NUnit.Framework.Test]
        public virtual void MultiplePairsAfterMoveToAbsoluteTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "multiplePairsAfterMoveToAbsolute");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorAbsoluteAfterMultiplePairsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHOperatorAbsoluteAfterMultiplePairs");
        }

        [NUnit.Framework.Test]
        public virtual void PathHOperatorRelativeAfterMultiplePairsTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHOperatorRelativeAfterMultiplePairs");
        }
    }
}
