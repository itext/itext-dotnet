using System;
using System.IO;

namespace iText.IO.Source {
	public class OutputStream<T> : Stream
		where T : Stream
	{
		private readonly ByteBuffer numBuffer = new ByteBuffer(32);

		protected internal Stream outputStream = null;

		protected internal long currentPos = 0;

		protected internal bool closeStream = true;

		//long=19 + max frac=6 => 26 => round to 32.
		public static bool GetHighPrecision()
		{
			return ByteUtils.HighPrecision;
		}

		public static void SetHighPrecision(bool value)
		{
			ByteUtils.HighPrecision = value;
		}

		public OutputStream(Stream outputStream)
			: base()
		{
			this.outputStream = outputStream;
		}

        /// <exception cref="System.IO.IOException"/>
        public virtual void Write(int b)
        {
            outputStream.WriteByte((byte)b);
            currentPos++;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void Write(byte[] b)
        {
            Write(b, 0, b.Length);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("You cann't set position for OutputStream");
        }

        public override void SetLength(long value)
        {
            outputStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("You cann't read from OutputStream");
        }

        /// <exception cref="System.IO.IOException"/>
        public override void Write(byte[] b, int off, int len)
        {
            outputStream.Write(b, off, len);
            currentPos += len;
        }

        public override void WriteByte(byte value)
        {
            try
            {
                Write(value);
            }
            catch (System.IO.IOException e)
            {
                throw new IOException(IOException.CannotWriteByte, e);
            }
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
	        get { return outputStream.Length; }
	    }

	    public override long Position
        {
            get { return outputStream.Position; }
            set
            {
                throw new NotSupportedException("You cann't set position for OutputStream");
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public override void Flush()
		{
			outputStream.Flush();
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Close()
		{
			if (closeStream)
			{
				outputStream.Close();
			}
		}

		public virtual T WriteLong(long value)
		{
			try
			{
				ByteUtils.GetIsoBytes(value, numBuffer.Reset());
				Write(numBuffer.GetInternalBuffer(), numBuffer.Capacity() - numBuffer.Size(), numBuffer.Size());
				return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteIntNumber, e);
			}
		}

		public virtual T WriteInteger(int value)
		{
			try
			{
				ByteUtils.GetIsoBytes(value, numBuffer.Reset());
				Write(numBuffer.GetInternalBuffer(), numBuffer.Capacity() - numBuffer.Size(), numBuffer.Size());
				return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteIntNumber, e);
			}
		}

		public virtual T WriteFloat(float value)
		{
			return WriteFloat(value, ByteUtils.HighPrecision);
		}

		public virtual T WriteFloat(float value, bool highPrecision)
		{
			return WriteDouble(value, highPrecision);
		}

		public virtual T WriteFloats(float[] value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				WriteFloat(value[i]);
				if (i < value.Length - 1)
				{
					WriteSpace();
				}
			}
			return this as T;
		}

		public virtual T WriteDouble(double value)
		{
			return WriteDouble(value, ByteUtils.HighPrecision);
		}

		public virtual T WriteDouble(double value, bool highPrecision)
		{
			try
			{
				ByteUtils.GetIsoBytes(value, numBuffer.Reset(), highPrecision);
				Write(numBuffer.GetInternalBuffer(), numBuffer.Capacity() - numBuffer.Size(), numBuffer.Size());
				return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteFloatNumber, e);
			}
		}

		public virtual T WriteByte(int value)
		{
			try
			{
                Write(value);
                return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteByte, e);
			}
		}

		public virtual T WriteSpace()
		{
			return WriteByte(' ');
		}

		public virtual T WriteNewLine()
		{
			return WriteByte('\n');
		}

		public virtual T WriteString(String value)
		{
			return WriteBytes(ByteUtils.GetIsoBytes(value));
		}

		public virtual T WriteBytes(byte[] b)
		{
			try
			{
				Write(b);
				return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteBytes, e);
			}
		}

		public virtual T WriteBytes(byte[] b, int off, int len)
		{
			try
			{
				Write(b, off, len);
				return this as T;
			}
			catch (IOException e)
			{
				throw new IOException(IOException.CannotWriteBytes, e);
			}
		}

		public virtual long GetCurrentPos()
		{
			return currentPos;
		}

		public virtual Stream GetOutputStream()
		{
			return outputStream;
		}

		public virtual bool IsCloseStream()
		{
			return closeStream;
		}

		public virtual void SetCloseStream(bool closeStream)
		{
			this.closeStream = closeStream;
		}

		public virtual void AssignBytes(byte[] bytes, int count)
		{
			if (outputStream is ByteArrayOutputStream)
			{
				((ByteArrayOutputStream)outputStream).AssignBytes(bytes, count);
				currentPos = count;
			}
			else
			{
				throw new IOException(IOException.BytesCanBeAssignedToByteArrayOutputStreamOnly);
			}
		}

		public virtual void Reset()
		{
			if (outputStream is ByteArrayOutputStream)
			{
			    outputStream.SetLength(0);
			}
			else
			{
				throw new IOException(IOException.BytesCanBeResetInByteArrayOutputStreamOnly);
			}
		}
	}
}
