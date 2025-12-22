/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>API for Brotli decompression.</summary>
    internal sealed class Decode {
//\cond DO_NOT_DOCUMENT
        internal const int MIN_LARGE_WINDOW_BITS = 10;
//\endcond

//\cond DO_NOT_DOCUMENT
        /* Maximum was chosen to be 30 to allow efficient decoder implementation.
        * Format allows bigger window, but Java does not support 2G+ arrays. */
        internal const int MAX_LARGE_WINDOW_BITS = 30;
//\endcond

        //----------------------------------------------------------------------------
        // RunningState
        //----------------------------------------------------------------------------
        // NB: negative values are used for errors.
        private const int UNINITIALIZED = 0;

        private const int INITIALIZED = 1;

        private const int BLOCK_START = 2;

        private const int COMPRESSED_BLOCK_START = 3;

        private const int MAIN_LOOP = 4;

        private const int READ_METADATA = 5;

        private const int COPY_UNCOMPRESSED = 6;

        private const int INSERT_LOOP = 7;

        private const int COPY_LOOP = 8;

        private const int USE_DICTIONARY = 9;

        private const int FINISHED = 10;

        private const int CLOSED = 11;

        private const int INIT_WRITE = 12;

        private const int WRITE = 13;

        private const int COPY_FROM_COMPOUND_DICTIONARY = 14;

        private const int DEFAULT_CODE_LENGTH = 8;

        private const int CODE_LENGTH_REPEAT_CODE = 16;

        private const int NUM_LITERAL_CODES = 256;

        private const int NUM_COMMAND_CODES = 704;

        private const int NUM_BLOCK_LENGTH_CODES = 26;

        private const int LITERAL_CONTEXT_BITS = 6;

        private const int DISTANCE_CONTEXT_BITS = 2;

        private const int CD_BLOCK_MAP_BITS = 8;

        private const int HUFFMAN_TABLE_BITS = 8;

        private const int HUFFMAN_TABLE_MASK = 0xFF;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Maximum possible Huffman table size for an alphabet size of (index * 32),
        /// max code length 15 and root table bits 8.
        /// </summary>
        /// <remarks>
        /// Maximum possible Huffman table size for an alphabet size of (index * 32),
        /// max code length 15 and root table bits 8.
        /// The biggest alphabet is "command" - 704 symbols. Though "distance" alphabet could theoretically
        /// outreach that limit (for 62 extra bit distances), practically it is limited by
        /// MAX_ALLOWED_DISTANCE and never gets bigger than 544 symbols.
        /// </remarks>
        internal static readonly int[] MAX_HUFFMAN_TABLE_SIZE = new int[] { 256, 402, 436, 468, 500, 534, 566, 598
            , 630, 662, 694, 726, 758, 790, 822, 854, 886, 920, 952, 984, 1016, 1048, 1080 };
//\endcond

        private const int HUFFMAN_TABLE_SIZE_26 = 396;

        private const int HUFFMAN_TABLE_SIZE_258 = 632;

        private const int CODE_LENGTH_CODES = 18;

        private static readonly int[] CODE_LENGTH_CODE_ORDER = new int[] { 1, 2, 3, 4, 0, 5, 17, 6, 16, 7, 8, 9, 10
            , 11, 12, 13, 14, 15 };

        private const int NUM_DISTANCE_SHORT_CODES = 16;

        private static readonly int[] DISTANCE_SHORT_CODE_INDEX_OFFSET = new int[] { 0, 3, 2, 1, 0, 0, 0, 0, 0, 0, 
            3, 3, 3, 3, 3, 3 };

        private static readonly int[] DISTANCE_SHORT_CODE_VALUE_OFFSET = new int[] { 0, 0, 0, 0, -1, 1, -2, 2, -3, 
            3, -1, 1, -2, 2, -3, 3 };

        /// <summary>Static Huffman code for the code length code lengths.</summary>
        private static readonly int[] FIXED_TABLE = new int[] { 0x020000, 0x020004, 0x020003, 0x030002, 0x020000, 
            0x020004, 0x020003, 0x040001, 0x020000, 0x020004, 0x020003, 0x030002, 0x020000, 0x020004, 0x020003, 0x040005
             };

//\cond DO_NOT_DOCUMENT
        // TODO(eustas): generalize.
        internal const int MAX_TRANSFORMED_WORD_LENGTH = 5 + 24 + 8;
//\endcond

        private const int MAX_DISTANCE_BITS = 24;

        private const int MAX_LARGE_WINDOW_DISTANCE_BITS = 62;

        /// <summary>Safe distance limit.</summary>
        /// <remarks>
        /// Safe distance limit.
        /// Limit ((1 &lt;&lt; 31) - 4) allows safe distance calculation without overflows,
        /// given the distance alphabet size is limited to corresponding size.
        /// </remarks>
        private const int MAX_ALLOWED_DISTANCE = 0x7FFFFFFC;

//\cond DO_NOT_DOCUMENT
        //----------------------------------------------------------------------------
        // Prefix code LUT.
        //----------------------------------------------------------------------------
        internal static readonly int[] BLOCK_LENGTH_OFFSET = new int[] { 1, 5, 9, 13, 17, 25, 33, 41, 49, 65, 81, 
            97, 113, 145, 177, 209, 241, 305, 369, 497, 753, 1265, 2289, 4337, 8433, 16625 };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly int[] BLOCK_LENGTH_N_BITS = new int[] { 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 
            5, 5, 6, 6, 7, 8, 9, 10, 11, 12, 13, 24 };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly short[] INSERT_LENGTH_N_BITS = new short[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0C, 0x0E, 
            0x18 };
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly short[] COPY_LENGTH_N_BITS = new short[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            , 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x18
             };
//\endcond

//\cond DO_NOT_DOCUMENT
        // Each command is represented with 4x16-bit values:
        //  * [insertLenExtraBits, copyLenExtraBits]
        //  * insertLenOffset
        //  * copyLenOffset
        //  * distanceContext
        internal static readonly short[] CMD_LOOKUP = new short[NUM_COMMAND_CODES * 4];
//\endcond

        static Decode() {
            UnpackCommandLookupTable(CMD_LOOKUP);
        }

        private static int Log2floor(int i) {
            // REQUIRED: i > 0
            int result = -1;
            int step = 16;
            int v = i;
            while (step > 0) {
                int next = v >> step;
                if (next != 0) {
                    result += step;
                    v = next;
                }
                step = step >> 1;
            }
            return result + v;
        }

        private static int CalculateDistanceAlphabetSize(int npostfix, int ndirect, int maxndistbits) {
            return NUM_DISTANCE_SHORT_CODES + ndirect + 2 * (maxndistbits << npostfix);
        }

        // TODO(eustas): add a correctness test for this function when
        //               large-window and dictionary are implemented.
        private static int CalculateDistanceAlphabetLimit(State s, int maxDistance, int npostfix, int ndirect) {
            if (maxDistance < ndirect + (2 << npostfix)) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_MAX_DISTANCE_TOO_SMALL);
            }
            int offset = ((maxDistance - ndirect) >> npostfix) + 4;
            int ndistbits = Log2floor(offset) - 1;
            int group = ((ndistbits - 1) << 1) | ((offset >> ndistbits) & 1);
            return ((group - 1) << npostfix) + (1 << npostfix) + ndirect + NUM_DISTANCE_SHORT_CODES;
        }

        private static void UnpackCommandLookupTable(short[] cmdLookup) {
            int[] insertLengthOffsets = new int[24];
            int[] copyLengthOffsets = new int[24];
            copyLengthOffsets[0] = 2;
            for (int i = 0; i < 23; ++i) {
                insertLengthOffsets[i + 1] = insertLengthOffsets[i] + (1 << (int)INSERT_LENGTH_N_BITS[i]);
                copyLengthOffsets[i + 1] = copyLengthOffsets[i] + (1 << (int)COPY_LENGTH_N_BITS[i]);
            }
            for (int cmdCode = 0; cmdCode < NUM_COMMAND_CODES; ++cmdCode) {
                int rangeIdx = cmdCode >> 6;
                /* -4 turns any regular distance code to negative. */
                int distanceContextOffset = -4;
                if (rangeIdx >= 2) {
                    rangeIdx -= 2;
                    distanceContextOffset = 0;
                }
                int insertCode = (((0x29850 >> (rangeIdx * 2)) & 0x3) << 3) | ((cmdCode >> 3) & 7);
                int copyCode = (((0x26244 >> (rangeIdx * 2)) & 0x3) << 3) | (cmdCode & 7);
                int copyLengthOffset = copyLengthOffsets[copyCode];
                int distanceContext = distanceContextOffset + Utils.Min(copyLengthOffset, 5) - 2;
                int index = cmdCode * 4;
                cmdLookup[index + 0] = (short)((int)INSERT_LENGTH_N_BITS[insertCode] | ((int)COPY_LENGTH_N_BITS[copyCode] 
                    << 8));
                cmdLookup[index + 1] = (short)insertLengthOffsets[insertCode];
                cmdLookup[index + 2] = (short)copyLengthOffsets[copyCode];
                cmdLookup[index + 3] = (short)distanceContext;
            }
        }

        /// <summary>Reads brotli stream header and parses "window bits".</summary>
        /// <param name="s">initialized state, before any read is performed.</param>
        /// <returns>-1 if header is invalid</returns>
        private static int DecodeWindowBits(State s) {
            /* Change the meaning of flag. Before that step it means "decoder must be capable of reading
            * "large-window" brotli stream. After this step it means that "large-window" feature
            * is actually detected. Despite the window size could be same as before (lgwin = 10..24),
            * encoded distances are allowed to be much greater, thus bigger dictionary could be used. */
            int largeWindowEnabled = s.isLargeWindow;
            s.isLargeWindow = 0;
            BitReader.FillBitWindow(s);
            if (BitReader.ReadFewBits(s, 1) == 0) {
                return 16;
            }
            int n = BitReader.ReadFewBits(s, 3);
            if (n != 0) {
                return 17 + n;
            }
            n = BitReader.ReadFewBits(s, 3);
            if (n != 0) {
                if (n == 1) {
                    if (largeWindowEnabled == 0) {
                        /* Reserved value in regular brotli stream. */
                        return -1;
                    }
                    s.isLargeWindow = 1;
                    /* Check "reserved" bit for future (post-large-window) extensions. */
                    if (BitReader.ReadFewBits(s, 1) == 1) {
                        return -1;
                    }
                    n = BitReader.ReadFewBits(s, 6);
                    if (n < MIN_LARGE_WINDOW_BITS || n > MAX_LARGE_WINDOW_BITS) {
                        /* Encoded window bits value is too small or too big. */
                        return -1;
                    }
                    return n;
                }
                return 8 + n;
            }
            return 17;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Switch decoder to "eager" mode.</summary>
        /// <remarks>
        /// Switch decoder to "eager" mode.
        /// In "eager" mode decoder returns as soon as there is enough data to fill output buffer.
        /// </remarks>
        /// <param name="s">initialized state, before any read is performed.</param>
        internal static int EnableEagerOutput(State s) {
            if (s.runningState != INITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_FRESH);
            }
            s.isEager = 1;
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int EnableLargeWindow(State s) {
            if (s.runningState != INITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_FRESH);
            }
            s.isLargeWindow = 1;
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // TODO(eustas): do we need byte views?
        internal static int AttachDictionaryChunk(State s, byte[] data) {
            if (s.runningState != INITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_FRESH);
            }
            if (s.cdNumChunks == 0) {
                s.cdChunks = new byte[16][];
                s.cdChunkOffsets = new int[16];
                s.cdBlockBits = -1;
            }
            if (s.cdNumChunks == 15) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_TOO_MANY_DICTIONARY_CHUNKS);
            }
            s.cdChunks[s.cdNumChunks] = data;
            s.cdNumChunks++;
            s.cdTotalSize += data.Length;
            s.cdChunkOffsets[s.cdNumChunks] = s.cdTotalSize;
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Associate input with decoder state.</summary>
        /// <param name="s">uninitialized state without associated input</param>
        internal static int InitState(State s) {
            if (s.runningState != UNINITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_UNINITIALIZED);
            }
            /* 6 trees + 1 extra "offset" slot to simplify table decoding logic. */
            s.blockTrees = new int[7 + 3 * (HUFFMAN_TABLE_SIZE_258 + HUFFMAN_TABLE_SIZE_26)];
            s.blockTrees[0] = 7;
            s.distRbIdx = 3;
            int result = CalculateDistanceAlphabetLimit(s, MAX_ALLOWED_DISTANCE, 3, 15 << 3);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int maxDistanceAlphabetLimit = result;
            s.distExtraBits = new byte[maxDistanceAlphabetLimit];
            s.distOffset = new int[maxDistanceAlphabetLimit];
            result = BitReader.InitBitReader(s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.runningState = INITIALIZED;
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int Close(State s) {
            if (s.runningState == UNINITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_INITIALIZED);
            }
            if (s.runningState > 0) {
                s.runningState = CLOSED;
            }
            return BrotliError.BROTLI_OK;
        }
