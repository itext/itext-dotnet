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
namespace iText.StyledXmlParser.Css.Parse.Syntax {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// <see cref="IParserState"/>
    /// implementation for the end comment state.
    /// </summary>
    internal class CommentEndState : IParserState {
        /// <summary>The state machine that parses the CSS.</summary>
        private CssParserStateController controller;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="CommentEndState"/>
        /// instance.
        /// </summary>
        /// <param name="controller">the state machine that parses the CSS</param>
        internal CommentEndState(CssParserStateController controller) {
            this.controller = controller;
        }
//\endcond

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.parse.syntax.IParserState#process(char)
        */
        public virtual void Process(char ch) {
            if (ch == '/') {
                controller.EnterPreviousActiveState();
            }
            else {
                if (ch == '*') {
                }
                else {
                    // stay in comment end state
                    controller.EnterCommentInnerState();
                }
            }
        }
    }
//\endcond
}
