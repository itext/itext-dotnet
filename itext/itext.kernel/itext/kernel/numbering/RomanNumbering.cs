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
using System.Text;

namespace iText.Kernel.Numbering {
    /// <summary>This class can produce String combinations representing a roman number.</summary>
    /// <remarks>
    /// This class can produce String combinations representing a roman number.
    /// The first roman numbers are: I, II, III, IV, V, VI, VII, VIII, IX, X
    /// See http://en.wikipedia.org/wiki/Roman_numerals
    /// </remarks>
    public class RomanNumbering {
        /// <summary>Array with Roman digits.</summary>
        private static readonly RomanNumbering.RomanDigit[] ROMAN_DIGITS = new RomanNumbering.RomanDigit[] { new RomanNumbering.RomanDigit
            ('m', 1000, false), new RomanNumbering.RomanDigit('d', 500, false), new RomanNumbering.RomanDigit('c', 
            100, true), new RomanNumbering.RomanDigit('l', 50, false), new RomanNumbering.RomanDigit('x', 10, true
            ), new RomanNumbering.RomanDigit('v', 5, false), new RomanNumbering.RomanDigit('i', 1, true) };

        /// <summary>Returns a lower case roman representation of an integer.</summary>
        /// <param name="number">a number to be converted to roman notation</param>
        /// <returns>a lower case roman representation of an integer</returns>
        public static String ToRomanLowerCase(int number) {
            return Convert(number);
        }

        /// <summary>Returns an upper case roman representation of an integer.</summary>
        /// <param name="number">a number to be converted to roman notation</param>
        /// <returns>an upper case roman representation of an integer</returns>
        public static String ToRomanUpperCase(int number) {
            return Convert(number).ToUpperInvariant();
        }

        /// <summary>Returns a roman representation of an integer.</summary>
        /// <param name="number">a number to be converted to roman notation</param>
        /// <param name="upperCase">
        /// <c>true</c> for upper case representation,
        /// <c>false</c> for lower case one
        /// </param>
        /// <returns>a roman representation of an integer</returns>
        public static String ToRoman(int number, bool upperCase) {
            return upperCase ? ToRomanUpperCase(number) : ToRomanLowerCase(number);
        }

        /// <summary>Returns a roman representation of an integer.</summary>
        /// <param name="index">the original number</param>
        /// <returns>the roman number representation (lower case)</returns>
        protected internal static String Convert(int index) {
            StringBuilder buf = new StringBuilder();
            // lower than 0 ? Add minus
            if (index < 0) {
                buf.Append('-');
                index = -index;
            }
            if (index >= 4000) {
                buf.Append('|');
                buf.Append(Convert(index / 1000));
                buf.Append('|');
                // remainder
                index = index - (index / 1000) * 1000;
            }
            // number between 1 and 3999
            int pos = 0;
            while (true) {
                // loop over the array with values for m-d-c-l-x-v-i
                RomanNumbering.RomanDigit dig = ROMAN_DIGITS[pos];
                // adding as many digits as we can
                while (index >= dig.GetValue()) {
                    buf.Append(dig.GetDigit());
                    index -= dig.GetValue();
                }
                // we have the complete number
                if (index <= 0) {
                    break;
                }
                // look for the next digit that can be used in a special way
                int j = pos;
                while (!ROMAN_DIGITS[++j].IsPre()) {
                }
                // does the special notation apply?
                if (index + ROMAN_DIGITS[j].GetValue() >= dig.GetValue()) {
                    buf.Append(ROMAN_DIGITS[j].GetDigit()).Append(dig.GetDigit());
                    index -= dig.GetValue() - ROMAN_DIGITS[j].GetValue();
                }
                pos++;
            }
            return buf.ToString();
        }

        /// <summary>Helper class for Roman Digits</summary>
        private class RomanDigit {
            /// <summary>part of a roman number</summary>
            private readonly char digit;

            /// <summary>value of the roman digit</summary>
            private readonly int value;

            /// <summary>can the digit be used as a prefix</summary>
            private readonly bool pre;

//\cond DO_NOT_DOCUMENT
            /// <summary>Constructs a roman digit</summary>
            /// <param name="digit">the roman digit</param>
            /// <param name="value">the value</param>
            /// <param name="pre">can it be used as a prefix</param>
            internal RomanDigit(char digit, int value, bool pre) {
                this.digit = digit;
                this.value = value;
                this.pre = pre;
            }
//\endcond

            /// <summary>Retrieves the roman digit.</summary>
            /// <returns>roman digit</returns>
            public virtual char GetDigit() {
                return digit;
            }

            /// <summary>Retrieves the value of the roman digit.</summary>
            /// <returns>value</returns>
            public virtual int GetValue() {
                return value;
            }

            /// <summary>Retrieves whether the roman digit can be used as prefix.</summary>
            /// <returns>true if it can, false otherwise</returns>
            public virtual bool IsPre() {
                return pre;
            }
        }
    }
}
