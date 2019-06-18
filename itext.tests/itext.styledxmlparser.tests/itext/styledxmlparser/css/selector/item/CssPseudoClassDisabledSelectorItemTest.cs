using System;
using System.IO;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;

namespace iText.StyledXmlParser.Css.Selector.Item {
    public class CssPseudoClassDisabledSelectorItemTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/selector/item/CssPseudoClassDisabledSelectorItemTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TestDisabledSelector() {
            String filename = sourceFolder + "disabled.html";
            CssPseudoClassDisabledSelectorItem item = CssPseudoClassDisabledSelectorItem.GetInstance();
            IXmlParser htmlParser = new JsoupHtmlParser();
            IDocumentNode documentNode = htmlParser.Parse(new FileStream(filename, FileMode.Open, FileAccess.Read), "UTF-8"
                );
            IElementNode disabledInput = new JsoupElementNode(((JsoupDocumentNode)documentNode).GetDocument().GetElementsByTag
                ("input").First());
            IElementNode enabledInput = new JsoupElementNode(((JsoupDocumentNode)documentNode).GetDocument().GetElementsByTag
                ("input")[1]);
            NUnit.Framework.Assert.IsFalse(item.Matches(documentNode));
            NUnit.Framework.Assert.IsTrue(item.Matches(disabledInput));
            NUnit.Framework.Assert.IsFalse(item.Matches(enabledInput));
            NUnit.Framework.Assert.IsFalse(item.Matches(null));
        }
    }
}
