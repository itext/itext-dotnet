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
using System;
using System.IO;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>A class for performing LZW decoding.</summary>
    public class LZWDecoder {
        internal byte[][] stringTable;

        internal byte[] data = null;

        internal Stream uncompData;

        internal int tableIndex;

        internal int bitsToGet = 9;

        internal int bytePointer;

        internal int bitPointer;

        internal int nextData = 0;

        internal int nextBits = 0;

        internal int[] andTable = new int[] { 511, 1023, 2047, 4095 };

        /// <summary>Creates an LZWDecoder instance.</summary>
        public LZWDecoder() {
        }

        // Empty body
        /// <summary>Method to decode LZW compressed data.</summary>
        /// <param name="data">The compressed data.</param>
        /// <param name="uncompData">Array to return the uncompressed data in.</param>
        public virtual void Decode(byte[] data, Stream uncompData) {
            if (data[0] == (byte)0x00 && data[1] == (byte)0x01) {
                throw new PdfException(KernelExceptionMessageConstant.LZW_FLAVOUR_NOT_SUPPORTED);
            }
            InitializeStringTable();
            this.data = data;
            this.uncompData = uncompData;
            // Initialize pointers
            bytePointer = 0;
            bitPointer = 0;
            nextData = 0;
            nextBits = 0;
            int code;
            int oldCode = 0;
            byte[] @string;
            while ((code = GetNextCode()) != 257) {
                if (code == 256) {
                    InitializeStringTable();
                    code = GetNextCode();
                    if (code == 257) {
                        break;
                    }
                    WriteString(stringTable[code]);
                    oldCode = code;
                }
                else {
                    if (code < tableIndex) {
                        @string = stringTable[code];
                        WriteString(@string);
                        AddStringToTable(stringTable[oldCode], @string[0]);
                        oldCode = code;
                    }
                    else {
                        @string = stringTable[oldCode];
                        @string = ComposeString(@string, @string[0]);
                        WriteString(@string);
                        AddStringToTable(@string);
                        oldCode = code;
                    }
                }
            }
        }

        /// <summary>Initialize the string table.</summary>
        public virtual void InitializeStringTable() {
            stringTable = new byte[8192][];
            for (int i = 0; i < 256; i++) {
                stringTable[i] = new byte[1];
                stringTable[i][0] = (byte)i;
            }
            tableIndex = 258;
            bitsToGet = 9;
        }

        /// <summary>Write out the string just uncompressed.</summary>
        /// <param name="string">content to write to the uncompressed data</param>
        public virtual void WriteString(byte[] @string) {
            try {
                uncompData.Write(@string);
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.LZW_DECODER_EXCEPTION, e);
            }
        }

        /// <summary>Add a new string to the string table.</summary>
        /// <param name="oldString">stored string</param>
        /// <param name="newString">string to be appended to the stored string</param>
        public virtual void AddStringToTable(byte[] oldString, byte newString) {
            int length = oldString.Length;
            byte[] @string = new byte[length + 1];
            Array.Copy(oldString, 0, @string, 0, length);
            @string[length] = newString;
            // Add this new String to the table
            stringTable[tableIndex++] = @string;
            if (tableIndex == 511) {
                bitsToGet = 10;
            }
            else {
                if (tableIndex == 1023) {
                    bitsToGet = 11;
                }
                else {
                    if (tableIndex == 2047) {
                        bitsToGet = 12;
                    }
                }
            }
        }

        /// <summary>Add a new string to the string table.</summary>
        /// <param name="string">byte[] to store in the string table</param>
        public virtual void AddStringToTable(byte[] @string) {
            // Add this new String to the table
            stringTable[tableIndex++] = @string;
            if (tableIndex == 511) {
                bitsToGet = 10;
            }
            else {
                if (tableIndex == 1023) {
                    bitsToGet = 11;
                }
                else {
                    if (tableIndex == 2047) {
                        bitsToGet = 12;
                    }
                }
            }
        }

        /// <summary>Append <c>newString</c> to the end of <c>oldString</c>.</summary>
        /// <param name="oldString">string be appended to</param>
        /// <param name="newString">string that is to be appended to oldString</param>
        /// <returns>combined string</returns>
        public virtual byte[] ComposeString(byte[] oldString, byte newString) {
            int length = oldString.Length;
            byte[] @string = new byte[length + 1];
            Array.Copy(oldString, 0, @string, 0, length);
            @string[length] = newString;
            return @string;
        }

        // Returns the next 9, 10, 11 or 12 bits
        /// <summary>Attempt to get the next code.</summary>
        /// <remarks>
        /// Attempt to get the next code. Exceptions are caught to make
        /// this robust to cases wherein the EndOfInformation code has been
        /// omitted from a strip. Examples of such cases have been observed
        /// in practice.
        /// </remarks>
        /// <returns>next code</returns>
        public virtual int GetNextCode() {
            //
            try {
                nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
                nextBits += 8;
                if (nextBits < bitsToGet) {
                    nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
                    nextBits += 8;
                }
                int code = (nextData >> (nextBits - bitsToGet)) & andTable[bitsToGet - 9];
                nextBits -= bitsToGet;
                return code;
            }
            catch (IndexOutOfRangeException) {
                // Strip not terminated as expected: return EndOfInformation code.
                return 257;
            }
        }
    }
}
