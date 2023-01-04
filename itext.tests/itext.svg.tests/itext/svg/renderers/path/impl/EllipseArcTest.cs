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
