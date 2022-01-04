/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Text;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Tokenizer for CSS declaration values.</summary>
    public class CssDeclarationValueTokenizer {
        /// <summary>The source string.</summary>
        private String src;

        /// <summary>The current index.</summary>
        private int index = -1;

        /// <summary>The quote string, either "'" or "\"".</summary>
        private char stringQuote;

        /// <summary>Indicates if we're inside a string.</summary>
        private bool inString;

        /// <summary>The depth.</summary>
        private int functionDepth = 0;

        /// <summary>
        /// Creates a new
        /// <see cref="CssDeclarationValueTokenizer"/>
        /// instance.
        /// </summary>
        /// <param name="propertyValue">the property value</param>
        public CssDeclarationValueTokenizer(String propertyValue) {
            this.src = propertyValue;
        }

        /// <summary>Gets the next valid token.</summary>
        /// <returns>the next valid token</returns>
        public virtual CssDeclarationValueTokenizer.Token GetNextValidToken() {
            CssDeclarationValueTokenizer.Token token = GetNextToken();
            while (token != null && !token.IsString() && String.IsNullOrEmpty(token.GetValue().Trim())) {
                token = GetNextToken();
            }
            if (token != null && functionDepth > 0) {
                StringBuilder functionBuffer = new StringBuilder();
                while (token != null && functionDepth > 0) {
                    ProcessFunctionToken(token, functionBuffer);
                    token = GetNextToken();
                }
                functionDepth = 0;
                if (functionBuffer.Length != 0) {
                    if (token != null) {
                        ProcessFunctionToken(token, functionBuffer);
                    }
                    return new CssDeclarationValueTokenizer.Token(functionBuffer.ToString(), CssDeclarationValueTokenizer.TokenType
                        .FUNCTION);
                }
            }
            return token;
        }

        /// <summary>Gets the next token.</summary>
        /// <returns>the next token</returns>
        private CssDeclarationValueTokenizer.Token GetNextToken() {
            StringBuilder buff = new StringBuilder();
            char curChar;
            if (index >= src.Length - 1) {
                return null;
            }
            if (inString) {
                bool isEscaped = false;
                StringBuilder pendingUnicodeSequence = new StringBuilder();
                while (++index < src.Length) {
                    curChar = src[index];
                    if (isEscaped) {
                        if (IsHexDigit(curChar) && pendingUnicodeSequence.Length < 6) {
                            pendingUnicodeSequence.Append(curChar);
                        }
                        else {
                            if (pendingUnicodeSequence.Length != 0) {
                                int codePoint = Convert.ToInt32(pendingUnicodeSequence.ToString(), 16);
                                if (JavaUtil.IsValidCodePoint(codePoint)) {
                                    buff.AppendCodePoint(codePoint);
                                }
                                else {
                                    buff.Append("\uFFFD");
                                }
                                pendingUnicodeSequence.Length = 0;
                                if (curChar == stringQuote) {
                                    inString = false;
                                    return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.STRING
                                        );
                                }
                                else {
                                    if (!iText.IO.Util.TextUtil.IsWhiteSpace(curChar)) {
                                        buff.Append(curChar);
                                    }
                                }
                                isEscaped = false;
                            }
                            else {
                                buff.Append(curChar);
                                isEscaped = false;
                            }
                        }
                    }
                    else {
                        if (curChar == stringQuote) {
                            inString = false;
                            return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.STRING
                                );
                        }
                        else {
                            if (curChar == '\\') {
                                isEscaped = true;
                            }
                            else {
                                buff.Append(curChar);
                            }
                        }
                    }
                }
            }
            else {
                while (++index < src.Length) {
                    curChar = src[index];
                    if (curChar == '(') {
                        ++functionDepth;
                        buff.Append(curChar);
                    }
                    else {
                        if (curChar == ')') {
                            --functionDepth;
                            buff.Append(curChar);
                            if (functionDepth == 0) {
                                return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.FUNCTION
                                    );
                            }
                        }
                        else {
                            if (curChar == '"' || curChar == '\'') {
                                stringQuote = curChar;
                                inString = true;
                                return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.FUNCTION
                                    );
                            }
                            else {
                                if (curChar == ',' && !inString && functionDepth == 0) {
                                    if (buff.Length == 0) {
                                        return new CssDeclarationValueTokenizer.Token(",", CssDeclarationValueTokenizer.TokenType.COMMA);
                                    }
                                    else {
                                        --index;
                                        return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.UNKNOWN
                                            );
                                    }
                                }
                                else {
                                    if (iText.IO.Util.TextUtil.IsWhiteSpace(curChar)) {
                                        if (functionDepth > 0) {
                                            buff.Append(curChar);
                                        }
                                        return new CssDeclarationValueTokenizer.Token(buff.ToString(), functionDepth > 0 ? CssDeclarationValueTokenizer.TokenType
                                            .FUNCTION : CssDeclarationValueTokenizer.TokenType.UNKNOWN);
                                    }
                                    else {
                                        buff.Append(curChar);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new CssDeclarationValueTokenizer.Token(buff.ToString(), CssDeclarationValueTokenizer.TokenType.FUNCTION
                );
        }

        /// <summary>Checks if a character is a hexadecimal digit.</summary>
        /// <param name="c">the character</param>
        /// <returns>true, if it's a hexadecimal digit</returns>
        private bool IsHexDigit(char c) {
            return (47 < c && c < 58) || (64 < c && c < 71) || (96 < c && c < 103);
        }

        /// <summary>Processes a function token.</summary>
        /// <param name="token">the token</param>
        /// <param name="functionBuffer">the function buffer</param>
        private void ProcessFunctionToken(CssDeclarationValueTokenizer.Token token, StringBuilder functionBuffer) {
            if (token.IsString()) {
                functionBuffer.Append(stringQuote);
                functionBuffer.Append(token.GetValue());
                functionBuffer.Append(stringQuote);
            }
            else {
                functionBuffer.Append(token.GetValue());
            }
        }

        /// <summary>The Token class.</summary>
        public class Token {
            /// <summary>The value.</summary>
            private String value;

            /// <summary>The type.</summary>
            private CssDeclarationValueTokenizer.TokenType type;

            /// <summary>
            /// Creates a new
            /// <see cref="Token"/>
            /// instance.
            /// </summary>
            /// <param name="value">the value</param>
            /// <param name="type">the type</param>
            public Token(String value, CssDeclarationValueTokenizer.TokenType type) {
                this.value = value;
                this.type = type;
            }

            /// <summary>Gets the value.</summary>
            /// <returns>the value</returns>
            public virtual String GetValue() {
                return value;
            }

            /// <summary>Gets the type.</summary>
            /// <returns>the type</returns>
            public virtual CssDeclarationValueTokenizer.TokenType GetType() {
                return type;
            }

            /// <summary>Checks if the token is a string.</summary>
            /// <returns>true, if is string</returns>
            public virtual bool IsString() {
                return type == CssDeclarationValueTokenizer.TokenType.STRING;
            }

            /* (non-Javadoc)
            * @see java.lang.Object#toString()
            */
            public override String ToString() {
                return value;
            }
        }

        /// <summary>Enumeration of the different token types.</summary>
        public enum TokenType {
            /// <summary>The string type.</summary>
            STRING,
            /// <summary>The function type.</summary>
            FUNCTION,
            /// <summary>The comma type.</summary>
            COMMA,
            /// <summary>Unknown type.</summary>
            UNKNOWN
        }
    }
}
