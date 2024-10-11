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
using System.Text;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Select {
    [NUnit.Framework.Category("UnitTest")]
    public class TraversorTest : ExtendedITextTest {
        // Note: NodeTraversor.traverse(new NodeVisitor) is tested in
        // ElementsTest#traverse()
        [NUnit.Framework.Test]
        public virtual void FilterVisit() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            NodeTraversor.Filter(new _NodeFilter_43(accum), doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p></div><div><#text></#text></div>", accum.ToString
                ());
        }

        private sealed class _NodeFilter_43 : NodeFilter {
            public _NodeFilter_43(StringBuilder accum) {
                this.accum = accum;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void FilterSkipChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            NodeTraversor.Filter(new _NodeFilter_63(accum), 
                        // OMIT contents of p:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p></p></div><div><#text></#text></div>", accum.ToString());
        }

        private sealed class _NodeFilter_63 : NodeFilter {
            public _NodeFilter_63(StringBuilder accum) {
                this.accum = accum;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<").Append(node.NodeName()).Append(">");
                return ("p".Equals(node.NodeName())) ? NodeFilter.FilterResult.SKIP_CHILDREN : NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void FilterSkipEntirely() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            NodeTraversor.Filter(new _NodeFilter_84(accum), 
                        // OMIT p:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div></div><div><#text></#text></div>", accum.ToString());
        }

        private sealed class _NodeFilter_84 : NodeFilter {
            public _NodeFilter_84(StringBuilder accum) {
                this.accum = accum;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if ("p".Equals(node.NodeName())) {
                    return NodeFilter.FilterResult.SKIP_ENTIRELY;
                }
                accum.Append("<").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void FilterRemove() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There be <b>bold</b></div>"
                );
            NodeTraversor.Filter(new _NodeFilter_106(), 
                        // Delete "p" in head:
                        
                        // Delete "b" in tail:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n There be \n</div>", doc.Select("body").Html());
        }

        private sealed class _NodeFilter_106 : NodeFilter {
            public _NodeFilter_106() {
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return ("p".Equals(node.NodeName())) ? NodeFilter.FilterResult.REMOVE : NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return ("b".Equals(node.NodeName())) ? NodeFilter.FilterResult.REMOVE : NodeFilter.FilterResult.CONTINUE;
            }
        }

        [NUnit.Framework.Test]
        public virtual void FilterStop() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p>Hello</p></div><div>There</div>");
            StringBuilder accum = new StringBuilder();
            NodeTraversor.Filter(new _NodeFilter_126(accum), 
                        // Stop after p.
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p>", accum.ToString());
        }

        private sealed class _NodeFilter_126 : NodeFilter {
            public _NodeFilter_126(StringBuilder accum) {
                this.accum = accum;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("<").Append(node.NodeName()).Append(">");
                return NodeFilter.FilterResult.CONTINUE;
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                accum.Append("</").Append(node.NodeName()).Append(">");
                return ("p".Equals(node.NodeName())) ? NodeFilter.FilterResult.STOP : NodeFilter.FilterResult.CONTINUE;
            }

            private readonly StringBuilder accum;
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceElement() {
            // https://github.com/jhy/jsoup/issues/1289
            // test we can replace an element during traversal
            String html = "<div><p>One <i>two</i> <i>three</i> four.</p></div>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            NodeTraversor.Traverse(new _NodeVisitor_149(), doc);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            NUnit.Framework.Assert.IsNotNull(p);
            NUnit.Framework.Assert.AreEqual("<p>One <u>two</u> <u>three</u> four.</p>", p.OuterHtml());
        }

        private sealed class _NodeVisitor_149 : NodeVisitor {
            public _NodeVisitor_149() {
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element el = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                    if (el.NormalName().Equals("i")) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element u = new iText.StyledXmlParser.Jsoup.Nodes.Element("u").InsertChildren
                            (0, el.ChildNodes());
                        el.ReplaceWith(u);
                    }
                }
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanAddChildren() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div><p></p><p></p></div>");
            NodeTraversor.Traverse(new _NodeVisitor_173(), doc);
            NUnit.Framework.Assert.AreEqual("<div>\n" + " <p><span>0</span><span>1</span></p>\n" + " <p><span>2</span><span>3</span></p>\n"
                 + "</div>", doc.Body().Html());
        }

        private sealed class _NodeVisitor_173 : NodeVisitor {
            public _NodeVisitor_173() {
                this.i = 0;
            }

//\cond DO_NOT_DOCUMENT
            internal int i;
//\endcond

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node.NodeName().Equals("p")) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                    p.Append("<span>" + this.i++ + "</span>");
                }
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node.NodeName().Equals("p")) {
                    iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                    p.Append("<span>" + this.i++ + "</span>");
                }
            }
        }
    }
}
