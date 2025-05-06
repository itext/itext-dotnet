/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    [NUnit.Framework.Category("UnitTest")]
    public class LeafNodeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DoesNotGetAttributesTooEasily() {
            // test to make sure we're not setting attributes on all nodes right away
            String body = "<p>One <!-- Two --> Three<![CDATA[Four]]></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(body);
            NUnit.Framework.Assert.IsTrue(HasAnyAttributes(doc));
            // should have one - the base uri on the doc
            iText.StyledXmlParser.Jsoup.Nodes.Element html = doc.Child(0);
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            String s = doc.OuterHtml();
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            Elements els = doc.Select("p");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = els.First();
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            els = doc.Select("p.none");
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            String id = p.Id();
            NUnit.Framework.Assert.AreEqual("", id);
            NUnit.Framework.Assert.IsFalse(p.HasClass("Foobs"));
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            p.AddClass("Foobs");
            NUnit.Framework.Assert.IsTrue(p.HasClass("Foobs"));
            NUnit.Framework.Assert.IsTrue(HasAnyAttributes(html));
            NUnit.Framework.Assert.IsTrue(HasAnyAttributes(p));
            Attributes attributes = p.Attributes();
            NUnit.Framework.Assert.IsTrue(attributes.HasKey("class"));
            p.ClearAttributes();
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(p));
            NUnit.Framework.Assert.IsFalse(HasAnyAttributes(html));
            NUnit.Framework.Assert.IsFalse(attributes.HasKey("class"));
        }

        private bool HasAnyAttributes(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            bool[] found = new bool[1];
            node.Filter(new _NodeFilter_59(found));
            return found[0];
        }

        private sealed class _NodeFilter_59 : NodeFilter {
            public _NodeFilter_59(bool[] found) {
                this.found = found;
            }

            public override NodeFilter.FilterResult Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node.HasAttributes()) {
                    found[0] = true;
                    return NodeFilter.FilterResult.STOP;
                }
                else {
                    return NodeFilter.FilterResult.CONTINUE;
                }
            }

            public override NodeFilter.FilterResult Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                return NodeFilter.FilterResult.CONTINUE;
            }

            private readonly bool[] found;
        }
    }
}
