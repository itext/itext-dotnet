/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;

namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    /// <summary>Unchecked exception used internally.</summary>
    internal class BrotliRuntimeException : Exception {
//\cond DO_NOT_DOCUMENT
        internal BrotliRuntimeException(String message)
            : base(message) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal BrotliRuntimeException(String message, Exception cause)
            : base(message, cause) {
        }
//\endcond
    }
//\endcond
}
