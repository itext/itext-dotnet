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
using System.IO;

namespace iText.IO.Source {
	/// <summary>
	/// Output stream based on <see cref="Stream"/> for which it is possible to set
	/// high precision in order to write all floats and doubles with high precision.
	/// </summary>
	/// <typeparam name="T"><see cref="Stream"/></typeparam>
	public class HighPrecisionOutputStream<T> : Stream
		where T : Stream
	{
		private readonly ByteBuffer numBuffer = new ByteBuffer(32);
		
		private bool? localHighPrecision;

		/// <summary>
		/// The stream that receives all output.
		/// </summary>
		protected Stream outputStream = null;

		private long currentPos = 0;

		private bool closeStream = true;

		//long=19 + max frac=6 => 26 => round to 32.
		/// <summary>
		/// Gets global high precision setting.
		/// </summary>
		/// <returns>global high precision setting.</returns>
		public static bool GetHighPrecision()
		{
			return ByteUtils.HighPrecision;
		}

		/// <summary>
		/// Sets global high precision setting for all <see cref="HighPrecisionOutputStream"/> instances.
		/// </summary>
		/// <param name="value">if true, all floats and double will be written with high
		/// precision in all <see cref="HighPrecisionOutputStream"/> instances.</param>
		public static void SetHighPrecision(bool value)
		{
			ByteUtils.HighPrecision = value;
		}
		
		/// <summary>
		/// Gets local high precision setting.
		/// </summary>
		/// <returns>local high precision setting.</returns>
		public bool? GetLocalHighPrecision()
		{
			return this.localHighPrecision;
		}

		/// <summary>
		/// Sets local high precision setting for the <see cref="HighPrecisionOutputStream"/>.
		/// Global setting will be overridden by this one.
		/// </summary>
		/// <param name="value">if true, all floats and double will be written with high
		/// precision in the underlying <see cref="HighPrecisionOutputStream"/>.</param>
		public void SetLocalHighPrecision(bool value)
		{
			this.localHighPrecision = value;
		}

		/// <summary>
		/// Creates a new <see cref="HighPrecisionOutputStream"/> instance.
		/// </summary>
		public HighPrecisionOutputStream()
			: base()
		{
		}
		
		/// <summary>
		/// Creates a new <see cref="HighPrecisionOutputStream"/> instance based on <see cref="Stream"/> instance.
		/// </summary>
		/// <param name="outputStream">the <see cref="HighPrecisionOutputStream"/> instance</param>
		public HighPrecisionOutputStream(Stream outputStream)
			: base()
		{
			this.outputStream = outputStream;
		}
		
		/// <summary>
		/// Creates a new <see cref="HighPrecisionOutputStream"/> instance based on
		/// <see cref="Stream"/> instance and precision setting value.
		/// </summary>
		/// <param name="outputStream">the <see cref="HighPrecisionOutputStream"/> instance</param>
		/// <param name="localHighPrecision">If true, all float and double values will be written with high precision.</param>
		public HighPrecisionOutputStream(Stream outputStream, bool localHighPrecision)
			: base()
		{
			this.outputStream = outputStream;
			this.localHighPrecision = localHighPrecision;
		}
		
		/// <summary>
		/// Writes the first 2 bytes of the integer to the stream.
		/// </summary>
		/// <param name="b">the first 2 bytes to be written</param>
        public virtual void Write(int b)
        {
            outputStream.WriteByte((byte)b);
            currentPos++;
        }

		/// <summary>
		/// Write a byte array to the stream.
		/// </summary>
		/// <param name="b">byte array to be written</param>
        public virtual void Write(byte[] b)
        {
            Write(b, 0, b.Length);
        }

		/// <summary><inheritDoc/></summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("You cann't set position for OutputStream");
        }

		/// <summary><inheritDoc/></summary>
        public override void SetLength(long value)
        {
            outputStream.SetLength(value);
        }

