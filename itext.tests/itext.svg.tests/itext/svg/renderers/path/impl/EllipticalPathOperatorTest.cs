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
using System;
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Svg.Renderers.Path.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class EllipticalPathOperatorTest : ExtendedITextTest {
        // tests for coordinates
        [NUnit.Framework.Test]
        public virtual void TestBasicParameterSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 7
            absoluteElliptic.SetCoordinates(new String[] { "40", "40", "0", "0", "0", "20", "20" }, new Point());
            String[] result = absoluteElliptic.GetCoordinates();
            NUnit.Framework.Assert.AreEqual(7, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestTooManyParameterSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 8
            absoluteElliptic.SetCoordinates(new String[] { "40", "40", "0", "0", "0", "20", "20", "1" }, new Point());
            String[] result = absoluteElliptic.GetCoordinates();
            NUnit.Framework.Assert.AreEqual(7, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectMultipleParameterSets() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 13
            absoluteElliptic.SetCoordinates(new String[] { "40", "40", "0", "0", "0", "20", "20", "40", "40", "0", "0"
                , "0", "20" }, new Point());
            String[] result = absoluteElliptic.GetCoordinates();
            NUnit.Framework.Assert.AreEqual(7, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleParameterSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 14
            absoluteElliptic.SetCoordinates(new String[] { "40", "40", "0", "0", "0", "20", "20", "40", "40", "0", "0"
                , "0", "20", "20" }, new Point());
            String[] result = absoluteElliptic.GetCoordinates();
            NUnit.Framework.Assert.AreEqual(7, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestRandomParameterAmountSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 17
            absoluteElliptic.SetCoordinates(new String[] { "40", "40", "0", "0", "0", "20", "20", "40", "40", "0", "0"
                , "0", "20", "20", "0", "1", "2" }, new Point());
            String[] result = absoluteElliptic.GetCoordinates();
            NUnit.Framework.Assert.AreEqual(7, result.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TestNotEnoughParameterSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 6
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => absoluteElliptic.SetCoordinates(new String[]
                 { "40", "0", "0", "0", "20", "20" }, new Point()));
        }

        [NUnit.Framework.Test]
        public virtual void TestNoParameterSet() {
            EllipticalCurveTo absoluteElliptic = new EllipticalCurveTo();
            // String array length = 0
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => absoluteElliptic.SetCoordinates(new String[]
                 {  }, new Point()));
        }

        // rotate tests
        private void AssertPointArrayArrayEquals(Point[][] expected, Point[][] actual) {
            NUnit.Framework.Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++) {
                AssertPointArrayEquals(expected[i], actual[i]);
            }
        }

        private void AssertPointArrayEquals(Point[] expected, Point[] actual) {
            NUnit.Framework.Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i].x, actual[i].x, 0.00001);
                NUnit.Framework.Assert.AreEqual(expected[i].y, actual[i].y, 0.00001);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ZeroRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, 0.0, new Point(0, 0));
            AssertPointArrayArrayEquals(actual, input);
        }

        [NUnit.Framework.Test]
        public virtual void FullCircleRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, 2 * Math.PI, new Point(0, 0));
            AssertPointArrayArrayEquals(actual, input);
        }

        [NUnit.Framework.Test]
        public virtual void HalfCircleRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, Math.PI, new Point(0, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(-50, -30) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ThirtyDegreesRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 6, new Point(0, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(15, Math.Cos(Math.PI / 6) * 30) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void FortyFiveDegreesRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 4, new Point(0, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(Math.Sin(Math.PI / 4) * 30, Math.Sin(Math.PI 
                / 4) * 30) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void SixtyDegreesRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 3, new Point(0, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(Math.Sin(Math.PI / 3) * 30, 15) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesRotationOriginTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 2, new Point(0, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(30, 0) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ZeroRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, 0.0, new Point(40, 90));
            AssertPointArrayArrayEquals(actual, input);
        }

        [NUnit.Framework.Test]
        public virtual void FullCircleRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, 2 * Math.PI, new Point(-200, 50));
            AssertPointArrayArrayEquals(actual, input);
        }

        [NUnit.Framework.Test]
        public virtual void HalfCircleRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(50, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, Math.PI, new Point(-20, -20));
            Point[][] expected = new Point[][] { new Point[] { new Point(-90, -70) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ThirtyDegreesRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 6, new Point(100, 100));
            Point[][] expected = new Point[][] { new Point[] { new Point(-21.60253882, 89.37822282) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void FortyFiveDegreesRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 4, new Point(20, 0));
            Point[][] expected = new Point[][] { new Point[] { new Point(27.07106769, 35.35533845) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void SixtyDegreesRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 3, new Point(0, -50));
            Point[][] expected = new Point[][] { new Point[] { new Point(69.28203105, -10) } };
            AssertPointArrayArrayEquals(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesRotationRandomPointTest() {
            Point[][] input = new Point[][] { new Point[] { new Point(0, 30) } };
            Point[][] actual = EllipticalCurveTo.Rotate(input, -Math.PI / 2, new Point(-0, 20));
            Point[][] expected = new Point[][] { new Point[] { new Point(10, 20) } };
            AssertPointArrayArrayEquals(expected, actual);
        }
    }
}
