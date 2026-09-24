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
using System;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class GroupSvgNodeRendererUnitTest : ExtendedITextTest {
        private const float EPSILON = 0.00001f;

        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            GroupSvgNodeRenderer renderer = new GroupSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }

        [NUnit.Framework.Test]
        public virtual void TranslatedChildObjectBoundingBoxTest() {
            GroupSvgNodeRenderer group = new GroupSvgNodeRenderer();
            RectangleSvgNodeRenderer child = CreateRectangle("10pt", "20pt", "30pt", "40pt");
            child.SetAttribute(SvgConstants.Attributes.TRANSFORM, "translate(5pt 7pt)");
            group.AddChild(child);
            Rectangle objectBoundingBox = group.GetObjectBoundingBox(CreateContext());
            NUnit.Framework.Assert.IsNotNull(objectBoundingBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(15, 27, 30, 40).EqualsWithEpsilon(objectBoundingBox, EPSILON));
        }

        [NUnit.Framework.Test]
        public virtual void RotatedChildObjectBoundingBoxTest() {
            GroupSvgNodeRenderer group = new GroupSvgNodeRenderer();
            RectangleSvgNodeRenderer child = CreateRectangle("10pt", "20pt", "30pt", "40pt");
            child.SetAttribute(SvgConstants.Attributes.TRANSFORM, "rotate(90)");
            group.AddChild(child);
            Rectangle objectBoundingBox = group.GetObjectBoundingBox(CreateContext());
            NUnit.Framework.Assert.IsNotNull(objectBoundingBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-60, 10, 40, 30).EqualsWithEpsilon(objectBoundingBox, EPSILON)
                );
        }

        private static RectangleSvgNodeRenderer CreateRectangle(String x, String y, String width, String height) {
            RectangleSvgNodeRenderer rectangle = new RectangleSvgNodeRenderer();
            rectangle.SetAttribute(SvgConstants.Attributes.X, x);
            rectangle.SetAttribute(SvgConstants.Attributes.Y, y);
            rectangle.SetAttribute(SvgConstants.Attributes.WIDTH, width);
            rectangle.SetAttribute(SvgConstants.Attributes.HEIGHT, height);
            return rectangle;
        }

        private static SvgDrawContext CreateContext() {
            SvgDrawContext context = new SvgDrawContext(null, null);
            context.AddViewPort(new Rectangle(0, 0, 200, 200));
            return context;
        }
    }
}
