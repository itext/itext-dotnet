/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO.Compression;
using iText.IO.Source;

namespace iText.Brotlicompressor {
    /// <summary>
    /// An output stream that compresses data using the Brotli compression algorithm.
    /// </summary>
    /// <remarks>
    /// This is a wrapper around <see cref="System.IO.Compression.BrotliStream"/> that implements the
    /// <see cref="iText.IO.Source.IFinishable"/> interface, allowing it to be used in contexts where finalization
    /// without closing the underlying stream is required.
    /// </remarks>
    public class BrotliOutputStream : Stream, IFinishable {
        private readonly System.IO.Compression.BrotliStream internalBrotliStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrotliOutputStream"/> class with the specified stream and compression level.
        /// </summary>
        /// <param name="stream">The underlying stream to write compressed data to.</param>
        /// <param name="compressionLevel">The compression level to use.</param>
        public BrotliOutputStream(Stream stream, CompressionLevel compressionLevel) {
            internalBrotliStream = new System.IO.Compression.BrotliStream(stream, compressionLevel, true);
        }

        /// <summary>
        /// Finishes writing compressed data to the underlying stream.
        /// </summary>
        /// <remarks>
        /// This method finalizes the compression and flushes any remaining data to the underlying stream.
        /// The underlying stream is not closed.
        /// </remarks>
        public virtual void Finish() {
            //We can safely call close because we passed true to leaveOpen in the constructor
            internalBrotliStream.Close();
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush() {
            internalBrotliStream.Flush();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int count) {
            return internalBrotliStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin) {
            return internalBrotliStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value) {
            internalBrotliStream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count) {
            internalBrotliStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => internalBrotliStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => internalBrotliStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => internalBrotliStream.CanWrite;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => internalBrotliStream.Length;

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position {
            get => internalBrotliStream.Position;
            set => internalBrotliStream.Position = value;
        }
    }
}