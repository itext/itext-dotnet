using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Pseudo {
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
            NUnit.Framework.Assert.That(() =>  {
                CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
                IDictionary<String, String> styles = new Dictionary<String, String>();
                styles.Put("font-size", "12px");
                styles.Put("color", "red");
                pseudoElementNode.AddAdditionalHtmlStyles(styles);
                NUnit.Framework.Assert.Fail();
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void GetLangTest() {
            CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
            NUnit.Framework.Assert.IsNull(pseudoElementNode.GetLang());
        }

        [NUnit.Framework.Test]
        public virtual void AttributesStubSetAttributeTest() {
            NUnit.Framework.Assert.That(() =>  {
                CssPseudoElementNode pseudoElementNode = new CssPseudoElementNode(null, "after");
                pseudoElementNode.GetAttributes().SetAttribute("content", "iText");
                NUnit.Framework.Assert.Fail();
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>())
;
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
