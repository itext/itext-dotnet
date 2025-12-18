/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;

namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>Bit reading helpers.</summary>
    internal sealed class BitReader {
        // Possible values: {5, 6}.  5 corresponds to 32-bit build, 6 to 64-bit. This value is used for
        // JIT conditional compilation.
        private static readonly int LOG_BITNESS = Utils.GetLogBintness();

        // Not only Java compiler prunes "if (const false)" code, but JVM as well.
        // Code under "if (BIT_READER_DEBUG != 0)" have zero performance impact (outside unit tests).
        private static readonly int BIT_READER_DEBUG = Utils.IsDebugMode();

//\cond DO_NOT_DOCUMENT
        internal static readonly int BITNESS = 1 << LOG_BITNESS;
//\endcond

        private static readonly int BYTENESS = BITNESS / 8;

        private const int CAPACITY = 4096;

        // After encountering the end of the input stream, this amount of zero bytes will be appended.
        private const int SLACK = 64;

        private const int BUFFER_SIZE = CAPACITY + SLACK;

        // Don't bother to replenish the buffer while this number of bytes is available.
        private const int SAFEGUARD = 36;

        private const int WATERLINE = CAPACITY - SAFEGUARD;

        // "Half" refers to "half of native integer type", i.e. on 64-bit machines it is 32-bit type,
        // on 32-bit machines it is 16-bit.
        private static readonly int HALF_BITNESS = BITNESS / 2;

        private static readonly int HALF_SIZE = BYTENESS / 2;

        private static readonly int HALVES_CAPACITY = CAPACITY / HALF_SIZE;

        private static readonly int HALF_BUFFER_SIZE = BUFFER_SIZE / HALF_SIZE;

        private static readonly int LOG_HALF_SIZE = LOG_BITNESS - 4;

//\cond DO_NOT_DOCUMENT
        internal static readonly int HALF_WATERLINE = WATERLINE / HALF_SIZE;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Fills up the input buffer.</summary>
        /// <remarks>
        /// Fills up the input buffer.
        /// <para /> Should not be called if there are at least 36 bytes present after current position.
        /// <para /> After encountering the end of the input stream, 64 additional zero bytes are copied to the
        /// buffer.
        /// </remarks>
        internal static int ReadMoreInput(State s) {
            if (s.endOfStreamReached != 0) {
                if (HalfAvailable(s) >= -2) {
                    return BrotliError.BROTLI_OK;
                }
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_TRUNCATED_INPUT);
            }
            int readOffset = s.halfOffset << LOG_HALF_SIZE;
            int bytesInBuffer = CAPACITY - readOffset;
            // Move unused bytes to the head of the buffer.
            Utils.CopyBytesWithin(s.byteBuffer, 0, readOffset, CAPACITY);
            s.halfOffset = 0;
            while (bytesInBuffer < CAPACITY) {
                int spaceLeft = CAPACITY - bytesInBuffer;
                int len = Utils.ReadInput(s, s.byteBuffer, bytesInBuffer, spaceLeft);
                if (len < BrotliError.BROTLI_ERROR) {
                    return len;
                }
                // EOF is -1 in Java, but 0 in C#.
                if (len <= 0) {
                    s.endOfStreamReached = 1;
                    s.tailBytes = bytesInBuffer;
                    bytesInBuffer += HALF_SIZE - 1;
                    break;
                }
                bytesInBuffer += len;
            }
            BytesToNibbles(s, bytesInBuffer);
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int CheckHealth(State s, int endOfStream) {
            if (s.endOfStreamReached == 0) {
                return BrotliError.BROTLI_OK;
            }
            int byteOffset = (s.halfOffset << LOG_HALF_SIZE) + ((s.bitOffset + 7) >> 3) - BYTENESS;
            if (byteOffset > s.tailBytes) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_READ_AFTER_END);
            }
            if ((endOfStream != 0) && (byteOffset != s.tailBytes)) {
                return Utils.MakeError(s, BrotliError.BROTLI_ERROR_UNUSED_BYTES_AFTER_END);
            }
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void AssertAccumulatorHealthy(State s) {
            if (s.bitOffset > BITNESS) {
                throw new InvalidOperationException("Accumulator underloaded: " + s.bitOffset);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void FillBitWindow(State s) {
            if (BIT_READER_DEBUG != 0) {
                AssertAccumulatorHealthy(s);
            }
            if (s.bitOffset >= HALF_BITNESS) {
                // Same as doFillBitWindow. JVM fails to inline it.
                if (BITNESS == 64) {
                    s.accumulator64 = ((long)s.intBuffer[s.halfOffset++] << HALF_BITNESS) | Utils.Shr64(s.accumulator64, HALF_BITNESS
                        );
                }
                else {
                    s.accumulator32 = ((int)s.shortBuffer[s.halfOffset++] << HALF_BITNESS) | Utils.Shr32(s.accumulator32, HALF_BITNESS
                        );
                }
                s.bitOffset -= HALF_BITNESS;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DoFillBitWindow(State s) {
            if (BIT_READER_DEBUG != 0) {
                AssertAccumulatorHealthy(s);
            }
            if (BITNESS == 64) {
                s.accumulator64 = ((long)s.intBuffer[s.halfOffset++] << HALF_BITNESS) | Utils.Shr64(s.accumulator64, HALF_BITNESS
                    );
            }
            else {
                s.accumulator32 = ((int)s.shortBuffer[s.halfOffset++] << HALF_BITNESS) | Utils.Shr32(s.accumulator32, HALF_BITNESS
                    );
            }
            s.bitOffset -= HALF_BITNESS;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int PeekBits(State s) {
            if (BITNESS == 64) {
                return (int)Utils.Shr64(s.accumulator64, s.bitOffset);
            }
            else {
                return Utils.Shr32(s.accumulator32, s.bitOffset);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Fetches bits from accumulator.</summary>
        /// <remarks>
        /// Fetches bits from accumulator.
        /// WARNING: accumulator MUST contain at least the specified amount of bits,
        /// otherwise BitReader will become broken.
        /// </remarks>
        internal static int ReadFewBits(State s, int n) {
            int v = PeekBits(s) & ((1 << n) - 1);
            s.bitOffset += n;
            return v;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int ReadBits(State s, int n) {
            if (HALF_BITNESS >= 24) {
                return ReadFewBits(s, n);
            }
            else {
                return (n <= 16) ? ReadFewBits(s, n) : ReadManyBits(s, n);
            }
        }
//\endcond

        private static int ReadManyBits(State s, int n) {
            int low = ReadFewBits(s, 16);
            DoFillBitWindow(s);
            return low | (ReadFewBits(s, n - 16) << 16);
        }

//\cond DO_NOT_DOCUMENT
        internal static int InitBitReader(State s) {
            s.byteBuffer = new byte[BUFFER_SIZE];
            if (BITNESS == 64) {
                s.accumulator64 = 0;
                s.intBuffer = new int[HALF_BUFFER_SIZE];
            }
            else {
                s.accumulator32 = 0;
                s.shortBuffer = new short[HALF_BUFFER_SIZE];
            }
            s.bitOffset = BITNESS;
            s.halfOffset = HALVES_CAPACITY;
            s.endOfStreamReached = 0;
            return Prepare(s);
        }
//\endcond

        private static int Prepare(State s) {
            if (s.halfOffset > BitReader.HALF_WATERLINE) {
                int result = ReadMoreInput(s);
                if (result != BrotliError.BROTLI_OK) {
                    return result;
                }
            }
            int health = CheckHealth(s, 0);
            if (health != BrotliError.BROTLI_OK) {
                return health;
            }
            DoFillBitWindow(s);
            DoFillBitWindow(s);
            return BrotliError.BROTLI_OK;
        }

//\cond DO_NOT_DOCUMENT
        internal static int Reload(State s) {
            if (s.bitOffset == BITNESS) {
                return Prepare(s);
            }
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int JumpToByteBoundary(State s) {
            int padding = (BITNESS - s.bitOffset) & 7;
            if (padding != 0) {
                int paddingBits = ReadFewBits(s, padding);
                if (paddingBits != 0) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_CORRUPTED_PADDING_BITS);
                }
            }
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int HalfAvailable(State s) {
            int limit = HALVES_CAPACITY;
            if (s.endOfStreamReached != 0) {
                limit = (s.tailBytes + (HALF_SIZE - 1)) >> LOG_HALF_SIZE;
            }
            return limit - s.halfOffset;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int CopyRawBytes(State s, byte[] data, int offset, int length) {
            int pos = offset;
            int len = length;
            if ((s.bitOffset & 7) != 0) {
                return Utils.MakeError(s, BrotliError.BROTLI_PANIC_UNALIGNED_COPY_BYTES);
            }
            // Drain accumulator.
            while ((s.bitOffset != BITNESS) && (len != 0)) {
                data[pos++] = (byte)PeekBits(s);
                s.bitOffset += 8;
                len--;
            }
            if (len == 0) {
                return BrotliError.BROTLI_OK;
            }
            // Get data from shadow buffer with "sizeof(int)" granularity.
            int copyNibbles = Utils.Min(HalfAvailable(s), len >> LOG_HALF_SIZE);
            if (copyNibbles > 0) {
                int readOffset = s.halfOffset << LOG_HALF_SIZE;
                int delta = copyNibbles << LOG_HALF_SIZE;
                Utils.CopyBytes(data, pos, s.byteBuffer, readOffset, readOffset + delta);
                pos += delta;
                len -= delta;
                s.halfOffset += copyNibbles;
            }
            if (len == 0) {
                return BrotliError.BROTLI_OK;
            }
            // Read tail bytes.
            if (HalfAvailable(s) > 0) {
                // length = 1..3
                FillBitWindow(s);
                while (len != 0) {
                    data[pos++] = (byte)PeekBits(s);
                    s.bitOffset += 8;
                    len--;
                }
                return CheckHealth(s, 0);
            }
            // Now it is possible to copy bytes directly.
            while (len > 0) {
                int chunkLen = Utils.ReadInput(s, data, pos, len);
                if (chunkLen < BrotliError.BROTLI_ERROR) {
                    return chunkLen;
                }
                // EOF is -1 in Java, but 0 in C#.
                if (chunkLen <= 0) {
                    return Utils.MakeError(s, BrotliError.BROTLI_ERROR_TRUNCATED_INPUT);
                }
                pos += chunkLen;
                len -= chunkLen;
            }
            return BrotliError.BROTLI_OK;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Translates bytes to halves (int/short).</summary>
        internal static void BytesToNibbles(State s, int byteLen) {
            byte[] byteBuffer = s.byteBuffer;
            int halfLen = byteLen >> LOG_HALF_SIZE;
            if (BITNESS == 64) {
                int[] intBuffer = s.intBuffer;
                for (int i = 0; i < halfLen; ++i) {
                    intBuffer[i] = ((int)byteBuffer[i * 4] & 0xFF) | (((int)byteBuffer[(i * 4) + 1] & 0xFF) << 8) | (((int)byteBuffer
                        [(i * 4) + 2] & 0xFF) << 16) | (((int)byteBuffer[(i * 4) + 3] & 0xFF) << 24);
                }
            }
            else {
                short[] shortBuffer = s.shortBuffer;
                for (int i = 0; i < halfLen; ++i) {
                    shortBuffer[i] = (short)(((int)byteBuffer[i * 2] & 0xFF) | (((int)byteBuffer[(i * 2) + 1] & 0xFF) << 8));
                }
            }
        }
//\endcond
    }
//\endcond
}
