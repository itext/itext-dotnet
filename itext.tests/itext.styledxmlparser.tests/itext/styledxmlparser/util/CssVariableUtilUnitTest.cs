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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssVariableUtilUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResolveSimpleVariableTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("margin", null);
            styles.Put("--test", "50px");
            styles.Put("padding-bottom", "var(--test)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-bottom"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveSimpleVariableWithSpacesTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("margin", null);
            styles.Put("--test", "50px");
            styles.Put("padding-right", "var ( --test )");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("var ( --test )", styles.Get("padding-right"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveSimpleVariableWithSpacesTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("margin", null);
            styles.Put("--test", "50px");
            styles.Put("padding-top", "var( --test )");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-top"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveSimpleVariableWithDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--test", "50px");
            styles.Put("padding", "var(--test, 30px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-left"));
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-right"));
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-top"));
            NUnit.Framework.Assert.AreEqual("50px", styles.Get("padding-bottom"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveIncorrectVariableWithDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--test", "50px");
            styles.Put("padding-left", "var(--incorrect, 30px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("30px", styles.Get("padding-left"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveIncorrectVariableWithoutDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--test", "50px");
            styles.Put("padding", "var(--incorrect)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("padding"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        [NUnit.Framework.Test]
        public virtual void ResolveIncorrectVariableWithValidationTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--test", "50px");
            styles.Put("word-break", "var(--test, 30px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("word-break"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVarExpressionInStyleSheetTest() {
            IList<CssDeclaration> declarations = new List<CssDeclaration>();
            declarations.Add(new CssDeclaration("--test", "normal"));
            declarations.Add(new CssDeclaration("word-break", "var(--test, break-all)"));
            IList<CssRuleSet> ruleSets = new List<CssRuleSet>();
            ruleSets.Add(new CssRuleSet(new CssSelector("a"), declarations));
            IDictionary<String, String> result = CssStyleSheet.ExtractStylesFromRuleSets(ruleSets);
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("--test", "normal");
            expected.Put("word-break", "var(--test,break-all)");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveInnerVariableTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-black", "rgb(0, 0, 0)");
            styles.Put("border", "1px dotted var(--color-black)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual(13, styles.Count);
            NUnit.Framework.Assert.AreEqual("dotted", styles.Get("border-right-style"));
            NUnit.Framework.Assert.AreEqual("1px", styles.Get("border-right-width"));
            NUnit.Framework.Assert.AreEqual("rgb(0,0,0)", styles.Get("border-right-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveComplexVariableTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-black", "black");
            styles.Put("--border-dotted", "1px dotted var(--color-black)");
            styles.Put("border", "var(--border-dotted)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("border"));
            NUnit.Framework.Assert.AreEqual(14, styles.Count);
            NUnit.Framework.Assert.AreEqual("dotted", styles.Get("border-right-style"));
            NUnit.Framework.Assert.AreEqual("1px", styles.Get("border-right-width"));
            NUnit.Framework.Assert.AreEqual("black", styles.Get("border-right-color"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        [NUnit.Framework.Test]
        public virtual void ResolveVariableWithFallbackOnDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-black-60", "bad-color");
            styles.Put("background-color", "var(--color-black-60, rgba(1, 255, 0))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("background-color"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        [NUnit.Framework.Test]
        public virtual void ResolveVariableWithFallbackOnDefaultTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--test1", "invalid");
            styles.Put("word-break", "var(--test1, var(--invalid, break-all))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("word-break"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVariableWithFallbackOnDefaultTest3() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("word-break", "var(--test1, var(--invalid, break-all))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("break-all", styles.Get("word-break"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        [NUnit.Framework.Test]
        public virtual void ResolveVariableWithFallbackOnInvalidDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-black-60", "bad-color");
            styles.Put("background-color", "var(--color-black-60, bad-color)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("background-color"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_VARIABLE_COUNT)]
        [NUnit.Framework.Test]
        public virtual void VariableCycleRefTest1() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--one", "calc(var(--two) + 20px)");
            styles.Put("--two", "calc(var(--one) - 20px)");
            styles.Put("margin", "var(--two)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("margin"));
        }

        [NUnit.Framework.Test]
        public virtual void VarAsPrimaryTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--value", "55px");
            styles.Put("--default", "33px");
            styles.Put("margin", "var(var(--value), --default)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("--default", styles.Get("margin-top"));
        }

        [NUnit.Framework.Test]
        public virtual void VarAsPrimaryTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--value", "55px");
            styles.Put("--default", "33px");
            styles.Put("margin", "var(var(--value), var(--default))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("33px", styles.Get("margin-top"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_VARIABLE_COUNT)]
        [NUnit.Framework.Test]
        public virtual void VariableCycleRefTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--one", "var(--two)");
            styles.Put("--two", "var(--one)");
            styles.Put("margin", "var(--two)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("margin"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_VARIABLE_COUNT)]
        [NUnit.Framework.Test]
        public virtual void VariableCycleRefTest3() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--one", "var(--two)");
            styles.Put("--two", "var(--one)");
            styles.Put("border", "1px dotted var(--two)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.IsNull(styles.Get("border"));
        }

        [NUnit.Framework.Test]
        public virtual void VariableInFontShorthandTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--one", "small-caps");
            styles.Put("--two", "var(--one, bold)");
            styles.Put("font", "italic var(--one) var(--two, thin) 12px/30px \"Fira Sans\", sans-serif");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("small-caps", styles.Get("font-variant"));
            NUnit.Framework.Assert.AreEqual("initial", styles.Get("font-weight"));
            NUnit.Framework.Assert.AreEqual("12px", styles.Get("font-size"));
            NUnit.Framework.Assert.AreEqual("30px", styles.Get("line-height"));
            NUnit.Framework.Assert.AreEqual("\"Fira Sans\",sans-serif", styles.Get("font-family"));
            NUnit.Framework.Assert.AreEqual("italic", styles.Get("font-style"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveComplexVariableWithFallbackOnDefaultTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-black-60", "bad-color");
            styles.Put("--color-to-use", "var(--color-black-60, rgb(0, 255, 0))");
            styles.Put("--border-dotted", "1px dotted var(--color-to-use)");
            styles.Put("border", "var(--border-dotted)");
            CssVariableUtil.ResolveCssVariables(styles);
            //it is expected to see rgb(0, 255, 0), but due to variable resolving mechanism we can't resolve defaults properly
            NUnit.Framework.Assert.AreEqual(15, styles.Count);
            NUnit.Framework.Assert.AreEqual("dotted", styles.Get("border-right-style"));
            NUnit.Framework.Assert.AreEqual("1px", styles.Get("border-right-width"));
            NUnit.Framework.Assert.AreEqual("initial", styles.Get("border-right-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveNestedComplexVariableTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("--color-black", "#000000");
            styles.Put("--color-black-60", "#00000060");
            styles.Put("--border-dotted", "var(--color-black-60, var(--color-black, #0000010)) dotted var(--thickness, 2px)"
                );
            CssVariableUtil.ResolveCssVariables(styles);
            //variables themselves are not resolve
            NUnit.Framework.Assert.AreEqual("var(--color-black-60, var(--color-black, #0000010)) dotted var(--thickness, 2px)"
                , styles.Get("--border-dotted"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveNestedComplexVariableTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("--color-black", "rgb(255, 255, 0)");
            styles.Put("--color-black-60", "rgb(0, 255, 0)");
            styles.Put("border", "var(--thickness, 2px) dotted var(--color-black-60, var(--color-black, #0000010))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual(15, styles.Count);
            NUnit.Framework.Assert.AreEqual("dotted", styles.Get("border-right-style"));
            NUnit.Framework.Assert.AreEqual("1px", styles.Get("border-right-width"));
            NUnit.Framework.Assert.AreEqual("rgb(0,255,0)", styles.Get("border-right-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVarInFunctionTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("border-right-width", "calc(var(--thickness))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("calc(1px)", styles.Get("border-right-width"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVarInFunctionTest2() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("border-right-width", "calc(var(--thickness) + 20px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("calc(1px + 20px)", styles.Get("border-right-width"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVarInFunctionTest3() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("border-right-width", "calc(20px + var(--thickness) + 20px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("calc(20px + 1px + 20px)", styles.Get("border-right-width"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveVarInFunctionTest4() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("border-right-width", "calc(20px + var(--invalid, var(--thickness, 20px)) + 20px)");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual("calc(20px + 1px + 20px)", styles.Get("border-right-width"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveNestedComplexVariableWithWhitespacesTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--thickness", "1px");
            styles.Put("--color-black", "black");
            styles.Put("--color-black-60", "gray");
            styles.Put("border", "var(  --thickness   , 2px) dotted var(  --color-black-60, var(    --color-black,   #0000010   ))  "
                );
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual(15, styles.Count);
            NUnit.Framework.Assert.AreEqual("dotted", styles.Get("border-right-style"));
            NUnit.Framework.Assert.AreEqual("1px", styles.Get("border-right-width"));
            NUnit.Framework.Assert.AreEqual("gray", styles.Get("border-right-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFunctionWithMultipleParamsTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-l", "48%");
            styles.Put("--color-s", "89%");
            styles.Put("--color-h", "27%");
            styles.Put("box-shadow", "3px solid hsl(var(--color-h),var(--color-s),var(--color-l))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual(4, styles.Count);
            NUnit.Framework.Assert.AreEqual("3px solid hsl(27%,89%,48%)", styles.Get("box-shadow"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFunctionWithMultipleParamsNestedTest() {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            styles.Put("--color-r", "255");
            styles.Put("--color-r-2", "0");
            styles.Put("--color-g", "127");
            styles.Put("--color-b", "0");
            styles.Put("border", "1px dotted rgb(var(--color-r, var(--color-r-2)), var(--color-g), var(--color-b))");
            CssVariableUtil.ResolveCssVariables(styles);
            NUnit.Framework.Assert.AreEqual(16, styles.Count);
            NUnit.Framework.Assert.AreEqual("rgb(255,127,0)", styles.Get("border-bottom-color"));
        }
    }
}
