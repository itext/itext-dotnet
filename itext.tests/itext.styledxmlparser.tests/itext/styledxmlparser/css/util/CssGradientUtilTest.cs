using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Colors.Gradients;
using iText.StyledXmlParser.Exceptions;
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    public class CssGradientUtilTest : ExtendedITextTest {
        // TODO: DEVSIX-4105
        //  1. add tests from background-image-angles-linear-gradient.html
        //  2. add tests from background-image-metrics-linear-gradient.html
        //  3. add tests from background-image-offsets-linear-gradient.html
        //  4. for points 1-3 the same tests for repeating gradient
        [NUnit.Framework.Test]
        public virtual void NullValueTest() {
            String gradientValue = null;
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void WebkitExtensionLinearGradientTest() {
            String gradientValue = "-webkit-linear-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientWithNamesTest() {
            String gradientValue = "  linear-gradient(red, green, blue) \t ";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientWithHexColorsTest() {
            String gradientValue = "linear-grADIENt(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientWithRgbFunctionsTest() {
            String gradientValue = "linear-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithNamesTest() {
            String gradientValue = "  repeating-linear-gradient(red, green, blue) \t ";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithHexColorsTest() {
            String gradientValue = "repeating-linear-grADIENt(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithRgbFunctionsTest() {
            String gradientValue = "repeating-linear-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFirstArgumentTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(not-angle-or-color, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "not-angle-or-color")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest0() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to , orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "to")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest1() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "to")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest2() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to left left, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING, "to left left")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest3() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to bottom top, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING, "to bottom top")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest4() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to left right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING, "to left right")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest5() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(to top right right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING, "to top right right")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidColorWithThreeOffsetsValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(red, orange 20pt 30pt 100pt, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "orange 20pt 30pt 100pt")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidColorOffsetValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(red, orange 20, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "orange 20")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidMultipleHintsInARowValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(red, orange, 20%, 30%, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "30%")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidMultipleHintsInARowWithoutCommaValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(red, orange, 20% 30%, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "20% 30%")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFirstElementIsAHintValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(5%, red, orange, 30%, green 200pt, blue 250pt)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "5%")))
;
        }

        [NUnit.Framework.Test]
        public virtual void InvalidLastElementIsAHintValueTest() {
            NUnit.Framework.Assert.That(() =>  {
                String gradientValue = "linear-gradient(red, orange, 30%, green 200pt, blue 250pt, 120%)";
                NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
                CssGradientUtil.ParseCssLinearGradient(gradientValue, 24, 12);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE, "120%")))
;
        }

        private void AssertStrategyBasedBuilderEquals(AbstractLinearGradientBuilder gradientBuilder, bool isCentralRotationStrategy
            , double rotateVectorAngle, StrategyBasedLinearGradientBuilder.GradientStrategy gradientStrategy, GradientSpreadMethod
             spreadMethod, IList<GradientColorStop> stops) {
            NUnit.Framework.Assert.IsTrue(gradientBuilder is StrategyBasedLinearGradientBuilder);
            StrategyBasedLinearGradientBuilder builder = (StrategyBasedLinearGradientBuilder)gradientBuilder;
            NUnit.Framework.Assert.AreEqual(isCentralRotationStrategy, builder.IsCentralRotationAngleStrategy());
            NUnit.Framework.Assert.AreEqual(rotateVectorAngle, builder.GetRotateVectorAngle(), 1e-10);
            NUnit.Framework.Assert.AreEqual(gradientStrategy, builder.GetGradientStrategy());
            NUnit.Framework.Assert.AreEqual(spreadMethod, builder.GetSpreadMethod());
            IList<GradientColorStop> actualStops = builder.GetColorStops();
            NUnit.Framework.Assert.AreEqual(stops.Count, actualStops.Count);
            for (int i = 0; i < stops.Count; ++i) {
                NUnit.Framework.Assert.AreEqual(stops[i], actualStops[i]);
            }
        }
    }
}
