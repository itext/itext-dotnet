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
//\cond DO_NOT_DOCUMENT
    /// <summary>See ISO 18004:2006, 6.4.1, Tables 2 and 3.</summary>
    /// <remarks>
    /// See ISO 18004:2006, 6.4.1, Tables 2 and 3. This enum encapsulates the various modes in which
    /// data can be encoded to bits in the QR code standard.
    /// </remarks>
    internal sealed class Mode {
        // Not really a mode...
        public static readonly iText.Barcodes.Qrcode.Mode TERMINATOR = new iText.Barcodes.Qrcode.Mode(new int[] { 
            0, 0, 0 }, 0x00, "TERMINATOR");

        public static readonly iText.Barcodes.Qrcode.Mode NUMERIC = new iText.Barcodes.Qrcode.Mode(new int[] { 10, 
            12, 14 }, 0x01, "NUMERIC");

        public static readonly iText.Barcodes.Qrcode.Mode ALPHANUMERIC = new iText.Barcodes.Qrcode.Mode(new int[] 
            { 9, 11, 13 }, 0x02, "ALPHANUMERIC");

        // Not supported
        public static readonly iText.Barcodes.Qrcode.Mode STRUCTURED_APPEND = new iText.Barcodes.Qrcode.Mode(new int
            [] { 0, 0, 0 }, 0x03, "STRUCTURED_APPEND");

        public static readonly iText.Barcodes.Qrcode.Mode BYTE = new iText.Barcodes.Qrcode.Mode(new int[] { 8, 16, 
            16 }, 0x04, "BYTE");

        // character counts don't apply
        public static readonly iText.Barcodes.Qrcode.Mode ECI = new iText.Barcodes.Qrcode.Mode(null, 0x07, "ECI");

        public static readonly iText.Barcodes.Qrcode.Mode KANJI = new iText.Barcodes.Qrcode.Mode(new int[] { 8, 10
            , 12 }, 0x08, "KANJI");

        public static readonly iText.Barcodes.Qrcode.Mode FNC1_FIRST_POSITION = new iText.Barcodes.Qrcode.Mode(null
            , 0x05, "FNC1_FIRST_POSITION");

        public static readonly iText.Barcodes.Qrcode.Mode FNC1_SECOND_POSITION = new iText.Barcodes.Qrcode.Mode(null
            , 0x09, "FNC1_SECOND_POSITION");

        private readonly int[] characterCountBitsForVersions;

        private readonly int bits;

        private readonly String name;

        private Mode(int[] characterCountBitsForVersions, int bits, String name) {
            this.characterCountBitsForVersions = characterCountBitsForVersions;
            this.bits = bits;
            this.name = name;
        }

        /// <param name="bits">four bits encoding a QR Code data mode</param>
        /// <returns>
        /// 
        /// <see cref="Mode"/>
        /// encoded by these bits
        /// </returns>
        public static iText.Barcodes.Qrcode.Mode ForBits(int bits) {
            switch (bits) {
                case 0x0: {
                    return TERMINATOR;
                }

                case 0x1: {
                    return NUMERIC;
                }

                case 0x2: {
                    return ALPHANUMERIC;
                }

                case 0x3: {
                    return STRUCTURED_APPEND;
                }

                case 0x4: {
                    return BYTE;
                }

                case 0x5: {
                    return FNC1_FIRST_POSITION;
                }

                case 0x7: {
                    return ECI;
                }

                case 0x8: {
                    return KANJI;
                }

                case 0x9: {
                    return FNC1_SECOND_POSITION;
                }

                default: {
                    throw new ArgumentException();
                }
            }
        }

        /// <param name="version">version in question</param>
        /// <returns>
        /// number of bits used, in this QR Code symbol
        /// <see cref="Version"/>
        /// , to encode the
        /// count of characters that will follow encoded in this
        /// <see cref="Mode"/>
        /// </returns>
        public int GetCharacterCountBits(Version version) {
            if (characterCountBitsForVersions == null) {
                throw new ArgumentException("Character count doesn't apply to this mode");
            }
            int number = version.GetVersionNumber();
            int offset;
            if (number <= 9) {
                offset = 0;
            }
            else {
                if (number <= 26) {
                    offset = 1;
                }
                else {
                    offset = 2;
                }
            }
            return characterCountBitsForVersions[offset];
        }

        /// <returns>the bits of the mode</returns>
        public int GetBits() {
            return bits;
        }

        /// <returns>the name of the mode.</returns>
        public String GetName() {
            return name;
        }

        /// <returns>the name of the mode.</returns>
        public override String ToString() {
            return name;
        }
    }
//\endcond
}
