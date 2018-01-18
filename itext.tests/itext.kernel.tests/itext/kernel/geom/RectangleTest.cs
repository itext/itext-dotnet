/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System.Collections.Generic;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Geom {
    public class RectangleTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RectangleOverlapTest01() {
            //Intersection
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(5, 5, 5, 5);
            bool result = one.Overlaps(two);
            NUnit.Framework.Assert.IsTrue(result);
            //envelopment
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(2, 2, 5, 5);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsTrue(result);
            //identical
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(0, 0, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void RectangleOverlapTest02() {
            //Left
            //Top left
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(15, 15, 10, 10);
            bool result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Middle left
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(15, 5, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Lower left
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(15, -5, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Bottom
            //Bottom left
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(5, -15, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Bottom right
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(-5, -15, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Right
            //Lower right
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(-15, -5, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Upper right
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(-15, 5, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Top
            //Top right
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(-5, 15, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
            //Top left
            one = new Rectangle(0, 0, 10, 10);
            two = new Rectangle(5, 15, 10, 10);
            result = one.Overlaps(two);
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void EnvelopTest01() {
            //one contains two
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(5, 5, 5, 5);
            bool result = one.Contains(two);
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void EnvelopsTest02() {
            //two identical rectangles
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(0, 0, 10, 10);
            bool result = one.Contains(two);
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void EnvelopsTest03() {
            //One intersects two but does not envelop
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(5, 5, 10, 10);
            bool result = one.Contains(two);
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void EnvelopsTest04() {
            //one and two do not
            Rectangle one = new Rectangle(0, 0, 10, 10);
            Rectangle two = new Rectangle(-15, -15, 10, 10);
            bool result = one.Contains(two);
            NUnit.Framework.Assert.IsFalse(result);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest01() {
            //Cases where there is an intersection rectangle
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool areEqual;
            main = new Rectangle(2, 2, 8, 8);
            //A. Main rectangle is greater in both dimension than second rectangle
            second = new Rectangle(4, 8, 4, 4);
            //1.Middle top
            expected = new Rectangle(4, 8, 4, 2);
            actual = main.GetIntersection(second);
            areEqual = expected.EqualsWithEpsilon(actual);
            //2.Middle Right
            second.MoveRight(4);
            expected = new Rectangle(8, 8, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //3.Right middle
            second.MoveDown(4);
            expected = new Rectangle(8, 4, 2, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //4.Bottom right
            second.MoveDown(4);
            expected = new Rectangle(8, 2, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //5.Bottom middle
            second.MoveLeft(4);
            expected = new Rectangle(4, 2, 4, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //6.Bottom Left
            second.MoveLeft(4);
            expected = new Rectangle(2, 2, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //7.Left Middle
            second.MoveUp(4);
            expected = new Rectangle(2, 4, 2, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //8.Left Top
            second.MoveUp(4);
            expected = new Rectangle(2, 8, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //B. Main rectangle is greater in width but not height than second rectangle
            //1. Left
            second = new Rectangle(0, 0, 4, 12);
            expected = new Rectangle(2, 2, 2, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //2. Middle
            second.MoveRight(4);
            expected = new Rectangle(4, 2, 4, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //3. Right
            second.MoveRight(4);
            expected = new Rectangle(8, 2, 2, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //C. Main rectangle is greater in height but not width than second rectangle
            //1. Top
            second = new Rectangle(0, 8, 12, 4);
            expected = new Rectangle(2, 8, 8, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //2. Middle
            second.MoveDown(4);
            expected = new Rectangle(2, 4, 8, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //3. Bottom
            second.MoveDown(4);
            expected = new Rectangle(2, 2, 8, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Check if any have failed
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest02() {
            //Cases where the two rectangles do not intersect
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            bool noIntersection;
            main = new Rectangle(2, 2, 8, 8);
            //Top
            second = new Rectangle(4, 12, 4, 4);
            actual = main.GetIntersection(second);
            noIntersection = actual == null;
            //Right
            second = new Rectangle(12, 4, 4, 4);
            actual = main.GetIntersection(second);
            noIntersection = noIntersection && ((actual) == null);
            //Bottom
            second = new Rectangle(4, -8, 4, 4);
            actual = main.GetIntersection(second);
            noIntersection = noIntersection && ((actual) == null);
            //Left
            second = new Rectangle(-8, 4, 4, 4);
            actual = main.GetIntersection(second);
            noIntersection = noIntersection && ((actual) == null);
            NUnit.Framework.Assert.IsTrue(noIntersection);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest03() {
            //Edge cases: envelopment
            //A equal rectangles
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool areEqual;
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(main);
            expected = new Rectangle(main);
            actual = main.GetIntersection(second);
            areEqual = expected.EqualsWithEpsilon(actual);
            //B main contains second
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(4, 4, 4, 4);
            expected = new Rectangle(second);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //C second contains main
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(0, 0, 12, 12);
            expected = new Rectangle(main);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest04() {
            //Edge case: intersections on edges
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool areEqual;
            main = new Rectangle(2, 2, 8, 8);
            //Top
            second = new Rectangle(4, 10, 4, 4);
            expected = new Rectangle(4, 10, 4, 0);
            actual = main.GetIntersection(second);
            areEqual = expected.EqualsWithEpsilon(actual);
            //Right
            second = new Rectangle(10, 4, 4, 4);
            expected = new Rectangle(10, 4, 0, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Bottom
            second = new Rectangle(4, -2, 4, 4);
            expected = new Rectangle(4, 2, 4, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Left
            second = new Rectangle(-2, 4, 4, 4);
            expected = new Rectangle(2, 4, 0, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Edge case: intersection on corners
            //Top-Left
            second = new Rectangle(-2, 10, 4, 4);
            expected = new Rectangle(2, 10, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Top-Right
            second = new Rectangle(10, 10, 4, 4);
            expected = new Rectangle(10, 10, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Bottom-Right
            second = new Rectangle(10, -2, 4, 4);
            expected = new Rectangle(10, 2, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            //Bottom-Left
            second = new Rectangle(-2, -2, 4, 4);
            expected = new Rectangle(2, 2, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.EqualsWithEpsilon(actual));
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectangleFromQuadPointsTest01() {
            Rectangle actual;
            Rectangle expected;
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1 };
            PdfArray quadpoints = new PdfArray(points);
            expected = new Rectangle(-2, 0, 4, 2);
            actual = Rectangle.CreateBoundingRectangleFromQuadPoint(quadpoints);
            bool? areEqual = expected.EqualsWithEpsilon(actual);
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectangleFromQuadPointsTest02() {
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0 };
            PdfArray quadpoints = new PdfArray(points);
            bool exception = false;
            try {
                Rectangle.CreateBoundingRectangleFromQuadPoint(quadpoints);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectanglesFromQuadPointsTest01() {
            IList<Rectangle> actual;
            IList<Rectangle> expected;
            bool areEqual = true;
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0, -1, 2, 0, 1, 1, -2, 0 };
            PdfArray quadpoints = new PdfArray(points);
            expected = new List<Rectangle>();
            expected.Add(new Rectangle(-2, 0, 4, 2));
            expected.Add(new Rectangle(-2, -1, 4, 2));
            actual = Rectangle.CreateBoundingRectanglesFromQuadPoint(quadpoints);
            for (int i = 0; i < expected.Count; i++) {
                areEqual = areEqual && expected[i].EqualsWithEpsilon(actual[i]);
            }
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectanglesFromQuadPointsTest02() {
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0, -1, 2, 0, 1, 1, -2, 0, 1 };
            PdfArray quadpoints = new PdfArray(points);
            bool exception = false;
            try {
                Rectangle.CreateBoundingRectanglesFromQuadPoint(quadpoints);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
        }
    }
}
