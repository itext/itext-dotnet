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
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for list styles.
    /// </summary>
    public class ListStyleShorthandResolver : IShorthandResolver {
        /// <summary>The list style types (disc, decimal,...).</summary>
        private static readonly ICollection<String> LIST_STYLE_TYPE_VALUES = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.DISC, CommonCssConstants.ARMENIAN, CommonCssConstants
            .CIRCLE, CommonCssConstants.CJK_IDEOGRAPHIC, CommonCssConstants.DECIMAL, CommonCssConstants.DECIMAL_LEADING_ZERO
            , CommonCssConstants.GEORGIAN, CommonCssConstants.HEBREW, CommonCssConstants.HIRAGANA, CommonCssConstants
            .HIRAGANA_IROHA, CommonCssConstants.LOWER_ALPHA, CommonCssConstants.LOWER_GREEK, CommonCssConstants.LOWER_LATIN
            , CommonCssConstants.LOWER_ROMAN, CommonCssConstants.NONE, CommonCssConstants.SQUARE, CommonCssConstants
            .UPPER_ALPHA, CommonCssConstants.UPPER_LATIN, CommonCssConstants.UPPER_ROMAN)));

        /// <summary>The list style positions (inside, outside).</summary>
        private static readonly ICollection<String> LIST_STYLE_POSITION_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.INSIDE, CommonCssConstants.OUTSIDE)));

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.LIST_STYLE_TYPE, shorthandExpression), 
                    new CssDeclaration(CommonCssConstants.LIST_STYLE_POSITION, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .LIST_STYLE_IMAGE, shorthandExpression));
            }
            IList<String> props = CssUtils.ExtractShorthandProperties(shorthandExpression)[0];
            String listStyleTypeValue = null;
            String listStylePositionValue = null;
            String listStyleImageValue = null;
            foreach (String value in props) {
                if (value.Contains("url(") || CssGradientUtil.IsCssLinearGradientValue(value) || (CommonCssConstants.NONE.
                    Equals(value) && listStyleTypeValue != null)) {
                    listStyleImageValue = value;
                }
                else {
                    if (LIST_STYLE_TYPE_VALUES.Contains(value)) {
                        listStyleTypeValue = value;
                    }
                    else {
                        if (LIST_STYLE_POSITION_VALUES.Contains(value)) {
                            listStylePositionValue = value;
                        }
                    }
                }
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            resolvedDecl.Add(new CssDeclaration(CommonCssConstants.LIST_STYLE_TYPE, listStyleTypeValue == null ? CommonCssConstants
                .INITIAL : listStyleTypeValue));
            resolvedDecl.Add(new CssDeclaration(CommonCssConstants.LIST_STYLE_POSITION, listStylePositionValue == null
                 ? CommonCssConstants.INITIAL : listStylePositionValue));
            resolvedDecl.Add(new CssDeclaration(CommonCssConstants.LIST_STYLE_IMAGE, listStyleImageValue == null ? CommonCssConstants
                .INITIAL : listStyleImageValue));
            return resolvedDecl;
        }
    }
}
