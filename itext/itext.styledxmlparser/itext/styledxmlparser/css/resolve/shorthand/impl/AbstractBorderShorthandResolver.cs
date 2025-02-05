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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// Abstract
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for borders.
    /// </summary>
    public abstract class AbstractBorderShorthandResolver : IShorthandResolver {
        /// <summary>The template for -width properties.</summary>
        private const String _0_WIDTH = "{0}-width";

        /// <summary>The template for -style properties.</summary>
        private const String _0_STYLE = "{0}-style";

        /// <summary>The template for -color properties.</summary>
        private const String _0_COLOR = "{0}-color";

        /// <summary>Gets the prefix of a property.</summary>
        /// <returns>the prefix</returns>
        protected internal abstract String GetPrefix();

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            String widthPropName = MessageFormatUtil.Format(_0_WIDTH, GetPrefix());
            String stylePropName = MessageFormatUtil.Format(_0_STYLE, GetPrefix());
            String colorPropName = MessageFormatUtil.Format(_0_COLOR, GetPrefix());
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(widthPropName, shorthandExpression), new CssDeclaration(stylePropName
                    , shorthandExpression), new CssDeclaration(colorPropName, shorthandExpression));
            }
            IList<String> props = CssUtils.ExtractShorthandProperties(shorthandExpression)[0];
            String borderColorValue = null;
            String borderStyleValue = null;
            String borderWidthValue = null;
            foreach (String value in props) {
                if (CommonCssConstants.INITIAL.Equals(value) || CommonCssConstants.INHERIT.Equals(value)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AbstractBorderShorthandResolver));
                    logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , shorthandExpression));
                    return JavaCollectionsUtil.EmptyList<CssDeclaration>();
                }
                if (CommonCssConstants.BORDER_WIDTH_VALUES.Contains(value) || CssTypesValidationUtils.IsNumber(value) || CssTypesValidationUtils
                    .IsMetricValue(value) || CssTypesValidationUtils.IsRelativeValue(value)) {
                    borderWidthValue = value;
                }
                else {
                    if (CommonCssConstants.BORDER_STYLE_VALUES.Contains(value) || value.Equals(CommonCssConstants.AUTO)) {
                        // AUTO property value is needed for outline property only
                        borderStyleValue = value;
                    }
                    else {
                        if (CssTypesValidationUtils.IsColorProperty(value)) {
                            borderColorValue = value;
                        }
                    }
                }
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            resolvedDecl.Add(new CssDeclaration(widthPropName, borderWidthValue == null ? CommonCssConstants.INITIAL : 
                borderWidthValue));
            resolvedDecl.Add(new CssDeclaration(stylePropName, borderStyleValue == null ? CommonCssConstants.INITIAL : 
                borderStyleValue));
            resolvedDecl.Add(new CssDeclaration(colorPropName, borderColorValue == null ? CommonCssConstants.INITIAL : 
                borderColorValue));
            return resolvedDecl;
        }
    }
}
