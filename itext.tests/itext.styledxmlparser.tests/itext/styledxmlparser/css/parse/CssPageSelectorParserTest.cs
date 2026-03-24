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
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPageSelectorParserTest : ExtendedITextTest {
        public static Object[] ProvideInvalidSelectorTestData() {
            return new Object[] { ":not(:first)", ":someselectorname", "customPageName :someselectorname", ":someselectorname customPageName"
                , ":left :someselectorname", ":someselectorname :right", ":first :someselectorname :blank", ":invalidselector:first:blank"
                , ":first :blank :invalidselector" };
        }

        public static Object[][] ProvideValidSelectorTestData() {
            return new Object[][] { new Object[] { "", new ICssSelectorItem[] {  } }, new Object[] { "    ", new ICssSelectorItem
                [] {  } }, new Object[] { ":first", new ICssSelectorItem[] { new CssPagePseudoClassSelectorItem("first"
                ) } }, new Object[] { ":right:left", new ICssSelectorItem[] { new CssPagePseudoClassSelectorItem("right"
                ), new CssPagePseudoClassSelectorItem("left") } }, new Object[] { ":first :right", new ICssSelectorItem
                [] { new CssPagePseudoClassSelectorItem("first"), new CssPagePseudoClassSelectorItem("right") } }, new 
                Object[] { ":blank    :first", new ICssSelectorItem[] { new CssPagePseudoClassSelectorItem("blank"), new 
                CssPagePseudoClassSelectorItem("first") } }, new Object[] { ":blank:right:first", new ICssSelectorItem
                [] { new CssPagePseudoClassSelectorItem("blank"), new CssPagePseudoClassSelectorItem("right"), new CssPagePseudoClassSelectorItem
                ("first") } }, new Object[] { "customPageName", new ICssSelectorItem[] { new CssPageTypeSelectorItem("customPageName"
                ) } }, new Object[] { "somePageName:first", new ICssSelectorItem[] { new CssPageTypeSelectorItem("somePageName"
                ), new CssPagePseudoClassSelectorItem("first") } }, new Object[] { "namedPageExample :first :blank", new 
                ICssSelectorItem[] { new CssPageTypeSelectorItem("namedPageExample"), new CssPagePseudoClassSelectorItem
                ("first"), new CssPagePseudoClassSelectorItem("blank") } } };
        }

        [NUnit.Framework.TestCaseSource("ProvideInvalidSelectorTestData")]
        public virtual void InvalidSelectorTest(String selector) {
            IList<ICssSelectorItem> parsedSelector = CssPageSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssPageSelectorParser.NeverMatchSelectorItem);
        }

        [NUnit.Framework.TestCaseSource("ProvideValidSelectorTestData")]
        public virtual void ValidSelectorTest(String selector, ICssSelectorItem[] expectedParsedSelector) {
            IList<ICssSelectorItem> parsedSelector = CssPageSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(expectedParsedSelector.Length, parsedSelector.Count);
            for (int i = 0; i < parsedSelector.Count; i++) {
                NUnit.Framework.Assert.AreEqual(expectedParsedSelector[i].GetType(), parsedSelector[i].GetType());
                NUnit.Framework.Assert.AreEqual(expectedParsedSelector[i].ToString(), parsedSelector[i].ToString());
            }
        }
    }
}
