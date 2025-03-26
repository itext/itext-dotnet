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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Validate;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Util {
    /// <summary>Utility class for resolving css variables in declarations.</summary>
    public class CssVariableUtil {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Util.CssVariableUtil
            ));

        /// <summary>Max count of css var expressions in single declaration.</summary>
        private const int MAX_CSS_VAR_COUNT = 30;

        private CssVariableUtil() {
        }

        //private constructor for util class
        /// <summary>Resolve all css variables in style map</summary>
        /// <param name="styles">css styles map</param>
        public static void ResolveCssVariables(IDictionary<String, String> styles) {
            IList<CssDeclaration> varExpressions = new List<CssDeclaration>();
            foreach (KeyValuePair<String, String> entry in styles) {
                if (!ContainsVarExpression(entry.Value) || IsCssVariable(entry.Key)) {
                    continue;
                }
                CssDeclaration result = new CssDeclaration(entry.Key, null);
                try {
                    result = ResolveSingleVar(entry.Key, entry.Value, styles);
                }
                catch (StyledXMLParserException exception) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(exception.Message, new CssDeclaration(entry.Key, entry.Value)));
                }
                varExpressions.Add(result);
            }
            foreach (CssDeclaration expression in varExpressions) {
                styles.JRemove(expression.GetProperty());
                if (expression.GetExpression() != null) {
                    IList<CssDeclaration> resolvedShorthandProperties = ExpandShorthand(expression);
                    foreach (CssDeclaration resolved in resolvedShorthandProperties) {
                        styles.Put(resolved.GetProperty(), resolved.GetExpression());
                    }
                }
            }
        }

        /// <summary>Checks for var expression.</summary>
        /// <param name="expression">css expression to check</param>
        /// <returns>true if there is a var expression, false otherwise</returns>
        public static bool ContainsVarExpression(String expression) {
            return expression != null && expression.Contains("var(");
        }

        /// <summary>Checks property for css variable.</summary>
        /// <param name="property">css property to check</param>
        /// <returns>true if it is a css variable, false otherwise</returns>
        public static bool IsCssVariable(String property) {
            return property != null && property.StartsWith("--");
        }

        private static IList<CssDeclaration> ExpandShorthand(CssDeclaration declaration) {
            IList<CssDeclaration> result = new List<CssDeclaration>();
            IShorthandResolver shorthandResolver = ShorthandResolverFactory.GetShorthandResolver(declaration.GetProperty
                ());
            if (shorthandResolver == null) {
                result.Add(declaration);
                return result;
            }
            else {
                IList<CssDeclaration> resolvedShorthandProps = shorthandResolver.ResolveShorthand(declaration.GetExpression
                    ());
                foreach (CssDeclaration resolved in resolvedShorthandProps) {
                    result.AddAll(ExpandShorthand(resolved));
                }
            }
            return result;
        }

        /// <summary>Resolve single css var expression recursively</summary>
        /// <param name="key">css style property</param>
        /// <param name="expression">css expression</param>
        /// <param name="styles">css styles map</param>
        /// <returns>resolved var expression if present or null if none found</returns>
        private static CssDeclaration ResolveSingleVar(String key, String expression, IDictionary<String, String> 
            styles) {
            if (!ContainsVarExpression(expression)) {
                return new CssDeclaration(key, expression);
            }
            String result = ResolveVarRecursively(expression, styles, 0);
            CssDeclaration declaration = new CssDeclaration(key, result);
            if (CssDeclarationValidationMaster.CheckDeclaration(declaration)) {
                return declaration;
            }
            else {
                // Throw exception to be able to log the whole css declaration
                throw new StyledXMLParserException(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    );
            }
        }

        /// <summary>Resolves variables without taking into account default values</summary>
        /// <param name="expression">var value</param>
        /// <param name="styles">element styles</param>
        /// <param name="level">current var expression nesting level</param>
        /// <returns>resolved var expression</returns>
        private static String ResolveVarRecursively(String expression, IDictionary<String, String> styles, int level
            ) {
            if (level > MAX_CSS_VAR_COUNT) {
                throw new StyledXMLParserException(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_VARIABLE_COUNT
                    );
            }
            StringBuilder expandedExpressionBuilder = new StringBuilder();
            CssDeclarationVarParser tokenizer = new CssDeclarationVarParser(expression);
            CssDeclarationVarParser.VarToken currentToken = tokenizer.GetFirstValidVarToken();
            if (currentToken != null) {
                String resolvedVar = ResolveVarExpression(currentToken.GetValue(), styles);
                expandedExpressionBuilder.JAppend(expression, 0, currentToken.GetStart()).Append(resolvedVar).JAppend(expression
                    , currentToken.GetEnd(), expression.Length);
            }
            else {
                throw new StyledXMLParserException(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_DURING_CSS_VARIABLE_RESOLVING
                    );
            }
            String expandedExpression = expandedExpressionBuilder.ToString();
            if (ContainsVarExpression(expandedExpression)) {
                level++;
                expandedExpression = ResolveVarRecursively(expandedExpression, styles, level);
            }
            return expandedExpression;
        }

        /// <summary>
        /// Resolve css variable expression, if there is a fallback value and primary value is null,
        /// default value will be returned.
        /// </summary>
        /// <param name="varExpression">expression as the following: var(.+?(?:,.*?)?)</param>
        /// <param name="styles">map of styles containing resolved variables</param>
        /// <returns>resolved var expression</returns>
        private static String ResolveVarExpression(String varExpression, IDictionary<String, String> styles) {
            int variableStartIndex = varExpression.IndexOf("--", StringComparison.Ordinal);
            int separatorIndex = varExpression.IndexOf(',');
            int variableEndIndex = separatorIndex == -1 ? varExpression.IndexOf(')') : separatorIndex;
            String name = varExpression.JSubstring(variableStartIndex, variableEndIndex).Trim();
            String value = styles.Get(name);
            if (value != null) {
                return value;
            }
            else {
                if (separatorIndex != -1) {
                    return varExpression.JSubstring(separatorIndex + 1, varExpression.LastIndexOf(')'));
                }
            }
            return "";
        }
    }
}
