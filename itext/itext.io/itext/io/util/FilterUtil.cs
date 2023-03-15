/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using System.util.zlib;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class FilterUtil {
        /// <summary>A helper to FlateDecode.</summary>
        /// <param name="input">the input data</param>
        /// <param name="strict">
        /// <CODE>true</CODE> to read a correct stream. <CODE>false</CODE>
        /// to try to read a corrupted stream
        /// </param>
        /// <returns>the decoded data</returns>
        public static byte[] FlateDecode(byte[] input, bool strict) {
            using (MemoryStream stream = new MemoryStream(input)) {
                using (ZInflaterInputStream zip = new ZInflaterInputStream(stream)) {
                    MemoryStream output = new MemoryStream();
                    byte[] b = new byte[strict ? 4092 : 1];
                    try {
                        int n;
                        while ((n = zip.Read(b, 0, b.Length)) > 0) {
                            output.Write(b, 0, n);
                        }
                        zip.Dispose();
                        output.Dispose();
                        return output.ToArray();
                    }
                    catch {
                        if (strict)
                            return null;
                        return output.ToArray();
                    }
                }
            }
        }
    

        /// <summary>Decodes a stream that has the FlateDecode filter.</summary>
        /// <param name="input">the input data</param>
        /// <returns>the decoded data</returns>
        public static byte[] FlateDecode(byte[] input) {
            byte[] b = FlateDecode(input, true);
            if (b == null) {
                return FlateDecode(input, false);
            }
            return b;
        }

        /// <summary>
        /// This method provides support for general purpose decompression using the
        /// popular ZLIB compression library.
        /// </summary>
        /// <param name="deflated">the input data bytes</param>
        /// <param name="inflated">the buffer for the uncompressed data</param>
        public static void InflateData(byte[] deflated, byte[] inflated) {
            byte[] outp = FlateDecode(deflated);
            System.Array.Copy(outp, 0, inflated, 0, Math.Min(outp.Length, inflated.Length));
        }

        public static Stream GetInflaterInputStream(Stream input) {
            return new ZInflaterInputStream(input);
        }
    }
}
