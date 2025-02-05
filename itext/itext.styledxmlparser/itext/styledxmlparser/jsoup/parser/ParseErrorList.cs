/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>A container for ParseErrors.</summary>
    public class ParseErrorList : List<ParseError> {
        private const int INITIAL_CAPACITY = 16;

        private readonly int initialCapacity;

        private readonly int maxSize;

//\cond DO_NOT_DOCUMENT
        internal ParseErrorList(int initialCapacity, int maxSize)
            : base(initialCapacity) {
            this.initialCapacity = initialCapacity;
            this.maxSize = maxSize;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a new ParseErrorList with the same settings, but no errors in the list</summary>
        /// <param name="copy">initial and max size details to copy</param>
        internal ParseErrorList(iText.StyledXmlParser.Jsoup.Parser.ParseErrorList copy)
            : this(copy.initialCapacity, copy.maxSize) {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool CanAddError() {
            return Count < maxSize;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetMaxSize() {
            return maxSize;
        }
//\endcond

        public static iText.StyledXmlParser.Jsoup.Parser.ParseErrorList NoTracking() {
            return new iText.StyledXmlParser.Jsoup.Parser.ParseErrorList(0, 0);
        }

        public static iText.StyledXmlParser.Jsoup.Parser.ParseErrorList Tracking(int maxSize) {
            return new iText.StyledXmlParser.Jsoup.Parser.ParseErrorList(INITIAL_CAPACITY, maxSize);
        }
    }
}
