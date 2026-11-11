/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;
using NUnit.Framework;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Element (DOM stuff mostly).</summary>
    /// <author>Jonathan Hedley</author>
    public class ElementExceptionTest : ExtendedITextTest {
        [Test]
        public virtual void TestThrowsOnAddNullText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            Element div = doc.GetElementById("1");
            Assert.Throws(typeof(ArgumentException), () => div.AppendText(null));
        }

        [Test]
        public virtual void TestThrowsOnPrependNullText() {
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<div id=1><p>Hello</p></div>");
            Element div = doc.GetElementById("1");
            Assert.Throws(typeof(ArgumentException), () => div.PrependText(null));
        }

        [Test]
        public virtual void TestChildThrowsIndexOutOfBoundsOnMissing() {
            Document doc = Jsoup.Parse("<div><p>One</p><p>Two</p></div>");
            Element div = doc.Select("div").First();

            Assert.AreEqual(2, div.Children().Count);
            Assert.AreEqual("One", div.Child(0).Text());

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => div.Child(3));
        }
    }
}
