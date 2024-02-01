/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Text;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>A character queue with parsing helpers.</summary>
    public class TokenQueue {
        private String queue;

        private int pos = 0;

        private const char ESC = '\\';

        // escape char for chomp balanced.
        /// <summary>Create a new TokenQueue.</summary>
        /// <param name="data">string of data to back queue.</param>
        public TokenQueue(String data) {
            Validate.NotNull(data);
            queue = data;
        }

        /// <summary>Is the queue empty?</summary>
        /// <returns>true if no data left in queue.</returns>
        public virtual bool IsEmpty() {
            return RemainingLength() == 0;
        }

        private int RemainingLength() {
            return queue.Length - pos;
        }

        /// <summary>Retrieves but does not remove the first character from the queue.</summary>
        /// <returns>First character, or 0 if empty.</returns>
        public virtual char Peek() {
            return IsEmpty() ? '\u0000' : queue[pos];
        }

        /// <summary>Add a character to the start of the queue (will be the next character retrieved).</summary>
        /// <param name="c">character to add</param>
        public virtual void AddFirst(char? c) {
            AddFirst(c.ToString());
        }

        /// <summary>Add a string to the start of the queue.</summary>
        /// <param name="seq">string to add.</param>
        public virtual void AddFirst(String seq) {
            // not very performant, but an edge case
            queue = seq + queue.Substring(pos);
            pos = 0;
        }

        /// <summary>Tests if the next characters on the queue match the sequence.</summary>
        /// <remarks>Tests if the next characters on the queue match the sequence. Case insensitive.</remarks>
        /// <param name="seq">String to check queue for.</param>
        /// <returns>true if the next characters match.</returns>
        public virtual bool Matches(String seq) {
            return queue.RegionMatches(true, pos, seq, 0, seq.Length);
        }

        /// <summary>Case sensitive match test.</summary>
        /// <param name="seq">string to case sensitively check for</param>
        /// <returns>true if matched, false if not</returns>
        public virtual bool MatchesCS(String seq) {
            return queue.StartsWith(seq, pos);
        }

        /// <summary>Tests if the next characters match any of the sequences.</summary>
        /// <remarks>Tests if the next characters match any of the sequences. Case insensitive.</remarks>
        /// <param name="seq">list of strings to case insensitively check for</param>
        /// <returns>true of any matched, false if none did</returns>
        public virtual bool MatchesAny(params String[] seq) {
            foreach (String s in seq) {
                if (Matches(s)) {
                    return true;
                }
            }
            return false;
        }

        public virtual bool MatchesAny(params char[] seq) {
            if (IsEmpty()) {
                return false;
            }
            foreach (char c in seq) {
                if (queue[pos] == c) {
                    return true;
                }
            }
            return false;
        }

        public virtual bool MatchesStartTag() {
            // micro opt for matching "<x"
            return (RemainingLength() >= 2 && queue[pos] == '<' && char.IsLetter(queue[pos + 1]));
        }

        /// <summary>
        /// Tests if the queue matches the sequence (as with match), and if they do, removes the matched string from the
        /// queue.
        /// </summary>
        /// <param name="seq">String to search for, and if found, remove from queue.</param>
        /// <returns>true if found and removed, false if not found.</returns>
        public virtual bool MatchChomp(String seq) {
            if (Matches(seq)) {
                pos += seq.Length;
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>Tests if queue starts with a whitespace character.</summary>
        /// <returns>if starts with whitespace</returns>
        public virtual bool MatchesWhitespace() {
            return !IsEmpty() && iText.StyledXmlParser.Jsoup.Internal.StringUtil.IsWhitespace(queue[pos]);
        }

        /// <summary>Test if the queue matches a word character (letter or digit).</summary>
        /// <returns>if matches a word character</returns>
        public virtual bool MatchesWord() {
            return !IsEmpty() && char.IsLetterOrDigit(queue[pos]);
        }

        /// <summary>Drops the next character off the queue.</summary>
        public virtual void Advance() {
            if (!IsEmpty()) {
                pos++;
            }
        }

        /// <summary>Consume one character off queue.</summary>
        /// <returns>first character on queue.</returns>
        public virtual char Consume() {
            return queue[pos++];
        }

        /// <summary>Consumes the supplied sequence of the queue.</summary>
        /// <remarks>
        /// Consumes the supplied sequence of the queue. If the queue does not start with the supplied sequence, will
        /// throw an illegal state exception -- but you should be running match() against that condition.
        /// <para />
        /// Case insensitive.
        /// </remarks>
        /// <param name="seq">sequence to remove from head of queue.</param>
        public virtual void Consume(String seq) {
            if (!Matches(seq)) {
                throw new InvalidOperationException("Queue did not match expected sequence");
            }
            int len = seq.Length;
            if (len > RemainingLength()) {
                throw new InvalidOperationException("Queue not long enough to consume sequence");
            }
            pos += len;
        }

        /// <summary>Pulls a string off the queue, up to but exclusive of the match sequence, or to the queue running out.
        ///     </summary>
        /// <param name="seq">String to end on (and not include in return, but leave on queue). <b>Case sensitive.</b>
        ///     </param>
        /// <returns>The matched data consumed from queue.</returns>
        public virtual String ConsumeTo(String seq) {
            int offset = queue.IndexOf(seq, pos);
            if (offset != -1) {
                String consumed = queue.JSubstring(pos, offset);
                pos += consumed.Length;
                return consumed;
            }
            else {
                return Remainder();
            }
        }

        public virtual String ConsumeToIgnoreCase(String seq) {
            int start = pos;
            String first = seq.JSubstring(0, 1);
            bool canScan = first.ToLowerInvariant().Equals(first.ToUpperInvariant());
            // if first is not cased, use index of
            while (!IsEmpty()) {
                if (Matches(seq)) {
                    break;
                }
                if (canScan) {
                    int skip = queue.IndexOf(first, pos) - pos;
                    if (skip == 0) {
                        // this char is the skip char, but not match, so force advance of pos
                        pos++;
                    }
                    else {
                        if (skip < 0) {
                            // no chance of finding, grab to end
                            pos = queue.Length;
                        }
                        else {
                            pos += skip;
                        }
                    }
                }
                else {
                    pos++;
                }
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Consumes to the first sequence provided, or to the end of the queue.</summary>
        /// <remarks>Consumes to the first sequence provided, or to the end of the queue. Leaves the terminator on the queue.
        ///     </remarks>
        /// <param name="seq">any number of terminators to consume to. <b>Case insensitive.</b></param>
        /// <returns>consumed string</returns>
        public virtual String ConsumeToAny(params String[] seq) {
            // is is a case sensitive time...
            int start = pos;
            while (!IsEmpty() && !MatchesAny(seq)) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Pulls a string off the queue (like consumeTo), and then pulls off the matched string (but does not return it).
        ///     </summary>
        /// <remarks>
        /// Pulls a string off the queue (like consumeTo), and then pulls off the matched string (but does not return it).
        /// <para />
        /// If the queue runs out of characters before finding the seq, will return as much as it can (and queue will go
        /// isEmpty() == true).
        /// </remarks>
        /// <param name="seq">String to match up to, and not include in return, and to pull off queue. <b>Case sensitive.</b>
        ///     </param>
        /// <returns>Data matched from queue.</returns>
        public virtual String ChompTo(String seq) {
            String data = ConsumeTo(seq);
            MatchChomp(seq);
            return data;
        }

        public virtual String ChompToIgnoreCase(String seq) {
            String data = ConsumeToIgnoreCase(seq);
            // case insensitive scan
            MatchChomp(seq);
            return data;
        }

        /// <summary>Pulls a balanced string off the queue.</summary>
        /// <remarks>
        /// Pulls a balanced string off the queue. E.g. if queue is "(one (two) three) four", (,) will return "one (two) three",
        /// and leave " four" on the queue. Unbalanced openers and closers can be quoted (with ' or ") or escaped (with \). Those escapes will be left
        /// in the returned string, which is suitable for regexes (where we need to preserve the escape), but unsuitable for
        /// contains text strings; use unescape for that.
        /// </remarks>
        /// <param name="open">opener</param>
        /// <param name="close">closer</param>
        /// <returns>data matched from the queue</returns>
        public virtual String ChompBalanced(char open, char close) {
            int start = -1;
            int end = -1;
            int depth = 0;
            char last = '\u0000';
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            do {
                if (IsEmpty()) {
                    break;
                }
                char c = Consume();
                if (last != ESC) {
                    if (c == '\'' && c != open && !inDoubleQuote) {
                        inSingleQuote = !inSingleQuote;
                    }
                    else {
                        if (c == '"' && c != open && !inSingleQuote) {
                            inDoubleQuote = !inDoubleQuote;
                        }
                    }
                    if (inSingleQuote || inDoubleQuote) {
                        continue;
                    }
                    if (c == open) {
                        depth++;
                        if (start == -1) {
                            start = pos;
                        }
                    }
                    else {
                        if (c == close) {
                            depth--;
                        }
                    }
                }
                if (depth > 0 && last != 0) {
                    end = pos;
                }
                // don't include the outer match pair in the return
                last = c;
            }
            while (depth > 0);
            String @out = (end >= 0) ? queue.JSubstring(start, end) : "";
            if (depth > 0) {
                // ran out of queue before seeing enough )
                Validate.Fail("Did not find balanced marker at '" + @out + "'");
            }
            return @out;
        }

        /// <summary>Unescape a \ escaped string.</summary>
        /// <param name="in">backslash escaped string</param>
        /// <returns>unescaped string</returns>
        public static String Unescape(String @in) {
            StringBuilder @out = iText.StyledXmlParser.Jsoup.Internal.StringUtil.BorrowBuilder();
            char last = '\u0000';
            foreach (char c in @in.ToCharArray()) {
                if (c == ESC) {
                    if (last == ESC) {
                        @out.Append(c);
                    }
                }
                else {
                    @out.Append(c);
                }
                last = c;
            }
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.ReleaseBuilder(@out);
        }

        /// <summary>Pulls the next run of whitespace characters of the queue.</summary>
        /// <returns>Whether consuming whitespace or not</returns>
        public virtual bool ConsumeWhitespace() {
            bool seen = false;
            while (MatchesWhitespace()) {
                pos++;
                seen = true;
            }
            return seen;
        }

        /// <summary>Retrieves the next run of word type (letter or digit) off the queue.</summary>
        /// <returns>String of word characters from queue, or empty string if none.</returns>
        public virtual String ConsumeWord() {
            int start = pos;
            while (MatchesWord()) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Consume an tag name off the queue (word or :, _, -)</summary>
        /// <returns>tag name</returns>
        public virtual String ConsumeTagName() {
            int start = pos;
            while (!IsEmpty() && (MatchesWord() || MatchesAny(':', '_', '-'))) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Consume a CSS element selector (tag name, but | instead of : for namespaces (or *| for wildcard namespace), to not conflict with :pseudo selects).
        ///     </summary>
        /// <returns>tag name</returns>
        public virtual String ConsumeElementSelector() {
            int start = pos;
            while (!IsEmpty() && (MatchesWord() || MatchesAny("*|", "|", "_", "-"))) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>
        /// Consume a CSS identifier (ID or class) off the queue (letter, digit, -, _)
        /// http://www.w3.org/TR/CSS2/syndata.html#value-def-identifier
        /// </summary>
        /// <returns>identifier</returns>
        public virtual String ConsumeCssIdentifier() {
            int start = pos;
            while (!IsEmpty() && (MatchesWord() || MatchesAny('-', '_'))) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Consume an attribute key off the queue (letter, digit, -, _, :")</summary>
        /// <returns>attribute key</returns>
        public virtual String ConsumeAttributeKey() {
            int start = pos;
            while (!IsEmpty() && (MatchesWord() || MatchesAny('-', '_', ':'))) {
                pos++;
            }
            return queue.JSubstring(start, pos);
        }

        /// <summary>Consume and return whatever is left on the queue.</summary>
        /// <returns>remained of queue.</returns>
        public virtual String Remainder() {
            String remainder = queue.Substring(pos);
            pos = queue.Length;
            return remainder;
        }

        public override String ToString() {
            return queue.Substring(pos);
        }
    }
}
