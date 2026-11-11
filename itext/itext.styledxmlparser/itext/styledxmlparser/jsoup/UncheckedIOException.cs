/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;

namespace iText.StyledXmlParser.Jsoup {
    public class UncheckedIOException : Exception {
        public UncheckedIOException(System.IO.IOException cause)
            : base(cause.Message, cause) {
        }

        public UncheckedIOException(String message)
            : base(message, new System.IO.IOException(message)) {
        }

        public virtual System.IO.IOException IoException() {
            return (System.IO.IOException)InnerException;
        }
    }
}
