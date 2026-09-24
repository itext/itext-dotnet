/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Source;

namespace iText.IO.Font.Cmap {
    /// <summary>Supplies a CMap tokenizer backed by an in-memory byte array.</summary>
    public class CMapLocationFromBytes : ICMapLocation {
        private byte[] data;

        /// <summary>Creates a location backed by the provided CMap bytes.</summary>
        /// <param name="data">the CMap source bytes; retained without copying</param>
        public CMapLocationFromBytes(byte[] data) {
            this.data = data;
        }

        /// <summary>Creates a tokenizer for the retained CMap bytes.</summary>
        /// <param name="location">ignored because this implementation has one in-memory source</param>
        /// <returns>a new tokenizer over the retained bytes</returns>
        public virtual PdfTokenizer GetLocation(String location) {
            return new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(data)));
        }
    }
}
