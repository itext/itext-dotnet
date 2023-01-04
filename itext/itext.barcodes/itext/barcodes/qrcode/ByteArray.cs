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

namespace iText.Barcodes.Qrcode {
    /// <summary>This class implements an array of unsigned bytes.</summary>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
    internal sealed class ByteArray {
        private const int INITIAL_SIZE = 32;

        private byte[] bytes;

        private int size;

        /// <summary>Creates a new ByteArray instance with size 0.</summary>
        public ByteArray() {
            bytes = null;
            size = 0;
        }

        /// <summary>Creates a new ByteArray instance of the specified size.</summary>
        /// <param name="size">size of the array</param>
        public ByteArray(int size) {
            bytes = new byte[size];
            this.size = size;
        }

        /// <summary>Creates a new ByteArray instance based on an existing byte[].</summary>
        /// <param name="byteArray">the byte[]</param>
        public ByteArray(byte[] byteArray) {
            bytes = byteArray;
            size = bytes.Length;
        }

        /// <summary>Access an unsigned byte at location index.</summary>
        /// <param name="index">The index in the array to access.</param>
        /// <returns>The unsigned value of the byte as an int.</returns>
        public int At(int index) {
            return bytes[index] & 0xff;
        }

        /// <summary>Set the value at "index" to "value"</summary>
        /// <param name="index">position in the byte-array</param>
        /// <param name="value">new value</param>
        public void Set(int index, int value) {
            bytes[index] = (byte)value;
        }

        /// <returns>size of the array</returns>
        public int Size() {
            return size;
        }

        /// <returns>true if size is equal to 0, false otherwise</returns>
        public bool IsEmpty() {
            return size == 0;
        }

        /// <summary>Append a byte to the end of the array.</summary>
        /// <remarks>Append a byte to the end of the array. If the array is too small, it's capacity is doubled.</remarks>
        /// <param name="value">byte to append.</param>
        public void AppendByte(int value) {
            if (size == 0 || size >= bytes.Length) {
                int newSize = Math.Max(INITIAL_SIZE, size << 1);
                Reserve(newSize);
            }
            bytes[size] = (byte)value;
            size++;
        }

        /// <summary>Increase the capacity of the array to "capacity" if the current capacity is smaller</summary>
        /// <param name="capacity">the new capacity</param>
        public void Reserve(int capacity) {
            if (bytes == null || bytes.Length < capacity) {
                byte[] newArray = new byte[capacity];
                if (bytes != null) {
                    Array.Copy(bytes, 0, newArray, 0, bytes.Length);
                }
                bytes = newArray;
            }
        }

        /// <summary>Copy count bytes from array source starting at offset.</summary>
        /// <param name="source">source of the copied bytes</param>
        /// <param name="offset">offset to start at</param>
        /// <param name="count">number of bytes to copy</param>
        public void Set(byte[] source, int offset, int count) {
            bytes = new byte[count];
            size = count;
            for (int x = 0; x < count; x++) {
                bytes[x] = source[offset + x];
            }
        }
    }
}
