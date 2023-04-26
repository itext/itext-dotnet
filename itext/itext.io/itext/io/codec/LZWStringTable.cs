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
using iText.Commons.Utils;

namespace iText.IO.Codec {
    /// <summary>General purpose LZW String Table.</summary>
    /// <remarks>
    /// General purpose LZW String Table.
    /// Extracted from GIFEncoder by Adam Doppelt
    /// Comments added by Robin Luiten
    /// <c>expandCode</c> added by Robin Luiten
    /// The strLen_ table to give quick access to the lenght of an expanded
    /// code for use by the <c>expandCode</c> method added by Robin.
    /// </remarks>
    public class LZWStringTable {
        /// <summary>codesize + Reserved Codes</summary>
        private const int RES_CODES = 2;

        //0xFFFF
        private const short HASH_FREE = -1;

        //0xFFFF
        private const short NEXT_FIRST = -1;

        private const int MAXBITS = 12;

        private const int MAXSTR = (1 << MAXBITS);

        private const short HASHSIZE = 9973;

        private const short HASHSTEP = 2039;

        // after predecessor character
        internal byte[] strChr_;

        // predecessor string
        internal short[] strNxt_;

        // hash table to find  predecessor + char pairs
        internal short[] strHsh_;

        // next code if adding new prestring + char
        internal short numStrings_;

        /// <summary>
        /// each entry corresponds to a code and contains the length of data
        /// that the code expands to when decoded.
        /// </summary>
        internal int[] strLen_;

        /// <summary>Constructor allocate memory for string store data</summary>
        public LZWStringTable() {
            strChr_ = new byte[MAXSTR];
            strNxt_ = new short[MAXSTR];
            strLen_ = new int[MAXSTR];
            strHsh_ = new short[HASHSIZE];
        }

        /// <param name="index">value of -1 indicates no predecessor [used in initialization]</param>
        /// <param name="b">
        /// the byte [character] to add to the string store which follows
        /// the predecessor string specified the index.
        /// </param>
        /// <returns>
        /// 0xFFFF if no space in table left for addition of predecessor
        /// index and byte b. Else return the code allocated for combination index + b.
        /// </returns>
        public virtual int AddCharString(short index, byte b) {
            int hshidx;
            // if used up all codes
            if (numStrings_ >= MAXSTR) {
                return 0xFFFF;
            }
            hshidx = Hash(index, b);
            while (strHsh_[hshidx] != HASH_FREE) {
                hshidx = (hshidx + HASHSTEP) % HASHSIZE;
            }
            strHsh_[hshidx] = numStrings_;
            strChr_[numStrings_] = b;
            if (index == HASH_FREE) {
                strNxt_[numStrings_] = NEXT_FIRST;
                strLen_[numStrings_] = 1;
            }
            else {
                strNxt_[numStrings_] = index;
                strLen_[numStrings_] = strLen_[index] + 1;
            }
            // return the code and inc for next code
            return numStrings_++;
        }

        /// <param name="index">index to prefix string</param>
        /// <param name="b">the character that follws the index prefix</param>
        /// <returns>
        /// b if param index is HASH_FREE. Else return the code
        /// for this prefix and byte successor
        /// </returns>
        public virtual short FindCharString(short index, byte b) {
            int hshidx;
            int nxtidx;
            if (index == HASH_FREE) {
                // Rob fixed used to sign extend
                return (short)(b & 0xFF);
            }
            hshidx = Hash(index, b);
            // search
            while ((nxtidx = strHsh_[hshidx]) != HASH_FREE) {
                if (strNxt_[nxtidx] == index && strChr_[nxtidx] == b) {
                    return (short)nxtidx;
                }
                hshidx = (hshidx + HASHSTEP) % HASHSIZE;
            }
            //return (short) 0xFFFF;
            return -1;
        }

        /// <param name="codesize">
        /// the size of code to be preallocated for the
        /// string store.
        /// </param>
        public virtual void ClearTable(int codesize) {
            numStrings_ = 0;
            for (int q = 0; q < HASHSIZE; q++) {
                strHsh_[q] = HASH_FREE;
            }
            int w = (1 << codesize) + RES_CODES;
            for (int q = 0; q < w; q++) {
                // init with no prefix
                AddCharString((short)-1, (byte)q);
            }
        }

        public static int Hash(short index, byte lastbyte) {
            return (((short)(lastbyte << 8) ^ index) & 0xFFFF) % HASHSIZE;
        }

        /// <summary>
        /// If expanded data doesn't fit into array only what will fit is written
        /// to buf and the return value indicates how much of the expanded code has
        /// been written to the buf.
        /// </summary>
        /// <remarks>
        /// If expanded data doesn't fit into array only what will fit is written
        /// to buf and the return value indicates how much of the expanded code has
        /// been written to the buf. The next call to expandCode() should be with
        /// the same code and have the skip parameter set the negated value of the
        /// previous return. Successive negative return values should be negated and
        /// added together for next skip parameter value with same code.
        /// </remarks>
        /// <param name="buf">buffer to place expanded data into</param>
        /// <param name="offset">offset to place expanded data</param>
        /// <param name="code">
        /// the code to expand to the byte array it represents.
        /// PRECONDITION This code must already be in the LZSS
        /// </param>
        /// <param name="skipHead">
        /// is the number of bytes at the start of the expanded code to
        /// be skipped before data is written to buf. It is possible that skipHead is
        /// equal to codeLen.
        /// </param>
        /// <returns>
        /// the length of data expanded into buf. If the expanded code is longer
        /// than space left in buf then the value returned is a negative number which when
        /// negated is equal to the number of bytes that were used of the code being expanded.
        /// This negative value also indicates the buffer is full.
        /// </returns>
        public virtual int ExpandCode(byte[] buf, int offset, short code, int skipHead) {
            if (offset == -2) {
                if (skipHead == 1) {
                    skipHead = 0;
                }
            }
            // code == -1 is checked just in case.
            //-1 ~ 0xFFFF
            if (code == -1 || 
                        // DONE no more unpacked
                        skipHead == strLen_[code]) {
                return 0;
            }
            // how much data we are actually expanding
            int expandLen;
            // length of expanded code left
            int codeLen = strLen_[code] - skipHead;
            // how much space left
            int bufSpace = buf.Length - offset;
            if (bufSpace > codeLen) {
                // only got this many to unpack
                expandLen = codeLen;
            }
            else {
                expandLen = bufSpace;
            }
            // only > 0 if codeLen > bufSpace [left overs]
            int skipTail = codeLen - expandLen;
            // initialise to exclusive end address of buffer area
            int idx = offset + expandLen;
            // NOTE: data unpacks in reverse direction and we are placing the
            // unpacked data directly into the array in the correct location.
            while ((idx > offset) && (code != -1)) {
                // skip required of expanded data
                if (--skipTail < 0) {
                    buf[--idx] = strChr_[code];
                }
                // to predecessor code
                code = strNxt_[code];
            }
            if (codeLen > expandLen) {
                // indicate what part of codeLen used
                return -expandLen;
            }
            else {
                // indicate length of dat unpacked
                return expandLen;
            }
        }

        public virtual void Dump(FormattingStreamWriter output) {
            int i;
            for (i = 258; i < numStrings_; ++i) {
                output.WriteLine(" strNxt_[" + i + "] = " + strNxt_[i] + " strChr_ " + JavaUtil.IntegerToHexString(strChr_
                    [i] & 0xFF) + " strLen_ " + JavaUtil.IntegerToHexString(strLen_[i]));
            }
        }
    }
}
