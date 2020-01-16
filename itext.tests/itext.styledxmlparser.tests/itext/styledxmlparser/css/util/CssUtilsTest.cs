/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Util {
    public class CssUtilsTest : ExtendedITextTest {
        public static float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNAN() {
            NUnit.Framework.Assert.That(() =>  {
                String value = "Definitely not a number";
                CssUtils.ParseAbsoluteLength(value);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.NAN, "Definitely not a number")))
;
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFromNull() {
            NUnit.Framework.Assert.That(() =>  {
                String value = null;
                CssUtils.ParseAbsoluteLength(value);
            }
            , NUnit.Framework.Throws.InstanceOf<StyledXMLParserException>().With.Message.EqualTo(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.NAN, "null")))
;
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10px() {
            String value = "10px";
            float actual = CssUtils.ParseAbsoluteLength(value, CommonCssConstants.PX);
            float expected = 7.5f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10cm() {
            String value = "10cm";
            float actual = CssUtils.ParseAbsoluteLength(value, CommonCssConstants.CM);
            float expected = 283.46457f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10in() {
            String value = "10in";
            float actual = CssUtils.ParseAbsoluteLength(value, CommonCssConstants.IN);
            float expected = 720.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthFrom10pt() {
            String value = "10pt";
            float actual = CssUtils.ParseAbsoluteLength(value, CommonCssConstants.PT);
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAboluteLengthExponential01() {
            String value = "1e2pt";
            float actual = CssUtils.ParseAbsoluteLength(value);
            float expected = 1e2f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAboluteLengthExponential02() {
            String value = "1e2px";
            float actual = CssUtils.ParseAbsoluteLength(value);
            float expected = 1e2f * 0.75f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED, Count = 1)]
        public virtual void ParseAbsoluteLengthFromUnknownType() {
            String value = "10pateekes";
            float actual = CssUtils.ParseAbsoluteLength(value, "pateekes");
            float expected = 10.0f;
            NUnit.Framework.Assert.AreEqual(expected, actual, 0);
        }

        [NUnit.Framework.Test]
        public virtual void ValidateMetricValue() {
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1px"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1in"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1cm"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1mm"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1pc"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsMetricValue("1em"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsMetricValue("1rem"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsMetricValue("1ex"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsMetricValue("1pt"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsMetricValue("1inch"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsMetricValue("+1m"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateNumericValue() {
            NUnit.Framework.Assert.IsTrue(CssUtils.IsNumericValue("1"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsNumericValue("12"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsNumericValue("1.2"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsNumericValue(".12"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsNumericValue("12f"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsNumericValue("f1.2"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsNumericValue(".12f"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseLength() {
            NUnit.Framework.Assert.AreEqual(9, CssUtils.ParseAbsoluteLength("12"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssUtils.ParseAbsoluteLength("8inch"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssUtils.ParseAbsoluteLength("8", CommonCssConstants.IN), 0);
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeProperty() {
            NUnit.Framework.Assert.AreEqual("part1 part2", CssUtils.NormalizeCssProperty("   part1   part2  "));
            NUnit.Framework.Assert.AreEqual("\" the next quote is ESCAPED \\\\\\\" still  IN string \"", CssUtils.NormalizeCssProperty
                ("\" the next quote is ESCAPED \\\\\\\" still  IN string \""));
            NUnit.Framework.Assert.AreEqual("\" the next quote is NOT ESCAPED \\\\\" not in the string", CssUtils.NormalizeCssProperty
                ("\" the next quote is NOT ESCAPED \\\\\" NOT in   THE string"));
            NUnit.Framework.Assert.AreEqual("\" You CAN put 'Single  Quotes' in double quotes WITHOUT escaping\"", CssUtils
                .NormalizeCssProperty("\" You CAN put 'Single  Quotes' in double quotes WITHOUT escaping\""));
            NUnit.Framework.Assert.AreEqual("' You CAN put \"DOUBLE  Quotes\" in double quotes WITHOUT escaping'", CssUtils
                .NormalizeCssProperty("' You CAN put \"DOUBLE  Quotes\" in double quotes WITHOUT escaping'"));
            NUnit.Framework.Assert.AreEqual("\" ( BLA \" attr(href)\" BLA )  \"", CssUtils.NormalizeCssProperty("\" ( BLA \"      AttR( Href  )\" BLA )  \""
                ));
            NUnit.Framework.Assert.AreEqual("\" (  \"attr(href) \"  )  \"", CssUtils.NormalizeCssProperty("\" (  \"aTTr( hREf  )   \"  )  \""
                ));
            NUnit.Framework.Assert.AreEqual("rgba(255,255,255,0.2)", CssUtils.NormalizeCssProperty("rgba(  255,  255 ,  255 ,0.2   )"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeUrlTest() {
            NUnit.Framework.Assert.AreEqual("url(data:application/font-woff;base64,2CBPCRXmgywtV1t4oWwjBju0kqkvfhPs0cYdMgFtDSY5uL7MIGT5wiGs078HrvBHekp0Yf=)"
                , CssUtils.NormalizeCssProperty("url(data:application/font-woff;base64,2CBPCRXmgywtV1t4oWwjBju0kqkvfhPs0cYdMgFtDSY5uL7MIGT5wiGs078HrvBHekp0Yf=)"
                ));
            NUnit.Framework.Assert.AreEqual("url(\"quoted  Url\")", CssUtils.NormalizeCssProperty("  url(  \"quoted  Url\")"
                ));
            NUnit.Framework.Assert.AreEqual("url('quoted  Url')", CssUtils.NormalizeCssProperty("  url(  'quoted  Url')"
                ));
            NUnit.Framework.Assert.AreEqual("url(haveEscapedEndBracket\\))", CssUtils.NormalizeCssProperty("url(  haveEscapedEndBracket\\) )"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ParseUnicodeRangeTest() {
            NUnit.Framework.Assert.AreEqual("[(0; 1048575)]", CssUtils.ParseUnicodeRange("U+?????").ToString());
            NUnit.Framework.Assert.AreEqual("[(38; 38)]", CssUtils.ParseUnicodeRange("U+26").ToString());
            NUnit.Framework.Assert.AreEqual("[(0; 127)]", CssUtils.ParseUnicodeRange(" U+0-7F").ToString());
            NUnit.Framework.Assert.AreEqual("[(37; 255)]", CssUtils.ParseUnicodeRange("U+0025-00FF").ToString());
            NUnit.Framework.Assert.AreEqual("[(1024; 1279)]", CssUtils.ParseUnicodeRange("U+4??").ToString());
            NUnit.Framework.Assert.AreEqual("[(262224; 327519)]", CssUtils.ParseUnicodeRange("U+4??5?").ToString());
            NUnit.Framework.Assert.AreEqual("[(37; 255), (1024; 1279)]", CssUtils.ParseUnicodeRange("U+0025-00FF, U+4??"
                ).ToString());
            NUnit.Framework.Assert.IsNull(CssUtils.ParseUnicodeRange("U+??????"));
            // more than 5 question marks are not allowed
            NUnit.Framework.Assert.IsNull(CssUtils.ParseUnicodeRange("UU+7-10"));
            // wrong syntax
            NUnit.Framework.Assert.IsNull(CssUtils.ParseUnicodeRange("U+7?-9?"));
            // wrong syntax
            NUnit.Framework.Assert.IsNull(CssUtils.ParseUnicodeRange("U+7-"));
        }

        // wrong syntax
        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteFontSizeTest() {
            NUnit.Framework.Assert.AreEqual(75, CssUtils.ParseAbsoluteFontSize("100", CommonCssConstants.PX), EPS);
            NUnit.Framework.Assert.AreEqual(75, CssUtils.ParseAbsoluteFontSize("100px"), EPS);
            NUnit.Framework.Assert.AreEqual(12, CssUtils.ParseAbsoluteFontSize(CommonCssConstants.MEDIUM), EPS);
            NUnit.Framework.Assert.AreEqual(0, CssUtils.ParseAbsoluteFontSize("", ""), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ParseRelativeFontSizeTest() {
            NUnit.Framework.Assert.AreEqual(120, CssUtils.ParseRelativeFontSize("10em", 12), EPS);
            NUnit.Framework.Assert.AreEqual(12.5f, CssUtils.ParseRelativeFontSize(CommonCssConstants.SMALLER, 15), EPS
                );
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLengthTest() {
            NUnit.Framework.Assert.AreEqual(75, CssUtils.ParseAbsoluteLength("100", CommonCssConstants.PX), EPS);
            NUnit.Framework.Assert.AreEqual(75, CssUtils.ParseAbsoluteLength("100px"), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidFloat() {
            String value = "invalidFloat";
            try {
                NUnit.Framework.Assert.IsNull(CssUtils.ParseFloat(value));
            }
            catch (Exception) {
                NUnit.Framework.Assert.Fail();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLength12cmTest() {
            // Calculations in CssUtils#parseAbsoluteLength were changed to work
            // with double values instead of float to improve precision and eliminate
            // the difference between java and .net. So the test verifies this fix.
            NUnit.Framework.Assert.AreEqual(340.15747f, CssUtils.ParseAbsoluteLength("12cm"), 0f);
        }

        [NUnit.Framework.Test]
        public virtual void ParseAbsoluteLength12qTest() {
            // Calculations in CssUtils#parseAbsoluteLength were changed to work
            // with double values instead of float to improve precision and eliminate
            // the difference between java and .net. So the test verifies this fix
            NUnit.Framework.Assert.AreEqual(8.503937f, CssUtils.ParseAbsoluteLength("12q"), 0f);
        }

        [NUnit.Framework.Test]
        public virtual void TestIsAngleCorrectValues() {
            NUnit.Framework.Assert.IsTrue(CssUtils.IsAngleValue("10deg"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsAngleValue("-20grad"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsAngleValue("30.5rad"));
            NUnit.Framework.Assert.IsTrue(CssUtils.IsAngleValue("0rad"));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsAngleNullValue() {
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue(null));
        }

        [NUnit.Framework.Test]
        public virtual void TestIsAngleIncorrectValues() {
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue("deg"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue("-20,6grad"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue("0"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue("10in"));
            NUnit.Framework.Assert.IsFalse(CssUtils.IsAngleValue("10px"));
        }
    }
}
