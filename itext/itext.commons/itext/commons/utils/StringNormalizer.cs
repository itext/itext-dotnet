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

namespace iText.Commons.Utils {
    /// <summary>Utility class for string normalization.</summary>
    public sealed class StringNormalizer {
        private StringNormalizer() {
        }

        // Empty constructor
        /// <summary>Converts a string to lowercase using Root locale.</summary>
        /// <param name="str">a string to convert</param>
        /// <returns>a converted string</returns>
        public static String ToLowerCase(String str) {
            if (str == null) {
                return null;
            }
            return str.ToLower(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Converts a string to uppercase using Root locale.</summary>
        /// <param name="str">a string to convert</param>
        /// <returns>a converted string</returns>
        public static String ToUpperCase(String str) {
            if (str == null) {
                return null;
            }
            return str.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Converts a string to lowercase using Root locale and trims it.</summary>
        /// <param name="str">a string to convert</param>
        /// <returns>a converted string</returns>
        public static String Normalize(String str) {
            if (str == null) {
                return null;
            }
            return ToLowerCase(str).Trim();
        }
    }
}
