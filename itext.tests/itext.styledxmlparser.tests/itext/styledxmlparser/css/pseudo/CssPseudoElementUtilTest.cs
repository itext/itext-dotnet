using System;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Pseudo {
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
