/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Colors.Gradients;
using iText.StyledXmlParser.Exceptions;
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssGradientUtilTest : ExtendedITextTest {
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
        public virtual void RepeatingLinearGradientWithHexColorsAndUpperCaseTest() {
            String gradientValue = "rePEATing-linear-grADIENt(#ff0000, #008000, #0000ff)";
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
        public virtual void EmptyParsedArguments1Test() {
            String gradientValue = "linear-gradient()";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , "linear-gradient()"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyParsedArguments2Test() {
            String gradientValue = "linear-gradient( , )";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , "linear-gradient( , )"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFirstArgumentTest() {
            String gradientValue = "linear-gradient(not-angle-or-color, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "not-angle-or-color"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest0() {
            String gradientValue = "linear-gradient(to , orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "to"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest1() {
            String gradientValue = "linear-gradient(to, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "to"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest2() {
            String gradientValue = "linear-gradient(to left left, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                , "to left left"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest3() {
            String gradientValue = "linear-gradient(to bottom top, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                , "to bottom top"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest4() {
            String gradientValue = "linear-gradient(to left right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                , "to left right"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidToSideTest5() {
            String gradientValue = "linear-gradient(to top right right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                , "to top right right"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidColorWithThreeOffsetsValueTest() {
            String gradientValue = "linear-gradient(red, orange 20pt 30pt 100pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 20pt 30pt 100pt"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidColorOffsetValueTest() {
            String gradientValue = "linear-gradient(red, orange 20, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 20"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidMultipleHintsInARowValueTest() {
            String gradientValue = "linear-gradient(red, orange, 20%, 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "30%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidMultipleHintsInARowWithoutCommaValueTest() {
            String gradientValue = "linear-gradient(red, orange, 20% 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "20% 30%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidFirstElementIsAHintValueTest() {
            String gradientValue = "linear-gradient(5%, red, orange, 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "5%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidLastElementIsAHintValueTest() {
            String gradientValue = "linear-gradient(red, orange, 30%, green 200pt, blue 250pt, 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "120%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesLeftTest() {
            String gradientValue = "linear-gradient(to left, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesRightTest() {
            String gradientValue = "linear-gradient(to right, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesBottomTest() {
            String gradientValue = "linear-gradient(to bottom, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesTopTest() {
            String gradientValue = "linear-gradient(to top, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToLeftTopTest() {
            String gradientValue = "linear-gradient(to left top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToTopLeftTest() {
            String gradientValue = "linear-gradient(to top left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToLeftBottomTest() {
            String gradientValue = "linear-gradient(to left bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToBottomLeftTest() {
            String gradientValue = "linear-gradient(to bottom left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToRightTopTest() {
            String gradientValue = "linear-gradient(to right top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToTopRightTest() {
            String gradientValue = "linear-gradient(to top right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToRightBottomTest() {
            String gradientValue = "linear-gradient(to right bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToBottomRightTest() {
            String gradientValue = "linear-gradient(to bottom right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegPositiveTest() {
            String gradientValue = "linear-gradient(41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegNegativeTest() {
            String gradientValue = "linear-gradient(-41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegZeroTest() {
            String gradientValue = "linear-gradient(0deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 0 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadPositiveTest() {
            String gradientValue = "linear-gradient(0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadNegativeTest() {
            String gradientValue = "linear-gradient(-0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadZeroTest() {
            String gradientValue = "linear-gradient(0rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradPositiveTest() {
            String gradientValue = "linear-gradient(41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)-Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradNegativeTest() {
            String gradientValue = "linear-gradient(-41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradZeroTest() {
            String gradientValue = "linear-gradient(0grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 0 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentTurnPositiveTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(0.17turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "0.17turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentTurnNegativeTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(-0.17turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "-0.17turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentTurnZeroTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(0turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "0turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesLeftTest() {
            String gradientValue = "repeating-linear-gradient(to left, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesRightTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesBottomTest() {
            String gradientValue = "repeating-linear-gradient(to bottom, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesTopTest() {
            String gradientValue = "repeating-linear-gradient(to top, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToLeftTopTest() {
            String gradientValue = "repeating-linear-gradient(to left top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToTopLeftTest() {
            String gradientValue = "repeating-linear-gradient(to top left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToLeftBottomTest() {
            String gradientValue = "repeating-linear-gradient(to left bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToBottomLeftTest() {
            String gradientValue = "repeating-linear-gradient(to bottom left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToRightTopTest() {
            String gradientValue = "repeating-linear-gradient(to right top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToTopRightTest() {
            String gradientValue = "repeating-linear-gradient(to top right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToRightBottomTest() {
            String gradientValue = "repeating-linear-gradient(to right bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToBottomRightTest() {
            String gradientValue = "repeating-linear-gradient(to bottom right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegPositiveTest() {
            String gradientValue = "repeating-linear-gradient(41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegZeroTest() {
            String gradientValue = "repeating-linear-gradient(0deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 0 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadPositiveTest() {
            String gradientValue = "repeating-linear-gradient(0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadZeroTest() {
            String gradientValue = "repeating-linear-gradient(0rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradPositiveTest() {
            String gradientValue = "repeating-linear-gradient(41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)-Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradZeroTest() {
            String gradientValue = "repeating-linear-gradient(0grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 0 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentTurnPositiveTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(0.17turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "0.17turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentTurnNegativeTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(-0.17turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "-0.17turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentTurnZeroTest() {
            // TODO: DEVSIX-3595. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(0turn, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "0turn"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorNameTest() {
            String gradientValue = "linear-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorHexTest() {
            String gradientValue = "linear-gradient(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorRGBTest() {
            String gradientValue = "linear-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorRGBaTest() {
            String gradientValue = "linear-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopTest() {
            String gradientValue = "linear-gradient(to left top, red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopRGBTest() {
            String gradientValue = "linear-gradient(to left top, red, rgb(0, 127, 0), blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopRGBOffsetsHintsTest() {
            String gradientValue = "linear-gradient(to left top, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType.RELATIVE
                ).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorNameTest() {
            String gradientValue = "repeating-linear-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorHexTest() {
            String gradientValue = "repeating-linear-gradient(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorRGBTest() {
            String gradientValue = "repeating-linear-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorRGBaTest() {
            String gradientValue = "repeating-linear-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopRGBTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red, rgb(0, 127, 0), blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopRGBOffsetsHintsTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType.RELATIVE
                ).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteCMTest() {
            String gradientValue = "linear-gradient(to right, orange 3cm, red 3cm, green 9cm, blue 9cm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "linear-gradient(to right, orange 3mm, red 3mm, green 9mm, blue 9mm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "linear-gradient(to right, orange 30Q, red 30Q, green 90Q, blue 90Q)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "linear-gradient(to right, orange 1in, red 1in, green 3in, blue 3in)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "linear-gradient(to right, orange 10pc, red 10pc, green 30pc, blue 30pc)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "linear-gradient(to right, orange 100pt, red 100pt, green 300pt, blue 300pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "linear-gradient(to right, orange 100px, red 100px, green 300px, blue 300px)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "linear-gradient(to right, orange 3em, red 3em, green 9em, blue 9em)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "linear-gradient(to right, orange 3rem, red 3rem, green 9rem, blue 9rem)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "linear-gradient(to right, orange 3ex, red 3ex, green 9ex, blue 9ex)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedChTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(to right, orange 3ch, red 3ch, green 9ch, blue 9ch)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3ch"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedVhTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(to right, orange 3vh, red 3vh, green 9vh, blue 9vh)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vh"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsViewPortVwTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(to right, orange 3vw, red 3vw, green 9vw, blue 9vw)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vw"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsViewPortVminTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(to right, orange 3vmin, red 3vmin, green 9vmin, blue 9vmin)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vmin"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsViewPortVmaxTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "linear-gradient(to right, orange 3vmax, red 3vmax, green 9vmax, blue 9vmax)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vmax"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteCMTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3cm, red 3cm, green 9cm, blue 9cm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3mm, red 3mm, green 9mm, blue 9mm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 30Q, red 30Q, green 90Q, blue 90Q)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 1in, red 1in, green 3in, blue 3in)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10pc, red 10pc, green 30pc, blue 30pc)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100pt, red 100pt, green 300pt, blue 300pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100px, red 100px, green 300px, blue 300px)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3em, red 3em, green 9em, blue 9em)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3rem, red 3rem, green 9rem, blue 9rem)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3ex, red 3ex, green 9ex, blue 9ex)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedChTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(to right, orange 3ch, red 3ch, green 9ch, blue 9ch)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3ch"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedVhTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(to right, orange 3vh, red 3vh, green 9vh, blue 9vh)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vh"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsViewPortVwTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(to right, orange 3vw, red 3vw, green 9vw, blue 9vw)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vw"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsViewPortVminTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(to right, orange 3vmin, red 3vmin, green 9vmin, blue 9vmin)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vmin"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsViewPortVmaxTest() {
            // TODO: DEVSIX-3596. Remove Exception expectation after fix and update the logic of the test similar to the already existed tests logic
            String gradientValue = "repeating-linear-gradient(to right, orange 3vmax, red 3vmax, green 9vmax, blue 9vmax)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                (gradientValue, 12, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 3vmax"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetStartEndInsideTest() {
            String gradientValue = "linear-gradient(to right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetStartEndOutTest() {
            String gradientValue = "linear-gradient(to right, orange -100pt, red 150pt, green 200pt, blue 750pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "linear-gradient(to right, orange, red -20%, green 120%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "linear-gradient(to right, orange, red 300pt, green, blue 80%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "linear-gradient(to right, orange, red 10%, lime, 80%, blue 80.5%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO).SetHint
                (0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "linear-gradient(to right, orange 10%, red, lime, 40%, blue, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO).SetHint
                (0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetSmallHintTest() {
            String gradientValue = "linear-gradient(to right, orange, 1%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetNegativeHintTest() {
            String gradientValue = "linear-gradient(to right, orange, -100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetHintTest() {
            String gradientValue = "linear-gradient(to right, orange, 100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMultipleOffsetsTest() {
            String gradientValue = "linear-gradient(to right, orange 10%, blue 60% 70%, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetStartEndInsideTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetStartEndOutTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange -100pt, red 150pt, green 200pt, blue 750pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType.ABSOLUTE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red -20%, green 120%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red 300pt, green, blue 80%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType.ABSOLUTE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red 10%, lime, 80%, blue 80.5%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO).SetHint
                (0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE)
                );
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10%, red, lime, 40%, blue, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO).SetHint
                (0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetSmallHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, 1%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetNegativeHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, -100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, 100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType.
                RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffMultipleOffsetsTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10%, blue 60% 70%, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> colorStops = new List<GradientColorStop>();
            colorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType.RELATIVE
                ));
            colorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, colorStops);
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
