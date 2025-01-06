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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    public class AttributeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Html() {
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("key", 
                "value &");
            NUnit.Framework.Assert.AreEqual("key=\"value &amp;\"", attr.Html());
            NUnit.Framework.Assert.AreEqual(attr.Html(), attr.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestWithSupplementaryCharacterInAttributeKeyAndValue() {
            String s = new String(iText.IO.Util.TextUtil.ToChars(135361));
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(s, "A" 
                + s + "B");
            NUnit.Framework.Assert.AreEqual(s + "=\"A" + s + "B\"", attr.Html());
            NUnit.Framework.Assert.AreEqual(attr.Html(), attr.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void ValidatesKeysNotEmpty() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new iText.StyledXmlParser.Jsoup.Nodes.Attribute
                (" ", "Check"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidatesKeysNotEmptyViaSet() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("One", 
                    "Check");
                attr.SetKey(" ");
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BooleanAttributesAreEmptyStringValues() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div hidden>");
            Attributes attributes = doc.Body().Child(0).Attributes();
            NUnit.Framework.Assert.AreEqual("", attributes.Get("hidden"));
            IEnumerator<Attribute> enumerator = attributes.GetEnumerator();
            NUnit.Framework.Assert.IsTrue(enumerator.MoveNext());
            iText.StyledXmlParser.Jsoup.Nodes.Attribute first = enumerator.Current;
            NUnit.Framework.Assert.AreEqual("hidden", first.Key);
            NUnit.Framework.Assert.AreEqual("", first.Value);
            NUnit.Framework.Assert.IsFalse(first.HasDeclaredValue());
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Nodes.Attribute.IsBooleanAttribute(first.Key));
        }

        [NUnit.Framework.Test]
        public virtual void SettersOnOrphanAttribute() {
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("one", 
                "two");
            attr.SetKey("three");
            String oldVal = attr.SetValue("four");
            NUnit.Framework.Assert.AreEqual("two", oldVal);
            NUnit.Framework.Assert.AreEqual("three", attr.Key);
            NUnit.Framework.Assert.AreEqual("four", attr.Value);
            NUnit.Framework.Assert.IsNull(attr.parent);
        }

        [NUnit.Framework.Test]
        public virtual void HasValue() {
            iText.StyledXmlParser.Jsoup.Nodes.Attribute a1 = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("one", ""
                );
            iText.StyledXmlParser.Jsoup.Nodes.Attribute a2 = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("two", null
                );
            iText.StyledXmlParser.Jsoup.Nodes.Attribute a3 = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("thr", "thr"
                );
            NUnit.Framework.Assert.IsTrue(a1.HasDeclaredValue());
            NUnit.Framework.Assert.IsFalse(a2.HasDeclaredValue());
            NUnit.Framework.Assert.IsTrue(a3.HasDeclaredValue());
        }
    }
}
