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
using System;

namespace iText.IO.Source {
    /// <summary>
    /// A RandomAccessSource that wraps another RandomAccessSource and provides a window of it at a specific offset and over
    /// a specific length.
    /// </summary>
    /// <remarks>
    /// A RandomAccessSource that wraps another RandomAccessSource and provides a window of it at a specific offset and over
    /// a specific length.  Position 0 becomes the offset position in the underlying source.
    /// </remarks>
    public class WindowRandomAccessSource : IRandomAccessSource {
        /// <summary>The source</summary>
        private readonly IRandomAccessSource source;

        /// <summary>The amount to offset the source by</summary>
        private readonly long offset;

        /// <summary>The length</summary>
        private readonly long length;

        /// <summary>Constructs a new OffsetRandomAccessSource that extends to the end of the underlying source</summary>
        /// <param name="source">the source</param>
        /// <param name="offset">the amount of the offset to use</param>
        public WindowRandomAccessSource(IRandomAccessSource source, long offset)
            : this(source, offset, source.Length() - offset) {
        }

        /// <summary>Constructs a new OffsetRandomAccessSource with an explicit length</summary>
        /// <param name="source">the source</param>
        /// <param name="offset">the amount of the offset to use</param>
        /// <param name="length">the number of bytes to be included in this RAS</param>
        public WindowRandomAccessSource(IRandomAccessSource source, long offset, long length) {
            this.source = source;
            this.offset = offset;
            this.length = length;
        }

        /// <summary>
        /// <inheritDoc/>
        /// Note that the position will be adjusted to read from the corrected location in the underlying source
        /// </summary>
        public virtual int Get(long position) {
            if (position >= length) {
                return -1;
            }
            return source.Get(offset + position);
        }

        /// <summary>
        /// <inheritDoc/>
        /// Note that the position will be adjusted to read from the corrected location in the underlying source
        /// </summary>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            if (position >= length) {
                return -1;
            }
            long toRead = Math.Min(len, length - position);
            return source.Get(offset + position, bytes, off, (int)toRead);
        }

        /// <summary>
        /// <inheritDoc/>
        /// Note that the length will be adjusted to read from the corrected location in the underlying source
        /// </summary>
        public virtual long Length() {
            return length;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Close() {
            source.Close();
        }
    }
}
