/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// Abstract
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for corners definitions.
    /// </summary>
    public abstract class AbstractCornersShorthandResolver : IShorthandResolver {
        /// <summary>The template for -bottom-left properties.</summary>
        private const String _0_BOTTOM_LEFT_1 = "{0}-bottom-left{1}";

        /// <summary>The template for -bottom-right properties.</summary>
        private const String _0_BOTTOM_RIGHT_1 = "{0}-bottom-right{1}";

        /// <summary>The template for -top-left properties.</summary>
        private const String _0_TOP_LEFT_1 = "{0}-top-left{1}";

        /// <summary>The template for -top-right properties.</summary>
        private const String _0_TOP_RIGHT_1 = "{0}-top-right{1}";

        /// <summary>Gets the prefix of a property.</summary>
        /// <returns>the prefix</returns>
        protected internal abstract String GetPrefix();

        /// <summary>Gets the postfix of a property.</summary>
        /// <returns>the postfix</returns>
        protected internal abstract String GetPostfix();

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            String[] props = iText.Commons.Utils.StringUtil.Split(shorthandExpression, "\\/");
            String[][] properties = new String[props.Length][];
            for (int i = 0; i < props.Length; i++) {
                properties[i] = iText.Commons.Utils.StringUtil.Split(props[i].Trim(), "\\s+");
            }
            String[] resultExpressions = new String[4];
            for (int i = 0; i < resultExpressions.Length; i++) {
                resultExpressions[i] = "";
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            String topLeftProperty = MessageFormatUtil.Format(_0_TOP_LEFT_1, GetPrefix(), GetPostfix());
            String topRightProperty = MessageFormatUtil.Format(_0_TOP_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomRightProperty = MessageFormatUtil.Format(_0_BOTTOM_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomLeftProperty = MessageFormatUtil.Format(_0_BOTTOM_LEFT_1, GetPrefix(), GetPostfix());
            for (int i = 0; i < properties.Length; i++) {
                if (properties[i].Length == 1) {
                    resultExpressions[0] += properties[i][0] + " ";
                    resultExpressions[1] += properties[i][0] + " ";
                    resultExpressions[2] += properties[i][0] + " ";
                    resultExpressions[3] += properties[i][0] + " ";
                }
                else {
                    if (properties[i].Length == 2) {
                        resultExpressions[0] += properties[i][0] + " ";
                        resultExpressions[1] += properties[i][1] + " ";
                        resultExpressions[2] += properties[i][0] + " ";
                        resultExpressions[3] += properties[i][1] + " ";
                    }
                    else {
                        if (properties[i].Length == 3) {
                            resultExpressions[0] += properties[i][0] + " ";
                            resultExpressions[1] += properties[i][1] + " ";
                            resultExpressions[2] += properties[i][2] + " ";
                            resultExpressions[3] += properties[i][1] + " ";
                        }
                        else {
                            if (properties[i].Length == 4) {
                                resultExpressions[0] += properties[i][0] + " ";
                                resultExpressions[1] += properties[i][1] + " ";
                                resultExpressions[2] += properties[i][2] + " ";
                                resultExpressions[3] += properties[i][3] + " ";
                            }
                        }
                    }
                }
            }
            resolvedDecl.Add(new CssDeclaration(topLeftProperty, resultExpressions[0]));
            resolvedDecl.Add(new CssDeclaration(topRightProperty, resultExpressions[1]));
            resolvedDecl.Add(new CssDeclaration(bottomRightProperty, resultExpressions[2]));
            resolvedDecl.Add(new CssDeclaration(bottomLeftProperty, resultExpressions[3]));
            return resolvedDecl;
        }
    }
}
