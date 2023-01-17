/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace iText.IO.Font.Cmap {
    /// <author>psoares</author>
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
