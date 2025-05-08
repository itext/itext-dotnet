/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.Text;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    [NUnit.Framework.Category("UnitTest")]
    public class ElementItTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestFastReparent() {
            StringBuilder htmlBuf = new StringBuilder();
            int rows = 30000;
            for (int i = 1; i <= rows; i++) {
                htmlBuf.Append("<p>El-").Append(i).Append("</p>");
            }
            String html = htmlBuf.ToString();
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            long start = System.DateTime.Now.Ticks;
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapper = new iText.StyledXmlParser.Jsoup.Nodes.Element("div");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = doc.Body().ChildNodes();
            wrapper.InsertChildren(0, childNodes);
            long runtime = (System.DateTime.Now.Ticks - start) / 1000000;
            NUnit.Framework.Assert.AreEqual(rows, wrapper.childNodes.Count);
            NUnit.Framework.Assert.AreEqual(rows, childNodes.Count);
            // child nodes is a wrapper, so still there
            NUnit.Framework.Assert.AreEqual(0, doc.Body().ChildNodes().Count);
            // but on a fresh look, all gone
            ((iText.StyledXmlParser.Jsoup.Nodes.Element)doc.Body().Empty()).AppendChild(wrapper);
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapperActual = doc.Body().Children()[0];
            NUnit.Framework.Assert.AreEqual(wrapper, wrapperActual);
            NUnit.Framework.Assert.AreEqual("El-1", wrapperActual.Children()[0].Text());
            NUnit.Framework.Assert.AreEqual("El-" + rows, wrapperActual.Children()[rows - 1].Text());
            NUnit.Framework.Assert.IsTrue(runtime <= 1000);
        }

        [NUnit.Framework.Test]
        public virtual void TestFastReparentExistingContent() {
            StringBuilder htmlBuf = new StringBuilder();
            int rows = 30000;
            for (int i = 1; i <= rows; i++) {
                htmlBuf.Append("<p>El-").Append(i).Append("</p>");
            }
            String html = htmlBuf.ToString();
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            long start = System.DateTime.Now.Ticks;
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapper = new iText.StyledXmlParser.Jsoup.Nodes.Element("div");
            wrapper.Append("<p>Prior Content</p>");
            wrapper.Append("<p>End Content</p>");
            NUnit.Framework.Assert.AreEqual(2, wrapper.childNodes.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element docBody = doc.Body();
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = docBody.ChildNodes();
            wrapper.InsertChildren(1, childNodes);
            long runtime = (System.DateTime.Now.Ticks - start) / 1000000;
            NUnit.Framework.Assert.AreEqual(rows + 2, wrapper.childNodes.Count);
            NUnit.Framework.Assert.AreEqual(rows, childNodes.Count);
            // child nodes is a wrapper, so still there
            NUnit.Framework.Assert.AreEqual(0, docBody.ChildNodes().Count);
            // but on a fresh look, all gone
            ((iText.StyledXmlParser.Jsoup.Nodes.Element)docBody.Empty()).AppendChild(wrapper);
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapperAcutal = doc.Body().Children()[0];
            NUnit.Framework.Assert.AreEqual(wrapper, wrapperAcutal);
            Elements children = wrapperAcutal.Children();
            NUnit.Framework.Assert.AreEqual("Prior Content", children[0].Text());
            NUnit.Framework.Assert.AreEqual("El-1", children[1].Text());
            NUnit.Framework.Assert.AreEqual("El-" + rows, children[rows].Text());
            NUnit.Framework.Assert.AreEqual("End Content", children[rows + 1].Text());
            NUnit.Framework.Assert.IsTrue(runtime <= 1000);
        }
    }
}
