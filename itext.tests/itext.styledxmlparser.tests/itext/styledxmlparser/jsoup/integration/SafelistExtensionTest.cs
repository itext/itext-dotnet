/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Safety;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Integration {
    /// <summary>Check that we can extend Safelist methods</summary>
    [NUnit.Framework.Category("IntegrationTest")]
    public class SafelistExtensionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CanCustomizeSafeTests() {
            SafelistExtensionTest.OpenSafelist openSafelist = new SafelistExtensionTest.OpenSafelist(Safelist.Relaxed(
                ));
            Safelist safelist = Safelist.Relaxed();
            String html = "<p><opentag openattr>Hello</opentag></p>";
            String openClean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, openSafelist);
            String clean = iText.StyledXmlParser.Jsoup.Jsoup.Clean(html, safelist);
            NUnit.Framework.Assert.AreEqual("<p><opentag openattr=\"\">Hello</opentag></p>", TextUtil.StripNewlines(openClean
                ));
            NUnit.Framework.Assert.AreEqual("<p>Hello</p>", clean);
        }

        // passes tags and attributes starting with "open"
        private class OpenSafelist : Safelist {
            public OpenSafelist(Safelist safelist)
                : base(safelist) {
            }

            protected internal override bool IsSafeAttribute(String tagName, iText.StyledXmlParser.Jsoup.Nodes.Element
                 el, iText.StyledXmlParser.Jsoup.Nodes.Attribute attr) {
                if (attr.Key.StartsWith("open")) {
                    return true;
                }
                return base.IsSafeAttribute(tagName, el, attr);
            }

            protected internal override bool IsSafeTag(String tag) {
                if (tag.StartsWith("open")) {
                    return true;
                }
                return base.IsSafeTag(tag);
            }
        }
    }
}
