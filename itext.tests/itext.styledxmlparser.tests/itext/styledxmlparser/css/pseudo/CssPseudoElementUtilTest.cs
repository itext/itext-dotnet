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
using System;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Pseudo {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoElementUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreatePseudoElementTagNameTest() {
            String beforePseudoElemName = CssPseudoElementUtil.CreatePseudoElementTagName("before");
            String expected = "pseudo-element::before";
            NUnit.Framework.Assert.AreEqual(expected, beforePseudoElemName);
        }

        [NUnit.Framework.Test]
        public virtual void HasBeforeAfterElementsNullScenarioTest() {
            NUnit.Framework.Assert.IsFalse(CssPseudoElementUtil.HasBeforeAfterElements(null));
        }

        [NUnit.Framework.Test]
        public virtual void HasBeforeAfterElementsInstanceOfTest() {
            NUnit.Framework.Assert.IsFalse(CssPseudoElementUtil.HasBeforeAfterElements(new CssPseudoElementNode(null, 
                "")));
        }

        [NUnit.Framework.Test]
        public virtual void HasBeforeAfterElementsNodeNameTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("pseudo-element::"), "");
            IElementNode node = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsFalse(CssPseudoElementUtil.HasBeforeAfterElements(node));
        }

        [NUnit.Framework.Test]
        public virtual void HasAfterElementTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("after"), "");
            IElementNode node = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsTrue(CssPseudoElementUtil.HasBeforeAfterElements(node));
        }

        [NUnit.Framework.Test]
        public virtual void HasBeforeElementTest() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("before"), "");
            IElementNode node = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsTrue(CssPseudoElementUtil.HasBeforeAfterElements(node));
        }
    }
}
