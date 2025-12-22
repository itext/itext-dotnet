/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>Utilities for building Huffman decoding tables.</summary>
    internal sealed class Huffman {
        private const int MAX_LENGTH = 15;

        /// <summary>Returns reverse(reverse(key, len) + 1, len).</summary>
        /// <remarks>
        /// Returns reverse(reverse(key, len) + 1, len).
        /// <para /> reverse(key, len) is the bit-wise reversal of the len least significant bits of key.
        /// </remarks>
        private static int GetNextKey(int key, int len) {
            int step = 1 << (len - 1);
            while ((key & step) != 0) {
                step = step >> 1;
            }
            return (key & (step - 1)) + step;
        }

        /// <summary>
        /// Stores
        /// <paramref name="item"/>
        /// in
        /// <c>table[0], table[step], table[2 * step] .., table[end]</c>.
        /// </summary>
        /// <remarks>
        /// Stores
        /// <paramref name="item"/>
        /// in
        /// <c>table[0], table[step], table[2 * step] .., table[end]</c>.
        /// <para /> Assumes that end is an integer multiple of step.
        /// </remarks>
        private static void ReplicateValue(int[] table, int offset, int step, int end, int item) {
            int pos = end;
            while (pos > 0) {
                pos -= step;
                table[offset + pos] = item;
            }
        }

        /// <param name="count">histogram of bit lengths for the remaining symbols,</param>
        /// <param name="len">code length of the next processed symbol.</param>
        /// <returns>table width of the next 2nd level table.</returns>
        private static int NextTableBitSize(int[] count, int len, int rootBits) {
            int bits = len;
            int left = 1 << (bits - rootBits);
            while (bits < MAX_LENGTH) {
                left -= count[bits];
                if (left <= 0) {
                    break;
                }
                bits++;
                left = left << 1;
            }
            return bits - rootBits;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Builds Huffman lookup table assuming code lengths are in symbol order.</summary>
        /// <returns>number of slots used by resulting Huffman table</returns>
        internal static int BuildHuffmanTable(int[] tableGroup, int tableIdx, int rootBits, int[] codeLengths, int
             codeLengthsSize) {
            int tableOffset = tableGroup[tableIdx];
            int[] sorted = new int[codeLengthsSize];
            // Symbols sorted by code length.
            // TODO(eustas): fill with zeroes?
            int[] count = new int[MAX_LENGTH + 1];
            // Number of codes of each length.
            int[] offset = new int[MAX_LENGTH + 1];
            // Offsets in sorted table for each length.
            // Build histogram of code lengths.
            for (int sym = 0; sym < codeLengthsSize; ++sym) {
                count[codeLengths[sym]]++;
            }
            // Generate offsets into sorted symbol table by code length.
            offset[1] = 0;
            for (int len = 1; len < MAX_LENGTH; ++len) {
                offset[len + 1] = offset[len] + count[len];
            }
            // Sort symbols by length, by symbol order within each length.
            for (int sym = 0; sym < codeLengthsSize; ++sym) {
                if (codeLengths[sym] != 0) {
                    sorted[offset[codeLengths[sym]]++] = sym;
                }
            }
            int tableBits = rootBits;
            int tableSize = 1 << tableBits;
            int totalSize = tableSize;
            // Special case code with only one value.
            if (offset[MAX_LENGTH] == 1) {
                for (int k = 0; k < totalSize; ++k) {
                    tableGroup[tableOffset + k] = sorted[0];
                }
                return totalSize;
            }
            // Fill in root table.
            int key = 0;
            // Reversed prefix code.
            int symbol = 0;
            int step = 1;
            for (int len = 1; len <= rootBits; ++len) {
                step = step << 1;
                while (count[len] > 0) {
                    ReplicateValue(tableGroup, tableOffset + key, step, tableSize, len << 16 | sorted[symbol++]);
                    key = GetNextKey(key, len);
                    count[len]--;
                }
            }
            // Fill in 2nd level tables and add pointers to root table.
            int mask = totalSize - 1;
            int low = -1;
            int currentOffset = tableOffset;
            step = 1;
            for (int len = rootBits + 1; len <= MAX_LENGTH; ++len) {
                step = step << 1;
                while (count[len] > 0) {
                    if ((key & mask) != low) {
                        currentOffset += tableSize;
                        tableBits = NextTableBitSize(count, len, rootBits);
                        tableSize = 1 << tableBits;
                        totalSize += tableSize;
                        low = key & mask;
                        tableGroup[tableOffset + low] = (tableBits + rootBits) << 16 | (currentOffset - tableOffset - low);
                    }
                    ReplicateValue(tableGroup, currentOffset + (key >> rootBits), step, tableSize, (len - rootBits) << 16 | sorted
                        [symbol++]);
                    key = GetNextKey(key, len);
                    count[len]--;
                }
            }
            return totalSize;
        }
//\endcond
    }
//\endcond
}
