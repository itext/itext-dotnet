using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Css.Media {
    public class CssMediaRuleTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MatchMediaDeviceTest() {
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            deviceDescription.SetHeight(450);
            deviceDescription.SetWidth(600);
            CssMediaRule rule = new CssMediaRule("@media all and (min-width: 600px) and (min-height: 600px)");
            NUnit.Framework.Assert.IsTrue(rule.MatchMediaDevice(deviceDescription));
        }

        [NUnit.Framework.Test]
        public virtual void GetCssRuleSetsTest() {
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            String html = "<a id=\"123\" class=\"baz = 'bar'\" style = media= all and (min-width: 600px) />";
            IDocumentNode node = new JsoupHtmlParser().Parse(html);
            IList<CssRuleSet> ruleSets = new CssMediaRule("only all and (min-width: 600px) and (min-height: 600px)").GetCssRuleSets
                (node, deviceDescription);
            NUnit.Framework.Assert.IsNotNull(ruleSets);
        }
    }
}
