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
