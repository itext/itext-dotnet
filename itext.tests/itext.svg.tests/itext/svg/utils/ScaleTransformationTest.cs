using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class ScaleTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalScaleTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(7.5d, 15d);
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
            AffineTransform expected = AffineTransform.GetScaleInstance(7.5d, 7.5d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoScaleValuesTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(17.25d, 43.5d);
            AffineTransform actual = TransformUtils.ParseTransform("scale(23,58)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NegativeScaleValuesTest() {
            AffineTransform expected = AffineTransform.GetScaleInstance(-7.5d, -37.5d);
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
