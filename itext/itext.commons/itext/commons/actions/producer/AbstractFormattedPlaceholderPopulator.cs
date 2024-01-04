/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Exceptions;

namespace iText.Commons.Actions.Producer {
    /// <summary>Abstract populator for placeholders consuming a parameter which is a pattern string.</summary>
    /// <remarks>
    /// Abstract populator for placeholders consuming a parameter which is a pattern string. Any latin
    /// letter inside the pattern which is not quoted considered as a param defining the component of the
    /// outputted value.
    /// </remarks>
    internal abstract class AbstractFormattedPlaceholderPopulator : IPlaceholderPopulator {
        /// <summary>Escaping character.</summary>
        protected internal const char APOSTROPHE = '\'';

        private const char ESCAPE_CHARACTER = '\\';

        private const char A_UPPERCASE = 'A';

        private const char Z_UPPERCASE = 'Z';

        private const char A_LOWERCASE = 'a';

        private const char Z_LOWERCASE = 'z';

        /// <summary>Processes quoted string inside format array.</summary>
        /// <remarks>
        /// Processes quoted string inside format array. It is expected that provided index points to the
        /// apostrophe character so that since the <c>index + 1</c> position quoted string starts.
        /// <para />
        /// String may contain escaped apostrophes <c>\'</c> which processed as characters.
        /// Backslash is used for escaping so you need double backslash to print it <c>\\</c>. All
        /// the rest backslashes (not followed by apostrophe or one more backslash) are simply ignored.
        /// </remarks>
        /// <param name="index">is a index of apostrophe starting a new quoted string</param>
        /// <param name="builder">
        /// is a
        /// <see cref="System.Text.StringBuilder"/>
        /// building a resulting formatted string. It is
        /// updated by the method: quoted string is attached
        /// </param>
        /// <param name="formatArray">is a format representation</param>
        /// <returns>index of the character after the closing apostrophe</returns>
        protected internal virtual int AttachQuotedString(int index, StringBuilder builder, char[] formatArray) {
            bool isEscaped = false;
            index++;
            while (index < formatArray.Length && (formatArray[index] != APOSTROPHE || isEscaped)) {
                isEscaped = formatArray[index] == ESCAPE_CHARACTER && !isEscaped;
                if (!isEscaped) {
                    builder.Append(formatArray[index]);
                }
                index++;
            }
            if (index == formatArray.Length) {
                throw new ArgumentException(CommonsExceptionMessageConstant.PATTERN_CONTAINS_OPEN_QUOTATION);
            }
            return index;
        }

        /// <summary>Checks if provided character is a latin letter.</summary>
        /// <param name="ch">is character to check</param>
        /// <returns><c>true</c> if character is a latin letter and <c>false</c> otherwise</returns>
        protected internal bool IsLetter(char ch) {
            return (A_LOWERCASE <= ch && Z_LOWERCASE >= ch) || (A_UPPERCASE <= ch && Z_UPPERCASE >= ch);
        }

        public abstract String Populate(IList<ConfirmedEventWrapper> arg1, String arg2);
    }
}
