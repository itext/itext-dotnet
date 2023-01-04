/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
