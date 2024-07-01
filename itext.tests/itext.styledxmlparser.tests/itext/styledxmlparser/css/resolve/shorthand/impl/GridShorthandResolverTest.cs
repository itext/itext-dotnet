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
