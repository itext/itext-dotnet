/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class ParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnescapeEntities() {
            String s = iText.StyledXmlParser.Jsoup.Parser.Parser.UnescapeEntities("One &amp; Two", false);
            NUnit.Framework.Assert.AreEqual("One & Two", s);
        }

        [NUnit.Framework.Test]
        public virtual void UnescapeEntitiesHandlesLargeInput() {
            StringBuilder longBody = new StringBuilder(500000);
            do {
                longBody.Append("SomeNonEncodedInput");
            }
            while (longBody.Length < 64 * 1024);
            String body = longBody.ToString();
            NUnit.Framework.Assert.AreEqual(body, iText.StyledXmlParser.Jsoup.Parser.Parser.UnescapeEntities(body, false
                ));
        }
    }
}
