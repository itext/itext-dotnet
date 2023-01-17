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
using System.Text;

namespace iText.Barcodes.Qrcode {
    /// <summary>A class which wraps a 2D array of bytes.</summary>
    /// <remarks>
    /// A class which wraps a 2D array of bytes. The default usage is signed. If you want to use it as a
    /// unsigned container, it's up to you to do byteValue &amp; 0xff at each location.
    /// JAVAPORT: The original code was a 2D array of ints, but since it only ever gets assigned
    /// -1, 0, and 1, I'm going to use less memory and go with bytes.
    /// </remarks>
    /// <author>dswitkin@google.com (Daniel Switkin)</author>
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
