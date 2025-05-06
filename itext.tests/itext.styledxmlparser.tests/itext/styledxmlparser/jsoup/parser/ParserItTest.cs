/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Text;
using NUnit.Framework;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Longer running Parser tests.</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class ParserItTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestIssue1251() {
            // https://github.com/jhy/jsoup/issues/1251
            String testString = "<a href=\"\"ca";
            StringBuilder str = new StringBuilder();
            // initial max length of the buffer is 2**15 * 0.75 = 24576
            int spacesToReproduceIssue = 24577 - testString.Length;
            for (int i = 0; i < spacesToReproduceIssue; i++) {
                str.Append(" ");
            }
            str.Append(testString);
            try {
                iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser().SetTrackErrors(1).ParseInput(str.ToString(), "");
            }
            catch (Exception e) {
                throw new AssertionException("failed at length " + str.Length, e);
            }
        }

        [NUnit.Framework.Test]
        public virtual void HandlesDeepStack() {
            // inspired by http://sv.stargate.wikia.com/wiki/M2J and https://github.com/jhy/jsoup/issues/955
            // I didn't put it in the integration tests, because explorer and intellij kept dieing trying to preview/index it
            // Arrange
            StringBuilder longBody = new StringBuilder(500000);
            for (int i = 0; i < 25000; i++) {
                longBody.Append(i).Append("<dl><dd>");
            }
            for (int i = 0; i < 25000; i++) {
                longBody.Append(i).Append("</dd></dl>");
            }
            // Act
            long start = System.DateTime.Now.Ticks;
            Document doc = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseBodyFragment(longBody.ToString(), "");
            // Assert
            NUnit.Framework.Assert.AreEqual(2, doc.Body().ChildNodeSize());
            NUnit.Framework.Assert.AreEqual(25000, doc.Select("dd").Count);
            NUnit.Framework.Assert.IsTrue((System.DateTime.Now.Ticks - start) / 1000000 < 20000);
        }
        // I get ~ 1.5 seconds, but others have reported slower
        // was originally much longer, or stack overflow.
    }
}
