using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    public class BackgroundPositionShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialValueTest() {
            String shorthandExpression = "initial";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: initial"
                , "background-position-y: initial"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY)]
        public virtual void FullEmptyValueTest() {
            String shorthandExpression = " ";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY)]
        public virtual void EmptyValueTest() {
            String shorthandExpression = "50pt,  , 20pt";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidXValueTest() {
            String shorthandExpression = "left right";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void NotExistingXValueTest() {
            String shorthandExpression = "30jacoco 50pt";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidYValueTest() {
            String shorthandExpression = "top bottom";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void NotExistingYValueTest() {
            String shorthandExpression = "50pt 30jacoco";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidNumericValueTest() {
            String shorthandExpression = "50px left top";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidNotShortNumericValueTest() {
            String shorthandExpression = "50pt 30px 10pt";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidTopPxShortNumericValueTest() {
            String shorthandExpression = "top 50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void PxCenterShortNumericValueTest() {
            String shorthandExpression = "50px center";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: 50px"
                , "background-position-y: center"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidPxLeftShortNumericValueTest() {
            String shorthandExpression = "50px left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TopPxLeftLargeNumericValueTest() {
            String shorthandExpression = "top 50px left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left"
                , "background-position-y: top 50px"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TopLeftPxLargeNumericValueTest() {
            String shorthandExpression = "top left 50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left 50px"
                , "background-position-y: top"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TopPxLeftPxLargeNumericValueTest() {
            String shorthandExpression = "top 10px left 50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left 50px"
                , "background-position-y: top 10px"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidCenterPxTopLargeNumericValueTest() {
            String shorthandExpression = "center 50px top";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void DoubleHorizontalWithCenterValueTest() {
            String shorthandExpression = "center left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left"
                , "background-position-y: center"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidDoubleHorizontalWithCenterAndVerticalValueTest() {
            String shorthandExpression = "center top left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void LeftCenterValueTest() {
            String shorthandExpression = "left center";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left"
                , "background-position-y: center"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.LogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidLeftTopCenterValueTest() {
            String shorthandExpression = "left bottom center";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void MultiValueMissedValueTest() {
            String shorthandExpression = "left,top";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-position-x: left,center"
                , "background-position-y: center,top"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND_POSITION
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }
    }
}
