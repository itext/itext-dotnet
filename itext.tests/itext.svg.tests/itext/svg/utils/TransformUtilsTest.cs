using iText.IO.Util;
using iText.Kernel.Geom;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Exceptions;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class TransformUtilsTest {
        [NUnit.Framework.Test]
        public virtual void NullStringTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform(null);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_NULL));
;
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_EMPTY));
;
        }

        [NUnit.Framework.Test]
        public virtual void NoTransformationTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("Lorem ipsum");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.INVALID_TRANSFORM_DECLARATION));
;
        }

        [NUnit.Framework.Test]
        public virtual void WrongTypeOfValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("matrix(a b c d e f)");
            }
            , NUnit.Framework.Throws.TypeOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(LogMessageConstant.NAN, "a")));
;
        }

        [NUnit.Framework.Test]
        public virtual void TooManyParenthesesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("(((())))");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.INVALID_TRANSFORM_DECLARATION));
;
        }

        [NUnit.Framework.Test]
        public virtual void NoClosingParenthesisTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(0 0 0 0 0 0");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void MixedCaseTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("maTRix(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void UpperCaseTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("MATRIX(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void WhitespaceTest() {
            AffineTransform expected = new AffineTransform(0d, 0d, 0d, 0d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(0 0 0 0 0 0)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CommasWithWhitespaceTest() {
            AffineTransform expected = new AffineTransform(7.5d, 15d, 22.5d, 30d, 37.5d, 45d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(10, 20, 30, 40, 50, 60)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CommasTest() {
            AffineTransform expected = new AffineTransform(7.5d, 15d, 22.5d, 30d, 37.5d, 45d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(10,20,30,40,50,60)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CombinedTransformTest() {
            AffineTransform actual = TransformUtils.ParseTransform("translate(40,20) scale(3)");
            AffineTransform expected = new AffineTransform(2.25d, 0d, 0d, 2.25d, 30d, 15d);
            NUnit.Framework.Assert.AreEqual(actual, expected);
        }

        [NUnit.Framework.Test]
        public virtual void CombinedReverseTransformTest() {
            AffineTransform actual = TransformUtils.ParseTransform("scale(3) translate(40,20)");
            AffineTransform expected = new AffineTransform(2.25d, 0d, 0d, 2.25d, 67.5d, 33.75d);
            NUnit.Framework.Assert.AreEqual(actual, expected);
        }

        [NUnit.Framework.Test]
        public virtual void DoubleTransformationTest() {
            AffineTransform expected = new AffineTransform(1040.0625d, 0d, 0d, 1040.0625d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(43) scale(43)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void OppositeTransformationSequenceTest() {
            AffineTransform expected = new AffineTransform(1, 0, 0, 1, 0, 0);
            AffineTransform actual = TransformUtils.ParseTransform("translate(10 10) translate(-10 -10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void UnknownTransformationTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("unknown(1 2 3)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.UNKNOWN_TRANSFORMATION_TYPE));
;
        }
    }
}
