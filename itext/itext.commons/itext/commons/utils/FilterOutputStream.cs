/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;

namespace iText.Commons.Utils
{

    /// <summary>
    /// Analog of FilterOutputStream.
    /// </summary>
    public class FilterOutputStream : Stream
    {
        /// <summary>
        /// The output stream to be filtered.
        /// </summary>
        protected Stream @out;

        /// <summary>
        /// Whether the stream is closed;
        /// </summary>
        private volatile bool closed;

        /// <summary>
        /// Creates a FilterOutputStream on top of the specified output stream.
        /// </summary>
        /// <param name="out">The output stream to be assigned</param>
        public FilterOutputStream(Stream @out)
        {
            this.@out = @out;
        }

        /// <summary>
        /// Writes the specified int value to output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public virtual void Write(int value)
        {
            @out.Write(value);
        }

        /// <summary>
        /// Writes buffer.Length bytes to output stream.
        /// </summary>
        public void Write(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes len bytes from the specified byte array starting at offset off to output stream.
        /// </summary>
        public override void Write(byte[] b, int off, int len)
        {
            if (off < 0 || len < 0 || off + len > b.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
                
            for (int i = 0; i < len; i++)
            {
                Write(b[off + i]);
            }
        }

        /// <summary>
        /// Flushes output stream.
        /// </summary>
        public override void Flush()
        {
            @out.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (closed)
            {
                return;
            }

            try
            {
                Flush();
            }
            finally
            {
                @out?.Dispose();
            }

            @out = null;
            closed = true;
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return @out.CanWrite; }
        }

        public override long Length
        {
            get { return @out.Length; }
        }

        public override void SetLength(long value)
        {
            @out.SetLength(value);
        }

        public override long Position
        {
            get { return @out.Position; }
            set
            {
                throw new NotSupportedException("You can't set position for FilterOutputStream");
            }
        }

        public override int Read(byte[] buffer, int offset, int count) 
        {
            throw new NotSupportedException("You can't read from FilterOutputStream");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("You can't set position for FilterOutputStream");
        }
    }

}
