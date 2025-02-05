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

namespace iText.Kernel.Numbering {
    /// <summary>
    /// This class is responsible for converting integer numbers to their
    /// English alphabet letter representations.
    /// </summary>
    public class EnglishAlphabetNumbering {
        protected internal static readonly char[] ALPHABET_LOWERCASE;

        protected internal static readonly char[] ALPHABET_UPPERCASE;

        protected internal const int ALPHABET_LENGTH = 26;

        static EnglishAlphabetNumbering() {
            ALPHABET_LOWERCASE = new char[ALPHABET_LENGTH];
            ALPHABET_UPPERCASE = new char[ALPHABET_LENGTH];
            for (int i = 0; i < ALPHABET_LENGTH; i++) {
                ALPHABET_LOWERCASE[i] = (char)('a' + i);
                ALPHABET_UPPERCASE[i] = (char)('A' + i);
            }
        }

        /// <summary>Converts the given number to its English alphabet lowercase string representation.</summary>
        /// <remarks>
        /// Converts the given number to its English alphabet lowercase string representation.
        /// E.g. 1 will be converted to "a", 2 to "b", ..., 27 to "aa", and so on.
        /// </remarks>
        /// <param name="number">the number greater than zero to be converted</param>
        /// <returns>English alphabet lowercase string representation of an integer</returns>
        public static String ToLatinAlphabetNumberLowerCase(int number) {
            return AlphabetNumbering.ToAlphabetNumber(number, ALPHABET_LOWERCASE);
        }

        /// <summary>Converts the given number to its English alphabet uppercase string representation.</summary>
        /// <remarks>
        /// Converts the given number to its English alphabet uppercase string representation.
        /// E.g. 1 will be converted to "A", 2 to "B", ..., 27 to "AA", and so on.
        /// </remarks>
        /// <param name="number">the number greater than zero to be converted</param>
        /// <returns>English alphabet uppercase string representation of an integer</returns>
        public static String ToLatinAlphabetNumberUpperCase(int number) {
            return AlphabetNumbering.ToAlphabetNumber(number, ALPHABET_UPPERCASE);
        }

        /// <summary>Converts the given number to its English alphabet string representation.</summary>
        /// <remarks>
        /// Converts the given number to its English alphabet string representation.
        /// E.g. for <c>upperCase</c> set to false,
        /// 1 will be converted to "a", 2 to "b", ..., 27 to "aa", and so on.
        /// </remarks>
        /// <param name="number">the number greater than zero to be converted</param>
        /// <param name="upperCase">whether to use uppercase or lowercase alphabet</param>
        /// <returns>English alphabet string representation of an integer</returns>
        public static String ToLatinAlphabetNumber(int number, bool upperCase) {
            return upperCase ? ToLatinAlphabetNumberUpperCase(number) : ToLatinAlphabetNumberLowerCase(number);
        }
    }
}
