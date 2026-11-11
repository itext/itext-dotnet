/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    public class AttributeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Html() {
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute("key", 
                "value &");
            NUnit.Framework.Assert.AreEqual("key=\"value &amp;\"", attr.Html());
            NUnit.Framework.Assert.AreEqual(attr.Html(), attr.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void TestWithSupplementaryCharacterInAttributeKeyAndValue() {
            String s = new String(iText.IO.Util.TextUtil.ToChars(135361));
            iText.StyledXmlParser.Jsoup.Nodes.Attribute attr = new iText.StyledXmlParser.Jsoup.Nodes.Attribute(s, "A" 
                + s + "B");
            NUnit.Framework.Assert.AreEqual(s + "=\"A" + s + "B\"", attr.Html());
            NUnit.Framework.Assert.AreEqual(attr.Html(), attr.ToString());
        }
    }
}
