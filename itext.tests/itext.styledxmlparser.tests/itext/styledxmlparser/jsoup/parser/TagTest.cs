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

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tag tests.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class TagTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IsCaseInsensitive() {
            iText.StyledXmlParser.Jsoup.Parser.Tag p1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("P");
            iText.StyledXmlParser.Jsoup.Parser.Tag p2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            NUnit.Framework.Assert.AreEqual(p1, p2);
        }

        [NUnit.Framework.Test]
        public virtual void Trims() {
            iText.StyledXmlParser.Jsoup.Parser.Tag p1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            iText.StyledXmlParser.Jsoup.Parser.Tag p2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(" p ");
            NUnit.Framework.Assert.AreEqual(p1, p2);
        }

        [NUnit.Framework.Test]
        public virtual void Equality() {
            iText.StyledXmlParser.Jsoup.Parser.Tag p1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            iText.StyledXmlParser.Jsoup.Parser.Tag p2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            NUnit.Framework.Assert.IsTrue(p1.Equals(p2));
            NUnit.Framework.Assert.IsTrue(p1 == p2);
        }

        [NUnit.Framework.Test]
        public virtual void DivSemantics() {
            iText.StyledXmlParser.Jsoup.Parser.Tag div = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("div");
            NUnit.Framework.Assert.IsTrue(div.IsBlock());
            NUnit.Framework.Assert.IsTrue(div.FormatAsBlock());
        }

        [NUnit.Framework.Test]
        public virtual void PSemantics() {
            iText.StyledXmlParser.Jsoup.Parser.Tag p = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            NUnit.Framework.Assert.IsTrue(p.IsBlock());
            NUnit.Framework.Assert.IsFalse(p.FormatAsBlock());
        }

        [NUnit.Framework.Test]
        public virtual void ImgSemantics() {
            iText.StyledXmlParser.Jsoup.Parser.Tag img = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("img");
            NUnit.Framework.Assert.IsTrue(img.IsInline());
            NUnit.Framework.Assert.IsTrue(img.IsSelfClosing());
            NUnit.Framework.Assert.IsFalse(img.IsBlock());
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSemantics() {
            iText.StyledXmlParser.Jsoup.Parser.Tag foo = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("foo");
            // not defined
            iText.StyledXmlParser.Jsoup.Parser.Tag foo2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("FOO");
            NUnit.Framework.Assert.AreEqual(foo, foo2);
            NUnit.Framework.Assert.IsTrue(foo.IsInline());
            NUnit.Framework.Assert.IsTrue(foo.FormatAsBlock());
        }
    }
}
