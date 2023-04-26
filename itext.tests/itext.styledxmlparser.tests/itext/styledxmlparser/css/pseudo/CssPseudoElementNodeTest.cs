/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Pseudo {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoElementNodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetPseudoElementNameTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.AreEqual("after", pseudoElementNode.GetPseudoElementName());
        }

        [NUnit.Framework.Test]
        public virtual void GetPseudoElementTagNameTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.AreEqual("pseudo-element::after", pseudoElementNode.Name());
        }

        [NUnit.Framework.Test]
        public virtual void GetAttributeStringTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsNull(pseudoElementNode.GetAttribute("after"));
        }

        [NUnit.Framework.Test]
        public virtual void GetAttributesTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsTrue(pseudoElementNode.GetAttributes() is IAttributes);
            NUnit.Framework.Assert.IsFalse(pseudoElementNode.GetAttributes() == pseudoElementNode.GetAttributes());
        }

        [NUnit.Framework.Test]
        public virtual void GetAdditionalHtmlStylesTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsNull(pseudoElementNode.GetAdditionalHtmlStyles());
        }

        [NUnit.Framework.Test]
        public virtual void AddAdditionalHtmlStylesTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("font-size", "12px");
            styles.Put("color", "red");
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => pseudoElementNode.AddAdditionalHtmlStyles
                (styles));
        }

        [NUnit.Framework.Test]
        public virtual void GetLangTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsNull(pseudoElementNode.GetLang());
        }

        [NUnit.Framework.Test]
        public virtual void AttributesStubSetAttributeTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            IAttributes attributes = pseudoElementNode.GetAttributes();
            NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => attributes.SetAttribute("content", "iText"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AttributesStubGetSizeTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.AreEqual(0, pseudoElementNode.GetAttributes().Size());
        }

        [NUnit.Framework.Test]
        public virtual void AttributesStubGetAttributeTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsNull(pseudoElementNode.GetAttributes().GetAttribute("after"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributesStubIteratorTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            foreach (IAttribute attr in pseudoElementNode.GetAttributes()) {
                NUnit.Framework.Assert.Fail("AttributesStub must return an empty iterator");
            }
        }
    }
}
