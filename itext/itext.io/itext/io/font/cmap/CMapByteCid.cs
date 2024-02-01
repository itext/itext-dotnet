/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Text;

namespace iText.IO.Font.Cmap {
    public class CMapByteCid : AbstractCMap {
        protected internal class Cursor {
            public int offset;

            public int length;

            public Cursor(int offset, int length) {
                this.offset = offset;
                this.length = length;
            }
        }

        private IList<int[]> planes = new List<int[]>();

        public CMapByteCid() {
            planes.Add(new int[256]);
        }

        internal override void AddChar(String mark, CMapObject code) {
            if (code.IsNumber()) {
                EncodeSequence(DecodeStringToByte(mark), (int)code.GetValue());
            }
        }

        /// <summary>Decode byte sequence.</summary>
        /// <param name="cidBytes">byteCodeBytes</param>
        /// <param name="offset">number of bytes to skip before starting to return chars from the sequence</param>
        /// <param name="length">number of bytes to process</param>
        /// <returns>string that contains decoded representation of the given sequence</returns>
        public virtual String DecodeSequence(byte[] cidBytes, int offset, int length) {
            StringBuilder sb = new StringBuilder();
            CMapByteCid.Cursor cursor = new CMapByteCid.Cursor(offset, length);
            int cid;
            while ((cid = DecodeSingle(cidBytes, cursor)) >= 0) {
                sb.Append((char)cid);
            }
            return sb.ToString();
        }

        protected internal virtual int DecodeSingle(byte[] cidBytes, CMapByteCid.Cursor cursor) {
            int end = cursor.offset + cursor.length;
            int currentPlane = 0;
            while (cursor.offset < end) {
                int one = cidBytes[cursor.offset++] & 0xff;
                cursor.length--;
                int[] plane = planes[currentPlane];
                int cid = plane[one];
                if ((cid & 0x8000) == 0) {
                    return cid;
                }
                else {
                    currentPlane = cid & 0x7fff;
                }
            }
            return -1;
        }

        private void EncodeSequence(byte[] seq, int cid) {
            int size = seq.Length - 1;
            int nextPlane = 0;
            for (int idx = 0; idx < size; ++idx) {
                int[] plane = planes[nextPlane];
                int one = seq[idx] & 0xff;
                int c = plane[one];
                if (c != 0 && (c & 0x8000) == 0) {
                    throw new iText.IO.Exceptions.IOException("Inconsistent mapping.");
                }
                if (c == 0) {
                    planes.Add(new int[256]);
                    c = (planes.Count - 1 | 0x8000);
                    plane[one] = c;
                }
                nextPlane = c & 0x7fff;
            }
            int[] plane_1 = planes[nextPlane];
            int one_1 = seq[size] & 0xff;
            int c_1 = plane_1[one_1];
            if ((c_1 & 0x8000) != 0) {
                throw new iText.IO.Exceptions.IOException("Inconsistent mapping.");
            }
            plane_1[one_1] = cid;
        }
    }
}
