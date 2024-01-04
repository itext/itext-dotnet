/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
