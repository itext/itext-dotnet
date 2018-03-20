using System;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class RotateTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalRotateTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10f), 20f, 30f);
            AffineTransform actual = TransformUtils.ParseTransform("rotate(10, 20, 30)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NoRotateValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("rotate()");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void OneRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10d));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(10)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TwoRotateValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("rotate(23,58)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void ThreeRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(23d), 58d, 57d);
            AffineTransform actual = TransformUtils.ParseTransform("rotate(23,58, 57)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void TooManyRotateValuesTest() {
            NUnit.Framework.Assert.That(() =>  {
                TransformUtils.ParseTransform("rotate(1 2 3 4)");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.TRANSFORM_INCORRECT_NUMBER_OF_VALUES));
;
        }

        [NUnit.Framework.Test]
        public virtual void NegativeRotateValuesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(-23d), -58d, -1d);
            AffineTransform actual = TransformUtils.ParseTransform("rotate(-23,-58,-1)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void NinetyDegreesTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(90));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(90)");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
