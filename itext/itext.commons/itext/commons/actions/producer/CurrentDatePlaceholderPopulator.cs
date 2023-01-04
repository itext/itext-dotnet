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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Exceptions;
using iText.Commons.Utils;

namespace iText.Commons.Actions.Producer {
    /// <summary>Class is used to populate <c>currentDate</c> placeholder.</summary>
    /// <remarks>
    /// Class is used to populate <c>currentDate</c> placeholder. Placeholder should be configured
    /// with parameter defining the format of date output. Within format strings, unquoted letters from
    /// <c>A</c> to <c>Z</c> and from <c>a</c> to <c>z</c> are process as pattern
    /// letters. Chain of equal pattern letters forms an appropriate component of
    /// <c>currentDate</c> format. There following components are supported:
    /// <para />
    /// <list type="bullet">
    /// <item><description><c>d</c> is for the day of the month, from 1 through 31
    /// </description></item>
    /// <item><description><c>dd</c> is for the day of the month, from 01 through 31
    /// </description></item>
    /// <item><description><c>M</c> defines the month from 1 to 12
    /// </description></item>
    /// <item><description><c>MM</c> defines the month from 01 to 12
    /// </description></item>
    /// <item><description><c>MMM</c> defines the abbreviated name of the month
    /// </description></item>
    /// <item><description><c>MMMM</c> defines the full name of month
    /// </description></item>
    /// <item><description><c>yy</c> means the year from 00 to 99
    /// </description></item>
    /// <item><description><c>yyyy</c> means the year in for digits format
    /// </description></item>
    /// <item><description><c>s</c> shows current second, from 0 through 59
    /// </description></item>
    /// <item><description><c>ss</c> shows current second, from 00 through 59
    /// </description></item>
    /// <item><description><c>m</c> is replaced with the current minute from 0 to 59
    /// </description></item>
    /// <item><description><c>mm</c> is replaced with the current minute from 00 to 59
    /// </description></item>
    /// <item><description><c>H</c> stands for the current hour, using a 24-hour clock from 0 to 23
    /// </description></item>
    /// <item><description><c>HH</c> stands for the current hour, using a 24-hour clock from 00 to 23
    /// </description></item>
    /// </list>
    /// <para />
    /// Text can be quoted using single quotes (') to avoid interpretation. All other characters are not
    /// interpreted and just copied into the output string. String may contain escaped apostrophes
    /// <c>\'</c> which processed as characters. Backslash is used for escaping so you need double
    /// backslash to print it <c>\\</c>. All the rest backslashes (not followed by apostrophe or
    /// one more backslash) are simply ignored.
    /// <para />
    /// The result of the processing is current date representing in accordance with the provided format.
    /// </remarks>
    internal class CurrentDatePlaceholderPopulator : AbstractFormattedPlaceholderPopulator {
        private static readonly ICollection<String> ALLOWED_PATTERNS = new HashSet<String>(JavaUtil.ArraysAsList("dd"
            , "MM", "MMM", "MMMM", "yy", "yyyy", "ss", "mm", "HH"));

        public CurrentDatePlaceholderPopulator() {
        }

        // Empty constructor.
        /// <summary>
        /// Builds a replacement for a placeholder <c>currentDate</c> in accordance with the
        /// provided format.
        /// </summary>
        /// <param name="events">
        /// is a list of event involved into document processing. It is not used during
        /// the placeholder replacement
        /// </param>
        /// <param name="parameter">defines output format in accordance with the description</param>
        /// <returns>date of producer line creation in accordance with defined format</returns>
        public override String Populate(IList<ConfirmedEventWrapper> events, String parameter) {
            if (parameter == null) {
                throw new ArgumentException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.INVALID_USAGE_FORMAT_REQUIRED
                    , "currentDate"));
            }
            DateTime now = DateTimeUtil.GetCurrentUtcTime();
            return FormatDate(now, parameter);
        }

        private String FormatDate(DateTime date, String format) {
            StringBuilder builder = new StringBuilder();
            char[] formatArray = format.ToCharArray();
            for (int i = 0; i < formatArray.Length; i++) {
                if (formatArray[i] == APOSTROPHE) {
                    i = AttachQuotedString(i, builder, formatArray);
                }
                else {
                    if (IsLetter(formatArray[i])) {
                        i = ProcessDateComponent(i, date, builder, formatArray);
                    }
                    else {
                        builder.Append(formatArray[i]);
                    }
                }
            }
            return builder.ToString();
        }

        private int ProcessDateComponent(int index, DateTime date, StringBuilder builder, char[] formatArray) {
            StringBuilder peaceBuilder = new StringBuilder();
            char currentChar = formatArray[index];
            peaceBuilder.Append(currentChar);
            while (index + 1 < formatArray.Length && currentChar == formatArray[index + 1]) {
                index++;
                peaceBuilder.Append(formatArray[index]);
            }
            String piece = peaceBuilder.ToString();
            if (ALLOWED_PATTERNS.Contains(piece)) {
                builder.Append(DateTimeUtil.Format(date, piece));
            }
            else {
                throw new ArgumentException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.PATTERN_CONTAINS_UNEXPECTED_COMPONENT
                    , piece));
            }
            return index;
        }
    }
}
