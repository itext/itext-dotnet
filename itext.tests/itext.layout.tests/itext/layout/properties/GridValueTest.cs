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
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class GridValueTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnitValueTest() {
            GridValue value = GridValue.CreatePointValue(3.2f);
            NUnit.Framework.Assert.IsTrue(value.IsPointValue());
            NUnit.Framework.Assert.AreEqual(3.2f, value.GetValue(), 0.00001);
            value = GridValue.CreatePercentValue(30f);
            NUnit.Framework.Assert.IsTrue(value.IsPercentValue());
            NUnit.Framework.Assert.AreEqual(30, value.GetValue(), 0.00001);
        }

        [NUnit.Framework.Test]
        public virtual void MinMaxContentTest() {
            GridValue value = GridValue.CreateMinContentValue();
            NUnit.Framework.Assert.IsTrue(value.IsMinContentValue());
            NUnit.Framework.Assert.IsNull(value.GetValue());
            value = GridValue.CreateMaxContentValue();
            NUnit.Framework.Assert.IsTrue(value.IsMaxContentValue());
            NUnit.Framework.Assert.IsNull(value.GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void AutoTest() {
            GridValue value = GridValue.CreateAutoValue();
            NUnit.Framework.Assert.IsTrue(value.IsAutoValue());
            NUnit.Framework.Assert.IsNull(value.GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void FlexValueTest() {
            GridValue value = GridValue.CreateFlexValue(1.5f);
            NUnit.Framework.Assert.IsTrue(value.IsFlexibleValue());
            NUnit.Framework.Assert.AreEqual(1.5f, (float)value.GetValue(), 0.00001);
        }
    }
}
