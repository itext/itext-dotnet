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
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class FlexFlowShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(FlexFlowShorthandResolver));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.FLEX_WRAP, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.FLEX_FLOW, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.FLEX_FLOW, shorthandExpression);
            }
            String[] flexFlowProps = iText.Commons.Utils.StringUtil.Split(shorthandExpression, " ");
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            if (1 == flexFlowProps.Length) {
                CssDeclaration flexDirectionDeclaration = new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, flexFlowProps
                    [0]);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexDirectionDeclaration)) {
                    resolvedProperties.Add(flexDirectionDeclaration);
                }
                else {
                    CssDeclaration flexWrapDeclaration = new CssDeclaration(CommonCssConstants.FLEX_WRAP, flexFlowProps[0]);
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexWrapDeclaration)) {
                        resolvedProperties.Add(flexWrapDeclaration);
                    }
                    else {
                        return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , CommonCssConstants.FLEX_FLOW, shorthandExpression);
                    }
                }
            }
            else {
                if (2 == flexFlowProps.Length) {
                    CssDeclaration flexDirectionDeclaration = new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, flexFlowProps
                        [0]);
                    CssDeclaration flexWrapDeclaration = new CssDeclaration(CommonCssConstants.FLEX_WRAP, flexFlowProps[1]);
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexDirectionDeclaration)) {
                        resolvedProperties.Add(flexDirectionDeclaration);
                    }
                    else {
                        // for some reasons browsers do support flex-wrap flex-direction order
                        flexDirectionDeclaration = new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, flexFlowProps[1]);
                        flexWrapDeclaration = new CssDeclaration(CommonCssConstants.FLEX_WRAP, flexFlowProps[0]);
                        if (CssDeclarationValidationMaster.CheckDeclaration(flexDirectionDeclaration)) {
                            resolvedProperties.Add(flexDirectionDeclaration);
                        }
                        else {
                            return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                                , CommonCssConstants.FLEX_DIRECTION, shorthandExpression);
                        }
                    }
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexWrapDeclaration)) {
                        resolvedProperties.Add(flexWrapDeclaration);
                    }
                    else {
                        return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , CommonCssConstants.FLEX_WRAP, shorthandExpression);
                    }
                }
                else {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_FLOW, shorthandExpression);
                }
            }
            FillUnresolvedPropertiesWithDefaultValues(resolvedProperties);
            return resolvedProperties;
        }

        private static IList<CssDeclaration> HandleExpressionError(String logMessage, String attribute, String shorthandExpression
            ) {
            LOGGER.LogWarning(MessageFormatUtil.Format(logMessage, attribute, shorthandExpression));
            return JavaCollectionsUtil.EmptyList<CssDeclaration>();
        }

        private void FillUnresolvedPropertiesWithDefaultValues(IList<CssDeclaration> resolvedProperties) {
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_DIRECTION)
                )) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_DIRECTION, CssDefaults.GetDefaultValue(CommonCssConstants
                    .FLEX_DIRECTION)));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_WRAP))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_WRAP, CssDefaults.GetDefaultValue(CommonCssConstants
                    .FLEX_WRAP)));
            }
        }
    }
}
