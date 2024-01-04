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

namespace iText.Svg.Utils {
    /// <summary>Utility class that facilitates parsing values from CSS.</summary>
    public sealed class SvgCssUtils {
        // TODO DEVSIX-2266
        private SvgCssUtils() {
        }

        /// <summary>Splits a given String into a list of substrings.</summary>
        /// <remarks>
        /// Splits a given String into a list of substrings.
        /// The string is split up by commas and whitespace characters (\t, \n, \r, \f).
        /// </remarks>
        /// <param name="value">the string to be split</param>
        /// <returns>a list containing the split strings, an empty list if the value is null or empty</returns>
        public static IList<String> SplitValueList(String value) {
            IList<String> result = new List<String>();
            if (value != null && value.Length > 0) {
                value = value.Trim();
                String[] list = iText.Commons.Utils.StringUtil.Split(value, "[,|\\s]");
                foreach (String element in list) {
                    if (!String.IsNullOrEmpty(element)) {
                        result.Add(element);
                    }
                }
            }
            return result;
        }

        /// <summary>Converts a float to a String.</summary>
        /// <param name="value">to be converted float value</param>
        /// <returns>the value in a String representation</returns>
        public static String ConvertFloatToString(float value) {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Converts a double to a String.</summary>
        /// <param name="value">to be converted double value</param>
        /// <returns>the value in a String representation</returns>
        public static String ConvertDoubleToString(double value) {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
