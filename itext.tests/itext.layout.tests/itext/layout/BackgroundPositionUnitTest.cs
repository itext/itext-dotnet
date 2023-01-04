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
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class BackgroundPositionUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultConstructorTest() {
            BackgroundPosition position = new BackgroundPosition();
            NUnit.Framework.Assert.AreEqual(new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.LEFT).SetPositionY
                (BackgroundPosition.PositionY.TOP).SetXShift(new UnitValue(UnitValue.POINT, 0)).SetYShift(new UnitValue
                (UnitValue.POINT, 0)), position);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessPercentageTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.TOP).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            UnitValue xPosition = UnitValue.CreatePointValue(0);
            UnitValue yPosition = UnitValue.CreatePointValue(0);
            position.CalculatePositionValues(1000, 300, xPosition, yPosition);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 970), xPosition);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 30), yPosition);
            position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetPositionY(BackgroundPosition.PositionY
                .BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue(UnitValue.PERCENT, 10));
            xPosition = UnitValue.CreatePointValue(0);
            yPosition = UnitValue.CreatePointValue(0);
            position.CalculatePositionValues(1000, 300, xPosition, yPosition);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), xPosition);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 270), yPosition);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessValidPositionTest() {
            BackgroundPosition position = new BackgroundPosition().SetXShift(new UnitValue(UnitValue.PERCENT, 17));
            UnitValue valueX = new UnitValue(UnitValue.PERCENT, 0);
            UnitValue valueY = new UnitValue(UnitValue.PERCENT, 0);
            position.CalculatePositionValues(1000, 1000, valueX, valueY);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 170), valueX);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), valueY);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessInvalidPositionTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetXShift
                (new UnitValue(UnitValue.PERCENT, 17)).SetPositionY(BackgroundPosition.PositionY.CENTER).SetYShift(UnitValue
                .CreatePointValue(40));
            UnitValue valueX = new UnitValue(UnitValue.PERCENT, 0);
            UnitValue valueY = new UnitValue(UnitValue.PERCENT, 0);
            position.CalculatePositionValues(1000, 1000, valueX, valueY);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), valueX);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), valueY);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessCenterWithoutShiftPositionTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetXShift
                (null).SetPositionY(BackgroundPosition.PositionY.CENTER).SetYShift(null);
            UnitValue valueX = new UnitValue(UnitValue.PERCENT, 0);
            UnitValue valueY = new UnitValue(UnitValue.PERCENT, 0);
            position.CalculatePositionValues(1000, 1000, valueX, valueY);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 500), valueX);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 500), valueY);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessCenterWithZeroShiftPositionTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetXShift
                (UnitValue.CreatePointValue(0)).SetPositionY(BackgroundPosition.PositionY.CENTER).SetYShift(UnitValue.
                CreatePercentValue(0));
            UnitValue valueX = new UnitValue(UnitValue.PERCENT, 0);
            UnitValue valueY = new UnitValue(UnitValue.PERCENT, 0);
            position.CalculatePositionValues(1000, 1000, valueX, valueY);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 500), valueX);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 500), valueY);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessCenterWithAlmostZeroShiftPositionTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.CENTER).SetXShift
                (UnitValue.CreatePointValue(0.002f)).SetPositionY(BackgroundPosition.PositionY.CENTER).SetYShift(UnitValue
                .CreatePercentValue(0.0002f));
            UnitValue valueX = new UnitValue(UnitValue.PERCENT, 0);
            UnitValue valueY = new UnitValue(UnitValue.PERCENT, 0);
            position.CalculatePositionValues(1000, 1000, valueX, valueY);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), valueX);
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 0), valueY);
        }

        [NUnit.Framework.Test]
        public virtual void GettersTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreEqual(BackgroundPosition.PositionX.RIGHT, position.GetPositionX());
            NUnit.Framework.Assert.AreEqual(BackgroundPosition.PositionY.BOTTOM, position.GetPositionY());
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 30), position.GetXShift());
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.PERCENT, 10), position.GetYShift());
        }

        [NUnit.Framework.Test]
        public virtual void SettersTest() {
            BackgroundPosition position = new BackgroundPosition();
            position.SetPositionX(BackgroundPosition.PositionX.RIGHT);
            position.SetPositionY(BackgroundPosition.PositionY.BOTTOM);
            position.SetXShift(new UnitValue(UnitValue.POINT, 30));
            position.SetYShift(new UnitValue(UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreEqual(BackgroundPosition.PositionX.RIGHT, position.GetPositionX());
            NUnit.Framework.Assert.AreEqual(BackgroundPosition.PositionY.BOTTOM, position.GetPositionY());
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.POINT, 30), position.GetXShift());
            NUnit.Framework.Assert.AreEqual(new UnitValue(UnitValue.PERCENT, 10), position.GetYShift());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsTest() {
            BackgroundPosition position1 = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            BackgroundPosition position2 = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreEqual(position1, position2);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsSameObjectTest() {
            BackgroundPosition position1 = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreEqual(position1, position1);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsNullTest() {
            BackgroundPosition position1 = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreNotEqual(position1, null);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWrongTypeTest() {
            BackgroundPosition position1 = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreNotEqual(position1, 5);
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeTest() {
            BackgroundPosition position = new BackgroundPosition().SetPositionX(BackgroundPosition.PositionX.RIGHT).SetPositionY
                (BackgroundPosition.PositionY.BOTTOM).SetXShift(new UnitValue(UnitValue.POINT, 30)).SetYShift(new UnitValue
                (UnitValue.PERCENT, 10));
            NUnit.Framework.Assert.AreEqual(1028641704, position.GetHashCode());
            position.SetXShift(new UnitValue(UnitValue.POINT, 37));
            NUnit.Framework.Assert.AreEqual(1101779880, position.GetHashCode());
        }
    }
}
