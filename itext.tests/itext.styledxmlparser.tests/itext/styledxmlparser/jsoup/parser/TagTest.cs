/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
