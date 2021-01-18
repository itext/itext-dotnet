using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    public class FlexFlowShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetValuesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String initialShorthand = CommonCssConstants.INITIAL;
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
            String inheritShorthand = CommonCssConstants.INHERIT;
            resolvedShorthand = resolver.ResolveShorthand(inheritShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[1].GetExpression());
            String unsetShorthand = CommonCssConstants.UNSET;
            resolvedShorthand = resolver.ResolveShorthand(unsetShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void InitialWithSpacesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String initialWithSpacesShorthand = "  initial  ";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialWithSpacesShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, Count = 3)]
        public virtual void ContainsInitialOrInheritOrUnsetShorthandTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String containsInitialShorthand = "row initial ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsInitialShorthand
                ));
            String containsInheritShorthand = "inherit wrap";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsInheritShorthand
                ));
            String containsUnsetShorthand = "wrap unset";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsUnsetShorthand
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY, Count = 2)]
        public virtual void EmptyShorthandTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String emptyShorthand = "";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(emptyShorthand)
                );
            String shorthandWithSpaces = "    ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(shorthandWithSpaces
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneDirectionValueTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "column";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("column", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("nowrap", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneWrapValueTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "wrap-reverse";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 flex-direction shall be "row" and flex-wrap shall be "wrap-reverse"
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("wrap-reverse", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("nowrap", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneInvalidValueTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_DIRECTION, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.FLEX_WRAP, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("nowrap", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithDirectionAndWrapValuesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "row-reverse wrap";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 flex-direction shall be "row-reverse" and flex-wrap shall be "wrap"
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithWrapAndDirectionValuesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "wrap-reverse column";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 flex-direction shall be "column" and flex-wrap shall be "wrap-reverse"
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithTwoDirectionValuesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "column-reverse row";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithTwoWrapValuesTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "nowrap wrap-reverse";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithTwoValuesAndSecondIsInvalidTest() {
            IShorthandResolver resolver = new FlexFlowShorthandResolver();
            String shorthand = "column-reverse invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }
    }
}
