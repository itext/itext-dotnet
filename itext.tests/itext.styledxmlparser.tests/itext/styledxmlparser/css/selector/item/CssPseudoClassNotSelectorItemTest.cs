using System;
using iText.Commons.Utils;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPseudoClassNotSelectorItemTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/selector/item/CssPseudoClassNotSelectorItemTest/";

        [NUnit.Framework.Test]
        public virtual void CssPseudoClassNotSelectorItemWithSelectorsListTest() {
            String filename = SOURCE_FOLDER + "cssPseudoClassNotSelectorItemTest.html";
            CssPseudoClassNotSelectorItem item = new CssPseudoClassNotSelectorItem(new CssSelector("p > :not(strong, b.important)"
                ));
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse(FileUtil.GetInputStreamForFile(filename), "UTF-8");
            IElementNode body = new JsoupElementNode(((JsoupDocumentNode)documentNode).GetDocument().GetElementsByTag(
                "body")[0]);
            NUnit.Framework.Assert.IsFalse(item.Matches(documentNode));
            NUnit.Framework.Assert.IsTrue(item.Matches(body));
            NUnit.Framework.Assert.IsFalse(item.Matches(null));
        }
    }
}
