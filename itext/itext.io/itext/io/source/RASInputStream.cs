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
using System;
using System.IO;

namespace iText.IO.Source {
	/// <summary>
	/// An input stream that uses a
	/// <see cref="RandomAccessSource"/>
	/// as
	/// its underlying source.
	/// </summary>
	public class RASInputStream : Stream
	{
		/// <summary>The source.</summary>
		private readonly IRandomAccessSource source;

		/// <summary>The current position in the source.</summary>
		private long position = 0;

		/// <summary>Creates an input stream based on the source.</summary>
		/// <param name="source">The source.</param>
        public RASInputStream(IRandomAccessSource source)
		{
			this.source = source;
		}

        public IRandomAccessSource GetSource()
        {
            return source;
        }

	    public override void Flush()
	    {
	    }

	    public override long Seek(long offset, SeekOrigin origin)
	    {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.Current:
                    position += offset;
                    break;
                default:
                    position = offset + source.Length();
                    break;
            }
            return position;
	    }

	    public override void SetLength(long value)
	    {
	        throw new NotSupportedException("It is input stream.");
	    }

	    /// <summary><inheritDoc/></summary>
		public override int Read(byte[] b, int off, int len)
		{
			int count = source.Get(position, b, off, len);
            if (count == -1)
                return 0;
            position += count;
			return count;
		}

		/// <summary><inheritDoc/></summary>
		public override int ReadByte()
		{
            int c = source.Get(position);
		    if (c == -1)
		        return 0;
            position++;
            return c;
		}

	    public override void Write(byte[] buffer, int offset, int count)
	    {
            throw new NotSupportedException("It is input stream.");
	    }

	    public override bool CanRead
	    {
	        get { return true; }
	    }

	    public override bool CanSeek
	    {
	        get { return true; }
	    }

	    public override bool CanWrite
	    {
	        get { return false; }
	    }

	    public override long Length
	    {
            get { return source.Length(); }
	    }

	    public override long Position
	    {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
	    }
	}
}
