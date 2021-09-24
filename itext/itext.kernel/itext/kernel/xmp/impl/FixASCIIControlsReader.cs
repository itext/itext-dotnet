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
using System.IO;
using iText.Commons.Utils;

namespace iText.Kernel.XMP.Impl {
    /// <since>22.08.2006</since>
    public class FixASCIIControlsReader : PushbackReader {
        private const int STATE_START = 0;

        private const int STATE_AMP = 1;

        private const int STATE_HASH = 2;

        private const int STATE_HEX = 3;

        private const int STATE_DIG1 = 4;

        private const int STATE_ERROR = 5;

        private const int BUFFER_SIZE = 8;

        /// <summary>the state of the automaton</summary>
        private int state = STATE_START;

        /// <summary>the result of the escaping sequence</summary>
        private int control = 0;

        /// <summary>count the digits of the sequence</summary>
        private int digits = 0;

        /// <summary>The look-ahead size is 6 at maximum (&amp;#xAB;)</summary>
        /// <seealso cref="iText.Commons.Utils.PushbackReader.PushbackReader(System.IO.TextReader, int)"/>
        /// <param name="input">a Reader</param>
        public FixASCIIControlsReader(TextReader input)
            : base(input, BUFFER_SIZE) {
        }

        /// <seealso cref="System.IO.TextReader.Read(char[], int, int)"/>
        public override int Read(char[] cbuf, int off, int len) {
            int readAhead = 0;
            int read = 0;
            int pos = off;
            char[] readAheadBuffer = new char[BUFFER_SIZE];
            bool available = true;
            while (available && read < len) {
                available = base.Read(readAheadBuffer, readAhead, 1) == 1;
                if (available) {
                    char c = ProcessChar(readAheadBuffer[readAhead]);
                    if (state == STATE_START) {
                        // replace control chars with space
                        if (iText.Kernel.XMP.Impl.Utils.IsControlChar(c)) {
                            c = ' ';
                        }
                        cbuf[pos++] = c;
                        readAhead = 0;
                        read++;
                    }
                    else {
                        if (state == STATE_ERROR) {
                            Unread(readAheadBuffer, 0, readAhead + 1);
                            readAhead = 0;
                        }
                        else {
                            readAhead++;
                        }
                    }
                }
                else {
                    if (readAhead > 0) {
                        // handles case when file ends within excaped sequence
                        Unread(readAheadBuffer, 0, readAhead);
                        state = STATE_ERROR;
                        readAhead = 0;
                        available = true;
                    }
                }
            }
            return read > 0 || available ? read : -1;
        }

        /// <summary>Processes numeric escaped chars to find out if they are a control character.</summary>
        /// <param name="ch">a char</param>
        /// <returns>Returns the char directly or as replacement for the escaped sequence.</returns>
        private char ProcessChar(char ch) {
            switch (state) {
                case STATE_START: {
                    if (ch == '&') {
                        state = STATE_AMP;
                    }
                    return ch;
                }

                case STATE_AMP: {
                    if (ch == '#') {
                        state = STATE_HASH;
                    }
                    else {
                        state = STATE_ERROR;
                    }
                    return ch;
                }

                case STATE_HASH: {
                    if (ch == 'x') {
                        control = 0;
                        digits = 0;
                        state = STATE_HEX;
                    }
                    else {
                        if ('0' <= ch && ch <= '9') {
                            control = JavaUtil.CharacterDigit(ch, 10);
                            digits = 1;
                            state = STATE_DIG1;
                        }
                        else {
                            state = STATE_ERROR;
                        }
                    }
                    return ch;
                }

                case STATE_DIG1: {
                    if ('0' <= ch && ch <= '9') {
                        control = control * 10 + JavaUtil.CharacterDigit(ch, 10);
                        digits++;
                        if (digits <= 5) {
                            state = STATE_DIG1;
                        }
                        else {
                            state = STATE_ERROR;
                        }
                    }
                    else {
                        // sequence too long
                        if (ch == ';' && iText.Kernel.XMP.Impl.Utils.IsControlChar((char)control)) {
                            state = STATE_START;
                            return (char)control;
                        }
                        else {
                            state = STATE_ERROR;
                        }
                    }
                    return ch;
                }

                case STATE_HEX: {
                    if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'f') || ('A' <= ch && ch <= 'F')) {
                        control = control * 16 + JavaUtil.CharacterDigit(ch, 16);
                        digits++;
                        if (digits <= 4) {
                            state = STATE_HEX;
                        }
                        else {
                            state = STATE_ERROR;
                        }
                    }
                    else {
                        // sequence too long
                        if (ch == ';' && iText.Kernel.XMP.Impl.Utils.IsControlChar((char)control)) {
                            state = STATE_START;
                            return (char)control;
                        }
                        else {
                            state = STATE_ERROR;
                        }
                    }
                    return ch;
                }

                case STATE_ERROR: {
                    state = STATE_START;
                    return ch;
                }

                default: {
                    // not reachable
                    return ch;
                }
            }
        }
    }
}
