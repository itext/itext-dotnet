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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class FlexContainerRendererTest : ExtendedITextTest {
        private const float EPS = 0.0001F;

        [NUnit.Framework.Test]
        public virtual void WidthNotSetTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer = new DivRenderer(new Div());
            flexRenderer.AddChild(divRenderer);
            NUnit.Framework.Assert.AreEqual(0F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(0F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildOneChildTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            flexRenderer.AddChild(divRenderer);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void NestedFlexWrapReverseTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            FlexContainerRenderer flexRendererChild = new FlexContainerRenderer(new Div());
            FlexContainerRenderer flexRendererChildInner = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP_REVERSE);
            flexRendererChild.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP_REVERSE);
            flexRendererChildInner.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP_REVERSE);
            flexRendererChildInner.SetProperty(Property.WIDTH, new UnitValue(UnitValue.POINT, 100));
            TextRenderer textRenderer1 = new TextRenderer(new Text("1"));
            TextRenderer textRenderer2 = new TextRenderer(new Text("2"));
            TextRenderer textRenderer3 = new TextRenderer(new Text("3"));
            textRenderer1.SetProperty(Property.TEXT_RISE, 20F);
            textRenderer1.SetProperty(Property.CHARACTER_SPACING, 20F);
            textRenderer1.SetProperty(Property.WORD_SPACING, 20F);
            textRenderer1.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            textRenderer1.SetProperty(Property.FONT_SIZE, new UnitValue(UnitValue.POINT, 20));
            textRenderer1.SetProperty(Property.SPLIT_CHARACTERS, new DefaultSplitCharacters());
            textRenderer2.SetProperty(Property.TEXT_RISE, 20F);
            textRenderer2.SetProperty(Property.CHARACTER_SPACING, 20F);
            textRenderer2.SetProperty(Property.WORD_SPACING, 20F);
            textRenderer2.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            textRenderer2.SetProperty(Property.FONT_SIZE, new UnitValue(UnitValue.POINT, 20));
            textRenderer2.SetProperty(Property.SPLIT_CHARACTERS, new DefaultSplitCharacters());
            textRenderer3.SetProperty(Property.TEXT_RISE, 20F);
            textRenderer3.SetProperty(Property.CHARACTER_SPACING, 20F);
            textRenderer3.SetProperty(Property.WORD_SPACING, 20F);
            textRenderer3.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            textRenderer3.SetProperty(Property.FONT_SIZE, new UnitValue(UnitValue.POINT, 20));
            textRenderer3.SetProperty(Property.SPLIT_CHARACTERS, new DefaultSplitCharacters());
            flexRendererChildInner.AddChild(textRenderer1);
            flexRendererChildInner.AddChild(textRenderer1);
            flexRendererChildInner.AddChild(textRenderer2);
            flexRendererChildInner.AddChild(textRenderer2);
            flexRendererChildInner.AddChild(textRenderer3);
            flexRendererChildInner.AddChild(textRenderer3);
            flexRendererChild.AddChild(flexRendererChildInner);
            flexRenderer.AddChild(flexRendererChild);
            flexRenderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(100, 100))));
            IList<String> childRenderersLayout1 = new List<String>();
            foreach (IRenderer childRenderer in flexRenderer.GetChildRenderers()[0].GetChildRenderers()[0].GetChildRenderers
                ()) {
                childRenderersLayout1.Add(((Text)childRenderer.GetModelElement()).GetText());
            }
            flexRenderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(100, 100))));
            IList<String> childRenderersLayout2 = new List<String>();
            foreach (IRenderer childRenderer in flexRenderer.GetChildRenderers()[0].GetChildRenderers()[0].GetChildRenderers
                ()) {
                childRenderersLayout2.Add(((Text)childRenderer.GetModelElement()).GetText());
            }
            for (int i = 0; i < childRenderersLayout1.Count; i++) {
                NUnit.Framework.Assert.AreEqual(childRenderersLayout1[i], childRenderersLayout2[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenWithBordersMarginsPaddingsTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            SolidBorder border = new SolidBorder(5);
            divRenderer1.SetProperty(Property.BORDER_TOP, border);
            divRenderer1.SetProperty(Property.BORDER_RIGHT, border);
            divRenderer1.SetProperty(Property.BORDER_BOTTOM, border);
            divRenderer1.SetProperty(Property.BORDER_LEFT, border);
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            divRenderer2.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(10));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            divRenderer3.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(15));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(10));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(165F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(165F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToFlexRendererAndToChildManyChildrenWithBordersMarginsPaddingsTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            SolidBorder border = new SolidBorder(5);
            divRenderer1.SetProperty(Property.BORDER_TOP, border);
            divRenderer1.SetProperty(Property.BORDER_RIGHT, border);
            divRenderer1.SetProperty(Property.BORDER_BOTTOM, border);
            divRenderer1.SetProperty(Property.BORDER_LEFT, border);
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            divRenderer2.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(10));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            divRenderer3.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(15));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(10));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithRotationAngleTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.ROTATION_ANGLE, 10f);
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(104.892334F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(104.892334F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMinWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(71));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(71F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void DecreaseLayoutBoxAfterChildPlacementResultsOccupiedAreaNull() {
            FlexContainerRenderer splitRenderer = new FlexContainerRenderer(new Div());
            splitRenderer.occupiedArea = new LayoutArea(0, new Rectangle(0, 0));
            LayoutResult nothing = new LayoutResult(LayoutResult.NOTHING, null, splitRenderer, null);
            NUnit.Framework.Assert.IsNotNull(new FlexContainerRenderer(new Div()).GetOccupiedAreaInCaseNothingWasWrappedWithFull
                (nothing, splitRenderer));
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMinWidthBiggerThanMaxWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(150));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMaxWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(150));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMaxWidthLowerThanMinWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(100));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(100F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void GetNextRendererShouldBeOverriddenTest() {
            FlexContainerRenderer flexContainerRenderer = new _FlexContainerRenderer_341(new Div());
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(FlexContainerRenderer), flexContainerRenderer.GetNextRenderer().GetType
                ());
        }

        private sealed class _FlexContainerRenderer_341 : FlexContainerRenderer {
            public _FlexContainerRenderer_341(Div baseArg1)
                : base(baseArg1) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void HypotheticalCrossSizeCacheTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(150));
            FlexContainerRenderer flexRendererChild = new FlexContainerRenderer(new Div());
            flexRendererChild.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(150));
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(150));
            flexRendererChild.AddChild(divRenderer);
            flexRenderer.AddChild(flexRendererChild);
            // In general, it's possible that we might call layout more than once for 1 renderer
            flexRenderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(100, 0))));
            flexRendererChild.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(125));
            flexRenderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(200, 0))));
            // Test that hypotheticalCrossSizes can contain more than 1 value
            NUnit.Framework.Assert.IsNotNull(flexRendererChild.GetHypotheticalCrossSize(125F));
            NUnit.Framework.Assert.IsNotNull(flexRendererChild.GetHypotheticalCrossSize(150F));
        }

        [NUnit.Framework.Test]
        public virtual void MinMaxWidthForFlexRendererWithWrapTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(100));
            SolidBorder border = new SolidBorder(5);
            flexRenderer.SetProperty(Property.BORDER_TOP, border);
            flexRenderer.SetProperty(Property.BORDER_RIGHT, border);
            flexRenderer.SetProperty(Property.BORDER_BOTTOM, border);
            flexRenderer.SetProperty(Property.BORDER_LEFT, border);
            // line 1
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(30));
            divRenderer1.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            // line 2
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            // line 3
            DivRenderer divRenderer5 = new DivRenderer(new Div());
            divRenderer5.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(75));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            flexRenderer.AddChild(divRenderer5);
            flexRenderer.Layout(new LayoutContext(new LayoutArea(0, new Rectangle(100, 100))));
            MinMaxWidth minMaxWidth = flexRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(75F, minMaxWidth.GetChildrenMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(85F, minMaxWidth.GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100F, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(110F, minMaxWidth.GetMaxWidth(), EPS);
            flexRenderer.DeleteOwnProperty(Property.MAX_WIDTH);
            minMaxWidth = flexRenderer.GetMinMaxWidth();
            NUnit.Framework.Assert.AreEqual(90F, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100F, minMaxWidth.GetMaxWidth(), EPS);
        }
    }
}
