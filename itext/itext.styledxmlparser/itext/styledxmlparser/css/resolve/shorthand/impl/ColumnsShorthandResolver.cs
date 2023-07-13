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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>Shorthand resolver for the column property.</summary>
    /// <remarks>
    /// Shorthand resolver for the column property.
    /// This property is a shorthand for the column-count and column-width properties.
    /// </remarks>
    public class ColumnsShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.ColumnsShorthandResolver
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="ColumnsShorthandResolver"/>
        /// instance.
        /// </summary>
        public ColumnsShorthandResolver() {
        }

        //empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.COLUMN_COUNT, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.COLUMN_WIDTH, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMNS, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.COLUMNS, shorthandExpression);
            }
            String[] properties = iText.Commons.Utils.StringUtil.Split(shorthandExpression, " ");
            if (properties.Length > 2) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMNS, shorthandExpression);
            }
            IList<CssDeclaration> result = new List<CssDeclaration>(2);
            foreach (String property in properties) {
                CssDeclaration declaration = ProcessProperty(property);
                if (declaration != null) {
                    result.Add(declaration);
                }
                if (declaration == null && !CommonCssConstants.AUTO.Equals(property)) {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.COLUMNS, shorthandExpression);
                }
            }
            if (result.Count == 2 && result[0].GetProperty().Equals(result[1].GetProperty())) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMNS, shorthandExpression);
            }
            return result;
        }

        private static CssDeclaration ProcessProperty(String value) {
            if (CssTypesValidationUtils.IsMetricValue(value) || CssTypesValidationUtils.IsRelativeValue(value)) {
                return new CssDeclaration(CommonCssConstants.COLUMN_WIDTH, value);
            }
            if (CssTypesValidationUtils.IsNumber(value)) {
                return new CssDeclaration(CommonCssConstants.COLUMN_COUNT, value);
            }
            return null;
        }

        private static IList<CssDeclaration> HandleExpressionError(String logMessage, String attribute, String shorthandExpression
            ) {
            LOGGER.LogWarning(MessageFormatUtil.Format(logMessage, attribute, shorthandExpression));
            return JavaCollectionsUtil.EmptyList<CssDeclaration>();
        }
    }
}
