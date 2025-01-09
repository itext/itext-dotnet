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
    public class GridItemShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetValuesTest() {
            IShorthandResolver resolver = new GridRowShorthandResolver();
            String initialShorthand = CommonCssConstants.INITIAL;
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(initialShorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
            String inheritShorthand = CommonCssConstants.INHERIT;
            resolvedShorthand = resolver.ResolveShorthand(inheritShorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
            String unsetShorthand = CommonCssConstants.UNSET;
            resolvedShorthand = resolver.ResolveShorthand(unsetShorthand);
            NUnit.Framework.Assert.AreEqual(0, resolvedShorthand.Count);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            , Count = 4)]
        public virtual void EmptyShorthandTest() {
            IShorthandResolver resolver = new GridColumnShorthandResolver();
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
        public virtual void BasicRowValuesTest() {
            IShorthandResolver resolver = new GridRowShorthandResolver();
            String shorthand = "span 2 / 4";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_ROW_START, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_ROW_END, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("span 2", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("4", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void BasicColumnValuesTest() {
            IShorthandResolver resolver = new GridColumnShorthandResolver();
            String shorthand = "3 / span 6";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_COLUMN_START, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_COLUMN_END, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("3", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("span 6", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void SingleValueTest() {
            IShorthandResolver resolver = new GridColumnShorthandResolver();
            String shorthand = "3";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_COLUMN_START, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_COLUMN_END, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("3", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("3", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void SingleValueSpanTest() {
            IShorthandResolver resolver = new GridColumnShorthandResolver();
            String shorthand = "span 3";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_COLUMN_START, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual("span 3", resolvedShorthand[0].GetExpression());
        }
    }
}
