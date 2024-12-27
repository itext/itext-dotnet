using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
    public class MarkerShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitialValueTest() {
            String shorthandExpression = "initial";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("marker-start: initial"
                , "marker-mid: initial", "marker-end: initial"));
            IShorthandResolver markerResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARKER
                );
            NUnit.Framework.Assert.IsNotNull(markerResolver);
            IList<CssDeclaration> resolvedShorthandProps = markerResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void EmptyValueTest() {
            String shorthandExpression = " ";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver markerResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARKER
                );
            NUnit.Framework.Assert.IsNotNull(markerResolver);
            IList<CssDeclaration> resolvedShorthandProps = markerResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidValueTest() {
            String shorthandExpression = "junk";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver markerResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARKER
                );
            NUnit.Framework.Assert.IsNotNull(markerResolver);
            IList<CssDeclaration> resolvedShorthandProps = markerResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void InvalidUrlTest() {
            String shorthandExpression = "url(test";
            ICollection<String> expectedResolvedProperties = new HashSet<String>();
            IShorthandResolver markerResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARKER
                );
            NUnit.Framework.Assert.IsNotNull(markerResolver);
            IList<CssDeclaration> resolvedShorthandProps = markerResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void ValidTest() {
            String shorthandExpression = "url(markers.svg#arrow)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("marker-start: url(markers.svg#arrow)"
                , "marker-mid: url(markers.svg#arrow)", "marker-end: url(markers.svg#arrow)"));
            IShorthandResolver markerResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARKER
                );
            NUnit.Framework.Assert.IsNotNull(markerResolver);
            IList<CssDeclaration> resolvedShorthandProps = markerResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }
    }
}
