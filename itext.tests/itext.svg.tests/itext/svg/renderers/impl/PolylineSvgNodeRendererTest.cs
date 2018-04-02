using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    public class PolylineSvgNodeRendererTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PolylineSvgNodeRendererTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PolylineSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PolylineRendererTest() {
            String filename = "polylineRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgTagConstants.POINTS, "0,40 40,40 40,80 80,80 80,120 120,120 120,160");
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
        public virtual void PolyLineInvalidAttributeTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
                doc.AddNewPage();
                ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
                IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
                polyLineAttributes.Put(SvgTagConstants.POINTS, "0,0 notAnum,alsoNotANum");
                root.SetAttributesAndStyles(polyLineAttributes);
                SvgDrawContext context = new SvgDrawContext();
                PdfCanvas cv = new PdfCanvas(doc, 1);
                context.PushCanvas(cv);
                root.Draw(context);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        [NUnit.Framework.Test]
        public virtual void PolyLineInvalidAttributeTest02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
                doc.AddNewPage();
                ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
                IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
                polyLineAttributes.Put(SvgTagConstants.POINTS, "0,0 100,100 5, 20,30");
                root.SetAttributesAndStyles(polyLineAttributes);
                SvgDrawContext context = new SvgDrawContext();
                PdfCanvas cv = new PdfCanvas(doc, 1);
                context.PushCanvas(cv);
                root.Draw(context);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>());
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PolyLineEmptyPointsListTest() {
            String filename = "polyLineEmptyPointsListTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((PolylineSvgNodeRenderer)root).GetPoints().Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PolyPointCheckerTest() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgTagConstants.POINTS, "0,0 100,100 200,200 300,300");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext();
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            IList<Point> expectedPoints = new List<Point>();
            expectedPoints.Add(new Point(0, 0));
            expectedPoints.Add(new Point(100, 100));
            expectedPoints.Add(new Point(200, 200));
            expectedPoints.Add(new Point(300, 300));
            IList<Point> attributePoints = ((PolylineSvgNodeRenderer)root).GetPoints();
            NUnit.Framework.Assert.AreEqual(expectedPoints.Count, attributePoints.Count);
            for (int x = 0; x < attributePoints.Count; x++) {
                NUnit.Framework.Assert.AreEqual(expectedPoints[x], attributePoints[x]);
            }
        }
    }
}
