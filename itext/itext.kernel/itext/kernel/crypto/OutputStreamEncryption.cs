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

using System.IO;

namespace iText.Kernel.Crypto {
	public abstract class OutputStreamEncryption : Stream {
		protected internal Stream @out;

		private byte[] sb = new byte[1];

        /// <summary>
        /// Creates a new instance of <see cref="OutputStreamEncryption"/>
        /// </summary>
        /// <param name="out">the target <see cref="Stream"/> to write encrypted content to</param>
        protected internal OutputStreamEncryption(Stream @out) {
			this.@out = @out;
		}

		/// <summary>
		/// Closes this output stream and releases any system resources
		/// associated with this stream.
		/// </summary>
		/// <remarks>
		/// Closes this output stream and releases any system resources
		/// associated with this stream. The general contract of
		/// <c>close</c>
		/// is that it closes the output stream. A closed stream cannot perform
		/// output operations and cannot be reopened.
		/// <p>
		/// The
		/// <c>close</c>
		/// method of
		/// <c>OutputStream</c>
		/// does nothing.
		/// </remarks>
	    protected override void Dispose(bool disposing) {
	        if (disposing) {
	            Finish();
	            @out.Dispose();
	        }
	        base.Dispose(disposing);
	    }

	    /// <summary>
		/// Flushes this output stream and forces any buffered output bytes
		/// to be written out.
		/// </summary>
		/// <remarks>
		/// Flushes this output stream and forces any buffered output bytes
		/// to be written out. The general contract of
		/// <c>flush</c>
		/// is
		/// that calling it is an indication that, if any bytes previously
		/// written have been buffered by the implementation of the output
		/// stream, such bytes should immediately be written to their
		/// intended destination.
		/// <p>
		/// The
		/// <c>flush</c>
		/// method of
		/// <c>OutputStream</c>
		/// does nothing.
		/// </remarks>
		public override void Flush() {
			@out.Flush();
		}

		/// <summary>
		/// Writes
		/// <c>b.length</c>
		/// bytes from the specified byte array
		/// to this output stream. The general contract for
		/// <c>write(b)</c>
		/// is that it should have exactly the same effect as the call
		/// <c>write(b, 0, b.length)</c>
		/// .
		/// </summary>
		/// <param name="b">the data.</param>
		/// <seealso cref="System.IO.Stream.Write(byte[], int, int)"/>
		public virtual void Write(byte[] b) {
			Write(b, 0, b.Length);
		}

		/// <summary>Writes the specified byte to this output stream.</summary>
		/// <remarks>
		/// Writes the specified byte to this output stream. The general
		/// contract for
		/// <c>write</c>
		/// is that one byte is written
		/// to the output stream. The byte to be written is the eight
		/// low-order bits of the argument
		/// <paramref name="b"/>
		/// . The 24
		/// high-order bits of
		/// <paramref name="b"/>
		/// are ignored.
		/// <p>
		/// Subclasses of
		/// <c>OutputStream</c>
		/// must provide an
		/// implementation for this method.
		/// </remarks>
		/// <param name="b">
		/// the
		/// <c>byte</c>
		/// .
		/// </param>
		public virtual void Write(int b) {
			sb[0] = (byte)b;
			Write(sb, 0, 1);
		}

	    /// <summary>
		/// Writes
		/// <paramref name="len"/>
		/// bytes from the specified byte array
		/// starting at offset
		/// <paramref name="off"/>
		/// to this output stream.
		/// The general contract for
		/// <c>write(b, off, len)</c>
		/// is that
		/// some of the bytes in the array
		/// <paramref name="b"/>
		/// are written to the
		/// output stream in order; element
		/// <c>b[off]</c>
		/// is the first
		/// byte written and
		/// <c>b[off+len-1]</c>
		/// is the last byte written
		/// by this operation.
		/// <p>
		/// The
		/// <c>write</c>
		/// method of
		/// <c>OutputStream</c>
		/// calls
		/// the write method of one argument on each of the bytes to be
		/// written out. Subclasses are encouraged to override this method and
		/// provide a more efficient implementation.
		/// <p>
		/// If
		/// <paramref name="b"/>
		/// is
		/// <see langword="null"/>
		/// , a
		/// <c>NullPointerException</c>
		/// is thrown.
		/// <p>
		/// If
		/// <paramref name="off"/>
		/// is negative, or
		/// <paramref name="len"/>
		/// is negative, or
		/// <c>off+len</c>
		/// is greater than the length of the array
		/// <paramref name="b"/>
		/// , then an <tt>IndexOutOfBoundsException</tt> is thrown.
		/// </summary>
		/// <param name="b">the data.</param>
		/// <param name="off">the start offset in the data.</param>
		/// <param name="len">the number of bytes to write.</param>
		public abstract override void Write(byte[] b, int off, int len);

	    public abstract void Finish();

        public override long Seek(long offset, SeekOrigin origin) {
            return @out.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            @out.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return @out.Read(buffer, offset, count);
        }

        public override bool CanRead {
            get { return @out.CanRead; }
        }

        public override bool CanSeek {
            get { return @out.CanSeek; }
        }

        public override bool CanWrite {
            get { return @out.CanWrite; }
        }

        public override long Length {
            get { return @out.Length; }
        }

        public override long Position {
            get { return @out.Position; }
            set { @out.Position = value; }
        }
    }
}
