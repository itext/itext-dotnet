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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    [NUnit.Framework.Category("UnitTest")]
    public class CommentTest : ExtendedITextTest {
        private Comment comment = new Comment(" This is one heck of a comment! ");

        private Comment decl = new Comment("?xml encoding='ISO-8859-1'?");

        [NUnit.Framework.Test]
        public virtual void NodeName() {
            NUnit.Framework.Assert.AreEqual("#comment", comment.NodeName());
        }

        [NUnit.Framework.Test]
        public virtual void GetData() {
            NUnit.Framework.Assert.AreEqual(" This is one heck of a comment! ", comment.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void TestToString() {
            NUnit.Framework.Assert.AreEqual("<!-- This is one heck of a comment! -->", comment.ToString());
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><!-- comment--></div>");
            NUnit.Framework.Assert.AreEqual("<div>\n <!-- comment-->\n</div>", doc.Body().Html());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One<!-- comment -->Two</p>");
            NUnit.Framework.Assert.AreEqual("<p>One<!-- comment -->Two</p>", doc.Body().Html());
            NUnit.Framework.Assert.AreEqual("OneTwo", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestHtmlNoPretty() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!-- a simple comment -->");
            doc.OutputSettings().PrettyPrint(false);
            NUnit.Framework.Assert.AreEqual("<!-- a simple comment --><html><head></head><body></body></html>", doc.Html
                ());
            iText.StyledXmlParser.Jsoup.Nodes.Node node = doc.ChildNode(0);
            Comment c1 = (Comment)node;
            NUnit.Framework.Assert.AreEqual("<!-- a simple comment -->", c1.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TestClone() {
            Comment c1 = (Comment)comment.Clone();
            NUnit.Framework.Assert.AreNotSame(comment, c1);
            NUnit.Framework.Assert.AreEqual(comment.GetData(), comment.GetData());
            c1.SetData("New");
            NUnit.Framework.Assert.AreEqual("New", c1.GetData());
            NUnit.Framework.Assert.AreNotEqual(c1.GetData(), comment.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void IsXmlDeclaration() {
            NUnit.Framework.Assert.IsFalse(comment.IsXmlDeclaration());
            NUnit.Framework.Assert.IsTrue(decl.IsXmlDeclaration());
        }

        [NUnit.Framework.Test]
        public virtual void AsXmlDeclaration() {
            XmlDeclaration xmlDeclaration = decl.AsXmlDeclaration();
            NUnit.Framework.Assert.IsNotNull(xmlDeclaration);
        }
    }
}
