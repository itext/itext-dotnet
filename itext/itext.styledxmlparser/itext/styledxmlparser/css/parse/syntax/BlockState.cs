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
namespace iText.StyledXmlParser.Css.Parse.Syntax {
    /// <summary>
    /// <see cref="IParserState"/>
    /// implementation for the block state.
    /// </summary>
    internal class BlockState : IParserState {
        /// <summary>The state machine that parses the CSS.</summary>
        private CssParserStateController controller;

        /// <summary>
        /// Creates a new
        /// <see cref="BlockState"/>
        /// instance.
        /// </summary>
        /// <param name="controller">the state machine that parses the CSS</param>
        internal BlockState(CssParserStateController controller) {
            this.controller = controller;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.parse.syntax.IParserState#process(char)
        */
        public virtual void Process(char ch) {
            if (ch == '}') {
                controller.StoreCurrentProperties();
                controller.EnterUnknownStateIfNestedBlocksFinished();
            }
            else {
                if (ch == '/') {
                    controller.EnterCommentStartState();
                }
                else {
                    controller.AppendToBuffer(ch);
                }
            }
        }
    }
}
