/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>A Parse Error records an error in the input HTML that occurs in either the tokenisation or the tree building phase.
    ///     </summary>
    public class ParseError {
        private int pos;

        private String errorMsg;

//\cond DO_NOT_DOCUMENT
        internal ParseError(int pos, String errorMsg) {
            this.pos = pos;
            this.errorMsg = errorMsg;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal ParseError(int pos, String errorFormat, params Object[] args) {
            this.errorMsg = MessageFormatUtil.Format(errorFormat, args);
            this.pos = pos;
        }
//\endcond

        /// <summary>Retrieve the error message.</summary>
        /// <returns>the error message.</returns>
        public virtual String GetErrorMessage() {
            return errorMsg;
        }

        /// <summary>Retrieves the offset of the error.</summary>
        /// <returns>error offset within input</returns>
        public virtual int GetPosition() {
            return pos;
        }

        public override String ToString() {
            return pos + ": " + errorMsg;
        }
    }
}
