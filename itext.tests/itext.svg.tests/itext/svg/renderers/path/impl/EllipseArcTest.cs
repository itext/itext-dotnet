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
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Svg.Renderers.Path.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class EllipseArcTest : ExtendedITextTest {
        private const double DELTA = 0.00001;

        private void AssertPointEqual(Point expected, Point actual) {
            NUnit.Framework.Assert.AreEqual(expected.x, actual.x, DELTA);
            NUnit.Framework.Assert.AreEqual(expected.y, actual.y, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestCircleSweepLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 10, 10, true, true);
            AssertPointEqual(new Point(0, -10), arc.ll);
            AssertPointEqual(new Point(20, 10), arc.ur);
            NUnit.Framework.Assert.AreEqual(180, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(180, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestCircleSweepNotLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 10, 10, true, false);
            AssertPointEqual(new Point(0, -10), arc.ll);
            AssertPointEqual(new Point(20, 10), arc.ur);
            NUnit.Framework.Assert.AreEqual(180, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(180, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestCircleNotSweepLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 10, 10, false, true);
            AssertPointEqual(new Point(0, -10), arc.ll);
            AssertPointEqual(new Point(20, 10), arc.ur);
            NUnit.Framework.Assert.AreEqual(180, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(0, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestCircleNotSweepNotLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 10, 10, false, false);
            AssertPointEqual(new Point(0, -10), arc.ll);
            AssertPointEqual(new Point(20, 10), arc.ur);
            NUnit.Framework.Assert.AreEqual(180, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(0, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestEllipseSweepLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 30, 10, true, true);
            AssertPointEqual(new Point(-20, -19.428090), arc.ll);
            AssertPointEqual(new Point(40, 0.571909), arc.ur);
            NUnit.Framework.Assert.AreEqual(321.057558, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(109.471220, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestEllipseSweepNotLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 30, 10, true, false);
            AssertPointEqual(new Point(-20, -0.571909), arc.ll);
            AssertPointEqual(new Point(40, 19.428090), arc.ur);
            NUnit.Framework.Assert.AreEqual(38.942441, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(250.528779, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestEllipseNotSweepLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 30, 10, false, true);
            AssertPointEqual(new Point(-20, -0.571909), arc.ll);
            AssertPointEqual(new Point(40, 19.428090), arc.ur);
            NUnit.Framework.Assert.AreEqual(321.057558, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(289.4712206344907, arc.startAng, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestEllipseNotSweepNotLarge() {
            EllipticalCurveTo.EllipseArc arc = EllipticalCurveTo.EllipseArc.GetEllipse(new Point(0, 0), new Point(20, 
                0), 30, 10, false, false);
            AssertPointEqual(new Point(-20, -19.428090), arc.ll);
            AssertPointEqual(new Point(40, 0.5719095), arc.ur);
            NUnit.Framework.Assert.AreEqual(38.942441, arc.extent, DELTA);
            NUnit.Framework.Assert.AreEqual(70.528779, arc.startAng, DELTA);
        }
    }
}
