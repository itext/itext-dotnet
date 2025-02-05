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
