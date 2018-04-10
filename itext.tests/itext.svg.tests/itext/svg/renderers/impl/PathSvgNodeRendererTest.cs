using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class PathSvgNodeRendererTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PathSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PathSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest() {
            String filename = "pathNodeRendererMoveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M 100,100, L300,100,L200,300,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererMoveToTest1() {
            String filename = "pathNodeRendererMoveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M 100 100 l300 100 L200 300 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest() {
            String filename = "pathNodeRendererCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M100,200 C100,100 250,100 250,200 S400,300 400,200,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveToTest1() {
            String filename = "pathNodeRendererCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M100 200 C100 300 250 300 250 200 S400 100 400 200 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest() {
            String filename = "pathNodeRendererQCurveToCurveToTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M200,300 Q400,50 600,300,z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererQCurveToCurveToTest1() {
            String filename = "pathNodeRendererQCurveToCurveToTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> pathShapes = new Dictionary<String, String>();
            pathShapes.Put("d", "M200 300 Q400 50 600 300 z");
            ISvgNodeRenderer pathRenderer = new PathSvgNodeRenderer();
            pathRenderer.SetAttributesAndStyles(pathShapes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            pathRenderer.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest1() {
            String filename = "smoothCurveTest1.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest1.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer root = processor.Process(rootTag);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest2() {
            String filename = "smoothCurveTest2.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest2.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer root = processor.Process(rootTag);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SmoothCurveTest3() {
            String filename = "smoothCurveTest3.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            String svgFilename = "smoothCurveTest3.svg";
            Stream xmlStream = new FileStream(sourceFolder + svgFilename, FileMode.Open, FileAccess.Read);
            IElementNode rootTag = new JsoupXmlParser().Parse(xmlStream, "ISO-8859-1");
            DefaultSvgProcessor processor = new DefaultSvgProcessor();
            ISvgNodeRenderer root = processor.Process(rootTag);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.IsTrue(root.GetChildren()[0] is PathSvgNodeRenderer);
            root.GetChildren()[0].Draw(context);
            doc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PathNodeRendererCurveComplexTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(sourceFolder, destinationFolder, "curves");
        }
    }
}
