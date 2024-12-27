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
