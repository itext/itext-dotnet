/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles ASCIIHexDecode filter</summary>
    public class ASCIIHexDecodeFilter : MemoryLimitsAwareFilter {
        /// <summary>Decodes a byte[] according to ASCII Hex encoding.</summary>
        /// <param name="in">byte[] to be decoded</param>
        /// <returns>decoded byte[]</returns>
        public static byte[] ASCIIHexDecode(byte[] @in) {
            return ASCIIHexDecodeInternal(@in, new MemoryStream());
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            MemoryStream outputStream = EnableMemoryLimitsAwareHandler(streamDictionary);
            b = ASCIIHexDecodeInternal(b, outputStream);
            return b;
        }

        /// <summary>Decodes a byte[] according to ASCII Hex encoding.</summary>
        /// <param name="in">byte[] to be decoded</param>
        /// <param name="out">the out stream which will be used to write the bytes.</param>
        /// <returns>decoded byte[]</returns>
        private static byte[] ASCIIHexDecodeInternal(byte[] @in, MemoryStream @out) {
            bool first = true;
            int n1 = 0;
            for (int k = 0; k < @in.Length; ++k) {
                int ch = @in[k] & 0xff;
                if (ch == '>') {
                    break;
                }
                if (PdfTokenizer.IsWhitespace(ch)) {
                    continue;
                }
                int n = ByteBuffer.GetHex(ch);
                if (n == -1) {
                    throw new PdfException(KernelExceptionMessageConstant.ILLEGAL_CHARACTER_IN_ASCIIHEXDECODE);
                }
                if (first) {
                    n1 = n;
                }
                else {
                    @out.Write((byte)((n1 << 4) + n));
                }
                first = !first;
            }
            if (!first) {
                @out.Write((byte)(n1 << 4));
            }
            return @out.ToArray();
        }
    }
}
