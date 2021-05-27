/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Util {
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
            NUnit.Framework.Assert.AreEqual(iText.StyledXmlParser.LogMessageConstant.INCORRECT_RESOLUTION_UNIT_VALUE, 
                e.Message);
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
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 1)]
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
        public virtual void ParseAboluteLengthExponential01() {
            String value = "1e2pt";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value);
            float expected = 1e2f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAboluteLengthExponential02() {
            String value = "1e2px";
            float actual = CssDimensionParsingUtils.ParseAbsoluteLength(value);
            float expected = 1e2f * 0.75f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
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
    }
}
