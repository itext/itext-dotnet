/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
    }
}
