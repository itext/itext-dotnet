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

namespace iText.Barcodes.Qrcode {
    /// <summary>See ISO 18004:2006, 6.5.1.</summary>
    /// <remarks>
    /// See ISO 18004:2006, 6.5.1. This enum encapsulates the four error correction levels
    /// defined by the QR code standard.
    /// </remarks>
    public sealed class ErrorCorrectionLevel {
        /// <summary>L = ~7% correction</summary>
        public static readonly iText.Barcodes.Qrcode.ErrorCorrectionLevel L = new iText.Barcodes.Qrcode.ErrorCorrectionLevel
            (0, 0x01, "L");

        /// <summary>M = ~15% correction</summary>
        public static readonly iText.Barcodes.Qrcode.ErrorCorrectionLevel M = new iText.Barcodes.Qrcode.ErrorCorrectionLevel
            (1, 0x00, "M");

        /// <summary>Q = ~25% correction</summary>
        public static readonly iText.Barcodes.Qrcode.ErrorCorrectionLevel Q = new iText.Barcodes.Qrcode.ErrorCorrectionLevel
            (2, 0x03, "Q");

        /// <summary>H = ~30% correction</summary>
        public static readonly iText.Barcodes.Qrcode.ErrorCorrectionLevel H = new iText.Barcodes.Qrcode.ErrorCorrectionLevel
            (3, 0x02, "H");

        private static readonly iText.Barcodes.Qrcode.ErrorCorrectionLevel[] FOR_BITS = new iText.Barcodes.Qrcode.ErrorCorrectionLevel
            [] { M, L, H, Q };

        private readonly int ordinal;

        private readonly int bits;

        private readonly String name;

        private ErrorCorrectionLevel(int ordinal, int bits, String name) {
            this.ordinal = ordinal;
            this.bits = bits;
            this.name = name;
        }

        /// <summary>Gets the ordinal value.</summary>
        /// <returns>the ordinal</returns>
        public int Ordinal() {
            return ordinal;
        }

        /// <summary>Gets the bits.</summary>
        /// <returns>the bits</returns>
        public int GetBits() {
            return bits;
        }

        /// <summary>Gets the name.</summary>
        /// <returns>the name</returns>
        public String GetName() {
            return name;
        }

        public override String ToString() {
            return name;
        }

        /// <param name="bits">int containing the two bits encoding a QR Code's error correction level</param>
        /// <returns>
        /// 
        /// <see cref="ErrorCorrectionLevel"/>
        /// representing the encoded error correction level
        /// </returns>
        public static iText.Barcodes.Qrcode.ErrorCorrectionLevel ForBits(int bits) {
            if (bits < 0 || bits >= FOR_BITS.Length) {
                throw new ArgumentException();
            }
            return FOR_BITS[bits];
        }
    }
}
