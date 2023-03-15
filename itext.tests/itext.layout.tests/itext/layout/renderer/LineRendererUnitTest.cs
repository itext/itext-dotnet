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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class LineRendererUnitTest : RendererUnitTest {
        private const double EPS = 1e-5;

        [NUnit.Framework.Test]
        public virtual void AdjustChildPositionsAfterReorderingSimpleTest01() {
            Document dummyDocument = CreateDummyDocument();
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 4)]
        public virtual void AdjustChildPositionsAfterReorderingTestWithPercentMargins01() {
            Document dummyDocument = CreateDummyDocument();
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
            Document dummyDocument = CreateDummyDocument();
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INLINE_BLOCK_ELEMENT_WILL_BE_CLIPPED)]
        public virtual void InlineBlockWithBigMinWidth01() {
            Document dummyDocument = CreateDummyDocument();
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
            Document document = CreateDummyDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            lineRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 100, 200, 200));
            lineRenderer.maxAscent = 150;
            lineRenderer.maxDescent = -50;
            TextRenderer childTextRenderer = new TextRenderer(new Text("Hello"));
            childTextRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            childTextRenderer.occupiedArea = new LayoutArea(1, new Rectangle(100, 50, 200, 200));
            childTextRenderer.yLineOffset = 150;
            childTextRenderer.SetProperty(Property.TEXT_RISE, 0f);
            lineRenderer.AddChild(childTextRenderer);
            lineRenderer.AdjustChildrenYLine();
            NUnit.Framework.Assert.AreEqual(100f, lineRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
            NUnit.Framework.Assert.AreEqual(100f, childTextRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void AdjustChildrenYLineImageChildHtmlModeTest() {
            Document document = CreateDummyDocument();
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(document.GetRenderer());
            lineRenderer.occupiedArea = new LayoutArea(1, new Rectangle(50, 50, 200, 200));
            lineRenderer.maxAscent = 150;
            lineRenderer.maxDescent = -50;
            PdfFormXObject xObject = new PdfFormXObject(new Rectangle(200, 200));
            Image img = new Image(xObject);
            ImageRenderer childImageRenderer = new ImageRenderer(img);
            childImageRenderer.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            childImageRenderer.occupiedArea = new LayoutArea(1, new Rectangle(50, 50, 200, 200));
            lineRenderer.AddChild(childImageRenderer);
            lineRenderer.AdjustChildrenYLine();
            NUnit.Framework.Assert.AreEqual(50f, lineRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
            //image should be on the baseline top 250 - maxAscent 150 = 100
            NUnit.Framework.Assert.AreEqual(100.0, childImageRenderer.GetOccupiedAreaBBox().GetBottom(), EPS);
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
            Document document = CreateDummyDocument();
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
            Document document = CreateDummyDocument();
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
            Document document = CreateDummyDocument();
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
            lineRenderer.SetParent(CreateDummyDocument().GetRenderer());
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

        [NUnit.Framework.Test]
        public virtual void MinMaxWidthEqualsActualMarginsBordersPaddings() {
            Text ranText = new Text("ran");
            ranText.SetProperty(Property.MARGIN_LEFT, new UnitValue(UnitValue.POINT, 8f));
            ranText.SetProperty(Property.MARGIN_RIGHT, new UnitValue(UnitValue.POINT, 10f));
            ranText.SetProperty(Property.BORDER_RIGHT, new SolidBorder(3));
            ranText.SetProperty(Property.PADDING_RIGHT, new UnitValue(UnitValue.POINT, 13f));
            TextRenderer ran = new TextRenderer(ranText);
            Text domText = new Text("dom");
            domText.SetProperty(Property.MARGIN_LEFT, new UnitValue(UnitValue.POINT, 17f));
            domText.SetProperty(Property.BORDER_LEFT, new SolidBorder(4));
            domText.SetProperty(Property.PADDING_LEFT, new UnitValue(UnitValue.POINT, 12f));
            domText.SetProperty(Property.MARGIN_RIGHT, new UnitValue(UnitValue.POINT, 2f));
            TextRenderer dom = new TextRenderer(domText);
            LayoutArea layoutArea = new LayoutArea(1, new Rectangle(AbstractRenderer.INF, AbstractRenderer.INF));
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.SetParent(CreateDummyDocument().GetRenderer());
            lineRenderer.AddChild(ran);
            lineRenderer.AddChild(dom);
            float countedMinWidth = lineRenderer.GetMinMaxWidth().GetMinWidth();
            LayoutResult result = lineRenderer.Layout(new LayoutContext(layoutArea));
            NUnit.Framework.Assert.AreEqual(result.GetOccupiedArea().GetBBox().GetWidth(), countedMinWidth, 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void SplitLineIntoGlyphsSimpleTest() {
            Document dummyDocument = CreateDummyDocument();
            TextRenderer dummy1 = CreateLayoutedTextRenderer("hello", dummyDocument);
            TextRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            TextRenderer dummy3 = CreateLayoutedTextRenderer("!!!", dummyDocument);
            IRenderer dummyImage1 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage2 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage3 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage4 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage5 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            LineRenderer toSplit = new LineRenderer();
            toSplit.AddChildRenderer(dummyImage1);
            toSplit.AddChildRenderer(dummyImage2);
            toSplit.AddChildRenderer(dummy1);
            toSplit.AddChildRenderer(dummyImage3);
            toSplit.AddChildRenderer(dummy2);
            toSplit.AddChildRenderer(dummy3);
            toSplit.AddChildRenderer(dummyImage4);
            toSplit.AddChildRenderer(dummyImage5);
            LineRenderer.LineSplitIntoGlyphsData splitIntoGlyphsData = LineRenderer.SplitLineIntoGlyphs(toSplit);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(dummyImage1, dummyImage2), splitIntoGlyphsData.GetStarterNonTextRenderers
                ());
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(dummyImage3), splitIntoGlyphsData.GetInsertAfterAndRemove
                (dummy1));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy1));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy2));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(dummyImage4, dummyImage5), splitIntoGlyphsData.GetInsertAfterAndRemove
                (dummy3));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy3));
            NUnit.Framework.Assert.AreEqual(13, splitIntoGlyphsData.GetLineGlyphs().Count);
        }

        [NUnit.Framework.Test]
        public virtual void SplitLineIntoGlyphsWithLineBreakTest() {
            Document dummyDocument = CreateDummyDocument();
            TextRenderer dummy1 = CreateLayoutedTextRenderer("hello", dummyDocument);
            TextRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            dummy2.line.Set(2, new Glyph('\n', 0, '\n'));
            TextRenderer dummy3 = CreateLayoutedTextRenderer("!!!", dummyDocument);
            IRenderer dummyImage1 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage2 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage3 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage4 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage5 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            LineRenderer toSplit = new LineRenderer();
            toSplit.AddChildRenderer(dummyImage1);
            toSplit.AddChildRenderer(dummyImage2);
            toSplit.AddChildRenderer(dummy1);
            toSplit.AddChildRenderer(dummyImage3);
            toSplit.AddChildRenderer(dummy2);
            toSplit.AddChildRenderer(dummy3);
            toSplit.AddChildRenderer(dummyImage4);
            toSplit.AddChildRenderer(dummyImage5);
            LineRenderer.LineSplitIntoGlyphsData splitIntoGlyphsData = LineRenderer.SplitLineIntoGlyphs(toSplit);
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(dummyImage1, dummyImage2), splitIntoGlyphsData.GetStarterNonTextRenderers
                ());
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(dummyImage3), splitIntoGlyphsData.GetInsertAfterAndRemove
                (dummy1));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy1));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy2));
            NUnit.Framework.Assert.IsNull(splitIntoGlyphsData.GetInsertAfterAndRemove(dummy3));
            NUnit.Framework.Assert.AreEqual(7, splitIntoGlyphsData.GetLineGlyphs().Count);
        }

        [NUnit.Framework.Test]
        public virtual void ReorderSimpleTest() {
            Document dummyDocument = CreateDummyDocument();
            IRenderer dummy1 = CreateLayoutedTextRenderer("hello", dummyDocument);
            IRenderer dummy2 = CreateLayoutedTextRenderer("world", dummyDocument);
            IRenderer dummy3 = CreateLayoutedTextRenderer("!!!", dummyDocument);
            IRenderer dummyImage1 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage2 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage3 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage4 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            IRenderer dummyImage5 = CreateLayoutedImageRenderer(100, 100, dummyDocument);
            LineRenderer toSplit = new LineRenderer();
            toSplit.AddChildRenderer(dummyImage1);
            toSplit.AddChildRenderer(dummyImage2);
            toSplit.AddChildRenderer(dummy1);
            toSplit.AddChildRenderer(dummyImage3);
            toSplit.AddChildRenderer(dummy2);
            toSplit.AddChildRenderer(dummy3);
            toSplit.AddChildRenderer(dummyImage4);
            toSplit.AddChildRenderer(dummyImage5);
            LineRenderer.LineSplitIntoGlyphsData splitIntoGlyphsData = LineRenderer.SplitLineIntoGlyphs(toSplit);
            LineRenderer.Reorder(toSplit, splitIntoGlyphsData, new int[] { 0, 1, 4, 3, 2, 6, 5, 8, 7, 10, 9, 11, 12 });
            // validate that all non text renderers are in place and all text renderers contains
            // the right revers ranges
            IList<IRenderer> childRenderers = toSplit.GetChildRenderers();
            NUnit.Framework.Assert.AreEqual(8, childRenderers.Count);
            NUnit.Framework.Assert.AreSame(dummyImage1, childRenderers[0]);
            NUnit.Framework.Assert.AreSame(dummyImage2, childRenderers[1]);
            IList<int[]> firstReverseRanges = ((TextRenderer)childRenderers[2]).GetReversedRanges();
            NUnit.Framework.Assert.AreEqual(1, firstReverseRanges.Count);
            NUnit.Framework.Assert.AreEqual(new int[] { 2, 4 }, firstReverseRanges[0]);
            NUnit.Framework.Assert.AreSame(dummyImage3, childRenderers[3]);
            IList<int[]> secondReverseRanges = ((TextRenderer)childRenderers[4]).GetReversedRanges();
            NUnit.Framework.Assert.AreEqual(2, secondReverseRanges.Count);
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 1 }, secondReverseRanges[0]);
            NUnit.Framework.Assert.AreEqual(new int[] { 2, 3 }, secondReverseRanges[1]);
            IList<int[]> thirdReverseRanges = ((TextRenderer)childRenderers[5]).GetReversedRanges();
            NUnit.Framework.Assert.IsNull(thirdReverseRanges);
            NUnit.Framework.Assert.AreSame(dummyImage4, childRenderers[6]);
            NUnit.Framework.Assert.AreSame(dummyImage5, childRenderers[7]);
        }
    }
}
