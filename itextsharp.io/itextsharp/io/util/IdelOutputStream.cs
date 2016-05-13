using System;
using System.IO;

namespace iTextSharp.IO.Util {
	public class IdelOutputStream : Stream
	{
	    public override void Flush()
	    {
	        
	    }

	    public override long Seek(long offset, SeekOrigin origin)
	    {
	        throw new NotSupportedException();
	    }

	    public override void SetLength(long value)
	    {
            throw new NotSupportedException();
	    }

	    public override int Read(byte[] buffer, int offset, int count)
	    {
            throw new NotSupportedException();
	    }

	    public override void Write(byte[] buffer, int offset, int count)
	    {
	        
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
	        get { return true; }
	    }

	    public override long Length
	    {
            get { throw new NotSupportedException(); }
	    }

	    public override long Position { get; set; }
	}
}
