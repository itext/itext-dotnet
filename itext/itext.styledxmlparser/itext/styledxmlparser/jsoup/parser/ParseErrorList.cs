/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
