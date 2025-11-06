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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Exceptions;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("UnitTest")]
    public class CssSelectorParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SelectorBeginsWithSpaceTest() {
            String space = " ";
            String selectorWithSpaceAtTheBeginning = space + ".spaceBefore";
            Exception expectedException = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => CssSelectorParser
                .ParseSelectorItems(selectorWithSpaceAtTheBeginning));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_SELECTOR_STRING
                , space), expectedException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PseudoClassSelectorTest() {
            String selector = "li:first-of-type + li";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(4, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("li", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssPseudoClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(":first-of-type", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" + ", parsedSelector[2].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[3] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("li", parsedSelector[3].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void PseudoClassSelectorWithParametersTest() {
            String selector = "li:nth-child(-n + 3)";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(2, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("li", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssPseudoClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(":nth-child(-n + 3)", parsedSelector[1].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void AttributeSelectorTest() {
            String selector = "a[class~=\"logo\"]";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(2, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("a", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssAttributeSelectorItem);
            NUnit.Framework.Assert.AreEqual("[class~=\"logo\"]", parsedSelector[1].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void IdAttributeSelectorTest() {
            String selector = "[id=\"id_value\"]";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssAttributeSelectorItem);
            NUnit.Framework.Assert.AreEqual("[id=\"id_value\"]", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void UniversalAttributeSelectorTest() {
            String selector = "* [id=\"id_value\"]";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(3, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("*", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" ", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssAttributeSelectorItem);
            NUnit.Framework.Assert.AreEqual("[id=\"id_value\"]", parsedSelector[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void IdSelectorTest() {
            String selector = "#id_value";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssIdSelectorItem);
            NUnit.Framework.Assert.AreEqual("#id_value", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void UniversalIdSelectorTest() {
            String selector = "*#maincontent";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(2, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("*", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssIdSelectorItem);
            NUnit.Framework.Assert.AreEqual("#maincontent", parsedSelector[1].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnSelectorTest() {
            String selector = "::column";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssPseudoElementSelectorItem);
            NUnit.Framework.Assert.AreEqual("::column", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ChildCombinatorSelectorTest() {
            String selector = "ul.my-things > li";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(4, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("ul", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(".my-things", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" > ", parsedSelector[2].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[3] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("li", parsedSelector[3].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SubsequentSiblingCombinatorSelectorTest() {
            String selector = "img ~ p";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(3, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("img", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" ~ ", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("p", parsedSelector[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void DescendantCombinatorSelectorTest() {
            String selector = "ul.my-things li";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(4, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("ul", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(".my-things", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" ", parsedSelector[2].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[3] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("li", parsedSelector[3].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void BlankNamespaceSeparatorSelectorTest() {
            //| is unsupported
            String selector = "|h2";
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => CssSelectorParser.ParseSelectorItems(selector
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ListSelectorTest() {
            String selector = "span, div";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(3, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("span", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" , ", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("div", parsedSelector[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SeparatorAtStartTest() {
            String selector = "+test";
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => CssSelectorParser.ParseSelectorItems(selector
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SelectorWithEscapedDotsTest() {
            String selector = "#ReleaseiTextCore9\\.3\\.0-FAQ\\(latestones\\)";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssIdSelectorItem);
            NUnit.Framework.Assert.AreEqual("#ReleaseiTextCore9.3.0-FAQ(latestones)", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ComplexPseudoSelectorTest() {
            String selector = "b:not(:last-of-type)::after";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(3, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("b", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssPseudoClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(":not(:last-of-type)", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssPseudoElementSelectorItem);
            NUnit.Framework.Assert.AreEqual("::after", parsedSelector[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EscapeAtTheEndTest() {
            String selector = "abc\\";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("abc\uFFFD", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EscapedHexTest() {
            String selector = "abc\\000020def";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("abc def", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EscapedHexAfterSeparatorTest() {
            String selector = "p+\\000020def";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(3, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("p", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssSeparatorSelectorItem);
            NUnit.Framework.Assert.AreEqual(" + ", parsedSelector[1].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[2] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual(" def", parsedSelector[2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyClassTest() {
            String selector = ".";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(".", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyIdTest() {
            String selector = "#";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssIdSelectorItem);
            NUnit.Framework.Assert.AreEqual("#", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void EmptyAttributeTest() {
            String selector = "[]";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssAttributeSelectorItem);
            NUnit.Framework.Assert.AreEqual("[]", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void SlashesInTagTest() {
            String selector = "//";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("//", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void Utf32EscapeTest() {
            String selector = ".\\1F600";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(1, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssClassSelectorItem);
            //U+1F600 Grinning Face goes beyond UTF-16 and results in a surrogate pair which is handled poorly by the parser
            NUnit.Framework.Assert.AreEqual(".\uF600", parsedSelector[0].ToString());
            NUnit.Framework.Assert.AreNotEqual(".\uD83D\uDE00", parsedSelector[0].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CamelCaseInPseudoStateTest() {
            String selector = "dd:Nth-Last-Of-Type(EvEn)";
            IList<ICssSelectorItem> parsedSelector = CssSelectorParser.ParseSelectorItems(selector);
            NUnit.Framework.Assert.AreEqual(2, parsedSelector.Count);
            NUnit.Framework.Assert.IsTrue(parsedSelector[0] is CssTagSelectorItem);
            NUnit.Framework.Assert.AreEqual("dd", parsedSelector[0].ToString());
            NUnit.Framework.Assert.IsTrue(parsedSelector[1] is CssPseudoClassSelectorItem);
            NUnit.Framework.Assert.AreEqual(":nth-last-of-type(EvEn)", parsedSelector[1].ToString());
        }
    }
}
