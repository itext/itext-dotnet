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
using System.IO;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Jsoup {
    [NUnit.Framework.Category("UnitTest")]
    public class JsoupXmlParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestXmlDeclarationAndComment() {
            String xml = "<?xml version=\"1.0\" standalone=\"no\"?>\n" + "<!-- just declaration and comment -->";
            Stream stream = new MemoryStream(xml.GetBytes());
            IDocumentNode node = new JsoupXmlParser().Parse(stream, "UTF-8");
            // only text (whitespace) child node shall be fetched.
            NUnit.Framework.Assert.AreEqual(1, node.ChildNodes().Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_ADDING_CHILD_NODE)]
        public virtual void TestMessageAddingChild() {
            iText.StyledXmlParser.Jsoup.Nodes.Element jsoupSVGRoot = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), "");
            INode root = new JsoupElementNode(jsoupSVGRoot);
            root.AddChild(null);
            NUnit.Framework.Assert.AreEqual(0, root.ChildNodes().Count);
        }
    }
}
