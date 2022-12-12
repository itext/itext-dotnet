using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
    public class BorderRadiusShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BorderRadiusSlashTest() {
            String shorthandExpression = "20px 40px 40px / 20px 40px 40px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-bottom-left-radius: 40px 40px"
                , "border-bottom-right-radius: 40px 40px", "border-top-left-radius: 20px 20px", "border-top-right-radius: 40px 40px"
                ));
            IShorthandResolver borderRadiusResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants
                .BORDER_RADIUS);
            NUnit.Framework.Assert.IsNotNull(borderRadiusResolver);
            IList<CssDeclaration> resolvedShorthandProps = borderRadiusResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderRadiusSingleTest() {
            String shorthandExpression = " 20px ";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-bottom-left-radius: 20px"
                , "border-bottom-right-radius: 20px", "border-top-left-radius: 20px", "border-top-right-radius: 20px")
                );
            IShorthandResolver borderRadiusResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants
                .BORDER_RADIUS);
            NUnit.Framework.Assert.IsNotNull(borderRadiusResolver);
            IList<CssDeclaration> resolvedShorthandProps = borderRadiusResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }
    }
}
