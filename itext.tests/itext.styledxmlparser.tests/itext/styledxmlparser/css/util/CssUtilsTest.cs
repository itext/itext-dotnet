/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Layout.Properties;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Pseudo;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssUtilsTest : ExtendedITextTest {
        private static float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void ConvertFloatMaximumToPdfTest() {
            float expected = float.PositiveInfinity;
            float actual = CssUtils.ConvertPtsToPx(float.MaxValue);
            NUnit.Framework.Assert.AreEqual(expected, actual, 0f);
        }

        [NUnit.Framework.Test]
        public virtual void ConvertFloatMinimumToPdfTest() {
            float expected = 1.4E-45f;
            float actual = CssUtils.ConvertPtsToPx(float.Epsilon);
            NUnit.Framework.Assert.AreEqual(expected, actual, 0f);
        }

        [NUnit.Framework.Test]
        public virtual void ExtractShorthandPropertiesFromEmptyStringTest() {
            String sourceString = "";
            IList<IList<String>> expected = new List<IList<String>>();
            expected.Add(new List<String>());
            NUnit.Framework.Assert.AreEqual(expected, CssUtils.ExtractShorthandProperties(sourceString));
        }

        [NUnit.Framework.Test]
        public virtual void ExtractShorthandPropertiesFromStringWithOnePropertyTest() {
            String sourceString = "square inside url('sqpurple.gif')";
            IList<IList<String>> expected = new List<IList<String>>();
            IList<String> layer = new List<String>();
            layer.Add("square");
            layer.Add("inside");
            layer.Add("url('sqpurple.gif')");
            expected.Add(layer);
            NUnit.Framework.Assert.AreEqual(expected, CssUtils.ExtractShorthandProperties(sourceString));
        }

        [NUnit.Framework.Test]
        public virtual void ExtractShorthandPropertiesFromStringWithMultiplyPropertiesTest() {
            String sourceString = "center no-repeat url('sqpurple.gif'), #eee 35% url('sqpurple.gif')";
            IList<IList<String>> expected = new List<IList<String>>();
            IList<String> layer = new List<String>();
            layer.Add("center");
            layer.Add("no-repeat");
            layer.Add("url('sqpurple.gif')");
            expected.Add(layer);
            layer = new List<String>();
            layer.Add("#eee");
            layer.Add("35%");
            layer.Add("url('sqpurple.gif')");
            expected.Add(layer);
            NUnit.Framework.Assert.AreEqual(expected, CssUtils.ExtractShorthandProperties(sourceString));
        }

        [NUnit.Framework.Test]
        public virtual void NormalConvertPtsToPxTest() {
            float[] input = new float[] { -1f, 0f, 1f };
            float[] expected = new float[] { -1.3333334f, 0f, 1.3333334f };
            for (int i = 0; i < input.Length; i++) {
                float actual = CssUtils.ConvertPtsToPx(input[i]);
                NUnit.Framework.Assert.AreEqual(expected[i], actual, 0f);
            }
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
        public virtual void ElementNodeIsStyleSheetLink() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("link"), "");
            element.Attr(CommonAttributeConstants.REL, CommonAttributeConstants.STYLESHEET);
            JsoupElementNode elementNode = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsTrue(CssUtils.IsStyleSheetLink(elementNode));
        }

        [NUnit.Framework.Test]
        public virtual void ElementNodeIsNotLink() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("p"), "");
            element.Attr(CommonAttributeConstants.REL, CommonAttributeConstants.STYLESHEET);
            JsoupElementNode elementNode = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsFalse(CssUtils.IsStyleSheetLink(elementNode));
        }

        [NUnit.Framework.Test]
        public virtual void ElementNodeAttributeIsNotStylesheet() {
            iText.StyledXmlParser.Jsoup.Nodes.Element element = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("link"), "");
            element.Attr(CommonAttributeConstants.REL, "");
            JsoupElementNode elementNode = new JsoupElementNode(element);
            NUnit.Framework.Assert.IsFalse(CssUtils.IsStyleSheetLink(elementNode));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INCORRECT_CHARACTER_SEQUENCE)]
        public virtual void SplitStringWithCommaTest() {
            NUnit.Framework.Assert.AreEqual(new List<String>(), CssUtils.SplitStringWithComma(null));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "value2", "value3"), CssUtils.SplitStringWithComma
                ("value1,value2,value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", " value2", " value3"), CssUtils.SplitStringWithComma
                ("value1, value2, value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "(value,with,comma)", "value3"), CssUtils.
                SplitStringWithComma("value1,(value,with,comma),value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "(val(ue,with,comma),value3"), CssUtils.SplitStringWithComma
                ("value1,(val(ue,with,comma),value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "(value,with)", "comma)", "value3"), CssUtils
                .SplitStringWithComma("value1,(value,with),comma),value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "( v2,v3)", "(v4, v5)", "value3"), CssUtils
                .SplitStringWithComma("value1,( v2,v3),(v4, v5),value3"));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("v.al*ue1\"", "( v2,v3)", "\"(v4,v5;);", "value3"), 
                CssUtils.SplitStringWithComma("v.al*ue1\",( v2,v3),\"(v4,v5;);,value3"));
        }

        [NUnit.Framework.Test]
        public virtual void SplitStringTest() {
            NUnit.Framework.Assert.AreEqual(new List<String>(), CssUtils.SplitString(null, ','));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "(value,with,comma)", "value3"), CssUtils.
                SplitString("value1,(value,with,comma),value3", ',', new EscapeGroup('(', ')')));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1 ", " (val(ue,with,comma),value3"), CssUtils.
                SplitString("value1 , (val(ue,with,comma),value3", ',', new EscapeGroup('(', ')')));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("some text", " (some", " text in", " brackets)", " \"some, text, in quotes,\""
                ), CssUtils.SplitString("some text, (some, text in, brackets), \"some, text, in quotes,\"", ',', new EscapeGroup
                ('\"')));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("some text", " (some. text in. brackets)", " \"some. text. in quotes.\""
                ), CssUtils.SplitString("some text. (some. text in. brackets). \"some. text. in quotes.\"", '.', new EscapeGroup
                ('\"'), new EscapeGroup('(', ')')));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "(value", "with", "comma)", "value3"), CssUtils
                .SplitString("value1,(value,with,comma),value3", ','));
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList("value1", "value", "with", "comma", "value3"), CssUtils
                .SplitString("value1,value,with,comma,value3", ',', new EscapeGroup(',')));
        }

        [NUnit.Framework.Test]
        public virtual void ParseBlendModeTest() {
            NUnit.Framework.Assert.AreEqual(BlendMode.NORMAL, CssUtils.ParseBlendMode(null));
            NUnit.Framework.Assert.AreEqual(BlendMode.NORMAL, CssUtils.ParseBlendMode(CommonCssConstants.NORMAL));
            NUnit.Framework.Assert.AreEqual(BlendMode.MULTIPLY, CssUtils.ParseBlendMode(CommonCssConstants.MULTIPLY));
            NUnit.Framework.Assert.AreEqual(BlendMode.SCREEN, CssUtils.ParseBlendMode(CommonCssConstants.SCREEN));
            NUnit.Framework.Assert.AreEqual(BlendMode.OVERLAY, CssUtils.ParseBlendMode(CommonCssConstants.OVERLAY));
            NUnit.Framework.Assert.AreEqual(BlendMode.DARKEN, CssUtils.ParseBlendMode(CommonCssConstants.DARKEN));
            NUnit.Framework.Assert.AreEqual(BlendMode.LIGHTEN, CssUtils.ParseBlendMode(CommonCssConstants.LIGHTEN));
            NUnit.Framework.Assert.AreEqual(BlendMode.COLOR_DODGE, CssUtils.ParseBlendMode(CommonCssConstants.COLOR_DODGE
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.COLOR_BURN, CssUtils.ParseBlendMode(CommonCssConstants.COLOR_BURN
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.HARD_LIGHT, CssUtils.ParseBlendMode(CommonCssConstants.HARD_LIGHT
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.SOFT_LIGHT, CssUtils.ParseBlendMode(CommonCssConstants.SOFT_LIGHT
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.DIFFERENCE, CssUtils.ParseBlendMode(CommonCssConstants.DIFFERENCE
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.EXCLUSION, CssUtils.ParseBlendMode(CommonCssConstants.EXCLUSION)
                );
            NUnit.Framework.Assert.AreEqual(BlendMode.HUE, CssUtils.ParseBlendMode(CommonCssConstants.HUE));
            NUnit.Framework.Assert.AreEqual(BlendMode.SATURATION, CssUtils.ParseBlendMode(CommonCssConstants.SATURATION
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.COLOR, CssUtils.ParseBlendMode(CommonCssConstants.COLOR));
            NUnit.Framework.Assert.AreEqual(BlendMode.LUMINOSITY, CssUtils.ParseBlendMode(CommonCssConstants.LUMINOSITY
                ));
            NUnit.Framework.Assert.AreEqual(BlendMode.NORMAL, CssUtils.ParseBlendMode("invalid"));
            NUnit.Framework.Assert.AreEqual(BlendMode.NORMAL, CssUtils.ParseBlendMode("SCREEN"));
        }

        [NUnit.Framework.Test]
        public virtual void TestWrongAttrTest01() {
            String strToParse = "attr((href))";
            String result = CssUtils.ExtractAttributeValue(strToParse, null);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestWrongAttrTest02() {
            String strToParse = "attr('href')";
            String result = CssUtils.ExtractAttributeValue(strToParse, null);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestWrongAttrTest03() {
            String strToParse = "attrrname)";
            String result = CssUtils.ExtractAttributeValue(strToParse, null);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest01() {
            IElementNode iNode = new CssPseudoElementNode(null, "url");
            String strToParse = "attr(url)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest02() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(url url)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest03() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(url url,#one)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("#one", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest04() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr()";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest05() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr('\')";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest06() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str,\"hey\")";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("hey", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest07() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str string)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest08() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str string,\"value\")";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("value", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest09() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str string,\"val,ue\")";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("val,ue", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest10() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str string,'val,ue')";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.AreEqual("val,ue", result);
        }

        [NUnit.Framework.Test]
        public virtual void TestExtractingAttrTest11() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(name, \"value\", \"value\", \"value\")";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void WrongAttributeTypeTest() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str mem)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }

        [NUnit.Framework.Test]
        public virtual void WrongParamsInAttrFunctionTest() {
            IElementNode iNode = new CssPseudoElementNode(null, "test");
            String strToParse = "attr(str mem lol)";
            String result = CssUtils.ExtractAttributeValue(strToParse, iNode);
            NUnit.Framework.Assert.IsNull(result);
        }
    }
}
