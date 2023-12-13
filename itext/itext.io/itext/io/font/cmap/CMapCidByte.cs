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
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.IO.Font.Cmap {
    /// <author>psoares</author>
    public class CMapCidByte : AbstractCMap {
        private IDictionary<int, byte[]> map = new Dictionary<int, byte[]>();

        private readonly byte[] EMPTY = new byte[] {  };

        private IList<byte[]> codeSpaceRanges = new List<byte[]>();

        internal override void AddChar(String mark, CMapObject code) {
            if (code.IsNumber()) {
                byte[] ser = DecodeStringToByte(mark);
                map.Put((int)code.GetValue(), ser);
            }
        }

        public virtual byte[] Lookup(int cid) {
            byte[] ser = map.Get(cid);
            if (ser == null) {
                return EMPTY;
            }
            else {
                return ser;
            }
        }

        public virtual IntHashtable GetReversMap() {
            IntHashtable code2cid = new IntHashtable(map.Count);
            foreach (int cid in map.Keys) {
                byte[] bytes = map.Get(cid);
                int byteCode = 0;
                foreach (byte b in bytes) {
                    byteCode <<= 8;
                    byteCode += b & 0xff;
                }
                code2cid.Put(byteCode, cid);
            }
            return code2cid;
        }

        /// <summary>
        /// Returns a list containing sequential pairs of code space beginning and endings:
        /// (begincodespacerange1, endcodespacerange1, begincodespacerange2, endcodespacerange1, ...)
        /// </summary>
        /// <returns>
        /// list of
        /// <c>byte[]</c>
        /// that contain code space ranges
        /// </returns>
        public virtual IList<byte[]> GetCodeSpaceRanges() {
            return codeSpaceRanges;
        }

        internal override void AddCodeSpaceRange(byte[] low, byte[] high) {
            codeSpaceRanges.Add(low);
            codeSpaceRanges.Add(high);
        }
    }
}
