/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>CharacterReader consumes tokens off a string.</summary>
    /// <remarks>CharacterReader consumes tokens off a string. Used internally by jsoup. API subject to changes.</remarks>
    public sealed class CharacterReader {
        internal const char EOF = '\uffff';

        private const int maxStringCacheLen = 12;

        internal const int maxBufferLen = 1024 * 32;

        // visible for testing
        internal const int readAheadLimit = (int)(maxBufferLen * 0.75);

        // visible for testing
        private const int minReadAheadLen = 1024;

        // the minimum mark length supported. No HTML entities can be larger than this.
        private char[] charBuf;

        private BufferedReader reader;

        private int bufLength;

        private int bufSplitPoint;

        private int bufPos;

        private int readerPos;

        private int bufMark = -1;

        private const int stringCacheSize = 512;

        private String[] stringCache = new String[stringCacheSize];

        // holds reused strings in this doc, to lessen garbage
        public CharacterReader(TextReader input, int sz) {
            Validate.NotNull(input);
            reader = new BufferedReader(input, maxBufferLen);
            charBuf = new char[Math.Min(sz, maxBufferLen)];
            BufferUp();
        }

        public CharacterReader(TextReader input)
            : this(input, maxBufferLen) {
        }

        public CharacterReader(String input)
            : this(new StringReader(input), input.Length) {
        }

        public void Close() {
            if (reader == null) {
                return;
            }
            try {
                reader.Close();
            }
            catch (System.IO.IOException) {
            }
            finally {
                reader = null;
                charBuf = null;
                stringCache = null;
            }
        }

        private bool readFully;

        // if the underlying stream has been completely read, no value in further buffering
        private void BufferUp() {
            if (readFully || bufPos < bufSplitPoint) {
                return;
            }
            int pos;
            int offset;
            if (bufMark != -1) {
                pos = bufMark;
                offset = bufPos - bufMark;
            }
            else {
                pos = bufPos;
                offset = 0;
            }
            try {
                long skipped = reader.Skip(pos);
                int position = reader.Position;
                int read = 0;
                while (read <= minReadAheadLen) {
                    int thisRead = reader.Read(charBuf, read, charBuf.Length - read);
                    if (thisRead == -1) {
                        readFully = true;
                    }
                    if (thisRead <= 0) {
                        break;
                    }
                    read += thisRead;
                }
                reader.Position = position;
                if (read > 0) {
                    Validate.IsTrue(skipped == pos);
                    bufLength = read;
                    readerPos += pos;
                    bufPos = offset;
                    if (bufMark != -1) {
                        bufMark = 0;
                    }
                    bufSplitPoint = Math.Min(bufLength, readAheadLimit);
                }
            }
            catch (System.IO.IOException e) {
                throw new UncheckedIOException(e);
            }
        }

        /// <summary>Gets the current cursor position in the content.</summary>
        /// <returns>current position</returns>
        public int Pos() {
            return readerPos + bufPos;
        }

        /// <summary>Tests if all the content has been read.</summary>
        /// <returns>true if nothing left to read.</returns>
        public bool IsEmpty() {
            BufferUp();
            return bufPos >= bufLength;
        }

        private bool IsEmptyNoBufferUp() {
            return bufPos >= bufLength;
        }

        /// <summary>Get the char at the current position.</summary>
        /// <returns>char</returns>
        public char Current() {
            BufferUp();
            return IsEmptyNoBufferUp() ? EOF : charBuf[bufPos];
        }

        internal char Consume() {
            BufferUp();
            char val = IsEmptyNoBufferUp() ? EOF : charBuf[bufPos];
            bufPos++;
            return val;
        }

        /// <summary>Unconsume one character (bufPos--).</summary>
        /// <remarks>Unconsume one character (bufPos--). MUST only be called directly after a consume(), and no chance of a bufferUp.
        ///     </remarks>
        internal void Unconsume() {
            if (bufPos < 1) {
                // a bug if this fires, need to trace it.
                throw new UncheckedIOException(new System.IO.IOException("No buffer left to unconsume."));
            }
            bufPos--;
        }

        /// <summary>Moves the current position by one.</summary>
        public void Advance() {
            bufPos++;
        }

        internal void Mark() {
            // make sure there is enough look ahead capacity
            if (bufLength - bufPos < minReadAheadLen) {
                bufSplitPoint = 0;
            }
            BufferUp();
            bufMark = bufPos;
        }

        internal void Unmark() {
            bufMark = -1;
        }

        internal void RewindToMark() {
            if (bufMark == -1) {
                throw new UncheckedIOException(new System.IO.IOException("Mark invalid"));
            }
            bufPos = bufMark;
            Unmark();
        }

        /// <summary>Returns the number of characters between the current position and the next instance of the input char
        ///     </summary>
        /// <param name="c">scan target</param>
        /// <returns>offset between current position and next instance of target. -1 if not found.</returns>
        internal int NextIndexOf(char c) {
            // doesn't handle scanning for surrogates
            BufferUp();
            for (int i = bufPos; i < bufLength; i++) {
                if (c == charBuf[i]) {
                    return i - bufPos;
                }
            }
            return -1;
        }

        /// <summary>Returns the number of characters between the current position and the next instance of the input sequence
        ///     </summary>
        /// <param name="seq">scan target</param>
        /// <returns>offset between current position and next instance of target. -1 if not found.</returns>
        internal int NextIndexOf(String seq) {
            BufferUp();
            // doesn't handle scanning for surrogates
            char startChar = seq[0];
            for (int offset = bufPos; offset < bufLength; offset++) {
                // scan to first instance of startchar:
                if (startChar != charBuf[offset]) {
                    while (++offset < bufLength && startChar != charBuf[offset]) {
                    }
                }
                /* empty */
                int i = offset + 1;
                int last = i + seq.Length - 1;
                if (offset < bufLength && last <= bufLength) {
                    for (int j = 1; i < last && seq[j] == charBuf[i]; i++, j++) {
                    }
                    /* empty */
                    if (i == last) {
                        // found full sequence
                        return offset - bufPos;
                    }
                }
            }
            return -1;
        }

        /// <summary>Reads characters up to the specific char.</summary>
        /// <param name="c">the delimiter</param>
        /// <returns>the chars read</returns>
        public String ConsumeTo(char c) {
            int offset = NextIndexOf(c);
            if (offset != -1) {
                String consumed = CacheString(charBuf, stringCache, bufPos, offset);
                bufPos += offset;
                return consumed;
            }
            else {
                return ConsumeToEnd();
            }
        }

        internal String ConsumeTo(String seq) {
            int offset = NextIndexOf(seq);
            if (offset != -1) {
                String consumed = CacheString(charBuf, stringCache, bufPos, offset);
                bufPos += offset;
                return consumed;
            }
            else {
                if (bufLength - bufPos < seq.Length) {
                    // nextIndexOf() did a bufferUp(), so if the buffer is shorter than the search string, we must be at EOF
                    return ConsumeToEnd();
                }
                else {
                    // the string we're looking for may be straddling a buffer boundary, so keep (length - 1) characters
                    // unread in case they contain the beginning of the search string
                    int endPos = bufLength - seq.Length + 1;
                    String consumed = CacheString(charBuf, stringCache, bufPos, endPos - bufPos);
                    bufPos = endPos;
                    return consumed;
                }
            }
        }

        /// <summary>Read characters until the first of any delimiters is found.</summary>
        /// <param name="chars">delimiters to scan for</param>
        /// <returns>characters read up to the matched delimiter.</returns>
        public String ConsumeToAny(params char[] chars) {
            BufferUp();
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            int charLen = chars.Length;
            int i;
            while (pos < remaining) {
                for (i = 0; i < charLen; i++) {
                    if (val[pos] == chars[i]) {
                        goto OUTER_break;
                    }
                }
                pos++;
OUTER_continue: ;
            }
OUTER_break: ;
            bufPos = pos;
            return pos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeToAnySorted(params char[] chars) {
            BufferUp();
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            while (pos < remaining) {
                if (JavaUtil.ArraysBinarySearch(chars, val[pos]) >= 0) {
                    break;
                }
                pos++;
            }
            bufPos = pos;
            return bufPos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeData() {
            // &, <, null
            //bufferUp(); // no need to bufferUp, just called consume()
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            while (pos < remaining) {
                switch (val[pos]) {
                    case '&':
                    case '<':
                    case TokeniserState.nullChar: {
                        goto OUTER_break;
                    }

                    default: {
                        pos++;
                        break;
                    }
                }
OUTER_continue: ;
            }
OUTER_break: ;
            bufPos = pos;
            return pos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeAttributeQuoted(bool single) {
            // null, " or ', &
            //bufferUp(); // no need to bufferUp, just called consume()
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            while (pos < remaining) {
                switch (val[pos]) {
                    case '&':
                    case TokeniserState.nullChar: {
                        goto OUTER_break;
                    }

                    case '\'': {
                        if (single) {
                            goto OUTER_break;
                        }
                        goto case '"';
                    }

                    case '"': {
                        if (!single) {
                            goto OUTER_break;
                        }
                        goto default;
                    }

                    default: {
                        pos++;
                        break;
                    }
                }
OUTER_continue: ;
            }
OUTER_break: ;
            bufPos = pos;
            return pos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeRawData() {
            // <, null
            //bufferUp(); // no need to bufferUp, just called consume()
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            while (pos < remaining) {
                switch (val[pos]) {
                    case '<':
                    case TokeniserState.nullChar: {
                        goto OUTER_break;
                    }

                    default: {
                        pos++;
                        break;
                    }
                }
OUTER_continue: ;
            }
OUTER_break: ;
            bufPos = pos;
            return pos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeTagName() {
            // '\t', '\n', '\r', '\f', ' ', '/', '>', nullChar
            // NOTE: out of spec, added '<' to fix common author bugs
            BufferUp();
            int pos = bufPos;
            int start = pos;
            int remaining = bufLength;
            char[] val = charBuf;
            while (pos < remaining) {
                switch (val[pos]) {
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\f':
                    case ' ':
                    case '/':
                    case '>':
                    case '<':
                    case TokeniserState.nullChar: {
                        goto OUTER_break;
                    }
                }
                pos++;
OUTER_continue: ;
            }
OUTER_break: ;
            bufPos = pos;
            return pos > start ? CacheString(charBuf, stringCache, start, pos - start) : "";
        }

        internal String ConsumeToEnd() {
            BufferUp();
            String data = CacheString(charBuf, stringCache, bufPos, bufLength - bufPos);
            bufPos = bufLength;
            return data;
        }

        internal String ConsumeLetterSequence() {
            BufferUp();
            int start = bufPos;
            while (bufPos < bufLength) {
                char c = charBuf[bufPos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c)) {
                    bufPos++;
                }
                else {
                    break;
                }
            }
            return CacheString(charBuf, stringCache, start, bufPos - start);
        }

        internal String ConsumeLetterThenDigitSequence() {
            BufferUp();
            int start = bufPos;
            while (bufPos < bufLength) {
                char c = charBuf[bufPos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c)) {
                    bufPos++;
                }
                else {
                    break;
                }
            }
            while (!IsEmptyNoBufferUp()) {
                char c = charBuf[bufPos];
                if (c >= '0' && c <= '9') {
                    bufPos++;
                }
                else {
                    break;
                }
            }
            return CacheString(charBuf, stringCache, start, bufPos - start);
        }

        internal String ConsumeHexSequence() {
            BufferUp();
            int start = bufPos;
            while (bufPos < bufLength) {
                char c = charBuf[bufPos];
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')) {
                    bufPos++;
                }
                else {
                    break;
                }
            }
            return CacheString(charBuf, stringCache, start, bufPos - start);
        }

        internal String ConsumeDigitSequence() {
            BufferUp();
            int start = bufPos;
            while (bufPos < bufLength) {
                char c = charBuf[bufPos];
                if (c >= '0' && c <= '9') {
                    bufPos++;
                }
                else {
                    break;
                }
            }
            return CacheString(charBuf, stringCache, start, bufPos - start);
        }

        internal bool Matches(char c) {
            return !IsEmpty() && charBuf[bufPos] == c;
        }

        internal bool Matches(String seq) {
            BufferUp();
            int scanLength = seq.Length;
            if (scanLength > bufLength - bufPos) {
                return false;
            }
            for (int offset = 0; offset < scanLength; offset++) {
                if (seq[offset] != charBuf[bufPos + offset]) {
                    return false;
                }
            }
            return true;
        }

        internal bool MatchesIgnoreCase(String seq) {
            BufferUp();
            int scanLength = seq.Length;
            if (scanLength > bufLength - bufPos) {
                return false;
            }
            for (int offset = 0; offset < scanLength; offset++) {
                char upScan = char.ToUpper(seq[offset]);
                char upTarget = char.ToUpper(charBuf[bufPos + offset]);
                if (upScan != upTarget) {
                    return false;
                }
            }
            return true;
        }

        internal bool MatchesAny(params char[] seq) {
            if (IsEmpty()) {
                return false;
            }
            BufferUp();
            char c = charBuf[bufPos];
            foreach (char seek in seq) {
                if (seek == c) {
                    return true;
                }
            }
            return false;
        }

        internal bool MatchesAnySorted(char[] seq) {
            BufferUp();
            return !IsEmpty() && JavaUtil.ArraysBinarySearch(seq, charBuf[bufPos]) >= 0;
        }

        internal bool MatchesLetter() {
            if (IsEmpty()) {
                return false;
            }
            char c = charBuf[bufPos];
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c);
        }

        internal bool MatchesDigit() {
            if (IsEmpty()) {
                return false;
            }
            char c = charBuf[bufPos];
            return (c >= '0' && c <= '9');
        }

        internal bool MatchConsume(String seq) {
            BufferUp();
            if (Matches(seq)) {
                bufPos += seq.Length;
                return true;
            }
            else {
                return false;
            }
        }

        internal bool MatchConsumeIgnoreCase(String seq) {
            if (MatchesIgnoreCase(seq)) {
                bufPos += seq.Length;
                return true;
            }
            else {
                return false;
            }
        }

        internal bool ContainsIgnoreCase(String seq) {
            // used to check presence of </title>, </style>. only finds consistent case.
            String loScan = seq.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            String hiScan = seq.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            return (NextIndexOf(loScan) > -1) || (NextIndexOf(hiScan) > -1);
        }

        public override String ToString() {
            if (bufLength - bufPos < 0) {
                return "";
            }
            return new String(charBuf, bufPos, bufLength - bufPos);
        }

        /// <summary>Caches short strings, as a flywheel pattern, to reduce GC load.</summary>
        /// <remarks>
        /// Caches short strings, as a flywheel pattern, to reduce GC load. Just for this doc, to prevent leaks.
        /// <p />
        /// Simplistic, and on hash collisions just falls back to creating a new string, vs a full HashMap with Entry list.
        /// That saves both having to create objects as hash keys, and running through the entry list, at the expense of
        /// some more duplicates.
        /// </remarks>
        private static String CacheString(char[] charBuf, String[] stringCache, int start, int count) {
            // limit (no cache):
            if (count > maxStringCacheLen) {
                return new String(charBuf, start, count);
            }
            if (count < 1) {
                return "";
            }
            // calculate hash:
            int hash = 31 * count;
            int offset = start;
            for (int i = 0; i < count; i++) {
                hash = 31 * hash + charBuf[offset++];
            }
            // get from cache
            int index = hash & stringCacheSize - 1;
            String cached = stringCache[index];
            if (cached == null) {
                // miss, add
                cached = new String(charBuf, start, count);
                stringCache[index] = cached;
            }
            else {
                // hashcode hit, check equality
                if (RangeEquals(charBuf, start, count, cached)) {
                    // hit
                    return cached;
                }
                else {
                    // hashcode conflict
                    cached = new String(charBuf, start, count);
                    stringCache[index] = cached;
                }
            }
            // update the cache, as recently used strings are more likely to show up again
            return cached;
        }

        /// <summary>Check if the value of the provided range equals the string.</summary>
        internal static bool RangeEquals(char[] charBuf, int start, int count, String cached) {
            if (count == cached.Length) {
                int i = start;
                int j = 0;
                while (count-- != 0) {
                    if (charBuf[i++] != cached[j++]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        // just used for testing
        internal bool RangeEquals(int start, int count, String cached) {
            return RangeEquals(charBuf, start, count, cached);
        }
    }
}
