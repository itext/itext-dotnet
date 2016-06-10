/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.IO;
using System.Text;

namespace iTextSharp.IO.Source
{
    public class RandomAccessFileOrArray
    {
        /// <summary>When true the file access is not done through a memory mapped file.</summary>
        /// <remarks>
        /// When true the file access is not done through a memory mapped file. Use it if the file
        /// is too big to be mapped in your address space.
        /// </remarks>
        public static bool plainRandomAccess = false;

        /// <summary>The source that backs this object</summary>
        private readonly IRandomAccessSource byteSource;

        /// <summary>The physical location in the underlying byte source.</summary>
        private long byteSourcePosition;

        /// <summary>the pushed  back byte, if any</summary>
        private byte back;

        /// <summary>Whether there is a pushed back byte</summary>
        private bool isBack = false;

        /// <summary>Creates an independent view of this object (with it's own file pointer and push back queue).</summary>
        /// <remarks>
        /// Creates an independent view of this object (with it's own file pointer and push back queue).  Closing the new object will not close this object.
        /// Closing this object will have adverse effect on the view.
        /// </remarks>
        /// <returns>the new view</returns>
        public virtual iTextSharp.IO.Source.RandomAccessFileOrArray CreateView()
        {
            return new iTextSharp.IO.Source.RandomAccessFileOrArray(new IndependentRandomAccessSource(byteSource));
        }

        public virtual IRandomAccessSource CreateSourceView()
        {
            return new IndependentRandomAccessSource(byteSource);
        }

        /// <summary>Creates a RandomAccessFileOrArray that wraps the specified byte source.</summary>
        /// <remarks>
        /// Creates a RandomAccessFileOrArray that wraps the specified byte source.  The byte source will be closed when
        /// this RandomAccessFileOrArray is closed.
        /// </remarks>
        /// <param name="byteSource">the byte source to wrap</param>
        public RandomAccessFileOrArray(IRandomAccessSource byteSource)
        {
            this.byteSource = byteSource;
        }

        /// <summary>Pushes a byte back.</summary>
        /// <remarks>Pushes a byte back.  The next get() will return this byte instead of the value from the underlying data source
        ///     </remarks>
        /// <param name="b">the byte to push</param>
        public virtual void PushBack(byte b)
        {
            back = b;
            isBack = true;
        }

