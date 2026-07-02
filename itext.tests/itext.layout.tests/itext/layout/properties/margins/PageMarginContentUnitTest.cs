/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Properties.Margins {
    [NUnit.Framework.Category("UnitTest")]
    public class PageMarginContentUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void StaticMarginsConstructorTest() {
            PageMarginContent pageMarginContentTop = new PageMarginContent(MarginBoxName.TOP, 100);
            UnitValue topMarginHeight = ((Div)pageMarginContentTop.GetContent()).GetHeight();
            UnitValue topMarginWidth = ((Div)pageMarginContentTop.GetContent()).GetWidth();
            NUnit.Framework.Assert.AreEqual(100, topMarginHeight.GetValue());
            NUnit.Framework.Assert.IsNull(topMarginWidth);
            PageMarginContent pageMarginContentBottom = new PageMarginContent(MarginBoxName.BOTTOM, 150);
            UnitValue bottomMarginHeight = ((Div)pageMarginContentBottom.GetContent()).GetHeight();
            UnitValue bottomMarginWidth = ((Div)pageMarginContentBottom.GetContent()).GetWidth();
            NUnit.Framework.Assert.AreEqual(150, bottomMarginHeight.GetValue());
            NUnit.Framework.Assert.IsNull(bottomMarginWidth);
            PageMarginContent pageMarginContentLeft = new PageMarginContent(MarginBoxName.LEFT, 60);
            UnitValue leftMarginHeight = ((Div)pageMarginContentLeft.GetContent()).GetHeight();
            UnitValue leftMarginWidth = ((Div)pageMarginContentLeft.GetContent()).GetWidth();
            NUnit.Framework.Assert.IsNull(leftMarginHeight);
            NUnit.Framework.Assert.AreEqual(60, leftMarginWidth.GetValue());
            PageMarginContent pageMarginContentRight = new PageMarginContent(MarginBoxName.RIGHT, 200);
            UnitValue rightMarginHeight = ((Div)pageMarginContentRight.GetContent()).GetHeight();
            UnitValue rightMarginWidth = ((Div)pageMarginContentRight.GetContent()).GetWidth();
            NUnit.Framework.Assert.IsNull(rightMarginHeight);
            NUnit.Framework.Assert.AreEqual(200, rightMarginWidth.GetValue());
        }
    }
}
