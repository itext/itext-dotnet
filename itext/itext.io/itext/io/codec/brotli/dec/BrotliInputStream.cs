/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System;
using System.IO;

namespace iText.IO.Codec.Brotli.Dec {
    /// <summary>
    /// <see cref="System.IO.Stream"/>
    /// decorator that decompresses brotli data.
    /// </summary>
    /// <remarks>
    /// <see cref="System.IO.Stream"/>
    /// decorator that decompresses brotli data.
    /// <para /> Not thread-safe.
    /// </remarks>
    public class BrotliInputStream : Stream {
        /// <summary>Default size of internal buffer (used for faster byte-by-byte reading).</summary>
        public const int DEFAULT_INTERNAL_BUFFER_SIZE = 256;

        /// <summary>Value expected by InputStream contract when stream is over.</summary>
        /// <remarks>
        /// Value expected by InputStream contract when stream is over.
        /// In Java it is -1.
        /// In C# it is 0 (should be patched during transpilation).
        /// </remarks>
        private const int END_OF_STREAM_MARKER = -1;

        /// <summary>Internal buffer used for efficient byte-by-byte reading.</summary>
        private byte[] buffer;

        /// <summary>Number of decoded but still unused bytes in internal buffer.</summary>
        private int remainingBufferBytes;

        /// <summary>Next unused byte offset.</summary>
        private int bufferOffset;

        /// <summary>Decoder state.</summary>
        private readonly State state = new State();

        /// <summary>
        /// Creates a
        /// <see cref="System.IO.Stream"/>
        /// wrapper that decompresses brotli data.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="System.IO.Stream"/>
        /// wrapper that decompresses brotli data.
        /// <para /> For byte-by-byte reading (
        /// <see cref="ReadByte()"/>
        /// ) internal buffer with
        /// <see cref="DEFAULT_INTERNAL_BUFFER_SIZE"/>
        /// size is allocated and used.
        /// <para /> Will block the thread until first
        /// <see cref="BitReader#CAPACITY"/>
        /// bytes of data of source
        /// are available.
        /// </remarks>
        /// <param name="source">underlying data source</param>
        public BrotliInputStream(Stream source)
            : this(source, DEFAULT_INTERNAL_BUFFER_SIZE) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="System.IO.Stream"/>
        /// wrapper that decompresses brotli data.
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="System.IO.Stream"/>
        /// wrapper that decompresses brotli data.
        /// <para /> For byte-by-byte reading (
        /// <see cref="ReadByte()"/>
        /// ) internal buffer of specified size is
        /// allocated and used.
        /// <para /> Will block the thread until first
        /// <see cref="BitReader#CAPACITY"/>
        /// bytes of data of source
        /// are available.
        /// </remarks>
        /// <param name="source">compressed data source</param>
        /// <param name="byteReadBufferSize">
        /// size of internal buffer used in case of
        /// byte-by-byte reading
        /// </param>
        public BrotliInputStream(Stream source, int byteReadBufferSize) {
            if (byteReadBufferSize <= 0) {
                throw new ArgumentException("Bad buffer size:" + byteReadBufferSize);
            }
            else {
                if (source == null) {
                    throw new ArgumentException("source is null");
                }
            }
            this.buffer = new byte[byteReadBufferSize];
            this.remainingBufferBytes = 0;
            this.bufferOffset = 0;
            try {
                state.input = source;
                Decode.InitState(state);
            }
            catch (BrotliRuntimeException ex) {
                throw new System.IO.IOException("Brotli decoder initialization failed", ex);
            }
        }

        /// <summary>Attach "RAW" dictionary (chunk) to decoder.</summary>
        public virtual void AttachDictionaryChunk(byte[] data) {
            Decode.AttachDictionaryChunk(state, data);
        }

        /// <summary>Request decoder to produce output as soon as it is available.</summary>
        public virtual void EnableEagerOutput() {
            Decode.EnableEagerOutput(state);
        }

        /// <summary>Enable "large window" stream feature.</summary>
        public virtual void EnableLargeWindow() {
            Decode.EnableLargeWindow(state);
        }

        /// <summary><inheritDoc/></summary>
        public override void Close() {
            Decode.Close(state);
            Utils.CloseInput(state);
        }

        /// <summary><inheritDoc/></summary>
        public override int ReadByte() {
            if (bufferOffset >= remainingBufferBytes) {
                remainingBufferBytes = Read(buffer, 0, buffer.Length);
                bufferOffset = 0;
                if (remainingBufferBytes == END_OF_STREAM_MARKER) {
                    // Both Java and C# return the same value for EOF on single-byte read.
                    return -1;
                }
            }
            return buffer[bufferOffset++] & 0xFF;
        }

        /// <summary><inheritDoc/></summary>
        public override int Read(byte[] destBuffer, int destOffset, int destLen) {
            if (destOffset < 0) {
                throw new ArgumentException("Bad offset: " + destOffset);
            }
            else {
                if (destLen < 0) {
                    throw new ArgumentException("Bad length: " + destLen);
                }
                else {
                    if (destOffset + destLen > destBuffer.Length) {
                        throw new ArgumentException("Buffer overflow: " + (destOffset + destLen) + " > " + destBuffer.Length);
                    }
                    else {
                        if (destLen == 0) {
                            return 0;
                        }
                    }
                }
            }
            int copyLen = Math.Max(remainingBufferBytes - bufferOffset, 0);
            if (copyLen != 0) {
                copyLen = Math.Min(copyLen, destLen);
                Array.Copy(buffer, bufferOffset, destBuffer, destOffset, copyLen);
                bufferOffset += copyLen;
                destOffset += copyLen;
                destLen -= copyLen;
                if (destLen == 0) {
                    return copyLen;
                }
            }
            try {
                state.output = destBuffer;
                state.outputOffset = destOffset;
                state.outputLength = destLen;
                state.outputUsed = 0;
                Decode.Decompress(state);
                copyLen += state.outputUsed;
                copyLen = (copyLen > 0) ? copyLen : END_OF_STREAM_MARKER;
                return copyLen;
            }
            catch (BrotliRuntimeException ex) {
                throw new System.IO.IOException("Brotli stream decoding failed", ex);
            }
        }
        // <{[INJECTED CODE]}>
        public override bool CanRead {
            get {return true;}
        }

        public override bool CanSeek {
            get {return false;}
        }
        public override long Length {
            get {throw new System.NotSupportedException();}
        }
        public override long Position {
            get {throw new System.NotSupportedException();}
            set {throw new System.NotSupportedException();}
        }
        public override long Seek(long offset, System.IO.SeekOrigin origin) {
            throw new System.NotSupportedException();
        }
        public override void SetLength(long value){
            throw new System.NotSupportedException();
        }

        public override bool CanWrite{get{return false;}}
        public override System.IAsyncResult BeginWrite(byte[] buffer, int offset,
            int count, System.AsyncCallback callback, object state) {
            throw new System.NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count) {
            throw new System.NotSupportedException();
        }

        public override void Flush() {}
    }
}
