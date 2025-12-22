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
using iText.StyledXmlParser.Util;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Tokenizer for searching var expressions in css declarations.</summary>
    public class CssDeclarationVarParser : CssDeclarationValueTokenizer {
        /// <summary>
        /// Creates a new
        /// <see cref="CssDeclarationVarParser"/>
        /// instance.
        /// </summary>
        /// <param name="propertyValue">the property value</param>
        public CssDeclarationVarParser(String propertyValue)
            : base(propertyValue) {
        }

        /// <summary>Gets the first valid var expression token.</summary>
        /// <remarks>
        /// Gets the first valid var expression token. This method can't be called in chain to find all
        /// var expressions in declaration since it invalidates internal parser state.
        /// </remarks>
        /// <returns>the first valid var expression token</returns>
        public virtual CssDeclarationVarParser.VarToken GetFirstValidVarToken() {
            // This class and method is needed for only one purpose - parse var() expressions inside other functions
            // Since original tokenizer is not suited for 'sensible' token parsing and only parses top level properties,
            // in this method we perform some hacks to be able to extract var expression inside top level tokens (functions).
            // E.g. extract `var(--value)` expression from `calc(var(--value) + 50px)`
            CssDeclarationValueTokenizer.Token token;
            int start = -1;
            do {
                token = GetNextToken();
                if (token != null && CssDeclarationValueTokenizer.TokenType.FUNCTION == token.GetType() && CssVariableUtil
                    .ContainsVarExpression(token.GetValue())) {
                    // example of expected tokens: 'var(--one)', 'var(--one, ', 'calc(var(--one', ...
                    start = index - (token.GetValue().Length - token.GetValue().IndexOf("var", StringComparison.Ordinal)) + 1;
                    break;
                }
            }
            while (token != null);
            if (token == null) {
                return null;
            }
            String tokenValue = token.GetValue();
            // handle the following tokens: var(--one), calc(var(--one)), ...
            if (IsEndingWithBracket(tokenValue)) {
                String resultTokenValue = ExtractSingleVar(tokenValue.Substring(tokenValue.IndexOf("var", StringComparison.Ordinal
                    )));
                return new CssDeclarationVarParser.VarToken(resultTokenValue, start, index + 1);
            }
            // handle the following tokens: 'calc(var(--one', 'calc(20px + var(--one)', ...
            CssDeclarationValueTokenizer.Token func = ParseFunctionToken(token, functionDepth - 1);
            // func == null condition is not expected and shouldn't be invoked since all cases which can produce null func
            // are handled above
            String resultTokenValue_1 = ExtractSingleVar(func.GetValue().Substring(func.GetValue().IndexOf("var", StringComparison.Ordinal
                )));
            return new CssDeclarationVarParser.VarToken(resultTokenValue_1, start, index + 1);
        }

        /// <summary>Cut symbols not related to first variable.</summary>
        /// <param name="expression">expression to process</param>
        /// <returns>expression with single variable</returns>
        private String ExtractSingleVar(String expression) {
            //Starting from index 3 as we expect string to start like "var(..."
            int currentIndex = 3;
            int depth = 0;
            do {
                char ch = expression[currentIndex];
                if (ch == '(') {
                    depth++;
                }
                else {
                    if (ch == ')') {
                        depth--;
                    }
                }
                currentIndex++;
            }
            while (currentIndex <= expression.Length - 1 && depth != 0);
            String result = expression.JSubstring(0, currentIndex);
            this.index -= expression.Length - result.Length;
            return result;
        }

        private static bool IsEndingWithBracket(String expression) {
            for (int i = expression.Length - 1; i >= 0; --i) {
                if (!IsSpaceOrWhitespace(expression[i])) {
                    return ')' == expression[i];
                }
            }
            return false;
        }

        private static bool IsSpaceOrWhitespace(char character) {
            return char.IsSeparator(character) || iText.IO.Util.TextUtil.IsWhiteSpace(character);
        }

        /// <summary>The Token class which contains CSS var expression.</summary>
        public class VarToken {
            private readonly int start;

            private readonly int end;

            private readonly String value;

//\cond DO_NOT_DOCUMENT
            internal VarToken(String value, int start, int end) {
                this.value = value;
                this.start = start;
                this.end = end;
            }
//\endcond

            /// <summary>Gets the var expression value.</summary>
            /// <returns>the value</returns>
            public virtual String GetValue() {
                return value;
            }

            /// <summary>Gets start position of var expression in original css declaration</summary>
            /// <returns>start position in original css declaration</returns>
            public virtual int GetStart() {
                return start;
            }

            /// <summary>Gets end position of var expression in original css declaration</summary>
            /// <returns>end position in original css declaration</returns>
            public virtual int GetEnd() {
                return end;
            }

            /* (non-Javadoc)
            * @see java.lang.Object#toString()
            */
            public override String ToString() {
                return value;
            }
        }
    }
}
