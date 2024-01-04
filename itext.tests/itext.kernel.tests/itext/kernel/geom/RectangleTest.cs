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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("UnitTest")]
    public class RectangleTest : ExtendedITextTest {
        private const float OVERLAP_EPSILON = 0.1f;

        [NUnit.Framework.Test]
        public virtual void OverlapWithEpsilon() {
            Rectangle first = new Rectangle(0, 0, 10, 10);
            Rectangle second = new Rectangle(-10, 0, 10.09f, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, OVERLAP_EPSILON));
            second.SetWidth(10.11f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, OVERLAP_EPSILON));
            second = new Rectangle(5, 9.91f, 5, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, OVERLAP_EPSILON));
            second.SetY(9.89f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, OVERLAP_EPSILON));
            second = new Rectangle(9.91f, 0, 5, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, OVERLAP_EPSILON));
            second.SetX(9.89f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, OVERLAP_EPSILON));
            second = new Rectangle(5, -10, 5, 10.09f);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, OVERLAP_EPSILON));
            second.SetHeight(10.11f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, OVERLAP_EPSILON));
        }

        [NUnit.Framework.Test]
        public virtual void OverlapWithNegativeEpsilon() {
            Rectangle first = new Rectangle(0, 0, 10, 10);
            Rectangle second = new Rectangle(-10, 0, 9.89f, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, -OVERLAP_EPSILON));
            second.SetWidth(9.91f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, -OVERLAP_EPSILON));
            second = new Rectangle(5, 10.11f, 5, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, -OVERLAP_EPSILON));
            second.SetY(10.09f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, -OVERLAP_EPSILON));
            second = new Rectangle(10.11f, 0, 5, 5);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, -OVERLAP_EPSILON));
            second.SetX(10.09f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, -OVERLAP_EPSILON));
            second = new Rectangle(5, -10, 5, 9.89f);
            NUnit.Framework.Assert.IsFalse(first.Overlaps(second, -OVERLAP_EPSILON));
            second.SetHeight(9.91f);
            NUnit.Framework.Assert.IsTrue(first.Overlaps(second, -OVERLAP_EPSILON));
        }

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

        [NUnit.Framework.Test]
        public virtual void TranslateOnRotatedPageTest01() {
            // we need a page with set rotation and page size to test Rectangle#getRectangleOnRotatedPage
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDocument.AddNewPage();
            NUnit.Framework.Assert.IsTrue(PageSize.A4.EqualsWithEpsilon(page.GetPageSize()));
            // Test rectangle
            Rectangle testRectangle = new Rectangle(200, 200, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, page.GetRotation());
            NUnit.Framework.Assert.IsTrue(new Rectangle(200, 200, 100, 200).EqualsWithEpsilon(Rectangle.GetRectangleOnRotatedPage
                (testRectangle, page)));
            page.SetRotation(90);
            NUnit.Framework.Assert.AreEqual(90, page.GetRotation());
            NUnit.Framework.Assert.IsTrue(new Rectangle(195, 200, 200, 100).EqualsWithEpsilon(Rectangle.GetRectangleOnRotatedPage
                (testRectangle, page)));
            page.SetRotation(180);
            NUnit.Framework.Assert.AreEqual(180, page.GetRotation());
            NUnit.Framework.Assert.IsTrue(new Rectangle(295, 442, 100, 200).EqualsWithEpsilon(Rectangle.GetRectangleOnRotatedPage
                (testRectangle, page)));
            page.SetRotation(270);
            NUnit.Framework.Assert.AreEqual(270, page.GetRotation());
            NUnit.Framework.Assert.IsTrue(new Rectangle(200, 542, 200, 100).EqualsWithEpsilon(Rectangle.GetRectangleOnRotatedPage
                (testRectangle, page)));
            page.SetRotation(360);
            NUnit.Framework.Assert.AreEqual(0, page.GetRotation());
            NUnit.Framework.Assert.IsTrue(new Rectangle(200, 200, 100, 200).EqualsWithEpsilon(Rectangle.GetRectangleOnRotatedPage
                (testRectangle, page)));
        }

        [NUnit.Framework.Test]
        public virtual void CalculateBBoxTest() {
            Point a = new Point(100, 100);
            Point b = new Point(200, 100);
            Point c = new Point(200, 200);
            Point d = new Point(100, 200);
            // Zero rotation
            Rectangle.CalculateBBox(JavaUtil.ArraysAsList(a, b, c, d));
            NUnit.Framework.Assert.IsTrue(new Rectangle(100, 100, 100, 100).EqualsWithEpsilon(Rectangle.CalculateBBox(
                JavaUtil.ArraysAsList(a, b, c, d))));
            // 270 degree rotation
            a = new Point(200, 100);
            b = new Point(200, 200);
            c = new Point(100, 200);
            d = new Point(100, 100);
            NUnit.Framework.Assert.IsTrue(new Rectangle(100, 100, 100, 100).EqualsWithEpsilon(Rectangle.CalculateBBox(
                JavaUtil.ArraysAsList(a, b, c, d))));
            // it looks as follows:
            // dxxxxxx
            // xxxxxxx
            // cxxxxxa
            // xxxxxxx
            // xxxxxxb
            a = new Point(200, 100);
            b = new Point(200, 0);
            c = new Point(0, 100);
            d = new Point(0, 200);
            NUnit.Framework.Assert.IsTrue(new Rectangle(0, 0, 200, 200).EqualsWithEpsilon(Rectangle.CalculateBBox(JavaUtil.ArraysAsList
                (a, b, c, d))));
        }

        [NUnit.Framework.Test]
        public virtual void SetBBoxWithoutNormalizationTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            //set bBox without any normalization needed
            rectangle.SetBbox(10, 10, 90, 190);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(80, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(180, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetBBoxNormalizeXTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            //set bBox where llx > urx
            rectangle.SetBbox(90, 10, 10, 190);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(80, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(180, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetBBoxNormalizeYTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            //set bBox where lly > ury
            rectangle.SetBbox(10, 190, 90, 10);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(80, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(180, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetXTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetX(), 1e-5);
            rectangle.SetX(50);
            NUnit.Framework.Assert.AreEqual(50, rectangle.GetX(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetYTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetY(), 1e-5);
            rectangle.SetY(50);
            NUnit.Framework.Assert.AreEqual(50, rectangle.GetY(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetWidthTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            rectangle.SetWidth(50);
            NUnit.Framework.Assert.AreEqual(50, rectangle.GetWidth(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void SetHeightTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            rectangle.SetHeight(50);
            NUnit.Framework.Assert.AreEqual(50, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void IncreaseHeightTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            rectangle.IncreaseHeight(50);
            NUnit.Framework.Assert.AreEqual(250, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void DecreaseHeightTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            rectangle.DecreaseHeight(50);
            NUnit.Framework.Assert.AreEqual(150, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyMarginsShrinkTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100, 200);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(0, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            //shrink the rectangle
            rectangle.ApplyMargins(20, 20, 20, 20, false);
            NUnit.Framework.Assert.AreEqual(20, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(20, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(60, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(160, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyMarginsExpandTest() {
            Rectangle rectangle = new Rectangle(20, 20, 100, 200);
            NUnit.Framework.Assert.AreEqual(20, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(20, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(100, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(200, rectangle.GetHeight(), 1e-5);
            //expand the rectangle
            rectangle.ApplyMargins(10, 10, 10, 10, true);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetX(), 1e-5);
            NUnit.Framework.Assert.AreEqual(10, rectangle.GetY(), 1e-5);
            NUnit.Framework.Assert.AreEqual(120, rectangle.GetWidth(), 1e-5);
            NUnit.Framework.Assert.AreEqual(220, rectangle.GetHeight(), 1e-5);
        }

        [NUnit.Framework.Test]
        public virtual void ToStringTest() {
            Rectangle rectangle = new Rectangle(0, 0, 100f, 200f);
            String rectangleString = rectangle.ToString();
            //Using contains() to check for value instead of equals() on the whole string due to the
            //differences between decimal numbers formatting in java and .NET.
            NUnit.Framework.Assert.IsTrue(rectangleString.Contains("100"));
            NUnit.Framework.Assert.IsTrue(rectangleString.Contains("200"));
        }

        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            PageSize originalPageSize = new PageSize(15, 20);
            PageSize copyAsPageSize = (PageSize)originalPageSize.Clone();
            Rectangle copyAsRectangle = ((Rectangle)originalPageSize).Clone();
            NUnit.Framework.Assert.AreEqual(typeof(PageSize), copyAsPageSize.GetType());
            NUnit.Framework.Assert.AreEqual(typeof(PageSize), copyAsRectangle.GetType());
        }

        [NUnit.Framework.Test]
        public virtual void DecreaseWidthTest() {
            Rectangle rectangle = new Rectangle(100, 200);
            rectangle.DecreaseWidth(10);
            NUnit.Framework.Assert.AreEqual(90, rectangle.GetWidth(), Rectangle.EPS);
        }

        [NUnit.Framework.Test]
        public virtual void IncreaseWidthTest() {
            Rectangle rectangle = new Rectangle(100, 200);
            rectangle.IncreaseWidth(10);
            NUnit.Framework.Assert.AreEqual(110, rectangle.GetWidth(), Rectangle.EPS);
        }
    }
}
