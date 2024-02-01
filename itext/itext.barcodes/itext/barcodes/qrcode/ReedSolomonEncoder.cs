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
using System.Collections.Generic;

namespace iText.Barcodes.Qrcode {
    /// <summary>Implements Reed-Solomon encoding, as the name implies.</summary>
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
