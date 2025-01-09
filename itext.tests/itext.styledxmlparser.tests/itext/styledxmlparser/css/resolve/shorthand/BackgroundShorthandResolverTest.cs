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
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
    public class BackgroundShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BackgroundTest01() {
            String shorthandExpression = "red url('img.gif') 25%/50px 150px repeat-y border-box content-box fixed";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: red"
                , "background-image: url('img.gif')", "background-position: 25%", "background-size: 50px 150px", "background-repeat: repeat-y"
                , "background-origin: border-box", "background-clip: content-box", "background-attachment: fixed"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundTest02() {
            String shorthandExpression = "url('img.gif') red 25%/50px 150px repeat-y fixed border-box content-box";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: red"
                , "background-image: url('img.gif')", "background-position: 25%", "background-size: 50px 150px", "background-repeat: repeat-y"
                , "background-origin: border-box", "background-clip: content-box", "background-attachment: fixed"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundTest03() {
            String shorthandExpression = "url('img.gif') 25%/50px 150px fixed border-box";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: url('img.gif')", "background-position: 25%", "background-size: 50px 150px", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: fixed"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage("Was not able to define one of the background CSS shorthand properties: rgdbq(150,90,60)")]
        public virtual void BackgroundTest05() {
            String shorthandExpression = "rgdbq(150,90,60) url'smiley.gif') repeat-x scroll 20 60%";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundTest06() {
            String shorthandExpression = "DarkOliveGreen fixed center";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: darkolivegreen"
                , "background-image: none", "background-position: center", "background-size: auto", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: fixed"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidSizeTest1() {
            String shorthandExpression = "50px/50";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidSizeTest2() {
            String shorthandExpression = "50px/repeat";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidSizeTest3() {
            String shorthandExpression = "50px/left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashInvalidSizeTest4() {
            String shorthandExpression = "50px/url(img.jpg)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashInvalidSizeTest5() {
            String shorthandExpression = "50px/";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void BackgroundWithAnotherShorthandFailedTest() {
            String shorthandExpression = "no-repeat left right";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidPositionTest1() {
            String shorthandExpression = "50/50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidPositionTest2() {
            String shorthandExpression = "cover/50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashInvalidPositionTest3() {
            String shorthandExpression = "repeat/50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashInvalidPositionTest4() {
            String shorthandExpression = "url(img.jpg)/50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashInvalidPositionTest5() {
            String shorthandExpression = "/50px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            , Count = 3)]
        public virtual void BackgroundIncorrectPositionTest() {
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            String[] incorrectPositions = new String[] { "cover", "auto", "contain" };
            foreach (String incorrectPosition in incorrectPositions) {
                IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(incorrectPosition);
                CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
            }
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithMultiSlashTest() {
            String shorthandExpression = "50px 5px/25px 5%";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: none", "background-position: 50px 5px", "background-size: 25px 5%", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithMultiSlashFailedOnSizeTest1() {
            String shorthandExpression = "50px 5px/25px 5";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithMultiSlashFailedOnSizeTest2() {
            String shorthandExpression = "50px 5px/25px left";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithMultiSlashFailedOnPositionTest1() {
            String shorthandExpression = "50 5px/25px 5%";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithMultiSlashFailedOnPositionTest2() {
            String shorthandExpression = "cover 5px/25px 5%";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithTwoSlashesTest1() {
            String shorthandExpression = "5px/25px 5%/20px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithTwoSlashesTest2() {
            String shorthandExpression = "5px/25px/5%";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithSlashAndSpaceTest1() {
            String shorthandExpression = "5px / 25px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: none", "background-position: 5px", "background-size: 25px", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithSlashAndSpaceTest2() {
            String shorthandExpression = "5px/ 25px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: none", "background-position: 5px", "background-size: 25px", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundWithSlashAndSpaceTest3() {
            String shorthandExpression = "5px /25px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: none", "background-position: 5px", "background-size: 25px", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest1() {
            String shorthandExpression = "repeat / 25px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest2() {
            String shorthandExpression = "5px / repeat";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest3() {
            String shorthandExpression = "5px /repeat";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest4() {
            String shorthandExpression = "5px/ repeat-y";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest5() {
            String shorthandExpression = "repeat-x/ 20px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_PROPERTY)]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest6() {
            String shorthandExpression = "no-repeat /20px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest7() {
            String shorthandExpression = "20px /";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
            )]
        public virtual void BackgroundWithSlashAndSpaceIncorrectTest8() {
            String shorthandExpression = "/ 20px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundInitialInheritUnsetTest() {
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            String[] globalExpressions = new String[] { "initial", "inherit", "unset" };
            foreach (String globalExpression in globalExpressions) {
                ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: "
                     + globalExpression, "background-image: " + globalExpression, "background-position: " + globalExpression
                    , "background-size: " + globalExpression, "background-repeat: " + globalExpression, "background-origin: "
                     + globalExpression, "background-clip: " + globalExpression, "background-attachment: " + globalExpression
                    ));
                IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(globalExpression);
                CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void BackgroundEmptyShorthandTest() {
            String shorthandExpression = "";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void BackgroundEmptyShorthandWithSpaceTest() {
            String shorthandExpression = " ";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void MultiBackgroundEmptyShorthandTest1() {
            String shorthandExpression = "none,,none";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void MultiBackgroundEmptyShorthandTest2() {
            String shorthandExpression = "none,none,";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void MultiBackgroundEmptyShorthandTest3() {
            String shorthandExpression = ",none,none";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
            )]
        public virtual void MultiBackgroundEmptyShorthandWithSpaceTest() {
            String shorthandExpression = "none, ,none";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundDefaultValuesShorthandTest() {
            String shorthandExpression = "none";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: none", "background-position: 0% 0%", "background-size: auto", "background-repeat: repeat"
                , "background-origin: padding-box", "background-clip: border-box", "background-attachment: scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ONLY_THE_LAST_BACKGROUND_CAN_INCLUDE_BACKGROUND_COLOR
            )]
        public virtual void BackgroundColorNotLastTest() {
            String shorthandExpression = "url('img.gif') red, url('img2.gif')";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundColorInImageTest() {
            String shorthandExpression = "url('img.gif'), url('img2.gif') blue";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: blue"
                , "background-image: url('img.gif'),url('img2.gif')", "background-position: 0% 0%,0% 0%", "background-size: auto,auto"
                , "background-repeat: repeat,repeat", "background-origin: padding-box,padding-box", "background-clip: border-box,border-box"
                , "background-attachment: scroll,scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundColorNotInImageTest() {
            String shorthandExpression = "url('img.gif'), url('img2.gif'), blue";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: blue"
                , "background-image: url('img.gif'),url('img2.gif'),none", "background-position: 0% 0%,0% 0%,0% 0%", "background-size: auto,auto,auto"
                , "background-repeat: repeat,repeat,repeat", "background-origin: padding-box,padding-box,padding-box", 
                "background-clip: border-box,border-box,border-box", "background-attachment: scroll,scroll,scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void BackgroundDoubleColorTest() {
            String shorthandExpression = "url('img.gif'), url('img2.gif') red blue";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundMultiImageTest() {
            String shorthandExpression = "url('img.gif'), url('img2.gif'), url('img3.gif')";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: transparent"
                , "background-image: url('img.gif'),url('img2.gif'),url('img3.gif')", "background-position: 0% 0%,0% 0%,0% 0%"
                , "background-size: auto,auto,auto", "background-repeat: repeat,repeat,repeat", "background-origin: padding-box,padding-box,padding-box"
                , "background-clip: border-box,border-box,border-box", "background-attachment: scroll,scroll,scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION)]
        public virtual void BackgroundDoubleImageTest() {
            String shorthandExpression = "url('img.gif'), url('img2.gif') url('img3.gif')";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(new List<String>());
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundMultiImageWithOtherPropsTest() {
            String shorthandExpression = "url('img.gif') 5px/5% repeat-x border-box padding-box fixed," + " url('img2.gif') left/50px repeat-y border-box border-box local,"
                 + "url('img3.gif') center/cover no-repeat padding-box padding-box scroll red";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: red"
                , "background-image: url('img.gif'),url('img2.gif'),url('img3.gif')", "background-position: 5px,left,center"
                , "background-size: 5%,50px,cover", "background-repeat: repeat-x,repeat-y,no-repeat", "background-origin: border-box,border-box,padding-box"
                , "background-clip: padding-box,border-box,padding-box", "background-attachment: fixed,local,scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundMultiImageWithOtherPropsMissedTest() {
            String shorthandExpression = "url('img.gif') 5px/5% repeat-x fixed," + " repeat-y border-box border-box local,"
                 + "url('img3.gif') center/cover padding-box padding-box red";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("background-color: red"
                , "background-image: url('img.gif'),none,url('img3.gif')", "background-position: 5px,0% 0%,center", "background-size: 5%,auto,cover"
                , "background-repeat: repeat-x,repeat-y,repeat", "background-origin: padding-box,border-box,padding-box"
                , "background-clip: border-box,border-box,padding-box", "background-attachment: fixed,local,scroll"));
            IShorthandResolver backgroundResolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BACKGROUND
                );
            NUnit.Framework.Assert.IsNotNull(backgroundResolver);
            IList<CssDeclaration> resolvedShorthandProps = backgroundResolver.ResolveShorthand(shorthandExpression);
            CssShorthandResolverTest.CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }
    }
}
