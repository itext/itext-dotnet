/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    public class LineRendererUnitTest : AbstractRendererUnitTest {
        private const double EPS = 1e-5;

        [NUnit.Framework.Test]
        public virtual void AdjustChildPositionsAfterReorderingSimpleTest01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            IRenderer dummyImage = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummyImage, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(37.3359985, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(137.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 4)]
        public virtual void AdjustChildPositionsAfterReorderingTestWithPercentMargins01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            dummy1.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePercentValue(10));
            dummy1.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePercentValue(10));
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            // If margins and paddings are specified in percents, we treat them as point values for now
            NUnit.Framework.Assert.AreEqual(77.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void AdjustChildPositionsAfterReorderingTestWithFloats01() {
            Document dummyDocument = CreateDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("Hello", dummyDocument);
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            IRenderer dummyImage = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            dummyImage.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            NUnit.Framework.Assert.AreEqual(0, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            LineRenderer.AdjustChildPositionsAfterReordering(JavaUtil.ArraysAsList(dummy1, dummyImage, dummy2), 10);
            NUnit.Framework.Assert.AreEqual(10, dummy1.GetOccupiedArea().GetBBox().GetX(), EPS);
            // Floating renderer is not repositioned
            NUnit.Framework.Assert.AreEqual(0, dummyImage.GetOccupiedArea().GetBBox().GetX(), EPS);
            NUnit.Framework.Assert.AreEqual(37.3359985, dummy2.GetOccupiedArea().GetBBox().GetX(), EPS);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED)]
        public virtual void InlineBlockWithBigMinWidth01() {
            Document dummyDocument = CreateDocument();
            LineRenderer lineRenderer = (LineRenderer)new LineRenderer().SetParent(dummyDocument.GetRenderer());
            Div div = new Div().SetMinWidth(2000).SetHeight(100);
            DivRenderer inlineBlockRenderer = (DivRenderer)div.CreateRendererSubTree();
            lineRenderer.AddChild(inlineBlockRenderer);
            LayoutResult result = lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000)));
            // In case there is an inline block child with large min-width, the inline block child will be force placed (not layouted properly)
            NUnit.Framework.Assert.AreEqual(LayoutResult.FULL, result.GetStatus());
            NUnit.Framework.Assert.AreEqual(0, result.GetOccupiedArea().GetBBox().GetHeight(), EPS);
            NUnit.Framework.Assert.AreEqual(true, inlineBlockRenderer.GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
        }

        [NUnit.Framework.Test]
        public virtual void AdjustChildrenYLineTextChildHtmlModeTest() {
            Document document = CreateDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 100, 200, 200));
            lineRenderer.maxAscent = 100;
            TextRenderer childTextRenderer = new TextRenderer(new Text("Hello"));
            childTextRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            childTextRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 50, 200, 200));
            childTextRenderer.yLineOffset = 100;
            childTextRenderer.SetProperty(Property.TEXT_RISE, 0f);
            lineRenderer.AddChild(childTextRenderer);
            lineRenderer.AdjustChildrenYLine();
            NUnit.Framework.Assert.AreEqual(100f, lineRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
            NUnit.Framework.Assert.AreEqual(100f, childTextRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void AdjustChildrenYLineImageChildHtmlModeTest() {
            Document document = CreateDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.occupiedArea = new LayoutArea(1, new Rectangle(50, 50, 200, 200));
            lineRenderer.maxAscent = 100;
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(200, 200));
            Image img = new Image(xObject);
            ImageRenderer childImageRenderer = new ImageRenderer(img);
            childImageRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            childImageRenderer.occupiedArea = new LayoutArea(1, new Rectangle(50, 50, 200, 200));
            lineRenderer.AddChild(childImageRenderer);
            lineRenderer.AdjustChildrenYLine();
            NUnit.Framework.Assert.AreEqual(50f, lineRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
            NUnit.Framework.Assert.AreEqual(150.0, childImageRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void HasChildRendererInHtmlModeTest() {
            LineRenderer lineRenderer = new LineRenderer();
            TextRenderer textRenderer1 = new TextRenderer(new Text("text1"));
            TextRenderer textRenderer2 = new TextRenderer(new Text("text2"));
            textRenderer2.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            lineRenderer.AddChild(textRenderer1);
            lineRenderer.AddChild(textRenderer2);
            NUnit.Framework.Assert.IsTrue(lineRenderer.HasChildRendererInHtmlMode());
        }

        [NUnit.Framework.Test]
        public virtual void ChildRendererInDefaultModeTest() {
            LineRenderer lineRenderer = new LineRenderer();
            TextRenderer textRenderer1 = new TextRenderer(new Text("text1"));
            TextRenderer textRenderer2 = new TextRenderer(new Text("text2"));
            textRenderer2.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
            lineRenderer.AddChild(textRenderer1);
            lineRenderer.AddChild(textRenderer2);
            NUnit.Framework.Assert.IsFalse(lineRenderer.HasChildRendererInHtmlMode());
        }

        [NUnit.Framework.Test]
        public virtual void HasChildRendererInHtmlModeNoChildrenTest() {
            LineRenderer lineRenderer = new LineRenderer();
            NUnit.Framework.Assert.IsFalse(lineRenderer.HasChildRendererInHtmlMode());
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererLayoutInHtmlModeWithLineHeightAndNoChildrenTest() {
            Document document = CreateDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            lineRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000)));
            NUnit.Framework.Assert.AreEqual(0f, lineRenderer.maxAscent, 0f);
            NUnit.Framework.Assert.AreEqual(0f, lineRenderer.maxDescent, 0f);
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererLayoutInHtmlModeWithLineHeightAndChildrenInDefaultModeTest() {
            Document document = CreateDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            lineRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateFixedValue(50));
            TextRenderer textRenderer1 = new TextRenderer(new Text("text"));
            textRenderer1.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
            TextRenderer textRenderer2 = new TextRenderer(new Text("text"));
            textRenderer2.SetProperty(Property.RENDERING_MODE, RenderingMode.DEFAULT_LAYOUT_MODE);
            lineRenderer.AddChild(textRenderer1);
            lineRenderer.AddChild(textRenderer2);
            lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000)));
            NUnit.Framework.Assert.AreEqual(10.3392f, lineRenderer.maxAscent, EPS);
            NUnit.Framework.Assert.AreEqual(-2.98079f, lineRenderer.maxDescent, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererLayoutInHtmlModeWithLineHeightAndChildInHtmlModeTest() {
            Document document = CreateDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            lineRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateFixedValue(50));
            TextRenderer textRenderer1 = new TextRenderer(new Text("text"));
            textRenderer1.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            TextRenderer textRenderer2 = new TextRenderer(new Text("text"));
            lineRenderer.AddChild(textRenderer1);
            lineRenderer.AddChild(textRenderer2);
            lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000)));
            NUnit.Framework.Assert.AreEqual(28.67920f, lineRenderer.maxAscent, EPS);
            NUnit.Framework.Assert.AreEqual(-21.32080f, lineRenderer.maxDescent, EPS);
        }

        [NUnit.Framework.Test]
        public virtual void LineRendererLayoutInHtmlModeWithLineHeightPropertyNotSet() {
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(CreateDocument().GetRenderer());
            lineRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            // Set fonts with different ascent/descent to line and text
            lineRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            TextRenderer textRenderer = new TextRenderer(new Text("text"));
            textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.COURIER));
            lineRenderer.AddChild(textRenderer);
            LayoutResult layoutResLineHeightNotSet = lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000
                )));
            lineRenderer.SetProperty(Property.LINE_HEIGHT, LineHeight.CreateNormalValue());
            LayoutResult layoutResLineHeightNormal = lineRenderer.Layout(new LayoutContext(CreateLayoutArea(1000, 1000
                )));
            Rectangle bboxLineHeightNotSet = layoutResLineHeightNotSet.GetOccupiedArea().GetBBox();
            Rectangle bboxLineHeightNormal = layoutResLineHeightNormal.GetOccupiedArea().GetBBox();
            NUnit.Framework.Assert.IsTrue(bboxLineHeightNotSet.EqualsWithEpsilon(bboxLineHeightNormal));
        }
    }
}
