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
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <summary>Class represents real codepoint-CID mapping without any additional manipulation.</summary>
    /// <remarks>
    /// Class represents real codepoint-CID mapping without any additional manipulation.
    /// <para />
    /// See
    /// <see cref="CMapCidToCodepoint"/>
    /// for CID-codepoint representation.
    /// </remarks>
    public class CMapCodepointToCid : AbstractCMap {
        private readonly IntHashtable map;

        public CMapCodepointToCid() {
            map = new IntHashtable();
        }

        public CMapCodepointToCid(CMapCidToCodepoint reverseMap) {
            map = reverseMap.GetReversMap();
        }

//\cond DO_NOT_DOCUMENT
        internal override void AddChar(String mark, CMapObject code) {
            if (code.IsNumber()) {
                byte[] ser = DecodeStringToByte(mark);
                int byteCode = 0;
                foreach (byte b in ser) {
                    byteCode <<= 8;
                    byteCode += b & 0xff;
                }
                map.Put(byteCode, (int)code.GetValue());
            }
        }
//\endcond

        public virtual int Lookup(int codepoint) {
            return this.map.Get(codepoint);
        }
    }
}
