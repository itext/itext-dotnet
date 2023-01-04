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
