/*
 * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
 * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

using System;
using System.IO;


namespace iText.Commons.Utils {
	/// <summary>
	/// A character-stream reader that allows characters to be pushed back into the
	/// stream.
	/// 
	/// @author      Mark Reinhold
	/// @since       JDK1.1
	/// </summary>
	public class PushbackReader : FilterReader {
		/// <summary>
		/// Pushback buffer </summary>
		private char[] _buf;

		/// <summary>
		/// Current position in buffer </summary>
		private int _pos;

		/// <summary>
		/// Creates a new pushback reader with a pushback buffer of the given size.
		/// </summary>
		/// <param name="in">   The reader from which characters will be read </param>
		/// <param name="size"> The size of the pushback buffer </param>
		public PushbackReader(TextReader inp, int size)
			: base(inp) {
			if (size <= 0) {
				throw new ArgumentException("size <= 0");
			}
			_buf = new char[size];
			_pos = size;
		}

		/// <summary>
		/// Creates a new pushback reader with a one-character pushback buffer.
		/// </summary>
		/// <param name="in">  The reader from which characters will be read </param>
		public PushbackReader(TextReader inp)
			: this(inp, 1) {
		}

		/// <summary>
		/// Checks to make sure that the stream has not been closed. </summary>
		private void EnsureOpen() {
			if (_buf == null) {
				throw new IOException("Stream closed");
			}
		}

		/// <summary>
		/// Reads a single character.
		/// </summary>
		/// <returns>     The character read, or -1 if the end of the stream has been
		///             reached
		/// </returns>
		public override int Read() {
			EnsureOpen();
			if (_pos < _buf.Length) {
				return _buf[_pos++];
			}
			return base.Read();
		}

		/// <summary>
		/// Reads characters into a portion of an array.
		/// </summary>
		/// <param name="cbuf">  Destination buffer </param>
		/// <param name="off">   Offset at which to start writing characters </param>
		/// <param name="len">   Maximum number of characters to read
		/// </param>
		/// <returns>     The number of characters read, or -1 if the end of the
		///             stream has been reached
		/// </returns>
		public override int Read(char[] cbuf, int off, int len) {
			EnsureOpen();
			try {
				if (len <= 0) {
					if (len < 0) {
						throw new IndexOutOfRangeException();
					}
					if ((off < 0) || (off > cbuf.Length)) {
						throw new IndexOutOfRangeException();
					}
					return 0;
				}
				int avail = _buf.Length - _pos;
				if (avail > 0) {
					if (len < avail) {
						avail = len;
					}
					Array.Copy(_buf, _pos, cbuf, off, avail);
					_pos += avail;
					off += avail;
					len -= avail;
				}
				if (len > 0) {
					len = base.Read(cbuf, off, len);
					if (len == -1) {
						return (avail == 0) ? -1 : avail;
					}
					return avail + len;
				}
				return avail;
			}
			catch (IndexOutOfRangeException) {
				throw new IndexOutOfRangeException();
			}
		}

		/// <summary>
		/// Pushes back a single character by copying it to the front of the
		/// pushback buffer. After this method returns, the next character to be read
		/// will have the value <code>(char)c</code>.
		/// </summary>
		/// <param name="c">  The int value representing a character to be pushed back
		/// </param>
		public virtual void Unread(int c) {
			EnsureOpen();
			if (_pos == 0) {
				throw new IOException("Pushback buffer overflow");
			}
			_buf[--_pos] = (char) c;
		}

		/// <summary>
		/// Pushes back a portion of an array of characters by copying it to the
		/// front of the pushback buffer.  After this method returns, the next
		/// character to be read will have the value <code>cbuf[off]</code>, the
		/// character after that will have the value <code>cbuf[off+1]</code>, and
		/// so forth.
		/// </summary>
		/// <param name="cbuf">  Character array </param>
		/// <param name="off">   Offset of first character to push back </param>
		/// <param name="len">   Number of characters to push back
		/// </param>
		public virtual void Unread(char[] cbuf, int off, int len) {
			EnsureOpen();
			if (len > _pos) {
				throw new IOException("Pushback buffer overflow");
			}
			_pos -= len;
			Array.Copy(cbuf, off, _buf, _pos, len);
		}

		/// <summary>
		/// Pushes back an array of characters by copying it to the front of the
		/// pushback buffer.  After this method returns, the next character to be
		/// read will have the value <code>cbuf[0]</code>, the character after that
		/// will have the value <code>cbuf[1]</code>, and so forth.
		/// </summary>
		/// <param name="cbuf">  Character array to push back
		/// </param>
		public virtual void Unread(char[] cbuf) {
			Unread(cbuf, 0, cbuf.Length);
		}

		/// <summary>
		/// Closes the stream and releases any system resources associated with
		/// it. Once the stream has been closed, further read(),
		/// unread(), ready(), or skip() invocations will throw an IOException.
		/// Closing a previously closed stream has no effect.
		/// </summary>
	    protected override void Dispose(bool disposing) {
		    _buf = null;
	        base.Dispose(disposing);
	    }
	}
}
