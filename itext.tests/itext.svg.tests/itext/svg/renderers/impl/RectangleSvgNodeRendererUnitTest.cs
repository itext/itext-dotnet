namespace iText.Svg.Renderers.Impl {
    public class RectangleSvgNodeRendererUnitTest {
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
