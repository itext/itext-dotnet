/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>Tests for Attributes.</summary>
    /// <author>Jonathan Hedley</author>
    public class AttributesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Html() {
            Attributes a = new Attributes();
            a.Put("Tot", "a&p");
            a.Put("Hello", "There");
            a.Put("data-name", "Jsoup");
            NUnit.Framework.Assert.AreEqual(3, a.Size());
            NUnit.Framework.Assert.IsTrue(a.HasKey("tot"));
            NUnit.Framework.Assert.IsTrue(a.HasKey("Hello"));
            NUnit.Framework.Assert.IsTrue(a.HasKey("data-name"));
            NUnit.Framework.Assert.AreEqual(1, a.Dataset().Count);
            NUnit.Framework.Assert.AreEqual("Jsoup", a.Dataset().Get("name"));
            NUnit.Framework.Assert.AreEqual("a&p", a.Get("tot"));
            NUnit.Framework.Assert.AreEqual(" tot=\"a&amp;p\" hello=\"There\" data-name=\"Jsoup\"", a.Html());
            NUnit.Framework.Assert.AreEqual(a.Html(), a.ToString());
        }
    }
}
