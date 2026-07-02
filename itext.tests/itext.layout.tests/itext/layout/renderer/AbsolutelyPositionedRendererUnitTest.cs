/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class AbsolutelyPositionedRendererUnitTest : ExtendedITextTest {
        private static readonly LayoutArea DUMMY_AREA = new LayoutArea(1, new Rectangle(0, 0));

        [NUnit.Framework.Test]
        public virtual void LayoutForElementWhichReturnsNothingTest() {
            AbsolutelyPositionedRendererUnitTest.CustomRenderer customRenderer = new AbsolutelyPositionedRendererUnitTest.CustomRenderer
                (new Div());
            AbsolutelyPositionedRenderer absolutelyPositionedRenderer = new AbsolutelyPositionedRenderer(customRenderer
                , false, false);
            absolutelyPositionedRenderer.Layout(new LayoutContext(DUMMY_AREA));
            NUnit.Framework.Assert.AreEqual(2, customRenderer.counter);
        }

        [NUnit.Framework.Test]
        public virtual void GetNextRendererTest() {
            AbsolutelyPositionedRenderer absolutelyPositionedRenderer = new AbsolutelyPositionedRenderer(new DivRenderer
                (new Div()), false, false);
            IRenderer nextRenderer = absolutelyPositionedRenderer.GetNextRenderer();
            NUnit.Framework.Assert.IsTrue(nextRenderer is AbsolutelyPositionedRenderer);
            NUnit.Framework.Assert.IsTrue(((AbsolutelyPositionedRenderer)nextRenderer).GetWrappedRenderer() is DivRenderer
                );
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyTest() {
            DivRenderer wrappedRenderer = new DivRenderer(new Div());
            wrappedRenderer.SetProperty(Property.POSITION, LayoutPosition.ABSOLUTE);
            wrappedRenderer.SetProperty(Property.LEFT, 50);
            AbsolutelyPositionedRenderer absolutelyPositionedRenderer = new AbsolutelyPositionedRenderer(wrappedRenderer
                , false, false);
            NUnit.Framework.Assert.AreEqual(LayoutPosition.STATIC, absolutelyPositionedRenderer.GetProperty<int?>(Property
                .POSITION));
            NUnit.Framework.Assert.AreEqual(50, absolutelyPositionedRenderer.GetProperty<int?>(Property.LEFT));
        }

        [NUnit.Framework.Test]
        public virtual void GetPropertyDefaultValueTest() {
            DivRenderer wrappedRenderer = new DivRenderer(new Div());
            AbsolutelyPositionedRenderer absolutelyPositionedRenderer = new AbsolutelyPositionedRenderer(wrappedRenderer
                , false, false);
            NUnit.Framework.Assert.AreEqual(LayoutPosition.STATIC, absolutelyPositionedRenderer.GetProperty<int?>(Property
                .POSITION, LayoutPosition.FIXED));
            NUnit.Framework.Assert.AreEqual(50, absolutelyPositionedRenderer.GetProperty<int?>(Property.LEFT, 50));
        }

//\cond DO_NOT_DOCUMENT
        internal class CustomRenderer : DivRenderer {
            public int counter = 0;

            public CustomRenderer(Div modelElement)
                : base(modelElement) {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                counter++;
                return new LayoutResult(LayoutResult.NOTHING, DUMMY_AREA, null, null);
            }
        }
//\endcond
    }
}
