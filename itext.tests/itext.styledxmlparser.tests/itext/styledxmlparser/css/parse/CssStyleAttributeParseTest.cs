using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    public class CssStyleAttributeParseTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/parse/CssStyleAttributeParseTest/";

        [NUnit.Framework.Test]
        public virtual void StyleAttributeParseTest() {
            String fileName = SOURCE_FOLDER + "cssStyleAttributeParse.html";
            IXmlParser parser = new JsoupHtmlParser();
            IDocumentNode document = parser.Parse(new FileStream(fileName, FileMode.Open, FileAccess.Read), "UTF-8");
            IList<String> styleDeclarations = new List<String>();
            IList<String> expectStyleDeclarations = new List<String>();
            expectStyleDeclarations.Add("display:none;");
            expectStyleDeclarations.Add("position:relative;");
            expectStyleDeclarations.Add("display:none");
            expectStyleDeclarations.Add("text-align:center;");
            expectStyleDeclarations.Add("white-space:nowrap;");
            expectStyleDeclarations.Add("float:right; clear:right; width:22.0em; margin:0 0 1.0em 1.0em; background:#f9f9f9;"
                 + " border:1px solid #aaa; padding:0.2em; border-spacing:0.4em 0; text-align:center;" + " line-height:1.4em; font-size:88%;"
                );
            expectStyleDeclarations.Add("padding:0.2em 0.4em 0.2em; font-size:145%; line-height:1.15em; font-weight:bold;"
                 + " display:block; margin-bottom:0.25em;");
            ParseStyleAttrForSubtree(document, styleDeclarations);
            NUnit.Framework.Assert.AreEqual(styleDeclarations.Count, expectStyleDeclarations.Count);
            for (int i = 0; i < expectStyleDeclarations.Count; i++) {
                NUnit.Framework.Assert.AreEqual(expectStyleDeclarations[i], styleDeclarations[i]);
            }
        }

        private void ParseOwnStyleAttr(IElementNode element, IList<String> styleDeclarations) {
            IAttributes attributes = element.GetAttributes();
            String styleAttr = attributes.GetAttribute("style");
            if (styleAttr != null && styleAttr.Length > 0) {
                styleDeclarations.Add(styleAttr);
            }
        }

        private void ParseStyleAttrForSubtree(INode node, IList<String> styleDeclarations) {
            if (node is IElementNode) {
                ParseOwnStyleAttr((IElementNode)node, styleDeclarations);
            }
            foreach (INode child in node.ChildNodes()) {
                ParseStyleAttrForSubtree(child, styleDeclarations);
            }
        }
    }
}
