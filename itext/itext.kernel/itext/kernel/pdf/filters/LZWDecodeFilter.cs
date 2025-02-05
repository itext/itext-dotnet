/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles LZWDECODE filter</summary>
    public class LZWDecodeFilter : MemoryLimitsAwareFilter {
        /// <summary>Decodes a byte[] according to the LZW encoding.</summary>
        /// <param name="in">byte[] to be decoded</param>
        /// <returns>decoded byte[]</returns>
        public static byte[] LZWDecode(byte[] @in) {
            return LZWDecodeInternal(@in, new MemoryStream());
        }

        /// <summary><inheritDoc/></summary>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            MemoryStream outputStream = EnableMemoryLimitsAwareHandler(streamDictionary);
            b = LZWDecodeInternal(b, outputStream);
            b = FlateDecodeFilter.DecodePredictor(b, decodeParams);
            return b;
        }

        /// <summary>Decodes a byte[] according to the LZW encoding.</summary>
        /// <param name="in">byte[] to be decoded</param>
        /// <param name="out">the out stream which will be used to write the bytes.</param>
        /// <returns>decoded byte[]</returns>
        private static byte[] LZWDecodeInternal(byte[] @in, MemoryStream @out) {
            LZWDecoder lzw = new LZWDecoder();
            lzw.Decode(@in, @out);
            return @out.ToArray();
        }
    }
}
