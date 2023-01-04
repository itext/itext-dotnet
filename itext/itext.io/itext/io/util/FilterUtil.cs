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
