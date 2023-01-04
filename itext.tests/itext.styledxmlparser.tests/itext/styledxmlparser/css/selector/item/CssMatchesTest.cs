/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
