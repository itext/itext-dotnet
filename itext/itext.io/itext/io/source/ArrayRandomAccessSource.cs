/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Exceptions;

namespace iText.IO.Source {
//\cond DO_NOT_DOCUMENT
    /// <summary>A RandomAccessSource that is based on an underlying byte array</summary>
    internal class ArrayRandomAccessSource : IRandomAccessSource {
        private byte[] array;

        /// <summary>Creates a source backed directly by the specified byte array.</summary>
        /// <param name="array">the non-null array to read; subsequent mutations are visible to this source</param>
        public ArrayRandomAccessSource(byte[] array) {
            if (array == null) {
                throw new ArgumentException("Passed byte array can not be null.");
            }
            this.array = array;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long offset) {
            if (array == null) {
                throw new InvalidOperationException(IoExceptionMessageConstant.ALREADY_CLOSED);
            }
            if (offset >= array.Length) {
                return -1;
            }
            return 0xff & array[(int)offset];
        }

        /// <summary><inheritDoc/></summary>
        public virtual int Get(long offset, byte[] bytes, int off, int len) {
            if (array == null) {
                throw new InvalidOperationException(IoExceptionMessageConstant.ALREADY_CLOSED);
            }
            if (offset >= array.Length) {
                return -1;
            }
            if (offset + len > array.Length) {
                len = (int)(array.Length - offset);
            }
            Array.Copy(array, (int)offset, bytes, off, len);
            return len;
        }

        /// <summary><inheritDoc/></summary>
        public virtual long Length() {
            if (array == null) {
                throw new InvalidOperationException(IoExceptionMessageConstant.ALREADY_CLOSED);
            }
            return array.Length;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Close() {
            array = null;
        }
    }
//\endcond
}
