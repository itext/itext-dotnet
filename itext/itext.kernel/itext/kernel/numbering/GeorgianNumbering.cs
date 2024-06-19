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
    /// <summary>This class can produce String combinations representing a georgian numeral.</summary>
    /// <remarks>
    /// This class can produce String combinations representing a georgian numeral.
    /// See https://en.wikipedia.org/wiki/Georgian_numerals
    /// </remarks>
    public class GeorgianNumbering {
        private static readonly GeorgianNumbering.GeorgianDigit[] DIGITS = new GeorgianNumbering.GeorgianDigit[] { 
            new GeorgianNumbering.GeorgianDigit('\u10D0', 1), new GeorgianNumbering.GeorgianDigit('\u10D1', 2), new 
            GeorgianNumbering.GeorgianDigit('\u10D2', 3), new GeorgianNumbering.GeorgianDigit('\u10D3', 4), new GeorgianNumbering.GeorgianDigit
            ('\u10D4', 5), new GeorgianNumbering.GeorgianDigit('\u10D5', 6), new GeorgianNumbering.GeorgianDigit('\u10D6'
            , 7), new GeorgianNumbering.GeorgianDigit('\u10F1', 8), new GeorgianNumbering.GeorgianDigit('\u10D7', 
            9), new GeorgianNumbering.GeorgianDigit('\u10D8', 10), new GeorgianNumbering.GeorgianDigit('\u10D9', 20
            ), new GeorgianNumbering.GeorgianDigit('\u10DA', 30), new GeorgianNumbering.GeorgianDigit('\u10DB', 40
            ), new GeorgianNumbering.GeorgianDigit('\u10DC', 50), new GeorgianNumbering.GeorgianDigit('\u10F2', 60
            ), new GeorgianNumbering.GeorgianDigit('\u10DD', 70), new GeorgianNumbering.GeorgianDigit('\u10DE', 80
            ), new GeorgianNumbering.GeorgianDigit('\u10DF', 90), new GeorgianNumbering.GeorgianDigit('\u10E0', 100
            ), new GeorgianNumbering.GeorgianDigit('\u10E1', 200), new GeorgianNumbering.GeorgianDigit('\u10E2', 300
            ), new GeorgianNumbering.GeorgianDigit('\u10F3', 400), new GeorgianNumbering.GeorgianDigit('\u10E4', 500
            ), new GeorgianNumbering.GeorgianDigit('\u10E5', 600), new GeorgianNumbering.GeorgianDigit('\u10E6', 700
            ), new GeorgianNumbering.GeorgianDigit('\u10E7', 800), new GeorgianNumbering.GeorgianDigit('\u10E8', 900
            ), new GeorgianNumbering.GeorgianDigit('\u10E9', 1000), new GeorgianNumbering.GeorgianDigit('\u10EA', 
            2000), new GeorgianNumbering.GeorgianDigit('\u10EB', 3000), new GeorgianNumbering.GeorgianDigit('\u10EC'
            , 4000), new GeorgianNumbering.GeorgianDigit('\u10ED', 5000), new GeorgianNumbering.GeorgianDigit('\u10EE'
            , 6000), new GeorgianNumbering.GeorgianDigit('\u10F4', 7000), new GeorgianNumbering.GeorgianDigit('\u10EF'
            , 8000), new GeorgianNumbering.GeorgianDigit('\u10F0', 9000), new GeorgianNumbering.GeorgianDigit('\u10F5'
            , 10000) };

        private GeorgianNumbering() {
        }

        /// <summary>Converts the given number to its georgian numeral representation.</summary>
        /// <param name="number">a number greater than zero to be converted to georgian notation</param>
        /// <returns>a georgian numeral representation of an integer.</returns>
        public static String ToGeorgian(int number) {
            StringBuilder result = new StringBuilder();
            for (int i = DIGITS.Length - 1; i >= 0; i--) {
                GeorgianNumbering.GeorgianDigit curDigit = DIGITS[i];
                while (number >= curDigit.value) {
                    result.Append(curDigit.digit);
                    number -= curDigit.value;
                }
            }
            return result.ToString();
        }

        private class GeorgianDigit {
//\cond DO_NOT_DOCUMENT
            internal char digit;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal int value;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal GeorgianDigit(char digit, int value) {
                this.digit = digit;
                this.value = value;
            }
//\endcond
        }
    }
}
