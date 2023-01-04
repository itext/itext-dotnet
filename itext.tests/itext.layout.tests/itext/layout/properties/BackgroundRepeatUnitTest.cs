/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Geom;
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class BackgroundRepeatUnitTest : ExtendedITextTest {
        private const double EPSILON = 0.000001;

        [NUnit.Framework.Test]
        public virtual void DefaultConstructorTest() {
            BackgroundRepeat backgroundRepeat = new BackgroundRepeat();
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundRepeat.GetXAxisRepeat
                ());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, backgroundRepeat.GetYAxisRepeat
                ());
        }

        [NUnit.Framework.Test]
        public virtual void OneBackgroundRepeatValueConstructorTest() {
            BackgroundRepeat backgroundRepeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND);
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.ROUND, backgroundRepeat.GetXAxisRepeat
                ());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.ROUND, backgroundRepeat.GetYAxisRepeat
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TwoBackgroundRepeatValueConstructorTest() {
            BackgroundRepeat backgroundRepeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE, BackgroundRepeat.BackgroundRepeatValue
                .ROUND);
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.SPACE, backgroundRepeat.GetXAxisRepeat
                ());
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.ROUND, backgroundRepeat.GetYAxisRepeat
                ());
        }

        [NUnit.Framework.Test]
        public virtual void IsNoRepeatOnAxis() {
            BackgroundRepeat backgroundRepeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE, BackgroundRepeat.BackgroundRepeatValue
                .REPEAT);
            NUnit.Framework.Assert.IsFalse(backgroundRepeat.IsNoRepeatOnXAxis());
            NUnit.Framework.Assert.IsFalse(backgroundRepeat.IsNoRepeatOnYAxis());
            backgroundRepeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .ROUND);
            NUnit.Framework.Assert.IsTrue(backgroundRepeat.IsNoRepeatOnXAxis());
            NUnit.Framework.Assert.IsFalse(backgroundRepeat.IsNoRepeatOnYAxis());
            backgroundRepeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT);
            NUnit.Framework.Assert.IsTrue(backgroundRepeat.IsNoRepeatOnXAxis());
            NUnit.Framework.Assert.IsTrue(backgroundRepeat.IsNoRepeatOnYAxis());
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRepeatNoRepeatTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT);
            Rectangle imageRect = new Rectangle(0, 0, 50, 60);
            Rectangle originalRect = new Rectangle(imageRect);
            Rectangle availableArea = new Rectangle(0, 0, 160, 123);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.IsTrue(originalRect.EqualsWithEpsilon(imageRect));
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleSpaceRepeatTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE, BackgroundRepeat.BackgroundRepeatValue
                .REPEAT);
            Rectangle imageRect = new Rectangle(0, 0, 50, 60);
            Rectangle originalRect = new Rectangle(imageRect);
            Rectangle availableArea = new Rectangle(0, 0, 160, 123);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(5, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.IsTrue(originalRect.EqualsWithEpsilon(imageRect));
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleNoRepeatSpaceTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .SPACE);
            Rectangle imageRect = new Rectangle(0, 63, 50, 60);
            Rectangle originalRect = new Rectangle(imageRect);
            Rectangle availableArea = new Rectangle(0, 0, 160, 123);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(3, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.IsTrue(originalRect.EqualsWithEpsilon(imageRect));
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleSpaceSpaceTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE);
            Rectangle imageRect = new Rectangle(0, 63, 50, 60);
            Rectangle originalRect = new Rectangle(imageRect);
            Rectangle availableArea = new Rectangle(0, 0, 160, 123);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(5, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(3, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.IsTrue(originalRect.EqualsWithEpsilon(imageRect));
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleSpaceSpaceNoAvailableSpaceTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE);
            Rectangle imageRect = new Rectangle(0, -5, 50, 60);
            Rectangle originalRect = new Rectangle(imageRect);
            Rectangle availableArea = new Rectangle(0, 0, 45, 55);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.IsTrue(originalRect.EqualsWithEpsilon(imageRect));
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundNoRepeatLessAndMoreHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND, BackgroundRepeat.BackgroundRepeatValue
                .NO_REPEAT);
            Rectangle imageRect = new Rectangle(0, 0, 50, 70);
            Rectangle availableArea = new Rectangle(0, 0, 120, 180);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(-14, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(84, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRepeatRoundLessHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.REPEAT, BackgroundRepeat.BackgroundRepeatValue
                .ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 75);
            Rectangle availableArea = new Rectangle(0, 0, 120, 180);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(-15, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(90, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundRoundLessAndMoreHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 70);
            Rectangle availableArea = new Rectangle(0, 0, 120, 180);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(10, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundRoundMoreAndLessHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 70);
            Rectangle availableArea = new Rectangle(0, 0, 144, 160);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(-10, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(48, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(80, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundRoundMoreHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 70);
            Rectangle availableArea = new Rectangle(0, 0, 144, 180);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(10, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(48, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundRoundLessHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 70);
            Rectangle availableArea = new Rectangle(0, 0, 120, 160);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(-10, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(80, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleSpaceRoundMoreHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.SPACE, BackgroundRepeat.BackgroundRepeatValue
                .ROUND);
            Rectangle imageRect = new Rectangle(0, 0, 50, 75);
            Rectangle availableArea = new Rectangle(0, 0, 130, 180);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(10, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(-15, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(90, imageRect.GetHeight(), EPSILON);
        }

        [NUnit.Framework.Test]
        public virtual void PrepareRectangleRoundSpaceLessHalfImageSizeTest() {
            BackgroundRepeat repeat = new BackgroundRepeat(BackgroundRepeat.BackgroundRepeatValue.ROUND, BackgroundRepeat.BackgroundRepeatValue
                .SPACE);
            Rectangle imageRect = new Rectangle(0, 0, 50, 75);
            Rectangle availableArea = new Rectangle(0, 0, 120, 369);
            Point whitespace = repeat.PrepareRectangleToDrawingAndGetWhitespace(imageRect, availableArea, new BackgroundSize
                ());
            NUnit.Framework.Assert.AreEqual(0, whitespace.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(3, whitespace.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(0, imageRect.GetX(), EPSILON);
            NUnit.Framework.Assert.AreEqual(279, imageRect.GetY(), EPSILON);
            NUnit.Framework.Assert.AreEqual(60, imageRect.GetWidth(), EPSILON);
            NUnit.Framework.Assert.AreEqual(90, imageRect.GetHeight(), EPSILON);
        }
    }
}
