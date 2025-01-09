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
using System;
using System.Text;
using iText.IO.Source;

namespace iText.IO.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </remarks>
    public sealed class PdfNameUtil {
        private PdfNameUtil() {
        }

        // Do nothing
        public static String DecodeName(byte[] content) {
            StringBuilder buf = new StringBuilder();
            try {
                for (int k = 0; k < content.Length; ++k) {
                    char c = (char)content[k];
                    if (c == '#') {
                        byte c1 = content[k + 1];
                        byte c2 = content[k + 2];
                        c = (char)((ByteBuffer.GetHex(c1) << 4) + ByteBuffer.GetHex(c2));
                        k += 2;
                    }
                    buf.Append(c);
                }
            }
            catch (IndexOutOfRangeException) {
            }
            // empty on purpose
            return buf.ToString();
        }
    }
}
