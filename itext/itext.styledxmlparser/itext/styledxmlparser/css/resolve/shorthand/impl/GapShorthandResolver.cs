using System;
using System.Collections.Generic;
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class GapShorthandResolver : IShorthandResolver {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(GapShorthandResolver));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.ROW_GAP, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.COLUMN_GAP, shorthandExpression));
            }
            if (CssTypesValidationUtils.ContainsInitialOrInheritOrUnset(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .GAP, shorthandExpression));
                return JavaCollectionsUtil.EmptyList();
            }
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.GAP));
                return JavaCollectionsUtil.EmptyList();
            }
            String[] gapProps = iText.IO.Util.StringUtil.Split(shorthandExpression, " ");
            if (gapProps.Length == 1) {
                return ResolveGapWithOneProperty(gapProps[0]);
            }
            if (gapProps.Length == 2) {
                return ResolveGapWithTwoProperties(gapProps[0], gapProps[1]);
            }
            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                .GAP, shorthandExpression));
            return JavaCollectionsUtil.EmptyList();
        }

        private IList<CssDeclaration> ResolveGapWithOneProperty(String rowAndColumn) {
            CssDeclaration rowGapDeclaration = new CssDeclaration(CommonCssConstants.ROW_GAP, rowAndColumn);
            if (CssDeclarationValidationMaster.CheckDeclaration(rowGapDeclaration)) {
                CssDeclaration columnGapDeclaration = new CssDeclaration(CommonCssConstants.COLUMN_GAP, rowAndColumn);
                return JavaUtil.ArraysAsList(rowGapDeclaration, columnGapDeclaration);
            }
            LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                .ROW_GAP, rowAndColumn));
            return JavaCollectionsUtil.EmptyList();
        }

        private IList<CssDeclaration> ResolveGapWithTwoProperties(String row, String column) {
            CssDeclaration rowGapDeclaration = new CssDeclaration(CommonCssConstants.ROW_GAP, row);
            if (!CssDeclarationValidationMaster.CheckDeclaration(rowGapDeclaration)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .ROW_GAP, row));
                return JavaCollectionsUtil.EmptyList();
            }
            CssDeclaration columnGapDeclaration = new CssDeclaration(CommonCssConstants.COLUMN_GAP, column);
            if (!CssDeclarationValidationMaster.CheckDeclaration(columnGapDeclaration)) {
                LOGGER.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.UNKNOWN_PROPERTY, CommonCssConstants
                    .COLUMN_GAP, column));
                return JavaCollectionsUtil.EmptyList();
            }
            return JavaUtil.ArraysAsList(rowGapDeclaration, columnGapDeclaration);
        }
    }
}
