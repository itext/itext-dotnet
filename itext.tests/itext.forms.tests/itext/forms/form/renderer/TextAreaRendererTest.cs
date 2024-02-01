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
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Form.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class TextAreaRendererTest : ExtendedITextTest {
        private const double EPS = 0.0001;

        [NUnit.Framework.Test]
        public virtual void ColsPropertyIsSetToNullTest() {
            TextAreaRenderer areaRenderer = new TextAreaRenderer(new TextArea(""));
            areaRenderer.SetProperty(FormProperty.FORM_FIELD_COLS, null);
            NUnit.Framework.Assert.AreEqual(20, areaRenderer.GetCols());
        }

        [NUnit.Framework.Test]
        public virtual void ColsPropertyIsSetToZeroTest() {
            TextAreaRenderer areaRenderer = new TextAreaRenderer(new TextArea(""));
            areaRenderer.SetProperty(FormProperty.FORM_FIELD_COLS, 0);
            NUnit.Framework.Assert.AreEqual(20, areaRenderer.GetCols());
        }

        [NUnit.Framework.Test]
        public virtual void RowsPropertyIsSetToNullTest() {
            TextAreaRenderer areaRenderer = new TextAreaRenderer(new TextArea(""));
            areaRenderer.SetProperty(FormProperty.FORM_FIELD_ROWS, null);
            NUnit.Framework.Assert.AreEqual(2, areaRenderer.GetRows());
        }

        [NUnit.Framework.Test]
        public virtual void RowsPropertyIsSetToZeroTest() {
            TextAreaRenderer areaRenderer = new TextAreaRenderer(new TextArea(""));
            areaRenderer.SetProperty(FormProperty.FORM_FIELD_ROWS, 0);
            NUnit.Framework.Assert.AreEqual(2, areaRenderer.GetRows());
        }

        [NUnit.Framework.Test]
        public virtual void GetRendererTest() {
            TextAreaRenderer areaRenderer = new TextAreaRenderer(new TextArea(""));
            IRenderer nextRenderer = areaRenderer.GetNextRenderer();
            NUnit.Framework.Assert.IsTrue(nextRenderer is TextAreaRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithAbsoluteWidthTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithoutAbsoluteWidthTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            areaRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(10));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithoutAbsoluteWidthOnElementTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            areaRenderer.GetModelElement().SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(10));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void GetLastYLineRecursivelyNoOccupiedAreaTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            float? lastY = areaRenderer.CallGetLastYLineRecursively();
            NUnit.Framework.Assert.IsNull(lastY);
        }

        [NUnit.Framework.Test]
        public virtual void GetLastYLineRecursivelyEmptyOccupiedAreaTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            areaRenderer.SetOccupiedArea(new LayoutArea(1, null));
            float? lastY = areaRenderer.CallGetLastYLineRecursively();
            NUnit.Framework.Assert.IsNull(lastY);
        }

        [NUnit.Framework.Test]
        public virtual void GetLastYLineRecursivelyWithOccupiedAreaTest() {
            TextAreaRendererTest.CustomTextAreaRenderer areaRenderer = new TextAreaRendererTest.CustomTextAreaRenderer
                (new TextArea(""));
            areaRenderer.SetOccupiedArea(new LayoutArea(1, new Rectangle(100, 100, 100, 100)));
            float? lastY = areaRenderer.CallGetLastYLineRecursively();
            NUnit.Framework.Assert.AreEqual(100, lastY, EPS);
        }

        private class CustomTextAreaRenderer : TextAreaRenderer {
            public CustomTextAreaRenderer(TextArea modelElement)
                : base(modelElement) {
            }

            public virtual void SetOccupiedArea(LayoutArea occupiedArea) {
                this.occupiedArea = occupiedArea;
            }

            public virtual bool CallSetMinMaxWidthBasedOnFixedWidth(MinMaxWidth minMaxWidth) {
                return this.SetMinMaxWidthBasedOnFixedWidth(minMaxWidth);
            }

            public virtual float? CallGetLastYLineRecursively() {
                return this.GetLastYLineRecursively();
            }
        }
    }
}
