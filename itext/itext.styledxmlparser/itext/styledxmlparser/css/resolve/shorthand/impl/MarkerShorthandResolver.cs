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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class MarkerShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.MarkerShorthandResolver
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="MarkerShorthandResolver"/>
        /// instance.
        /// </summary>
        public MarkerShorthandResolver() {
        }

        //empty constructor
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.MARKER_START, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.MARKER_MID, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .MARKER_END, shorthandExpression));
            }
            String expression = shorthandExpression.Trim();
            if (String.IsNullOrEmpty(expression)) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.MARKER));
                return new List<CssDeclaration>();
            }
            if (!expression.StartsWith(CommonCssConstants.URL + "(") || !expression.EndsWith(")")) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , shorthandExpression));
                return new List<CssDeclaration>();
            }
            return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.MARKER_START, shorthandExpression), new 
                CssDeclaration(CommonCssConstants.MARKER_MID, shorthandExpression), new CssDeclaration(CommonCssConstants
                .MARKER_END, shorthandExpression));
        }
    }
}
