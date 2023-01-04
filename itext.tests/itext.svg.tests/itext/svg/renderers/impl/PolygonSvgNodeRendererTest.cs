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
