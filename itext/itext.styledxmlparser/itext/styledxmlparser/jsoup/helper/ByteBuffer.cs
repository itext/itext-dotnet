/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
