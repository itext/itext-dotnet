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
    public class GridTemplateShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialOrInheritOrUnsetValuesTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
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
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
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
        public virtual void BasicTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "auto 1fr / auto 1fr auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual("auto 1fr", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto 1fr auto", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void LineNamesTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "[linename] 100px / [columnname1] 30% [columnname2] 70%";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual("[linename] 100px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("[columnname1] 30% [columnname2] 70%", resolvedShorthand[1].GetExpression(
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AreaTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "'a a a'    'b b b'";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(1, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[0].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b'", resolvedShorthand[0].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void AreaWithRowsTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "    'a a a' 20%    'b b b' auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[1].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("20% auto", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b'", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void AreaWithRowsAndColumnsTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "    'a a a' 20%    'b b b' auto / auto 1fr auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[2].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("20% auto", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto 1fr auto", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b'", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void AreaWithMissingRowAtTheEndTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "    'a a a' 20% 'b b b' 1fr 'c c c' / auto 1fr auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[2].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("20% 1fr auto", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto 1fr auto", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b' 'c c c'", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void AreaWithMissingRowAtTheStartTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "    'a a a' 'b b b' 1fr 'c c c' 80% / auto 1fr auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[2].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("auto 1fr 80%", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto 1fr auto", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b' 'c c c'", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ComplexAreaTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "[header-top] 'a a a' [header-bottom] [main-top] 'b b b' 1fr [main-bottom] / auto 1fr auto";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[2].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("[header-top] [header-bottom] [main-top] 1fr [main-bottom]", resolvedShorthand
                [0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto 1fr auto", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("'a a a' 'b b b'", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void ComplexAreaWithoutLineNamesTest() {
            IShorthandResolver resolver = new GridTemplateShorthandResolver();
            String shorthand = "'head head' 30px 'nav  main' 1fr 'nav  foot' 30px / 120px 1fr";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_AREAS, resolvedShorthand[2].GetProperty()
                );
            NUnit.Framework.Assert.AreEqual("30px 1fr 30px", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("120px 1fr", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("'head head' 'nav  main' 'nav  foot'", resolvedShorthand[2].GetExpression(
                ));
        }
    }
}
