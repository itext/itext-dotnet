/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class TextDecorationShorthandResolver : IShorthandResolver {
        private static readonly ICollection<String> TEXT_DECORATION_LINE_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.UNDERLINE, CommonCssConstants.OVERLINE, 
            CommonCssConstants.LINE_THROUGH, CommonCssConstants.BLINK)));

        private static readonly ICollection<String> TEXT_DECORATION_STYLE_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.SOLID, CommonCssConstants.DOUBLE, CommonCssConstants
            .DOTTED, CommonCssConstants.DASHED, CommonCssConstants.WAVY)));

        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_LINE, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.TEXT_DECORATION_STYLE, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.TEXT_DECORATION_COLOR, shorthandExpression));
            }
            //regexp for separating line by spaces that are not inside the parentheses, so rgb()
            // and hsl() color declarations are parsed correctly
            String[] props = iText.Commons.Utils.StringUtil.Split(shorthandExpression, "\\s+(?![^\\(]*\\))");
            IList<String> textDecorationLineValues = new List<String>();
            String textDecorationStyleValue = null;
            String textDecorationColorValue = null;
            foreach (String value in props) {
                //For text-decoration-line attributes several attributes may be present at once.
                //However when "none" attribute is present, all the other attributes become invalid
                if (TEXT_DECORATION_LINE_VALUES.Contains(value) || CommonCssConstants.NONE.Equals(value)) {
                    textDecorationLineValues.Add(value);
                }
                else {
                    if (TEXT_DECORATION_STYLE_VALUES.Contains(value)) {
                        textDecorationStyleValue = value;
                    }
                    else {
                        if (!String.IsNullOrEmpty(value)) {
                            textDecorationColorValue = value;
                        }
                    }
                }
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            if (textDecorationLineValues.IsEmpty()) {
                resolvedDecl.Add(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_LINE, CommonCssConstants.INITIAL));
            }
            else {
                StringBuilder resultLine = new StringBuilder();
                foreach (String line in textDecorationLineValues) {
                    resultLine.Append(line).Append(" ");
                }
                resolvedDecl.Add(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_LINE, resultLine.ToString().Trim())
                    );
            }
            resolvedDecl.Add(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_STYLE, textDecorationStyleValue == 
                null ? CommonCssConstants.INITIAL : textDecorationStyleValue));
            resolvedDecl.Add(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_COLOR, textDecorationColorValue == 
                null ? CommonCssConstants.INITIAL : textDecorationColorValue));
            return resolvedDecl;
        }
    }
}
