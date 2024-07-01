using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for grid shorthand.
    /// </summary>
    public class GridShorthandResolver : IShorthandResolver {
        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (!shorthandExpression.Contains(CommonCssConstants.AUTO_FLOW)) {
                return new GridTemplateShorthandResolver().ResolveShorthand(shorthandExpression);
            }
            String[] values = iText.Commons.Utils.StringUtil.Split(shorthandExpression.Trim(), "/");
            IList<CssDeclaration> result = new List<CssDeclaration>();
            if (values[0].Contains(CommonCssConstants.AUTO_FLOW)) {
                if (values[0].Contains(CommonCssConstants.DENSE)) {
                    result.Add(new CssDeclaration(CommonCssConstants.GRID_AUTO_FLOW, CommonCssConstants.DENSE));
                }
                String rowsTemplate = values[0].Substring(Math.Max(values[0].IndexOf(CommonCssConstants.AUTO_FLOW, StringComparison.Ordinal
                    ) + CommonCssConstants.AUTO_FLOW.Length, values[0].IndexOf(CommonCssConstants.DENSE, StringComparison.Ordinal
                    ) + CommonCssConstants.DENSE.Length));
                if (!String.IsNullOrEmpty(rowsTemplate.Trim())) {
                    result.Add(new CssDeclaration(CommonCssConstants.GRID_AUTO_ROWS, rowsTemplate));
                }
                if (values.Length == 2) {
                    result.Add(new CssDeclaration(CommonCssConstants.GRID_TEMPLATE_COLUMNS, values[1]));
                }
            }
            else {
                if (values.Length == 2) {
                    result.Add(new CssDeclaration(CommonCssConstants.GRID_TEMPLATE_ROWS, values[0]));
                    if (values[1].Contains(CommonCssConstants.DENSE)) {
                        result.Add(new CssDeclaration(CommonCssConstants.GRID_AUTO_FLOW, CommonCssConstants.COLUMN + " " + CommonCssConstants
                            .DENSE));
                    }
                    String columnsTemplate = values[1].Substring(Math.Max(values[1].IndexOf(CommonCssConstants.AUTO_FLOW, StringComparison.Ordinal
                        ) + CommonCssConstants.AUTO_FLOW.Length, values[1].IndexOf(CommonCssConstants.DENSE, StringComparison.Ordinal
                        ) + CommonCssConstants.DENSE.Length));
                    if (!String.IsNullOrEmpty(columnsTemplate.Trim())) {
                        result.Add(new CssDeclaration(CommonCssConstants.GRID_AUTO_COLUMNS, columnsTemplate));
                    }
                }
            }
            return result;
        }
    }
}
