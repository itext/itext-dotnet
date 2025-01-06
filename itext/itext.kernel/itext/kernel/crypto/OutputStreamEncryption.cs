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
