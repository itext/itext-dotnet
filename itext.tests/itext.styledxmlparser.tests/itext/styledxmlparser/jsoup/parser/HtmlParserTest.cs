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
using System.IO;
using System.Text;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Integration;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tests for the Parser</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class HtmlParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParsesSimpleDocument() {
            String html = "<html><head><title>First!</title></head><body><p>First post! <img src=\"foo.png\" /></p></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            // need a better way to verify these:
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Body().Child(0);
            NUnit.Framework.Assert.AreEqual("p", p.TagName());
            iText.StyledXmlParser.Jsoup.Nodes.Element img = p.Child(0);
            NUnit.Framework.Assert.AreEqual("foo.png", img.Attr("src"));
            NUnit.Framework.Assert.AreEqual("img", img.TagName());
        }

        [NUnit.Framework.Test]
        public virtual void ParsesRoughAttributes() {
            String html = "<html><head><title>First!</title></head><body><p class=\"foo > bar\">First post! <img src=\"foo.png\" /></p></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            // need a better way to verify these:
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Body().Child(0);
            NUnit.Framework.Assert.AreEqual("p", p.TagName());
            NUnit.Framework.Assert.AreEqual("foo > bar", p.Attr("class"));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesQuiteRoughAttributes() {
            String html = "<p =a>One<a <p>Something</p>Else";
            // this gets a <p> with attr '=a' and an <a tag with an attribue named '<p'; and then auto-recreated
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<p =a>One<a <p>Something</a></p>\n" + "<a <p>Else</a>", doc.Body().Html()
                );
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p .....>");
            NUnit.Framework.Assert.AreEqual("<p .....></p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void ParsesComments() {
            String html = "<html><head></head><body><img src=foo><!-- <table><tr><td></table> --><p>Hello</p></body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            Comment comment = (Comment)body.ChildNode(1);
            // comment should not be sub of img, as it's an empty tag
            NUnit.Framework.Assert.AreEqual(" <table><tr><td></table> ", comment.GetData());
            iText.StyledXmlParser.Jsoup.Nodes.Element p = body.Child(1);
            TextNode text = (TextNode)p.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("Hello", text.GetWholeText());
        }

        [NUnit.Framework.Test]
        public virtual void ParsesUnterminatedComments() {
            String html = "<p>Hello<!-- <tr><td>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.GetElementsByTag("p")[0];
            NUnit.Framework.Assert.AreEqual("Hello", p.Text());
            TextNode text = (TextNode)p.ChildNode(0);
            NUnit.Framework.Assert.AreEqual("Hello", text.GetWholeText());
            Comment comment = (Comment)p.ChildNode(1);
            NUnit.Framework.Assert.AreEqual(" <tr><td>", comment.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void DropsUnterminatedTag() {
            // jsoup used to parse this to <p>, but whatwg, webkit will drop.
            String h1 = "<p";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h1);
            NUnit.Framework.Assert.AreEqual(0, doc.GetElementsByTag("p").Count);
            NUnit.Framework.Assert.AreEqual("", doc.Text());
            String h2 = "<div id=1<p id='2'";
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h2);
            NUnit.Framework.Assert.AreEqual("", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void DropsUnterminatedAttribute() {
            // jsoup used to parse this to <p id="foo">, but whatwg, webkit will drop.
            String h1 = "<p id=\"foo";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h1);
            NUnit.Framework.Assert.AreEqual("", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void ParsesUnterminatedTextarea() {
            // don't parse right to end, but break on <p>
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body><p><textarea>one<p>two");
            iText.StyledXmlParser.Jsoup.Nodes.Element t = doc.Select("textarea").First();
            NUnit.Framework.Assert.AreEqual("one", t.Text());
            NUnit.Framework.Assert.AreEqual("two", doc.Select("p")[1].Text());
        }

        [NUnit.Framework.Test]
        public virtual void ParsesUnterminatedOption() {
            // bit weird this -- browsers and spec get stuck in select until there's a </select>
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body><p><select><option>One<option>Two</p><p>Three</p>"
                );
            Elements options = doc.Select("option");
            NUnit.Framework.Assert.AreEqual(2, options.Count);
            NUnit.Framework.Assert.AreEqual("One", options.First().Text());
            NUnit.Framework.Assert.AreEqual("TwoThree", options.Last().Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestSpaceAfterTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div > <a name=\"top\"></a ><p id=1 >Hello</p></div>"
                );
            NUnit.Framework.Assert.AreEqual("<div> <a name=\"top\"></a><p id=\"1\">Hello</p></div>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void CreatesDocumentStructure() {
            String html = "<meta name=keywords /><link rel=stylesheet /><title>jsoup</title><p>Hello world</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element head = doc.Head();
            iText.StyledXmlParser.Jsoup.Nodes.Element body = doc.Body();
            NUnit.Framework.Assert.AreEqual(1, doc.Children().Count);
            // root node: contains html node
            NUnit.Framework.Assert.AreEqual(2, doc.Child(0).Children().Count);
            // html node: head and body
            NUnit.Framework.Assert.AreEqual(3, head.Children().Count);
            NUnit.Framework.Assert.AreEqual(1, body.Children().Count);
            NUnit.Framework.Assert.AreEqual("keywords", head.GetElementsByTag("meta")[0].Attr("name"));
            NUnit.Framework.Assert.AreEqual(0, body.GetElementsByTag("meta").Count);
            NUnit.Framework.Assert.AreEqual("jsoup", doc.Title());
            NUnit.Framework.Assert.AreEqual("Hello world", body.Text());
            NUnit.Framework.Assert.AreEqual("Hello world", body.Children()[0].Text());
        }

        [NUnit.Framework.Test]
        public virtual void CreatesStructureFromBodySnippet() {
            // the bar baz stuff naturally goes into the body, but the 'foo' goes into root, and the normalisation routine
            // needs to move into the start of the body
            String html = "foo <b>bar</b> baz";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("foo bar baz", doc.Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesEscapedData() {
            String html = "<div title='Surf &amp; Turf'>Reef &amp; Beef</div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementsByTag("div")[0];
            NUnit.Framework.Assert.AreEqual("Surf & Turf", div.Attr("title"));
            NUnit.Framework.Assert.AreEqual("Reef & Beef", div.Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesDataOnlyTags() {
            String t = "<style>font-family: bold</style>";
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> tels = iText.StyledXmlParser.Jsoup.Jsoup.Parse(t).GetElementsByTag
                ("style");
            NUnit.Framework.Assert.AreEqual("font-family: bold", tels[0].Data());
            NUnit.Framework.Assert.AreEqual("", tels[0].Text());
            String s = "<p>Hello</p><script>obj.insert('<a rel=\"none\" />');\ni++;</script><p>There</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(s);
            NUnit.Framework.Assert.AreEqual("Hello There", doc.Text());
            NUnit.Framework.Assert.AreEqual("obj.insert('<a rel=\"none\" />');\ni++;", doc.Data());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesTextAfterData() {
            String h = "<html><body>pre <script>inner</script> aft</body></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<html><head></head><body>pre <script>inner</script> aft</body></html>", TextUtil
                .StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesTextArea() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<textarea>Hello</textarea>");
            Elements els = doc.Select("textarea");
            NUnit.Framework.Assert.AreEqual("Hello", els.Text());
            NUnit.Framework.Assert.AreEqual("Hello", els.Val());
        }

        [NUnit.Framework.Test]
        public virtual void PreservesSpaceInTextArea() {
            // preserve because the tag is marked as preserve white space
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<textarea>\n\tOne\n\tTwo\n\tThree\n</textarea>");
            String expect = "One\n\tTwo\n\tThree";
            // the leading and trailing spaces are dropped as a convenience to authors
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("textarea").First();
            NUnit.Framework.Assert.AreEqual(expect, el.Text());
            NUnit.Framework.Assert.AreEqual(expect, el.Val());
            NUnit.Framework.Assert.AreEqual(expect, el.Html());
            NUnit.Framework.Assert.AreEqual("<textarea>\n\t" + expect + "\n</textarea>", el.OuterHtml());
        }

        // but preserved in round-trip html
        [NUnit.Framework.Test]
        public virtual void PreservesSpaceInScript() {
            // preserve because it's content is a data node
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>\nOne\n\tTwo\n\tThree\n</script>");
            String expect = "\nOne\n\tTwo\n\tThree\n";
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("script").First();
            NUnit.Framework.Assert.AreEqual(expect, el.Data());
            NUnit.Framework.Assert.AreEqual("One\n\tTwo\n\tThree", el.Html());
            NUnit.Framework.Assert.AreEqual("<script>" + expect + "</script>", el.OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void DoesNotCreateImplicitLists() {
            // old jsoup used to wrap this in <ul>, but that's not to spec
            String h = "<li>Point one<li>Point two";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements ol = doc.Select("ul");
            // should NOT have created a default ul.
            NUnit.Framework.Assert.AreEqual(0, ol.Count);
            Elements lis = doc.Select("li");
            NUnit.Framework.Assert.AreEqual(2, lis.Count);
            NUnit.Framework.Assert.AreEqual("body", ((iText.StyledXmlParser.Jsoup.Nodes.Element)lis.First().Parent()).
                TagName());
            // no fiddling with non-implicit lists
            String h2 = "<ol><li><p>Point the first<li><p>Point the second";
            Document doc2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h2);
            NUnit.Framework.Assert.AreEqual(0, doc2.Select("ul").Count);
            NUnit.Framework.Assert.AreEqual(1, doc2.Select("ol").Count);
            NUnit.Framework.Assert.AreEqual(2, doc2.Select("ol li").Count);
            NUnit.Framework.Assert.AreEqual(2, doc2.Select("ol li p").Count);
            NUnit.Framework.Assert.AreEqual(1, doc2.Select("ol li")[0].Children().Count);
        }

        // one p in first li
        [NUnit.Framework.Test]
        public virtual void DiscardsNakedTds() {
            // jsoup used to make this into an implicit table; but browsers make it into a text run
            String h = "<td>Hello<td><p>There<p>now";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("Hello<p>There</p><p>now</p>", TextUtil.StripNewlines(doc.Body().Html()));
        }

        // <tbody> is introduced if no implicitly creating table, but allows tr to be directly under table
        [NUnit.Framework.Test]
        public virtual void HandlesNestedImplicitTable() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><td>1</td></tr> <td>2</td></tr> <td> <table><td>3</td> <td>4</td></table> <tr><td>5</table>"
                );
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><td>1</td></tr> <tr><td>2</td></tr> <tr><td> <table><tbody><tr><td>3</td> <td>4</td></tr></tbody></table> </td></tr><tr><td>5</td></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesWhatWgExpensesTableExample() {
            // http://www.whatwg.org/specs/web-apps/current-work/multipage/tabular-data.html#examples-0
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table> <colgroup> <col> <colgroup> <col> <col> <col> <thead> <tr> <th> <th>2008 <th>2007 <th>2006 <tbody> <tr> <th scope=rowgroup> Research and development <td> $ 1,109 <td> $ 782 <td> $ 712 <tr> <th scope=row> Percentage of net sales <td> 3.4% <td> 3.3% <td> 3.7% <tbody> <tr> <th scope=rowgroup> Selling, general, and administrative <td> $ 3,761 <td> $ 2,963 <td> $ 2,433 <tr> <th scope=row> Percentage of net sales <td> 11.6% <td> 12.3% <td> 12.6% </table>"
                );
            NUnit.Framework.Assert.AreEqual("<table> <colgroup> <col> </colgroup><colgroup> <col> <col> <col> </colgroup><thead> <tr> <th> </th><th>2008 </th><th>2007 </th><th>2006 </th></tr></thead><tbody> <tr> <th scope=\"rowgroup\"> Research and development </th><td> $ 1,109 </td><td> $ 782 </td><td> $ 712 </td></tr><tr> <th scope=\"row\"> Percentage of net sales </th><td> 3.4% </td><td> 3.3% </td><td> 3.7% </td></tr></tbody><tbody> <tr> <th scope=\"rowgroup\"> Selling, general, and administrative </th><td> $ 3,761 </td><td> $ 2,963 </td><td> $ 2,433 </td></tr><tr> <th scope=\"row\"> Percentage of net sales </th><td> 11.6% </td><td> 12.3% </td><td> 12.6% </td></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesTbodyTable() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><head></head><body><table><tbody><tr><td>aaa</td><td>bbb</td></tr></tbody></table></body></html>"
                );
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><td>aaa</td><td>bbb</td></tr></tbody></table>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesImplicitCaptionClose() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><caption>A caption<td>One<td>Two");
            NUnit.Framework.Assert.AreEqual("<table><caption>A caption</caption><tbody><tr><td>One</td><td>Two</td></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void NoTableDirectInTable() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table> <td>One <td><table><td>Two</table> <table><td>Three"
                );
            NUnit.Framework.Assert.AreEqual("<table> <tbody><tr><td>One </td><td><table><tbody><tr><td>Two</td></tr></tbody></table> <table><tbody><tr><td>Three</td></tr></tbody></table></td></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void IgnoresDupeEndTrTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<table><tr><td>One</td><td><table><tr><td>Two</td></tr></tr></table></td><td>Three</td></tr></table>"
                );
            // two </tr></tr>, must ignore or will close table
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><td>One</td><td><table><tbody><tr><td>Two</td></tr></tbody></table></td><td>Three</td></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesBaseTags() {
            // only listen to the first base href
            String h = "<a href=1>#</a><base href='/2/'><a href='3'>#</a><base href='http://bar'><a href=/4>#</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h, "http://foo/");
            NUnit.Framework.Assert.AreEqual("http://foo/2/", doc.BaseUri());
            // gets set once, so doc and descendants have first only
            Elements anchors = doc.GetElementsByTag("a");
            NUnit.Framework.Assert.AreEqual(3, anchors.Count);
            NUnit.Framework.Assert.AreEqual("http://foo/2/", anchors[0].BaseUri());
            NUnit.Framework.Assert.AreEqual("http://foo/2/", anchors[1].BaseUri());
            NUnit.Framework.Assert.AreEqual("http://foo/2/", anchors[2].BaseUri());
            NUnit.Framework.Assert.AreEqual("http://foo/2/1", anchors[0].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://foo/2/3", anchors[1].AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("http://foo/4", anchors[2].AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesProtocolRelativeUrl() {
            String @base = "https://example.com/";
            String html = "<img src='//example.net/img.jpg'>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, @base);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("img").First();
            NUnit.Framework.Assert.AreEqual("https://example.net/img.jpg", el.AbsUrl("src"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCdata() {
            // todo: as this is html namespace, should actually treat as bogus comment, not cdata. keep as cdata for now
            String h = "<div id=1><![CDATA[<html>\n<foo><&amp;]]></div>";
            // the &amp; in there should remain literal
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element div = doc.GetElementById("1");
            NUnit.Framework.Assert.AreEqual("<html> <foo><&amp;", div.Text());
            NUnit.Framework.Assert.AreEqual(0, div.Children().Count);
            NUnit.Framework.Assert.AreEqual(1, div.ChildNodeSize());
        }

        // no elements, one text node
        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedCdataAtEOF() {
            // https://github.com/jhy/jsoup/issues/349 would crash, as character reader would try to seek past EOF
            String h = "<![CDATA[]]";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(1, doc.Body().ChildNodeSize());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesInvalidStartTags() {
            String h = "<div>Hello < There <&amp;></div>";
            // parse to <div {#text=Hello < There <&>}>
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("Hello < There <&>", doc.Select("div").First().Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnknownTags() {
            String h = "<div><foo title=bar>Hello<foo title=qux>there</foo></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            Elements foos = doc.Select("foo");
            NUnit.Framework.Assert.AreEqual(2, foos.Count);
            NUnit.Framework.Assert.AreEqual("bar", foos.First().Attr("title"));
            NUnit.Framework.Assert.AreEqual("qux", foos.Last().Attr("title"));
            NUnit.Framework.Assert.AreEqual("there", foos.Last().Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnknownInlineTags() {
            String h = "<p><cust>Test</cust></p><p><cust><cust>Test</cust></cust></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.ParseBodyFragment(h);
            String @out = doc.Body().Html();
            NUnit.Framework.Assert.AreEqual(h, TextUtil.StripNewlines(@out));
        }

        [NUnit.Framework.Test]
        public virtual void ParsesBodyFragment() {
            String h = "<!-- comment --><p><a href='foo'>One</a></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.ParseBodyFragment(h, "http://example.com");
            NUnit.Framework.Assert.AreEqual("<body><!-- comment --><p><a href=\"foo\">One</a></p></body>", TextUtil.StripNewlines
                (doc.Body().OuterHtml()));
            NUnit.Framework.Assert.AreEqual("http://example.com/foo", doc.Select("a").First().AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnknownNamespaceTags() {
            // note that the first foo:bar should not really be allowed to be self closing, if parsed in html mode.
            String h = "<foo:bar id='1' /><abc:def id=2>Foo<p>Hello</p></abc:def><foo:bar>There</foo:bar>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<foo:bar id=\"1\" /><abc:def id=\"2\">Foo<p>Hello</p></abc:def><foo:bar>There</foo:bar>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesKnownEmptyBlocks() {
            // if a known tag, allow self closing outside of spec, but force an end tag. unknown tags can be self closing.
            String h = "<div id='1' /><script src='/foo' /><div id=2><img /><img></div><a id=3 /><i /><foo /><foo>One</foo> <hr /> hr text <hr> hr text two";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<div id=\"1\"></div><script src=\"/foo\"></script><div id=\"2\"><img><img></div><a id=\"3\"></a><i></i><foo /><foo>One</foo> <hr> hr text <hr> hr text two"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesSolidusAtAttributeEnd() {
            // this test makes sure [<a href=/>link</a>] is parsed as [<a href="/">link</a>], not [<a href="" /><a>link</a>]
            String h = "<a href=/>link</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<a href=\"/\">link</a>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesMultiClosingBody() {
            String h = "<body><p>Hello</body><p>there</p></body></body></html><p>now";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(3, doc.Select("p").Count);
            NUnit.Framework.Assert.AreEqual(3, doc.Body().Children().Count);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedDefinitionLists() {
            // jsoup used to create a <dl>, but that's not to spec
            String h = "<dt>Foo<dd>Bar<dt>Qux<dd>Zug";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(0, doc.Select("dl").Count);
            // no auto dl
            NUnit.Framework.Assert.AreEqual(4, doc.Select("dt, dd").Count);
            Elements dts = doc.Select("dt");
            NUnit.Framework.Assert.AreEqual(2, dts.Count);
            NUnit.Framework.Assert.AreEqual("Zug", dts[1].NextElementSibling().Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesBlocksInDefinitions() {
            // per the spec, dt and dd are inline, but in practise are block
            String h = "<dl><dt><div id=1>Term</div></dt><dd><div id=2>Def</div></dd></dl>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("dt", ((iText.StyledXmlParser.Jsoup.Nodes.Element)doc.Select("#1").First()
                .Parent()).TagName());
            NUnit.Framework.Assert.AreEqual("dd", ((iText.StyledXmlParser.Jsoup.Nodes.Element)doc.Select("#2").First()
                .Parent()).TagName());
            NUnit.Framework.Assert.AreEqual("<dl><dt><div id=\"1\">Term</div></dt><dd><div id=\"2\">Def</div></dd></dl>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesFrames() {
            String h = "<html><head><script></script><noscript></noscript></head><frameset><frame src=foo></frame><frame src=foo></frameset></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<html><head><script></script><noscript></noscript></head><frameset><frame src=\"foo\"><frame src=\"foo\"></frameset></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        // no body auto vivification
        [NUnit.Framework.Test]
        public virtual void IgnoresContentAfterFrameset() {
            String h = "<html><head><title>One</title></head><frameset><frame /><frame /></frameset><table></table></html>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<html><head><title>One</title></head><frameset><frame><frame></frameset></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        // no body, no table. No crash!
        [NUnit.Framework.Test]
        public virtual void HandlesJavadocFont() {
            String h = "<TD BGCOLOR=\"#EEEEFF\" CLASS=\"NavBarCell1\">    <A HREF=\"deprecated-list.html\"><FONT CLASS=\"NavBarFont1\"><B>Deprecated</B></FONT></A>&nbsp;</TD>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("Deprecated", a.Text());
            NUnit.Framework.Assert.AreEqual("font", a.Child(0).TagName());
            NUnit.Framework.Assert.AreEqual("b", a.Child(0).Child(0).TagName());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesBaseWithoutHref() {
            String h = "<head><base target='_blank'></head><body><a href=/foo>Test</a></body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h, "http://example.com/");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("/foo", a.Attr("href"));
            NUnit.Framework.Assert.AreEqual("http://example.com/foo", a.Attr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void NormalisesDocument() {
            String h = "<!doctype html>One<html>Two<head>Three<link></head>Four<body>Five </body>Six </html>Seven ";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<!doctype html><html><head></head><body>OneTwoThree<link>FourFive Six Seven </body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void NormalisesEmptyDocument() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("");
            NUnit.Framework.Assert.AreEqual("<html><head></head><body></body></html>", TextUtil.StripNewlines(doc.Html
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void NormalisesHeadlessBody() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><body><span class=\"foo\">bar</span>");
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><span class=\"foo\">bar</span></body></html>", TextUtil
                .StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void NormalisedBodyAfterContent() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<font face=Arial><body class=name><div>One</div></body></font>"
                );
            NUnit.Framework.Assert.AreEqual("<html><head></head><body class=\"name\"><font face=\"Arial\"><div>One</div></font></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void FindsCharsetInMalformedMeta() {
            String h = "<meta http-equiv=Content-Type content=text/html; charset=gb2312>";
            // example cited for reason of html5's <meta charset> element
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("gb2312", doc.Select("meta").Attr("charset"));
        }

        [NUnit.Framework.Test]
        public virtual void TestHgroup() {
            // jsoup used to not allow hroup in h{n}, but that's not in spec, and browsers are OK
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<h1>Hello <h2>There <hgroup><h1>Another<h2>headline</hgroup> <hgroup><h1>More</h1><p>stuff</p></hgroup>"
                );
            NUnit.Framework.Assert.AreEqual("<h1>Hello </h1><h2>There <hgroup><h1>Another</h1><h2>headline</h2></hgroup> <hgroup><h1>More</h1><p>stuff</p></hgroup></h2>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestRelaxedTags() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<abc_def id=1>Hello</abc_def> <abc-def>There</abc-def>"
                );
            NUnit.Framework.Assert.AreEqual("<abc_def id=\"1\">Hello</abc_def> <abc-def>There</abc-def>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestHeaderContents() {
            // h* tags (h1 .. h9) in browsers can handle any internal content other than other h*. which is not per any
            // spec, which defines them as containing phrasing content only. so, reality over theory.
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<h1>Hello <div>There</div> now</h1> <h2>More <h3>Content</h3></h2>"
                );
            NUnit.Framework.Assert.AreEqual("<h1>Hello <div>There</div> now</h1> <h2>More </h2><h3>Content</h3>", TextUtil
                .StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpanContents() {
            // like h1 tags, the spec says SPAN is phrasing only, but browsers and publisher treat span as a block tag
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<span>Hello <div>there</div> <span>now</span></span>"
                );
            NUnit.Framework.Assert.AreEqual("<span>Hello <div>there</div> <span>now</span></span>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestNoImagesInNoScriptInHead() {
            // jsoup used to allow, but against spec if parsing with noscript
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<html><head><noscript><img src='foo'></noscript></head><body><p>Hello</p></body></html>"
                );
            NUnit.Framework.Assert.AreEqual("<html><head><noscript>&lt;img src=\"foo\"&gt;</noscript></head><body><p>Hello</p></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestAFlowContents() {
            // html5 has <a> as either phrasing or block
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a>Hello <div>there</div> <span>now</span></a>");
            NUnit.Framework.Assert.AreEqual("<a>Hello <div>there</div> <span>now</span></a>", TextUtil.StripNewlines(doc
                .Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestFontFlowContents() {
            // html5 has no definition of <font>; often used as flow
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<font>Hello <div>there</div> <span>now</span></font>"
                );
            NUnit.Framework.Assert.AreEqual("<font>Hello <div>there</div> <span>now</span></font>", TextUtil.StripNewlines
                (doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesMisnestedTagsBI() {
            // whatwg: <b><i></b></i>
            String h = "<p>1<b>2<i>3</b>4</i>5</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<p>1<b>2<i>3</i></b><i>4</i>5</p>", doc.Body().Html());
        }

        // adoption agency on </b>, reconstruction of formatters on 4.
        [NUnit.Framework.Test]
        public virtual void HandlesMisnestedTagsBP() {
            //  whatwg: <b><p></b></p>
            String h = "<b>1<p>2</b>3</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<b>1</b>\n<p><b>2</b>3</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnexpectedMarkupInTables() {
            // whatwg - tests markers in active formatting (if they didn't work, would get in in table)
            // also tests foster parenting
            String h = "<table><b><tr><td>aaa</td></tr>bbb</table>ccc";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<b></b><b>bbb</b><table><tbody><tr><td>aaa</td></tr></tbody></table><b>ccc</b>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedFormattingElements() {
            // whatwg: formatting elements get collected and applied, but excess elements are thrown away
            String h = "<!DOCTYPE html>\n" + "<p><b class=x><b class=x><b><b class=x><b class=x><b>X\n" + "<p>X\n" + "<p><b><b class=x><b>X\n"
                 + "<p></b></b></b></b></b></b>X";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            doc.OutputSettings().IndentAmount(0);
            String want = "<!doctype html>\n" + "<html>\n" + "<head></head>\n" + "<body>\n" + "<p><b class=\"x\"><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b>X </b></b></b></b></b></b></p>\n"
                 + "<p><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b>X </b></b></b></b></b></p>\n" + "<p><b class=\"x\"><b><b class=\"x\"><b class=\"x\"><b><b><b class=\"x\"><b>X </b></b></b></b></b></b></b></b></p>\n"
                 + "<p>X</p>\n" + "</body>\n" + "</html>";
            NUnit.Framework.Assert.AreEqual(want, doc.Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedAnchors() {
            String h = "<a href='http://example.com/'>Link<p>Error link</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            String want = "<a href=\"http://example.com/\">Link</a>\n<p><a href=\"http://example.com/\">Error link</a></p>";
            NUnit.Framework.Assert.AreEqual(want, doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void ReconstructFormattingElements() {
            // tests attributes and multi b
            String h = "<p><b class=one>One <i>Two <b>Three</p><p>Hello</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<p><b class=\"one\">One <i>Two <b>Three</b></i></b></p>\n<p><b class=\"one\"><i><b>Hello</b></i></b></p>"
                , doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void ReconstructFormattingElementsInTable() {
            // tests that tables get formatting markers -- the <b> applies outside the table and does not leak in,
            // and the <i> inside the table and does not leak out.
            String h = "<p><b>One</p> <table><tr><td><p><i>Three<p>Four</i></td></tr></table> <p>Five</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            String want = "<p><b>One</b></p>\n" + "<b> \n" + " <table>\n" + "  <tbody>\n" + "   <tr>\n" + "    <td><p><i>Three</i></p><p><i>Four</i></p></td>\n"
                 + "   </tr>\n" + "  </tbody>\n" + " </table> <p>Five</p></b>";
            NUnit.Framework.Assert.AreEqual(want, doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void CommentBeforeHtml() {
            String h = "<!-- comment --><!-- comment 2 --><p>One</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<!-- comment --><!-- comment 2 --><html><head></head><body><p>One</p></body></html>"
                , TextUtil.StripNewlines(doc.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTdTag() {
            String h = "<table><tr><td>One</td><td id='2' /></tr></table>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<td>One</td>\n<td id=\"2\"></td>", doc.Select("tr").First().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesSolidusInA() {
            // test for bug #66
            String h = "<a class=lp href=/lib/14160711/>link text</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("link text", a.Text());
            NUnit.Framework.Assert.AreEqual("/lib/14160711/", a.Attr("href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesSpanInTbody() {
            // test for bug 64
            String h = "<table><tbody><span class='1'><tr><td>One</td></tr><tr><td>Two</td></tr></span></tbody></table>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(doc.Select("span").First().Children().Count, 0);
            // the span gets closed
            NUnit.Framework.Assert.AreEqual(doc.Select("table").Count, 1);
        }

        // only one table
        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedTitleAtEof() {
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data").Title());
            NUnit.Framework.Assert.AreEqual("Data<", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data<").Title());
            NUnit.Framework.Assert.AreEqual("Data</", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data</").Title()
                );
            NUnit.Framework.Assert.AreEqual("Data</t", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data</t").Title
                ());
            NUnit.Framework.Assert.AreEqual("Data</ti", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data</ti").Title
                ());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data</title>").Title
                ());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>Data</title >").Title
                ());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedTitle() {
            Document one = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>One <b>Two <b>Three</TITLE><p>Test</p>");
            // has title, so <b> is plain text
            NUnit.Framework.Assert.AreEqual("One <b>Two <b>Three", one.Title());
            NUnit.Framework.Assert.AreEqual("Test", one.Select("p").First().Text());
            Document two = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<title>One<b>Two <p>Test</p>");
            // no title, so <b> causes </title> breakout
            NUnit.Framework.Assert.AreEqual("One", two.Title());
            NUnit.Framework.Assert.AreEqual("<b>Two <p>Test</p></b>", two.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedScriptAtEof() {
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data").Select("script"
                ).First().Data());
            NUnit.Framework.Assert.AreEqual("Data<", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data<").Select("script"
                ).First().Data());
            NUnit.Framework.Assert.AreEqual("Data</sc", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</sc").Select
                ("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</-sc", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</-sc").
                Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</sc-", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</sc-").
                Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</sc--", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</sc--"
                ).Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script>").Select
                ("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</script", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script"
                ).Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script ").Select
                ("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script n").
                Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script n=")
                .Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script n=\""
                ).Select("script").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Data</script n=\"p"
                ).Select("script").First().Data());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesUnclosedRawtextAtEof() {
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data").Select("style"
                ).First().Data());
            NUnit.Framework.Assert.AreEqual("Data</st", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</st").Select
                ("style").First().Data());
            NUnit.Framework.Assert.AreEqual("Data", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</style>").Select
                ("style").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</style", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</style"
                ).Select("style").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</-style", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</-style"
                ).Select("style").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</style-", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</style-"
                ).Select("style").First().Data());
            NUnit.Framework.Assert.AreEqual("Data</style--", iText.StyledXmlParser.Jsoup.Jsoup.Parse("<style>Data</style--"
                ).Select("style").First().Data());
        }

        [NUnit.Framework.Test]
        public virtual void NoImplicitFormForTextAreas() {
            // old jsoup parser would create implicit forms for form children like <textarea>, but no more
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<textarea>One</textarea>");
            NUnit.Framework.Assert.AreEqual("<textarea>One</textarea>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesEscapedScript() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script><!-- one <script>Blah</script> --></script>"
                );
            NUnit.Framework.Assert.AreEqual("<!-- one <script>Blah</script> -->", doc.Select("script").First().Data());
        }

        [NUnit.Framework.Test]
        public virtual void Handles0CharacterAsText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("0<p>0</p>");
            NUnit.Framework.Assert.AreEqual("0\n<p>0</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNullInData() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p id=\u0000>Blah \u0000</p>");
            NUnit.Framework.Assert.AreEqual("<p id=\"\uFFFD\">Blah \u0000</p>", doc.Body().Html());
        }

        // replaced in attr, NOT replaced in data
        [NUnit.Framework.Test]
        public virtual void HandlesNullInComments() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body><!-- \u0000 \u0000 -->");
            NUnit.Framework.Assert.AreEqual("<!-- \uFFFD \uFFFD -->", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNewlinesAndWhitespaceInTag() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a \n href=\"one\" \r\n id=\"two\" \f >");
            NUnit.Framework.Assert.AreEqual("<a href=\"one\" id=\"two\"></a>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesWhitespaceInoDocType() {
            String html = "<!DOCTYPE html\r\n" + "      PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n" + "      \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">"
                , doc.ChildNode(0).OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void TracksErrorsWhenRequested() {
            String html = "<p>One</p href='no'><!DOCTYPE html>&arrgh;<font /><br /><foo";
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().
                SetTrackErrors(500);
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "http://example.com", parser);
            IList<ParseError> errors = parser.GetErrors();
            NUnit.Framework.Assert.AreEqual(5, errors.Count);
            NUnit.Framework.Assert.AreEqual("20: Attributes incorrectly present on end tag", errors[0].ToString());
            NUnit.Framework.Assert.AreEqual("35: Unexpected token [Doctype] when in state [InBody]", errors[1].ToString
                ());
            NUnit.Framework.Assert.AreEqual("36: Invalid character reference: invalid named referenece 'arrgh'", errors
                [2].ToString());
            NUnit.Framework.Assert.AreEqual("50: Self closing flag not acknowledged", errors[3].ToString());
            NUnit.Framework.Assert.AreEqual("61: Unexpectedly reached end of file (EOF) in input state [TagName]", errors
                [4].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TracksLimitedErrorsWhenRequested() {
            String html = "<p>One</p href='no'><!DOCTYPE html>&arrgh;<font /><br /><foo";
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().
                SetTrackErrors(3);
            Document doc = parser.ParseInput(html, "http://example.com");
            IList<ParseError> errors = parser.GetErrors();
            NUnit.Framework.Assert.AreEqual(3, errors.Count);
            NUnit.Framework.Assert.AreEqual("20: Attributes incorrectly present on end tag", errors[0].ToString());
            NUnit.Framework.Assert.AreEqual("35: Unexpected token [Doctype] when in state [InBody]", errors[1].ToString
                ());
            NUnit.Framework.Assert.AreEqual("36: Invalid character reference: invalid named referenece 'arrgh'", errors
                [2].ToString());
        }

        [NUnit.Framework.Test]
        public virtual void NoErrorsByDefault() {
            String html = "<p>One</p href='no'>&arrgh;<font /><br /><foo";
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser();
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html, "http://example.com", parser);
            IList<ParseError> errors = parser.GetErrors();
            NUnit.Framework.Assert.AreEqual(0, errors.Count);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCommentsInTable() {
            String html = "<table><tr><td>text</td><!-- Comment --></tr></table>";
            Document node = iText.StyledXmlParser.Jsoup.Jsoup.ParseBodyFragment(html);
            NUnit.Framework.Assert.AreEqual("<html><head></head><body><table><tbody><tr><td>text</td><!-- Comment --></tr></tbody></table></body></html>"
                , TextUtil.StripNewlines(node.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesQuotesInCommentsInScripts() {
            String html = "<script>\n" + "  <!--\n" + "    document.write('</scr' + 'ipt>');\n" + "  // -->\n" + "</script>";
            Document node = iText.StyledXmlParser.Jsoup.Jsoup.ParseBodyFragment(html);
            NUnit.Framework.Assert.AreEqual("<script>\n" + "  <!--\n" + "    document.write('</scr' + 'ipt>');\n" + "  // -->\n"
                 + "</script>", node.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandleNullContextInParseFragment() {
            String html = "<ol><li>One</li></ol><p>Two</p>";
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment
                (html, null, "http://example.com/");
            NUnit.Framework.Assert.AreEqual(1, nodes.Count);
            // returns <html> node (not document) -- no context means doc gets created
            NUnit.Framework.Assert.AreEqual("html", nodes[0].NodeName());
            NUnit.Framework.Assert.AreEqual("<html> <head></head> <body> <ol> <li>One</li> </ol> <p>Two</p> </body> </html>"
                , iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace(nodes[0].OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void DoesNotFindShortestMatchingEntity() {
            // previous behaviour was to identify a possible entity, then chomp down the string until a match was found.
            // (as defined in html5.) However in practise that lead to spurious matches against the author's intent.
            String html = "One &clubsuite; &clubsuit;";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace("One &amp;clubsuite; ♣"
                ), doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void RelaxedBaseEntityMatchAndStrictExtendedMatch() {
            // extended entities need a ; at the end to match, base does not
            String html = "&amp &quot &reg &icy &hopf &icy; &hopf;";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.extended).Charset("ascii");
            // modifies output only to clarify test
            NUnit.Framework.Assert.AreEqual("&amp; \" &reg; &amp;icy &amp;hopf &icy; &hopf;", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesXmlDeclarationAsBogusComment() {
            String html = "<?xml encoding='UTF-8' ?><body>One</body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<!--?xml encoding='UTF-8' ?--> <html> <head></head> <body> One </body> </html>"
                , iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace(doc.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesTagsInTextarea() {
            String html = "<textarea><p>Jsoup</p></textarea>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NUnit.Framework.Assert.AreEqual("<textarea>&lt;p&gt;Jsoup&lt;/p&gt;</textarea>", doc.Body().Html());
        }

        // form tests
        [NUnit.Framework.Test]
        public virtual void CreatesFormElements() {
            String html = "<body><form><input id=1><input id=2></form></body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("form").First();
            NUnit.Framework.Assert.IsTrue(el is FormElement, "Is form element");
            FormElement form = (FormElement)el;
            Elements controls = form.Elements();
            NUnit.Framework.Assert.AreEqual(2, controls.Count);
            NUnit.Framework.Assert.AreEqual("1", controls[0].Id());
            NUnit.Framework.Assert.AreEqual("2", controls[1].Id());
        }

        [NUnit.Framework.Test]
        public virtual void AssociatedFormControlsWithDisjointForms() {
            // form gets closed, isn't parent of controls
            String html = "<table><tr><form><input type=hidden id=1><td><input type=text id=2></td><tr></table>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("form").First();
            NUnit.Framework.Assert.IsTrue(el is FormElement, "Is form element");
            FormElement form = (FormElement)el;
            Elements controls = form.Elements();
            NUnit.Framework.Assert.AreEqual(2, controls.Count);
            NUnit.Framework.Assert.AreEqual("1", controls[0].Id());
            NUnit.Framework.Assert.AreEqual("2", controls[1].Id());
            NUnit.Framework.Assert.AreEqual("<table><tbody><tr><form></form><input type=\"hidden\" id=\"1\"><td><input type=\"text\" id=\"2\"></td></tr><tr></tr></tbody></table>"
                , TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesInputInTable() {
            String h = "<body>\n" + "<input type=\"hidden\" name=\"a\" value=\"\">\n" + "<table>\n" + "<input type=\"hidden\" name=\"b\" value=\"\" />\n"
                 + "</table>\n" + "</body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual(1, doc.Select("table input").Count);
            NUnit.Framework.Assert.AreEqual(2, doc.Select("input").Count);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertsImageToImg() {
            // image to img, unless in a svg. old html cruft.
            String h = "<body><image><svg><image /></svg></body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(h);
            NUnit.Framework.Assert.AreEqual("<img>\n<svg>\n <image />\n</svg>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesInvalidDoctypes() {
            // would previously throw invalid name exception on empty doctype
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!DOCTYPE>");
            NUnit.Framework.Assert.AreEqual("<!doctype> <html> <head></head> <body></body> </html>", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(doc.OuterHtml()));
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!DOCTYPE><html><p>Foo</p></html>");
            NUnit.Framework.Assert.AreEqual("<!doctype> <html> <head></head> <body> <p>Foo</p> </body> </html>", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(doc.OuterHtml()));
            doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<!DOCTYPE \u0000>");
            NUnit.Framework.Assert.AreEqual("<!doctype \ufffd> <html> <head></head> <body></body> </html>", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(doc.OuterHtml()));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesManyChildren() {
            // Arrange
            StringBuilder longBody = new StringBuilder(500000);
            for (int i = 0; i < 25000; i++) {
                longBody.Append(i).Append("<br>");
            }
            // Act
            long start = SystemUtil.GetRelativeTimeMillis();
            Document doc = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseBodyFragment(longBody.ToString(), "");
            // Assert
            NUnit.Framework.Assert.AreEqual(50000, doc.Body().ChildNodeSize());
            NUnit.Framework.Assert.IsTrue(SystemUtil.GetRelativeTimeMillis() - start < 1000);
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidTableContents() {
            FileInfo @in = iText.StyledXmlParser.Jsoup.PortTestUtil.GetFile("/htmltests/table-invalid-elements.html");
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(@in, "UTF-8");
            doc.OutputSettings().PrettyPrint(true);
            String rendered = doc.ToString();
            int endOfEmail = rendered.IndexOf("Comment", StringComparison.Ordinal);
            int guarantee = rendered.IndexOf("Why am I here?", StringComparison.Ordinal);
            NUnit.Framework.Assert.IsTrue(endOfEmail > -1, "Comment not found");
            NUnit.Framework.Assert.IsTrue(guarantee > -1, "Search text not found");
            NUnit.Framework.Assert.IsTrue(guarantee > endOfEmail, "Search text did not come after comment");
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalisesIsIndex() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<body><isindex action='/submit'></body>");
            String html = doc.OuterHtml();
            NUnit.Framework.Assert.AreEqual("<form action=\"/submit\"> <hr> <label>This is a searchable index. Enter search keywords: <input name=\"isindex\"></label> <hr> </form>"
                , iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestReinsertionModeForThCelss() {
            String body = "<body> <table> <tr> <th> <table><tr><td></td></tr></table> <div> <table><tr><td></td></tr></table> </div> <div></div> <div></div> <div></div> </th> </tr> </table> </body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            NUnit.Framework.Assert.AreEqual(1, doc.Body().Children().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestUsingSingleQuotesInQueries() {
            String body = "<body> <div class='main'>hello</div></body>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            Elements main = doc.Select("div[class='main']");
            NUnit.Framework.Assert.AreEqual("hello", main.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestSupportsNonAsciiTags() {
            String body = "<進捗推移グラフ>Yes</進捗推移グラフ><русский-тэг>Correct</<русский-тэг>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            Elements els = doc.Select("進捗推移グラフ");
            NUnit.Framework.Assert.AreEqual("Yes", els.Text());
            els = doc.Select("русский-тэг");
            NUnit.Framework.Assert.AreEqual("Correct", els.Text());
        }

        [NUnit.Framework.Test]
        public virtual void TestSupportsPartiallyNonAsciiTags() {
            String body = "<div>Check</divá>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            Elements els = doc.Select("div");
            NUnit.Framework.Assert.AreEqual("Check", els.Text());
        }
    }
}
