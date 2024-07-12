/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssDimensionParsingUtilsTest : ExtendedITextTest {
        private const float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteFontSizeTest() {
            NUnit.Framework.Assert.AreEqual(75, CssDimensionParsingUtils.ParseAbsoluteFontSize("100", CommonCssConstants
                .PX), EPS);
            NUnit.Framework.Assert.AreEqual(75, CssDimensionParsingUtils.ParseAbsoluteFontSize("100px"), EPS);
            NUnit.Framework.Assert.AreEqual(12, CssDimensionParsingUtils.ParseAbsoluteFontSize(CommonCssConstants.MEDIUM
                ), EPS);
            NUnit.Framework.Assert.AreEqual(0, CssDimensionParsingUtils.ParseAbsoluteFontSize("", ""), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ParseRelativeFontSizeTest() {
            NUnit.Framework.Assert.AreEqual(120, CssDimensionParsingUtils.ParseRelativeFontSize("10em", 12), EPS);
            NUnit.Framework.Assert.AreEqual(12.5f, CssDimensionParsingUtils.ParseRelativeFontSize(CommonCssConstants.SMALLER
                , 15), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ParseResolutionValidDpiUnit() {
            NUnit.Framework.Assert.AreEqual(10f, CssDimensionParsingUtils.ParseResolution("10dpi"), 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseResolutionValidDpcmUnit() {
            NUnit.Framework.Assert.AreEqual(25.4f, CssDimensionParsingUtils.ParseResolution("10dpcm"), 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseResolutionValidDppxUnit() {
            NUnit.Framework.Assert.AreEqual(960f, CssDimensionParsingUtils.ParseResolution("10dppx"), 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseResolutionInvalidUnit() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssDimensionParsingUtils
                .ParseResolution("10incorrectUnit"));
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INCORRECT_RESOLUTION_UNIT_VALUE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidFloat() {
            String value = "invalidFloat";
            try {
                NUnit.Framework.Assert.IsNull(CssDimensionParsingUtils.ParseFloat(value));
            }
            catch (Exception) {
                NUnit.Framework.Assert.Fail();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10px() {
            String value = "10px";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value, CommonCssConstants.PX);
            float expected = 7.5f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10cm() {
            String value = "10cm";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value, CommonCssConstants.CM);
            float expected = 283.46457f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10in() {
            String value = "10in";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value, CommonCssConstants.IN);
            float expected = 720.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10pt() {
            String value = "10pt";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value, CommonCssConstants.PT);
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            , Count = 1)]
        public virtual void ParseAbsoluteLengthFromUnknownType() {
            String value = "10pateekes";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value, "pateekes");
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseLength() {
            NUnit.Framework.Assert.AreEqual(9, CssDimensionParsingUtils.ParseAbsoluteLength("12"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssDimensionParsingUtils.ParseAbsoluteLength("8inch"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssDimensionParsingUtils.ParseAbsoluteLength("8", CommonCssConstants.
                IN), 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthTest() {
            NUnit.Framework.Assert.AreEqual(75, CssDimensionParsingUtils.ParseAbsoluteLength("100", CommonCssConstants
                .PX), EPS);
            NUnit.Framework.Assert.AreEqual(75, CssDimensionParsingUtils.ParseAbsoluteLength("100px"), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNAN() {
            String value = "Definitely not a number";
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssDimensionParsingUtils
                .ParseAbsoluteLength(value));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.NAN, "Definitely not a number"
                ), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNull() {
            String value = null;
            Exception e = NUnit.Framework.Assert.Catch(typeof(StyledXMLParserException), () => CssDimensionParsingUtils
                .ParseAbsoluteLength(value));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(StyledXMLParserException.NAN, "null"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthExponentialPtTest() {
            String value = "1e2pt";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value);
            float expected = 1e2f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthExponentialPxTest() {
            String value = "1e2px";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value);
            float expected = 1e2f * 0.75f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthExponentialCapitalTest() {
            String value = "1E-4";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value);
            float expected = 1e-4f * 0.75f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 1e-9);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLength12cmTest() {
            // Calculations in CssUtils#parseAbsoluteLength were changed to work
            // with double values instead of float to improve precision and eliminate
            // the difference between java and .net. So the test verifies this fix.
            NUnit.Framework.Assert.AreEqual(340.15747f, CssDimensionParsingUtils.ParseAbsoluteLength("12cm"), 0f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLength12qTest() {
            // Calculations in CssUtils#parseAbsoluteLength were changed to work
            // with double values instead of float to improve precision and eliminate
            // the difference between java and .net. So the test verifies this fix
            NUnit.Framework.Assert.AreEqual(8.503937f, CssDimensionParsingUtils.ParseAbsoluteLength("12q"), 0f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleIntegerValueTest() {
            double? expectedString = 5.0;
            double? actualString = CssDimensionParsingUtils.ParseDouble("5");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleManyCharsAfterDotTest() {
            double? expectedString = 5.123456789;
            double? actualString = CssDimensionParsingUtils.ParseDouble("5.123456789");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleManyCharsAfterDotNegativeTest() {
            double? expectedString = -5.123456789;
            double? actualString = CssDimensionParsingUtils.ParseDouble("-5.123456789");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleNullValueTest() {
            double? expectedString = null;
            double? actualString = CssDimensionParsingUtils.ParseDouble(null);
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDoubleNegativeTextTest() {
            double? expectedString = null;
            double? actualString = CssDimensionParsingUtils.ParseDouble("text");
            NUnit.Framework.Assert.AreEqual(expectedString, actualString);
        }

        [NUnit.Framework.Test]
        public virtual void ParseSimpleDeviceCmykTest() {
            TransparentColor expected = new TransparentColor(new DeviceCmyk(0f, 0.4f, 0.6f, 1f), 1);
            TransparentColor actual = CssDimensionParsingUtils.ParseColor("device-cmyk(0 40% 60% 100%)");
            NUnit.Framework.Assert.AreEqual(expected.GetColor(), actual.GetColor());
            NUnit.Framework.Assert.AreEqual(expected.GetOpacity(), actual.GetOpacity(), 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDeviceCmykWithOpacityTest() {
            TransparentColor expected = new TransparentColor(new DeviceCmyk(0f, 0.4f, 0.6f, 1f), 0.5f);
            TransparentColor actual = CssDimensionParsingUtils.ParseColor("device-cmyk(0 40% 60% 100% / .5)");
            NUnit.Framework.Assert.AreEqual(expected.GetColor(), actual.GetColor());
            NUnit.Framework.Assert.AreEqual(expected.GetOpacity(), actual.GetOpacity(), 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseDeviceCmykWithFallbackAndOpacityTest() {
            TransparentColor expected = new TransparentColor(new DeviceCmyk(0f, 0.4f, 0.6f, 1f), 0.5f);
            TransparentColor actual = CssDimensionParsingUtils.ParseColor("device-cmyk(0 40% 60% 100% / .5 rgb(178 34 34))"
                );
            NUnit.Framework.Assert.AreEqual(expected.GetColor(), actual.GetColor());
            NUnit.Framework.Assert.AreEqual(expected.GetOpacity(), actual.GetOpacity(), 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseRgbTest() {
            TransparentColor expected = new TransparentColor(new DeviceRgb(255, 255, 128), 1f);
            TransparentColor actual = CssDimensionParsingUtils.ParseColor("rgb(255, 255, 128)");
            NUnit.Framework.Assert.AreEqual(expected.GetColor(), actual.GetColor());
            NUnit.Framework.Assert.AreEqual(expected.GetOpacity(), actual.GetOpacity(), 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidColorTest() {
            TransparentColor expected = new TransparentColor(new DeviceRgb(0, 0, 0), 1f);
            TransparentColor actual = CssDimensionParsingUtils.ParseColor("currentcolor");
            NUnit.Framework.Assert.AreEqual(expected.GetColor(), actual.GetColor());
            NUnit.Framework.Assert.AreEqual(expected.GetOpacity(), actual.GetOpacity(), 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseLengthAbsoluteTest() {
            float result = CssDimensionParsingUtils.ParseLength("10pt", 1, 2, 1, 1);
            NUnit.Framework.Assert.AreEqual(10, result, 0.0001f);
            result = CssDimensionParsingUtils.ParseLength("10px", 1, 1, 2, 1);
            NUnit.Framework.Assert.AreEqual(7.5, result, 0.0001f);
            result = CssDimensionParsingUtils.ParseLength("10in", 1, 1, 2, 1);
            NUnit.Framework.Assert.AreEqual(720, result, 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseLengthPercentTest() {
            float result = CssDimensionParsingUtils.ParseLength("10%", 10, 2, 1, 1);
            NUnit.Framework.Assert.AreEqual(1, result, 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseLengthFontTest() {
            float result = CssDimensionParsingUtils.ParseLength("10em", 10, 2, 8, 9);
            NUnit.Framework.Assert.AreEqual(80, result, 0.0001f);
            result = CssDimensionParsingUtils.ParseLength("10rem", 10, 2, 8, 9);
            NUnit.Framework.Assert.AreEqual(90, result, 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseLengthInvalidTest() {
            float result = CssDimensionParsingUtils.ParseLength("10cmm", 10, 2, 8, 9);
            NUnit.Framework.Assert.AreEqual(2, result, 0.0001f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseFlexTest() {
            NUnit.Framework.Assert.AreEqual(13.3f, CssDimensionParsingUtils.ParseFlex("13.3fr"), 0.0001);
            NUnit.Framework.Assert.AreEqual(13.3f, CssDimensionParsingUtils.ParseFlex("13.3fr "), 0.0001);
            NUnit.Framework.Assert.AreEqual(13.3f, CssDimensionParsingUtils.ParseFlex(" 13.3fr "), 0.0001);
            NUnit.Framework.Assert.IsNull(CssDimensionParsingUtils.ParseFlex("13.3 fr"));
            NUnit.Framework.Assert.IsNull(CssDimensionParsingUtils.ParseFlex("13.3f"));
            NUnit.Framework.Assert.IsNull(CssDimensionParsingUtils.ParseFlex("13.3"));
            NUnit.Framework.Assert.IsNull(CssDimensionParsingUtils.ParseFlex(null));
        }
    }
}
