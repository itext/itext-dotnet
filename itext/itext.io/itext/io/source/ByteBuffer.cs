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
using iText.Commons.Utils;

namespace iText.IO.Source {
    /// <summary>A growable byte buffer with append and byte-oriented conversion operations.</summary>
    public class ByteBuffer {
        private static readonly byte[] bytes = new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100
            , 101, 102 };

        /// <summary>The number of bytes currently stored in this buffer.</summary>
        protected internal int count;

        private byte[] buffer;

        /// <summary>Creates a buffer with the default initial capacity.</summary>
        public ByteBuffer()
            : this(128) {
        }

        /// <summary>Creates a buffer with the requested initial capacity.</summary>
        /// <param name="size">the initial capacity; values below one use the default capacity</param>
        public ByteBuffer(int size) {
            if (size < 1) {
                size = 128;
            }
            buffer = new byte[size];
        }

        /// <summary>Converts an ASCII hexadecimal digit to its numeric value.</summary>
        /// <param name="v">the character value to convert</param>
        /// <returns>
        /// a value from
        /// <c>0</c>
        /// through
        /// <c>15</c>
        /// , or
        /// <c>-1</c>
        /// when
        /// <paramref name="v"/>
        /// is not hexadecimal
        /// </returns>
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

        /// <summary>Appends one byte, expanding the backing array when necessary.</summary>
        /// <param name="b">the byte to append</param>
        /// <returns>this buffer</returns>
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

        /// <summary>Appends a range from a byte array.</summary>
        /// <param name="b">the source array</param>
        /// <param name="off">the zero-based source offset</param>
        /// <param name="len">the number of bytes to append</param>
        /// <returns>this buffer; invalid ranges and zero lengths leave it unchanged</returns>
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

        /// <summary>Appends all bytes from an array.</summary>
        /// <param name="b">the source array</param>
        /// <returns>this buffer</returns>
        public virtual iText.IO.Source.ByteBuffer Append(byte[] b) {
            return Append(b, 0, b.Length);
        }

        /// <summary>Appends the low eight bits of an integer.</summary>
        /// <param name="b">the value whose low byte is appended</param>
        /// <returns>this buffer</returns>
        public virtual iText.IO.Source.ByteBuffer Append(int b) {
            return Append((byte)b);
        }

        /// <summary>Appends the ISO-8859-1 compatible byte representation of a string.</summary>
        /// <param name="str">the string to append</param>
        /// <returns>this buffer</returns>
        public virtual iText.IO.Source.ByteBuffer Append(String str) {
            return Append(ByteUtils.GetIsoBytes(str));
        }

        /// <summary>Appends two lowercase hexadecimal characters representing a byte.</summary>
        /// <param name="b">the byte to encode</param>
        /// <returns>this buffer</returns>
        public virtual iText.IO.Source.ByteBuffer AppendHex(byte b) {
            Append(bytes[(b >> 4) & 0x0f]);
            return Append(bytes[b & 0x0f]);
        }

        /// <summary>Gets a stored byte by index.</summary>
        /// <param name="index">the zero-based index</param>
        /// <returns>
        /// the byte at
        /// <paramref name="index"/>
        /// </returns>
        public virtual byte Get(int index) {
            if (index >= count) {
                throw new IndexOutOfRangeException(MessageFormatUtil.Format("Index: {0}, Size: {1}", index, count));
            }
            return buffer[index];
        }

        /// <summary>Gets the mutable backing array without copying.</summary>
        /// <returns>
        /// the backing array, whose length may exceed
        /// <see cref="Size()"/>
        /// </returns>
        public virtual byte[] GetInternalBuffer() {
            return buffer;
        }

        /// <summary>Gets the number of bytes stored in this buffer.</summary>
        /// <returns>the logical byte count</returns>
        public virtual int Size() {
            return count;
        }

        /// <summary>Tests whether this buffer has no stored bytes.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when
        /// <see cref="Size()"/>
        /// is zero
        /// </returns>
        public virtual bool IsEmpty() {
            return Size() == 0;
        }

        /// <summary>Gets the current backing-array capacity.</summary>
        /// <returns>the number of bytes the backing array can hold without growing</returns>
        public virtual int Capacity() {
            return buffer.Length;
        }

        /// <summary>Discards all stored bytes while retaining the backing array.</summary>
        /// <returns>this buffer</returns>
        public virtual iText.IO.Source.ByteBuffer Reset() {
            count = 0;
            return this;
        }

        /// <summary>Copies a range from the backing array.</summary>
        /// <param name="off">the zero-based source offset</param>
        /// <param name="len">the number of bytes to copy</param>
        /// <returns>a new array containing the requested bytes</returns>
        public virtual byte[] ToByteArray(int off, int len) {
            byte[] newBuf = new byte[len];
            Array.Copy(buffer, off, newBuf, 0, len);
            return newBuf;
        }

        /// <summary>Copies all stored bytes.</summary>
        /// <returns>
        /// a new array containing bytes from zero through
        /// <see cref="Size()"/>
        /// </returns>
        public virtual byte[] ToByteArray() {
            return ToByteArray(0, count);
        }

        /// <summary>Tests whether the stored bytes begin with a sequence.</summary>
        /// <param name="b">the candidate prefix</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// when this buffer starts with
        /// <paramref name="b"/>
        /// </returns>
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
