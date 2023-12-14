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
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <author>psoares</author>
    public class CMapUniCid : AbstractCMap {
        private IntHashtable map = new IntHashtable(65537);

        internal override void AddChar(String mark, CMapObject code) {
            if (code.IsNumber()) {
                int codePoint;
                String s = ToUnicodeString(mark, true);
                if (iText.IO.Util.TextUtil.IsSurrogatePair(s, 0)) {
                    codePoint = iText.IO.Util.TextUtil.ConvertToUtf32(s, 0);
                }
                else {
                    codePoint = (int)s[0];
                }
                map.Put(codePoint, (int)code.GetValue());
            }
        }

        public virtual int Lookup(int character) {
            return map.Get(character);
        }

        public virtual CMapToUnicode ExportToUnicode() {
            CMapToUnicode uni = new CMapToUnicode();
            int[] keys = map.ToOrderedKeys();
            foreach (int key in keys) {
                uni.AddChar(map.Get(key), iText.IO.Util.TextUtil.ConvertFromUtf32(key));
            }
            int spaceCid = Lookup(32);
            if (spaceCid != 0) {
                uni.AddChar(spaceCid, iText.IO.Util.TextUtil.ConvertFromUtf32(32));
            }
            return uni;
        }
    }
}
