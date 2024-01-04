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
using System.IO;

namespace iText.IO.Codec {
    /// <summary>Came from GIFEncoder initially.</summary>
    /// <remarks>
    /// Came from GIFEncoder initially.
    /// Modified - to allow for output compressed data without the block counts
    /// which breakup the compressed data stream for GIF.
    /// </remarks>
    internal class BitFile {
        internal Stream output;

        internal byte[] buffer;

        internal int index;

        // bits left at current index that are avail.
        internal int bitsLeft;

        /// <summary>note this also indicates gif format BITFile.</summary>
        internal bool blocks = false;

        /// <param name="output">destination for output data</param>
        /// <param name="blocks">GIF LZW requires block counts for output data</param>
        public BitFile(Stream output, bool blocks) {
            this.output = output;
            this.blocks = blocks;
            buffer = new byte[256];
            index = 0;
            bitsLeft = 8;
        }

        public virtual void Flush() {
            int numBytes = index + (bitsLeft == 8 ? 0 : 1);
            if (numBytes > 0) {
                if (blocks) {
                    output.Write(numBytes);
                }
                output.Write(buffer, 0, numBytes);
                buffer[0] = 0;
                index = 0;
                bitsLeft = 8;
            }
        }

        public virtual void WriteBits(int bits, int numbits) {
            int bitsWritten = 0;
            // gif block count
            int numBytes = 255;
            do {
                // This handles the GIF block count stuff
                if ((index == 254 && bitsLeft == 0) || index > 254) {
                    if (blocks) {
                        output.Write(numBytes);
                    }
                    output.Write(buffer, 0, numBytes);
                    buffer[0] = 0;
                    index = 0;
                    bitsLeft = 8;
                }
                // bits contents fit in current index byte
                if (numbits <= bitsLeft) {
                    // GIF
                    if (blocks) {
                        buffer[index] |= (byte)((bits & ((1 << numbits) - 1)) << (8 - bitsLeft));
                        bitsWritten += numbits;
                        bitsLeft -= numbits;
                        numbits = 0;
                    }
                    else {
                        buffer[index] |= (byte)((bits & ((1 << numbits) - 1)) << (bitsLeft - numbits));
                        bitsWritten += numbits;
                        bitsLeft -= numbits;
                        numbits = 0;
                    }
                }
                else {
                    // bits overflow from current byte to next.
                    // GIF
                    if (blocks) {
                        // if bits  > space left in current byte then the lowest order bits
                        // of code are taken and put in current byte and rest put in next.
                        buffer[index] |= (byte)((bits & ((1 << bitsLeft) - 1)) << (8 - bitsLeft));
                        bitsWritten += bitsLeft;
                        bits >>= bitsLeft;
                        numbits -= bitsLeft;
                        buffer[++index] = 0;
                        bitsLeft = 8;
                    }
                    else {
                        // if bits  > space left in current byte then the highest order bits
                        // of code are taken and put in current byte and rest put in next.
                        // at highest order bit location !!
                        int topbits = ((int)(((uint)bits) >> (numbits - bitsLeft))) & ((1 << bitsLeft) - 1);
                        buffer[index] |= (byte)topbits;
                        // ok this many bits gone off the top
                        numbits -= bitsLeft;
                        bitsWritten += bitsLeft;
                        // next index
                        buffer[++index] = 0;
                        bitsLeft = 8;
                    }
                }
            }
            while (numbits != 0);
        }
    }
}
