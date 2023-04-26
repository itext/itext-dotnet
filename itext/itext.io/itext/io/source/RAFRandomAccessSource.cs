/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.IO.Source {
    /// <summary>
    /// A RandomAccessSource that uses a
    /// <see cref="System.IO.FileStream"/>
    /// as it's source
    /// Note: Unlike most of the RandomAccessSource implementations, this class is not thread safe
    /// </summary>
    internal class RAFRandomAccessSource : IRandomAccessSource {
        /// <summary>The source</summary>
        private readonly FileStream raf;

        /// <summary>The length of the underling RAF.</summary>
        /// <remarks>
        /// The length of the underling RAF.  Note that the length is cached at construction time to avoid the possibility
        /// of
        /// <see cref="System.IO.IOException"/>
        /// s when reading the length.
        /// </remarks>
        private readonly long length;

        /// <summary>Creates this object</summary>
        /// <param name="raf">the source for this RandomAccessSource</param>
        public RAFRandomAccessSource(FileStream raf) {
            this.raf = raf;
            length = raf.Length;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position) {
            if (position > length) {
                return -1;
            }
            // Not thread safe!
            if (raf.Position != position) {
                raf.Seek(position);
            }
            return raf.ReadByte();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            if (position > length) {
                return -1;
            }
            // Not thread safe!
            if (raf.Position != position) {
                raf.Seek(position);
            }
            return raf.JRead(bytes, off, len);
        }

        /// <summary>
        /// <inheritDoc/>
        /// Note: the length is determined when the
        /// <see cref="RAFRandomAccessSource"/>
        /// is constructed.
        /// </summary>
        /// <remarks>
        /// <inheritDoc/>
        /// Note: the length is determined when the
        /// <see cref="RAFRandomAccessSource"/>
        /// is constructed.  If the file length changes
        /// after construction, that change will not be reflected in this call.
        /// </remarks>
        public virtual long Length() {
            return length;
        }

        /// <summary>Closes the underlying RandomAccessFile</summary>
        public virtual void Close() {
            raf.Dispose();
        }
    }
}
