using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class ScaleTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalScaleTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(10d, 20d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(10, 20)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoScaleValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("scale()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void OneScaleValuesTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(10d, 10d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoScaleValuesTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(23d, 58d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(23,58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeScaleValuesTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(-10, -50d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(-10, -50)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TooManyScaleValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("scale(1 2 3)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }
    }
}
