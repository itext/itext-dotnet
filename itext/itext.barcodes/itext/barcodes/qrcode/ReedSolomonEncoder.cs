/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;

namespace iText.Barcodes.Qrcode {
    /// <summary>Implements Reed-Solomon encoding, as the name implies.</summary>
    /// <author>Sean Owen</author>
    /// <author>William Rucklidge</author>
    internal sealed class ReedSolomonEncoder {
        private readonly GF256 field;

        private readonly IList<GF256Poly> cachedGenerators;

        /// <summary>
        /// Creates a SolomonEncoder object based on a
        /// <see cref="GF256"/>
        /// object.
        /// </summary>
        /// <remarks>
        /// Creates a SolomonEncoder object based on a
        /// <see cref="GF256"/>
        /// object.
        /// Only QR codes are supported at the moment.
        /// </remarks>
        /// <param name="field">the galois field</param>
        public ReedSolomonEncoder(GF256 field) {
            if (!GF256.QR_CODE_FIELD.Equals(field)) {
                throw new NotSupportedException("Only QR Code is supported at this time");
            }
            this.field = field;
            this.cachedGenerators = new List<GF256Poly>();
            cachedGenerators.Add(new GF256Poly(field, new int[] { 1 }));
        }

        private GF256Poly BuildGenerator(int degree) {
            if (degree >= cachedGenerators.Count) {
                GF256Poly lastGenerator = cachedGenerators[cachedGenerators.Count - 1];
                for (int d = cachedGenerators.Count; d <= degree; d++) {
                    GF256Poly nextGenerator = lastGenerator.Multiply(new GF256Poly(field, new int[] { 1, field.Exp(d - 1) }));
                    cachedGenerators.Add(nextGenerator);
                    lastGenerator = nextGenerator;
                }
            }
            return cachedGenerators[degree];
        }

        /// <summary>Encodes the provided data.</summary>
        /// <param name="toEncode">data to encode</param>
        /// <param name="ecBytes">error correction bytes</param>
        public void Encode(int[] toEncode, int ecBytes) {
            if (ecBytes == 0) {
                throw new ArgumentException("No error correction bytes");
            }
            int dataBytes = toEncode.Length - ecBytes;
            if (dataBytes <= 0) {
                throw new ArgumentException("No data bytes provided");
            }
            GF256Poly generator = BuildGenerator(ecBytes);
            int[] infoCoefficients = new int[dataBytes];
            Array.Copy(toEncode, 0, infoCoefficients, 0, dataBytes);
            GF256Poly info = new GF256Poly(field, infoCoefficients);
            info = info.MultiplyByMonomial(ecBytes, 1);
            GF256Poly remainder = info.Divide(generator)[1];
            int[] coefficients = remainder.GetCoefficients();
            int numZeroCoefficients = ecBytes - coefficients.Length;
            for (int i = 0; i < numZeroCoefficients; i++) {
                toEncode[dataBytes + i] = 0;
            }
            Array.Copy(coefficients, 0, toEncode, dataBytes + numZeroCoefficients, coefficients.Length);
        }
    }
}
