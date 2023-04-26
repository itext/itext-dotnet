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
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class RectangleSvgNodeRendererUnitTest : ExtendedITextTest {
        private const float EPSILON = 0.00001f;

        internal RectangleSvgNodeRenderer renderer;

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            renderer = new RectangleSvgNodeRenderer();
        }

        [NUnit.Framework.Test]
        public virtual void CheckRadiusTest() {
            float rad = renderer.CheckRadius(0f, 20f);
            NUnit.Framework.Assert.AreEqual(0f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CheckRadiusNegativeTest() {
            float rad = renderer.CheckRadius(-1f, 20f);
            NUnit.Framework.Assert.AreEqual(0f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CheckRadiusTooLargeTest() {
            float rad = renderer.CheckRadius(30f, 20f);
            NUnit.Framework.Assert.AreEqual(10f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CheckRadiusTooLargeNegativeTest() {
            float rad = renderer.CheckRadius(-100f, 20f);
            NUnit.Framework.Assert.AreEqual(0f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void CheckRadiusHalfLengthTest() {
            float rad = renderer.CheckRadius(10f, 20f);
            NUnit.Framework.Assert.AreEqual(10f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void FindCircularRadiusTest() {
            float rad = renderer.FindCircularRadius(0f, 20f, 100f, 200f);
            NUnit.Framework.Assert.AreEqual(20f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void FindCircularRadiusHalfLengthTest() {
            float rad = renderer.FindCircularRadius(0f, 200f, 100f, 200f);
            NUnit.Framework.Assert.AreEqual(50f, rad, EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void FindCircularRadiusSmallWidthTest() {
            float rad = renderer.FindCircularRadius(0f, 20f, 5f, 200f);
            NUnit.Framework.Assert.AreEqual(2.5f, rad, EPSILON);
        }
    }
}
