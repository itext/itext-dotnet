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
