/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
