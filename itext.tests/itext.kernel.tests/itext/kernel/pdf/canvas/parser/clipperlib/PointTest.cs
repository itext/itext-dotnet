/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    public class PointTest : ExtendedITextTest {
        private const double DOUBLE_EPSILON_COMPARISON = 1E-6;
        
        [NUnit.Framework.Test]
        public virtual void DoublePointDefaultConstructorTest() {
            DoublePoint dp = new DoublePoint();
            PointTest.AssertDoublePointFields(dp, 0, 0);
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointTwoParamConstructorTest() {
            DoublePoint dp = new DoublePoint(1.23, 5.34);
            PointTest.AssertDoublePointFields(dp, 1.23, 5.34);
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointCopyConstructorTest() {
            DoublePoint dp = new DoublePoint(1.23, 5.34);
            DoublePoint copy = new DoublePoint(dp);
            PointTest.AssertDoublePointFields(copy, 1.23, 5.34);
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointEqualsAndHashCodeItselfTest() {
            DoublePoint dp = new DoublePoint(1.23, 5.34);
            NUnit.Framework.Assert.IsTrue(dp.Equals(dp));
            NUnit.Framework.Assert.AreEqual(dp.GetHashCode(), dp.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointEqualsAndHashCodeToAnotherEqualPointTest() {
            DoublePoint first = new DoublePoint(1.23, 5.34);
            DoublePoint second = new DoublePoint(1.23, 5.34);
            NUnit.Framework.Assert.IsTrue(first.Equals(second));
            NUnit.Framework.Assert.IsTrue(second.Equals(first));
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointEqualsAndHashCodeToAnotherNotEqualPointTest() {
            DoublePoint first = new DoublePoint(1.23, 5.34);
            DoublePoint second = new DoublePoint(0, 5.34);
            NUnit.Framework.Assert.IsFalse(first.Equals(second));
            NUnit.Framework.Assert.IsFalse(second.Equals(first));
            second = new DoublePoint(1.23, 0);
            NUnit.Framework.Assert.IsFalse(first.Equals(second));
            NUnit.Framework.Assert.IsFalse(second.Equals(first));
            // Do not check the hash code method, because it works differently depending on the framework
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointEqualsToNullTest() {
            DoublePoint dp = new DoublePoint(1.23, 5.34);
            NUnit.Framework.Assert.IsFalse(dp.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointEqualsToAnotherClassTest() {
            DoublePoint dp = new DoublePoint(1.23, 5.34);
            NUnit.Framework.Assert.IsFalse(dp.Equals(""));
        }

        [NUnit.Framework.Test]
        public virtual void DoublePointSetAndGetTest() {
            DoublePoint temp = new DoublePoint(1.23, 5.34);
            DoublePoint dp = new DoublePoint();
            NUnit.Framework.Assert.AreEqual(1.23, temp.X);
            NUnit.Framework.Assert.AreEqual(5.34, temp.Y);
            dp.X = temp.X;
            dp.Y = temp.Y;
            PointTest.AssertDoublePointFields(dp, 1.23, 5.34);
        }

        [NUnit.Framework.Test]
        public virtual void LongPointDefaultConstructorTest() {
            IntPoint lp = new IntPoint();
            PointTest.AssertLongPointFields(lp, 0, 0);
        }

        [NUnit.Framework.Test]
        public virtual void LongPointTwoLongParamConstructorTest() {
            IntPoint lp = new IntPoint(1, 5);
            PointTest.AssertLongPointFields(lp, 1, 5);
        }

        [NUnit.Framework.Test]
        public virtual void LongPointTwoDoubleParamConstructorTest() {
            IntPoint lp = new IntPoint(1.23, 5.34);
            PointTest.AssertLongPointFields(lp, 1, 5);
        }

        [NUnit.Framework.Test]
        public virtual void LongPointCopyConstructorTest() {
            IntPoint lp = new IntPoint(1, 5);
            IntPoint copy = new IntPoint(lp);
            PointTest.AssertLongPointFields(copy, 1, 5);
        }

        [NUnit.Framework.Test]
        public virtual void LongPointEqualsAndHashCodeItselfTest() {
            IntPoint lp = new IntPoint(1, 5);
            NUnit.Framework.Assert.IsTrue(lp.Equals(lp));
            NUnit.Framework.Assert.AreEqual(lp.GetHashCode(), lp.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void LongPointEqualsAndHashCodeToAnotherEqualPointTest() {
            IntPoint first = new IntPoint(1, 5);
            IntPoint second = new IntPoint(1, 5);
            NUnit.Framework.Assert.IsTrue(first.Equals(second));
            NUnit.Framework.Assert.IsTrue(second.Equals(first));
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void LongPointEqualsAndHashCodeToAnotherNotEqualPointTest() {
            IntPoint first = new IntPoint(1, 5);
            IntPoint second = new IntPoint(0, 5);
            NUnit.Framework.Assert.IsFalse(first.Equals(second));
            NUnit.Framework.Assert.IsFalse(second.Equals(first));
            second = new IntPoint(1, 0);
            NUnit.Framework.Assert.IsFalse(first.Equals(second));
            NUnit.Framework.Assert.IsFalse(second.Equals(first));
            // Do not check the hash code method, because it works differently depending on the framework
        }

        [NUnit.Framework.Test]
        public virtual void LongPointEqualsToNullTest() {
            IntPoint lp = new IntPoint(1, 5);
            NUnit.Framework.Assert.IsFalse(lp.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void LongPointEqualsToAnotherClassTest() {
            IntPoint lp = new IntPoint(1, 5);
            NUnit.Framework.Assert.IsFalse(lp.Equals(""));
        }

        [NUnit.Framework.Test]
        public virtual void LongPointSetAndGetTest() {
            IntPoint temp = new IntPoint(1, 5);
            IntPoint lp = new IntPoint();
            NUnit.Framework.Assert.AreEqual(1, temp.X);
            NUnit.Framework.Assert.AreEqual(5, temp.Y);
            lp.X = temp.X;
            lp.Y = temp.Y;
            PointTest.AssertLongPointFields(lp, 1, 5);
        }
        
        [NUnit.Framework.Test]
        public void Pt2IsBetweenPt1AndPt3Test() {
            ClipperBase clipperBase = new Clipper();
            IntPoint pt1 = new IntPoint();
            IntPoint pt2 = new IntPoint();
            IntPoint pt3 = new IntPoint();
            NUnit.Framework.Assert.IsFalse(clipperBase.Pt2IsBetweenPt1AndPt3(pt1, pt2, pt3));

            pt3.X = 10L;
            pt3.Y = 10L;
            NUnit.Framework.Assert.IsFalse(clipperBase.Pt2IsBetweenPt1AndPt3(pt1, pt2, pt3));

            pt2.X = 10L;
            pt2.Y = 10L;
            NUnit.Framework.Assert.IsFalse(clipperBase.Pt2IsBetweenPt1AndPt3(pt1, pt2, pt3));

            pt2.X = 5L;
            pt2.Y = 10L;
            NUnit.Framework.Assert.IsTrue(clipperBase.Pt2IsBetweenPt1AndPt3(pt1, pt2, pt3));

            pt1.X = 10L;
            pt1.Y = 0L;
            pt2.X = 10L;
            pt2.Y = 5L;
            NUnit.Framework.Assert.IsTrue(clipperBase.Pt2IsBetweenPt1AndPt3(pt1, pt2, pt3));
        }

        [NUnit.Framework.Test]
        public virtual void SlopesEqualThreePointTest() {
            IntPoint pt1 = new IntPoint(9, 0);
            IntPoint pt2 = new IntPoint(3, 2);
            IntPoint pt3 = new IntPoint(0, 3);
            NUnit.Framework.Assert.IsTrue(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, false));
            NUnit.Framework.Assert.IsTrue(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, true));
            pt1.X = 10L;
            NUnit.Framework.Assert.IsFalse(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, false));
            NUnit.Framework.Assert.IsFalse(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, true));
        }

        [NUnit.Framework.Test]
        public virtual void SlopesEqualFourPointTest() {
            IntPoint pt1 = new IntPoint(6, 0);
            IntPoint pt2 = new IntPoint(3, 3);
            IntPoint pt3 = new IntPoint(3, 2);
            IntPoint pt4 = new IntPoint(0, 5);
            NUnit.Framework.Assert.IsTrue(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, pt4, false));
            NUnit.Framework.Assert.IsTrue(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, pt4, true));
            pt1.X = 10L;
            NUnit.Framework.Assert.IsFalse(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, pt4, false));
            NUnit.Framework.Assert.IsFalse(PublicClipperBase.SlopesEqual(pt1, pt2, pt3, pt4, true));
        }
        

        [NUnit.Framework.Test]
        public virtual void GetUnitNormalTest() {
            IntPoint pt1 = new IntPoint(1, 1);
            IntPoint pt2 = new IntPoint(1, 1);
            NUnit.Framework.Assert.AreEqual(new DoublePoint(), ClipperOffset.GetUnitNormal(pt1, pt2));
            pt2.X = 5L;
            NUnit.Framework.Assert.AreEqual(new DoublePoint(0, -1), ClipperOffset.GetUnitNormal(pt1, pt2));
            pt2.Y = 4L;
            DoublePoint dp = ClipperOffset.GetUnitNormal(pt1, pt2);
            NUnit.Framework.Assert.AreEqual(0.6, dp.X, DOUBLE_EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(-0.8, dp.Y, DOUBLE_EPSILON_COMPARISON);
        }

        private static void AssertLongPointFields(IntPoint lp, long x, long y) {
            NUnit.Framework.Assert.AreEqual(x, lp.X);
            NUnit.Framework.Assert.AreEqual(y, lp.Y);
        }

        private static void AssertDoublePointFields(DoublePoint dp, double x, double y) {
            NUnit.Framework.Assert.AreEqual(x, dp.X, DOUBLE_EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(y, dp.Y, DOUBLE_EPSILON_COMPARISON);
        }

        private class PublicClipperBase : ClipperBase
        {
            public static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
            {
                return ClipperBase.SlopesEqual(pt1, pt2, pt3, UseFullRange);
            }
            
            public static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
            {
                return ClipperBase.SlopesEqual(pt1, pt2, pt3, pt4, UseFullRange);
            }
        }
    }
}
