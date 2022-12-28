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
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tag tests.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    [NUnit.Framework.Category("UnitTest")]
    public class TagTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void IsCaseSensitive() {
            iText.StyledXmlParser.Jsoup.Parser.Tag p1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("P");
            iText.StyledXmlParser.Jsoup.Parser.Tag p2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("p");
            NUnit.Framework.Assert.AreNotEqual(p1, p2);
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
            NUnit.Framework.Assert.AreEqual(p1, p2);
            NUnit.Framework.Assert.AreSame(p1, p2);
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
            iText.StyledXmlParser.Jsoup.Parser.Tag foo = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("FOO");
            // not defined
            iText.StyledXmlParser.Jsoup.Parser.Tag foo2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("FOO");
            NUnit.Framework.Assert.AreEqual(foo, foo2);
            NUnit.Framework.Assert.IsTrue(foo.IsInline());
            NUnit.Framework.Assert.IsTrue(foo.FormatAsBlock());
        }

        [NUnit.Framework.Test]
        public virtual void ValueOfChecksNotNull() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf
                (null));
        }

        [NUnit.Framework.Test]
        public virtual void ValueOfChecksNotEmpty() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf
                (" "));
        }

        [NUnit.Framework.Test]
        public virtual void KnownTags() {
            NUnit.Framework.Assert.IsTrue(iText.StyledXmlParser.Jsoup.Parser.Tag.IsKnownTag("div"));
            NUnit.Framework.Assert.IsFalse(iText.StyledXmlParser.Jsoup.Parser.Tag.IsKnownTag("explain"));
        }
    }
}
