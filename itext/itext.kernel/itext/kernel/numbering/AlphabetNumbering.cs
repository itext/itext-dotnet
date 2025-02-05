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
    /// <summary>This is a general class for alphabet numbering.</summary>
    /// <remarks>
    /// This is a general class for alphabet numbering.
    /// You can specify an alphabet and convert an integer into the corresponding
    /// alphabet number representation.
    /// E.g.: if the alphabet is English letters 'a' to 'z', then
    /// 1 is represented as "a", ..., 26 is represented as "z",
    /// 27 is represented as "aa" and so on.
    /// </remarks>
    public class AlphabetNumbering {
        /// <summary>
        /// Translates a positive integer (not equal to zero)
        /// into an alphabet number using the letters from the specified alphabet.
        /// </summary>
        /// <param name="number">the number</param>
        /// <param name="alphabet">the array containing all possible letters from the alphabet</param>
        /// <returns>a translated number representation</returns>
        public static String ToAlphabetNumber(int number, char[] alphabet) {
            if (number < 1) {
                throw new ArgumentException("The parameter must be a positive integer");
            }
            int cardinality = alphabet.Length;
            number--;
            int bytes = 1;
            long start = 0;
            long symbols = cardinality;
            while (number >= symbols + start) {
                bytes++;
                start += symbols;
                symbols *= cardinality;
            }
            long c = number - start;
            char[] value = new char[bytes];
            while (bytes > 0) {
                value[--bytes] = alphabet[(int)(c % cardinality)];
                c /= cardinality;
            }
            return new String(value);
        }
    }
}
