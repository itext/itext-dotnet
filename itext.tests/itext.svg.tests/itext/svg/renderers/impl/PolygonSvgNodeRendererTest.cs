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
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PolygonSvgNodeRendererTest : SvgIntegrationTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PolygonSvgNoderendererTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PolygonSvgNoderendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PolygonLineRendererTest() {
            String filename = "polygonLineRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "60,20 100,40 100,80 60,100 20,80 20,40");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PolygonLinkedPointCheckerImplicit() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "0,0 100,100 200,200 300,300");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
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
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "0,0 100,100 200,200 300,300 0,0");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
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
        public virtual void PolygonEmptyPointCheckerTest() {
            String filename = "polygonEmptyPointCheckerTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((PolygonSvgNodeRenderer)root).GetPoints().Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ConnectPointsWithSameYCoordinateTest() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolygonSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "100,100 100,200 150,200 150,100");
            polyLineAttributes.Put(SvgConstants.Attributes.FILL, "none");
            polyLineAttributes.Put(SvgConstants.Attributes.STROKE, "black");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            IList<Point> expectedPoints = new List<Point>();
            expectedPoints.Add(new Point(75, 75));
            expectedPoints.Add(new Point(75, 150));
            expectedPoints.Add(new Point(112.5, 150));
            expectedPoints.Add(new Point(112.5, 75));
            expectedPoints.Add(new Point(75, 75));
            IList<Point> attributePoints = ((PolygonSvgNodeRenderer)root).GetPoints();
            NUnit.Framework.Assert.AreEqual(expectedPoints.Count, attributePoints.Count);
            for (int x = 0; x < attributePoints.Count; x++) {
                NUnit.Framework.Assert.AreEqual(expectedPoints[x], attributePoints[x]);
            }
        }
    }
}
