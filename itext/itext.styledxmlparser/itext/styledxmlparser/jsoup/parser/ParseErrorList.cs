/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System.Collections.Generic;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>A container for ParseErrors.</summary>
    /// <author>Jonathan Hedley</author>
    internal class ParseErrorList : List<ParseError> {
        private const int INITIAL_CAPACITY = 16;

        private readonly int maxSize;

        internal ParseErrorList(int initialCapacity, int maxSize)
            : base(initialCapacity) {
            this.maxSize = maxSize;
        }

        internal virtual bool CanAddError() {
            return Count < maxSize;
        }

        internal virtual int GetMaxSize() {
            return maxSize;
        }

        internal static iText.StyledXmlParser.Jsoup.Parser.ParseErrorList NoTracking() {
            return new iText.StyledXmlParser.Jsoup.Parser.ParseErrorList(0, 0);
        }

        internal static iText.StyledXmlParser.Jsoup.Parser.ParseErrorList Tracking(int maxSize) {
            return new iText.StyledXmlParser.Jsoup.Parser.ParseErrorList(INITIAL_CAPACITY, maxSize);
        }
    }
}
