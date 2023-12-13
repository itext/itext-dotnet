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

namespace iText.StyledXmlParser.Jsoup.Nodes {
    public class EntitiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Escape() {
            String text = "Hello &<> Å å π 新 there ¾ © »";
            String escapedAscii = Entities.Escape(text, new OutputSettings().Charset("ascii").EscapeMode(Entities.EscapeMode
                .@base));
            String escapedAsciiFull = Entities.Escape(text, new OutputSettings().Charset("ascii").EscapeMode(Entities.EscapeMode
                .extended));
            String escapedAsciiXhtml = Entities.Escape(text, new OutputSettings().Charset("ascii").EscapeMode(Entities.EscapeMode
                .xhtml));
            String escapedUtfFull = Entities.Escape(text, new OutputSettings().Charset("UTF-8").EscapeMode(Entities.EscapeMode
                .extended));
            String escapedUtfMin = Entities.Escape(text, new OutputSettings().Charset("UTF-8").EscapeMode(Entities.EscapeMode
                .xhtml));
            NUnit.Framework.Assert.AreEqual("Hello &amp;&lt;&gt; &Aring; &aring; &#x3c0; &#x65b0; there &frac34; &copy; &raquo;"
                , escapedAscii);
            NUnit.Framework.Assert.AreEqual("Hello &amp;&lt;&gt; &angst; &aring; &pi; &#x65b0; there &frac34; &copy; &raquo;"
                , escapedAsciiFull);
            NUnit.Framework.Assert.AreEqual("Hello &amp;&lt;&gt; &#xc5; &#xe5; &#x3c0; &#x65b0; there &#xbe; &#xa9; &#xbb;"
                , escapedAsciiXhtml);
            NUnit.Framework.Assert.AreEqual("Hello &amp;&lt;&gt; Å å π 新 there ¾ © »", escapedUtfFull);
            NUnit.Framework.Assert.AreEqual("Hello &amp;&lt;&gt; Å å π 新 there ¾ © »", escapedUtfMin);
            // odd that it's defined as aring in base but angst in full
            // round trip
            NUnit.Framework.Assert.AreEqual(text, Entities.Unescape(escapedAscii));
            NUnit.Framework.Assert.AreEqual(text, Entities.Unescape(escapedAsciiFull));
            NUnit.Framework.Assert.AreEqual(text, Entities.Unescape(escapedAsciiXhtml));
            NUnit.Framework.Assert.AreEqual(text, Entities.Unescape(escapedUtfFull));
            NUnit.Framework.Assert.AreEqual(text, Entities.Unescape(escapedUtfMin));
        }

        [NUnit.Framework.Test]
        public virtual void EscapeSupplementaryCharacter() {
            String text = new String(iText.IO.Util.TextUtil.ToChars(135361));
            String escapedAscii = Entities.Escape(text, new OutputSettings().Charset("ascii").EscapeMode(Entities.EscapeMode
                .@base));
            NUnit.Framework.Assert.AreEqual("&#x210c1;", escapedAscii);
            String escapedUtf = Entities.Escape(text, new OutputSettings().Charset("UTF-8").EscapeMode(Entities.EscapeMode
                .@base));
            NUnit.Framework.Assert.AreEqual(text, escapedUtf);
        }

        [NUnit.Framework.Test]
        public virtual void Unescape() {
            String text = "Hello &amp;&LT&gt; &reg &angst; &angst &#960; &#960 &#x65B0; there &! &frac34; &copy; &COPY;";
            NUnit.Framework.Assert.AreEqual("Hello &<> ® Å &angst π π 新 there &! ¾ © ©", Entities.Unescape(text));
            NUnit.Framework.Assert.AreEqual("&0987654321; &unknown", Entities.Unescape("&0987654321; &unknown"));
        }

        [NUnit.Framework.Test]
        public virtual void StrictUnescape() {
            // for attributes, enforce strict unescaping (must look like &#xxx; , not just &#xxx)
            String text = "Hello &amp= &amp;";
            NUnit.Framework.Assert.AreEqual("Hello &amp= &", Entities.Unescape(text, true));
            NUnit.Framework.Assert.AreEqual("Hello &= &", Entities.Unescape(text));
            NUnit.Framework.Assert.AreEqual("Hello &= &", Entities.Unescape(text, false));
        }

        [NUnit.Framework.Test]
        public virtual void CaseSensitive() {
            String unescaped = "Ü ü & &";
            NUnit.Framework.Assert.AreEqual("&Uuml; &uuml; &amp; &amp;", Entities.Escape(unescaped, new OutputSettings
                ().Charset("ascii").EscapeMode(Entities.EscapeMode.extended)));
            String escaped = "&Uuml; &uuml; &amp; &AMP";
            NUnit.Framework.Assert.AreEqual("Ü ü & &", Entities.Unescape(escaped));
        }

        [NUnit.Framework.Test]
        public virtual void QuoteReplacements() {
            String escaped = "&#92; &#36;";
            String unescaped = "\\ $";
            NUnit.Framework.Assert.AreEqual(unescaped, Entities.Unescape(escaped));
        }

        [NUnit.Framework.Test]
        public virtual void LetterDigitEntities() {
            String html = "<p>&sup1;&sup2;&sup3;&frac14;&frac12;&frac34;</p>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(html);
            doc.OutputSettings().Charset("ascii");
            iText.StyledXmlParser.Jsoup.Nodes.Element p = doc.Select("p").First();
            NUnit.Framework.Assert.AreEqual("&sup1;&sup2;&sup3;&frac14;&frac12;&frac34;", p.Html());
            NUnit.Framework.Assert.AreEqual("¹²³¼½¾", p.Text());
            doc.OutputSettings().Charset("UTF-8");
            NUnit.Framework.Assert.AreEqual("¹²³¼½¾", p.Html());
        }

        [NUnit.Framework.Test]
        public virtual void NoSpuriousDecodes() {
            String @string = "http://www.foo.com?a=1&num_rooms=1&children=0&int=VA&b=2";
            NUnit.Framework.Assert.AreEqual(@string, Entities.Unescape(@string));
        }

        [NUnit.Framework.Test]
        public virtual void EscapesGtInXmlAttributesButNotInHtml() {
            // https://github.com/jhy/jsoup/issues/528 - < is OK in HTML attribute values, but not in XML
            String docHtml = "<a title='<p>One</p>'>One</a>";
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(docHtml);
            iText.StyledXmlParser.Jsoup.Nodes.Element element = doc.Select("a").First();
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.@base);
            NUnit.Framework.Assert.AreEqual("<a title=\"<p>One</p>\">One</a>", element.OuterHtml());
            doc.OutputSettings().EscapeMode(Entities.EscapeMode.xhtml);
            NUnit.Framework.Assert.AreEqual("<a title=\"&lt;p>One&lt;/p>\">One</a>", element.OuterHtml());
        }
    }
}
