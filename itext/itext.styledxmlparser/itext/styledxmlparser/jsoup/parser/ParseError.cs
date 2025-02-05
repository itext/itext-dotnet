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
