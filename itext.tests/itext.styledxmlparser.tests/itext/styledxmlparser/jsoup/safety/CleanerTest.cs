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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Safety {
    /// <summary>Tests for the cleaner.</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class CleanerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimpleBehaviourTest() {
            String h = "<div><p class=foo><a href='http://evil.com'>Hello <b id=bar>there</b>!</a></div>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.SimpleText());
            NUnit.Framework.Assert.AreEqual("Hello <b>there</b>!", TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleBehaviourTest2() {
            String h = "Hello <b>there</b>!";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.SimpleText());
            NUnit.Framework.Assert.AreEqual("Hello <b>there</b>!", TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void BasicBehaviourTest() {
            String h = "<div><p><a href='javascript:sendAllMoney()'>Dodgy</a> <A HREF='HTTP://nice.com/'>Nice</a></p><blockquote>Hello</blockquote>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Basic());
            NUnit.Framework.Assert.AreEqual("<p><a rel=\"nofollow\">Dodgy</a> <a href=\"http://nice.com/\" rel=\"nofollow\">Nice</a></p><blockquote>Hello</blockquote>"
                , TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void BasicWithImagesTest() {
            String h = "<div><p><img src='http://example.com/' alt=Image></p><p><img src='ftp://ftp.example.com'></p></div>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.BasicWithImages());
            NUnit.Framework.Assert.AreEqual("<p><img src=\"http://example.com/\" alt=\"Image\"></p><p><img></p>", TextUtil
                .StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void TestRelaxed() {
            String h = "<h1>Head</h1><table><tr><td>One<td>Two</td></tr></table>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<h1>Head</h1><table><tbody><tr><td>One</td><td>Two</td></tr></tbody></table>"
                , TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveTags() {
            String h = "<div><p><A HREF='HTTP://nice.com'>Nice</a></p><blockquote>Hello</blockquote>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Basic().RemoveTags("a"));
            NUnit.Framework.Assert.AreEqual("<p>Nice</p><blockquote>Hello</blockquote>", TextUtil.StripNewlines(cleanHtml
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveAttributes() {
            String h = "<div><p>Nice</p><blockquote cite='http://example.com/quotations'>Hello</blockquote>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Basic().RemoveAttributes("blockquote"
                , "cite"));
            NUnit.Framework.Assert.AreEqual("<p>Nice</p><blockquote>Hello</blockquote>", TextUtil.StripNewlines(cleanHtml
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveEnforcedAttributes() {
            String h = "<div><p><A HREF='HTTP://nice.com/'>Nice</a></p><blockquote>Hello</blockquote>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Basic().RemoveEnforcedAttribute("a"
                , "rel"));
            NUnit.Framework.Assert.AreEqual("<p><a href=\"http://nice.com/\">Nice</a></p><blockquote>Hello</blockquote>"
                , TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void TestRemoveProtocols() {
            String h = "<p>Contact me <a href='mailto:info@example.com'>here</a></p>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Basic().RemoveProtocols("a", "href"
                , "ftp", "mailto"));
            NUnit.Framework.Assert.AreEqual("<p>Contact me <a rel=\"nofollow\">here</a></p>", TextUtil.StripNewlines(cleanHtml
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestDropComments() {
            String h = "<p>Hello<!-- no --></p>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<p>Hello</p>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestDropXmlProc() {
            String h = "<?import namespace=\"xss\"><p>Hello</p>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<p>Hello</p>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestDropScript() {
            String h = "<SCRIPT SRC=//ha.ckers.org/.j><SCRIPT>alert(/XSS/.source)</SCRIPT>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestDropImageScript() {
            String h = "<IMG SRC=\"javascript:alert('XSS')\">";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<img>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestCleanJavascriptHref() {
            String h = "<A HREF=\"javascript:document.location='http://www.google.com/'\">XSS</A>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<a>XSS</a>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestCleanAnchorProtocol() {
            String validAnchor = "<a href=\"#valid\">Valid anchor</a>";
            String invalidAnchor = "<a href=\"#anchor with spaces\">Invalid anchor</a>";
            // A Safelist that does not allow anchors will strip them out.
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(validAnchor, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<a>Valid anchor</a>", cleanHtml);
            cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(invalidAnchor, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<a>Invalid anchor</a>", cleanHtml);
            // A Safelist that allows them will keep them.
            Safelist relaxedWithAnchor = Safelist.Relaxed().AddProtocols("a", "href", "#");
            cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(validAnchor, relaxedWithAnchor);
            NUnit.Framework.Assert.AreEqual(validAnchor, cleanHtml);
            // An invalid anchor is never valid.
            cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(invalidAnchor, relaxedWithAnchor);
            NUnit.Framework.Assert.AreEqual("<a>Invalid anchor</a>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestDropsUnknownTags() {
            String h = "<p><custom foo=true>Test</custom></p>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.Relaxed());
            NUnit.Framework.Assert.AreEqual("<p>Test</p>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestHandlesEmptyAttributes() {
            String h = "<img alt=\"\" src= unknown=''>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, Safelist.BasicWithImages());
            NUnit.Framework.Assert.AreEqual("<img alt=\"\">", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void TestIsValidBodyHtml() {
            String ok = "<p>Test <b><a href='http://example.com/' rel='nofollow'>OK</a></b></p>";
            String ok1 = "<p>Test <b><a href='http://example.com/'>OK</a></b></p>";
            // missing enforced is OK because still needs run thru cleaner
            String nok1 = "<p><script></script>Not <b>OK</b></p>";
            String nok2 = "<p align=right>Test Not <b>OK</b></p>";
            String nok3 = "<!-- comment --><p>Not OK</p>";
            // comments and the like will be cleaned
            String nok4 = "<html><head>Foo</head><body><b>OK</b></body></html>";
            // not body html
            String nok5 = "<p>Test <b><a href='http://example.com/' rel='nofollowme'>OK</a></b></p>";
            String nok6 = "<p>Test <b><a href='http://example.com/'>OK</b></p>";
            // missing close tag
            String nok7 = "</div>What";
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(ok, Safelist.Basic()));
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(ok1, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok1, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok2, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok3, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok4, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok5, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok6, Safelist.Basic()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(ok, Safelist.None()));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Jsoup.IsValid(nok7, Safelist.Basic()));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsValidDocument() {
            String ok = "<html><head></head><body><p>Hello</p></body><html>";
            String nok = "<html><head><script>woops</script><title>Hello</title></head><body><p>Hello</p></body><html>";
            Safelist relaxed = Safelist.Relaxed();
            Cleaner cleaner = new Cleaner(relaxed);
            Document okDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(ok);
            NUnit.Framework.Assert.IsTrue(cleaner.IsValid(okDoc));
            NUnit.Framework.Assert.IsFalse(cleaner.IsValid(iText.StyledXmlParser.Jsoup.Jsoup.Parse(nok)));
            NUnit.Framework.Assert.IsFalse(new Cleaner(Safelist.None()).IsValid(okDoc));
        }

        [NUnit.Framework.Test]
        public virtual void ResolvesRelativeLinks() {
            String html = "<a href='/foo'>Link</a><img src='/bar'>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://example.com/", Safelist.BasicWithImages
                ());
            NUnit.Framework.Assert.AreEqual("<a href=\"http://example.com/foo\" rel=\"nofollow\">Link</a>\n<img src=\"http://example.com/bar\">"
                , clean);
        }

        [NUnit.Framework.Test]
        public virtual void PreservesRelativeLinksIfConfigured() {
            String html = "<a href='/foo'>Link</a><img src='/bar'> <img src='javascript:alert()'>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://example.com/", Safelist.BasicWithImages
                ().PreserveRelativeLinks(true));
            NUnit.Framework.Assert.AreEqual("<a href=\"/foo\" rel=\"nofollow\">Link</a>\n<img src=\"/bar\"> \n<img>", 
                clean);
        }

        [NUnit.Framework.Test]
        public virtual void DropsUnresolvableRelativeLinks() {
            String html = "<a href='/foo'>Link</a>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, Safelist.Basic());
            NUnit.Framework.Assert.AreEqual("<a rel=\"nofollow\">Link</a>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCustomProtocols() {
            String html = "<img src='cid:12345' /> <img src='data:gzzt' />";
            String dropped = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, Safelist.BasicWithImages());
            NUnit.Framework.Assert.AreEqual("<img> \n<img>", dropped);
            String preserved = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, Safelist.BasicWithImages().AddProtocols("img"
                , "src", "cid", "data"));
            NUnit.Framework.Assert.AreEqual("<img src=\"cid:12345\"> \n<img src=\"data:gzzt\">", preserved);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAllPseudoTag() {
            String html = "<p class='foo' src='bar'><a class='qux'>link</a></p>";
            Safelist safelist = new Safelist().AddAttributes(":all", "class").AddAttributes("p", "style").AddTags("p", 
                "a");
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, safelist);
            NUnit.Framework.Assert.AreEqual("<p class=\"foo\"><a class=\"qux\">link</a></p>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void AddsTagOnAttributesIfNotSet() {
            String html = "<p class='foo' src='bar'>One</p>";
            Safelist safelist = new Safelist().AddAttributes("p", "class");
            // ^^ safelist does not have explicit tag add for p, inferred from add attributes.
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, safelist);
            NUnit.Framework.Assert.AreEqual("<p class=\"foo\">One</p>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void SupplyOutputSettings() {
            // test that one can override the default document output settings
            OutputSettings os = new OutputSettings();
            os.PrettyPrint(false);
            os.EscapeMode(Entities.EscapeMode.extended);
            os.Charset("ascii");
            String html = "<div><p>&bernou;</p></div>";
            String customOut = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", Safelist.Relaxed(), os
                );
            String defaultOut = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", Safelist.Relaxed());
            NUnit.Framework.Assert.AreNotSame(defaultOut, customOut);
            NUnit.Framework.Assert.AreEqual("<div><p>&Bscr;</p></div>", customOut);
            // entities now prefers shorted names if aliased
            NUnit.Framework.Assert.AreEqual("<div>\n" + " <p>ℬ</p>\n" + "</div>", defaultOut);
            os.Charset("ASCII");
            os.EscapeMode(Entities.EscapeMode.@base);
            String customOut2 = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", Safelist.Relaxed(), os
                );
            NUnit.Framework.Assert.AreEqual("<div><p>&#x212c;</p></div>", customOut2);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesFramesets() {
            String dirty = "<html><head><script></script><noscript></noscript></head><frameset><frame src=\"foo\" /><frame src=\"foo\" /></frameset></html>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(dirty, Safelist.Basic());
            NUnit.Framework.Assert.AreEqual("", clean);
            // nothing good can come out of that
            Document dirtyDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(dirty);
            Document cleanDoc = new Cleaner(Safelist.Basic()).Clean(dirtyDoc);
            NUnit.Framework.Assert.IsNotNull(cleanDoc);
            NUnit.Framework.Assert.AreEqual(0, cleanDoc.Body().ChildNodeSize());
        }

        [NUnit.Framework.Test]
        public virtual void CleansInternationalText() {
            NUnit.Framework.Assert.AreEqual("привет", iText.StyledXmlParser.Jsoup.Jsoup.Clean("привет", Safelist.None(
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestScriptTagInSafeList() {
            Safelist safelist = Safelist.Relaxed();
            safelist.AddTags("script");
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Jsoup.IsValid("Hello<script>alert('Doh')</script>World !"
                , safelist));
        }

        [NUnit.Framework.Test]
        public virtual void BailsIfRemovingProtocolThatsNotSet() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                // a case that came up on the email list
                Safelist w = Safelist.None();
                // note no add tag, and removing protocol without adding first
                w.AddAttributes("a", "href");
                w.RemoveProtocols("a", "href", "javascript");
            }
            );
        }

        // with no protocols enforced, this was a noop. Now validates.
        [NUnit.Framework.Test]
        public virtual void HandlesControlCharactersAfterTagName() {
            String html = "<a/\x6>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, Safelist.Basic());
            NUnit.Framework.Assert.AreEqual("<a rel=\"nofollow\"></a>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesAttributesWithNoValue() {
            // https://github.com/jhy/jsoup/issues/973
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean("<a href>Clean</a>", Safelist.Basic());
            NUnit.Framework.Assert.AreEqual("<a rel=\"nofollow\">Clean</a>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNoHrefAttribute() {
            String dirty = "<a>One</a> <a href>Two</a>";
            Safelist relaxedWithAnchor = Safelist.Relaxed().AddProtocols("a", "href", "#");
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(dirty, relaxedWithAnchor);
            NUnit.Framework.Assert.AreEqual("<a>One</a> <a>Two</a>", clean);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesNestedQuotesInAttribute() {
            // https://github.com/jhy/jsoup/issues/1243 - no repro
            String orig = "<div style=\"font-family: 'Calibri'\">Will (not) fail</div>";
            Safelist allow = Safelist.Relaxed().AddAttributes("div", "style");
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(orig, allow);
            bool isValid = iText.StyledXmlParser.Jsoup.Jsoup.IsValid(orig, allow);
            NUnit.Framework.Assert.AreEqual(orig, TextUtil.StripNewlines(clean));
            // only difference is pretty print wrap & indent
            NUnit.Framework.Assert.IsTrue(isValid);
        }

        [NUnit.Framework.Test]
        public virtual void CopiesOutputSettings() {
            Document orig = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<p>test<br></p>");
            orig.OutputSettings().Syntax(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml);
            orig.OutputSettings().EscapeMode(Entities.EscapeMode.xhtml);
            Safelist whitelist = Safelist.None().AddTags("p", "br");
            Document result = new Cleaner(whitelist).Clean(orig);
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Jsoup.Nodes.Syntax.xml, result.OutputSettings().Syntax
                ());
            NUnit.Framework.Assert.AreEqual("<p>test<br /></p>", result.Body().Html());
        }
    }
}
