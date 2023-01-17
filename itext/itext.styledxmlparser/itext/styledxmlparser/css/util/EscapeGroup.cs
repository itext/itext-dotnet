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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Class to store escape characters and their processing logic.</summary>
    /// <remarks>
    /// Class to store escape characters and their processing logic.
    /// This class is used in
    /// <see cref="CssUtils.SplitString(System.String, char, EscapeGroup[])"/>
    /// method.
    /// </remarks>
    public class EscapeGroup {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Util.EscapeGroup
            ));

        private readonly char openCharacter;

        private readonly char closeCharacter;

        private int counter = 0;

        /// <summary>
        /// Creates instance of
        /// <see cref="EscapeGroup"/>.
        /// </summary>
        /// <param name="openCharacter">opening escape character</param>
        /// <param name="closeCharacter">closing escape character</param>
        public EscapeGroup(char openCharacter, char closeCharacter) {
            this.openCharacter = openCharacter;
            this.closeCharacter = closeCharacter;
        }

        /// <summary>
        /// Creates instance of
        /// <see cref="EscapeGroup"/>
        /// when opening and closing characters are the same.
        /// </summary>
        /// <param name="escapeChar">opening and closing escape character</param>
        public EscapeGroup(char escapeChar) {
            this.openCharacter = escapeChar;
            this.closeCharacter = escapeChar;
        }

        /// <summary>
        /// Is currently processed character in
        /// <see cref="CssUtils.SplitString(System.String, char, EscapeGroup[])"/>
        /// escaped.
        /// </summary>
        /// <returns>true if escaped, false otherwise</returns>
        internal virtual bool IsEscaped() {
            return counter != 0;
        }

        /// <summary>Processes given character.</summary>
        /// <param name="nextCharacter">next character to process</param>
        internal virtual void ProcessCharacter(char nextCharacter) {
            if (openCharacter == closeCharacter) {
                if (nextCharacter == openCharacter) {
                    if (IsEscaped()) {
                        ++counter;
                    }
                    else {
                        --counter;
                    }
                }
            }
            else {
                if (nextCharacter == openCharacter) {
                    ++counter;
                }
                else {
                    if (nextCharacter == closeCharacter) {
                        --counter;
                        if (counter < 0) {
                            LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INCORRECT_CHARACTER_SEQUENCE
                                ));
                            counter = 0;
                        }
                    }
                }
            }
        }
    }
}
