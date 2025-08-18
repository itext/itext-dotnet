/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class implements an array of unsigned bytes.</summary>
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
//\endcond
}
