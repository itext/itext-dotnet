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
using System.IO;
using System.Text;
using iText.Commons.Utils;

namespace iText.IO.Source {
    /// <summary>Class that is used to unify reading from random access files and arrays.</summary>
    public class RandomAccessFileOrArray {
        /// <summary>The source that backs this object</summary>
        private IRandomAccessSource byteSource;

        /// <summary>The physical location in the underlying byte source.</summary>
        private long byteSourcePosition;

        /// <summary>the pushed  back byte, if any</summary>
        private byte back;

        /// <summary>Whether there is a pushed back byte</summary>
        private bool isBack = false;

        /// <summary>Creates a RandomAccessFileOrArray that wraps the specified byte source.</summary>
        /// <remarks>
        /// Creates a RandomAccessFileOrArray that wraps the specified byte source.  The byte source will be closed when
        /// this RandomAccessFileOrArray is closed.
        /// </remarks>
        /// <param name="byteSource">the byte source to wrap</param>
        public RandomAccessFileOrArray(IRandomAccessSource byteSource) {
            this.byteSource = byteSource;
        }

        /// <summary>Creates an independent view of this object (with it's own file pointer and push back queue).</summary>
        /// <remarks>
        /// Creates an independent view of this object (with it's own file pointer and push back queue).  Closing the new object will not close this object.
        /// Closing this object will have adverse effect on the view.
        /// </remarks>
        /// <returns>the new view</returns>
        public virtual iText.IO.Source.RandomAccessFileOrArray CreateView() {
            EnsureByteSourceIsThreadSafe();
            return new iText.IO.Source.RandomAccessFileOrArray(new IndependentRandomAccessSource(byteSource));
        }

        /// <summary>Creates the view of the byte source of this object.</summary>
        /// <remarks>
        /// Creates the view of the byte source of this object. Closing the view won't affect this object.
        /// Closing source will have adverse effect on the view.
        /// </remarks>
        /// <returns>the byte source view.</returns>
        public virtual IRandomAccessSource CreateSourceView() {
            EnsureByteSourceIsThreadSafe();
            return new IndependentRandomAccessSource(byteSource);
        }

        /// <summary>Pushes a byte back.</summary>
        /// <remarks>Pushes a byte back.  The next get() will return this byte instead of the value from the underlying data source
        ///     </remarks>
        /// <param name="b">the byte to push</param>
        public virtual void PushBack(byte b) {
            back = b;
            isBack = true;
        }

        /// <summary>Reads a single byte</summary>
        /// <returns>the byte, or -1 if EOF is reached</returns>
        public virtual int Read() {
            if (isBack) {
                isBack = false;
                return back & 0xff;
            }
            return byteSource.Get(byteSourcePosition++);
        }

        /// <summary>Gets the next byte without moving current position.</summary>
        /// <returns>the next byte, or -1 if EOF is reached</returns>
        public virtual int Peek() {
            if (isBack) {
                return back & 0xff;
            }
            return byteSource.Get(byteSourcePosition);
        }

        /// <summary>
        /// Gets the next
        /// <c>buffer.length</c>
        /// bytes without moving current position.
        /// </summary>
        /// <param name="buffer">buffer to store read bytes</param>
        /// <returns>
        /// the number of read bytes. If it is less than
        /// <c>buffer.length</c>
        /// it means EOF has been reached.
        /// </returns>
        public virtual int Peek(byte[] buffer) {
            int offset = 0;
            int length = buffer.Length;
            int count = 0;
            if (isBack && length > 0) {
                buffer[offset++] = back;
                --length;
                ++count;
            }
            if (length > 0) {
                int byteSourceCount = byteSource.Get(byteSourcePosition, buffer, offset, length);
                if (byteSourceCount > 0) {
                    count += byteSourceCount;
                }
            }
            return count;
        }

        /// <summary>Reads the specified amount of bytes to the buffer applying the offset.</summary>
        /// <param name="b">destination buffer</param>
        /// <param name="off">offset at which to start storing characters</param>
        /// <param name="len">maximum number of characters to read</param>
        /// <returns>the number of bytes actually read or -1 in case of EOF</returns>
        public virtual int Read(byte[] b, int off, int len) {
            if (len == 0) {
                return 0;
            }
            int count = 0;
            if (isBack && len > 0) {
                isBack = false;
                b[off++] = back;
                --len;
                count++;
            }
            if (len > 0) {
                int byteSourceCount = byteSource.Get(byteSourcePosition, b, off, len);
                if (byteSourceCount > 0) {
                    count += byteSourceCount;
                    byteSourcePosition += byteSourceCount;
                }
            }
            if (count == 0) {
                return -1;
            }
            return count;
        }

