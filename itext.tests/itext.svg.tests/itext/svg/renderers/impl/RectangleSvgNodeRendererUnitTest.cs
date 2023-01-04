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