        /// <summary>Reads a single byte</summary>
        /// <returns>the byte, or -1 if EOF is reached</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual int Read()
        {
            if (isBack)
            {
                isBack = false;
                return back & 0xff;
            }
            return byteSource.Get(byteSourcePosition++);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int Read(byte[] b, int off, int len)
        {
            if (len == 0)
            {
                return 0;
            }
            int count = 0;
            if (isBack && len > 0)
            {
                isBack = false;
                b[off++] = back;
                --len;
                count++;
            }
            if (len > 0)
            {
                int byteSourceCount = byteSource.Get(byteSourcePosition, b, off, len);
                if (byteSourceCount > 0)
                {
                    count += byteSourceCount;
                    byteSourcePosition += byteSourceCount;
                }
            }
            if (count == 0)
            {
                return -1;
            }
            return count;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int Read(byte[] b)
        {
            return Read(b, 0, b.Length);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void ReadFully(byte[] b, int off, int len)
        {
            int n = 0;
            do
            {
                int count = Read(b, off + n, len - n);
                if (count < 0)
                {
                    throw new EndOfStreamException();
                }
                n += count;
            }
            while (n < len);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual long Skip(long n)
        {
            if (n <= 0)
            {
                return 0;
            }
            int adj = 0;
            if (isBack)
            {
                isBack = false;
                if (n == 1)
                {
                    return 1;
                }
                else
                {
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
            if (newpos > len)
            {
                newpos = len;
            }
            Seek(newpos);
            /* return the actual number of bytes skipped */
            return newpos - pos + adj;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int SkipBytes(int n)
        {
            return (int)Skip(n);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void Close()
        {
            isBack = false;
            byteSource.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual long Length()
        {
            return byteSource.Length();
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void Seek(long pos)
        {
            byteSourcePosition = pos;
            isBack = false;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual long GetPosition()
        {
            return byteSourcePosition - (isBack ? 1 : 0);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual bool ReadBoolean()
        {
            int ch = this.Read();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return (ch != 0);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual byte ReadByte()
        {
            int ch = this.Read();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return (byte)(ch);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int ReadUnsignedByte()
        {
            int ch = this.Read();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return ch;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual short ReadShort()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
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
        /// <p>
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next two bytes of this stream, interpreted as a signed
        /// 16-bit number.
        /// </returns>
        /// <exception>
        /// EOFException
        /// if this stream reaches the end before reading
        /// two bytes.
        /// </exception>
        /// <exception>
        /// java.io.IOException
        /// if an I/O error occurs.
        /// </exception>
        /// <exception cref="System.IO.IOException"/>
        public short ReadShortLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (short)((ch2 << 8) + ch1);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int ReadUnsignedShort()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
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
        /// <p>
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next two bytes of this stream, interpreted as an
        /// unsigned 16-bit integer.
        /// </returns>
        /// <exception>
        /// EOFException
        /// if this stream reaches the end before reading
        /// two bytes.
        /// </exception>
        /// <exception>
        /// java.io.IOException
        /// if an I/O error occurs.
        /// </exception>
        /// <exception cref="System.IO.IOException"/>
        public int ReadUnsignedShortLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (ch2 << 8) + ch1;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual char ReadChar()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
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
        /// <p>
        /// This method blocks until the two bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>the next two bytes of this stream as a Unicode character.</returns>
        /// <exception>
        /// EOFException
        /// if this stream reaches the end before reading
        /// two bytes.
        /// </exception>
        /// <exception>
        /// java.io.IOException
        /// if an I/O error occurs.
        /// </exception>
        /// <exception cref="System.IO.IOException"/>
        public char ReadCharLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (char)((ch2 << 8) + ch2);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual int ReadInt()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
            {
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
        /// <p>
        /// This method blocks until the four bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next four bytes of this stream, interpreted as an
        /// <c>int</c>
        /// .
        /// </returns>
        /// <exception>
        /// EOFException
        /// if this stream reaches the end before reading
        /// four bytes.
        /// </exception>
        /// <exception>
        /// java.io.IOException
        /// if an I/O error occurs.
        /// </exception>
        /// <exception cref="System.IO.IOException"/>
        public int ReadIntLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
            {
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
        /// <p>
        /// This method blocks until the four bytes are read, the end of the
        /// stream is detected, or an exception is thrown.
        /// </remarks>
        /// <returns>
        /// the next four bytes of this stream, interpreted as a
        /// <c>long</c>
        /// .
        /// </returns>
        /// <exception>
        /// EOFException
        /// if this stream reaches the end before reading
        /// four bytes.
        /// </exception>
        /// <exception>
        /// java.io.IOException
        /// if an I/O error occurs.
        /// </exception>
        /// <exception cref="System.IO.IOException"/>
        public long ReadUnsignedInt()
        {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
            {
                throw new EndOfStreamException();
            }
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }

        /// <exception cref="System.IO.IOException"/>
        public long ReadUnsignedIntLE()
        {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
            {
                throw new EndOfStreamException();
            }
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + ch1);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual long ReadLong()
        {
            return ((long)(ReadInt()) << 32) + (ReadInt() & 0xFFFFFFFFL);
        }

        /// <exception cref="System.IO.IOException"/>
        public long ReadLongLE()
        {
            int i1 = ReadIntLE();
            int i2 = ReadIntLE();
            return ((long)i2 << 32) + (i1 & 0xFFFFFFFFL);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual float ReadFloat()
        {
            return iTextSharp.IO.Util.JavaUtil.IntBitsToFloat(ReadInt());
        }

        /// <exception cref="System.IO.IOException"/>
        public float ReadFloatLE()
        {
            return iTextSharp.IO.Util.JavaUtil.IntBitsToFloat(ReadIntLE());
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual double ReadDouble()
        {
            return iTextSharp.IO.Util.JavaUtil.LongBitsToDouble(ReadLong());
        }

        /// <exception cref="System.IO.IOException"/>
        public double ReadDoubleLE()
        {
            return iTextSharp.IO.Util.JavaUtil.LongBitsToDouble(ReadLongLE());
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual String ReadLine()
        {
            StringBuilder input = new StringBuilder();
            int c = -1;
            bool eol = false;
            while (!eol)
            {
                switch (c = Read())
                {
                    case -1:
                    case '\n':
                    {
                        eol = true;
                        break;
                    }

                    case '\r':
                    {
                        eol = true;
                        long cur = GetPosition();
                        if ((Read()) != '\n')
                        {
                            Seek(cur);
                        }
                        break;
                    }

                    default:
                    {
                        input.Append((char)c);
                        break;
                    }
                }
            }
            if ((c == -1) && (input.Length == 0))
            {
                return null;
            }
            return input.ToString();
        }

        /// <summary>
        /// Reads a
        /// <c>String</c>
        /// from the font file as bytes using the given
        /// encoding.
        /// </summary>
        /// <param name="length">the length of bytes to read</param>
        /// <param name="encoding">the given encoding</param>
        /// <returns>
        /// the
        /// <c>String</c>
        /// read
        /// </returns>
        /// <exception cref="System.IO.IOException">the font file could not be read</exception>
        public virtual String ReadString(int length, String encoding)
        {
            byte[] buf = new byte[length];
            ReadFully(buf);
            return iTextSharp.IO.Util.JavaUtil.GetStringForBytes(buf, encoding);
        }
    }
}
