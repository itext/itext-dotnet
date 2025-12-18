/* Copyright 2025 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>Possible errors from decoder.</summary>
    public sealed class BrotliError {
        /// <summary>Success; anything greater is also success.</summary>
        public const int BROTLI_OK = 0;

        /// <summary>Success; decoder has finished decompressing the input.</summary>
        public const int BROTLI_OK_DONE = BROTLI_OK + 1;

        /// <summary>Success; decoder has more output to produce.</summary>
        public const int BROTLI_OK_NEED_MORE_OUTPUT = BROTLI_OK + 2;

        /// <summary>Error code threshold; actual error codes are LESS than -1!</summary>
        public const int BROTLI_ERROR = -1;

        /// <summary>Stream error: corrupted code length table.</summary>
        public const int BROTLI_ERROR_CORRUPTED_CODE_LENGTH_TABLE = BROTLI_ERROR - 1;

        /// <summary>Stream error: corrupted context map.</summary>
        public const int BROTLI_ERROR_CORRUPTED_CONTEXT_MAP = BROTLI_ERROR - 2;

        /// <summary>Stream error: corrupted Huffman code histogram.</summary>
        public const int BROTLI_ERROR_CORRUPTED_HUFFMAN_CODE_HISTOGRAM = BROTLI_ERROR - 3;

        /// <summary>Stream error: corrupted padding bits.</summary>
        public const int BROTLI_ERROR_CORRUPTED_PADDING_BITS = BROTLI_ERROR - 4;

        /// <summary>Stream error: corrupted reserved bit.</summary>
        public const int BROTLI_ERROR_CORRUPTED_RESERVED_BIT = BROTLI_ERROR - 5;

        /// <summary>Stream error: duplicate simple Huffman symbol.</summary>
        public const int BROTLI_ERROR_DUPLICATE_SIMPLE_HUFFMAN_SYMBOL = BROTLI_ERROR - 6;

        /// <summary>Stream error: exuberant nibble.</summary>
        public const int BROTLI_ERROR_EXUBERANT_NIBBLE = BROTLI_ERROR - 7;

        /// <summary>Stream error: invalid backward reference.</summary>
        public const int BROTLI_ERROR_INVALID_BACKWARD_REFERENCE = BROTLI_ERROR - 8;

        /// <summary>Stream error: invalid metablock length.</summary>
        public const int BROTLI_ERROR_INVALID_METABLOCK_LENGTH = BROTLI_ERROR - 9;

        /// <summary>Stream error: invalid window bits.</summary>
        public const int BROTLI_ERROR_INVALID_WINDOW_BITS = BROTLI_ERROR - 10;

        /// <summary>Stream error: negative distance.</summary>
        public const int BROTLI_ERROR_NEGATIVE_DISTANCE = BROTLI_ERROR - 11;

        /// <summary>Stream error: read after end of input buffer.</summary>
        public const int BROTLI_ERROR_READ_AFTER_END = BROTLI_ERROR - 12;

        /// <summary>IO error: read failed.</summary>
        public const int BROTLI_ERROR_READ_FAILED = BROTLI_ERROR - 13;

        /// <summary>IO error: symbol out of range.</summary>
        public const int BROTLI_ERROR_SYMBOL_OUT_OF_RANGE = BROTLI_ERROR - 14;

        /// <summary>Stream error: truncated input.</summary>
        public const int BROTLI_ERROR_TRUNCATED_INPUT = BROTLI_ERROR - 15;

        /// <summary>Stream error: unused bytes after end of stream.</summary>
        public const int BROTLI_ERROR_UNUSED_BYTES_AFTER_END = BROTLI_ERROR - 16;

        /// <summary>Stream error: unused Huffman space.</summary>
        public const int BROTLI_ERROR_UNUSED_HUFFMAN_SPACE = BROTLI_ERROR - 17;

        /// <summary>Exception code threshold.</summary>
        public const int BROTLI_PANIC = -21;

        /// <summary>Exception: stream is already closed.</summary>
        public const int BROTLI_PANIC_ALREADY_CLOSED = BROTLI_PANIC - 1;

        /// <summary>Exception: max distance is too small.</summary>
        public const int BROTLI_PANIC_MAX_DISTANCE_TOO_SMALL = BROTLI_PANIC - 2;

        /// <summary>Exception: state is not fresh.</summary>
        public const int BROTLI_PANIC_STATE_NOT_FRESH = BROTLI_PANIC - 3;

        /// <summary>Exception: state is not initialized.</summary>
        public const int BROTLI_PANIC_STATE_NOT_INITIALIZED = BROTLI_PANIC - 4;

        /// <summary>Exception: state is not uninitialized.</summary>
        public const int BROTLI_PANIC_STATE_NOT_UNINITIALIZED = BROTLI_PANIC - 5;

        /// <summary>Exception: too many dictionary chunks.</summary>
        public const int BROTLI_PANIC_TOO_MANY_DICTIONARY_CHUNKS = BROTLI_PANIC - 6;

        /// <summary>Exception: unexpected state.</summary>
        public const int BROTLI_PANIC_UNEXPECTED_STATE = BROTLI_PANIC - 7;

        /// <summary>Exception: unreachable code.</summary>
        public const int BROTLI_PANIC_UNREACHABLE = BROTLI_PANIC - 8;

        /// <summary>Exception: unaligned copy bytes.</summary>
        public const int BROTLI_PANIC_UNALIGNED_COPY_BYTES = BROTLI_PANIC - 9;

        /// <summary>Non-instantiable.</summary>
        private BrotliError() {
        }
    }
}
