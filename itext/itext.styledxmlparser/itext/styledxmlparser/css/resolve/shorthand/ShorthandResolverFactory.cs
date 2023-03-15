/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    /// <summary>A factory for creating ShorthandResolver objects.</summary>
    public class ShorthandResolverFactory {
        /// <summary>The map of shorthand resolvers.</summary>
        private static readonly IDictionary<String, IShorthandResolver> shorthandResolvers;

        static ShorthandResolverFactory() {
            shorthandResolvers = new Dictionary<String, IShorthandResolver>();
            shorthandResolvers.Put(CommonCssConstants.BACKGROUND, new BackgroundShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BACKGROUND_POSITION, new BackgroundPositionShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER, new BorderShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_BOTTOM, new BorderBottomShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_COLOR, new BorderColorShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_LEFT, new BorderLeftShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_RADIUS, new BorderRadiusShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_RIGHT, new BorderRightShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_STYLE, new BorderStyleShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_TOP, new BorderTopShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.BORDER_WIDTH, new BorderWidthShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.FONT, new FontShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.LIST_STYLE, new ListStyleShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.MARGIN, new MarginShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.OUTLINE, new OutlineShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.PADDING, new PaddingShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.TEXT_DECORATION, new TextDecorationShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.FLEX, new FlexShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.FLEX_FLOW, new FlexFlowShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.GAP, new GapShorthandResolver());
            shorthandResolvers.Put(CommonCssConstants.PLACE_ITEMS, new PlaceItemsShorthandResolver());
        }

        /// <summary>Gets a shorthand resolver.</summary>
        /// <param name="shorthandProperty">the property</param>
        /// <returns>the shorthand resolver</returns>
        public static IShorthandResolver GetShorthandResolver(String shorthandProperty) {
            return shorthandResolvers.Get(shorthandProperty);
        }
    }
}
