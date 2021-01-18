using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    public class FlexShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY, Count = 2)]
        public virtual void EmptyShorthandTest() {
            String emptyShorthand = "";
            IShorthandResolver resolver = new FlexShorthandResolver();
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (emptyShorthand));
            String shorthandWithSpaces = "    ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (shorthandWithSpaces));
        }

        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetShorthandTest() {
            String initialShorthand = CommonCssConstants.INITIAL;
            IShorthandResolver resolver = new FlexShorthandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[2].GetExpression());
            String inheritShorthand = CommonCssConstants.INHERIT;
            resolvedShorthand = resolver.ResolveShorthand(inheritShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[2].GetExpression());
            String unsetShorthand = CommonCssConstants.UNSET;
            resolvedShorthand = resolver.ResolveShorthand(unsetShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void InitialWithSpacesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String initialWithSpacesShorthand = "  initial  ";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialWithSpacesShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void AutoShorthandTest() {
            String initialShorthand = CommonCssConstants.AUTO;
            IShorthandResolver resolver = new FlexShorthandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.AUTO, resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void NoneShorthandTest() {
            String initialShorthand = CommonCssConstants.NONE;
            IShorthandResolver resolver = new FlexShorthandResolver();
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("0", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("0", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.AUTO, resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, Count = 3)]
        public virtual void ContainsInitialOrInheritOrUnsetShorthandTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String containsInitialShorthand = "1 initial 50px";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsInitialShorthand));
            String containsInheritShorthand = "inherit 2 50px";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsInheritShorthand));
            String containsUnsetShorthand = "0 2 unset";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsUnsetShorthand));
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneUnitlessNumberValueTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("auto", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneUnitNumberValueTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("0", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneInvalidValueTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5pixels";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoUnitlessNumberValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("7", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("auto", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithUnitlessAndUnitNumberValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("7px", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithUnitAndUnitlessNumberValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px 7";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("7", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("1", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoUnitValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px 7px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneUnitlessAndOneInvalidValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoValuesAndFirstIsInvalidTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "invalid 5px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoUnitlessAndOneUnitValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7 10px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_GROW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("5", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_SHRINK, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("7", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_BASIS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("10px", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneUnitAndTwoUnitlessValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px 7 10";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithThreeUnitlessValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7 10";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneUnitlessOneUnitAndOneUnitlessValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7px 10";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithThreeUnitValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px 7px 10px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneUnitOneUnitlessAndOneUnitValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5px 7 10px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithThreeValuesAndFirstIsInvalidTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "invalid 7 10";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithFourValuesTest() {
            IShorthandResolver resolver = new FlexShorthandResolver();
            String shorthand = "5 7 10 13";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolvedShorthand);
        }
    }
}
