/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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

namespace iText.Barcodes.Qrcode {
    /// <summary>
    /// This class contains utility methods for performing mathematical operations over
    /// the Galois Field GF(256).
    /// </summary>
    /// <remarks>
    /// This class contains utility methods for performing mathematical operations over
    /// the Galois Field GF(256). Operations use a given primitive polynomial in calculations.
    /// <para />
    /// Throughout this package, elements of GF(256) are represented as an <c>int</c>
    /// for convenience and speed (but at the cost of memory).
    /// Only the bottom 8 bits are really used.
    /// </remarks>
    /// <author>Sean Owen</author>
    internal sealed class GF256 {
        // x^8 + x^4 + x^3 + x^2 + 1
        public static readonly iText.Barcodes.Qrcode.GF256 QR_CODE_FIELD = new iText.Barcodes.Qrcode.GF256(0x011D);

        // x^8 + x^5 + x^3 + x^2 + 1
        public static readonly iText.Barcodes.Qrcode.GF256 DATA_MATRIX_FIELD = new iText.Barcodes.Qrcode.GF256(0x012D
            );

        private readonly int[] expTable;

        private readonly int[] logTable;

        private readonly GF256Poly zero;

        private readonly GF256Poly one;

        /// <summary>Create a representation of GF(256) using the given primitive polynomial.</summary>
        /// <param name="primitive">
        /// irreducible polynomial whose coefficients are represented by
        /// the bits of an int, where the least-significant bit represents the constant
        /// coefficient
        /// </param>
        private GF256(int primitive) {
            expTable = new int[256];
            logTable = new int[256];
            int x = 1;
            for (int i = 0; i < 256; i++) {
                expTable[i] = x;
                // x = x * 2; we're assuming the generator alpha is 2
                x <<= 1;
                if (x >= 0x100) {
                    x ^= primitive;
                }
            }
            for (int i = 0; i < 255; i++) {
                logTable[expTable[i]] = i;
            }
            // logTable[0] == 0 but this should never be used
            zero = new GF256Poly(this, new int[] { 0 });
            one = new GF256Poly(this, new int[] { 1 });
        }

        internal GF256Poly GetZero() {
            return zero;
        }

        internal GF256Poly GetOne() {
            return one;
        }

        /// <returns>the monomial representing coefficient * x^degree</returns>
        internal GF256Poly BuildMonomial(int degree, int coefficient) {
            if (degree < 0) {
                throw new ArgumentException();
            }
            if (coefficient == 0) {
                return zero;
            }
            int[] coefficients = new int[degree + 1];
            coefficients[0] = coefficient;
            return new GF256Poly(this, coefficients);
        }

        /// <summary>Implements both addition and subtraction -- they are the same in GF(256).</summary>
        /// <returns>sum/difference of a and b</returns>
        internal static int AddOrSubtract(int a, int b) {
            return a ^ b;
        }

        /// <returns>2 to the power of a in GF(256)</returns>
        internal int Exp(int a) {
            return expTable[a];
        }

        /// <returns>base 2 log of a in GF(256)</returns>
        internal int Log(int a) {
            if (a == 0) {
                throw new ArgumentException();
            }
            return logTable[a];
        }

        /// <returns>multiplicative inverse of a</returns>
        internal int Inverse(int a) {
            if (a == 0) {
                throw new ArithmeticException();
            }
            return expTable[255 - logTable[a]];
        }

        /// <param name="a"/>
        /// <param name="b"/>
        /// <returns>product of a and b in GF(256)</returns>
        internal int Multiply(int a, int b) {
            if (a == 0 || b == 0) {
                return 0;
            }
            if (a == 1) {
                return b;
            }
            if (b == 1) {
                return a;
            }
            return expTable[(logTable[a] + logTable[b]) % 255];
        }
    }
}
