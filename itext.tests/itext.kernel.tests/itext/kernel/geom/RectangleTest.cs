using System.Collections.Generic;
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
            bool areEqual = true;
            main = new Rectangle(2, 2, 8, 8);
            //A. Main rectangle is greater in both dimension than second rectangle
            second = new Rectangle(4, 8, 4, 4);
            //1.Middle top
            expected = new Rectangle(4, 8, 4, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //2.Middle Right
            second.MoveRight(4);
            expected = new Rectangle(8, 8, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //3.Right middle
            second.MoveDown(4);
            expected = new Rectangle(8, 4, 2, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //4.Bottom right
            second.MoveDown(4);
            expected = new Rectangle(8, 2, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //5.Bottom middle
            second.MoveLeft(4);
            expected = new Rectangle(4, 2, 4, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //6.Bottom Left
            second.MoveLeft(4);
            expected = new Rectangle(2, 2, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //7.Left Middle
            second.MoveUp(4);
            expected = new Rectangle(2, 4, 2, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //8.Left Top
            second.MoveUp(4);
            expected = new Rectangle(2, 8, 2, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //B. Main rectangle is greater in width but not height than second rectangle
            //1. Left
            second = new Rectangle(0, 0, 4, 12);
            expected = new Rectangle(2, 2, 2, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //2. Middle
            second.MoveRight(4);
            expected = new Rectangle(4, 2, 4, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //3. Right
            second.MoveRight(4);
            expected = new Rectangle(8, 2, 2, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //C. Main rectangle is greater in height but not width than second rectangle
            //1. Top
            second = new Rectangle(0, 8, 12, 4);
            expected = new Rectangle(2, 8, 8, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //2. Middle
            second.MoveDown(4);
            expected = new Rectangle(2, 4, 8, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //3. Bottom
            second.MoveDown(4);
            expected = new Rectangle(2, 2, 8, 2);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Check if any have failed
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest02() {
            //Cases where the two rectangles do not intersect
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool noIntersection = true;
            main = new Rectangle(2, 2, 8, 8);
            //Top
            second = new Rectangle(4, 12, 4, 4);
            actual = main.GetIntersection(second);
            noIntersection = noIntersection && ((actual) == null);
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
            bool areEqual = true;
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(main);
            expected = new Rectangle(main);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //B main contains second
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(4, 4, 4, 4);
            expected = new Rectangle(second);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //C second contains main
            main = new Rectangle(2, 2, 8, 8);
            second = new Rectangle(0, 0, 12, 12);
            expected = new Rectangle(main);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest04() {
            //Edge case: intersections on edges
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool areEqual = true;
            main = new Rectangle(2, 2, 8, 8);
            //Top
            second = new Rectangle(4, 10, 4, 4);
            expected = new Rectangle(4, 10, 4, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Right
            second = new Rectangle(10, 4, 4, 4);
            expected = new Rectangle(10, 4, 0, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Bottom
            second = new Rectangle(4, -2, 4, 4);
            expected = new Rectangle(4, 2, 4, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Left
            second = new Rectangle(-2, 4, 4, 4);
            expected = new Rectangle(2, 4, 0, 4);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Edge case: intersection on corners
            //Top-Left
            second = new Rectangle(-2, 10, 4, 4);
            expected = new Rectangle(2, 10, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Top-Right
            second = new Rectangle(10, 10, 4, 4);
            expected = new Rectangle(10, 10, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Bottom-Right
            second = new Rectangle(10, -2, 4, 4);
            expected = new Rectangle(10, 2, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            //Bottom-Left
            second = new Rectangle(-2, -2, 4, 4);
            expected = new Rectangle(2, 2, 0, 0);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
            NUnit.Framework.Assert.IsTrue(areEqual);
        }

        [NUnit.Framework.Test]
        public virtual void GetIntersectionTest05() {
            //Infinity scenarios
            Rectangle main;
            Rectangle second;
            Rectangle actual;
            Rectangle expected;
            bool areEqual = true;
            main = new Rectangle(0, 2, 2 * float.PositiveInfinity, 8);
            second = new Rectangle(float.NegativeInfinity, 2, float.PositiveInfinity, 8);
            expected = new Rectangle(0, 2, float.PositiveInfinity, 8);
            actual = main.GetIntersection(second);
            areEqual = areEqual && (expected.Equals(actual));
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
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectangleFromQuadPointsTest02() {
            Rectangle actual;
            Rectangle expected;
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0 };
            PdfArray quadpoints = new PdfArray(points);
            bool exception = false;
            try {
                Rectangle.CreateBoundingRectanglesFromQuadPoint(quadpoints);
            } catch (PdfException e) {
                exception = true;
            }

            NUnit.Framework.Assert.True(exception);
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectanglesFromQuadPointsTest01() {
            IList<Rectangle> actual;
            IList<Rectangle> expected;
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0, -1, 2, 0, 1, 1, -2, 0 };
            PdfArray quadpoints = new PdfArray(points);
            expected = new List<Rectangle>();
            expected.Add(new Rectangle(-2, 0, 4, 2));
            expected.Add(new Rectangle(-2, -1, 4, 2));
            actual = Rectangle.CreateBoundingRectanglesFromQuadPoint(quadpoints);
            NUnit.Framework.Assert.AreEqual(expected.ToArray(), actual.ToArray());
        }

        [NUnit.Framework.Test]
        public virtual void CreateBoundingRectanglesFromQuadPointsTest02() {
            IList<Rectangle> actual;
            IList<Rectangle> expected;
            float[] points = new float[] { 0, 0, 2, 1, 1, 2, -2, 1, 0, -1, 2, 0, 1, 1, -2, 0, 1 };
            PdfArray quadpoints = new PdfArray(points);
            expected = new List<Rectangle>();
            expected.Add(new Rectangle(-2, 0, 4, 2));
            expected.Add(new Rectangle(-2, -1, 4, 2));
            actual = Rectangle.CreateBoundingRectanglesFromQuadPoint(quadpoints);
            NUnit.Framework.Assert.AreEqual(expected.ToArray(), actual.ToArray());
        }
    }
}
