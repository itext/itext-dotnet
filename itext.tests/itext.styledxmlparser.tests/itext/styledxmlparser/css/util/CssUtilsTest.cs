using System;
using iText.IO.Util;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
    public class CssUtilsTest {
        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNAN() {
            NUnit.Framework.Assert.That(() =>  {
                String value = "Definitely not a number";
                CssUtils.ParseAbsoluteLength(value);
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "Definitely not a number")));
;
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNull() {
            NUnit.Framework.Assert.That(() =>  {
                String value = null;
                CssUtils.ParseAbsoluteLength(value);
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "null")));
;
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10px() {
            String value = "10px";
            float actual = CssUtils.ParseAbsoluteLength(value, CssConstants.PX);
            float expected = 7.5f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10cm() {
            String value = "10cm";
            float actual = CssUtils.ParseAbsoluteLength(value, CssConstants.CM);
            float expected = 283.46457f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10in() {
            String value = "10in";
            float actual = CssUtils.ParseAbsoluteLength(value, CssConstants.IN);
            float expected = 720.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10pt() {
            String value = "10pt";
            float actual = CssUtils.ParseAbsoluteLength(value, CssConstants.PT);
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromUnknownType() {
            String value = "10pateekes";
            float actual = CssUtils.ParseAbsoluteLength(value, "pateekes");
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }
    }
}
