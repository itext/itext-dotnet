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

namespace iText.Kernel.XMP.Impl {
    /// <since>12.10.2006</since>
    public class Latin1Converter {
        private const int STATE_START = 0;

        private const int STATE_UTF8CHAR = 11;

        /// <summary>Private constructor</summary>
        private Latin1Converter() {
        }

        // EMPTY
        /// <summary>A converter that processes a byte buffer containing a mix of UTF8 and Latin-1/Cp1252 chars.</summary>
        /// <remarks>
        /// A converter that processes a byte buffer containing a mix of UTF8 and Latin-1/Cp1252 chars.
        /// The result is a buffer where those chars have been converted to UTF-8;
        /// that means it contains only valid UTF-8 chars.
        /// <para />
        /// <em>Explanation of the processing:</em> First the encoding of the buffer is detected looking
        /// at the first four bytes (that works only if the buffer starts with an ASCII-char,
        /// like xmls '&lt;'). UTF-16/32 flavours do not require further processing.
        /// <para />
        /// In the case, UTF-8 is detected, it assumes wrong UTF8 chars to be a sequence of
        /// Latin-1/Cp1252 encoded bytes and converts the chars to their corresponding UTF-8 byte
        /// sequence.
        /// <para />
        /// The 0x80..0x9F range is undefined in Latin-1, but is defined in Windows code
        /// page 1252. The bytes 0x81, 0x8D, 0x8F, 0x90, and 0x9D are formally undefined
        /// by Windows 1252. These are in XML's RestrictedChar set, so we map them to a
        /// space.
        /// <para />
        /// The official Latin-1 characters in the range 0xA0..0xFF are converted into
        /// the Unicode Latin Supplement range U+00A0 - U+00FF.
        /// <para />
        /// <em>Example:</em> If an Euro-symbol (€) appears in the byte buffer (0xE2, 0x82, 0xAC),
        /// it will be left as is. But if only the first two bytes are appearing,
        /// followed by an ASCII char a (0xE2 - 0x82 - 0x41), it will be converted to
        /// 0xC3, 0xA2 (â) - 0xE2, 0x80, 0x9A (‚) - 0x41 (a).
        /// </remarks>
        /// <param name="buffer">a byte buffer contain</param>
        /// <returns>Returns a new buffer containing valid UTF-8</returns>
        public static ByteBuffer Convert(ByteBuffer buffer) {
            if ("UTF-8".Equals(buffer.GetEncoding())) {
                // the buffer containing one UTF-8 char (up to 8 bytes) 
                byte[] readAheadBuffer = new byte[8];
                // the number of bytes read ahead.
                int readAhead = 0;
                // expected UTF8 bytesto come
                int expectedBytes = 0;
                // output buffer with estimated length
                ByteBuffer @out = new ByteBuffer(buffer.Length() * 4 / 3);
                int state = STATE_START;
                for (int i = 0; i < buffer.Length(); i++) {
                    int b = buffer.CharAt(i);
                    switch (state) {
                        case STATE_START:
                        default: {
                            if (b < 0x7F) {
                                @out.Append((byte)b);
                            }
                            else {
                                if (b >= 0xC0) {
                                    // start of UTF8 sequence
                                    expectedBytes = -1;
                                    int test = b;
                                    for (; expectedBytes < 8 && (test & 0x80) == 0x80; test = test << 1) {
                                        expectedBytes++;
                                    }
                                    readAheadBuffer[readAhead++] = (byte)b;
                                    state = STATE_UTF8CHAR;
                                }
                                else {
                                    //  implicitly:  b >= 0x80  &&  b < 0xC0
                                    // invalid UTF8 start char, assume to be Latin-1
                                    byte[] utf8 = ConvertToUTF8((byte)b);
                                    @out.Append(utf8);
                                }
                            }
                            break;
                        }

                        case STATE_UTF8CHAR: {
                            if (expectedBytes > 0 && (b & 0xC0) == 0x80) {
                                // valid UTF8 char, add to readAheadBuffer
                                readAheadBuffer[readAhead++] = (byte)b;
                                expectedBytes--;
                                if (expectedBytes == 0) {
                                    @out.Append(readAheadBuffer, 0, readAhead);
                                    readAhead = 0;
                                    state = STATE_START;
                                }
                            }
                            else {
                                // invalid UTF8 char: 
                                // 1. convert first of seq to UTF8 
                                byte[] utf8 = ConvertToUTF8(readAheadBuffer[0]);
                                @out.Append(utf8);
                                // 2. continue processing at second byte of sequence
                                i = i - readAhead;
                                readAhead = 0;
                                state = STATE_START;
                            }
                            break;
                        }
                    }
                }
                // loop ends with "half" Utf8 char --> assume that the bytes are Latin-1
                if (state == STATE_UTF8CHAR) {
                    for (int j = 0; j < readAhead; j++) {
                        byte b = readAheadBuffer[j];
                        byte[] utf8 = ConvertToUTF8(b);
                        @out.Append(utf8);
                    }
                }
                return @out;
            }
            else {
                // Latin-1 fixing applies only to UTF-8
                return buffer;
            }
        }

        /// <summary>
        /// Converts a Cp1252 char (contains all Latin-1 chars above 0x80) into a
        /// UTF-8 byte sequence.
        /// </summary>
        /// <remarks>
        /// Converts a Cp1252 char (contains all Latin-1 chars above 0x80) into a
        /// UTF-8 byte sequence. The bytes 0x81, 0x8D, 0x8F, 0x90, and 0x9D are
        /// formally undefined by Windows 1252 and therefore replaced by a space
        /// (0x20).
        /// </remarks>
        /// <param name="ch">an Cp1252 / Latin-1 byte</param>
        /// <returns>Returns a byte array containing a UTF-8 byte sequence.</returns>
        private static byte[] ConvertToUTF8(byte ch) {
            int c = ch & 0xFF;
            try {
                if (c >= 0x80) {
                    if (c == 0x81 || c == 0x8D || c == 0x8F || c == 0x90 || c == 0x9D) {
                        // space for undefined
                        return new byte[] { 0x20 };
                    }
                    // interpret byte as Windows Cp1252 char
                    return iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { ch }, "cp1252").GetBytes("UTF-8");
                }
            }
            catch (ArgumentException) {
            }
            // EMPTY
            return new byte[] { ch };
        }
    }
}
