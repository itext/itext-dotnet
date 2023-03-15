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
    public class FlexShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(FlexShorthandResolver));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.FLEX_SHRINK, shorthandExpression), new CssDeclaration(CommonCssConstants.FLEX_BASIS
                    , shorthandExpression));
            }
            if (CommonCssConstants.AUTO.Equals(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, "1"), new CssDeclaration(CommonCssConstants
                    .FLEX_SHRINK, "1"), new CssDeclaration(CommonCssConstants.FLEX_BASIS, CommonCssConstants.AUTO));
            }
            if (CommonCssConstants.NONE.Equals(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FLEX_GROW, "0"), new CssDeclaration(CommonCssConstants
                    .FLEX_SHRINK, "0"), new CssDeclaration(CommonCssConstants.FLEX_BASIS, CommonCssConstants.AUTO));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.FLEX, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.FLEX, shorthandExpression);
            }
            String[] flexProps = iText.Commons.Utils.StringUtil.Split(shorthandExpression, " ");
            IList<CssDeclaration> resolvedProperties;
            switch (flexProps.Length) {
                case 1: {
                    resolvedProperties = ResolveShorthandWithOneValue(flexProps[0]);
                    break;
                }

                case 2: {
                    resolvedProperties = ResolveShorthandWithTwoValues(flexProps[0], flexProps[1]);
                    break;
                }

                case 3: {
                    resolvedProperties = ResolveShorthandWithThreeValues(flexProps[0], flexProps[1], flexProps[2]);
                    break;
                }

                default: {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX, shorthandExpression);
                }
            }
            if (!resolvedProperties.IsEmpty()) {
                FillUnresolvedPropertiesWithDefaultValues(resolvedProperties);
            }
            return resolvedProperties;
        }

        private IList<CssDeclaration> ResolveShorthandWithOneValue(String firstProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                return resolvedProperties;
            }
            else {
                CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                    resolvedProperties.Add(flexBasisDeclaration);
                    return resolvedProperties;
                }
                else {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_GROW, firstProperty);
                }
            }
        }

        private IList<CssDeclaration> ResolveShorthandWithTwoValues(String firstProperty, String secondProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, secondProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                    resolvedProperties.Add(flexShrinkDeclaration);
                    return resolvedProperties;
                }
                else {
                    CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, secondProperty);
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                        resolvedProperties.Add(flexBasisDeclaration);
                        return resolvedProperties;
                    }
                    else {
                        return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , CommonCssConstants.FLEX_BASIS, secondProperty);
                    }
                }
            }
            else {
                CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                    resolvedProperties.Add(flexBasisDeclaration);
                    flexGrowDeclaration.SetExpression(secondProperty);
                    if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                        resolvedProperties.Add(flexGrowDeclaration);
                        return resolvedProperties;
                    }
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_GROW, secondProperty);
                }
                else {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_SHRINK, secondProperty);
                }
            }
        }

        private IList<CssDeclaration> ResolveShorthandWithThreeValues(String firstProperty, String secondProperty, 
            String thirdProperty) {
            IList<CssDeclaration> resolvedProperties = new List<CssDeclaration>();
            CssDeclaration flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, firstProperty);
            if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                resolvedProperties.Add(flexGrowDeclaration);
                CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, secondProperty);
                if (!CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_SHRINK, secondProperty);
                }
                else {
                    resolvedProperties.Add(flexShrinkDeclaration);
                    CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, thirdProperty);
                    if (!CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                        return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , CommonCssConstants.FLEX_BASIS, thirdProperty);
                    }
                    else {
                        resolvedProperties.Add(flexBasisDeclaration);
                        return resolvedProperties;
                    }
                }
            }
            else {
                // For some reason browsers support flex-basis, flex-grow, flex-shrink order as well
                flexGrowDeclaration = new CssDeclaration(CommonCssConstants.FLEX_GROW, secondProperty);
                if (CssDeclarationValidationMaster.CheckDeclaration(flexGrowDeclaration)) {
                    resolvedProperties.Add(flexGrowDeclaration);
                    CssDeclaration flexShrinkDeclaration = new CssDeclaration(CommonCssConstants.FLEX_SHRINK, thirdProperty);
                    if (!CssDeclarationValidationMaster.CheckDeclaration(flexShrinkDeclaration)) {
                        return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , CommonCssConstants.FLEX_SHRINK, thirdProperty);
                    }
                    else {
                        resolvedProperties.Add(flexShrinkDeclaration);
                        CssDeclaration flexBasisDeclaration = new CssDeclaration(CommonCssConstants.FLEX_BASIS, firstProperty);
                        if (!CssDeclarationValidationMaster.CheckDeclaration(flexBasisDeclaration)) {
                            return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                                , CommonCssConstants.FLEX_BASIS, firstProperty);
                        }
                        else {
                            resolvedProperties.Add(flexBasisDeclaration);
                            return resolvedProperties;
                        }
                    }
                }
                else {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.FLEX_GROW, secondProperty);
                }
            }
        }

        private void FillUnresolvedPropertiesWithDefaultValues(IList<CssDeclaration> resolvedProperties) {
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_GROW))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_GROW, CssDefaults.GetDefaultValue(CommonCssConstants
                    .FLEX_GROW)));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_SHRINK))) {
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_SHRINK, CssDefaults.GetDefaultValue(CommonCssConstants
                    .FLEX_SHRINK)));
            }
            if (!resolvedProperties.Any((property) => property.GetProperty().Equals(CommonCssConstants.FLEX_BASIS))) {
                // When flex-basis is omitted from the flex shorthand, its specified value is 0.
                resolvedProperties.Add(new CssDeclaration(CommonCssConstants.FLEX_BASIS, "0"));
            }
        }

        private static IList<CssDeclaration> HandleExpressionError(String logMessage, String attribute, String shorthandExpression
            ) {
            LOGGER.LogWarning(MessageFormatUtil.Format(logMessage, attribute, shorthandExpression));
            return JavaCollectionsUtil.EmptyList<CssDeclaration>();
        }
    }
}