		/// <summary><inheritDoc/></summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("You cann't read from OutputStream");
        }

		/// <summary><inheritDoc/></summary>
        public override void Write(byte[] b, int off, int len)
        {
            outputStream.Write(b, off, len);
            currentPos += len;
        }

		/// <summary><inheritDoc/></summary>
        public override void WriteByte(byte value)
        {
            try
            {
                Write(value);
            }
            catch (System.IO.IOException e)
            {
                throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_BYTE, e);
            }
        }

		/// <summary><inheritDoc/></summary>
        public override bool CanRead
        {
            get { return false; }
        }

		/// <summary><inheritDoc/></summary>
	    public override bool CanSeek
	    {
	        get { return false; }
	    }

		/// <summary><inheritDoc/></summary>
	    public override bool CanWrite
	    {
	        get { return true; }
	    }

		/// <summary><inheritDoc/></summary>
	    public override long Length
	    {
	        get { return outputStream.Length; }
	    }

		/// <summary><inheritDoc/></summary>
	    public override long Position
        {
            get { return outputStream.Position; }
            set
            {
                throw new NotSupportedException("You cann't set position for OutputStream");
            }
        }

		/// <summary><inheritDoc/></summary>
        public override void Flush()
		{
			outputStream.Flush();
		}

		/// <summary><inheritDoc/></summary>
	    protected override void Dispose(bool disposing) {
	        if (disposing) {
	            if (closeStream)
	            {
	                outputStream.Dispose();
	            }
	        }
	        base.Dispose(disposing);
	    }

		/// <summary>
		/// Writes long to internal stream in ISO format.
		/// </summary>
		/// <param name="value">long to write</param>
		/// <returns>this stream as passed generic stream.</returns>
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
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_INT_NUMBER, e);
			}
		}

		/// <summary>
		/// Writes integer to internal stream in ISO format.
		/// </summary>
		/// <param name="value">integer to write</param>
		/// <returns>this stream as passed generic stream.</returns>
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
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_INT_NUMBER, e);
			}
		}

		/// <summary>
		/// Writes float to internal stream in ISO format with high precision according to local or global setting.
		/// </summary>
		/// <param name="value">float to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteFloat(float value)
		{
			return WriteFloat(value, localHighPrecision == null ? ByteUtils.HighPrecision : (bool)localHighPrecision);
		}

		/// <summary>
		/// Writes float to internal stream in ISO format.
		/// </summary>
		/// <param name="value">float to write</param>
		/// <param name="highPrecision">if true, value will be written with high precision.</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteFloat(float value, bool highPrecision)
		{
			return WriteDouble(value, highPrecision);
		}

		/// <summary>
		/// Writes float array to internal stream in ISO format separated by spaces.
		/// </summary>
		/// <param name="value">float array to write</param>
		/// <returns>this stream as passed generic stream.</returns>
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

		/// <summary>
		/// Writes double to internal stream in ISO format with high precision according to local or global setting.
		/// </summary>
		/// <param name="value">double to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteDouble(double value)
		{
			return WriteDouble(value, localHighPrecision == null ? ByteUtils.HighPrecision : (bool)localHighPrecision);
		}

		/// <summary>
		/// Writes double to internal stream in ISO format.
		/// </summary>
		/// <param name="value">double to write</param>
		/// <param name="highPrecision">if true, value will be written with high precision.</param>
		/// <returns>this stream as passed generic stream.</returns>
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
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_FLOAT_NUMBER, e);
			}
		}

		/// <summary>
		/// Writes byte to internal stream.
		/// </summary>
		/// <param name="value">byte value to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteByte(int value)
		{
			try
			{
                Write(value);
                return this as T;
			}
			catch (IOException e)
			{
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_BYTE, e);
			}
		}

		/// <summary>
		/// Writes space character to internal stream.
		/// </summary>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteSpace()
		{
			return WriteByte(' ');
		}

		/// <summary>
		/// Writes line feed character to internal stream.
		/// </summary>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteNewLine()
		{
			return WriteByte('\n');
		}

		/// <summary>
		/// Writes string to internal stream in ISO format.
		/// </summary>
		/// <param name="value">string to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteString(String value)
		{
			return WriteBytes(ByteUtils.GetIsoBytes(value));
		}

		/// <summary>
		/// Writes byte array to internal stream.
		/// </summary>
		/// <param name="b">byte array to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteBytes(byte[] b)
		{
			try
			{
				Write(b);
				return this as T;
			}
			catch (IOException e)
			{
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_BYTES, e);
			}
		}

		/// <summary>
		/// Writes byte array range to internal stream.
		/// </summary>
		/// <param name="b">byte array to write</param>
		/// <param name="off">start offset in the array</param>
		/// <param name="len">number of bytes to write</param>
		/// <returns>this stream as passed generic stream.</returns>
		public virtual T WriteBytes(byte[] b, int off, int len)
		{
			try
			{
				Write(b, off, len);
				return this as T;
			}
			catch (IOException e)
			{
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.CANNOT_WRITE_BYTES, e);
			}
		}

		/// <summary>
		/// Gets number of bytes written to this stream.
		/// </summary>
		/// <returns>current stream position by write operations.</returns>
		public virtual long GetCurrentPos()
		{
			return currentPos;
		}

		/// <summary>
		/// Gets underlying output stream.
		/// </summary>
		/// <returns>underlying output stream.</returns>
		public virtual Stream GetOutputStream()
		{
			return outputStream;
		}

		/// <summary>
		/// Indicates whether underlying stream will be closed when this stream is disposed.
		/// </summary>
		/// <returns>true if underlying stream will be closed; otherwise false.</returns>
		public virtual bool IsCloseStream()
		{
			return closeStream;
		}

		/// <summary>
		/// Sets whether underlying stream should be closed when this stream is disposed.
		/// </summary>
		/// <param name="closeStream">true to close underlying stream on dispose; otherwise false.</param>
		public virtual void SetCloseStream(bool closeStream)
		{
			this.closeStream = closeStream;
		}

		/// <summary>
		/// Assigns bytes directly to the underlying stream.
		/// </summary>
		/// <param name="bytes">bytes to assign</param>
		/// <param name="count">number of bytes to assign</param>
		public virtual void AssignBytes(byte[] bytes, int count)
		{
			if (outputStream is ByteArrayOutputStream)
			{
				((ByteArrayOutputStream)outputStream).AssignBytes(bytes, count);
				currentPos = count;
			}
			else
			{
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.BYTES_CAN_BE_ASSIGNED_TO_BYTE_ARRAY_OUTPUT_STREAM_ONLY);
			}
		}

		/// <summary>
		/// Resets bytes in the underlying stream.
		/// </summary>
		public virtual void Reset()
		{
			if (outputStream is ByteArrayOutputStream)
			{
			    outputStream.SetLength(0);
			}
			else
			{
				throw new iText.IO.Exceptions.IOException(iText.IO.Exceptions.IoExceptionMessageConstant.BYTES_CAN_BE_RESET_IN_BYTE_ARRAY_OUTPUT_STREAM_ONLY);
			}
		}
	}
}
