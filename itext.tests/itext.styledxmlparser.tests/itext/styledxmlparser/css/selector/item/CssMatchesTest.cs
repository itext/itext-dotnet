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
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class CssMatchesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MatchesEmptySelectorItemTest() {
            CssPseudoClassEmptySelectorItem item = CssPseudoClassEmptySelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><input value=\"Alexander\"></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[0];
            NUnit.Framework.Assert.IsTrue(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesEmptySelectorItemNotTaggedTextTest() {
            CssPseudoClassEmptySelectorItem item = CssPseudoClassEmptySelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("Some text!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesEmptySelectorItemSpaceTest() {
            CssPseudoClassEmptySelectorItem item = CssPseudoClassEmptySelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div> </div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesFirstOfTypeSelectorItemTest() {
            CssPseudoClassFirstOfTypeSelectorItem item = CssPseudoClassFirstOfTypeSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[0];
            NUnit.Framework.Assert.IsTrue(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesFirstOfTypeSelectorItemTestNotTaggedText() {
            CssPseudoClassFirstOfTypeSelectorItem item = CssPseudoClassFirstOfTypeSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("Some text!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesLastOfTypeSelectorItemTest() {
            CssPseudoClassLastOfTypeSelectorItem item = CssPseudoClassLastOfTypeSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[1];
            NUnit.Framework.Assert.IsTrue(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesLastOfTypeSelectorItemTestNotTaggedText() {
            CssPseudoClassLastOfTypeSelectorItem item = CssPseudoClassLastOfTypeSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("SomeText!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesLastChildSelectorItemTest() {
            CssPseudoClassLastChildSelectorItem item = CssPseudoClassLastChildSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[1];
            NUnit.Framework.Assert.IsTrue(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesLastChildSelectorItemTestNotTaggedText() {
            CssPseudoClassLastChildSelectorItem item = CssPseudoClassLastChildSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("SomeText!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesNthOfTypeSelectorItemTest() {
            CssPseudoClassNthOfTypeSelectorItem item = new CssPseudoClassNthOfTypeSelectorItem("1n");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[0];
            NUnit.Framework.Assert.IsTrue(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesNthOfTypeSelectorItemTestNotTaggedText() {
            CssPseudoClassNthOfTypeSelectorItem item = new CssPseudoClassNthOfTypeSelectorItem("1n");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("SomeText!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesNthOfTypeSelectorItemTestBadNodeArgument() {
            CssPseudoClassNthOfTypeSelectorItem item = new CssPseudoClassNthOfTypeSelectorItem("text");
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0].ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesRootSelectorItemTest() {
            CssPseudoClassRootSelectorItem item = CssPseudoClassRootSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("<div><p>Alexander</p><p>Alexander</p></div>");
            INode headNode = documentNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsTrue(item.Matches(headNode));
        }

        [NUnit.Framework.Test]
        public virtual void MatchesRootSelectorItemTestNotTaggedText() {
            CssPseudoClassRootSelectorItem item = CssPseudoClassRootSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse("SomeText!");
            INode bodyNode = documentNode.ChildNodes()[0].ChildNodes()[1];
            INode divNode = bodyNode.ChildNodes()[0];
            NUnit.Framework.Assert.IsFalse(item.Matches(divNode));
        }
    }
}
