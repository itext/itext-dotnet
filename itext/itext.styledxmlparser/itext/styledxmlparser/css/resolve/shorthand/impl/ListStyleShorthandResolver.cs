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