//\endcond

        /// <summary>Decodes a number in the range [0..255], by reading 1 - 11 bits.</summary>
        private static int DecodeVarLenUnsignedByte(State s) {
            BitReader.FillBitWindow(s);
            if (BitReader.ReadFewBits(s, 1) != 0) {
                int n = BitReader.ReadFewBits(s, 3);
                if (n == 0) {
                    return 1;
                }
                return BitReader.ReadFewBits(s, n) + (1 << n);
            }
            return 0;
        }

        private static int DecodeMetaBlockLength(State s) {
            BitReader.FillBitWindow(s);
            s.inputEnd = BitReader.ReadFewBits(s, 1);
            s.metaBlockLength = 0;
            s.isUncompressed = 0;
            s.isMetadata = 0;
            if ((s.inputEnd != 0) && BitReader.ReadFewBits(s, 1) != 0) {
                return BrotliError.BROTLI_OK;
            }
            int sizeNibbles = BitReader.ReadFewBits(s, 2) + 4;
            if (sizeNibbles == 7) {
                s.isMetadata = 1;
                if (BitReader.ReadFewBits(s, 1) != 0) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_CORRUPTED_RESERVED_BIT);
                }
                int sizeBytes = BitReader.ReadFewBits(s, 2);
                if (sizeBytes == 0) {
                    return BrotliError.BROTLI_OK;
                }
                for (int i = 0; i < sizeBytes; ++i) {
                    BitReader.FillBitWindow(s);
                    int bits = BitReader.ReadFewBits(s, 8);
                    if (bits == 0 && i + 1 == sizeBytes && sizeBytes > 1) {
                        return Utils.MakeError(s, BrotliError.BROTLI_ERROR_EXUBERANT_NIBBLE);
                    }
                    s.metaBlockLength += bits << (i * 8);
                }
            }
            else {
                for (int i = 0; i < sizeNibbles; ++i) {
                    BitReader.FillBitWindow(s);
                    int bits = BitReader.ReadFewBits(s, 4);
                    if (bits == 0 && i + 1 == sizeNibbles && sizeNibbles > 4) {
                        return Utils.MakeError(s, BrotliError.BROTLI_ERROR_EXUBERANT_NIBBLE);
                    }
                    s.metaBlockLength += bits << (i * 4);
                }
            }
            s.metaBlockLength++;
            if (s.inputEnd == 0) {
                s.isUncompressed = BitReader.ReadFewBits(s, 1);
            }
            return BrotliError.BROTLI_OK;
        }

        /// <summary>Decodes the next Huffman code from bit-stream.</summary>
        private static int ReadSymbol(int[] tableGroup, int tableIdx, State s) {
            int offset = tableGroup[tableIdx];
            int v = BitReader.PeekBits(s);
            offset += v & HUFFMAN_TABLE_MASK;
            int bits = tableGroup[offset] >> 16;
            int sym = tableGroup[offset] & 0xFFFF;
            if (bits <= HUFFMAN_TABLE_BITS) {
                s.bitOffset += bits;
                return sym;
            }
            offset += sym;
            int mask = (1 << bits) - 1;
            offset += Utils.Shr32(v & mask, HUFFMAN_TABLE_BITS);
            s.bitOffset += ((tableGroup[offset] >> 16) + HUFFMAN_TABLE_BITS);
            return tableGroup[offset] & 0xFFFF;
        }

        private static int ReadBlockLength(int[] tableGroup, int tableIdx, State s) {
            BitReader.FillBitWindow(s);
            int code = ReadSymbol(tableGroup, tableIdx, s);
            int n = BLOCK_LENGTH_N_BITS[code];
            BitReader.FillBitWindow(s);
            return BLOCK_LENGTH_OFFSET[code] + BitReader.ReadBits(s, n);
        }

        private static void MoveToFront(int[] v, int index) {
            int i = index;
            int value = v[i];
            while (i > 0) {
                v[i] = v[i - 1];
                i--;
            }
            v[0] = value;
        }

        private static void InverseMoveToFrontTransform(byte[] v, int vLen) {
            int[] mtf = new int[256];
            for (int i = 0; i < 256; ++i) {
                mtf[i] = i;
            }
            for (int i = 0; i < vLen; ++i) {
                int index = (int)v[i] & 0xFF;
                v[i] = (byte)mtf[index];
                if (index != 0) {
                    MoveToFront(mtf, index);
                }
            }
        }

        private static int ReadHuffmanCodeLengths(int[] codeLengthCodeLengths, int numSymbols, int[] codeLengths, 
            State s) {
            int symbol = 0;
            int prevCodeLen = DEFAULT_CODE_LENGTH;
            int repeat = 0;
            int repeatCodeLen = 0;
            int space = 32768;
            int[] table = new int[32 + 1];
            /* Speculative single entry table group. */
            int tableIdx = table.Length - 1;
            Huffman.BuildHuffmanTable(table, tableIdx, 5, codeLengthCodeLengths, CODE_LENGTH_CODES);
            while (symbol < numSymbols && space > 0) {
                if (s.halfOffset > BitReader.HALF_WATERLINE) {
                    int result = BitReader.ReadMoreInput(s);
                    if (result < BrotliError.BROTLI_OK) {
                        return result;
                    }
                }
                BitReader.FillBitWindow(s);
                int p = BitReader.PeekBits(s) & 31;
                s.bitOffset += table[p] >> 16;
                int codeLen = table[p] & 0xFFFF;
                if (codeLen < CODE_LENGTH_REPEAT_CODE) {
                    repeat = 0;
                    codeLengths[symbol++] = codeLen;
                    if (codeLen != 0) {
                        prevCodeLen = codeLen;
                        space -= 32768 >> codeLen;
                    }
                }
                else {
                    int extraBits = codeLen - 14;
                    int newLen = 0;
                    if (codeLen == CODE_LENGTH_REPEAT_CODE) {
                        newLen = prevCodeLen;
                    }
                    if (repeatCodeLen != newLen) {
                        repeat = 0;
                        repeatCodeLen = newLen;
                    }
                    int oldRepeat = repeat;
                    if (repeat > 0) {
                        repeat -= 2;
                        repeat = repeat << extraBits;
                    }
                    BitReader.FillBitWindow(s);
                    repeat += BitReader.ReadFewBits(s, extraBits) + 3;
                    int repeatDelta = repeat - oldRepeat;
                    if (symbol + repeatDelta > numSymbols) {
                        return Utils.MakeError(s, BrotliError.BROTLI_ERROR_CORRUPTED_CODE_LENGTH_TABLE);
                    }
                    for (int i = 0; i < repeatDelta; ++i) {
                        codeLengths[symbol++] = repeatCodeLen;
                    }
                    if (repeatCodeLen != 0) {
                        space -= repeatDelta << (15 - repeatCodeLen);
                    }
                }
            }
            if (space != 0) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_UNUSED_HUFFMAN_SPACE);
            }
            // TODO(eustas): Pass max_symbol to Huffman table builder instead?
            Utils.FillIntsWithZeroes(codeLengths, symbol, numSymbols);
            return BrotliError.BROTLI_OK;
        }

        private static int CheckDupes(State s, int[] symbols, int length) {
            for (int i = 0; i < length - 1; ++i) {
                for (int j = i + 1; j < length; ++j) {
                    if (symbols[i] == symbols[j]) {
                        return Utils.MakeError(s, BrotliError.BROTLI_ERROR_DUPLICATE_SIMPLE_HUFFMAN_SYMBOL);
                    }
                }
            }
            return BrotliError.BROTLI_OK;
        }

        /// <summary>Reads up to 4 symbols directly and applies predefined histograms.</summary>
        private static int ReadSimpleHuffmanCode(int alphabetSizeMax, int alphabetSizeLimit, int[] tableGroup, int
             tableIdx, State s) {
            // TODO(eustas): Avoid allocation?
            int[] codeLengths = new int[alphabetSizeLimit];
            int[] symbols = new int[4];
            int maxBits = 1 + Log2floor(alphabetSizeMax - 1);
            int numSymbols = BitReader.ReadFewBits(s, 2) + 1;
            for (int i = 0; i < numSymbols; ++i) {
                BitReader.FillBitWindow(s);
                int symbol = BitReader.ReadFewBits(s, maxBits);
                if (symbol >= alphabetSizeLimit) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_SYMBOL_OUT_OF_RANGE);
                }
                symbols[i] = symbol;
            }
            int result = CheckDupes(s, symbols, numSymbols);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int histogramId = numSymbols;
            if (numSymbols == 4) {
                histogramId += BitReader.ReadFewBits(s, 1);
            }
            switch (histogramId) {
                case 1: {
                    codeLengths[symbols[0]] = 1;
                    break;
                }

                case 2: {
                    codeLengths[symbols[0]] = 1;
                    codeLengths[symbols[1]] = 1;
                    break;
                }

                case 3: {
                    codeLengths[symbols[0]] = 1;
                    codeLengths[symbols[1]] = 2;
                    codeLengths[symbols[2]] = 2;
                    break;
                }

                case 4: {
                    // uniform 4-symbol histogram
                    codeLengths[symbols[0]] = 2;
                    codeLengths[symbols[1]] = 2;
                    codeLengths[symbols[2]] = 2;
                    codeLengths[symbols[3]] = 2;
                    break;
                }

                case 5: {
                    // prioritized 4-symbol histogram
                    codeLengths[symbols[0]] = 1;
                    codeLengths[symbols[1]] = 2;
                    codeLengths[symbols[2]] = 3;
                    codeLengths[symbols[3]] = 3;
                    break;
                }

                default: {
                    break;
                }
            }
            // TODO(eustas): Use specialized version?
            return Huffman.BuildHuffmanTable(tableGroup, tableIdx, HUFFMAN_TABLE_BITS, codeLengths, alphabetSizeLimit);
        }

        // Decode Huffman-coded code lengths.
        private static int ReadComplexHuffmanCode(int alphabetSizeLimit, int skip, int[] tableGroup, int tableIdx, 
            State s) {
            // TODO(eustas): Avoid allocation?
            int[] codeLengths = new int[alphabetSizeLimit];
            int[] codeLengthCodeLengths = new int[CODE_LENGTH_CODES];
            int space = 32;
            int numCodes = 0;
            for (int i = skip; i < CODE_LENGTH_CODES; ++i) {
                int codeLenIdx = CODE_LENGTH_CODE_ORDER[i];
                BitReader.FillBitWindow(s);
                int p = BitReader.PeekBits(s) & 15;
                // TODO(eustas): Demultiplex FIXED_TABLE.
                s.bitOffset += FIXED_TABLE[p] >> 16;
                int v = FIXED_TABLE[p] & 0xFFFF;
                codeLengthCodeLengths[codeLenIdx] = v;
                if (v != 0) {
                    space -= (32 >> v);
                    numCodes++;
                    if (space <= 0) {
                        break;
                    }
                }
            }
            if (space != 0 && numCodes != 1) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_CORRUPTED_HUFFMAN_CODE_HISTOGRAM);
            }
            int result = ReadHuffmanCodeLengths(codeLengthCodeLengths, alphabetSizeLimit, codeLengths, s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            return Huffman.BuildHuffmanTable(tableGroup, tableIdx, HUFFMAN_TABLE_BITS, codeLengths, alphabetSizeLimit);
        }

        /// <summary>Decodes Huffman table from bit-stream.</summary>
        /// <returns>number of slots used by resulting Huffman table</returns>
        private static int ReadHuffmanCode(int alphabetSizeMax, int alphabetSizeLimit, int[] tableGroup, int tableIdx
            , State s) {
            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                int result = BitReader.ReadMoreInput(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
            }
            BitReader.FillBitWindow(s);
            int simpleCodeOrSkip = BitReader.ReadFewBits(s, 2);
            if (simpleCodeOrSkip == 1) {
                return ReadSimpleHuffmanCode(alphabetSizeMax, alphabetSizeLimit, tableGroup, tableIdx, s);
            }
            return ReadComplexHuffmanCode(alphabetSizeLimit, simpleCodeOrSkip, tableGroup, tableIdx, s);
        }

        private static int DecodeContextMap(int contextMapSize, byte[] contextMap, State s) {
            int result;
            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                result = BitReader.ReadMoreInput(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
            }
            int numTrees = DecodeVarLenUnsignedByte(s) + 1;
            if (numTrees == 1) {
                Utils.FillBytesWithZeroes(contextMap, 0, contextMapSize);
                return numTrees;
            }
            BitReader.FillBitWindow(s);
            int useRleForZeros = BitReader.ReadFewBits(s, 1);
            int maxRunLengthPrefix = 0;
            if (useRleForZeros != 0) {
                maxRunLengthPrefix = BitReader.ReadFewBits(s, 4) + 1;
            }
            int alphabetSize = numTrees + maxRunLengthPrefix;
            int tableSize = MAX_HUFFMAN_TABLE_SIZE[(alphabetSize + 31) >> 5];
            /* Speculative single entry table group. */
            int[] table = new int[tableSize + 1];
            int tableIdx = table.Length - 1;
            result = ReadHuffmanCode(alphabetSize, alphabetSize, table, tableIdx, s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int i = 0;
            while (i < contextMapSize) {
                if (s.halfOffset > BitReader.HALF_WATERLINE) {
                    result = BitReader.ReadMoreInput(s);
                    if (result < BrotliError.BROTLI_OK) {
                        return result;
                    }
                }
                BitReader.FillBitWindow(s);
                int code = ReadSymbol(table, tableIdx, s);
                if (code == 0) {
                    contextMap[i] = 0;
                    i++;
                }
                else {
                    if (code <= maxRunLengthPrefix) {
                        BitReader.FillBitWindow(s);
                        int reps = (1 << code) + BitReader.ReadFewBits(s, code);
                        while (reps != 0) {
                            if (i >= contextMapSize) {
                                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_CORRUPTED_CONTEXT_MAP);
                            }
                            contextMap[i] = 0;
                            i++;
                            reps--;
                        }
                    }
                    else {
                        contextMap[i] = (byte)(code - maxRunLengthPrefix);
                        i++;
                    }
                }
            }
            BitReader.FillBitWindow(s);
            if (BitReader.ReadFewBits(s, 1) == 1) {
                InverseMoveToFrontTransform(contextMap, contextMapSize);
            }
            return numTrees;
        }

        private static int DecodeBlockTypeAndLength(State s, int treeType, int numBlockTypes) {
            int[] ringBuffers = s.rings;
            int offset = 4 + treeType * 2;
            BitReader.FillBitWindow(s);
            int blockType = ReadSymbol(s.blockTrees, 2 * treeType, s);
            int result = ReadBlockLength(s.blockTrees, 2 * treeType + 1, s);
            if (blockType == 1) {
                blockType = ringBuffers[offset + 1] + 1;
            }
            else {
                if (blockType == 0) {
                    blockType = ringBuffers[offset];
                }
                else {
                    blockType -= 2;
                }
            }
            if (blockType >= numBlockTypes) {
                blockType -= numBlockTypes;
            }
            ringBuffers[offset] = ringBuffers[offset + 1];
            ringBuffers[offset + 1] = blockType;
            return result;
        }

        private static void DecodeLiteralBlockSwitch(State s) {
            s.literalBlockLength = DecodeBlockTypeAndLength(s, 0, s.numLiteralBlockTypes);
            int literalBlockType = s.rings[5];
            s.contextMapSlice = literalBlockType << LITERAL_CONTEXT_BITS;
            s.literalTreeIdx = (int)s.contextMap[s.contextMapSlice] & 0xFF;
            int contextMode = (int)s.contextModes[literalBlockType];
            s.contextLookupOffset1 = contextMode << 9;
            s.contextLookupOffset2 = s.contextLookupOffset1 + 256;
        }

        private static void DecodeCommandBlockSwitch(State s) {
            s.commandBlockLength = DecodeBlockTypeAndLength(s, 1, s.numCommandBlockTypes);
            s.commandTreeIdx = s.rings[7];
        }

        private static void DecodeDistanceBlockSwitch(State s) {
            s.distanceBlockLength = DecodeBlockTypeAndLength(s, 2, s.numDistanceBlockTypes);
            s.distContextMapSlice = s.rings[9] << DISTANCE_CONTEXT_BITS;
        }

        private static void MaybeReallocateRingBuffer(State s) {
            int newSize = s.maxRingBufferSize;
            if (newSize > s.expectedTotalSize) {
                /* TODO(eustas): Handle 2GB+ cases more gracefully. */
                int minimalNewSize = s.expectedTotalSize;
                while ((newSize >> 1) > minimalNewSize) {
                    newSize = newSize >> 1;
                }
                if ((s.inputEnd == 0) && newSize < 16384 && s.maxRingBufferSize >= 16384) {
                    newSize = 16384;
                }
            }
            if (newSize <= s.ringBufferSize) {
                return;
            }
            int ringBufferSizeWithSlack = newSize + MAX_TRANSFORMED_WORD_LENGTH;
            byte[] newBuffer = new byte[ringBufferSizeWithSlack];
            byte[] oldBuffer = s.ringBuffer;
            if (oldBuffer.Length != 0) {
                Utils.CopyBytes(newBuffer, 0, oldBuffer, 0, s.ringBufferSize);
            }
            s.ringBuffer = newBuffer;
            s.ringBufferSize = newSize;
        }

        private static int ReadNextMetablockHeader(State s) {
            if (s.inputEnd != 0) {
                s.nextRunningState = FINISHED;
                s.runningState = INIT_WRITE;
                return BrotliError.BROTLI_OK;
            }
            // TODO(eustas): Reset? Do we need this?
            s.literalTreeGroup = new int[0];
            s.commandTreeGroup = new int[0];
            s.distanceTreeGroup = new int[0];
            int result;
            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                result = BitReader.ReadMoreInput(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
            }
            result = DecodeMetaBlockLength(s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            if ((s.metaBlockLength == 0) && (s.isMetadata == 0)) {
                return BrotliError.BROTLI_OK;
            }
            if ((s.isUncompressed != 0) || (s.isMetadata != 0)) {
                result = BitReader.JumpToByteBoundary(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
                if (s.isMetadata == 0) {
                    s.runningState = COPY_UNCOMPRESSED;
                }
                else {
                    s.runningState = READ_METADATA;
                }
            }
            else {
                s.runningState = COMPRESSED_BLOCK_START;
            }
            if (s.isMetadata != 0) {
                return BrotliError.BROTLI_OK;
            }
            s.expectedTotalSize += s.metaBlockLength;
            if (s.expectedTotalSize > 1 << 30) {
                s.expectedTotalSize = 1 << 30;
            }
            if (s.ringBufferSize < s.maxRingBufferSize) {
                MaybeReallocateRingBuffer(s);
            }
            return BrotliError.BROTLI_OK;
        }

        private static int ReadMetablockPartition(State s, int treeType, int numBlockTypes) {
            int offset = s.blockTrees[2 * treeType];
            if (numBlockTypes <= 1) {
                s.blockTrees[2 * treeType + 1] = offset;
                s.blockTrees[2 * treeType + 2] = offset;
                return 1 << 28;
            }
            int blockTypeAlphabetSize = numBlockTypes + 2;
            int result = ReadHuffmanCode(blockTypeAlphabetSize, blockTypeAlphabetSize, s.blockTrees, 2 * treeType, s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            offset += result;
            s.blockTrees[2 * treeType + 1] = offset;
            int blockLengthAlphabetSize = NUM_BLOCK_LENGTH_CODES;
            result = ReadHuffmanCode(blockLengthAlphabetSize, blockLengthAlphabetSize, s.blockTrees, 2 * treeType + 1, 
                s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            offset += result;
            s.blockTrees[2 * treeType + 2] = offset;
            return ReadBlockLength(s.blockTrees, 2 * treeType + 1, s);
        }

        private static void CalculateDistanceLut(State s, int alphabetSizeLimit) {
            byte[] distExtraBits = s.distExtraBits;
            int[] distOffset = s.distOffset;
            int npostfix = s.distancePostfixBits;
            int ndirect = s.numDirectDistanceCodes;
            int postfix = 1 << npostfix;
            int bits = 1;
            int half = 0;
            /* Skip short codes. */
            int i = NUM_DISTANCE_SHORT_CODES;
            /* Fill direct codes. */
            for (int j = 0; j < ndirect; ++j) {
                distExtraBits[i] = 0;
                distOffset[i] = j + 1;
                ++i;
            }
            /* Fill regular distance codes. */
            while (i < alphabetSizeLimit) {
                int @base = ndirect + ((((2 + half) << bits) - 4) << npostfix) + 1;
                /* Always fill the complete group. */
                for (int j = 0; j < postfix; ++j) {
                    distExtraBits[i] = (byte)bits;
                    distOffset[i] = @base + j;
                    ++i;
                }
                bits = bits + half;
                half = half ^ 1;
            }
        }

        private static int ReadMetablockHuffmanCodesAndContextMaps(State s) {
            s.numLiteralBlockTypes = DecodeVarLenUnsignedByte(s) + 1;
            int result = ReadMetablockPartition(s, 0, s.numLiteralBlockTypes);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.literalBlockLength = result;
            s.numCommandBlockTypes = DecodeVarLenUnsignedByte(s) + 1;
            result = ReadMetablockPartition(s, 1, s.numCommandBlockTypes);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.commandBlockLength = result;
            s.numDistanceBlockTypes = DecodeVarLenUnsignedByte(s) + 1;
            result = ReadMetablockPartition(s, 2, s.numDistanceBlockTypes);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.distanceBlockLength = result;
            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                result = BitReader.ReadMoreInput(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
            }
            BitReader.FillBitWindow(s);
            s.distancePostfixBits = BitReader.ReadFewBits(s, 2);
            s.numDirectDistanceCodes = BitReader.ReadFewBits(s, 4) << s.distancePostfixBits;
            // TODO(eustas): Reuse?
            s.contextModes = new byte[s.numLiteralBlockTypes];
            int i = 0;
            while (i < s.numLiteralBlockTypes) {
                /* Ensure that less than 256 bits read between readMoreInput. */
                int limit = Utils.Min(i + 96, s.numLiteralBlockTypes);
                while (i < limit) {
                    BitReader.FillBitWindow(s);
                    s.contextModes[i] = (byte)BitReader.ReadFewBits(s, 2);
                    i++;
                }
                if (s.halfOffset > BitReader.HALF_WATERLINE) {
                    result = BitReader.ReadMoreInput(s);
                    if (result < BrotliError.BROTLI_OK) {
                        return result;
                    }
                }
            }
            // TODO(eustas): Reuse?
            int contextMapLength = s.numLiteralBlockTypes << LITERAL_CONTEXT_BITS;
            s.contextMap = new byte[contextMapLength];
            result = DecodeContextMap(contextMapLength, s.contextMap, s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int numLiteralTrees = result;
            s.trivialLiteralContext = 1;
            for (int j = 0; j < contextMapLength; ++j) {
                if ((int)s.contextMap[j] != j >> LITERAL_CONTEXT_BITS) {
                    s.trivialLiteralContext = 0;
                    break;
                }
            }
            // TODO(eustas): Reuse?
            s.distContextMap = new byte[s.numDistanceBlockTypes << DISTANCE_CONTEXT_BITS];
            result = DecodeContextMap(s.numDistanceBlockTypes << DISTANCE_CONTEXT_BITS, s.distContextMap, s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int numDistTrees = result;
            s.literalTreeGroup = new int[HuffmanTreeGroupAllocSize(NUM_LITERAL_CODES, numLiteralTrees)];
            result = DecodeHuffmanTreeGroup(NUM_LITERAL_CODES, NUM_LITERAL_CODES, numLiteralTrees, s, s.literalTreeGroup
                );
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.commandTreeGroup = new int[HuffmanTreeGroupAllocSize(NUM_COMMAND_CODES, s.numCommandBlockTypes)];
            result = DecodeHuffmanTreeGroup(NUM_COMMAND_CODES, NUM_COMMAND_CODES, s.numCommandBlockTypes, s, s.commandTreeGroup
                );
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            int distanceAlphabetSizeMax = CalculateDistanceAlphabetSize(s.distancePostfixBits, s.numDirectDistanceCodes
                , MAX_DISTANCE_BITS);
            int distanceAlphabetSizeLimit = distanceAlphabetSizeMax;
            if (s.isLargeWindow == 1) {
                distanceAlphabetSizeMax = CalculateDistanceAlphabetSize(s.distancePostfixBits, s.numDirectDistanceCodes, MAX_LARGE_WINDOW_DISTANCE_BITS
                    );
                result = CalculateDistanceAlphabetLimit(s, MAX_ALLOWED_DISTANCE, s.distancePostfixBits, s.numDirectDistanceCodes
                    );
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
                distanceAlphabetSizeLimit = result;
            }
            s.distanceTreeGroup = new int[HuffmanTreeGroupAllocSize(distanceAlphabetSizeLimit, numDistTrees)];
            result = DecodeHuffmanTreeGroup(distanceAlphabetSizeMax, distanceAlphabetSizeLimit, numDistTrees, s, s.distanceTreeGroup
                );
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            CalculateDistanceLut(s, distanceAlphabetSizeLimit);
            s.contextMapSlice = 0;
            s.distContextMapSlice = 0;
            s.contextLookupOffset1 = (int)s.contextModes[0] * 512;
            s.contextLookupOffset2 = s.contextLookupOffset1 + 256;
            s.literalTreeIdx = 0;
            s.commandTreeIdx = 0;
            s.rings[4] = 1;
            s.rings[5] = 0;
            s.rings[6] = 1;
            s.rings[7] = 0;
            s.rings[8] = 1;
            s.rings[9] = 0;
            return BrotliError.BROTLI_OK;
        }

        private static int CopyUncompressedData(State s) {
            byte[] ringBuffer = s.ringBuffer;
            int result;
            // Could happen if block ends at ring buffer end.
            if (s.metaBlockLength <= 0) {
                result = BitReader.Reload(s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
                s.runningState = BLOCK_START;
                return BrotliError.BROTLI_OK;
            }
            int chunkLength = Utils.Min(s.ringBufferSize - s.pos, s.metaBlockLength);
            result = BitReader.CopyRawBytes(s, ringBuffer, s.pos, chunkLength);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.metaBlockLength -= chunkLength;
            s.pos += chunkLength;
            if (s.pos == s.ringBufferSize) {
                s.nextRunningState = COPY_UNCOMPRESSED;
                s.runningState = INIT_WRITE;
                return BrotliError.BROTLI_OK;
            }
            result = BitReader.Reload(s);
            if (result < BrotliError.BROTLI_OK) {
                return result;
            }
            s.runningState = BLOCK_START;
            return BrotliError.BROTLI_OK;
        }

        private static int WriteRingBuffer(State s) {
            int toWrite = Utils.Min(s.outputLength - s.outputUsed, s.ringBufferBytesReady - s.ringBufferBytesWritten);
            // TODO(eustas): DCHECK(toWrite >= 0)
            if (toWrite != 0) {
                Utils.CopyBytes(s.output, s.outputOffset + s.outputUsed, s.ringBuffer, s.ringBufferBytesWritten, s.ringBufferBytesWritten
                     + toWrite);
                s.outputUsed += toWrite;
                s.ringBufferBytesWritten += toWrite;
            }
            if (s.outputUsed < s.outputLength) {
                return BrotliError.BROTLI_OK;
            }
            return BrotliError.BROTLI_OK_NEED_MORE_OUTPUT;
        }

        private static int HuffmanTreeGroupAllocSize(int alphabetSizeLimit, int n) {
            int maxTableSize = MAX_HUFFMAN_TABLE_SIZE[(alphabetSizeLimit + 31) >> 5];
            return n + n * maxTableSize;
        }

        private static int DecodeHuffmanTreeGroup(int alphabetSizeMax, int alphabetSizeLimit, int n, State s, int[]
             group) {
            int next = n;
            for (int i = 0; i < n; ++i) {
                group[i] = next;
                int result = ReadHuffmanCode(alphabetSizeMax, alphabetSizeLimit, group, i, s);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
                next += result;
            }
            return BrotliError.BROTLI_OK;
        }

        // Returns offset in ringBuffer that should trigger WRITE when filled.
        private static int CalculateFence(State s) {
            int result = s.ringBufferSize;
            if (s.isEager != 0) {
                result = Utils.Min(result, s.ringBufferBytesWritten + s.outputLength - s.outputUsed);
            }
            return result;
        }

        private static int DoUseDictionary(State s, int fence) {
            if (s.distance > MAX_ALLOWED_DISTANCE) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
            }
            int address = s.distance - s.maxDistance - 1 - s.cdTotalSize;
            if (address < 0) {
                int result = InitializeCompoundDictionaryCopy(s, -address - 1, s.copyLength);
                if (result < BrotliError.BROTLI_OK) {
                    return result;
                }
                s.runningState = COPY_FROM_COMPOUND_DICTIONARY;
            }
            else {
                // Force lazy dictionary initialization.
                byte[] dictionaryData = Dictionary.GetData();
                int wordLength = s.copyLength;
                if (wordLength > Dictionary.MAX_DICTIONARY_WORD_LENGTH) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
                }
                int shift = Dictionary.sizeBits[wordLength];
                if (shift == 0) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
                }
                int offset = Dictionary.offsets[wordLength];
                int mask = (1 << shift) - 1;
                int wordIdx = address & mask;
                int transformIdx = address >> shift;
                offset += wordIdx * wordLength;
                Transform.Transforms transforms = Transform.RFC_TRANSFORMS;
                if (transformIdx >= transforms.numTransforms) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
                }
                int len = Transform.TransformDictionaryWord(s.ringBuffer, s.pos, dictionaryData, offset, wordLength, transforms
                    , transformIdx);
                s.pos += len;
                s.metaBlockLength -= len;
                if (s.pos >= fence) {
                    s.nextRunningState = MAIN_LOOP;
                    s.runningState = INIT_WRITE;
                    return BrotliError.BROTLI_OK;
                }
                s.runningState = MAIN_LOOP;
            }
            return BrotliError.BROTLI_OK;
        }

        private static void InitializeCompoundDictionary(State s) {
            s.cdBlockMap = new byte[1 << CD_BLOCK_MAP_BITS];
            int blockBits = CD_BLOCK_MAP_BITS;
            // If this function is executed, then s.cdTotalSize > 0.
            while (((s.cdTotalSize - 1) >> blockBits) != 0) {
                blockBits++;
            }
            blockBits -= CD_BLOCK_MAP_BITS;
            s.cdBlockBits = blockBits;
            int cursor = 0;
            int index = 0;
            while (cursor < s.cdTotalSize) {
                while (s.cdChunkOffsets[index + 1] < cursor) {
                    index++;
                }
                s.cdBlockMap[cursor >> blockBits] = (byte)index;
                cursor += 1 << blockBits;
            }
        }

        private static int InitializeCompoundDictionaryCopy(State s, int address, int length) {
            if (s.cdBlockBits == -1) {
                InitializeCompoundDictionary(s);
            }
            int index = (int)s.cdBlockMap[address >> s.cdBlockBits];
            while (address >= s.cdChunkOffsets[index + 1]) {
                index++;
            }
            if (s.cdTotalSize > address + length) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
            }
            /* Update the recent distances cache */
            s.distRbIdx = (s.distRbIdx + 1) & 0x3;
            s.rings[s.distRbIdx] = s.distance;
            s.metaBlockLength -= length;
            s.cdBrIndex = index;
            s.cdBrOffset = address - s.cdChunkOffsets[index];
            s.cdBrLength = length;
            s.cdBrCopied = 0;
            return BrotliError.BROTLI_OK;
        }

        private static int CopyFromCompoundDictionary(State s, int fence) {
            int pos = s.pos;
            int origPos = pos;
            while (s.cdBrLength != s.cdBrCopied) {
                int space = fence - pos;
                int chunkLength = s.cdChunkOffsets[s.cdBrIndex + 1] - s.cdChunkOffsets[s.cdBrIndex];
                int remChunkLength = chunkLength - s.cdBrOffset;
                int length = s.cdBrLength - s.cdBrCopied;
                if (length > remChunkLength) {
                    length = remChunkLength;
                }
                if (length > space) {
                    length = space;
                }
                Utils.CopyBytes(s.ringBuffer, pos, s.cdChunks[s.cdBrIndex], s.cdBrOffset, s.cdBrOffset + length);
                pos += length;
                s.cdBrOffset += length;
                s.cdBrCopied += length;
                if (length == remChunkLength) {
                    s.cdBrIndex++;
                    s.cdBrOffset = 0;
                }
                if (pos >= fence) {
                    break;
                }
            }
            return pos - origPos;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Actual decompress implementation.</summary>
        internal static int Decompress(State s) {
            int result;
            if (s.runningState == UNINITIALIZED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_STATE_NOT_INITIALIZED);
            }
            if (s.runningState < 0) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_UNEXPECTED_STATE);
            }
            if (s.runningState == CLOSED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_ALREADY_CLOSED);
            }
            if (s.runningState == INITIALIZED) {
                int windowBits = DecodeWindowBits(s);
                if (windowBits == -1) {
                    /* Reserved case for future expansion. */
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_WINDOW_BITS);
                }
                s.maxRingBufferSize = 1 << windowBits;
                s.maxBackwardDistance = s.maxRingBufferSize - 16;
                s.runningState = BLOCK_START;
            }
            int fence = CalculateFence(s);
            int ringBufferMask = s.ringBufferSize - 1;
            byte[] ringBuffer = s.ringBuffer;
            while (s.runningState != FINISHED) {
                // TODO(eustas): extract cases to methods for the better readability.
                switch (s.runningState) {
                    case BLOCK_START: {
                        if (s.metaBlockLength < 0) {
                            return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_METABLOCK_LENGTH);
                        }
                        result = ReadNextMetablockHeader(s);
                        if (result < BrotliError.BROTLI_OK) {
                            return result;
                        }
                        /* Ring-buffer would be reallocated here. */
                        fence = CalculateFence(s);
                        ringBufferMask = s.ringBufferSize - 1;
                        ringBuffer = s.ringBuffer;
                        continue;
                    }

                    case COMPRESSED_BLOCK_START: {
                        result = ReadMetablockHuffmanCodesAndContextMaps(s);
                        if (result < BrotliError.BROTLI_OK) {
                            return result;
                        }
                        s.runningState = MAIN_LOOP;
                        continue;
                    }

                    case MAIN_LOOP: {
                        if (s.metaBlockLength <= 0) {
                            s.runningState = BLOCK_START;
                            continue;
                        }
                        if (s.halfOffset > BitReader.HALF_WATERLINE) {
                            result = BitReader.ReadMoreInput(s);
                            if (result < BrotliError.BROTLI_OK) {
                                return result;
                            }
                        }
                        if (s.commandBlockLength == 0) {
                            DecodeCommandBlockSwitch(s);
                        }
                        s.commandBlockLength--;
                        BitReader.FillBitWindow(s);
                        int cmdCode = ReadSymbol(s.commandTreeGroup, s.commandTreeIdx, s) << 2;
                        int insertAndCopyExtraBits = (int)CMD_LOOKUP[cmdCode];
                        int insertLengthOffset = (int)CMD_LOOKUP[cmdCode + 1];
                        int copyLengthOffset = (int)CMD_LOOKUP[cmdCode + 2];
                        s.distanceCode = (int)CMD_LOOKUP[cmdCode + 3];
                        BitReader.FillBitWindow(s);
                        int insertLengthExtraBits = insertAndCopyExtraBits & 0xFF;
                        s.insertLength = insertLengthOffset + BitReader.ReadBits(s, insertLengthExtraBits);
                        BitReader.FillBitWindow(s);
                        int copyLengthExtraBits = insertAndCopyExtraBits >> 8;
                        s.copyLength = copyLengthOffset + BitReader.ReadBits(s, copyLengthExtraBits);
                        s.j = 0;
                        s.runningState = INSERT_LOOP;
                        continue;
                    }

                    case INSERT_LOOP: {
                        if (s.trivialLiteralContext != 0) {
                            while (s.j < s.insertLength) {
                                if (s.halfOffset > BitReader.HALF_WATERLINE) {
                                    result = BitReader.ReadMoreInput(s);
                                    if (result < BrotliError.BROTLI_OK) {
                                        return result;
                                    }
                                }
                                if (s.literalBlockLength == 0) {
                                    DecodeLiteralBlockSwitch(s);
                                }
                                s.literalBlockLength--;
                                BitReader.FillBitWindow(s);
                                ringBuffer[s.pos] = (byte)ReadSymbol(s.literalTreeGroup, s.literalTreeIdx, s);
                                s.pos++;
                                s.j++;
                                if (s.pos >= fence) {
                                    s.nextRunningState = INSERT_LOOP;
                                    s.runningState = INIT_WRITE;
                                    break;
                                }
                            }
                        }
                        else {
                            int prevByte1 = (int)ringBuffer[(s.pos - 1) & ringBufferMask] & 0xFF;
                            int prevByte2 = (int)ringBuffer[(s.pos - 2) & ringBufferMask] & 0xFF;
                            while (s.j < s.insertLength) {
                                if (s.halfOffset > BitReader.HALF_WATERLINE) {
                                    result = BitReader.ReadMoreInput(s);
                                    if (result < BrotliError.BROTLI_OK) {
                                        return result;
                                    }
                                }
                                if (s.literalBlockLength == 0) {
                                    DecodeLiteralBlockSwitch(s);
                                }
                                int literalContext = Context.LOOKUP[s.contextLookupOffset1 + prevByte1] | Context.LOOKUP[s.contextLookupOffset2
                                     + prevByte2];
                                int literalTreeIdx = (int)s.contextMap[s.contextMapSlice + literalContext] & 0xFF;
                                s.literalBlockLength--;
                                prevByte2 = prevByte1;
                                BitReader.FillBitWindow(s);
                                prevByte1 = ReadSymbol(s.literalTreeGroup, literalTreeIdx, s);
                                ringBuffer[s.pos] = (byte)prevByte1;
                                s.pos++;
                                s.j++;
                                if (s.pos >= fence) {
                                    s.nextRunningState = INSERT_LOOP;
                                    s.runningState = INIT_WRITE;
                                    break;
                                }
                            }
                        }
                        if (s.runningState != INSERT_LOOP) {
                            continue;
                        }
                        s.metaBlockLength -= s.insertLength;
                        if (s.metaBlockLength <= 0) {
                            s.runningState = MAIN_LOOP;
                            continue;
                        }
                        int distanceCode = s.distanceCode;
                        if (distanceCode < 0) {
                            // distanceCode in untouched; assigning it 0 won't affect distance ring buffer rolling.
                            s.distance = s.rings[s.distRbIdx];
                        }
                        else {
                            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                                result = BitReader.ReadMoreInput(s);
                                if (result < BrotliError.BROTLI_OK) {
                                    return result;
                                }
                            }
                            if (s.distanceBlockLength == 0) {
                                DecodeDistanceBlockSwitch(s);
                            }
                            s.distanceBlockLength--;
                            BitReader.FillBitWindow(s);
                            int distTreeIdx = (int)s.distContextMap[s.distContextMapSlice + distanceCode] & 0xFF;
                            distanceCode = ReadSymbol(s.distanceTreeGroup, distTreeIdx, s);
                            if (distanceCode < NUM_DISTANCE_SHORT_CODES) {
                                int index = (s.distRbIdx + DISTANCE_SHORT_CODE_INDEX_OFFSET[distanceCode]) & 0x3;
                                s.distance = s.rings[index] + DISTANCE_SHORT_CODE_VALUE_OFFSET[distanceCode];
                                if (s.distance < 0) {
                                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_NEGATIVE_DISTANCE);
                                }
                            }
                            else {
                                int extraBits = (int)s.distExtraBits[distanceCode];
                                int bits;
                                if (s.bitOffset + extraBits <= BitReader.BITNESS) {
                                    bits = BitReader.ReadFewBits(s, extraBits);
                                }
                                else {
                                    BitReader.FillBitWindow(s);
                                    bits = BitReader.ReadBits(s, extraBits);
                                }
                                s.distance = s.distOffset[distanceCode] + (bits << s.distancePostfixBits);
                            }
                        }
                        if (s.maxDistance != s.maxBackwardDistance && s.pos < s.maxBackwardDistance) {
                            s.maxDistance = s.pos;
                        }
                        else {
                            s.maxDistance = s.maxBackwardDistance;
                        }
                        if (s.distance > s.maxDistance) {
                            s.runningState = USE_DICTIONARY;
                            continue;
                        }
                        if (distanceCode > 0) {
                            s.distRbIdx = (s.distRbIdx + 1) & 0x3;
                            s.rings[s.distRbIdx] = s.distance;
                        }
                        if (s.copyLength > s.metaBlockLength) {
                            return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_BACKWARD_REFERENCE);
                        }
                        s.j = 0;
                        s.runningState = COPY_LOOP;
                        continue;
                    }

                    case COPY_LOOP: {
                        int src = (s.pos - s.distance) & ringBufferMask;
                        int dst = s.pos;
                        int copyLength = s.copyLength - s.j;
                        int srcEnd = src + copyLength;
                        int dstEnd = dst + copyLength;
                        if ((srcEnd < ringBufferMask) && (dstEnd < ringBufferMask)) {
                            if (copyLength < 12 || (srcEnd > dst && dstEnd > src)) {
                                int numQuads = (copyLength + 3) >> 2;
                                for (int k = 0; k < numQuads; ++k) {
                                    ringBuffer[dst++] = ringBuffer[src++];
                                    ringBuffer[dst++] = ringBuffer[src++];
                                    ringBuffer[dst++] = ringBuffer[src++];
                                    ringBuffer[dst++] = ringBuffer[src++];
                                }
                            }
                            else {
                                Utils.CopyBytesWithin(ringBuffer, dst, src, srcEnd);
                            }
                            s.j += copyLength;
                            s.metaBlockLength -= copyLength;
                            s.pos += copyLength;
                        }
                        else {
                            while (s.j < s.copyLength) {
                                ringBuffer[s.pos] = ringBuffer[(s.pos - s.distance) & ringBufferMask];
                                s.metaBlockLength--;
                                s.pos++;
                                s.j++;
                                if (s.pos >= fence) {
                                    s.nextRunningState = COPY_LOOP;
                                    s.runningState = INIT_WRITE;
                                    break;
                                }
                            }
                        }
                        if (s.runningState == COPY_LOOP) {
                            s.runningState = MAIN_LOOP;
                        }
                        continue;
                    }

                    case USE_DICTIONARY: {
                        result = DoUseDictionary(s, fence);
                        if (result < BrotliError.BROTLI_OK) {
                            return result;
                        }
                        continue;
                    }

                    case COPY_FROM_COMPOUND_DICTIONARY: {
                        s.pos += CopyFromCompoundDictionary(s, fence);
                        if (s.pos >= fence) {
                            s.nextRunningState = COPY_FROM_COMPOUND_DICTIONARY;
                            s.runningState = INIT_WRITE;
                            return BrotliError.BROTLI_OK_NEED_MORE_OUTPUT;
                        }
                        s.runningState = MAIN_LOOP;
                        continue;
                    }

                    case READ_METADATA: {
                        while (s.metaBlockLength > 0) {
                            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                                result = BitReader.ReadMoreInput(s);
                                if (result < BrotliError.BROTLI_OK) {
                                    return result;
                                }
                            }
                            // Optimize
                            BitReader.FillBitWindow(s);
                            BitReader.ReadFewBits(s, 8);
                            s.metaBlockLength--;
                        }
                        s.runningState = BLOCK_START;
                        continue;
                    }

                    case COPY_UNCOMPRESSED: {
                        result = CopyUncompressedData(s);
                        if (result < BrotliError.BROTLI_OK) {
                            return result;
                        }
                        continue;
                    }

                    case INIT_WRITE: {
                        s.ringBufferBytesReady = Utils.Min(s.pos, s.ringBufferSize);
                        s.runningState = WRITE;
                        continue;
                    }

                    case WRITE: {
                        result = WriteRingBuffer(s);
                        if (result != BrotliError.BROTLI_OK) {
                            // Output buffer is full.
                            return result;
                        }
                        if (s.pos >= s.maxBackwardDistance) {
                            s.maxDistance = s.maxBackwardDistance;
                        }
                        // Wrap the ringBuffer.
                        if (s.pos >= s.ringBufferSize) {
                            if (s.pos > s.ringBufferSize) {
                                Utils.CopyBytesWithin(ringBuffer, 0, s.ringBufferSize, s.pos);
                            }
                            s.pos = s.pos & ringBufferMask;
                            s.ringBufferBytesWritten = 0;
                        }
                        s.runningState = s.nextRunningState;
                        continue;
                    }

                    default: {
                        return Utils.MakeError(s, BrotliError.BROTLI_PANIC_UNEXPECTED_STATE);
                    }
                }
            }
            if (s.runningState != FINISHED) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_UNREACHABLE);
            }
            if (s.metaBlockLength < 0) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_INVALID_METABLOCK_LENGTH);
            }
            result = BitReader.JumpToByteBoundary(s);
            if (result != BrotliError.BROTLI_OK) {
                return result;
            }
            result = BitReader.CheckHealth(s, 1);
            if (result != BrotliError.BROTLI_OK) {
                return result;
            }
            return BrotliError.BROTLI_OK_DONE;
        }
//\endcond
    }
//\endcond
}
