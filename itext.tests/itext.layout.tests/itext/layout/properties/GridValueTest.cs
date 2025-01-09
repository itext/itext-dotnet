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
using iText.Layout.Properties.Grid;
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class GridValueTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnitValueTest() {
            LengthValue value = new PointValue(3.2f);
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.POINT);
            NUnit.Framework.Assert.AreEqual(3.2f, (float)value.GetValue(), 0.00001);
            value = new PercentValue(30f);
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.PERCENT);
            NUnit.Framework.Assert.AreEqual(30, (float)value.GetValue(), 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void MinMaxContentTest() {
            GridValue value = MinContentValue.VALUE;
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.MIN_CONTENT);
            value = MaxContentValue.VALUE;
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.MAX_CONTENT);
        }

        [NUnit.Framework.Test]
        public virtual void AutoTest() {
            GridValue value = AutoValue.VALUE;
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.AUTO);
        }

        [NUnit.Framework.Test]
        public virtual void FlexValueTest() {
            FlexValue value = new FlexValue(1.5f);
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.FLEX);
            NUnit.Framework.Assert.AreEqual(1.5f, (float)value.GetFlex(), 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void FitContentTest() {
            FitContentValue value = new FitContentValue(new PointValue(50.0f));
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.FIT_CONTENT);
            NUnit.Framework.Assert.AreEqual(new PointValue(50.0f).GetValue(), value.GetLength().GetValue(), 0.00001);
            value = new FitContentValue(UnitValue.CreatePercentValue(20.0f));
            NUnit.Framework.Assert.AreEqual(new PercentValue(20.0f).GetValue(), value.GetLength().GetValue(), 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void MinMaxTest() {
            MinMaxValue value = new MinMaxValue(new PointValue(50.0f), new FlexValue(2.0f));
            NUnit.Framework.Assert.AreEqual(value.GetType(), TemplateValue.ValueType.MINMAX);
            NUnit.Framework.Assert.AreEqual(new PointValue(50.0f).GetValue(), ((PointValue)value.GetMin()).GetValue(), 
                0.00001);
            NUnit.Framework.Assert.AreEqual(new FlexValue(2.0f).GetFlex(), ((FlexValue)value.GetMax()).GetFlex(), 0.00001
                );
        }
    }
}
