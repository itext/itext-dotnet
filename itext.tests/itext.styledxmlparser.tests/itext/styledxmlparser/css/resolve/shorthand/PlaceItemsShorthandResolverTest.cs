/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 3)]
        public virtual void ContainsInitialOrInheritOrUnsetShorthandTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String containsInitialShorthand = "start initial ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsInitialShorthand));
            String containsInheritShorthand = "inherit safe end";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsInheritShorthand));
            String containsUnsetShorthand = "baseline unset";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (containsUnsetShorthand));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            , Count = 2)]
        public virtual void EmptyShorthandTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String emptyShorthand = "";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (emptyShorthand));
            String shorthandWithSpaces = "    ";
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolver.ResolveShorthand
                (shorthandWithSpaces));
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneInvalidAlignItemsWordTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "legacy";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneInvalidWordTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe start";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe start", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe start", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithOneWordAlignItemsAndOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = CommonCssConstants.CENTER + " " + CommonCssConstants.LEGACY + " " + CommonCssConstants.
                RIGHT;
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.CENTER, resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.LEGACY + " " + CommonCssConstants.RIGHT, resolvedShorthand
                [1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoWordsAndFirstWordIsInvalidTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid self-end";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoWordsAndSecondWordIsInvalidTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "flex-start invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithOneWordAlignItemsAndInvalidTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "flex-start legacy invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        public virtual void ShorthandWithTwoWordsAlignItemsAndOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe flex-start normal";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.ALIGN_ITEMS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("unsafe flex-start", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.JUSTIFY_ITEMS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("normal", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoWordsAlignItemsAndInvalidOneWordJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "unsafe flex-start invalid";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithThreeWordsAndInvalidAlignItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid safe self-end";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
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
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithTwoWordsAlignItemsAndInvalidTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "first baseline invalid center";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithInvalidTwoWordsAlignItemsAndTwoWordsJustifyItemsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "invalid baseline legacy left";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void ShorthandWithFiveWordsTest() {
            IShorthandResolver resolver = new PlaceItemsShorthandResolver();
            String shorthand = "last baseline unsafe safe center";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.EmptyList<CssDeclaration>(), resolvedShorthand);
        }
    }
}
