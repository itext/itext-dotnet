using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class PolygonSvgNodeRendererTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PolygonSvgNoderendererTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PolygonSvgNoderendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PolygonLineRendererTest() {
            String filename = "polygonLineRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgTagConstants.POINTS, "60,20 100,40 100,80 60,100 20,80 20,40");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PolygonLinkedPointCheckerImplicit() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgTagConstants.POINTS, "0,0 100,100 200,200 300,300");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            IList<Point> expectedPoints = new List<Point>();
            expectedPoints.Add(new Point(0, 0));
            expectedPoints.Add(new Point(75, 75));
            expectedPoints.Add(new Point(150, 150));
            expectedPoints.Add(new Point(225, 225));
            expectedPoints.Add(new Point(0, 0));
            IList<Point> attributePoints = ((PolygonSvgNodeRenderer)root).GetPoints();
            NUnit.Framework.Assert.AreEqual(expectedPoints.Count, attributePoints.Count);
            for (int x = 0; x < attributePoints.Count; x++) {
                NUnit.Framework.Assert.AreEqual(expectedPoints[x], attributePoints[x]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void PolygonLinkedPointCheckerExplicit() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgTagConstants.POINTS, "0,0 100,100 200,200 300,300 0,0");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            IList<Point> expectedPoints = new List<Point>();
            expectedPoints.Add(new Point(0, 0));
            expectedPoints.Add(new Point(75, 75));
            expectedPoints.Add(new Point(150, 150));
            expectedPoints.Add(new Point(225, 225));
            expectedPoints.Add(new Point(0, 0));
            IList<Point> attributePoints = ((PolygonSvgNodeRenderer)root).GetPoints();
            NUnit.Framework.Assert.AreEqual(expectedPoints.Count, attributePoints.Count);
            for (int x = 0; x < attributePoints.Count; x++) {
                NUnit.Framework.Assert.AreEqual(expectedPoints[x], attributePoints[x]);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PolygonEmptyPointCheckerTest() {
            String filename = "polygonEmptyPointCheckerTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((PolygonSvgNodeRenderer)root).GetPoints().Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }
    }
}
