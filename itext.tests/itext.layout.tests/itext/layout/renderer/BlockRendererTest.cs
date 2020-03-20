/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class BlockRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ApplyMinHeightForSpecificDimensionsCausingFloatPrecisionError() {
            float divHeight = 42.55f;
            Div div = new Div();
            div.SetHeight(UnitValue.CreatePointValue(divHeight));
            float occupiedHeight = 17.981995f;
            float leftHeight = 24.567993f;
            NUnit.Framework.Assert.IsTrue(occupiedHeight + leftHeight < divHeight);
            BlockRenderer blockRenderer = (BlockRenderer)div.CreateRendererSubTree();
            blockRenderer.occupiedArea = new LayoutArea(1, new Rectangle(0, 267.9681f, 0, occupiedHeight));
            AbstractRenderer renderer = blockRenderer.ApplyMinHeight(OverflowPropertyValue.FIT, new Rectangle(0, 243.40012f
                , 0, leftHeight));
            NUnit.Framework.Assert.IsNull(renderer);
        }
    }
}
