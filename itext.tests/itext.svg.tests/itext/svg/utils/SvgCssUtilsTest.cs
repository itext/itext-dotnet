using System;
using System.Collections.Generic;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class SvgCssUtilsTest {
        [NUnit.Framework.Test]
        public virtual void NormalParseFloat() {
            float? expected = 3.0f;
            float? actual = SvgCssUtils.ParseFloat("3.0");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeFloatParsing() {
            float? expected = -3.0f;
            float? actual = SvgCssUtils.ParseFloat("-3.0");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TextFloatParsing() {
            NUnit.Framework.Assert.That(() =>  {
                SvgCssUtils.ParseFloat("Definitely not a float.");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.FLOAT_PARSING_NAN));
;
        }

        [NUnit.Framework.Test]
        public virtual void DoubleFloatParsing() {
            float? expected = 2.0f;
            float? actual = SvgCssUtils.ParseFloat("2.0d");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedTextFloatParsing() {
            NUnit.Framework.Assert.That(() =>  {
                SvgCssUtils.ParseFloat("15.0WaitWhat?30.0f");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.FLOAT_PARSING_NAN));
;
        }

        [NUnit.Framework.Test]
        public virtual void NullFloatParsing() {
            NUnit.Framework.Assert.That(() =>  {
                SvgCssUtils.ParseFloat(null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.FLOAT_PARSING_NAN));
;
        }

        [NUnit.Framework.Test]
        public virtual void CommaSplitValueTest() {
            String input = "a,b,c,d";
            IList<String> expected = new List<String>();
            expected.Add("a");
            expected.Add("b");
            expected.Add("c");
            expected.Add("d");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceSplitValueTest() {
            String input = "1 2 3 4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NewLineSplitValueTest() {
            String input = "1\n2\n3\n4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TabSplitValueTest() {
            String input = "1\t2\t3\t4";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("3");
            expected.Add("4");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedCommaWhitespaceSplitValueTest() {
            String input = "1,2 a,b";
            IList<String> expected = new List<String>();
            expected.Add("1");
            expected.Add("2");
            expected.Add("a");
            expected.Add("b");
            IList<String> actual = SvgCssUtils.SplitValueList(input);
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NullSplitValueTest() {
            IList<String> actual = SvgCssUtils.SplitValueList(null);
            NUnit.Framework.Assert.IsTrue(actual.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void EmptySplitValueTest() {
            IList<String> actual = SvgCssUtils.SplitValueList("");
            NUnit.Framework.Assert.IsTrue(actual.IsEmpty());
        }
    }
}
