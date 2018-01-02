/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
    /// <summary>
    /// This class is responsible for converting integer numbers to their
    /// Greek alphabet letter representations.
    /// </summary>
    /// <remarks>
    /// This class is responsible for converting integer numbers to their
    /// Greek alphabet letter representations.
    /// We are aware of the fact that the original Greek numbering is different.
    /// See http://www.cogsci.indiana.edu/farg/harry/lan/grknum.htm#ancient
    /// but this isn't implemented yet; the main reason being the fact that we
    /// need a font that has the obsolete Greek characters qoppa and sampi.
    /// So we use standard 24 letter Greek alphabet
    /// </remarks>
    public class GreekAlphabetNumbering {
        protected internal static readonly char[] ALPHABET_LOWERCASE;

        protected internal static readonly char[] ALPHABET_UPPERCASE;

        protected internal const int ALPHABET_LENGTH = 24;

        static GreekAlphabetNumbering() {
            ALPHABET_LOWERCASE = new char[ALPHABET_LENGTH];
            ALPHABET_UPPERCASE = new char[ALPHABET_LENGTH];
            for (int i = 0; i < ALPHABET_LENGTH; i++) {
                ALPHABET_LOWERCASE[i] = (char)(945 + i + (i > 16 ? 1 : 0));
                ALPHABET_UPPERCASE[i] = (char)(913 + i + (i > 16 ? 1 : 0));
            }
        }

        /// <summary>Converts the given number to its Greek alphabet lowercase string representation.</summary>
        /// <remarks>
        /// Converts the given number to its Greek alphabet lowercase string representation.
        /// E.g. 1 will be converted to "alpha", 2 to "beta", and so on.
        /// </remarks>
        /// <param name="number">the number to be converted</param>
        public static String ToGreekAlphabetNumberLowerCase(int number) {
            return AlphabetNumbering.ToAlphabetNumber(number, ALPHABET_LOWERCASE);
        }

        /// <summary>Converts the given number to its Greek alphabet lowercase string representation.</summary>
        /// <remarks>
        /// Converts the given number to its Greek alphabet lowercase string representation.
        /// E.g. 1 will be converted to "A", 2 to "B", and so on.
        /// </remarks>
        /// <param name="number">the number to be converted</param>
        public static String ToGreekAlphabetNumberUpperCase(int number) {
            return AlphabetNumbering.ToAlphabetNumber(number, ALPHABET_UPPERCASE);
        }

        /// <summary>Converts the given number to its Greek alphabet string representation.</summary>
        /// <remarks>
        /// Converts the given number to its Greek alphabet string representation.
        /// E.g. for <code>upperCase</code> set to false,
        /// 1 will be converted to "alpha", 2 to "beta", and so on.
        /// </remarks>
        /// <param name="number">the number to be converted</param>
        /// <param name="upperCase">whether to use uppercase or lowercase alphabet</param>
        public static String ToGreekAlphabetNumber(int number, bool upperCase) {
            return ToGreekAlphabetNumber(number, upperCase, false);
        }

        /// <summary>Converts the given number to its Greek alphabet string representation.</summary>
        /// <remarks>
        /// Converts the given number to its Greek alphabet string representation.
        /// E.g. for <code>upperCase</code> set to false,
        /// 1 will be converted to "alpha", 2 to "beta", and so on.
        /// </remarks>
        /// <param name="number">the number to be converted</param>
        /// <param name="upperCase">whether to use uppercase or lowercase alphabet</param>
        /// <param name="symbolFont">if <code>true</code>, then the string representation will be returned ready to write it in Symbol font
        ///     </param>
        public static String ToGreekAlphabetNumber(int number, bool upperCase, bool symbolFont) {
            String result = upperCase ? ToGreekAlphabetNumberUpperCase(number) : ToGreekAlphabetNumberLowerCase(number
                );
            if (symbolFont) {
                StringBuilder symbolFontStr = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    symbolFontStr.Append(GetSymbolFontChar(result[i]));
                }
                return symbolFontStr.ToString();
            }
            else {
                return result;
            }
        }

        /// <summary>Converts a given greek unicode character code into the code of the corresponding char Symbol font.
        ///     </summary>
        /// <param name="unicodeChar">original unicode char</param>
        /// <returns>the corresponding symbol code in Symbol standard font</returns>
        private static char GetSymbolFontChar(char unicodeChar) {
            switch (unicodeChar) {
                case (char)913: {
                    return 'A';
                }

                case (char)914: {
                    // ALFA
                    return 'B';
                }

                case (char)915: {
                    // BETA
                    return 'G';
                }

                case (char)916: {
                    // GAMMA
                    return 'D';
                }

                case (char)917: {
                    // DELTA
                    return 'E';
                }

                case (char)918: {
                    // EPSILON
                    return 'Z';
                }

                case (char)919: {
                    // ZETA
                    return 'H';
                }

                case (char)920: {
                    // ETA
                    return 'Q';
                }

                case (char)921: {
                    // THETA
                    return 'I';
                }

                case (char)922: {
                    // IOTA
                    return 'K';
                }

                case (char)923: {
                    // KAPPA
                    return 'L';
                }

                case (char)924: {
                    // LAMBDA
                    return 'M';
                }

                case (char)925: {
                    // MU
                    return 'N';
                }

                case (char)926: {
                    // NU
                    return 'X';
                }

                case (char)927: {
                    // XI
                    return 'O';
                }

                case (char)928: {
                    // OMICRON
                    return 'P';
                }

                case (char)929: {
                    // PI
                    return 'R';
                }

                case (char)931: {
                    // RHO
                    return 'S';
                }

                case (char)932: {
                    // SIGMA
                    return 'T';
                }

                case (char)933: {
                    // TAU
                    return 'U';
                }

                case (char)934: {
                    // UPSILON
                    return 'F';
                }

                case (char)935: {
                    // PHI
                    return 'C';
                }

                case (char)936: {
                    // CHI
                    return 'Y';
                }

                case (char)937: {
                    // PSI
                    return 'W';
                }

                case (char)945: {
                    // OMEGA
                    return 'a';
                }

                case (char)946: {
                    // alfa
                    return 'b';
                }

                case (char)947: {
                    // beta
                    return 'g';
                }

                case (char)948: {
                    // gamma
                    return 'd';
                }

                case (char)949: {
                    // delta
                    return 'e';
                }

                case (char)950: {
                    // epsilon
                    return 'z';
                }

                case (char)951: {
                    // zeta
                    return 'h';
                }

                case (char)952: {
                    // eta
                    return 'q';
                }

                case (char)953: {
                    // theta
                    return 'i';
                }

                case (char)954: {
                    // iota
                    return 'k';
                }

                case (char)955: {
                    // kappa
                    return 'l';
                }

                case (char)956: {
                    // lambda
                    return 'm';
                }

                case (char)957: {
                    // mu
                    return 'n';
                }

                case (char)958: {
                    // nu
                    return 'x';
                }

                case (char)959: {
                    // xi
                    return 'o';
                }

                case (char)960: {
                    // omicron
                    return 'p';
                }

                case (char)961: {
                    // pi
                    return 'r';
                }

                case (char)962: {
                    // rho
                    return 'V';
                }

                case (char)963: {
                    // sigma
                    return 's';
                }

                case (char)964: {
                    // sigma
                    return 't';
                }

                case (char)965: {
                    // tau
                    return 'u';
                }

                case (char)966: {
                    // upsilon
                    return 'f';
                }

                case (char)967: {
                    // phi
                    return 'c';
                }

                case (char)968: {
                    // chi
                    return 'y';
                }

                case (char)969: {
                    // psi
                    return 'w';
                }

                default: {
                    // omega
                    return ' ';
                }
            }
        }
    }
}
