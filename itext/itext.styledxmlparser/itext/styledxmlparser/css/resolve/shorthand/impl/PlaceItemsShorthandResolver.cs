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
    public class PlaceItemsShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(PlaceItemsShorthandResolver));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.ALIGN_ITEMS, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.JUSTIFY_ITEMS, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.PLACE_ITEMS, shorthandExpression);
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.PLACE_ITEMS, shorthandExpression);
            }
            String[] placeItemsProps = iText.Commons.Utils.StringUtil.Split(shorthandExpression, " ");
            switch (placeItemsProps.Length) {
                case 1: {
                    return ResolveShorthandWithOneWord(placeItemsProps[0]);
                }

                case 2: {
                    return ResolveShorthandWithTwoWords(placeItemsProps[0], placeItemsProps[1]);
                }

                case 3: {
                    return ResolveShorthandWithThreeWords(placeItemsProps[0], placeItemsProps[1], placeItemsProps[2]);
                }

                case 4: {
                    return ResolveShorthandWithFourWords(placeItemsProps[0], placeItemsProps[1], placeItemsProps[2], placeItemsProps
                        [3]);
                }

                default: {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.PLACE_ITEMS, shorthandExpression);
                }
            }
        }

        private IList<CssDeclaration> ResolveShorthandWithOneWord(String firstWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, firstWord);
            if (resolvedShorthand.IsEmpty()) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.PLACE_ITEMS, firstWord);
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithTwoWords(String firstWord, String secondWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, secondWord);
            if (resolvedShorthand.IsEmpty()) {
                resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, firstWord + " " + secondWord
                    );
                if (resolvedShorthand.IsEmpty()) {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.PLACE_ITEMS, firstWord + " " + secondWord);
                }
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithThreeWords(String firstWord, String secondWord, String thirdWord
            ) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, secondWord + " " + thirdWord
                );
            if (resolvedShorthand.IsEmpty()) {
                resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, thirdWord);
                if (resolvedShorthand.IsEmpty()) {
                    return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , CommonCssConstants.PLACE_ITEMS, firstWord + " " + secondWord + " " + thirdWord);
                }
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithFourWords(String firstWord, String secondWord, String thirdWord
            , String fourthWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, thirdWord
                 + " " + fourthWord);
            if (resolvedShorthand.IsEmpty()) {
                return HandleExpressionError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , CommonCssConstants.PLACE_ITEMS, firstWord + " " + secondWord + " " + thirdWord + " " + fourthWord);
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveAlignItemsAndJustifyItems(String alignItems, String justifyItems) {
            CssDeclaration alignItemsDeclaration = new CssDeclaration(CommonCssConstants.ALIGN_ITEMS, alignItems);
            if (CssDeclarationValidationMaster.CheckDeclaration(alignItemsDeclaration)) {
                CssDeclaration justifyItemsDeclaration = new CssDeclaration(CommonCssConstants.JUSTIFY_ITEMS, justifyItems
                    );
                if (CssDeclarationValidationMaster.CheckDeclaration(justifyItemsDeclaration)) {
                    return JavaUtil.ArraysAsList(alignItemsDeclaration, justifyItemsDeclaration);
                }
                return JavaCollectionsUtil.EmptyList<CssDeclaration>();
            }
            else {
                return JavaCollectionsUtil.EmptyList<CssDeclaration>();
            }
        }

        private static IList<CssDeclaration> HandleExpressionError(String logMessage, String attribute, String shorthandExpression
            ) {
            LOGGER.LogWarning(MessageFormatUtil.Format(logMessage, attribute, shorthandExpression));
            return JavaCollectionsUtil.EmptyList<CssDeclaration>();
        }
    }
}
