/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class FlexContainerRendererTest : ExtendedITextTest {
        private static float EPS = 0.0001F;

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
            divRenderer1.SetProperty(Property.BORDER, new SolidBorder(5));
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
            divRenderer1.SetProperty(Property.BORDER, new SolidBorder(5));
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
            FlexContainerRenderer flexContainerRenderer = new _FlexContainerRenderer_271(new Div());
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(FlexContainerRenderer), flexContainerRenderer.GetNextRenderer().GetType
                ());
        }

        private sealed class _FlexContainerRenderer_271 : FlexContainerRenderer {
            public _FlexContainerRenderer_271(Div baseArg1)
                : base(baseArg1) {
            }
        }
    }
}
