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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filters {
    /// <summary>A filter that doesn't modify the stream at all</summary>
    public class DoNothingFilter : IFilterHandler {
        private PdfName lastFilterName;

        public virtual byte[] Decode(byte[] b, PdfName filterName, PdfObject decodeParams, PdfDictionary streamDictionary
            ) {
            lastFilterName = filterName;
            return b;
        }

        /// <summary>Returns the last decoded filter name.</summary>
        /// <returns>the last decoded filter name.</returns>
        [System.ObsoleteAttribute(@"Will be removed in 7.2. Used as a crutch iniText.Kernel.Pdf.Xobject.PdfImageXObject.GetImageBytes() implementation. Now this method does not needed. If the user has been used it, then the same approach can be reached with nested class."
            )]
        public virtual PdfName GetLastFilterName() {
            return lastFilterName;
        }
    }
}
