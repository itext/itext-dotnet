/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>CharacterReader consumes tokens off a string.</summary>
    /// <remarks>CharacterReader consumes tokens off a string. To replace the old TokenQueue.</remarks>
    internal sealed class CharacterReader {
        internal const char EOF = '\uffff';

        private const int maxCacheLen = 12;

        private readonly char[] input;

        private readonly int length;

        private int pos = 0;

        private int mark = 0;

        private readonly String[] stringCache = new String[512];

        // holds reused strings in this doc, to lessen garbage
        internal CharacterReader(String input) {
            Validate.NotNull(input);
            this.input = input.ToCharArray();
            this.length = this.input.Length;
        }

        internal int Pos() {
            return pos;
        }

        internal bool IsEmpty() {
            return pos >= length;
        }

        internal char Current() {
            return pos >= length ? EOF : input[pos];
        }

        internal char Consume() {
            char val = pos >= length ? EOF : input[pos];
            pos++;
            return val;
        }

        internal void Unconsume() {
            pos--;
        }

        internal void Advance() {
            pos++;
        }

        internal void Mark() {
            mark = pos;
        }

        internal void RewindToMark() {
            pos = mark;
        }

        internal String ConsumeAsString() {
            return new String(input, pos++, 1);
        }

        /// <summary>Returns the number of characters between the current position and the next instance of the input char
        ///     </summary>
        /// <param name="c">scan target</param>
        /// <returns>offset between current position and next instance of target. -1 if not found.</returns>
        internal int NextIndexOf(char c) {
            // doesn't handle scanning for surrogates
            for (int i = pos; i < length; i++) {
                if (c == input[i]) {
                    return i - pos;
                }
            }
            return -1;
        }

        /// <summary>Returns the number of characters between the current position and the next instance of the input sequence
        ///     </summary>
        /// <param name="seq">scan target</param>
        /// <returns>offset between current position and next instance of target. -1 if not found.</returns>
        internal int NextIndexOf(String seq) {
            // doesn't handle scanning for surrogates
            char startChar = seq[0];
            for (int offset = pos; offset < length; offset++) {
                // scan to first instance of startchar:
                if (startChar != input[offset]) {
                    while (++offset < length && startChar != input[offset]) {
                    }
                }
                /* empty */
                int i = offset + 1;
                int last = i + seq.Length - 1;
                if (offset < length && last <= length) {
                    for (int j = 1; i < last && seq[j] == input[i]; i++, j++) {
                    }
                    /* empty */
                    if (i == last) {
                        // found full sequence
                        return offset - pos;
                    }
                }
            }
            return -1;
        }

        internal String ConsumeTo(char c) {
            int offset = NextIndexOf(c);
            if (offset != -1) {
                String consumed = CacheString(pos, offset);
                pos += offset;
                return consumed;
            }
            else {
                return ConsumeToEnd();
            }
        }

        internal String ConsumeTo(String seq) {
            int offset = NextIndexOf(seq);
            if (offset != -1) {
                String consumed = CacheString(pos, offset);
                pos += offset;
                return consumed;
            }
            else {
                return ConsumeToEnd();
            }
        }

        internal String ConsumeToAny(params char[] chars) {
            int start = pos;
            int remaining = length;
            char[] val = input;
            while (pos < remaining) {
                foreach (char c in chars) {
                    if (val[pos] == c) {
                        goto OUTER_break;
                    }
                }
                pos++;
OUTER_continue: ;
            }
OUTER_break: ;
            return pos > start ? CacheString(start, pos - start) : "";
        }

        internal String ConsumeToAnySorted(params char[] chars) {
            int start = pos;
            int remaining = length;
            char[] val = input;
            while (pos < remaining) {
                if (JavaUtil.ArraysBinarySearch(chars, val[pos]) >= 0) {
                    break;
                }
                pos++;
            }
            return pos > start ? CacheString(start, pos - start) : "";
        }

        internal String ConsumeData() {
            // &, <, null
            int start = pos;
            int remaining = length;
            char[] val = input;
            while (pos < remaining) {
                char c = val[pos];
                if (c == '&' || c == '<' || c == TokeniserState.nullChar) {
                    break;
                }
                pos++;
            }
            return pos > start ? CacheString(start, pos - start) : "";
        }

        internal String ConsumeTagName() {
            // '\t', '\n', '\r', '\f', ' ', '/', '>', nullChar
            int start = pos;
            int remaining = length;
            char[] val = input;
            while (pos < remaining) {
                char c = val[pos];
                if (c == '\t' || c == '\n' || c == '\r' || c == '\f' || c == ' ' || c == '/' || c == '>' || c == TokeniserState
                    .nullChar) {
                    break;
                }
                pos++;
            }
            return pos > start ? CacheString(start, pos - start) : "";
        }

        internal String ConsumeToEnd() {
            String data = CacheString(pos, length - pos);
            pos = length;
            return data;
        }

        internal String ConsumeLetterSequence() {
            int start = pos;
            while (pos < length) {
                char c = input[pos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c)) {
                    pos++;
                }
                else {
                    break;
                }
            }
            return CacheString(start, pos - start);
        }

        internal String ConsumeLetterThenDigitSequence() {
            int start = pos;
            while (pos < length) {
                char c = input[pos];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c)) {
                    pos++;
                }
                else {
                    break;
                }
            }
            while (!IsEmpty()) {
                char c = input[pos];
                if (c >= '0' && c <= '9') {
                    pos++;
                }
                else {
                    break;
                }
            }
            return CacheString(start, pos - start);
        }

        internal String ConsumeHexSequence() {
            int start = pos;
            while (pos < length) {
                char c = input[pos];
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')) {
                    pos++;
                }
                else {
                    break;
                }
            }
            return CacheString(start, pos - start);
        }

        internal String ConsumeDigitSequence() {
            int start = pos;
            while (pos < length) {
                char c = input[pos];
                if (c >= '0' && c <= '9') {
                    pos++;
                }
                else {
                    break;
                }
            }
            return CacheString(start, pos - start);
        }

        internal bool Matches(char c) {
            return !IsEmpty() && input[pos] == c;
        }

        internal bool Matches(String seq) {
            int scanLength = seq.Length;
            if (scanLength > length - pos) {
                return false;
            }
            for (int offset = 0; offset < scanLength; offset++) {
                if (seq[offset] != input[pos + offset]) {
                    return false;
                }
            }
            return true;
        }

        internal bool MatchesIgnoreCase(String seq) {
            int scanLength = seq.Length;
            if (scanLength > length - pos) {
                return false;
            }
            for (int offset = 0; offset < scanLength; offset++) {
                char upScan = char.ToUpper(seq[offset]);
                char upTarget = char.ToUpper(input[pos + offset]);
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
            char c = input[pos];
            foreach (char seek in seq) {
                if (seek == c) {
                    return true;
                }
            }
            return false;
        }

        internal bool MatchesAnySorted(char[] seq) {
            return !IsEmpty() && JavaUtil.ArraysBinarySearch(seq, input[pos]) >= 0;
        }

        internal bool MatchesLetter() {
            if (IsEmpty()) {
                return false;
            }
            char c = input[pos];
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || char.IsLetter(c);
        }

        internal bool MatchesDigit() {
            if (IsEmpty()) {
                return false;
            }
            char c = input[pos];
            return (c >= '0' && c <= '9');
        }

        internal bool MatchConsume(String seq) {
            if (Matches(seq)) {
                pos += seq.Length;
                return true;
            }
            else {
                return false;
            }
        }

        internal bool MatchConsumeIgnoreCase(String seq) {
            if (MatchesIgnoreCase(seq)) {
                pos += seq.Length;
                return true;
            }
            else {
                return false;
            }
        }

        internal bool ContainsIgnoreCase(String seq) {
            // used to check presence of </title>, </style>. only finds consistent case.
            String loScan = seq.ToLowerInvariant();
            String hiScan = seq.ToUpperInvariant();
            return (NextIndexOf(loScan) > -1) || (NextIndexOf(hiScan) > -1);
        }

        public override String ToString() {
            return new String(input, pos, length - pos);
        }

        /// <summary>Caches short strings, as a flywheel pattern, to reduce GC load.</summary>
        /// <remarks>
        /// Caches short strings, as a flywheel pattern, to reduce GC load. Just for this doc, to prevent leaks.
        /// <p />
        /// Simplistic, and on hash collisions just falls back to creating a new string, vs a full HashMap with Entry list.
        /// That saves both having to create objects as hash keys, and running through the entry list, at the expense of
        /// some more duplicates.
        /// </remarks>
        private String CacheString(int start, int count) {
            char[] val = input;
            String[] cache = stringCache;
            // limit (no cache):
            if (count > maxCacheLen) {
                return new String(val, start, count);
            }
            // calculate hash:
            int hash = 0;
            int offset = start;
            for (int i = 0; i < count; i++) {
                hash = 31 * hash + val[offset++];
            }
            // get from cache
            int index = hash & cache.Length - 1;
            String cached = cache[index];
            if (cached == null) {
                // miss, add
                cached = new String(val, start, count);
                cache[index] = cached;
            }
            else {
                // hashcode hit, check equality
                if (RangeEquals(start, count, cached)) {
                    // hit
                    return cached;
                }
                else {
                    // hashcode conflict
                    cached = new String(val, start, count);
                    cache[index] = cached;
                }
            }
            // update the cache, as recently used strings are more likely to show up again
            return cached;
        }

        /// <summary>Check if the value of the provided range equals the string.</summary>
        internal bool RangeEquals(int start, int count, String cached) {
            if (count == cached.Length) {
                char[] one = input;
                int i = start;
                int j = 0;
                while (count-- != 0) {
                    if (one[i++] != cached[j++]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
