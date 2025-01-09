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
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>Shorthand resolver for the column-rule property.</summary>
    /// <remarks>
    /// Shorthand resolver for the column-rule property.
    /// This property is a shorthand for the column-rule-width, column-rule-style, and column-rule-color  properties.
    /// </remarks>
    public class ColumnRuleShortHandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.ColumnRuleShortHandResolver
            ));

        private readonly CssEnumValidator borderStyleValidators = new CssEnumValidator(CommonCssConstants.BORDER_STYLE_VALUES
            );

        private readonly CssEnumValidator borderWithValidators = new CssEnumValidator(CommonCssConstants.BORDER_WIDTH_VALUES
            );

        /// <summary>
        /// Creates a new
        /// <see cref="ColumnsShorthandResolver"/>
        /// instance.
        /// </summary>
        public ColumnRuleShortHandResolver() {
        }

        //empty constructor
        /// <summary>Resolves a shorthand expression.</summary>
        /// <param name="shorthandExpression">the shorthand expression</param>
        /// <returns>a list of CSS declaration</returns>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.COLUMN_RULE_COLOR, shorthandExpression)
                    , new CssDeclaration(CommonCssConstants.COLUMN_RULE_WIDTH, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .COLUMN_RULE_STYLE, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMN_RULE, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.COLUMN_RULE, shorthandExpression);
            }
            int maxProperties = 3;
            IList<String> properties = CssUtils.ExtractShorthandProperties(shorthandExpression)[0];
            if (properties.Count > maxProperties) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMN_RULE, shorthandExpression);
            }
            IList<CssDeclaration> result = new List<CssDeclaration>(maxProperties);
            foreach (String property in properties) {
                String cleanProperty = property.Trim();
                CssDeclaration declaration = ProcessProperty(cleanProperty);
                if (declaration != null) {
                    result.Add(declaration);
                }
                if (declaration == null) {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.COLUMN_RULE_STYLE, shorthandExpression);
                }
            }
            return result;
        }

        private CssDeclaration ProcessProperty(String value) {
            if (CssTypesValidationUtils.IsMetricValue(value) || CssTypesValidationUtils.IsRelativeValue(value) || borderWithValidators
                .IsValid(value)) {
                return new CssDeclaration(CommonCssConstants.COLUMN_RULE_WIDTH, value);
            }
            if (CssTypesValidationUtils.IsColorProperty(value)) {
                return new CssDeclaration(CommonCssConstants.COLUMN_RULE_COLOR, value);
            }
            if (borderStyleValidators.IsValid(value)) {
                return new CssDeclaration(CommonCssConstants.COLUMN_RULE_STYLE, value);
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
