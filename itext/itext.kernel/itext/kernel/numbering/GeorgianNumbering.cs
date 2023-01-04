/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
            internal char digit;

            internal int value;

            internal GeorgianDigit(char digit, int value) {
                this.digit = digit;
                this.value = value;
            }
        }
    }
}
