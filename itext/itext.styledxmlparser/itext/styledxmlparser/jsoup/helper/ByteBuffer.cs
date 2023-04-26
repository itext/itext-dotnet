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

namespace iText.StyledXmlParser.Jsoup.Helper
{
    public class ByteBuffer {
        internal byte[] buffer;
        internal int position = 0;
        internal int mark = -1;

        private ByteBuffer(byte[] bytes) {
            buffer = bytes;
        }

        public ByteBuffer Position(int index) {
            position = index;
            if (mark >= position) {
                mark = -1;
            }
            return this;
        }

        public ByteBuffer Mark() {
            mark = position;
            return this;
        }

        public ByteBuffer Rewind() {
            position = 0;
            mark = -1;
            return this;
        }

        public byte[] Array() {
            return buffer;
        }

        private byte Get() {
            return buffer[position++];
        }

        public ByteBuffer Get(byte[] dest) {
            if (dest.Length > Remaining()) {
                throw new Exception("BufferUnderflowException");
            }
            for (int i = 0; i < dest.Length; ++i) {
                dest[i] = Get();
            }
            return this;
        }

        public ByteBuffer Peek(byte[] dest) {
            int start = position;
            Get(dest);
            return Position(start);
        }

        public int Remaining() {
            return buffer.Length - position;
        }

        public static ByteBuffer Wrap(byte[] bytes) {
            return new ByteBuffer(bytes);
        }

        public static ByteBuffer EmptyByteBuffer() {
            return new ByteBuffer(new byte[0]);
        }
    }
}