        /// <summary>Reads the bytes to the buffer.</summary>
        /// <remarks>Reads the bytes to the buffer. This method will try to read as many bytes as the buffer can hold.
        ///     </remarks>
        /// <param name="b">the destination buffer</param>
        /// <returns>the number of bytes actually read</returns>
        public virtual int Read(byte[] b) {
            return Read(b, 0, b.Length);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ReadFully(byte[] b) {
            ReadFully(b, 0, b.Length);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ReadFully(byte[] b, int off, int len) {
            int n = 0;
            do {
                int count = Read(b, off + n, len - n);
                if (count < 0) {
                    throw new EndOfStreamException();
                }
                n += count;
            }
            while (n < len);
        }

        /// <summary>Make an attempt to skip the specified amount of bytes in source.</summary>
        /// <remarks>
        /// Make an attempt to skip the specified amount of bytes in source.
        /// However it may skip less amount of bytes. Possibly zero.
        /// </remarks>
        /// <param name="n">the number of bytes to skip</param>
        /// <returns>the actual number of bytes skipped</returns>
        public virtual long Skip(long n) {
            if (n <= 0) {
                return 0;
            }
            int adj = 0;
            if (isBack) {
                isBack = false;
                if (n == 1) {
                    return 1;
                }
                else {
                    --n;
                    adj = 1;
                }
            }
            long pos;
            long len;
            long newpos;
            pos = GetPosition();
            len = Length();
            newpos = pos + n;
            if (newpos > len) {
                newpos = len;
            }
            Seek(newpos);
            return newpos - pos + adj;
        }

        /// <summary><inheritDoc/></summary>
        public virtual int SkipBytes(int n) {
            return (int)Skip(n);
        }

        /// <summary>Closes the underlying source.</summary>
        public virtual void Close() {
            isBack = false;
            byteSource.Close();
        }

        /// <summary>Gets the total amount of bytes in the source.</summary>
        /// <returns>source's size.</returns>
        public virtual long Length() {
            return byteSource.Length();
        }

        /// <summary>Sets the current position in the source to the specified index.</summary>
        /// <param name="pos">the position to set</param>
        public virtual void Seek(long pos) {
            byteSourcePosition = pos;
            isBack = false;
        }

        /// <summary>Gets the current position of the source considering the pushed byte to the source.</summary>
        /// <returns>
        /// the index of last read byte in the source in
        /// or the index of last read byte in source - 1 in case byte was pushed.
        /// </returns>
        public virtual long GetPosition() {
            return byteSourcePosition - (isBack ? 1 : 0);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool ReadBoolean() {
            int ch = this.Read();
            if (ch < 0) {
                throw new EndOfStreamException();
            }
            return (ch != 0);
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte ReadByte() {
            int ch = this.Read();
            if (ch < 0) {
                throw new EndOfStreamException();
            }
            return (byte)(ch);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int ReadUnsignedByte() {
            int ch = this.Read();
            if (ch < 0) {
                throw new EndOfStreamException();
            }
            return ch;
        }

        /// <summary><inheritDoc/></summary>
        public virtual short ReadShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (short)((ch1 << 8) + ch2);
        }

        /// <summary>Reads a signed 16-bit number from this stream in little-endian order.</summary>
        /// <remarks>
        /// Reads a signed 16-bit number from this stream in little-endian order.
        /// The method reads two
        /// bytes from this stream, starting at the current stream pointer.
        /// If the two bytes read, in order, are
        /// <c>b1</c>
        /// and
        /// <c>b2</c>
        /// , where each of the two values is
        /// between
        /// <c>0</c>
        /// and
        /// <c>255</c>
        /// , inclusive, then the
        /// result is equal to:
        /// <blockquote><pre>
        /// (short)((b2 &lt;&lt; 8) | b1)
        /// </pre></blockquote>
        /// <para />
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next two bytes of this stream, interpreted as a signed
        /// 16-bit number.
        /// </returns>
        public short ReadShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (short)((ch2 << 8) + ch1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int ReadUnsignedShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (ch1 << 8) + ch2;
        }

        /// <summary>Reads an unsigned 16-bit number from this stream in little-endian order.</summary>
        /// <remarks>
        /// Reads an unsigned 16-bit number from this stream in little-endian order.
        /// This method reads
        /// two bytes from the stream, starting at the current stream pointer.
        /// If the bytes read, in order, are
        /// <c>b1</c>
        /// and
        /// <c>b2</c>
        /// , where
        /// <c>0 &lt;= b1, b2 &lt;= 255</c>
        /// ,
        /// then the result is equal to:
        /// <blockquote><pre>
        /// (b2 &lt;&lt; 8) | b1
        /// </pre></blockquote>
        /// <para />
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next two bytes of this stream, interpreted as an
        /// unsigned 16-bit integer.
        /// </returns>
        public int ReadUnsignedShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (ch2 << 8) + ch1;
        }

        /// <summary><inheritDoc/></summary>
        public virtual char ReadChar() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (char)((ch1 << 8) + ch2);
        }

        /// <summary>Reads a Unicode character from this stream in little-endian order.</summary>
        /// <remarks>
        /// Reads a Unicode character from this stream in little-endian order.
        /// This method reads two
        /// bytes from the stream, starting at the current stream pointer.
        /// If the bytes read, in order, are
        /// <c>b1</c>
        /// and
        /// <c>b2</c>
        /// , where
        /// <c>0 &lt;= b1, b2 &lt;= 255</c>
        /// ,
        /// then the result is equal to:
        /// <blockquote><pre>
        /// (char)((b2 &lt;&lt; 8) | b1)
        /// </pre></blockquote>
        /// <para />
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>the next two bytes of this stream as a Unicode character.</returns>
        public char ReadCharLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0) {
                throw new EndOfStreamException();
            }
            return (char)((ch2 << 8) + ch2);
        }

