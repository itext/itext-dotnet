using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class TranslateTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalTranslateTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 20d, 50d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(20, 50)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoTranslateValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("translate()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void OneTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 10d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, 23d, 58d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(23,58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeTranslateValuesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, 0d, 1d, -23d, -58d);
            AffineTransform actual = TransformUtils.ParseTransform("translate(-23,-58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TooManyTranslateValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("translate(1 2 3)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }
    }
}
