/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Tag tests.</summary>
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
