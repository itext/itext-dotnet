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
using System.Text;

namespace iText.Kernel.Numbering {
    /// <summary>This class can produce String combinations representing an armenian numeral.</summary>
    /// <remarks>
    /// This class can produce String combinations representing an armenian numeral.
    /// See https://en.wikipedia.org/wiki/Armenian_numerals
    /// </remarks>
    public class ArmenianNumbering {
        private static readonly ArmenianNumbering.ArmenianDigit[] DIGITS = new ArmenianNumbering.ArmenianDigit[] { 
            new ArmenianNumbering.ArmenianDigit('\u0531', 1), new ArmenianNumbering.ArmenianDigit('\u0532', 2), new 
            ArmenianNumbering.ArmenianDigit('\u0533', 3), new ArmenianNumbering.ArmenianDigit('\u0534', 4), new ArmenianNumbering.ArmenianDigit
            ('\u0535', 5), new ArmenianNumbering.ArmenianDigit('\u0536', 6), new ArmenianNumbering.ArmenianDigit('\u0537'
            , 7), new ArmenianNumbering.ArmenianDigit('\u0538', 8), new ArmenianNumbering.ArmenianDigit('\u0539', 
            9), new ArmenianNumbering.ArmenianDigit('\u053A', 10), new ArmenianNumbering.ArmenianDigit('\u053B', 20
            ), new ArmenianNumbering.ArmenianDigit('\u053C', 30), new ArmenianNumbering.ArmenianDigit('\u053D', 40
            ), new ArmenianNumbering.ArmenianDigit('\u053E', 50), new ArmenianNumbering.ArmenianDigit('\u053F', 60
            ), new ArmenianNumbering.ArmenianDigit('\u0540', 70), new ArmenianNumbering.ArmenianDigit('\u0541', 80
            ), new ArmenianNumbering.ArmenianDigit('\u0542', 90), new ArmenianNumbering.ArmenianDigit('\u0543', 100
            ), new ArmenianNumbering.ArmenianDigit('\u0544', 200), new ArmenianNumbering.ArmenianDigit('\u0545', 300
            ), new ArmenianNumbering.ArmenianDigit('\u0546', 400), new ArmenianNumbering.ArmenianDigit('\u0547', 500
            ), new ArmenianNumbering.ArmenianDigit('\u0548', 600), new ArmenianNumbering.ArmenianDigit('\u0549', 700
            ), new ArmenianNumbering.ArmenianDigit('\u054A', 800), new ArmenianNumbering.ArmenianDigit('\u054B', 900
            ), new ArmenianNumbering.ArmenianDigit('\u054C', 1000), new ArmenianNumbering.ArmenianDigit('\u054D', 
            2000), new ArmenianNumbering.ArmenianDigit('\u054E', 3000), new ArmenianNumbering.ArmenianDigit('\u054F'
            , 4000), new ArmenianNumbering.ArmenianDigit('\u0550', 5000), new ArmenianNumbering.ArmenianDigit('\u0551'
            , 6000), new ArmenianNumbering.ArmenianDigit('\u0552', 7000), new ArmenianNumbering.ArmenianDigit('\u0553'
            , 8000), new ArmenianNumbering.ArmenianDigit('\u0554', 9000) };

        private ArmenianNumbering() {
        }

        /// <summary>Converts an integer to armenian numeral representation.</summary>
        /// <param name="number">a number greater than zero to be converted to armenian notation</param>
        /// <returns>an armenian numeral representation of an integer.</returns>
        public static String ToArmenian(int number) {
            StringBuilder result = new StringBuilder();
            for (int i = DIGITS.Length - 1; i >= 0; i--) {
                ArmenianNumbering.ArmenianDigit curDigit = DIGITS[i];
                while (number >= curDigit.value) {
                    result.Append(curDigit.digit);
                    number -= curDigit.value;
                }
            }
            return result.ToString();
        }

        private class ArmenianDigit {
//\cond DO_NOT_DOCUMENT
            internal char digit;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int value;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ArmenianDigit(char digit, int value) {
                this.digit = digit;
                this.value = value;
            }
//\endcond
        }
    }
}
