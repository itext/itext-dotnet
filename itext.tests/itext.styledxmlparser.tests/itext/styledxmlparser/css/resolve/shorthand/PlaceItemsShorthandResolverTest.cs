using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    public class PlaceItemsShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetValuesTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String initialShorthand = CommonCssConstants.INITIAL;
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
            String inheritShorthand = CommonCssConstants.INHERIT;
            resolvedShorthand = resolver.ResolveShorthand(inheritShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INHERIT, resolvedShorthand[1].GetExpression());
            String unsetShorthand = CommonCssConstants.UNSET;
            resolvedShorthand = resolver.ResolveShorthand(unsetShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNSET, resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void InitialWithSpacesTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String initialWithSpacesShorthand = "  initial  ";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialWithSpacesShorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.INITIAL, resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, Count = 3)]
        public virtual void ContainsInitialOrInheritOrUnsetShorthandTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String containsInitialShorthand = "start initial ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsInitialShorthand
                ));
            String containsInheritShorthand = "inherit safe end";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsInheritShorthand
                ));
            String containsUnsetShorthand = "baseline unset";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(containsUnsetShorthand
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY, Count = 2)]
        public virtual void EmptyShorthandTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String emptyShorthand = "";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(emptyShorthand)
                );
            String shorthandWithSpaces = "    ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolver.ResolveShorthand(shorthandWithSpaces
                ));
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneValidWordTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "baseline";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("baseline", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("baseline", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneInvalidAlignItemsWordTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "legacy";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneInvalidWordTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe start";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 align-items and justify-items shall be "unsafe start"
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("start", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneWordAlignItemsAndOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "center legacy";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("center", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAndFirstWordIsInvalidTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid self-end";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("self-end", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAndSecondWordIsInvalidTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "flex-start invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("flex-start", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneWordAlignItemsAndTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "flex-start legacy right";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("flex-start", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy right", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneWordAlignItemsAndInvalidTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "flex-start legacy invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("flex-start", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy invalid", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsAndOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe flex-start normal";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 align-items shall be "unsafe flex-start" and justify-items shall be "normal"
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("flex-start normal", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsAndInvalidOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe flex-start invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("flex-start invalid", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithThreeWordsAndInvalidAlignItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid safe self-end";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("safe self-end", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsAndTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "first baseline legacy center";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("first baseline", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy center", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsAndInvalidTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "first baseline invalid center";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("first baseline", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid center", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithInvalidTwoWordsAlignItemsAndTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid baseline legacy left";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            // TODO DEVSIX-4933 resulting List shall be empty
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("invalid baseline", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("legacy left", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void ShorthandWithFiveWordsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "last baseline unsafe safe center";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList(), resolvedShorthand);
        }
    }
}
