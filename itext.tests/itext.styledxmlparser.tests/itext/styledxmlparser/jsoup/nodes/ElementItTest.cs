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
using System.Collections.Generic;
using System.Text;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    [NUnit.Framework.Category("UnitTest")]
    public class ElementItTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestFastReparent() {
            StringBuilder htmlBuf = new StringBuilder();
            int rows = 300000;
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
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapperAcutal = doc.Body().Children()[0];
            NUnit.Framework.Assert.AreEqual(wrapper, wrapperAcutal);
            NUnit.Framework.Assert.AreEqual("El-1", wrapperAcutal.Children()[0].Text());
            NUnit.Framework.Assert.AreEqual("El-" + rows, wrapperAcutal.Children()[rows - 1].Text());
            NUnit.Framework.Assert.IsTrue(runtime <= 10000);
        }

        [NUnit.Framework.Test]
        public virtual void TestFastReparentExistingContent() {
            StringBuilder htmlBuf = new StringBuilder();
            int rows = 300000;
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
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = doc.Body().ChildNodes();
            wrapper.InsertChildren(1, childNodes);
            long runtime = (System.DateTime.Now.Ticks - start) / 1000000;
            NUnit.Framework.Assert.AreEqual(rows + 2, wrapper.childNodes.Count);
            NUnit.Framework.Assert.AreEqual(rows, childNodes.Count);
            // child nodes is a wrapper, so still there
            NUnit.Framework.Assert.AreEqual(0, doc.Body().ChildNodes().Count);
            // but on a fresh look, all gone
            ((iText.StyledXmlParser.Jsoup.Nodes.Element)doc.Body().Empty()).AppendChild(wrapper);
            iText.StyledXmlParser.Jsoup.Nodes.Element wrapperAcutal = doc.Body().Children()[0];
            NUnit.Framework.Assert.AreEqual(wrapper, wrapperAcutal);
            NUnit.Framework.Assert.AreEqual("Prior Content", wrapperAcutal.Children()[0].Text());
            NUnit.Framework.Assert.AreEqual("El-1", wrapperAcutal.Children()[1].Text());
            NUnit.Framework.Assert.AreEqual("El-" + rows, wrapperAcutal.Children()[rows].Text());
            NUnit.Framework.Assert.AreEqual("End Content", wrapperAcutal.Children()[rows + 1].Text());
            NUnit.Framework.Assert.IsTrue(runtime <= 10000);
        }
    }
}
