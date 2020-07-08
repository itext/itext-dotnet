//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.IO;

namespace iText.Kernel.XMP.Impl {
    /// <summary>Byte buffer container including length of valid data.</summary>
    /// <since>11.10.2006</since>
    public class ByteBuffer {
        private byte[] buffer;

        private int length;

        private String encoding = null;

        /// <param name="initialCapacity">the initial capacity for this buffer</param>
        public ByteBuffer(int initialCapacity) {
            this.buffer = new byte[initialCapacity];
            this.length = 0;
        }

        /// <param name="buffer">a byte array that will be wrapped with <c>ByteBuffer</c>.</param>
        public ByteBuffer(byte[] buffer) {
            this.buffer = buffer;
            this.length = buffer.Length;
        }

        /// <param name="buffer">a byte array that will be wrapped with <c>ByteBuffer</c>.</param>
        /// <param name="length">the length of valid bytes in the array</param>
        public ByteBuffer(byte[] buffer, int length) {
            if (length > buffer.Length) {
                throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
            }
            this.buffer = buffer;
            this.length = length;
        }

        /// <summary>Loads the stream into a buffer.</summary>
        /// <param name="in">an InputStream</param>
        public ByteBuffer(Stream @in) {
            // load stream into buffer
            int chunk = 16384;
            this.length = 0;
            this.buffer = new byte[chunk];
            int read;
            while ((read = @in.JRead(this.buffer, this.length, chunk)) > 0) {
                this.length += read;
                if (read == chunk) {
                    EnsureCapacity(length + chunk);
                }
                else {
                    break;
                }
            }
        }

        /// <param name="buffer">a byte array that will be wrapped with <c>ByteBuffer</c>.</param>
        /// <param name="offset">the offset of the provided buffer.</param>
        /// <param name="length">the length of valid bytes in the array</param>
        public ByteBuffer(byte[] buffer, int offset, int length) {
            if (length > buffer.Length - offset) {
                throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
            }
            this.buffer = new byte[length];
            Array.Copy(buffer, offset, this.buffer, 0, length);
            this.length = length;
        }

        /// <returns>Returns a byte stream that is limited to the valid amount of bytes.</returns>
        public virtual Stream GetByteStream() {
            return new MemoryStream(buffer, 0, length);
        }

        /// <returns>
        /// Returns the length, that means the number of valid bytes, of the buffer;
        /// the inner byte array might be bigger than that.
        /// </returns>
        public virtual int Length() {
            return length;
        }

        //	/**
        //	 * <em>Note:</em> Only the byte up to length are valid!
        //	 * @return Returns the inner byte buffer.
        //	 */
        //	public byte[] getBuffer()
        //	{
        //		return buffer;
        //	}
        /// <param name="index">the index to retrieve the byte from</param>
        /// <returns>Returns a byte from the buffer</returns>
        public virtual byte ByteAt(int index) {
            if (index < length) {
                return buffer[index];
            }
            else {
                throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
            }
        }

        /// <param name="index">the index to retrieve a byte as int or char.</param>
        /// <returns>Returns a byte from the buffer</returns>
        public virtual int CharAt(int index) {
            if (index < length) {
                return buffer[index] & 0xFF;
            }
            else {
                throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
            }
        }

        /// <summary>Appends a byte to the buffer.</summary>
        /// <param name="b">a byte</param>
        public virtual void Append(byte b) {
            EnsureCapacity(length + 1);
            buffer[length++] = b;
        }

        /// <summary>Appends a part of byte array to the buffer.</summary>
        /// <remarks>
        /// Appends a part of byte array to the buffer. Elements on positions
        /// <paramref name="offset"/>
        /// through
        /// <c>offset+len-1</c>
        /// of provided array will be copied
        /// </remarks>
        /// <param name="bytes">a byte array</param>
        /// <param name="offset">is a position of the first element to copy</param>
        /// <param name="len">the number of array elements to be added</param>
        public virtual void Append(byte[] bytes, int offset, int len) {
            EnsureCapacity(length + len);
            Array.Copy(bytes, offset, buffer, length, len);
            length += len;
        }

        /// <summary>Append a byte array to the buffer</summary>
        /// <param name="bytes">a byte array</param>
        public virtual void Append(byte[] bytes) {
            Append(bytes, 0, bytes.Length);
        }

        /// <summary>Append another buffer to this buffer.</summary>
        /// <param name="anotherBuffer">another <c>ByteBuffer</c></param>
        public virtual void Append(iText.Kernel.XMP.Impl.ByteBuffer anotherBuffer) {
            Append(anotherBuffer.buffer, 0, anotherBuffer.length);
        }

        /// <summary>Detects the encoding of the byte buffer, stores and returns it.</summary>
        /// <remarks>
        /// Detects the encoding of the byte buffer, stores and returns it.
        /// Only UTF-8, UTF-16LE/BE and UTF-32LE/BE are recognized.
        /// <em>Note:</em> UTF-32 flavors are not supported by Java, the XML-parser will complain.
        /// </remarks>
        /// <returns>Returns the encoding string.</returns>
        public virtual String GetEncoding() {
            if (encoding == null) {
                // needs four byte at maximum to determine encoding
                if (length < 2) {
                    // only one byte length must be UTF-8
                    encoding = "UTF-8";
                }
                else {
                    if (buffer[0] == 0) {
                        // These cases are:
                        //   00 nn -- -- - Big endian UTF-16
                        //   00 00 00 nn - Big endian UTF-32
                        //   00 00 FE FF - Big endian UTF 32
                        if (length < 4 || buffer[1] != 0) {
                            encoding = "UTF-16BE";
                        }
                        else {
                            if ((buffer[2] & 0xFF) == 0xFE && (buffer[3] & 0xFF) == 0xFF) {
                                encoding = "UTF-32BE";
                            }
                            else {
                                encoding = "UTF-32";
                            }
                        }
                    }
                    else {
                        if ((buffer[0] & 0xFF) < 0x80) {
                            // These cases are:
                            //   nn mm -- -- - UTF-8, includes EF BB BF case
                            //   nn 00 -- -- - Little endian UTF-16
                            if (buffer[1] != 0) {
                                encoding = "UTF-8";
                            }
                            else {
                                if (length < 4 || buffer[2] != 0) {
                                    encoding = "UTF-16LE";
                                }
                                else {
                                    encoding = "UTF-32LE";
                                }
                            }
                        }
                        else {
                            // These cases are:
                            //   EF BB BF -- - UTF-8
                            //   FE FF -- -- - Big endian UTF-16
                            //   FF FE 00 00 - Little endian UTF-32
                            //   FF FE -- -- - Little endian UTF-16
                            if ((buffer[0] & 0xFF) == 0xEF) {
                                encoding = "UTF-8";
                            }
                            else {
                                if ((buffer[0] & 0xFF) == 0xFE) {
                                    encoding = "UTF-16";
                                }
                                else {
                                    // in fact BE 
                                    if (length < 4 || buffer[2] != 0) {
                                        encoding = "UTF-16";
                                    }
                                    else {
                                        // in fact LE
                                        encoding = "UTF-32";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // in fact LE
            return encoding;
        }

        /// <summary>
        /// Ensures the requested capacity by increasing the buffer size when the
        /// current length is exceeded.
        /// </summary>
        /// <param name="requestedLength">requested new buffer length</param>
        private void EnsureCapacity(int requestedLength) {
            if (requestedLength > buffer.Length) {
                byte[] oldBuf = buffer;
                buffer = new byte[oldBuf.Length * 2];
                Array.Copy(oldBuf, 0, buffer, 0, oldBuf.Length);
            }
        }
    }
}
