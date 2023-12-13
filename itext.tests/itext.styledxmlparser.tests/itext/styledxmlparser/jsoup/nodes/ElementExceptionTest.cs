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
