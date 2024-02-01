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
using iText.Svg.Renderers.Path.Impl;
using iText.Test;

namespace iText.Svg.Renderers.Path {
    [NUnit.Framework.Category("UnitTest")]
    public class PathShapeMapperTest : ExtendedITextTest {
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
        public virtual void TestExistsQuadRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("q"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsSmoothCubicAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("S"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsSmoothCubicRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("s"));
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

        [NUnit.Framework.Test]
        public virtual void TestExistsSmoothQuadAbs() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("T"));
        }

        [NUnit.Framework.Test]
        public virtual void TestExistsSmoothQuadRel() {
            NUnit.Framework.Assert.IsNotNull(mapper.GetMapping().Get("t"));
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
