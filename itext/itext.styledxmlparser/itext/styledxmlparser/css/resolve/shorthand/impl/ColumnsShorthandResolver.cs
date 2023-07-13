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
