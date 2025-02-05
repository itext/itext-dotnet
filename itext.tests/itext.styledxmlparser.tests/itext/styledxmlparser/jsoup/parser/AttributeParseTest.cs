/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Test suite for attribute parser.</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class AttributeParseTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParsesRoughAttributeString() {
            String html = "<a id=\"123\" class=\"baz = 'bar'\" style = 'border: 2px'qux zim foo = 12 mux=18 />";
            // should be: <id=123>, <class=baz = 'bar'>, <qux=>, <zim=>, <foo=12>, <mux.=18>
            iText.StyledXmlParser.Jsoup.Nodes.Element el = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).GetElementsByTag
                ("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(7, attr.Size());
            NUnit.Framework.Assert.AreEqual("123", attr.Get("id"));
            NUnit.Framework.Assert.AreEqual("baz = 'bar'", attr.Get("class"));
            NUnit.Framework.Assert.AreEqual("border: 2px", attr.Get("style"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("qux"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("zim"));
            NUnit.Framework.Assert.AreEqual("12", attr.Get("foo"));
            NUnit.Framework.Assert.AreEqual("18", attr.Get("mux"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNewLinesAndReturns() {
            String html = "<a\r\nfoo='bar\r\nqux'\r\nbar\r\n=\r\ntwo>One</a>";
            iText.StyledXmlParser.Jsoup.Nodes.Element el = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("a").First
                ();
            NUnit.Framework.Assert.AreEqual(2, el.Attributes().Size());
            NUnit.Framework.Assert.AreEqual("bar\r\nqux", el.Attr("foo"));
            NUnit.Framework.Assert.AreEqual("two", el.Attr("bar"));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesEmptyString() {
            String html = "<a />";
            iText.StyledXmlParser.Jsoup.Nodes.Element el = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).GetElementsByTag
                ("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(0, attr.Size());
        }

        [NUnit.Framework.Test]
        public virtual void CanStartWithEq() {
            String html = "<a =empty />";
            iText.StyledXmlParser.Jsoup.Nodes.Element el = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).GetElementsByTag
                ("a")[0];
            Attributes attr = el.Attributes();
            NUnit.Framework.Assert.AreEqual(1, attr.Size());
            NUnit.Framework.Assert.IsTrue(attr.HasKey("=empty"));
            NUnit.Framework.Assert.AreEqual("", attr.Get("=empty"));
        }

        [NUnit.Framework.Test]
        public virtual void StrictAttributeUnescapes() {
            String html = "<a id=1 href='?foo=bar&mid&lt=true'>One</a> <a id=2 href='?foo=bar&lt;qux&lg=1'>Two</a>";
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("a");
            NUnit.Framework.Assert.AreEqual("?foo=bar&mid&lt=true", els.First().Attr("href"));
            NUnit.Framework.Assert.AreEqual("?foo=bar<qux&lg=1", els.Last().Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void MoreAttributeUnescapes() {
            String html = "<a href='&wr_id=123&mid-size=true&ok=&wr'>Check</a>";
            Elements els = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("a");
            NUnit.Framework.Assert.AreEqual("&wr_id=123&mid-size=true&ok=&wr", els.First().Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesBooleanAttributes() {
            String html = "<a normal=\"123\" boolean empty=\"\"></a>";
            iText.StyledXmlParser.Jsoup.Nodes.Element el = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html).Select("a").First
                ();
            NUnit.Framework.Assert.AreEqual("123", el.Attr("normal"));
            NUnit.Framework.Assert.AreEqual("", el.Attr("boolean"));
            NUnit.Framework.Assert.AreEqual("", el.Attr("empty"));
            IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> attributes = el.Attributes().AsList();
            NUnit.Framework.Assert.AreEqual(3, attributes.Count);
            NUnit.Framework.Assert.AreEqual(html, el.OuterHtml());
        }

        // vets boolean syntax
        [NUnit.Framework.Test]
        public virtual void DropsSlashFromAttributeName() {
            String html = "<img /onerror='doMyJob'/>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.IsFalse(doc.Select("img[onerror]").IsEmpty());
            NUnit.Framework.Assert.AreEqual("<img onerror=\"doMyJob\">", doc.Body().Html());
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "", iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser
                ());
            NUnit.Framework.Assert.AreEqual("<img onerror=\"doMyJob\" />", doc.Html());
        }
    }
}
