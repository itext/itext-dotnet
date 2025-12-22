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
namespace iText.IO.Source {
    /// <summary>Interface for output streams that supports finalization without closing the underlying stream.</summary>
    /// <remarks>
    /// Interface for output streams that supports finalization without closing the underlying stream.
    /// <para />
    /// This interface is designed for output streams that wrap other streams and need to complete
    /// their processing (such as finishing compression, flushing buffers, or writing final data)
    /// without closing the underlying output stream. This is particularly useful when multiple
    /// operations need to be performed on the same underlying stream sequentially.
    /// <para />
    /// Implementations of this interface should ensure that calling
    /// <see cref="Finish()"/>
    /// completes
    /// all pending operations and releases any resources associated with the stream processing,
    /// but does not close the underlying stream.
    /// </remarks>
    /// <seealso cref="DeflaterOutputStream"/>
    /// <seealso cref="System.IO.Stream.Dispose()"/>
    public interface IFinishable {
        /// <summary>
        /// Is called to finalize the stream, implementing classes should ensure that the underlying
        /// output stream remains open after this method is called.
        /// </summary>
        /// <remarks>
        /// Is called to finalize the stream, implementing classes should ensure that the underlying
        /// output stream remains open after this method is called.
        /// <para />
        /// This method completes any pending write operations, flushes internal buffers,
        /// writes any final data required by the stream format, and releases resources
        /// associated with the stream processing. However, unlike
        /// <see cref="System.IO.Stream.Dispose()"/>
        /// ,
        /// it does not close the underlying output stream.
        /// <para />
        /// After calling this method, no further data should be written to this stream,
        /// but the underlying stream remains open and can be used for other operations.
        /// <para />
        /// This method should be idempotent - calling it multiple times should have the
        /// same effect as calling it once.
        /// </remarks>
        void Finish();
    }
}
