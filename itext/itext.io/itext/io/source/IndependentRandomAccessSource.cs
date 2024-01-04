/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    /// <summary>A RandomAccessSource that is wraps another RandomAccessSource but does not propagate close().</summary>
    /// <remarks>
    /// A RandomAccessSource that is wraps another RandomAccessSource but does not propagate close().  This is useful when
    /// passing a RandomAccessSource to a method that would normally close the source.
    /// </remarks>
    public class IndependentRandomAccessSource : IRandomAccessSource {
        /// <summary>The source</summary>
        private readonly IRandomAccessSource source;

        /// <summary>Constructs a new IndependentRandomAccessSource object</summary>
        /// <param name="source">the source</param>
        public IndependentRandomAccessSource(IRandomAccessSource source) {
            this.source = source;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position) {
            return source.Get(position);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            return source.Get(position, bytes, off, len);
        }

        /// <summary><inheritDoc/></summary>
        public virtual long Length() {
            return source.Length();
        }

        /// <summary>Does nothing - the underlying source is not closed</summary>
        public virtual void Close() {
        }
        // do not close the source
    }
}
