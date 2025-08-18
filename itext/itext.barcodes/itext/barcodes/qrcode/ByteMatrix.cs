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
using System.Text;

namespace iText.Barcodes.Qrcode {
    /// <summary>A class which wraps a 2D array of bytes.</summary>
    /// <remarks>
    /// A class which wraps a 2D array of bytes. The default usage is signed. If you want to use it as a
    /// unsigned container, it's up to you to do byteValue &amp; 0xff at each location.
    /// JAVAPORT: The original code was a 2D array of ints, but since it only ever gets assigned
    /// -1, 0, and 1, I'm going to use less memory and go with bytes.
    /// </remarks>
    public sealed class ByteMatrix {
        private readonly byte[][] bytes;

        private readonly int width;

        private readonly int height;

        /// <summary>Create a ByteMatix of given width and height, with the values initialized to 0</summary>
        /// <param name="width">width of the matrix</param>
        /// <param name="height">height of the matrix</param>
        public ByteMatrix(int width, int height) {
            bytes = new byte[height][];
            for (int i = 0; i < height; i++) {
                bytes[i] = new byte[width];
            }
            this.width = width;
            this.height = height;
        }

        /// <returns>height of the matrix</returns>
        public int GetHeight() {
            return height;
        }

        /// <returns>width of the matrix</returns>
        public int GetWidth() {
            return width;
        }

        /// <summary>Get the value of the byte at (x,y)</summary>
        /// <param name="x">the width coordinate</param>
        /// <param name="y">the height coordinate</param>
        /// <returns>the byte value at position (x,y)</returns>
        public byte Get(int x, int y) {
            return bytes[y][x];
        }

        /// <returns>matrix as byte[][]</returns>
        public byte[][] GetArray() {
            return bytes;
        }

        /// <summary>Set the value of the byte at (x,y)</summary>
        /// <param name="x">the width coordinate</param>
        /// <param name="y">the height coordinate</param>
        /// <param name="value">the new byte value</param>
        public void Set(int x, int y, byte value) {
            bytes[y][x] = value;
        }

        /// <summary>Set the value of the byte at (x,y)</summary>
        /// <param name="x">the width coordinate</param>
        /// <param name="y">the height coordinate</param>
        /// <param name="value">the new byte value</param>
        public void Set(int x, int y, int value) {
            bytes[y][x] = (byte)value;
        }

        /// <summary>Resets the contents of the entire matrix to value</summary>
        /// <param name="value">new value of every element</param>
        public void Clear(byte value) {
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    bytes[y][x] = value;
                }
            }
        }

        /// <returns>String representation</returns>
        public override String ToString() {
            StringBuilder result = new StringBuilder(2 * width * height + 2);
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    switch (bytes[y][x]) {
                        case 0: {
                            result.Append(" 0");
                            break;
                        }

                        case 1: {
                            result.Append(" 1");
                            break;
                        }

                        default: {
                            result.Append("  ");
                            break;
                        }
                    }
                }
                result.Append('\n');
            }
            return result.ToString();
        }
    }
}
