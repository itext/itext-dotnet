using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for grid-template shorthand.
    /// </summary>
    public class GridTemplateShorthandResolver : IShorthandResolver {
        /// <summary>Creates grid template shorthand resolver.</summary>
        public GridTemplateShorthandResolver() {
        }

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.GridTemplateShorthandResolver
            ));

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.GRID_TEMPLATE));
                return new List<CssDeclaration>();
            }
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression) || CommonCssConstants.AUTO.Equals
                (shorthandExpression) || CommonCssConstants.NONE.Equals(shorthandExpression)) {
                return new List<CssDeclaration>();
            }
            StringBuilder rowsTemplateBuilder = new StringBuilder();
            StringBuilder areasBuilder = new StringBuilder();
            String columnsTemplate = "";
            String[] values = iText.Commons.Utils.StringUtil.Split(shorthandExpression, "/");
            if (values.Length == 2) {
                columnsTemplate = values[1];
            }
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(values[0]);
            CssDeclarationValueTokenizer.Token token;
            bool templateRowsEncountered = false;
            bool previousTokenIsArea = false;
            for (int i = 0; ((token = tokenizer.GetNextValidToken()) != null); ++i) {
                if (token.IsString() && !token.GetValue().StartsWith("[")) {
                    if (previousTokenIsArea) {
                        rowsTemplateBuilder.Append(CommonCssConstants.AUTO).Append(" ");
                    }
                    areasBuilder.Append("'").Append(token.GetValue()).Append("'").Append(" ");
                    previousTokenIsArea = true;
                }
                else {
                    rowsTemplateBuilder.Append(token.GetValue()).Append(" ");
                    templateRowsEncountered = true;
                    previousTokenIsArea = false;
                }
            }
            if (previousTokenIsArea) {
                rowsTemplateBuilder.Append(CommonCssConstants.AUTO).Append(" ");
            }
            if (!templateRowsEncountered) {
                rowsTemplateBuilder.Length = 0;
            }
            String rowsTemplate = rowsTemplateBuilder.ToString();
            String areas = areasBuilder.ToString();
            IList<CssDeclaration> result = new List<CssDeclaration>(3);
            if (!String.IsNullOrEmpty(rowsTemplate)) {
                result.Add(new CssDeclaration(CommonCssConstants.GRID_TEMPLATE_ROWS, rowsTemplate));
            }
            if (!String.IsNullOrEmpty(columnsTemplate)) {
                result.Add(new CssDeclaration(CommonCssConstants.GRID_TEMPLATE_COLUMNS, columnsTemplate));
            }
            if (!String.IsNullOrEmpty(areas)) {
                result.Add(new CssDeclaration(CommonCssConstants.GRID_TEMPLATE_AREAS, areas));
            }
            return result;
        }
    }
}
