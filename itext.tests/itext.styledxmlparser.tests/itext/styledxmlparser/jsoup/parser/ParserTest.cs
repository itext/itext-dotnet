/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
