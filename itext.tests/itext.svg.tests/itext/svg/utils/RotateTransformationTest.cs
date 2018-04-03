using System;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css.Util;
using iText.Svg.Exceptions;

namespace iText.Svg.Utils {
    public class RotateTransformationTest {
        [NUnit.Framework.Test]
        public virtual void NormalRotateTest() {
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10), CssUtils.ParseAbsoluteLength
                ("5"), CssUtils.ParseAbsoluteLength("10"));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(10, 5, 10)");
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
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(10));
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
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(23), CssUtils.ParseAbsoluteLength
                ("58"), CssUtils.ParseAbsoluteLength("57"));
            AffineTransform actual = TransformUtils.ParseTransform("rotate(23, 58, 57)");
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
            AffineTransform expected = AffineTransform.GetRotateInstance(MathUtil.ToRadians(-23), CssUtils.ParseAbsoluteLength
                ("-58"), CssUtils.ParseAbsoluteLength("-1"));
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
