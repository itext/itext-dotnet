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
using System;
using iText.IO.Exceptions;

namespace iText.IO.Source {
    /// <summary>A RandomAccessSource that is based on an underlying byte array</summary>
    internal class ArrayRandomAccessSource : IRandomAccessSource {
        private byte[] array;

        public ArrayRandomAccessSource(byte[] array) {
            if (array == null) {
                throw new ArgumentException("Passed byte array can not be null.");
            }
            this.array = array;
        }

        public virtual int Get(long offset) {
            if (array == null) {
                throw new InvalidOperationException(IoExceptionMessageConstant.ALREADY_CLOSED);
            }
            if (offset >= array.Length) {
                return -1;
            }
            return 0xff & array[(int)offset];
        }

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

        public virtual long Length() {
            if (array == null) {
                throw new InvalidOperationException(IoExceptionMessageConstant.ALREADY_CLOSED);
            }
            return array.Length;
        }

        public virtual void Close() {
            array = null;
        }
    }
}
