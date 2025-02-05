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
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class GridShorthandResolverTest : ExtendedITextTest {
        /// <summary>Creates grid shorthand resolver.</summary>
        public GridShorthandResolverTest() {
        }

        [NUnit.Framework.Test]
        public virtual void TemplateAreasTest() {
            IShorthandResolver resolver = new GridShorthandResolver();
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
        public virtual void ColumnFlowTest() {
            IShorthandResolver resolver = new GridShorthandResolver();
            String shorthand = "20% 100px 1fr / auto-flow dense 50px";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_FLOW, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_COLUMNS, resolvedShorthand[2].GetProperty());
            NUnit.Framework.Assert.AreEqual("20% 100px 1fr", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("column dense", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("50px", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void RowFlowTest() {
            IShorthandResolver resolver = new GridShorthandResolver();
            String shorthand = "auto-flow dense auto / 1fr auto minmax(100px, 1fr)";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(3, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_FLOW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_ROWS, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[2].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual("dense", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("auto", resolvedShorthand[1].GetExpression());
            NUnit.Framework.Assert.AreEqual("1fr auto minmax(100px,1fr)", resolvedShorthand[2].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void NoRowTemplateTest() {
            IShorthandResolver resolver = new GridShorthandResolver();
            String shorthand = "auto-flow dense / 1fr auto minmax(100px, 1fr)";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_FLOW, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_COLUMNS, resolvedShorthand[1].GetProperty
                ());
            NUnit.Framework.Assert.AreEqual("dense", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("1fr auto minmax(100px,1fr)", resolvedShorthand[1].GetExpression());
        }

        [NUnit.Framework.Test]
        public virtual void NoColumnTemplateTest() {
            IShorthandResolver resolver = new GridShorthandResolver();
            String shorthand = "1fr auto minmax(100px, 1fr) / auto-flow dense";
            IList<CssDeclaration> resolvedShorthand = resolver.ResolveShorthand(shorthand);
            NUnit.Framework.Assert.AreEqual(2, resolvedShorthand.Count);
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_TEMPLATE_ROWS, resolvedShorthand[0].GetProperty());
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.GRID_AUTO_FLOW, resolvedShorthand[1].GetProperty());
            NUnit.Framework.Assert.AreEqual("1fr auto minmax(100px,1fr)", resolvedShorthand[0].GetExpression());
            NUnit.Framework.Assert.AreEqual("column dense", resolvedShorthand[1].GetExpression());
        }
    }
}
