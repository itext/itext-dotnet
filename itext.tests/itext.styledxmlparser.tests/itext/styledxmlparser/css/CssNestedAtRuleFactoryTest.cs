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
using iText.StyledXmlParser.Css.Page;
using iText.Test;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class CssNestedAtRuleFactoryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestCreatingNestedRule() {
            CssNestedAtRule pageRule = CssNestedAtRuleFactory.CreateNestedRule("page:first");
            NUnit.Framework.Assert.IsTrue(pageRule is CssPageRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.PAGE, pageRule.GetRuleName());
            NUnit.Framework.Assert.AreEqual(":first", pageRule.GetRuleParameters());
            CssNestedAtRule rightBottomMarginRule = CssNestedAtRuleFactory.CreateNestedRule("bottom-right");
            NUnit.Framework.Assert.IsTrue(rightBottomMarginRule is CssMarginRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.BOTTOM_RIGHT, rightBottomMarginRule.GetRuleName());
            CssNestedAtRule fontFaceRule = CssNestedAtRuleFactory.CreateNestedRule("font-face");
            NUnit.Framework.Assert.IsTrue(fontFaceRule is CssFontFaceRule);
            NUnit.Framework.Assert.AreEqual(CssRuleName.FONT_FACE, fontFaceRule.GetRuleName());
        }
    }
}
