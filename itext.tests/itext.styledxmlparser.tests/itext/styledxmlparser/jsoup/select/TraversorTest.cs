/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
            NodeTraversor.Filter(new _NodeFilter_25(accum), doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p></div><div><#text></#text></div>", accum.ToString
                ());
        }

        private sealed class _NodeFilter_25 : NodeFilter {
            public _NodeFilter_25(StringBuilder accum) {
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
            NodeTraversor.Filter(new _NodeFilter_45(accum), 
                        // OMIT contents of p:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p></p></div><div><#text></#text></div>", accum.ToString());
        }

        private sealed class _NodeFilter_45 : NodeFilter {
            public _NodeFilter_45(StringBuilder accum) {
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
            NodeTraversor.Filter(new _NodeFilter_66(accum), 
                        // OMIT p:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div></div><div><#text></#text></div>", accum.ToString());
        }

        private sealed class _NodeFilter_66 : NodeFilter {
            public _NodeFilter_66(StringBuilder accum) {
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
            NodeTraversor.Filter(new _NodeFilter_88(), 
                        // Delete "p" in head:
                        
                        // Delete "b" in tail:
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div></div>\n<div>\n There be \n</div>", doc.Select("body").Html());
        }

        private sealed class _NodeFilter_88 : NodeFilter {
            public _NodeFilter_88() {
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
            NodeTraversor.Filter(new _NodeFilter_108(accum), 
                        // Stop after p.
                        doc.Select("div"));
            NUnit.Framework.Assert.AreEqual("<div><p><#text></#text></p>", accum.ToString());
        }

        private sealed class _NodeFilter_108 : NodeFilter {
            public _NodeFilter_108(StringBuilder accum) {
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
            NodeTraversor.Traverse(new _NodeVisitor_131(), doc);
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.SelectFirst("p");
            NUnit.Framework.Assert.IsNotNull(p);
            NUnit.Framework.Assert.AreEqual("<p>One <u>two</u> <u>three</u> four.</p>", p.OuterHtml());
        }

        private sealed class _NodeVisitor_131 : NodeVisitor {
            public _NodeVisitor_131() {
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
            NodeTraversor.Traverse(new _NodeVisitor_155(), doc);
            NUnit.Framework.Assert.AreEqual("<div>\n" + " <p><span>0</span><span>1</span></p>\n" + " <p><span>2</span><span>3</span></p>\n"
                 + "</div>", doc.Body().Html());
        }

        private sealed class _NodeVisitor_155 : NodeVisitor {
            public _NodeVisitor_155() {
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
