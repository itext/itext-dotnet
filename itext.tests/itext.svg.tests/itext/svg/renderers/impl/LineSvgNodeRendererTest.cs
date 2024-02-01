/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LineSvgNodeRendererTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/LineSvgNodeRendererTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/LineSvgNodeRendererTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererTest() {
            String filename = "lineSvgRendererTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "100");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "300");
            lineProperties.Put("y2", "800");
            lineProperties.Put("stroke", "green");
            lineProperties.Put("stroke-width", "25");
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LineWithEmpyAttributesTest() {
            String filename = "lineWithEmpyAttributesTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidAttributeTest01() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()));
            doc.AddNewPage();
            ISvgNodeRenderer root = new LineSvgNodeRenderer();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "1");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "notAnum");
            lineProperties.Put("y2", "alsoNotANum");
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => root.Draw(context));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.NAN, "notAnum"), e.Message
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 2)]
        public virtual void InvalidAttributeTest02() {
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            lineProperties.Put("x1", "100");
            lineProperties.Put("y1", "800");
            lineProperties.Put("x2", "1 0");
            lineProperties.Put("y2", "0 2 0");
            lineProperties.Put("stroke", "orange");
            String filename = "invalidAttributes02.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            LineSvgNodeRenderer root = new LineSvgNodeRenderer();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void EmptyPointsListTest() {
            String filename = "lineEmptyPointsListTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            doc.AddNewPage();
            ISvgNodeRenderer root = new LineSvgNodeRenderer();
            IDictionary<String, String> lineProperties = new Dictionary<String, String>();
            root.SetAttributesAndStyles(lineProperties);
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            root.Draw(context);
            doc.Close();
            int numPoints = ((LineSvgNodeRenderer)root).attributesAndStyles.Count;
            NUnit.Framework.Assert.AreEqual(numPoints, 0);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAttributeTest() {
            float expected = 0.75f;
            LineSvgNodeRenderer lineSvgNodeRenderer = new LineSvgNodeRenderer();
            IDictionary<String, String> attributes = new Dictionary<String, String>();
            attributes.Put("key", "1.0");
            float actual = lineSvgNodeRenderer.GetAttribute(attributes, "key");
            NUnit.Framework.Assert.AreEqual(expected, actual, 0f);
        }

        [NUnit.Framework.Test]
        public virtual void GetNotPresentAttributeTest() {
            float expected = 0f;
            LineSvgNodeRenderer lineSvgNodeRenderer = new LineSvgNodeRenderer();
            IDictionary<String, String> attributes = new Dictionary<String, String>();
            attributes.Put("key", "1.0");
            float actual = lineSvgNodeRenderer.GetAttribute(attributes, "notHere");
            NUnit.Framework.Assert.AreEqual(expected, actual, 0f);
        }
    }
}
