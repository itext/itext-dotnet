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
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Parse {
    [NUnit.Framework.Category("IntegrationTest")]
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
