/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
