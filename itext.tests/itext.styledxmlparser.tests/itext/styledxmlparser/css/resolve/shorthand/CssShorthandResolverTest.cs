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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
    public class CssShorthandResolverTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BorderBottomTest01() {
            String shorthandExpression = "15px dotted blue";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-bottom-width: 15px"
                , "border-bottom-style: dotted", "border-bottom-color: blue"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_BOTTOM
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderLeftTest01() {
            String shorthandExpression = "10px solid #ff0000";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-left-width: 10px"
                , "border-left-style: solid", "border-left-color: #ff0000"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_LEFT
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderRightTest01() {
            String shorthandExpression = "10px double rgb(12,220,100)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-right-width: 10px"
                , "border-right-style: double", "border-right-color: rgb(12,220,100)"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_RIGHT
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTopTest01() {
            String shorthandExpression = "10px hidden rgba(12,225,100,0.7)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: 10px"
                , "border-top-style: hidden", "border-top-color: rgba(12,225,100,0.7)"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_TOP);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest01() {
            String shorthandExpression = "thick groove black";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: thick"
                , "border-right-width: thick", "border-bottom-width: thick", "border-left-width: thick", "border-top-style: groove"
                , "border-right-style: groove", "border-bottom-style: groove", "border-left-style: groove", "border-top-color: black"
                , "border-right-color: black", "border-bottom-color: black", "border-left-color: black"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest02() {
            String shorthandExpression = "groove";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: initial"
                , "border-right-width: initial", "border-bottom-width: initial", "border-left-width: initial", "border-top-style: groove"
                , "border-right-style: groove", "border-bottom-style: groove", "border-left-style: groove", "border-bottom-color: initial"
                , "border-left-color: initial", "border-right-color: initial", "border-top-color: initial"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest03() {
            String shorthandExpression = "inherit";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: inherit"
                , "border-right-width: inherit", "border-bottom-width: inherit", "border-left-width: inherit", "border-top-style: inherit"
                , "border-right-style: inherit", "border-bottom-style: inherit", "border-left-style: inherit", "border-top-color: inherit"
                , "border-right-color: inherit", "border-bottom-color: inherit", "border-left-color: inherit"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest04() {
            String shorthandExpression = "dashed";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: initial"
                , "border-right-width: initial", "border-bottom-width: initial", "border-left-width: initial", "border-top-style: dashed"
                , "border-right-style: dashed", "border-bottom-style: dashed", "border-left-style: dashed", "border-bottom-color: initial"
                , "border-left-color: initial", "border-right-color: initial", "border-top-color: initial"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest05() {
            String shorthandExpression = "dashed green";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: initial"
                , "border-right-width: initial", "border-bottom-width: initial", "border-left-width: initial", "border-top-style: dashed"
                , "border-right-style: dashed", "border-bottom-style: dashed", "border-left-style: dashed", "border-top-color: green"
                , "border-right-color: green", "border-bottom-color: green", "border-left-color: green"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest06() {
            String shorthandExpression = "1px dashed";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: 1px"
                , "border-right-width: 1px", "border-bottom-width: 1px", "border-left-width: 1px", "border-top-style: dashed"
                , "border-right-style: dashed", "border-bottom-style: dashed", "border-left-style: dashed", "border-bottom-color: initial"
                , "border-left-color: initial", "border-right-color: initial", "border-top-color: initial"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderTest07() {
            String shorthandExpression = "1px dashed green";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: 1px"
                , "border-right-width: 1px", "border-bottom-width: 1px", "border-left-width: 1px", "border-top-style: dashed"
                , "border-right-style: dashed", "border-bottom-style: dashed", "border-left-style: dashed", "border-top-color: green"
                , "border-right-color: green", "border-bottom-color: green", "border-left-color: green"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthTest01() {
            String shorthandExpression = "thin medium thick 10px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: thin"
                , "border-right-width: medium", "border-bottom-width: thick", "border-left-width: 10px"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_WIDTH
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthTest02() {
            String shorthandExpression = "thin 20% thick";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: thin"
                , "border-right-width: 20%", "border-bottom-width: thick", "border-left-width: 20%"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_WIDTH
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderWidthTest03() {
            String shorthandExpression = "inherit";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-width: inherit"
                , "border-right-width: inherit", "border-bottom-width: inherit", "border-left-width: inherit"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_WIDTH
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderStyleTest01() {
            String shorthandExpression = "dotted solid double dashed";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-style: dotted"
                , "border-right-style: solid", "border-bottom-style: double", "border-left-style: dashed"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_STYLE
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderStyleTest02() {
            String shorthandExpression = "dotted solid";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-style: dotted"
                , "border-right-style: solid", "border-bottom-style: dotted", "border-left-style: solid"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_STYLE
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderColorTest01() {
            String shorthandExpression = "red rgba(125,0,50,0.4) rgb(12,255,0) #0000ff";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-color: red"
                , "border-right-color: rgba(125,0,50,0.4)", "border-bottom-color: rgb(12,255,0)", "border-left-color: #0000ff"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_COLOR
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BorderColorTest02() {
            String shorthandExpression = "red";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("border-top-color: red"
                , "border-right-color: red", "border-bottom-color: red", "border-left-color: red"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.BORDER_COLOR
                );
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void ListStyleTest01() {
            String shorthandExpression = "square inside url('sqpurple.gif')";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: square"
                , "list-style-position: inside", "list-style-image: url('sqpurple.gif')"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void ListStyleTest02() {
            String shorthandExpression = "inside url('sqpurple.gif')";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: initial"
                , "list-style-position: inside", "list-style-image: url('sqpurple.gif')"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void ListStyleTest03() {
            String shorthandExpression = "inherit";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: inherit"
                , "list-style-position: inherit", "list-style-image: inherit"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void MarginTest01() {
            String shorthandExpression = "2cm -4cm 3cm 4cm";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("margin-top: 2cm"
                , "margin-right: -4cm", "margin-bottom: 3cm", "margin-left: 4cm"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARGIN);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void MarginTest02() {
            String shorthandExpression = "2cm auto 4cm";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("margin-top: 2cm"
                , "margin-right: auto", "margin-bottom: 4cm", "margin-left: auto"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.MARGIN);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void OutlineTest01() {
            String shorthandExpression = "#00ff00 dashed medium";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("outline-color: #00ff00"
                , "outline-style: dashed", "outline-width: medium"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.OUTLINE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void PaddingTest01() {
            String shorthandExpression = "10px 5px 15px 20px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("padding-top: 10px"
                , "padding-right: 5px", "padding-bottom: 15px", "padding-left: 20px"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.PADDING);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void PaddingTest02() {
            String shorthandExpression = "10px 5px";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("padding-top: 10px"
                , "padding-right: 5px", "padding-bottom: 10px", "padding-left: 5px"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.PADDING);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void PaddingTest03() {
            String shorthandExpression = "inherit";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("padding-top: inherit"
                , "padding-right: inherit", "padding-bottom: inherit", "padding-left: inherit"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.PADDING);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void LinearGradientInlistStyleImageTest() {
            String shorthandExpression = "inside linear-gradient(red, green, blue)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: initial"
                , "list-style-position: inside", "list-style-image: linear-gradient(red,green,blue)"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void RepeatingLinearGradientInlistStyleImageTest() {
            String shorthandExpression = "square inside repeating-linear-gradient(45deg, blue 7%, red 10%)";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: square"
                , "list-style-position: inside", "list-style-image: repeating-linear-gradient(45deg,blue 7%,red 10%)")
                );
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void NoneInlistStyleImageTest() {
            String shorthandExpression = "circle none inside";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("list-style-type: circle"
                , "list-style-position: inside", "list-style-image: none"));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.LIST_STYLE);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest01() {
            String shorthandExpression = "italic normal bold 12px/30px Georgia, serif";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: italic"
                , "font-variant: initial", "font-weight: bold", "font-size: 12px", "line-height: 30px", "font-family: georgia,serif"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest02() {
            String shorthandExpression = "bold Georgia, serif";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: initial"
                , "font-variant: initial", "font-weight: bold", "font-size: initial", "line-height: initial", "font-family: georgia,serif"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest03() {
            String shorthandExpression = "inherit";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: inherit"
                , "font-variant: inherit", "font-weight: inherit", "font-size: inherit", "line-height: inherit", "font-family: inherit"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest04() {
            String shorthandExpression = "bold Georgia, serif, \"Times New Roman\"";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: initial"
                , "font-variant: initial", "font-weight: bold", "font-size: initial", "line-height: initial", "font-family: georgia,serif,\"Times New Roman\""
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest05() {
            String shorthandExpression = "italic normal bold 12px/30px Georgia, \"Times New Roman\", serif";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: italic"
                , "font-variant: initial", "font-weight: bold", "font-size: 12px", "line-height: 30px", "font-family: georgia,\"Times New Roman\",serif"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest06() {
            String shorthandExpression = "italic normal bold 12px/30px Georgia    ,   \"Times New Roman\"   ,    serif";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: italic"
                , "font-variant: initial", "font-weight: bold", "font-size: 12px", "line-height: 30px", "font-family: georgia,\"Times New Roman\",serif"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest07() {
            String shorthandExpression = "italic normal bold 12px/30px Georgia    ,   \"Times New Roman\"   ";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: italic"
                , "font-variant: initial", "font-weight: bold", "font-size: 12px", "line-height: 30px", "font-family: georgia,\"Times New Roman\""
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest08() {
            String shorthandExpression = "Georgia,'Times New Roman'";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: initial"
                , "font-variant: initial", "font-weight: initial", "font-size: initial", "line-height: initial", "font-family: georgia,'Times New Roman'"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

        [NUnit.Framework.Test]
        public virtual void FontTest09() {
            String shorthandExpression = "Georgia  ,   'Times New Roman', serif";
            ICollection<String> expectedResolvedProperties = new HashSet<String>(JavaUtil.ArraysAsList("font-style: initial"
                , "font-variant: initial", "font-weight: initial", "font-size: initial", "line-height: initial", "font-family: georgia,'Times New Roman',serif"
                ));
            IShorthandResolver resolver = ShorthandResolverFactory.GetShorthandResolver(CommonCssConstants.FONT);
            NUnit.Framework.Assert.IsNotNull(resolver);
            IList<CssDeclaration> resolvedShorthandProps = resolver.ResolveShorthand(shorthandExpression);
            CompareResolvedProps(resolvedShorthandProps, expectedResolvedProperties);
        }

//\cond DO_NOT_DOCUMENT
        internal static void CompareResolvedProps(IList<CssDeclaration> actual, ICollection<String> expected) {
            ICollection<String> actualSet = new HashSet<String>();
            foreach (CssDeclaration cssDecl in actual) {
                actualSet.Add(cssDecl.ToString());
            }
            bool areDifferent = false;
            StringBuilder sb = new StringBuilder("Resolved styles are different from expected!");
            ICollection<String> expCopy = new SortedSet<String>(expected);
            ICollection<String> actCopy = new SortedSet<String>(actualSet);
            expCopy.RemoveAll(actualSet);
            actCopy.RemoveAll(expected);
            if (!expCopy.IsEmpty()) {
                areDifferent = true;
                sb.Append("\nExpected but not found properties:\n");
                foreach (String expProp in expCopy) {
                    sb.Append(expProp).Append('\n');
                }
            }
            if (!actCopy.IsEmpty()) {
                areDifferent = true;
                sb.Append("\nNot expected but found properties:\n");
                foreach (String actProp in actCopy) {
                    sb.Append(actProp).Append('\n');
                }
            }
            if (areDifferent) {
                NUnit.Framework.Assert.Fail(sb.ToString());
            }
        }
//\endcond
    }
}
