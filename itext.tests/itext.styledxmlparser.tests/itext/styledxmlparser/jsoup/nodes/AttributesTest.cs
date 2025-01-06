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
    /// <summary>Tests for Attributes.</summary>
    public class AttributesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Html() {
            Attributes a = new Attributes();
            a.Put("Tot", "a&p");
            a.Put("Hello", "There");
            a.Put("data-name", "Jsoup");
            NUnit.Framework.Assert.AreEqual(3, a.Size());
            NUnit.Framework.Assert.IsTrue(a.HasKey("Tot"));
            NUnit.Framework.Assert.IsTrue(a.HasKey("Hello"));
            NUnit.Framework.Assert.IsTrue(a.HasKey("data-name"));
            NUnit.Framework.Assert.IsFalse(a.HasKey("tot"));
            NUnit.Framework.Assert.IsTrue(a.HasKeyIgnoreCase("tot"));
            NUnit.Framework.Assert.AreEqual("There", a.GetIgnoreCase("hEllo"));
            IDictionary<String, String> dataset = a.Dataset();
            NUnit.Framework.Assert.AreEqual(1, dataset.Count);
            NUnit.Framework.Assert.AreEqual("Jsoup", dataset.Get("name"));
            NUnit.Framework.Assert.AreEqual("", a.Get("tot"));
            NUnit.Framework.Assert.AreEqual("a&p", a.Get("Tot"));
            NUnit.Framework.Assert.AreEqual("a&p", a.GetIgnoreCase("tot"));
            NUnit.Framework.Assert.AreEqual(" Tot=\"a&amp;p\" Hello=\"There\" data-name=\"Jsoup\"", a.Html());
            NUnit.Framework.Assert.AreEqual(a.Html(), a.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestIteratorUpdateable() {
            Attributes a = new Attributes();
            a.Put("Tot", "a&p");
            a.Put("Hello", "There");
            NUnit.Framework.Assert.IsFalse(a.HasKey("Foo"));
            IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> iterator = a.GetEnumerator();
            iterator.MoveNext();
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = iterator.Current;
            iterator.MoveNext();
            attr.SetKey("Foo");
            attr = iterator.Current;
            attr.SetKey("Bar");
            attr.SetValue("Qux");
            NUnit.Framework.Assert.AreEqual("a&p", a.Get("Foo"));
            NUnit.Framework.Assert.AreEqual("Qux", a.Get("Bar"));
            NUnit.Framework.Assert.IsFalse(a.HasKey("Tot"));
            NUnit.Framework.Assert.IsFalse(a.HasKey("Hello"));
        }

        [NUnit.Framework.Test]
        public virtual void TestIteratorHasNext() {
            Attributes a = new Attributes();
            a.Put("Tot", "1");
            a.Put("Hello", "2");
            a.Put("data-name", "3");
            int seen = 0;
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in a) {
                seen++;
                NUnit.Framework.Assert.AreEqual(seen.ToString(), attribute.Value);
            }
            NUnit.Framework.Assert.AreEqual(3, seen);
        }

        [NUnit.Framework.Test]
        public virtual void TestIterator() {
            Attributes a = new Attributes();
            String[][] datas = new String[][] { new String[] { "Tot", "raul" }, new String[] { "Hello", "pismuth" }, new 
                String[] { "data-name", "Jsoup" } };
            foreach (String[] atts in datas) {
                a.Put(atts[0], atts[1]);
            }
            
            int i = 0;
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in a) {
                NUnit.Framework.Assert.AreEqual(datas[i][0], attribute.Key);
                NUnit.Framework.Assert.AreEqual(datas[i][1], attribute.Value);
                i++;
            }
            NUnit.Framework.Assert.AreEqual(datas.Length, i);
        }

        [NUnit.Framework.Test]
        public virtual void TestIteratorSkipsInternal() {
            Attributes a = new Attributes();
            a.Put("One", "One");
            a.Put(Attributes.InternalKey("baseUri"), "example.com");
            a.Put("Two", "Two");
            a.Put(Attributes.InternalKey("another"), "example.com");
            IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> it = a.GetEnumerator();
            NUnit.Framework.Assert.IsTrue(it.MoveNext());
            NUnit.Framework.Assert.AreEqual("One", it.Current.Key);
            NUnit.Framework.Assert.IsTrue(it.MoveNext());
            NUnit.Framework.Assert.AreEqual("Two", it.Current.Key);
            NUnit.Framework.Assert.IsFalse(it.MoveNext());
            int seen = 0;
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in a) {
                seen++;
            }
            NUnit.Framework.Assert.AreEqual(2, seen);
        }

        [NUnit.Framework.Test]
        public virtual void TestListSkipsInternal() {
            Attributes a = new Attributes();
            a.Put("One", "One");
            a.Put(Attributes.InternalKey("baseUri"), "example.com");
            a.Put("Two", "Two");
            a.Put(Attributes.InternalKey("another"), "example.com");
            IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> attributes = a.AsList();
            NUnit.Framework.Assert.AreEqual(2, attributes.Count);
            NUnit.Framework.Assert.AreEqual("One", attributes[0].Key);
            NUnit.Framework.Assert.AreEqual("Two", attributes[1].Key);
        }

        [NUnit.Framework.Test]
        public virtual void HtmlSkipsInternals() {
            Attributes a = new Attributes();
            a.Put("One", "One");
            a.Put(Attributes.InternalKey("baseUri"), "example.com");
            a.Put("Two", "Two");
            a.Put(Attributes.InternalKey("another"), "example.com");
            NUnit.Framework.Assert.AreEqual(" One=\"One\" Two=\"Two\"", a.Html());
        }

        [NUnit.Framework.Test]
        public virtual void TestIteratorEmpty() {
            Attributes a = new Attributes();
            IEnumerator<iText.StyledXmlParser.Jsoup.Nodes.Attribute> iterator = a.GetEnumerator();
            NUnit.Framework.Assert.IsFalse(iterator.MoveNext());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveCaseSensitive() {
            Attributes a = new Attributes();
            a.Put("Tot", "a&p");
            a.Put("tot", "one");
            a.Put("Hello", "There");
            a.Put("hello", "There");
            a.Put("data-name", "Jsoup");
            NUnit.Framework.Assert.AreEqual(5, a.Size());
            a.Remove("Tot");
            a.Remove("Hello");
            NUnit.Framework.Assert.AreEqual(3, a.Size());
            NUnit.Framework.Assert.IsTrue(a.HasKey("tot"));
            NUnit.Framework.Assert.IsFalse(a.HasKey("Tot"));
        }

        [NUnit.Framework.Test]
        public virtual void TestSetKeyConsistency() {
            Attributes a = new Attributes();
            a.Put("a", "a");
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute at in a) {
                at.SetKey("b");
            }
            NUnit.Framework.Assert.IsFalse(a.HasKey("a"));
            NUnit.Framework.Assert.IsTrue(a.HasKey("b"));
        }

        [NUnit.Framework.Test]
        public virtual void TestBoolean() {
            Attributes ats = new Attributes();
            ats.Put("a", "a");
            ats.Put("B", "b");
            ats.Put("c", null);
            NUnit.Framework.Assert.IsTrue(ats.HasDeclaredValueForKey("a"));
            NUnit.Framework.Assert.IsFalse(ats.HasDeclaredValueForKey("A"));
            NUnit.Framework.Assert.IsTrue(ats.HasDeclaredValueForKeyIgnoreCase("A"));
            NUnit.Framework.Assert.IsFalse(ats.HasDeclaredValueForKey("c"));
            NUnit.Framework.Assert.IsFalse(ats.HasDeclaredValueForKey("C"));
            NUnit.Framework.Assert.IsFalse(ats.HasDeclaredValueForKeyIgnoreCase("C"));
        }

        [NUnit.Framework.Test]
        public virtual void TestSizeWhenHasInternal() {
            Attributes a = new Attributes();
            a.Put("One", "One");
            a.Put("Two", "Two");
            NUnit.Framework.Assert.AreEqual(2, a.Size());
            a.Put(Attributes.InternalKey("baseUri"), "example.com");
            a.Put(Attributes.InternalKey("another"), "example.com");
            NUnit.Framework.Assert.AreEqual(2, a.Size());
        }
    }
}
