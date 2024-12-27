using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class MarkerShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.MarkerShorthandResolver
            ));

        /// <summary>
        /// Creates a new
        /// <see cref="MarkerShorthandResolver"/>
        /// instance.
        /// </summary>
        public MarkerShorthandResolver() {
        }

        //empty constructor
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.MARKER_START, shorthandExpression), new 
                    CssDeclaration(CommonCssConstants.MARKER_MID, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .MARKER_END, shorthandExpression));
            }
            String expression = shorthandExpression.Trim();
            if (String.IsNullOrEmpty(expression)) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.MARKER));
                return new List<CssDeclaration>();
            }
            if (!expression.StartsWith(CommonCssConstants.URL + "(") || !expression.EndsWith(")")) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , shorthandExpression));
                return new List<CssDeclaration>();
            }
            return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.MARKER_START, shorthandExpression), new 
                CssDeclaration(CommonCssConstants.MARKER_MID, shorthandExpression), new CssDeclaration(CommonCssConstants
                .MARKER_END, shorthandExpression));
        }
    }
}
