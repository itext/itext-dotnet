/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    /// <summary>
    /// Modified from original LZWCompressor to change interface to passing a
    /// buffer of data to be compressed.
    /// </summary>
    public class LZWCompressor {
        /// <summary>base underlying code size of data being compressed 8 for TIFF, 1 to 8 for GIF</summary>
        internal int codeSize_;

        /// <summary>reserved clear code based on code size</summary>
        internal int clearCode_;

        /// <summary>reserved end of data code based on code size</summary>
        internal int endOfInfo_;

        /// <summary>current number bits output for each code</summary>
        internal int numBits_;

        /// <summary>limit at which current number of bits code size has to be increased</summary>
        internal int limit_;

        /// <summary>the prefix code which represents the predecessor string to current input point</summary>
        internal short prefix_;

        /// <summary>output destination for bit codes</summary>
        internal BitFile bf_;

        /// <summary>general purpose LZW string table</summary>
        internal LZWStringTable lzss_;

        /// <summary>modify the limits of the code values in LZW encoding due to TIFF bug / feature</summary>
        internal bool tiffFudge_;

        /// <param name="outputStream">destination for compressed data</param>
        /// <param name="codeSize">the initial code size for the LZW compressor</param>
        /// <param name="TIFF">flag indicating that TIFF lzw fudge needs to be applied</param>
        public LZWCompressor(Stream outputStream, int codeSize, bool TIFF) {
            // set flag for GIF as NOT tiff
            bf_ = new BitFile(outputStream, !TIFF);
            codeSize_ = codeSize;
            tiffFudge_ = TIFF;
            clearCode_ = 1 << codeSize_;
            endOfInfo_ = clearCode_ + 1;
            numBits_ = codeSize_ + 1;
            limit_ = (1 << numBits_) - 1;
            if (tiffFudge_) {
                --limit_;
            }
            //0xFFFF
            prefix_ = -1;
            lzss_ = new LZWStringTable();
            lzss_.ClearTable(codeSize_);
            bf_.WriteBits(clearCode_, numBits_);
        }

        /// <param name="buf">The data to be compressed to output stream</param>
        /// <param name="offset">The offset at which the data starts</param>
        /// <param name="length">The length of the data being compressed</param>
        public virtual void Compress(byte[] buf, int offset, int length) {
            int idx;
            byte c;
            short index;
            int maxOffset = offset + length;
            for (idx = offset; idx < maxOffset; ++idx) {
                c = buf[idx];
                if ((index = lzss_.FindCharString(prefix_, c)) != -1) {
                    prefix_ = index;
                }
                else {
                    bf_.WriteBits(prefix_, numBits_);
                    if (lzss_.AddCharString(prefix_, c) > limit_) {
                        if (numBits_ == 12) {
                            bf_.WriteBits(clearCode_, numBits_);
                            lzss_.ClearTable(codeSize_);
                            numBits_ = codeSize_ + 1;
                        }
                        else {
                            ++numBits_;
                        }
                        limit_ = (1 << numBits_) - 1;
                        if (tiffFudge_) {
                            --limit_;
                        }
                    }
                    prefix_ = (short)((short)c & 0xFF);
                }
            }
        }

        /// <summary>
        /// Indicate to compressor that no more data to go so write out
        /// any remaining buffered data.
        /// </summary>
        public virtual void Flush() {
            if (prefix_ != -1) {
                bf_.WriteBits(prefix_, numBits_);
            }
            bf_.WriteBits(endOfInfo_, numBits_);
            bf_.Flush();
        }
    }
}
