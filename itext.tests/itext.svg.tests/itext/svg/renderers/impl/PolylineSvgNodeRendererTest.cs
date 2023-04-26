/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Exceptions;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PolylineSvgNodeRendererTest : SvgIntegrationTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PolylineSvgNodeRendererTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PolylineSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PolylineRendererTest() {
            String filename = "polylineRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "0,40 40,40 40,80 80,80 80,120 120,120 120,160");
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
        public virtual void PolyLineInvalidAttributeTest01() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "0,0 notAnum,alsoNotANum");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => root.Draw(context));
        }

        [NUnit.Framework.Test]
        public virtual void PolyLineInvalidAttributeTest02() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            polyLineAttributes.Put(SvgConstants.Attributes.POINTS, "0,0 100,100 5, 20,30");
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => root.Draw(context));
        }

        [NUnit.Framework.Test]
        public virtual void PolyLineEmptyPointsListTest() {
            String filename = "polyLineEmptyPointsListTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
            IDictionary<String, String> polyLineAttributes = new Dictionary<String, String>();
            root.SetAttributesAndStyles(polyLineAttributes);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((PolylineSvgNodeRenderer)root).GetPoints().Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PolyPointCheckerTest() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new PolylineSvgNodeRenderer();
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
            IList<Point> attributePoints = ((PolylineSvgNodeRenderer)root).GetPoints();
            NUnit.Framework.Assert.AreEqual(expectedPoints.Count, attributePoints.Count);
            for (int x = 0; x < attributePoints.Count; x++) {
                NUnit.Framework.Assert.AreEqual(expectedPoints[x], attributePoints[x]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ElementDimensionExceedsViewboxBoundaryTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "elementDimensionExceedsViewboxBoundary");
        }
    }
}
