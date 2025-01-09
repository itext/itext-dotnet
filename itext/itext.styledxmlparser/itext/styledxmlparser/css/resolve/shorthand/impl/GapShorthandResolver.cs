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
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// Shorthand resolver for gap shorthand properties, can be used for
    /// different gap properties like
    /// <c>gap</c>
    /// or
    /// <c>grid-gap</c>.
    /// </summary>
    public class GapShorthandResolver : IShorthandResolver {
        private readonly String gapShorthandProperty;

        /// <summary>
        /// Instantiates default
        /// <see cref="GapShorthandResolver"/>
        /// for
        /// <c>gap</c>
        /// shorthand.
        /// </summary>
        public GapShorthandResolver()
            : this(CommonCssConstants.GAP) {
        }

        /// <summary>
        /// Instantiates default
        /// <see cref="GapShorthandResolver"/>
        /// for passed gap shorthand.
        /// </summary>
        /// <param name="gapShorthandProperty">the name of the gap shorthand property</param>
        public GapShorthandResolver(String gapShorthandProperty) {
            this.gapShorthandProperty = gapShorthandProperty;
        }

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.GapShorthandResolver
            ));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.ROW_GAP, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.COLUMN_GAP, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , gapShorthandProperty, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , gapShorthandProperty, shorthandExpression);
            }
            String[] gapProps = iText.Commons.Utils.StringUtil.Split(shorthandExpression, " ");
            if (gapProps.Length == 1) {
                return ResolveGapWithTwoProperties(gapProps[0], gapProps[0]);
            }
            else {
                if (gapProps.Length == 2) {
                    return ResolveGapWithTwoProperties(gapProps[0], gapProps[1]);
                }
                else {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , gapShorthandProperty, shorthandExpression);
                }
            }
        }

        private IList<CssDeclaration> ResolveGapWithTwoProperties(String row, String column) {
            CssDeclaration rowGapDeclaration = new CssDeclaration(CommonCssConstants.ROW_GAP, row);
            if (!CssDeclarationValidationMaster.CheckDeclaration(rowGapDeclaration)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.ROW_GAP, row);
            }
            CssDeclaration columnGapDeclaration = new CssDeclaration(CommonCssConstants.COLUMN_GAP, column);
            if (!CssDeclarationValidationMaster.CheckDeclaration(columnGapDeclaration)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.COLUMN_GAP, column);
            }
            return JavaUtil.ArraysAsList(rowGapDeclaration, columnGapDeclaration);
        }

        private static IList<CssDeclaration> HandleExpressionError(String logMessage, String attribute, String shorthandExpression
            ) {
            LOGGER.LogWarning(MessageFormatUtil.Format(logMessage, attribute, shorthandExpression));
            return JavaCollectionsUtil.EmptyList<CssDeclaration>();
        }
    }
}
