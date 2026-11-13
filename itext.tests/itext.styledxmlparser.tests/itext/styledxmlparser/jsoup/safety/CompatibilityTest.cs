/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Safety {
    /// <summary>
    /// Tests for the deprecated
    /// <see cref="Whitelist"/>
    /// class source compatibility.
    /// </summary>
    /// <remarks>
    /// Tests for the deprecated
    /// <see cref="Whitelist"/>
    /// class source compatibility. Will be removed in
    /// <c>v.1.15.1</c>. No net new tests here so safe to blow up.
    /// </remarks>
    [NUnit.Framework.Category("UnitTest")]
    public class CompatibilityTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResolvesRelativeLinks() {
            String html = "<a href='/foo'>Link</a><img src='/bar'>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://example.com/", ((Whitelist)Whitelist.
                BasicWithImages()));
            NUnit.Framework.Assert.AreEqual("<a href=\"http://example.com/foo\" rel=\"nofollow\">Link</a>\n<img src=\"http://example.com/bar\">"
                , clean);
        }

        [NUnit.Framework.Test]
        public virtual void TestDropsUnknownTags() {
            String h = "<p><custom foo=true>Test</custom></p>";
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean(h, ((Whitelist)Whitelist.Relaxed()));
            NUnit.Framework.Assert.AreEqual("<p>Test</p>", cleanHtml);
        }

        [NUnit.Framework.Test]
        public virtual void PreservesRelativeLinksIfConfigured() {
            String html = "<a href='/foo'>Link</a><img src='/bar'> <img src='javascript:alert()'>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://example.com/", ((Whitelist)Whitelist.
                BasicWithImages()).PreserveRelativeLinks(true));
            NUnit.Framework.Assert.AreEqual("<a href=\"/foo\" rel=\"nofollow\">Link</a>\n<img src=\"/bar\"> \n<img>", 
                clean);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCustomProtocols() {
            String html = "<img src='cid:12345' /> <img src='data:gzzt' />";
            String dropped = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, ((Whitelist)Whitelist.BasicWithImages()));
            NUnit.Framework.Assert.AreEqual("<img> \n<img>", dropped);
            String preserved = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, ((Whitelist)Whitelist.BasicWithImages()).
                AddProtocols("img", "src", "cid", "data"));
            NUnit.Framework.Assert.AreEqual("<img src=\"cid:12345\"> \n<img src=\"data:gzzt\">", preserved);
        }

        [NUnit.Framework.Test]
        public virtual void HandlesFramesets() {
            String dirty = "<html><head><script></script><noscript></noscript></head><frameset><frame src=\"foo\" /><frame src=\"foo\" /></frameset></html>";
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(dirty, ((Whitelist)Whitelist.Basic()));
            NUnit.Framework.Assert.AreEqual("", clean);
            // nothing good can come out of that
            Document dirtyDoc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(dirty);
            Document cleanDoc = new Cleaner(((Whitelist)Whitelist.Basic())).Clean(dirtyDoc);
            NUnit.Framework.Assert.IsNotNull(cleanDoc);
            NUnit.Framework.Assert.AreEqual(0, cleanDoc.Body().ChildNodeSize());
        }

        [NUnit.Framework.Test]
        public virtual void HandlesCleanerFromWhitelist() {
            Cleaner cleaner = new Cleaner(((Whitelist)Whitelist.Basic()));
            Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse("<script>Script</script><p>Text</p>");
            Document clean = cleaner.Clean(doc);
            NUnit.Framework.Assert.AreEqual("<p>Text</p>", clean.Body().Html());
        }

        [NUnit.Framework.Test]
        public virtual void SupplyOutputSettings() {
            // test that one can override the default document output settings
            OutputSettings os = new OutputSettings();
            os.PrettyPrint(false);
            os.EscapeMode(Entities.EscapeMode.extended);
            os.Charset("ascii");
            String html = "<div><p>&bernou;</p></div>";
            String customOut = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", ((Whitelist)Whitelist.
                Relaxed()), os);
            String defaultOut = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", ((Whitelist)Whitelist
                .Relaxed()));
            NUnit.Framework.Assert.AreNotSame(defaultOut, customOut);
            NUnit.Framework.Assert.AreEqual("<div><p>&Bscr;</p></div>", customOut);
            // entities now prefers shorted names if aliased
            NUnit.Framework.Assert.AreEqual("<div>\n" + " <p>ℬ</p>\n" + "</div>", defaultOut);
            os.Charset("ASCII");
            os.EscapeMode(Entities.EscapeMode.@base);
            String customOut2 = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, "http://foo.com/", ((Whitelist)Whitelist
                .Relaxed()), os);
            NUnit.Framework.Assert.AreEqual("<div><p>&#x212c;</p></div>", customOut2);
        }
    }
}
