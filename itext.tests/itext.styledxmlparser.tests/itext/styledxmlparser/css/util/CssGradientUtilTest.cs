/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Test.Attributes;

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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientWithHexColorsTest() {
            String gradientValue = "linear-grADIENt(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientWithRgbFunctionsTest() {
            String gradientValue = "linear-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithNamesTest() {
            String gradientValue = "  repeating-linear-gradient(red, green, blue) \t ";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithHexColorsAndUpperCaseTest() {
            String gradientValue = "rePEATing-linear-grADIENt(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientWithRgbFunctionsTest() {
            String gradientValue = "repeating-linear-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
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
        public virtual void EmptyCommaSeparatedLinearGradientArgumentsTest() {
            String[] invalidGradientValues = new String[] { "linear-gradient(, red, blue)", "linear-gradient(red,,blue)"
                , "linear-gradient(red, blue,)" };
            foreach (String gradientValue in invalidGradientValues) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssLinearGradient
                    (gradientValue, 24, 12));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , gradientValue), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EmptyCommaSeparatedRadialGradientArgumentsTest() {
            String[] invalidGradientValues = new String[] { "radial-gradient(, red, blue)", "radial-gradient(red,,blue)"
                , "radial-gradient(red, blue,)" };
            foreach (String gradientValue in invalidGradientValues) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                    (gradientValue, 24, 12));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , gradientValue), e.Message);
            }
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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesRightTest() {
            String gradientValue = "linear-gradient(to right, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesBottomTest() {
            String gradientValue = "linear-gradient(to bottom, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentSidesTopTest() {
            String gradientValue = "linear-gradient(to top, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToLeftTopTest() {
            String gradientValue = "linear-gradient(to left top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToTopLeftTest() {
            String gradientValue = "linear-gradient(to top left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToLeftBottomTest() {
            String gradientValue = "linear-gradient(to left bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToBottomLeftTest() {
            String gradientValue = "linear-gradient(to bottom left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToRightTopTest() {
            String gradientValue = "linear-gradient(to right top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToTopRightTest() {
            String gradientValue = "linear-gradient(to top right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToRightBottomTest() {
            String gradientValue = "linear-gradient(to right bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentCornersToBottomRightTest() {
            String gradientValue = "linear-gradient(to bottom right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegPositiveTest() {
            String gradientValue = "linear-gradient(41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegNegativeTest() {
            String gradientValue = "linear-gradient(-41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentDegZeroTest() {
            String gradientValue = "linear-gradient(0deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 0 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadPositiveTest() {
            String gradientValue = "linear-gradient(0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadNegativeTest() {
            String gradientValue = "linear-gradient(-0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentRadZeroTest() {
            String gradientValue = "linear-gradient(0rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradPositiveTest() {
            String gradientValue = "linear-gradient(41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)-Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradNegativeTest() {
            String gradientValue = "linear-gradient(-41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDifferentGradZeroTest() {
            String gradientValue = "linear-gradient(0grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 0 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesRightTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesBottomTest() {
            String gradientValue = "repeating-linear-gradient(to bottom, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentSidesTopTest() {
            String gradientValue = "repeating-linear-gradient(to top, orange -20pt, red 0%, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToLeftTopTest() {
            String gradientValue = "repeating-linear-gradient(to left top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToTopLeftTest() {
            String gradientValue = "repeating-linear-gradient(to top left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToLeftBottomTest() {
            String gradientValue = "repeating-linear-gradient(to left bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToBottomLeftTest() {
            String gradientValue = "repeating-linear-gradient(to bottom left, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToRightTopTest() {
            String gradientValue = "repeating-linear-gradient(to right top, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToTopRightTest() {
            String gradientValue = "repeating-linear-gradient(to top right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToRightBottomTest() {
            String gradientValue = "repeating-linear-gradient(to right bottom, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentCornersToBottomRightTest() {
            String gradientValue = "repeating-linear-gradient(to bottom right, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegPositiveTest() {
            String gradientValue = "repeating-linear-gradient(41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-41deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 41 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentDegZeroTest() {
            String gradientValue = "repeating-linear-gradient(0deg, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, Math.PI * 0 / 180, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadPositiveTest() {
            String gradientValue = "repeating-linear-gradient(0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, -0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-0.5rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0.5, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentRadZeroTest() {
            String gradientValue = "repeating-linear-gradient(0rad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, 0, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradPositiveTest() {
            String gradientValue = "repeating-linear-gradient(41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)-Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradNegativeTest() {
            String gradientValue = "repeating-linear-gradient(-41grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 41 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDifferentGradZeroTest() {
            String gradientValue = "repeating-linear-gradient(0grad, orange -20pt, red 0%, green, blue 100%, orange 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -20f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, true, (float)Math.PI * 0 / 200, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorHexTest() {
            String gradientValue = "linear-gradient(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorRGBTest() {
            String gradientValue = "linear-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffColorRGBaTest() {
            String gradientValue = "linear-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopTest() {
            String gradientValue = "linear-gradient(to left top, red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopRGBTest() {
            String gradientValue = "linear-gradient(to left top, red, rgb(0, 127, 0), blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradComplexArgsLeftTopRGBOffsetsHintsTest() {
            String gradientValue = "linear-gradient(to left top, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorNameTest() {
            String gradientValue = "repeating-linear-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorHexTest() {
            String gradientValue = "repeating-linear-gradient(#ff0000, #008000, #0000ff)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorRGBTest() {
            String gradientValue = "repeating-linear-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffColorRGBaTest() {
            String gradientValue = "repeating-linear-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_BOTTOM, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopRGBTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red, rgb(0, 127, 0), blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradComplexArgsLeftTopRGBOffsetsHintsTest() {
            String gradientValue = "repeating-linear-gradient(to left top, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_TOP_LEFT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteCMTest() {
            String gradientValue = "linear-gradient(to right, orange 3cm, red 3cm, green 9cm, blue 9cm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "linear-gradient(to right, orange 3mm, red 3mm, green 9mm, blue 9mm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "linear-gradient(to right, orange 30Q, red 30Q, green 90Q, blue 90Q)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "linear-gradient(to right, orange 1in, red 1in, green 3in, blue 3in)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "linear-gradient(to right, orange 10pc, red 10pc, green 30pc, blue 30pc)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "linear-gradient(to right, orange 100pt, red 100pt, green 300pt, blue 300pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "linear-gradient(to right, orange 100px, red 100px, green 300px, blue 300px)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "linear-gradient(to right, orange 3em, red 3em, green 9em, blue 9em)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "linear-gradient(to right, orange 3rem, red 3rem, green 9rem, blue 9rem)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "linear-gradient(to right, orange 3ex, red 3ex, green 9ex, blue 9ex)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3mm, red 3mm, green 9mm, blue 9mm)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 30Q, red 30Q, green 90Q, blue 90Q)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 1in, red 1in, green 3in, blue 3in)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10pc, red 10pc, green 30pc, blue 30pc)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100pt, red 100pt, green 300pt, blue 300pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100px, red 100px, green 300px, blue 300px)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3em, red 3em, green 9em, blue 9em)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3rem, red 3rem, green 9rem, blue 9rem)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatLinearGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 3ex, red 3ex, green 9ex, blue 9ex)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
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
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetStartEndOutTest() {
            String gradientValue = "linear-gradient(to right, orange -100pt, red 150pt, green 200pt, blue 750pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "linear-gradient(to right, orange, red -20%, green 120%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "linear-gradient(to right, orange, red 300pt, green, blue 80%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "linear-gradient(to right, orange, red 10%, lime, 80%, blue 80.5%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "linear-gradient(to right, orange 10%, red, lime, 40%, blue, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetSmallHintTest() {
            String gradientValue = "linear-gradient(to right, orange, 1%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetNegativeHintTest() {
            String gradientValue = "linear-gradient(to right, orange, -100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffOffsetHintTest() {
            String gradientValue = "linear-gradient(to right, orange, 100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradDiffMultipleOffsetsTest() {
            String gradientValue = "linear-gradient(to right, orange 10%, blue 60% 70%, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetStartEndInsideTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetStartEndOutTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange -100pt, red 150pt, green 200pt, blue 750pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red -20%, green 120%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red 300pt, green, blue 80%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, red 10%, lime, 80%, blue 80.5%, black)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10%, red, lime, 40%, blue, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetSmallHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, 1%, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetNegativeHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, -100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffOffsetHintTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange, 100pt, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradDiffMultipleOffsetsTest() {
            String gradientValue = "repeating-linear-gradient(to right, orange 10%, blue 60% 70%, black 90%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssLinearGradientValue(gradientValue));
            StrategyBasedLinearGradientBuilder gradientBuilder = CssGradientUtil.ParseCssLinearGradient(gradientValue, 
                12, 12);
            NUnit.Framework.Assert.IsNotNull(gradientBuilder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertStrategyBasedBuilderEquals(gradientBuilder, false, 0d, StrategyBasedLinearGradientBuilder.GradientStrategy
                .TO_RIGHT, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        private void AssertStrategyBasedBuilderEquals(AbstractLinearGradientBuilder gradientBuilder, bool expectedIsCentralRotationStrategy
            , double expectedRotateVectorAngle, StrategyBasedLinearGradientBuilder.GradientStrategy expectedGradientStrategy
            , GradientSpreadMethod expectedSpreadMethod, IList<GradientColorStop> expectedColorStops) {
            NUnit.Framework.Assert.IsTrue(gradientBuilder is StrategyBasedLinearGradientBuilder);
            StrategyBasedLinearGradientBuilder builder = (StrategyBasedLinearGradientBuilder)gradientBuilder;
            NUnit.Framework.Assert.AreEqual(expectedIsCentralRotationStrategy, builder.IsCentralRotationAngleStrategy(
                ));
            NUnit.Framework.Assert.AreEqual(expectedRotateVectorAngle, builder.GetRotateVectorAngle(), 1e-10);
            NUnit.Framework.Assert.AreEqual(expectedGradientStrategy, builder.GetGradientStrategy());
            NUnit.Framework.Assert.AreEqual(expectedSpreadMethod, builder.GetSpread());
            IList<GradientColorStop> actualStops = builder.GetStopColors();
            NUnit.Framework.Assert.AreEqual(expectedColorStops.Count, actualStops.Count);
            for (int i = 0; i < expectedColorStops.Count; ++i) {
                NUnit.Framework.Assert.AreEqual(expectedColorStops[i], actualStops[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsCssRadialGradientValueTest() {
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue(null));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue(""));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue("linear-gradient(red, green)"));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue("-webkit-radial-gradient(red, green)"
                ));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue("radial-gradient"));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue("radial-gradient(red, green"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue("radial-gradient(red, green)"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue("repeating-radial-gradient(red, green)"
                ));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue("  radial-gradient(circle, red, blue) \t "
                ));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue("RADIAL-GRADIENT(red, blue)"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssGradientValue("linear-gradient(red, green)"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssGradientValue("radial-gradient(red, green)"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssGradientValue("repeating-linear-gradient(red, green)"));
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssGradientValue("repeating-radial-gradient(red, green)"));
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssGradientValue("url(image.png)"));
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientBasicTest() {
            String gradientValue = "radial-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(builder, true, false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER, true, 0.5d, true, false, 0.5d, true, 0d, false, 0d, false, GradientSpreadMethod.PAD, 
                expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradientTest() {
            String gradientValue = "repeating-radial-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(builder, true, false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER, true, 0.5d, true, false, 0.5d, true, 0d, false, 0d, false, GradientSpreadMethod.REPEAT
                , expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientCircleTest() {
            String gradientValue = "radial-gradient(circle, red, blue)";
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.IsTrue(builder.IsCircular());
            NUnit.Framework.Assert.AreEqual(StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, builder
                .GetGradientStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientEllipseTest() {
            String gradientValue = "radial-gradient(ellipse, red, blue)";
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.IsFalse(builder.IsCircular());
            NUnit.Framework.Assert.AreEqual(StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, builder
                .GetGradientStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientExtentKeywordsTest() {
            String[] keywords = new String[] { "closest-side", "closest-corner", "farthest-side", "farthest-corner" };
            StrategyBasedRadialGradientBuilder.GradientStrategy[] expectedStrategies = new StrategyBasedRadialGradientBuilder.GradientStrategy
                [] { StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_SIDE, StrategyBasedRadialGradientBuilder.GradientStrategy
                .CLOSEST_CORNER, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_SIDE, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER };
            for (int i = 0; i < keywords.Length; i++) {
                StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(" + keywords
                    [i] + ", red, blue)", 24, 12);
                NUnit.Framework.Assert.IsNotNull(builder);
                NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
                NUnit.Framework.Assert.IsFalse(builder.IsCircular());
                NUnit.Framework.Assert.AreEqual(expectedStrategies[i], builder.GetGradientStrategy());
                builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(circle " + keywords[i] + ", red, blue)", 
                    24, 12);
                NUnit.Framework.Assert.IsNotNull(builder);
                NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
                NUnit.Framework.Assert.IsTrue(builder.IsCircular());
                NUnit.Framework.Assert.AreEqual(expectedStrategies[i], builder.GetGradientStrategy());
                builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(" + keywords[i] + " circle, red, blue)", 
                    24, 12);
                NUnit.Framework.Assert.IsNotNull(builder);
                NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
                NUnit.Framework.Assert.IsTrue(builder.IsCircular());
                NUnit.Framework.Assert.AreEqual(expectedStrategies[i], builder.GetGradientStrategy());
                builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(ellipse " + keywords[i] + ", red, blue)"
                    , 24, 12);
                NUnit.Framework.Assert.IsNotNull(builder);
                NUnit.Framework.Assert.IsTrue(builder.IsFromCenterStrategy());
                NUnit.Framework.Assert.IsFalse(builder.IsCircular());
                NUnit.Framework.Assert.AreEqual(expectedStrategies[i], builder.GetGradientStrategy());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientExplicitSizeTest() {
            // Single length -> circle (100px = 75pt)
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(100px, red, blue)"
                , 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.AreEqual(75.0, builder.GetXRadius(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsXRadiusRelative());
            NUnit.Framework.Assert.AreEqual(75.0, builder.GetYRadius(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsYRadiusRelative());
            // Circle + length
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(circle 50pt, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.AreEqual(50.0, builder.GetXRadius(), 1e-6);
            NUnit.Framework.Assert.AreEqual(50.0, builder.GetYRadius(), 1e-6);
            // Two lengths (100px 50px = 75pt 37.5pt)
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(100px 50px, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.AreEqual(75.0, builder.GetXRadius(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsXRadiusRelative());
            NUnit.Framework.Assert.AreEqual(37.5, builder.GetYRadius(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsYRadiusRelative());
            // Two percentages
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(50% 60%, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.AreEqual(0.5, builder.GetXRadius(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsXRadiusRelative());
            NUnit.Framework.Assert.AreEqual(0.6, builder.GetYRadius(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsYRadiusRelative());
            // Ellipse + mixed
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(ellipse 50% 100pt, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsFromCenterStrategy());
            NUnit.Framework.Assert.AreEqual(0.5, builder.GetXRadius(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsXRadiusRelative());
            NUnit.Framework.Assert.AreEqual(100.0, builder.GetYRadius(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsYRadiusRelative());
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientPositionTest() {
            // at center
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(at center, red, blue)"
                , 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(0.5, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsXOffsetRelative());
            NUnit.Framework.Assert.IsFalse(builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(0.5, builder.GetYOffset(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsYOffsetRelative());
            // at top left
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(at top left, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(0.0, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(0.0, builder.GetYOffset(), 1e-6);
            // at 20% 30%
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(at 20% 30%, red, blue)", 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(0.2, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsXOffsetRelative());
            NUnit.Framework.Assert.IsFalse(builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(0.3, builder.GetYOffset(), 1e-6);
            NUnit.Framework.Assert.IsTrue(builder.IsYOffsetRelative());
            // at right 10px bottom 20px (10px = 7.5pt, 20px = 15pt)
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(at right 10px bottom 20px, red, blue)", 
                24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsFalse(builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(7.5, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsXOffsetRelative());
            NUnit.Framework.Assert.IsTrue(builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(15.0, builder.GetYOffset(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsYOffsetRelative());
            // shape + extent + position
            builder = CssGradientUtil.ParseCssRadialGradient("radial-gradient(circle closest-side at 20% 30%, red, blue)"
                , 24, 12);
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.IsTrue(builder.IsCircular());
            NUnit.Framework.Assert.AreEqual(StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_SIDE, builder.
                GetGradientStrategy());
            NUnit.Framework.Assert.IsTrue(builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(0.2, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.IsFalse(builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(0.3, builder.GetYOffset(), 1e-6);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientInvalidArgumentsTest() {
            String[] invalidGradients = new String[] { "radial-gradient()", "radial-gradient( , )", "radial-gradient(circle)"
                , "radial-gradient(not-shape-or-color, red, blue)", "radial-gradient(circle ellipse, red, blue)", "radial-gradient(closest-side farthest-corner, red, blue)"
                , "radial-gradient(circle 100px 200px, red, blue)", "radial-gradient(ellipse 100px, red, blue)", "radial-gradient(closest-side 100px, red, blue)"
                , "radial-gradient(at, red, blue)", "radial-gradient(circle at center at top, red, blue)", "radial-gradient(-50px, red, blue)"
                 };
            foreach (String invalidGradient in invalidGradients) {
                NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                    (invalidGradient, 24, 12), "Expected exception for: " + invalidGradient);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void RadialGradientInvalidPositionTest() {
            NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                ("radial-gradient(at left right, red, blue)", 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void ParseCssGradientTest() {
            NUnit.Framework.Assert.IsNotNull(CssGradientUtil.ParseCssGradient("linear-gradient(red, blue)", 24, 12));
            NUnit.Framework.Assert.IsNotNull(CssGradientUtil.ParseCssGradient("radial-gradient(circle, red, blue)", 24
                , 12));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssGradient("none", 24, 12));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssGradient("url(image.png)", 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void NullRadialGradientValueTest() {
            String gradientValue = null;
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void WebkitExtensionRadialGradientTest() {
            String gradientValue = "-webkit-radial-gradient(red, green, blue)";
            NUnit.Framework.Assert.IsFalse(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            NUnit.Framework.Assert.IsNull(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12));
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientWithNamesTest() {
            String gradientValue = "  radial-gradient(red, green, blue) \t ";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientWithHexColorsTest() {
            String gradientValue = "radial-grADIENt(#ff0000, #008000, #0000ff)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradientWithRgbFunctionsTest() {
            String gradientValue = "radial-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradientWithNamesTest() {
            String gradientValue = "  repeating-radial-gradient(red, green, blue) \t ";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradientWithHexColorsAndUpperCaseTest() {
            String gradientValue = "rePEATing-radIAL-grADIENt(#ff0000, #008000, #0000ff)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradientWithRgbFunctionsTest() {
            String gradientValue = "repeating-radial-gradient(  rgb(255, 0, 0), rgb(0, 127, 0), rgb(0, 0,   255))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyRadialGradientArguments1Test() {
            String gradientValue = "radial-gradient()";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , "radial-gradient()"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyRadialGradientArguments2Test() {
            String gradientValue = "radial-gradient( , )";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , "radial-gradient( , )"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientFirstArgumentTest() {
            String gradientValue = "radial-gradient(not-shape-or-color, orange 100pt, red 150pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , "not-shape-or-color"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientColorWithThreeOffsetsValueTest() {
            String gradientValue = "radial-gradient(red, orange 20pt 30pt 100pt, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 20pt 30pt 100pt"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientColorOffsetValueTest() {
            String gradientValue = "radial-gradient(red, orange 20, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "orange 20"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientMultipleHintsInARowValueTest() {
            String gradientValue = "radial-gradient(red, orange, 20%, 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "30%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientMultipleHintsInARowWithoutCommaValueTest() {
            String gradientValue = "radial-gradient(red, orange, 20% 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "20% 30%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientFirstElementIsAHintValueTest() {
            String gradientValue = "radial-gradient(circle, 5%, red, orange, 30%, green 200pt, blue 250pt)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "5%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidRadialGradientLastElementIsAHintValueTest() {
            String gradientValue = "radial-gradient(circle, red, orange, 30%, green 200pt, blue 250pt, 120%)";
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, 24, 12));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , "120%"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffColorNameTest() {
            String gradientValue = "radial-gradient(red, green, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffColorHexTest() {
            String gradientValue = "radial-gradient(#ff0000, #008000, #0000ff)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffColorRGBTest() {
            String gradientValue = "radial-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffColorRGBaTest() {
            String gradientValue = "radial-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradComplexArgsTopLeftTest() {
            String gradientValue = "radial-gradient(at top left, red, green, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradComplexArgsTopLeftRGBTest() {
            String gradientValue = "radial-gradient(at top left, red, rgb(0, 127, 0), blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradComplexArgsTopLeftRGBOffsetsHintsTest() {
            String gradientValue = "radial-gradient(at top left, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffColorNameTest() {
            String gradientValue = "repeating-radial-gradient(red, green, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffColorHexTest() {
            String gradientValue = "repeating-radial-gradient(#ff0000, #008000, #0000ff)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffColorRGBTest() {
            String gradientValue = "repeating-radial-gradient(rgb(255, 0, 0), rgb(0, 128, 0), rgb(0, 0, 255))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffColorRGBaTest() {
            String gradientValue = "repeating-radial-gradient(rgba(255, 0, 0, 1),  rgba(0, 128, 0, 1), rgba(0, 0, 255, 1))";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradComplexArgsTopLeftTest() {
            String gradientValue = "repeating-radial-gradient(at top left, red, green, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradComplexArgsTopLeftRGBTest() {
            String gradientValue = "repeating-radial-gradient(at top left, red, rgb(0, 127, 0), blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0f, GradientColorStop.OffsetType.
                RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0f, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradComplexArgsTopLeftRGBOffsetsHintsTest() {
            String gradientValue = "repeating-radial-gradient(at top left, red 10% 20%, 30%, rgb(0, 127, 0) 80%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.2f, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.3f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 127f / 255f, 0f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertStrategyBasedRadialBuilderEquals(CssGradientUtil.ParseCssRadialGradient(gradientValue, 24, 12), true
                , false, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER, true, 0d, true, false, 0d
                , true, 0d, false, 0d, false, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsoluteCMTest() {
            String gradientValue = "radial-gradient(orange 3cm, red 3cm, green 9cm, blue 9cm)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "radial-gradient(orange 3mm, red 3mm, green 9mm, blue 9mm)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "radial-gradient(orange 30Q, red 30Q, green 90Q, blue 90Q)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "radial-gradient(orange 1in, red 1in, green 3in, blue 3in)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "radial-gradient(orange 10pc, red 10pc, green 30pc, blue 30pc)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "radial-gradient(orange 100pt, red 100pt, green 300pt, blue 300pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "radial-gradient(orange 100px, red 100px, green 300px, blue 300px)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "radial-gradient(orange 3em, red 3em, green 9em, blue 9em)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "radial-gradient(orange 3rem, red 3rem, green 9rem, blue 9rem)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "radial-gradient(orange 3ex, red 3ex, green 9ex, blue 9ex)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsFontRelatedChTest() {
            AssertInvalidRadialGradientMetric("radial-gradient(orange 3ch, red 3ch, green 9ch, blue 9ch)", 24, 12, "orange 3ch"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsFontRelatedVhTest() {
            AssertInvalidRadialGradientMetric("radial-gradient(orange 3vh, red 3vh, green 9vh, blue 9vh)", 12, 12, "orange 3vh"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsViewPortVwTest() {
            AssertInvalidRadialGradientMetric("radial-gradient(orange 3vw, red 3vw, green 9vw, blue 9vw)", 12, 12, "orange 3vw"
                );
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsViewPortVminTest() {
            AssertInvalidRadialGradientMetric("radial-gradient(orange 3vmin, red 3vmin, green 9vmin, blue 9vmin)", 12, 
                12, "orange 3vmin");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMetricsViewPortVmaxTest() {
            AssertInvalidRadialGradientMetric("radial-gradient(orange 3vmax, red 3vmax, green 9vmax, blue 9vmax)", 12, 
                12, "orange 3vmax");
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsoluteCMTest() {
            String gradientValue = "repeating-radial-gradient(orange 3cm, red 3cm, green 9cm, blue 9cm)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9 / 2.54) * 72)
                , GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9 / 2.54) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsoluteMMTest() {
            String gradientValue = "repeating-radial-gradient(orange 3mm, red 3mm, green 9mm, blue 9mm)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((3f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((3f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((9f / 25.4) * 72
                ), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((9f / 25.4) * 72), GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsoluteQTest() {
            String gradientValue = "repeating-radial-gradient(orange 30Q, red 30Q, green 90Q, blue 90Q)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, (float)((30f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, (float)((30f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, (float)((90f / 2.54) * 72
                 / 40), GradientColorStop.OffsetType.ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, (float)((90f / 2.54) * 72 / 40), 
                GradientColorStop.OffsetType.ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsoluteInTest() {
            String gradientValue = "repeating-radial-gradient(orange 1in, red 1in, green 3in, blue 3in)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 1 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 3 * 72f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsolutePcTest() {
            String gradientValue = "repeating-radial-gradient(orange 10pc, red 10pc, green 30pc, blue 30pc)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 10 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 30 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsolutePtTest() {
            String gradientValue = "repeating-radial-gradient(orange 100pt, red 100pt, green 300pt, blue 300pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsAbsolutePxTest() {
            String gradientValue = "repeating-radial-gradient(orange 100px, red 100px, green 300px, blue 300px)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 100 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 300 * 0.75f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsFontRelatedEmTest() {
            String gradientValue = "repeating-radial-gradient(orange 3em, red 3em, green 9em, blue 9em)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsFontRelatedRemTest() {
            String gradientValue = "repeating-radial-gradient(orange 3rem, red 3rem, green 9rem, blue 9rem)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 12f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 24, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsFontRelatedExTest() {
            String gradientValue = "repeating-radial-gradient(orange 3ex, red 3ex, green 9ex, blue 9ex)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 3 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 9 * 6f, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsFontRelatedChTest() {
            AssertInvalidRadialGradientMetric("repeating-radial-gradient(orange 3ch, red 3ch, green 9ch, blue 9ch)", 12
                , 12, "orange 3ch");
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsFontRelatedVhTest() {
            AssertInvalidRadialGradientMetric("repeating-radial-gradient(orange 3vh, red 3vh, green 9vh, blue 9vh)", 12
                , 12, "orange 3vh");
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsViewPortVwTest() {
            AssertInvalidRadialGradientMetric("repeating-radial-gradient(orange 3vw, red 3vw, green 9vw, blue 9vw)", 12
                , 12, "orange 3vw");
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsViewPortVminTest() {
            AssertInvalidRadialGradientMetric("repeating-radial-gradient(orange 3vmin, red 3vmin, green 9vmin, blue 9vmin)"
                , 12, 12, "orange 3vmin");
        }

        [NUnit.Framework.Test]
        public virtual void RepeatRadialGradDiffMetricsViewPortVmaxTest() {
            AssertInvalidRadialGradientMetric("repeating-radial-gradient(orange 3vmax, red 3vmax, green 9vmax, blue 9vmax)"
                , 12, 12, "orange 3vmax");
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetStartEndInsideTest() {
            String gradientValue = "radial-gradient(orange 100pt, red 150pt, green 200pt, blue 250pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetStartEndOutTest() {
            String gradientValue = "radial-gradient(orange -100pt, red 150pt, green 200pt, blue 750pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "radial-gradient(orange, red -20%, green 120%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "radial-gradient(orange, red 300pt, green, blue 80%, black)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "radial-gradient(orange, red 10%, lime, 80%, blue 80.5%, black)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "radial-gradient(orange 10%, red, lime, 40%, blue, black 90%)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetSmallHintTest() {
            String gradientValue = "radial-gradient(orange, 1%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetNegativeHintTest() {
            String gradientValue = "radial-gradient(orange, -100pt, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffOffsetHintTest() {
            String gradientValue = "radial-gradient(orange, 100pt, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RadialGradDiffMultipleOffsetsTest() {
            String gradientValue = "radial-gradient(orange 10%, blue 60% 70%, black 90%)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.PAD, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetStartEndInsideTest() {
            String gradientValue = "repeating-radial-gradient(orange 100pt, red 150pt, green 200pt, blue 250pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 250, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetStartEndOutTest() {
            String gradientValue = "repeating-radial-gradient(orange -100pt, red 150pt, green 200pt, blue 750pt)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, -100, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 150, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 200, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 750, GradientColorStop.OffsetType
                .ABSOLUTE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetAutoStartEndMiddleElementsOutRangeTest() {
            String gradientValue = "repeating-radial-gradient(orange, red -20%, green 120%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, -0.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 1.2f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetAutoBetweenAbsoluteRelativeTest() {
            String gradientValue = "repeating-radial-gradient(orange, red 300pt, green, blue 80%, black)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 300f, GradientColorStop.OffsetType
                .ABSOLUTE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 128f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .AUTO));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.8f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetAutoBetweenRelativeHintTest() {
            String gradientValue = "repeating-radial-gradient(orange, red 10%, lime, 80%, blue 80.5%, black)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.8f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.805f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 1, GradientColorStop.OffsetType.RELATIVE
                ));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetHintBetweenAutosTest() {
            String gradientValue = "repeating-radial-gradient(orange 10%, red, lime, 40%, blue, black 90%)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 0f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 1f, 0f }, 0, GradientColorStop.OffsetType.AUTO
                ).SetHint(0.4f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0, GradientColorStop.OffsetType.AUTO
                ));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetSmallHintTest() {
            String gradientValue = "repeating-radial-gradient(orange, 1%, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(0.01f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetNegativeHintTest() {
            String gradientValue = "repeating-radial-gradient(orange, -100pt, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(-100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffOffsetHintTest() {
            String gradientValue = "repeating-radial-gradient(orange, 100pt, blue)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0, GradientColorStop.OffsetType
                .RELATIVE).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 1f, GradientColorStop.OffsetType.
                RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingRadialGradDiffMultipleOffsetsTest() {
            String gradientValue = "repeating-radial-gradient(orange 10%, blue 60% 70%, black 90%)";
            IList<GradientColorStop> expectedColorStops = new List<GradientColorStop>();
            expectedColorStops.Add(new GradientColorStop(new float[] { 1f, 165f / 255f, 0f }, 0.1f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.6f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 1f }, 0.7f, GradientColorStop.OffsetType
                .RELATIVE));
            expectedColorStops.Add(new GradientColorStop(new float[] { 0f, 0f, 0f }, 0.9f, GradientColorStop.OffsetType
                .RELATIVE));
            AssertDefaultRadialGradient(gradientValue, 12, 12, GradientSpreadMethod.REPEAT, expectedColorStops);
        }

        private void AssertInvalidRadialGradientMetric(String gradientValue, float emValue, float remValue, String
             expectedInvalidColorStop) {
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssGradientUtil.ParseCssRadialGradient
                (gradientValue, emValue, remValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                , expectedInvalidColorStop), e.Message);
        }

        private void AssertDefaultRadialGradient(String gradientValue, float emValue, float remValue, GradientSpreadMethod
             expectedSpreadMethod, IList<GradientColorStop> expectedColorStops) {
            NUnit.Framework.Assert.IsTrue(CssGradientUtil.IsCssRadialGradientValue(gradientValue));
            StrategyBasedRadialGradientBuilder builder = CssGradientUtil.ParseCssRadialGradient(gradientValue, emValue
                , remValue);
            AssertStrategyBasedRadialBuilderEquals(builder, true, false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER, true, 0.5d, true, false, 0.5d, true, 0d, false, 0d, false, expectedSpreadMethod, expectedColorStops
                );
        }

        private void AssertStrategyBasedRadialBuilderEquals(StrategyBasedRadialGradientBuilder builder, bool expectedIsFromCenterStrategy
            , bool expectedIsCircular, StrategyBasedRadialGradientBuilder.GradientStrategy expectedGradientStrategy
            , bool expectedLeftToRight, double expectedXOffset, bool expectedXOffsetRelative, bool expectedBottomToTop
            , double expectedYOffset, bool expectedYOffsetRelative, double expectedXRadius, bool expectedXRadiusRelative
            , double expectedYRadius, bool expectedYRadiusRelative, GradientSpreadMethod expectedSpreadMethod, IList
            <GradientColorStop> expectedColorStops) {
            NUnit.Framework.Assert.IsNotNull(builder);
            NUnit.Framework.Assert.AreEqual(expectedIsFromCenterStrategy, builder.IsFromCenterStrategy());
            if (expectedIsFromCenterStrategy) {
                NUnit.Framework.Assert.AreEqual(expectedIsCircular, builder.IsCircular());
                NUnit.Framework.Assert.AreEqual(expectedGradientStrategy, builder.GetGradientStrategy());
            }
            else {
                NUnit.Framework.Assert.AreEqual(expectedXRadius, builder.GetXRadius(), 1e-6);
                NUnit.Framework.Assert.AreEqual(expectedXRadiusRelative, builder.IsXRadiusRelative());
                NUnit.Framework.Assert.AreEqual(expectedYRadius, builder.GetYRadius(), 1e-6);
                NUnit.Framework.Assert.AreEqual(expectedYRadiusRelative, builder.IsYRadiusRelative());
            }
            NUnit.Framework.Assert.AreEqual(expectedLeftToRight, builder.IsLeftToRight());
            NUnit.Framework.Assert.AreEqual(expectedXOffset, builder.GetXOffset(), 1e-6);
            NUnit.Framework.Assert.AreEqual(expectedXOffsetRelative, builder.IsXOffsetRelative());
            NUnit.Framework.Assert.AreEqual(expectedBottomToTop, builder.IsBottomToTop());
            NUnit.Framework.Assert.AreEqual(expectedYOffset, builder.GetYOffset(), 1e-6);
            NUnit.Framework.Assert.AreEqual(expectedYOffsetRelative, builder.IsYOffsetRelative());
            NUnit.Framework.Assert.AreEqual(expectedSpreadMethod, builder.GetSpread());
            IList<GradientColorStop> actualStops = builder.GetStopColors();
            NUnit.Framework.Assert.AreEqual(expectedColorStops.Count, actualStops.Count);
            for (int i = 0; i < expectedColorStops.Count; ++i) {
                NUnit.Framework.Assert.AreEqual(expectedColorStops[i], actualStops[i]);
            }
        }
    }
}
