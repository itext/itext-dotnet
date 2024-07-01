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
