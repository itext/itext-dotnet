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
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    public class CMapLocationResource : ICMapLocation {
        public virtual PdfTokenizer GetLocation(String location) {
            String fullName = GetLocationPath() + location;
            Stream inp = ResourceUtil.GetResourceStream(fullName);
            if (inp == null) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.CMAP_WAS_NOT_FOUND).SetMessageParams(
                    fullName);
            }
            return new PdfTokenizer(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(inp)));
        }

        /// <summary>Retrieve base folder path where CMaps are located.</summary>
        /// <returns>CMaps location path.</returns>
        public virtual String GetLocationPath() {
            return FontResources.CMAPS;
        }
    }
}
