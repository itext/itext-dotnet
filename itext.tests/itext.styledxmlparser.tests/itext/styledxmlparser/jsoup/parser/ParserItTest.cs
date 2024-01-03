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
