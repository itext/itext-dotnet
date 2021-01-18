using System;
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class PlaceItemsShorthandResolver : IShorthandResolver {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(PlaceItemsShorthandResolver));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.ALIGN_ITEMS, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.JUSTIFY_ITEMS, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .PLACE_ITEMS, shorthandExpression));
                return JavaCollectionsUtil.EmptyList();
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.PLACE_ITEMS));
                return JavaCollectionsUtil.EmptyList();
            }
            String[] placeItemsProps = iText.IO.Util.StringUtil.Split(shorthandExpression, " ");
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
                    LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                        .PLACE_ITEMS, shorthandExpression));
                    return JavaCollectionsUtil.EmptyList();
                }
            }
        }

        private IList<CssDeclaration> ResolveShorthandWithOneWord(String firstWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, firstWord);
            if (resolvedShorthand == null) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .ALIGN_ITEMS, firstWord));
                return JavaCollectionsUtil.EmptyList();
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithTwoWords(String firstWord, String secondWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, secondWord);
            if (resolvedShorthand != null) {
                return resolvedShorthand;
            }
            resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, firstWord + " " + secondWord
                );
            if (resolvedShorthand == null) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .ALIGN_ITEMS, firstWord + " " + secondWord));
                return JavaCollectionsUtil.EmptyList();
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithThreeWords(String firstWord, String secondWord, String thirdWord
            ) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord, secondWord + " " + thirdWord
                );
            if (resolvedShorthand != null) {
                return resolvedShorthand;
            }
            resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, thirdWord);
            if (resolvedShorthand == null) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .ALIGN_ITEMS, firstWord + " " + secondWord));
                return JavaCollectionsUtil.EmptyList();
            }
            return resolvedShorthand;
        }

        private IList<CssDeclaration> ResolveShorthandWithFourWords(String firstWord, String secondWord, String thirdWord
            , String fourthWord) {
            IList<CssDeclaration> resolvedShorthand = ResolveAlignItemsAndJustifyItems(firstWord + " " + secondWord, thirdWord
                 + " " + fourthWord);
            if (resolvedShorthand == null) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .ALIGN_ITEMS, firstWord + " " + secondWord));
                return JavaCollectionsUtil.EmptyList();
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
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .JUSTIFY_ITEMS, justifyItemsDeclaration.GetExpression()));
                return JavaCollectionsUtil.EmptyList();
            }
            return null;
        }
    }
}
