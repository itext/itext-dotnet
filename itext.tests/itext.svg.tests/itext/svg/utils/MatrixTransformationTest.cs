using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class MatrixTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalMatrixTest() {
            AffineTransform expected = new AffineTransform(7.5d, 15d, 22.5d, 30d, 37.5d, 45d);
            AffineTransform actual = TransformUtils.ParseTransform("matrix(10 20 30 40 50 60)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoMatrixValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("matrix()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void NotEnoughMatrixValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("matrix(0)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void TooManyMatrixValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("matrix(1 2 3 4 5 6 7 8)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }
    }
}
