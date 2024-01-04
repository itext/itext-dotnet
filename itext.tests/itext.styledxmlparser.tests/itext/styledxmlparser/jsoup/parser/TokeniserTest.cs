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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class TokeniserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BufferUpInAttributeVal() {
            // https://github.com/jhy/jsoup/issues/967
            // check each double, singlem, unquoted impls
            String[] quotes = new String[] { "\"", "'", "" };
            foreach (String quote in quotes) {
                String preamble = "<img src=" + quote;
                String tail = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
                StringBuilder sb = new StringBuilder(preamble);
                int charsToFillBuffer = CharacterReader.maxBufferLen - preamble.Length;
                for (int i = 0; i < charsToFillBuffer; i++) {
                    sb.Append('a');
                }
                sb.Append('X');
                // First character to cross character buffer boundary
                sb.Append(tail).Append(quote).Append(">\n");
                String html = sb.ToString();
                Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
                String src = doc.Select("img").Attr("src");
                NUnit.Framework.Assert.IsTrue(src.Contains("X"));
                NUnit.Framework.Assert.IsTrue(src.Contains(tail));
            }
        }

        [NUnit.Framework.Test]
        public virtual void HandleSuperLargeTagNames() {
            // unlikely, but valid. so who knows.
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("LargeTagName");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String tag = sb.ToString();
            String html = "<" + tag + ">One</" + tag + ">";
            Document doc = iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().Settings(ParseSettings.preserveCase)
                .ParseInput(html, "");
            Elements els = doc.Select(tag);
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            NUnit.Framework.Assert.AreEqual("One", el.Text());
            NUnit.Framework.Assert.AreEqual(tag, el.TagName());
        }

        [NUnit.Framework.Test]
        public virtual void HandleSuperLargeAttributeName() {
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("LargAttributeName");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String attrName = sb.ToString();
            String html = "<p " + attrName + "=foo>One</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.GetElementsByAttribute(attrName);
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            NUnit.Framework.Assert.AreEqual("One", el.Text());
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute = el.Attributes().AsList()[0];
            NUnit.Framework.Assert.AreEqual(attrName.ToLowerInvariant(), attribute.Key);
            NUnit.Framework.Assert.AreEqual("foo", attribute.Value);
        }

        [NUnit.Framework.Test]
        public virtual void HandleLargeText() {
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("A Large Amount of Text");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String text = sb.ToString();
            String html = "<p>" + text + "</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            NUnit.Framework.Assert.AreEqual(text, el.Text());
        }

        [NUnit.Framework.Test]
        public virtual void HandleLargeComment() {
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("Quite a comment ");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String comment = sb.ToString();
            String html = "<p><!-- " + comment + " --></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            Comment child = (Comment)el.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(" " + comment + " ", child.GetData());
        }

        [NUnit.Framework.Test]
        public virtual void HandleLargeCdata() {
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("Quite a lot of CDATA <><><><>");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String cdata = sb.ToString();
            String html = "<p><![CDATA[" + cdata + "]]></p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("p");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            TextNode child = (TextNode)el.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(cdata, el.Text());
            NUnit.Framework.Assert.AreEqual(cdata, child.GetWholeText());
        }

        [NUnit.Framework.Test]
        public virtual void HandleLargeTitle() {
            StringBuilder sb = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                sb.Append("Quite a long title");
            }
            while (sb.Length < CharacterReader.maxBufferLen);
            String title = sb.ToString();
            String html = "<title>" + title + "</title>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            Elements els = doc.Select("title");
            NUnit.Framework.Assert.AreEqual(1, els.Count);
            iText.StyledXmlParser.Jsoup.Nodes.Element el = els.First();
            NUnit.Framework.Assert.IsNotNull(el);
            TextNode child = (TextNode)el.ChildNode(0);
            NUnit.Framework.Assert.AreEqual(title, el.Text());
            NUnit.Framework.Assert.AreEqual(title, child.GetWholeText());
            NUnit.Framework.Assert.AreEqual(title, doc.Title());
        }

        [NUnit.Framework.Test]
        public virtual void Cp1252Entities() {
            NUnit.Framework.Assert.AreEqual("\u20ac", iText.StyledXmlParser.Jsoup.Jsoup.Parse("&#0128;").Text());
            NUnit.Framework.Assert.AreEqual("\u201a", iText.StyledXmlParser.Jsoup.Jsoup.Parse("&#0130;").Text());
            NUnit.Framework.Assert.AreEqual("\u20ac", iText.StyledXmlParser.Jsoup.Jsoup.Parse("&#x80;").Text());
        }

        [NUnit.Framework.Test]
        public virtual void Cp1252EntitiesProduceError() {
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = new iText.StyledXmlParser.Jsoup.Parser.Parser(new HtmlTreeBuilder
                ());
            parser.SetTrackErrors(10);
            NUnit.Framework.Assert.AreEqual("\u20ac", parser.ParseInput("<html><body>&#0128;</body></html>", "").Text(
                ));
            NUnit.Framework.Assert.AreEqual(1, parser.GetErrors().Count);
        }

        [NUnit.Framework.Test]
        public virtual void Cp1252SubstitutionTable() {
            for (int i = 0; i < Tokeniser.win1252Extensions.Length; i++) {
                String s = iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { (byte)(i + Tokeniser.win1252ExtensionsStart
                    ) }, "Windows-1252");
                NUnit.Framework.Assert.AreEqual(1, s.Length);
                // some of these characters are illegal
                if (s[0] == '\ufffd') {
                    continue;
                }
                NUnit.Framework.Assert.AreEqual(s[0], Tokeniser.win1252Extensions[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CanParseVeryLongBogusComment() {
            StringBuilder commentData = new StringBuilder(CharacterReader.maxBufferLen);
            do {
                commentData.Append("blah blah blah blah ");
            }
            while (commentData.Length < CharacterReader.maxBufferLen);
            String expectedCommentData = commentData.ToString();
            String testMarkup = "<html><body><!" + expectedCommentData + "></body></html>";
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = new iText.StyledXmlParser.Jsoup.Parser.Parser(new HtmlTreeBuilder
                ());
            Document doc = parser.ParseInput(testMarkup, "");
            iText.StyledXmlParser.Jsoup.Nodes.Node commentNode = doc.Body().ChildNode(0);
            NUnit.Framework.Assert.IsTrue(commentNode is Comment);
            NUnit.Framework.Assert.AreEqual(expectedCommentData, ((Comment)commentNode).GetData());
        }

        [NUnit.Framework.Test]
        public virtual void CanParseCdataEndingAtEdgeOfBuffer() {
            String cdataStart = "<![CDATA[";
            String cdataEnd = "]]>";
            int bufLen = CharacterReader.maxBufferLen - cdataStart.Length - 1;
            // also breaks with -2, but not with -3 or 0
            char[] cdataContentsArray = new char[bufLen];
            JavaUtil.Fill(cdataContentsArray, 'x');
            String cdataContents = new String(cdataContentsArray);
            String testMarkup = cdataStart + cdataContents + cdataEnd;
            iText.StyledXmlParser.Jsoup.Parser.Parser parser = new iText.StyledXmlParser.Jsoup.Parser.Parser(new HtmlTreeBuilder
                ());
            Document doc = parser.ParseInput(testMarkup, "");
            iText.StyledXmlParser.Jsoup.Nodes.Node cdataNode = doc.Body().ChildNode(0);
            NUnit.Framework.Assert.IsTrue(cdataNode is CDataNode);
            NUnit.Framework.Assert.AreEqual(cdataContents, ((CDataNode)cdataNode).Text());
        }
    }
}
