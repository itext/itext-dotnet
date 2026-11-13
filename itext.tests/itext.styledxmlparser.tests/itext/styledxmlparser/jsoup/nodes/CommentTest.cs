/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
