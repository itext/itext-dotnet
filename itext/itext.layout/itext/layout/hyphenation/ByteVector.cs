/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
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

namespace iText.Layout.Hyphenation {
    /// <summary>
    /// This class implements a simple byte vector with access to the
    /// underlying array.
    /// </summary>
    /// <remarks>
    /// This class implements a simple byte vector with access to the
    /// underlying array.
    /// <para />
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </remarks>
    public class ByteVector {
        /// <summary>Capacity increment size</summary>
        private const int DEFAULT_BLOCK_SIZE = 2048;

        private int blockSize;

        /// <summary>The encapsulated array</summary>
        private byte[] array;

        /// <summary>Points to next free item</summary>
        private int n;

        /// <summary>Construct byte vector instance with default block size.</summary>
        public ByteVector()
            : this(DEFAULT_BLOCK_SIZE) {
        }

        /// <summary>Construct byte vector instance.</summary>
        /// <param name="capacity">initial block size</param>
        public ByteVector(int capacity) {
            if (capacity > 0) {
                blockSize = capacity;
            }
            else {
                blockSize = DEFAULT_BLOCK_SIZE;
            }
            array = new byte[blockSize];
            n = 0;
        }

        /// <summary>Construct byte vector instance.</summary>
        /// <param name="a">
        /// byte array to use
        /// TODO should n should be initialized to a.length to be consistent with
        /// CharVector behavior? [GA]
        /// </param>
        public ByteVector(byte[] a) {
            blockSize = DEFAULT_BLOCK_SIZE;
            array = a;
            n = 0;
        }

        /// <summary>Construct byte vector instance.</summary>
        /// <param name="a">byte array to use</param>
        /// <param name="capacity">
        /// initial block size
        /// TODO should n should be initialized to a.length to be consistent with
        /// CharVector behavior? [GA]
        /// </param>
        public ByteVector(byte[] a, int capacity) {
            if (capacity > 0) {
                blockSize = capacity;
            }
            else {
                blockSize = DEFAULT_BLOCK_SIZE;
            }
            array = a;
            n = 0;
        }

        /// <summary>Obtain byte vector array.</summary>
        /// <returns>byte array</returns>
        public virtual byte[] GetArray() {
            return array;
        }

        /// <summary>Obtain number of items in array.</summary>
        /// <returns>number of items</returns>
        public virtual int Length() {
            return n;
        }

        /// <summary>Obtain capacity of array.</summary>
        /// <returns>current capacity of array</returns>
        public virtual int Capacity() {
            return array.Length;
        }

        /// <summary>Pet byte at index.</summary>
        /// <param name="index">the index</param>
        /// <param name="val">a byte</param>
        public virtual void Put(int index, byte val) {
            array[index] = val;
        }

        /// <summary>Get byte at index.</summary>
        /// <param name="index">the index</param>
        /// <returns>a byte</returns>
        public virtual byte Get(int index) {
            return array[index];
        }

        /// <summary>This is to implement memory allocation in the array.</summary>
        /// <remarks>This is to implement memory allocation in the array. Like malloc().</remarks>
        /// <param name="size">to allocate</param>
        /// <returns>previous length</returns>
        public virtual int Alloc(int size) {
            int index = n;
            int len = array.Length;
            if (n + size >= len) {
                byte[] aux = new byte[len + blockSize];
                Array.Copy(array, 0, aux, 0, len);
                array = aux;
            }
            n += size;
            return index;
        }

        /// <summary>Trim byte vector to current length.</summary>
        public virtual void TrimToSize() {
            if (n < array.Length) {
                byte[] aux = new byte[n];
                Array.Copy(array, 0, aux, 0, n);
                array = aux;
            }
        }
    }
}
