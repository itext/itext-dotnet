using System;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class SkewYTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalSkewYTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(143d)), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(143)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoSkewYValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("skewY()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void TwoSkewYValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("skewY(1 2)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void NegativeSkewYTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(-26d)), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(-26)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(90d)), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewY(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
