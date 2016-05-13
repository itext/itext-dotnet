using System;
using System.IO;

namespace iTextSharp.IO.Source {
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
		/// <exception cref="System.IO.IOException"/>
		public override int Read(byte[] b, int off, int len)
		{
			int count = source.Get(position, b, off, len);
            if (count == -1)
                return 0;
            position += count;
			return count;
		}

		/// <summary><inheritDoc/></summary>
		/// <exception cref="System.IO.IOException"/>
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
