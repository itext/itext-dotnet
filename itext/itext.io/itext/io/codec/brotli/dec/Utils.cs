/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.IO;

namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>A set of utility methods.</summary>
    internal sealed class Utils {
        private static readonly byte[] BYTE_ZEROES = new byte[1024];

        private static readonly int[] INT_ZEROES = new int[1024];

//\cond DO_NOT_DOCUMENT
        /// <summary>Fills byte array with zeroes.</summary>
        /// <remarks>
        /// Fills byte array with zeroes.
        /// <para /> Current implementation uses
        /// <see cref="System.Array.Copy(System.Object, int, System.Object, int, int)"/>
        /// , so it should be used for length not
        /// less than 16.
        /// </remarks>
        /// <param name="dest">array to fill with zeroes</param>
        /// <param name="start">the first item to fill</param>
        /// <param name="end">the last item to fill (exclusive)</param>
        internal static void FillBytesWithZeroes(byte[] dest, int start, int end) {
            int cursor = start;
            while (cursor < end) {
                int step = Math.Min(cursor + 1024, end) - cursor;
                Array.Copy(BYTE_ZEROES, 0, dest, cursor, step);
                cursor += step;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Fills int array with zeroes.</summary>
        /// <remarks>
        /// Fills int array with zeroes.
        /// <para /> Current implementation uses
        /// <see cref="System.Array.Copy(System.Object, int, System.Object, int, int)"/>
        /// , so it should be used for length not
        /// less than 16.
        /// </remarks>
        /// <param name="dest">array to fill with zeroes</param>
        /// <param name="start">the first item to fill</param>
        /// <param name="end">the last item to fill (exclusive)</param>
        internal static void FillIntsWithZeroes(int[] dest, int start, int end) {
            int cursor = start;
            while (cursor < end) {
                int step = Math.Min(cursor + 1024, end) - cursor;
                Array.Copy(INT_ZEROES, 0, dest, cursor, step);
                cursor += step;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void CopyBytes(byte[] dst, int target, byte[] src, int start, int end) {
            Array.Copy(src, start, dst, target, end - start);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void CopyBytesWithin(byte[] bytes, int target, int start, int end) {
            Array.Copy(bytes, start, bytes, target, end - start);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int ReadInput(State s, byte[] dst, int offset, int length) {
            try {
                return s.input.JRead(dst, offset, length);
            }
            catch (System.IO.IOException) {
                return MakeError(s, BrotliError.BROTLI_ERROR_READ_FAILED);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static Stream MakeEmptyInput() {
            return new MemoryStream(new byte[0]);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void CloseInput(State s) {
            s.input.Dispose();
            s.input = MakeEmptyInput();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static byte[] ToUsAsciiBytes(String src) {
            try {
                // NB: String#getBytes(String) is present in JDK 1.1, while other variants require JDK 1.6 and
                // above.
                return src.GetBytes("US-ASCII");
            }
            catch (ArgumentException e) {
                throw new Exception(e.Message, e);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // cannot happen
        internal static int[] ToUtf8Runes(String src) {
            int[] result = new int[src.Length];
            for (int i = 0; i < src.Length; i++) {
                result[i] = (int)src[i];
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int IsDebugMode() {
            string enableAsserts = Environment.GetEnvironmentVariable("BROTLI_ENABLE_ASSERTS");
            bool assertsEnabled = enableAsserts == null ? false : bool.Parse(enableAsserts);
            return assertsEnabled ? 1 : 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // See BitReader.LOG_BITNESS
        internal static int GetLogBintness() {
            string longExpensive = Environment.GetEnvironmentVariable("BROTLI_32_BIT_CPU");
            bool isLongExpensive = longExpensive == null ? false : bool.Parse(longExpensive);
            return isLongExpensive ? 5 : 6;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int Shr32(int x, int y) {
            return (int)(((uint)x) >> y);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static long Shr64(long x, int y) {
            return (long)(((ulong)x) >> y);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int Min(int a, int b) {
            return Math.Min(a, b);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int MakeError(State s, int code) {
            if (code >= BrotliError.BROTLI_OK) {
                return code;
            }
            if (s.runningState >= 0) {
                s.runningState = code;
            }
            // Only the first error is remembered.
            // TODO(eustas): expand codes to messages, if ever necessary.
            if (code <= BrotliError.BROTLI_PANIC) {
                throw new InvalidOperationException("Brotli error code: " + code);
            }
            throw new BrotliRuntimeException("Error code: " + code);
        }
//\endcond
    }
//\endcond
}
