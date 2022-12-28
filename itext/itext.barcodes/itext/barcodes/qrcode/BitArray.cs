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
using System.Text;

namespace iText.Barcodes.Qrcode {
    /// <summary>A simple, fast array of bits, represented compactly by an array of ints internally.</summary>
    /// <author>Sean Owen</author>
    internal sealed class BitArray {
        private int[] bits;

        private readonly int size;

        public BitArray(int size) {
            if (size < 1) {
                throw new ArgumentException("size must be at least 1");
            }
            this.size = size;
            this.bits = MakeArray(size);
        }

        public int GetSize() {
            return size;
        }

        /// <param name="i">bit to get.</param>
        /// <returns>true iff bit i is set</returns>
        public bool Get(int i) {
            return (bits[i >> 5] & (1 << (i & 0x1F))) != 0;
        }

        /// <summary>Sets bit i.</summary>
        /// <param name="i">bit to set</param>
        public void Set(int i) {
            bits[i >> 5] |= 1 << (i & 0x1F);
        }

        /// <summary>Flips bit i.</summary>
        /// <param name="i">bit to set</param>
        public void Flip(int i) {
            bits[i >> 5] ^= 1 << (i & 0x1F);
        }

        /// <summary>Sets a block of 32 bits, starting at bit i.</summary>
        /// <param name="i">first bit to set</param>
        /// <param name="newBits">
        /// the new value of the next 32 bits. Note again that the least-significant bit
        /// corresponds to bit i, the next-least-significant to i+1, and so on.
        /// </param>
        public void SetBulk(int i, int newBits) {
            bits[i >> 5] = newBits;
        }

        /// <summary>Clears all bits (sets to false).</summary>
        public void Clear() {
            int max = bits.Length;
            for (int i = 0; i < max; i++) {
                bits[i] = 0;
            }
        }

        /// <summary>Efficient method to check if a range of bits is set, or not set.</summary>
        /// <param name="start">start of range, inclusive.</param>
        /// <param name="end">end of range, exclusive</param>
        /// <param name="value">if true, checks that bits in range are set, otherwise checks that they are not set</param>
        /// <returns>true iff all bits are set or not set in range, according to value argument</returns>
        public bool IsRange(int start, int end, bool value) {
            if (end < start) {
                throw new ArgumentException();
            }
            if (end == start) {
                // empty range matches
                return true;
            }
            // will be easier to treat this as the last actually set bit -- inclusive
            end--;
            int firstInt = start >> 5;
            int lastInt = end >> 5;
            for (int i = firstInt; i <= lastInt; i++) {
                int firstBit = i > firstInt ? 0 : start & 0x1F;
                int lastBit = i < lastInt ? 31 : end & 0x1F;
                int mask;
                if (firstBit == 0 && lastBit == 31) {
                    mask = -1;
                }
                else {
                    mask = 0;
                    for (int j = firstBit; j <= lastBit; j++) {
                        mask |= 1 << j;
                    }
                }
                // Return false if we're looking for 1s and the masked bits[i] isn't all 1s (that is,
                // equals the mask, or we're looking for 0s and the masked portion is not all 0s
                if ((bits[i] & mask) != (value ? mask : 0)) {
                    return false;
                }
            }
            return true;
        }

        /// <returns>
        /// underlying array of ints. The first element holds the first 32 bits, and the least
        /// significant bit is bit 0.
        /// </returns>
        public int[] GetBitArray() {
            return bits;
        }

        /// <summary>Reverses all bits in the array.</summary>
        public void Reverse() {
            int[] newBits = new int[bits.Length];
            int size = this.size;
            for (int i = 0; i < size; i++) {
                if (Get(size - i - 1)) {
                    newBits[i >> 5] |= 1 << (i & 0x1F);
                }
            }
            bits = newBits;
        }

        private static int[] MakeArray(int size) {
            int arraySize = size >> 5;
            if ((size & 0x1F) != 0) {
                arraySize++;
            }
            return new int[arraySize];
        }

        public override String ToString() {
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++) {
                if ((i & 0x07) == 0) {
                    result.Append(' ');
                }
                result.Append(Get(i) ? 'X' : '.');
            }
            return result.ToString();
        }
    }
}
