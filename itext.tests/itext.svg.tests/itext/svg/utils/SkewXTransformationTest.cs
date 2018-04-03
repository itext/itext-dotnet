using System;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class SkewXTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalSkewXTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(SvgCssUtils.ParseFloat(
                "143"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(143)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoSkewXValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("skewX()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void TwoSkewXValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("skewX(1 2)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void NegativeSkewXTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(SvgCssUtils.ParseFloat(
                "-26"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(-26)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = new AffineTransform(1d, 0d, Math.Tan(MathUtil.ToRadians(SvgCssUtils.ParseFloat(
                "90"))), 1d, 0d, 0d);
            AffineTransform actual = TransformUtils.ParseTransform("skewX(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
