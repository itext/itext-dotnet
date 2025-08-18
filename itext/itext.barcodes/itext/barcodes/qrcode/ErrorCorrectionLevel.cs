/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
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
