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
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class BackgroundSizeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            BackgroundSize size = new BackgroundSize();
            NUnit.Framework.Assert.IsFalse(size.IsContain());
            NUnit.Framework.Assert.IsFalse(size.IsCover());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundWidthSize());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundHeightSize());
        }

        [NUnit.Framework.Test]
        public virtual void ClearAndSetToCoverTest() {
            BackgroundSize size = new BackgroundSize();
            size.SetBackgroundSizeToValues(UnitValue.CreatePointValue(10), UnitValue.CreatePointValue(10));
            size.SetBackgroundSizeToCover();
            NUnit.Framework.Assert.IsFalse(size.IsContain());
            NUnit.Framework.Assert.IsTrue(size.IsCover());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundWidthSize());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundHeightSize());
        }

        [NUnit.Framework.Test]
        public virtual void ClearAndSetToContainTest() {
            BackgroundSize size = new BackgroundSize();
            size.SetBackgroundSizeToValues(UnitValue.CreatePointValue(10), UnitValue.CreatePointValue(10));
            size.SetBackgroundSizeToContain();
            NUnit.Framework.Assert.IsTrue(size.IsContain());
            NUnit.Framework.Assert.IsFalse(size.IsCover());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundWidthSize());
            NUnit.Framework.Assert.IsNull(size.GetBackgroundHeightSize());
        }
    }
}
