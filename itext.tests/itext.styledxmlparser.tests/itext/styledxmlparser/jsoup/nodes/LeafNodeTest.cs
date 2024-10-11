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
            node.Filter(new _NodeFilter_77(found));
            return found[0];
        }

        private sealed class _NodeFilter_77 : NodeFilter {
            public _NodeFilter_77(bool[] found) {
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