        /// <summary><inheritDoc/></summary>
        public virtual int ReadInt() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0) {
                throw new EndOfStreamException();
            }
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }

        /// <summary>Reads a signed 32-bit integer from this stream in little-endian order.</summary>
        /// <remarks>
        /// Reads a signed 32-bit integer from this stream in little-endian order.
        /// This method reads 4
        /// bytes from the stream, starting at the current stream pointer.
        /// If the bytes read, in order, are
        /// <c>b1</c>
        /// ,
        /// <c>b2</c>
        /// ,
        /// <c>b3</c>
        /// , and
        /// <c>b4</c>
        /// , where
        /// <c>0 &lt;= b1, b2, b3, b4 &lt;= 255</c>
        /// ,
        /// then the result is equal to:
        /// <blockquote><pre>
        /// (b4 &lt;&lt; 24) | (b3 &lt;&lt; 16) + (b2 &lt;&lt; 8) + b1
        /// </pre></blockquote>
        /// <para />
        /// This method blocks until the four bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next four bytes of this stream, interpreted as an
        /// <c>int</c>.
        /// </returns>
        public int ReadIntLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0) {
                throw new EndOfStreamException();
            }
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + ch1);
        }

        /// <summary>Reads an unsigned 32-bit integer from this stream.</summary>
        /// <remarks>
        /// Reads an unsigned 32-bit integer from this stream. This method reads 4
        /// bytes from the stream, starting at the current stream pointer.
        /// If the bytes read, in order, are
        /// <c>b1</c>
        /// ,
        /// <c>b2</c>
        /// ,
        /// <c>b3</c>
        /// , and
        /// <c>b4</c>
        /// , where
        /// <c>0 &lt;= b1, b2, b3, b4 &lt;= 255</c>
        /// ,
        /// then the result is equal to:
        /// <blockquote><pre>
        /// (b1 &lt;&lt; 24) | (b2 &lt;&lt; 16) + (b3 &lt;&lt; 8) + b4
        /// </pre></blockquote>
        /// <para />
        /// This method blocks until the four bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next four bytes of this stream, interpreted as a
        /// <c>long</c>.
        /// </returns>
        public long ReadUnsignedInt() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0) {
                throw new EndOfStreamException();
            }
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }

        public long ReadUnsignedIntLE() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0) {
                throw new EndOfStreamException();
            }
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + ch1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual long ReadLong() {
            return ((long)(ReadInt()) << 32) + (ReadInt() & 0xFFFFFFFFL);
        }

        public long ReadLongLE() {
            int i1 = ReadIntLE();
            int i2 = ReadIntLE();
            return ((long)i2 << 32) + (i1 & 0xFFFFFFFFL);
        }

        /// <summary><inheritDoc/></summary>
        public virtual float ReadFloat() {
            return JavaUtil.IntBitsToFloat(ReadInt());
        }

        public float ReadFloatLE() {
            return JavaUtil.IntBitsToFloat(ReadIntLE());
        }

        /// <summary><inheritDoc/></summary>
        public virtual double ReadDouble() {
            return JavaUtil.LongBitsToDouble(ReadLong());
        }

        public double ReadDoubleLE() {
            return JavaUtil.LongBitsToDouble(ReadLongLE());
        }

        /// <summary><inheritDoc/></summary>
        public virtual String ReadLine() {
            StringBuilder input = new StringBuilder();
            int c = -1;
            bool eol = false;
            while (!eol) {
                switch (c = Read()) {
                    case -1:
                    case '\n': {
                        eol = true;
                        break;
                    }

                    case '\r': {
                        eol = true;
                        long cur = GetPosition();
                        if ((Read()) != '\n') {
                            Seek(cur);
                        }
                        break;
                    }

                    default: {
                        input.Append((char)c);
                        break;
                    }
                }
            }
            if ((c == -1) && (input.Length == 0)) {
                return null;
            }
            return input.ToString();
        }

        /// <summary>
        /// Reads a
        /// <c>String</c>
        /// from the font file as bytes using the given encoding.
        /// </summary>
        /// <param name="length">the length of bytes to read</param>
        /// <param name="encoding">the given encoding</param>
        /// <returns>
        /// the
        /// <c>String</c>
        /// read
        /// </returns>
        public virtual String ReadString(int length, String encoding) {
            byte[] buf = new byte[length];
            ReadFully(buf);
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(buf, encoding);
        }

        private void EnsureByteSourceIsThreadSafe() {
            if (!(byteSource is ThreadSafeRandomAccessSource)) {
                byteSource = new ThreadSafeRandomAccessSource(byteSource);
            }
        }
    }
}
