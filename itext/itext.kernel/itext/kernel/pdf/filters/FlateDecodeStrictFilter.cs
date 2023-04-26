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
using System.IO;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Handles strict FlateDecode filter.</summary>
    public class FlateDecodeStrictFilter : FlateDecodeFilter {
        /// <summary><inheritDoc/></summary>
        public override byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            MemoryStream outputStream = EnableMemoryLimitsAwareHandler(streamDictionary);
            byte[] res = FlateDecode(b, outputStream);
            b = DecodePredictor(res, decodeParams);
            return b;
        }

        /// <summary>A helper to flateDecode.</summary>
        /// <param name="in">the input data</param>
        /// <param name="out">the out stream which will be used to write the bytes.</param>
        /// <returns>the decoded data</returns>
        private static byte[] FlateDecode(byte[] @in, MemoryStream @out) {
            return FlateDecodeInternal(@in, true, @out);
        }
    }
}
