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
using iText.Commons.Utils;

namespace iText.IO.Source {
    public class ByteBuffer {
        private static readonly byte[] bytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100
            , 101, 102 };

        protected internal int count;

        private byte[] buffer;

        public ByteBuffer()
            : this(128) {
        }

        public ByteBuffer(int size) {
            if (size < 1) {
                size = 128;
            }
            buffer = new byte[size];
        }

        public static int GetHex(int v) {
            if (v >= '0' && v <= '9') {
                return v - '0';
            }
            if (v >= 'A' && v <= 'F') {
                return v - 'A' + 10;
            }
            if (v >= 'a' && v <= 'f') {
                return v - 'a' + 10;
            }
            return -1;
        }

        public virtual iText.IO.Source.ByteBuffer Append(byte b) {
            int newCount = count + 1;
            if (newCount > buffer.Length) {
                byte[] newBuffer = new byte[Math.Max(buffer.Length << 1, newCount)];
                Array.Copy(buffer, 0, newBuffer, 0, count);
                buffer = newBuffer;
            }
            buffer[count] = b;
            count = newCount;
            return this;
        }

        public virtual iText.IO.Source.ByteBuffer Append(byte[] b, int off, int len) {
            if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0) || len == 
                0) {
                return this;
            }
            int newCount = count + len;
            if (newCount > buffer.Length) {
                byte[] newBuffer = new byte[Math.Max(buffer.Length << 1, newCount)];
                Array.Copy(buffer, 0, newBuffer, 0, count);
                buffer = newBuffer;
            }
            Array.Copy(b, off, buffer, count, len);
            count = newCount;
            return this;
        }

        public virtual iText.IO.Source.ByteBuffer Append(byte[] b) {
            return Append(b, 0, b.Length);
        }

        public virtual iText.IO.Source.ByteBuffer Append(int b) {
            return Append((byte)b);
        }

        public virtual iText.IO.Source.ByteBuffer Append(String str) {
            return Append(ByteUtils.GetIsoBytes(str));
        }

        public virtual iText.IO.Source.ByteBuffer AppendHex(byte b) {
            Append(bytes[(b >> 4) & 0x0f]);
            return Append(bytes[b & 0x0f]);
        }

        public virtual byte Get(int index) {
            if (index >= count) {
                throw new IndexOutOfRangeException(MessageFormatUtil.Format("Index: {0}, Size: {1}", index, count));
            }
            return buffer[index];
        }

        public virtual byte[] GetInternalBuffer() {
            return buffer;
        }

        public virtual int Size() {
            return count;
        }

        public virtual bool IsEmpty() {
            return Size() == 0;
        }

        public virtual int Capacity() {
            return buffer.Length;
        }

        public virtual iText.IO.Source.ByteBuffer Reset() {
            count = 0;
            return this;
        }

        public virtual byte[] ToByteArray(int off, int len) {
            byte[] newBuf = new byte[len];
            Array.Copy(buffer, off, newBuf, 0, len);
            return newBuf;
        }

        public virtual byte[] ToByteArray() {
            return ToByteArray(0, count);
        }

        public virtual bool StartsWith(byte[] b) {
            if (Size() < b.Length) {
                return false;
            }
            for (int k = 0; k < b.Length; ++k) {
                if (buffer[k] != b[k]) {
                    return false;
                }
            }
            return true;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Fill
        /// <c>ByteBuffer</c>
        /// from the end.
        /// </summary>
        /// <remarks>
        /// Fill
        /// <c>ByteBuffer</c>
        /// from the end.
        /// Set byte at
        /// <c>capacity() - size() - 1</c>
        /// position.
        /// </remarks>
        /// <param name="b">
        /// 
        /// <c>byte</c>.
        /// </param>
        /// <returns>
        /// 
        /// <c>ByteBuffer</c>.
        /// </returns>
        internal virtual iText.IO.Source.ByteBuffer Prepend(byte b) {
            buffer[buffer.Length - count - 1] = b;
            count++;
            return this;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Fill
        /// <c>ByteBuffer</c>
        /// from the end.
        /// </summary>
        /// <remarks>
        /// Fill
        /// <c>ByteBuffer</c>
        /// from the end.
        /// Set bytes from
        /// <c>capacity() - size() - b.length</c>
        /// position.
        /// </remarks>
        /// <param name="b">
        /// 
        /// <c>byte</c>.
        /// </param>
        /// <returns>
        /// 
        /// <c>ByteBuffer</c>.
        /// </returns>
        internal virtual iText.IO.Source.ByteBuffer Prepend(byte[] b) {
            Array.Copy(b, 0, buffer, buffer.Length - count - b.Length, b.Length);
            count += b.Length;
            return this;
        }
//\endcond
    }
}
