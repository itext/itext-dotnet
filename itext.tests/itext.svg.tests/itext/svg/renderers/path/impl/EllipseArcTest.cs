using iText.Kernel.Geom;

namespace iText.Svg.Renderers.Path.Impl {
    public class EllipseArcTest {
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
