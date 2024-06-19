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

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// JAVAPORT: This should be combined with BitArray in the future, although that class is not yet
    /// dynamically resizeable.
    /// </summary>
    /// <remarks>
    /// JAVAPORT: This should be combined with BitArray in the future, although that class is not yet
    /// dynamically resizeable. This implementation is reasonable but there is a lot of function calling
    /// in loops I'd like to get rid of.
    /// </remarks>
    internal sealed class BitVector {
        private int sizeInBits;

        private byte[] array;

        // For efficiency, start out with some room to work.
        private const int DEFAULT_SIZE_IN_BYTES = 32;

        /// <summary>Create a bitvector usng the default size</summary>
        public BitVector() {
            sizeInBits = 0;
            array = new byte[DEFAULT_SIZE_IN_BYTES];
        }

        // Return the bit value at "index".
        /// <summary>Return the bit value at "index".</summary>
        /// <param name="index">index in the vector</param>
        /// <returns>bit value at "index"</returns>
        public int At(int index) {
            if (index < 0 || index >= sizeInBits) {
                throw new ArgumentException("Bad index: " + index);
            }
            int value = array[index >> 3] & 0xff;
            return (value >> (7 - (index & 0x7))) & 1;
        }

        /// <returns>the number of bits in the bit vector.</returns>
        public int Size() {
            return sizeInBits;
        }

        /// <returns>the number of bytes in the bit vector.</returns>
        public int SizeInBytes() {
            return (sizeInBits + 7) >> 3;
        }

        // Append one bit to the bit vector.
        /// <summary>Append the a bit to the bit vector</summary>
        /// <param name="bit">0 or 1</param>
        public void AppendBit(int bit) {
            if (!(bit == 0 || bit == 1)) {
                throw new ArgumentException("Bad bit");
            }
            int numBitsInLastByte = sizeInBits & 0x7;
            // We'll expand array if we don't have bits in the last byte.
            if (numBitsInLastByte == 0) {
                AppendByte(0);
                sizeInBits -= 8;
            }
            // Modify the last byte.
            array[sizeInBits >> 3] |= (byte)(bit << (7 - numBitsInLastByte));
            ++sizeInBits;
        }

        //
        // REQUIRES:
        //
        //
        //
        //
        //
        /// <summary>Append "numBits" bits in "value" to the bit vector.</summary>
        /// <remarks>
        /// Append "numBits" bits in "value" to the bit vector.
        /// Examples:
        /// - appendBits(0x00, 1) adds 0.
        /// - appendBits(0x00, 4) adds 0000.
        /// - appendBits(0xff, 8) adds 11111111.
        /// </remarks>
        /// <param name="value">int interpreted as bitvector</param>
        /// <param name="numBits">0 &lt;= numBits &lt;= 32.</param>
        public void AppendBits(int value, int numBits) {
            if (numBits < 0 || numBits > 32) {
                throw new ArgumentException("Num bits must be between 0 and 32");
            }
            int numBitsLeft = numBits;
            while (numBitsLeft > 0) {
                // Optimization for byte-oriented appending.
                if ((sizeInBits & 0x7) == 0 && numBitsLeft >= 8) {
                    int newByte = (value >> (numBitsLeft - 8)) & 0xff;
                    AppendByte(newByte);
                    numBitsLeft -= 8;
                }
                else {
                    int bit = (value >> (numBitsLeft - 1)) & 1;
                    AppendBit(bit);
                    --numBitsLeft;
                }
            }
        }

        /// <summary>Append a different BitVector to this BitVector</summary>
        /// <param name="bits">BitVector to append</param>
        public void AppendBitVector(iText.Barcodes.Qrcode.BitVector bits) {
            int size = bits.Size();
            for (int i = 0; i < size; ++i) {
                AppendBit(bits.At(i));
            }
        }

        /// <summary>XOR the contents of this bitvector with the contetns of "other"</summary>
        /// <param name="other">Bitvector of equal length</param>
        public void Xor(iText.Barcodes.Qrcode.BitVector other) {
            if (sizeInBits != other.Size()) {
                throw new ArgumentException("BitVector sizes don't match");
            }
            int sizeInBytes = (sizeInBits + 7) >> 3;
            for (int i = 0; i < sizeInBytes; ++i) {
                // The last byte could be incomplete (i.e. not have 8 bits in
                // it) but there is no problem since 0 XOR 0 == 0.
                array[i] ^= other.array[i];
            }
        }

        // Return String like "01110111" for debugging.
        /// <returns>String representation of the bitvector</returns>
        public override String ToString() {
            StringBuilder result = new StringBuilder(sizeInBits);
            for (int i = 0; i < sizeInBits; ++i) {
                if (At(i) == 0) {
                    result.Append('0');
                }
                else {
                    if (At(i) == 1) {
                        result.Append('1');
                    }
                    else {
                        throw new ArgumentException("Byte isn't 0 or 1");
                    }
                }
            }
            return result.ToString();
        }

        //
        //
        /// <summary>
        /// Callers should not assume that array.length is the exact number of bytes needed to hold
        /// sizeInBits - it will typically be larger for efficiency.
        /// </summary>
        /// <returns>size of the array containing the bitvector</returns>
        public byte[] GetArray() {
            return array;
        }

        //
        //
        /// <summary>
        /// Add a new byte to the end, possibly reallocating and doubling the size of the array if we've
        /// run out of room.
        /// </summary>
        /// <param name="value">byte to add.</param>
        private void AppendByte(int value) {
            if ((sizeInBits >> 3) == array.Length) {
                byte[] newArray = new byte[(array.Length << 1)];
                Array.Copy(array, 0, newArray, 0, array.Length);
                array = newArray;
            }
            array[sizeInBits >> 3] = (byte)value;
            sizeInBits += 8;
        }
    }
//\endcond
}
