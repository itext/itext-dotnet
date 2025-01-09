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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>Encapsulates filter behavior for PDF streams.</summary>
    /// <remarks>
    /// Encapsulates filter behavior for PDF streams.  Classes generally interace with this
    /// using the static getDefaultFilterHandlers() method, then obtain the desired
    /// <see cref="IFilterHandler"/>
    /// via a lookup.
    /// </remarks>
    public sealed class FilterHandlers {
        // Dev note:  we eventually want to refactor PdfReader so all of the existing filter functionality is moved into this class
        // it may also be better to split the sub-classes out into a separate package
        /// <summary>
        /// The default
        /// <see cref="IFilterHandler"/>
        /// s used by iText
        /// </summary>
        private static readonly IDictionary<PdfName, IFilterHandler> defaults;

        static FilterHandlers() {
            IDictionary<PdfName, IFilterHandler> map = new Dictionary<PdfName, IFilterHandler>();
            map.Put(PdfName.FlateDecode, new FlateDecodeFilter());
            map.Put(PdfName.Fl, new FlateDecodeFilter());
            map.Put(PdfName.ASCIIHexDecode, new ASCIIHexDecodeFilter());
            map.Put(PdfName.AHx, new ASCIIHexDecodeFilter());
            map.Put(PdfName.ASCII85Decode, new ASCII85DecodeFilter());
            map.Put(PdfName.A85, new ASCII85DecodeFilter());
            map.Put(PdfName.LZWDecode, new LZWDecodeFilter());
            map.Put(PdfName.CCITTFaxDecode, new CCITTFaxDecodeFilter());
            map.Put(PdfName.Crypt, new DoNothingFilter());
            map.Put(PdfName.RunLengthDecode, new RunLengthDecodeFilter());
            map.Put(PdfName.DCTDecode, new DctDecodeFilter());
            map.Put(PdfName.JPXDecode, new JpxDecodeFilter());
            defaults = JavaCollectionsUtil.UnmodifiableMap(map);
        }

        /// <returns>
        /// the default
        /// <see cref="IFilterHandler"/>
        /// s used by iText
        /// </returns>
        public static IDictionary<PdfName, IFilterHandler> GetDefaultFilterHandlers() {
            return defaults;
        }
    }
}
