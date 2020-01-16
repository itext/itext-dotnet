/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Test suite for attribute parser.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
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
            // currently preserves newlines in quoted attributes. todo confirm if should.
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
            NUnit.Framework.Assert.AreEqual(3, attributes.Count, "There should be 3 attribute present");
            // Assuming the list order always follows the parsed html
            NUnit.Framework.Assert.IsFalse(attributes[0] is BooleanAttribute, "'normal' attribute should not be boolean"
                );
            NUnit.Framework.Assert.IsTrue(attributes[1] is BooleanAttribute, "'boolean' attribute should be boolean");
            NUnit.Framework.Assert.IsFalse(attributes[2] is BooleanAttribute, "'empty' attribute should not be boolean"
                );
            NUnit.Framework.Assert.AreEqual(html, el.OuterHtml());
        }
    }
}
