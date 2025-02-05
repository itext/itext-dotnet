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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class ColumnRuleShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetValuesTest() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String initialShorthand = CommonCssConstants.INITIAL;
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[2].GetExpression());
            String inheritShorthand = CommonCssConstants.INHERIT;
            resolvedShorthand = resolver.ResolveShorthand(inheritShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[2].GetExpression());
            String unsetShorthand = CommonCssConstants.UNSET;
            resolvedShorthand = resolver.ResolveShorthand(unsetShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            , Count = 4)]
        public virtual void EmptyShorthandTest() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String emptyShorthand = "";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (emptyShorthand));
            String shorthandWithSpaces = "    ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (shorthandWithSpaces));
            String shorthandWithTabs = "\t";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (shorthandWithTabs));
            String shorthandWithNewLines = "\n";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (shorthandWithNewLines));
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsWidthSingleTest01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "10px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("10px", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsWidthSingleTest02() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "10em";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("10em", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsWidthSingleTest03() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "thin";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("thin", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 1)]
        public virtual void ColumnsWidthSingleInvalidTest01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "10dfx";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 1)]
        public virtual void ColumnsWidthSingleInvalidTest02() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "big";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsStyleSingleTest01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            foreach (String borderStyleValue in CommonCssConstants.BORDER_STYLE_VALUES) {
                IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(borderStyleValue);
                NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
                NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[0].GetProperty());
                NUnit.Framework.Assert.AreEqual(borderStyleValue, resolvedShorthand[0].GetExpression());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 1)]
        public virtual void ColumnsWidthStyleInvalidTest01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "curly";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsColorSingleTest01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand("red");
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("red", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsColorSingleTest02() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand("rgb(10,20,30)");
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("rgb(10,20,30)", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsColorSingleTest03() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand("rgb(10 ,20 ,30)");
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("rgb(10,20,30)", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ColumnsColorSingleTest04() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand("#aabbcc");
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("#aabbcc", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTogether01() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "10px solid red";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("10px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("solid", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("red", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTogether02() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "10px solid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("10px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("solid", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTogether03() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "solid blue";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("solid", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("blue", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleTogether04() {
            IShorthandResolver resolver = new ColumnRuleShortHandResolver();
            String shorthand = "thick inset blue";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_WIDTH, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("thick", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_STYLE, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("inset", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.COLUMN_RULE_COLOR, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("blue", resolvedShorthand[2].GetExpression());
        }
    }
}
