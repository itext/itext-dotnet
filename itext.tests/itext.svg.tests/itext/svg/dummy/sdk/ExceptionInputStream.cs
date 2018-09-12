using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace iText.Svg.Dummy.Sdk
{
    public class ExceptionInputStream : Stream
    {
        public ExceptionInputStream()
        {
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
            get { return true; }
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position { get; set; }

        public override void Flush()
        {
            throw new IOException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new IOException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new IOException();
        }

        public override void SetLength(long value)
        {
            throw new IOException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new IOException();
        }
    }
}
