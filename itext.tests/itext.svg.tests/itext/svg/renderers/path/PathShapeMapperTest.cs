using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Path {
    public class PathShapeMapperTest {
        private static IPathShapeMapper mapper;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpClass() {
            mapper = new PathShapeMapper();
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsEllipseRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("a"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsEllipseAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("A"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsCubicRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("c"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsCubicAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("C"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsHorizontalLineRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("h"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsHorizontalLineAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("H"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsLineRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("l"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsLineAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("L"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsMoveRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("m"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsMoveAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("M"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsQuadAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("Q"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsSmoothCubicAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("S"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsVerticalLineRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("v"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsVerticalLineAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("V"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsClosePathRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("z"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsClosePathAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("Z"));
        }

        /* TODO: implement currently unsupported operator
        * DEVSIX-2267: relative alternatives for existing absolute operators
        * DEVSIX-2611: smooth quadratic curves (absolute and relative)
        */
        [NUnit.Framework.Test]
        public virtual void TestNotExistsQuadRel() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("q"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsSmoothCubicRel() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("s"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsSmoothQuadRel() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("t"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsSmoothQuadAbs() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("T"));
        }

        // nonsensical operators
        [NUnit.Framework.Test]
        public virtual void TestNotExistsNonExistingOperator1() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("e"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsNonExistingOperator2() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("Y"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsNonExistingOperator3() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("3"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsNonExistingOperator4() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("am"));
        }

        [NUnit.Framework.Test]
        public virtual void TestNotExistsNonExistingOperator5() {
            NUnit.Framework.Assert.IsNull(mapper.GetMapping().Get("Pos"));
        }
    }
}
