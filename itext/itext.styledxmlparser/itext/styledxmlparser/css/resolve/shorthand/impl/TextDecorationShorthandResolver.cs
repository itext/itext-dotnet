using System;
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    public class TextDecorationShorthandResolver : IShorthandResolver {
        private static readonly ICollection<String> TEXT_DECORATION_LINE_VALUES = new HashSet<String>(JavaUtil.ArraysAsList
            (CommonCssConstants.UNDERLINE, CommonCssConstants.OVERLINE, CommonCssConstants.LINE_THROUGH, CommonCssConstants
            .BLINK));

        private static readonly ICollection<String> TEXT_DECORATION_STYLE_VALUES = new HashSet<String>(JavaUtil.ArraysAsList
            (CommonCssConstants.SOLID, CommonCssConstants.DOUBLE, CommonCssConstants.DOTTED, CommonCssConstants.DASHED
            , CommonCssConstants.WAVY));

        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.TEXT_DECORATION_LINE, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.TEXT_DECORATION_STYLE, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.TEXT_DECORATION_COLOR, shorthandExpression));
            }
            //regexp for separating line by spaces that are not inside the parentheses, so rgb()
            // and hsl() color declarations are parsed correctly
            String[] props = iText.IO.Util.StringUtil.Split(shorthandExpression, "\\s+(?![^\\(]*\\))");
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
