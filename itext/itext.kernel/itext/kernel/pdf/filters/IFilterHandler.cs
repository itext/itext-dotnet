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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>
    /// The main interface for creating a new
    /// <c>FilterHandler</c>
    /// </summary>
    public interface IFilterHandler {
        /// <summary>Decode the byte[] using the provided filterName.</summary>
        /// <param name="b">the bytes that need to be decoded</param>
        /// <param name="filterName">PdfName of the filter</param>
        /// <param name="decodeParams">decode parameters</param>
        /// <param name="streamDictionary">
        /// the dictionary of the stream. Can contain additional information needed to decode the
        /// byte[].
        /// </param>
        /// <returns>decoded byte array</returns>
        byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary);
    }
}
