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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests Nodes</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class NodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void HandlesBaseUri() {
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("a");
            Attributes attribs = new Attributes();
            attribs.Put("relHref", "/foo");
            attribs.Put("absHref", "http://bar/qux");
            iText.StyledXmlParser.Jsoup.Nodes.Element noBase = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, "", 
                attribs);
            NUnit.Framework.Assert.AreEqual("", noBase.AbsUrl("relHref"));
            // with no base, should NOT fallback to href attrib, whatever it is
            NUnit.Framework.Assert.AreEqual("http://bar/qux", noBase.AbsUrl("absHref"));
            // no base but valid attrib, return attrib
            iText.StyledXmlParser.Jsoup.Nodes.Element withBase = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, "http://foo/"
                , attribs);
            NUnit.Framework.Assert.AreEqual("http://foo/foo", withBase.AbsUrl("relHref"));
            // construct abs from base + rel
            NUnit.Framework.Assert.AreEqual("http://bar/qux", withBase.AbsUrl("absHref"));
            // href is abs, so returns that
            NUnit.Framework.Assert.AreEqual("", withBase.AbsUrl("noval"));
            iText.StyledXmlParser.Jsoup.Nodes.Element dodgyBase = new iText.StyledXmlParser.Jsoup.Nodes.Element(tag, "fff://no-such-protocol/"
                , attribs);
            NUnit.Framework.Assert.AreEqual("http://bar/qux", dodgyBase.AbsUrl("absHref"));
        }

        // base fails, but href good, so get that
        [NUnit.Framework.Test]
        public virtual void SetBaseUriIsRecursive() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p></p></div>");
            String baseUri = "https://jsoup.org";
            doc.SetBaseUri(baseUri);
            NUnit.Framework.Assert.AreEqual(baseUri, doc.BaseUri());
            NUnit.Framework.Assert.AreEqual(baseUri, doc.Select("div").First().BaseUri());
            NUnit.Framework.Assert.AreEqual(baseUri, doc.Select("p").First().BaseUri());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAbsPrefix() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href=/foo>Hello</a>", "https://jsoup.org/");
            iText.StyledXmlParser.Jsoup.Nodes.Element a = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("/foo", a.Attr("href"));
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/foo", a.Attr("abs:href"));
            NUnit.Framework.Assert.IsTrue(a.HasAttr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAbsOnImage() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p><img src=\"/rez/osi_logo.png\" /></p>", "https://jsoup.org/"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element img = doc.Select("img").First();
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/rez/osi_logo.png", img.Attr("abs:src"));
            NUnit.Framework.Assert.AreEqual(img.AbsUrl("src"), img.Attr("abs:src"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAbsPrefixOnHasAttr() {
            // 1: no abs url; 2: has abs url
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a id=1 href='/foo'>One</a> <a id=2 href='https://jsoup.org/'>Two</a>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element one = doc.Select("#1").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element two = doc.Select("#2").First();
            NUnit.Framework.Assert.IsFalse(one.HasAttr("abs:href"));
            NUnit.Framework.Assert.IsTrue(one.HasAttr("href"));
            NUnit.Framework.Assert.AreEqual("", one.AbsUrl("href"));
            NUnit.Framework.Assert.IsTrue(two.HasAttr("abs:href"));
            NUnit.Framework.Assert.IsTrue(two.HasAttr("href"));
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/", two.AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void LiteralAbsPrefix() {
            // if there is a literal attribute "abs:xxx", don't try and make absolute.
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a abs:href='odd'>One</a>");
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("a").First();
            NUnit.Framework.Assert.IsTrue(el.HasAttr("abs:href"));
            NUnit.Framework.Assert.AreEqual("odd", el.Attr("abs:href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandleAbsOnLocalhostFileUris() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href='password'>One/a><a href='/var/log/messages'>Two</a>"
                , "file://localhost/etc/");
            iText.StyledXmlParser.Jsoup.Nodes.Element one = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("file://localhost/etc/password", one.AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAbsOnProtocolessAbsoluteUris() {
            Document doc1 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href='//example.net/foo'>One</a>", "http://example.com/"
                );
            Document doc2 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href='//example.net/foo'>One</a>", "https://example.com/"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element one = doc1.Select("a").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element two = doc2.Select("a").First();
            NUnit.Framework.Assert.AreEqual("http://example.net/foo", one.AbsUrl("href"));
            NUnit.Framework.Assert.AreEqual("https://example.net/foo", two.AbsUrl("href"));
            Document doc3 = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<img src=//www.google.com/images/errors/logo_sm.gif alt=Google>"
                , "https://google.com");
            NUnit.Framework.Assert.AreEqual("https://www.google.com/images/errors/logo_sm.gif", doc3.Select("img").Attr
                ("abs:src"));
        }

        /*
        Test for an issue with Java's abs URL handler.
        */
        [NUnit.Framework.Test]
        public virtual void AbsHandlesRelativeQuery() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href='?foo'>One</a> <a href='bar.html?foo'>Two</a>"
                , "https://jsoup.org/path/file?bar");
            iText.StyledXmlParser.Jsoup.Nodes.Element a1 = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/path/file?foo", a1.AbsUrl("href"));
            iText.StyledXmlParser.Jsoup.Nodes.Element a2 = doc.Select("a")[1];
            NUnit.Framework.Assert.AreEqual("https://jsoup.org/path/bar.html?foo", a2.AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void AbsHandlesDotFromIndex() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<a href='./one/two.html'>One</a>", "http://example.com"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element a1 = doc.Select("a").First();
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two.html", a1.AbsUrl("href"));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemove() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <span>two</span> three</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            p.ChildNode(0).Remove();
            NUnit.Framework.Assert.AreEqual("two three", p.Text());
            NUnit.Framework.Assert.AreEqual("<span>two</span> three", TextUtil.StripNewlines(p.Html()));
        }

        [NUnit.Framework.Test]
        public virtual void TestReplace() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <span>two</span> three</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element insert = doc.CreateElement("em").Text("foo");
            p.ChildNode(1).ReplaceWith(insert);
            NUnit.Framework.Assert.AreEqual("One <em>foo</em> three", p.Html());
        }

        [NUnit.Framework.Test]
        public virtual void OwnerDocument() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>Hello");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreSame(p.OwnerDocument(), doc);
            NUnit.Framework.Assert.AreSame(doc.OwnerDocument(), doc);
            NUnit.Framework.Assert.IsNull(doc.Parent());
        }

        [NUnit.Framework.Test]
        public virtual void Root() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            iText.StyledXmlParser.Jsoup.Nodes.Node root = p.Root();
            NUnit.Framework.Assert.AreSame(doc, root);
            NUnit.Framework.Assert.IsNull(root.Parent());
            NUnit.Framework.Assert.AreSame(doc.Root(), doc);
            NUnit.Framework.Assert.AreSame(doc.Root(), doc.OwnerDocument());
            iText.StyledXmlParser.Jsoup.Nodes.Element standAlone = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("p"), "");
            NUnit.Framework.Assert.IsNull(standAlone.Parent());
            NUnit.Framework.Assert.AreSame(standAlone.Root(), standAlone);
            NUnit.Framework.Assert.IsNull(standAlone.OwnerDocument());
        }

        [NUnit.Framework.Test]
        public virtual void Before() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <b>two</b> three</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element newNode = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("em"), "");
            newNode.AppendText("four");
            doc.Select("b").First().Before(newNode);
            NUnit.Framework.Assert.AreEqual("<p>One <em>four</em><b>two</b> three</p>", doc.Body().Html());
            doc.Select("b").First().Before("<i>five</i>");
            NUnit.Framework.Assert.AreEqual("<p>One <em>four</em><i>five</i><b>two</b> three</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void After() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>One <b>two</b> three</p>");
            iText.StyledXmlParser.Jsoup.Nodes.Element newNode = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("em"), "");
            newNode.AppendText("four");
            doc.Select("b").First().After(newNode);
            NUnit.Framework.Assert.AreEqual("<p>One <b>two</b><em>four</em> three</p>", doc.Body().Html());
            doc.Select("b").First().After("<i>five</i>");
            NUnit.Framework.Assert.AreEqual("<p>One <b>two</b><i>five</i><em>four</em> three</p>", doc.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void Unwrap() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One <span>Two <b>Three</b></span> Four</div>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element span = doc.Select("span").First();
            iText.StyledXmlParser.Jsoup.Nodes.Node twoText = span.ChildNode(0);
            iText.StyledXmlParser.Jsoup.Nodes.Node node = span.Unwrap();
            NUnit.Framework.Assert.AreEqual("<div>One Two <b>Three</b> Four</div>", TextUtil.StripNewlines(doc.Body().
                Html()));
            NUnit.Framework.Assert.IsTrue(node is TextNode);
            NUnit.Framework.Assert.AreEqual("Two ", ((TextNode)node).Text());
            NUnit.Framework.Assert.AreEqual(node, twoText);
            NUnit.Framework.Assert.AreEqual(node.Parent(), doc.Select("div").First());
        }

        [NUnit.Framework.Test]
        public virtual void UnwrapNoChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div>One <span></span> Two</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element span = doc.Select("span").First();
            iText.StyledXmlParser.Jsoup.Nodes.Node node = span.Unwrap();
            NUnit.Framework.Assert.AreEqual("<div>One  Two</div>", TextUtil.StripNewlines(doc.Body().Html()));
            NUnit.Framework.Assert.IsNull(node);
        }

        [NUnit.Framework.Test]
        public virtual void Traverse() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            doc.Select("div").First().Traverse(new _NodeVisitor_289(accum));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p></div>", accum.ToString());
        }

        private sealed class _NodeVisitor_289 : NodeVisitor {
            public _NodeVisitor_289(StringBuilder accum) {
                this.accum = accum;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<").Append(node.NodeName()).Append(">");
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void OrphanNodeReturnsNullForSiblingElements() {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("p"), "");
            iText.StyledXmlParser.Jsoup.Nodes.Element el = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("p"), "");
            NUnit.Framework.Assert.AreEqual(0, node.SiblingIndex());
            NUnit.Framework.Assert.AreEqual(0, node.SiblingNodes().Count);
            NUnit.Framework.Assert.IsNull(node.PreviousSibling());
            NUnit.Framework.Assert.IsNull(node.NextSibling());
            NUnit.Framework.Assert.AreEqual(0, el.SiblingElements().Count);
            NUnit.Framework.Assert.IsNull(el.PreviousElementSibling());
            NUnit.Framework.Assert.IsNull(el.NextElementSibling());
        }

        [NUnit.Framework.Test]
        public virtual void NodeIsNotASiblingOfItself() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>One<p>Two<p>Three</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element p2 = doc.Select("p")[1];
            NUnit.Framework.Assert.AreEqual("Two", p2.Text());
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = p2.SiblingNodes();
            NUnit.Framework.Assert.AreEqual(2, nodes.Count);
            NUnit.Framework.Assert.AreEqual("<p>One</p>", nodes[0].OuterHtml());
            NUnit.Framework.Assert.AreEqual("<p>Three</p>", nodes[1].OuterHtml());
        }

        [NUnit.Framework.Test]
        public virtual void ChildNodesCopy() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1>Text 1 <p>One</p> Text 2 <p>Two<p>Three</div><div id=2>"
                );
            iText.StyledXmlParser.Jsoup.Nodes.Element div1 = doc.Select("#1").First();
            iText.StyledXmlParser.Jsoup.Nodes.Element div2 = doc.Select("#2").First();
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> divChildren = div1.ChildNodesCopy();
            NUnit.Framework.Assert.AreEqual(5, divChildren.Count);
            TextNode tn1 = (TextNode)div1.ChildNode(0);
            TextNode tn2 = (TextNode)divChildren[0];
            tn2.Text("Text 1 updated");
            NUnit.Framework.Assert.AreEqual("Text 1 ", tn1.Text());
            div2.InsertChildren(-1, divChildren);
            NUnit.Framework.Assert.AreEqual("<div id=\"1\">Text 1 <p>One</p> Text 2 <p>Two</p><p>Three</p></div><div id=\"2\">Text 1 updated"
                 + "<p>One</p> Text 2 <p>Two</p><p>Three</p></div>", TextUtil.StripNewlines(doc.Body().Html()));
        }

        [NUnit.Framework.Test]
        public virtual void SupportsClone() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div class=foo>Text</div>");
            iText.StyledXmlParser.Jsoup.Nodes.Element el = doc.Select("div").First();
            NUnit.Framework.Assert.IsTrue(el.HasClass("foo"));
            iText.StyledXmlParser.Jsoup.Nodes.Element elClone = ((Document)doc.Clone()).Select("div").First();
            NUnit.Framework.Assert.IsTrue(elClone.HasClass("foo"));
            NUnit.Framework.Assert.AreEqual("Text", elClone.Text());
            el.RemoveClass("foo");
            el.Text("None");
            NUnit.Framework.Assert.IsFalse(el.HasClass("foo"));
            NUnit.Framework.Assert.IsTrue(elClone.HasClass("foo"));
            NUnit.Framework.Assert.AreEqual("None", el.Text());
            NUnit.Framework.Assert.AreEqual("Text", elClone.Text());
        }

        [NUnit.Framework.Test]
        public virtual void ChangingAttributeValueShouldReplaceExistingAttributeCaseInsensitive() {
            Document document = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<INPUT id=\"foo\" NAME=\"foo\" VALUE=\"\">");
            iText.StyledXmlParser.Jsoup.Nodes.Element inputElement = document.Select("#foo").First();
            inputElement.Attr("value", "bar");
            NUnit.Framework.Assert.AreEqual(SingletonAttributes(), GetAttributesCaseInsensitive(inputElement));
        }

        private Attributes GetAttributesCaseInsensitive(iText.StyledXmlParser.Jsoup.Nodes.Element element) {
            Attributes matches = new Attributes();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in element.Attributes()) {
                if (attribute.Key.EqualsIgnoreCase("value")) {
                    matches.Put(attribute);
                }
            }
            return matches;
        }

        private Attributes SingletonAttributes() {
            Attributes attributes = new Attributes();
            attributes.Put("value", "bar");
            return attributes;
        }

        private static String CreateAnotherValidUrlVersion(String url) {
            if (url.StartsWith("file:///")) {
                return "file:/" + url.Substring("file:///".Length);
            }
            else {
                if (url.StartsWith("file:/")) {
                    return "file:///" + url.Substring("file:/".Length);
                }
                else {
                    return url;
                }
            }
        }
    }
}
