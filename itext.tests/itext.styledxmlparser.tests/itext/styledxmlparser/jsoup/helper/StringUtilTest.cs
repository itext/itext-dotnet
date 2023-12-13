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
using iText.IO.Util;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Helper {
    public class StringUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Join() {
            NUnit.Framework.Assert.AreEqual("", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList
                (""), " "));
            NUnit.Framework.Assert.AreEqual("one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList
                ("one"), " "));
            NUnit.Framework.Assert.AreEqual("one two three", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Join(JavaUtil.ArraysAsList
                ("one", "two", "three"), " "));
        }

        [NUnit.Framework.Test]
        public virtual void Padding() {
            NUnit.Framework.Assert.AreEqual("", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Padding(0));
            NUnit.Framework.Assert.AreEqual(" ", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Padding(1));
            NUnit.Framework.Assert.AreEqual("  ", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Padding(2));
            NUnit.Framework.Assert.AreEqual("               ", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Padding(15
                ));
        }

        [NUnit.Framework.Test]
        public virtual void IsBlank() {
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank(null));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank(""));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank("      "));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank("   \r\n  "));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank("hello"));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsBlank("   hello   "));
        }

        [NUnit.Framework.Test]
        public virtual void IsNumeric() {
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric(null));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric(" "));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric("123 546"));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric("hello"));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric("123.334"));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric("1"));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric("1234"));
        }

        [NUnit.Framework.Test]
        public virtual void IsWhitespace() {
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\t'));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\n'));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\r'));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\f'));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace(' '));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\u00a0'));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\u2000'));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsWhitespace('\u3000'));
        }

        [NUnit.Framework.Test]
        public virtual void NormaliseWhiteSpace() {
            NUnit.Framework.Assert.AreEqual(" ", iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace("    \r \n \r\n"
                ));
            NUnit.Framework.Assert.AreEqual(" hello there ", iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace
                ("   hello   \r \n  there    \n"));
            NUnit.Framework.Assert.AreEqual("hello", iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace
                ("hello"));
            NUnit.Framework.Assert.AreEqual("hello there", iText.StyledXmlParser.Jsoup.Helper.StringUtil.NormaliseWhitespace
                ("hello\nthere"));
        }

        [NUnit.Framework.Test]
        public virtual void NormaliseWhiteSpaceHandlesHighSurrogates() {
            String test71540chars = "\ud869\udeb2\u304b\u309a  1";
            String test71540charsExpectedSingleWhitespace = "\ud869\udeb2\u304b\u309a 1";
            NUnit.Framework.Assert.AreEqual(test71540charsExpectedSingleWhitespace, iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .NormaliseWhitespace(test71540chars));
            String extractedText = iText.StyledXmlParser.Jsoup.Jsoup.Parse(test71540chars).Text();
            NUnit.Framework.Assert.AreEqual(test71540charsExpectedSingleWhitespace, extractedText);
        }

        [NUnit.Framework.Test]
        public virtual void ResolvesRelativeUrls() {
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("http://example.com", "./one/two?three"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("http://example.com?one", "./one/two?three"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two?three#four", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("http://example.com", "./one/two?three#four"));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve
                ("http://example.com/", "https://example.com/one"));
            NUnit.Framework.Assert.AreEqual("http://example.com/one/two.html", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("http://example.com/two/", "../one/two.html"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.
                Resolve("https://example.com/", "//example2.com/one"));
            NUnit.Framework.Assert.AreEqual("https://example.com:8080/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("https://example.com:8080", "./one"));
            NUnit.Framework.Assert.AreEqual("https://example2.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.
                Resolve("http://example.com/", "https://example2.com/one"));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve
                ("wrong", "https://example.com/one"));
            NUnit.Framework.Assert.AreEqual("https://example.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve
                ("https://example.com/one", ""));
            NUnit.Framework.Assert.AreEqual("", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve("wrong", "also wrong"
                ));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve
                ("ftp://example.com/two/", "../one"));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one/two.c", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("ftp://example.com/one/", "./two.c"));
            NUnit.Framework.Assert.AreEqual("ftp://example.com/one/two.c", iText.StyledXmlParser.Jsoup.Helper.StringUtil
                .Resolve("ftp://example.com/one/", "two.c"));
        }
    }
}
