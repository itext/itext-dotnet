/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.IO.Font.Otf {
    public abstract class ContextualRule {
        /// <summary>Gets the length of the context glyph sequence defined by this rule</summary>
        /// <returns>length of the context</returns>
        public abstract int GetContextLength();

        /// <summary>Checks if glyph line element matches element from input sequence of the rule.</summary>
        /// <remarks>
        /// Checks if glyph line element matches element from input sequence of the rule.
        /// <br /><br />
        /// NOTE: rules do not contain the first element of the input sequence, the first element is defined by rule
        /// position in substitution table. Therefore atIdx shall not be 0.
        /// </remarks>
        /// <param name="glyphId">glyph code id</param>
        /// <param name="atIdx">
        /// index in the rule sequence. Shall be: 0 &lt; atIdx &lt;
        /// <see cref="GetContextLength()"/>
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if glyph matches element
        /// </returns>
        public abstract bool IsGlyphMatchesInput(int glyphId, int atIdx);

        /// <summary>Gets the length of the lookahead context glyph sequence defined by this rule</summary>
        /// <returns>length of the lookahead context</returns>
        public virtual int GetLookaheadContextLength() {
            return 0;
        }

        /// <summary>Gets the length of the backtrack context glyph sequence defined by this rule</summary>
        /// <returns>length of the backtrack context</returns>
        public virtual int GetBacktrackContextLength() {
            return 0;
        }

        /// <summary>Checks if glyph line element matches element from lookahead sequence of the rule.</summary>
        /// <param name="glyphId">glyph code id</param>
        /// <param name="atIdx">
        /// index in rule sequence. Shall be: 0 &lt;= atIdx &lt;
        /// <see cref="GetLookaheadContextLength()"/>
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if glyph matches element from lookahead sequence
        /// </returns>
        public virtual bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
            return false;
        }

        /// <summary>Checks if glyph line element matches element from backtrack sequence of the rule.</summary>
        /// <param name="glyphId">glyph code id</param>
        /// <param name="atIdx">
        /// index in rule sequence. Shall be: 0 &lt;= atIdx &lt;
        /// <see cref="GetBacktrackContextLength()"/>
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if glyph matches element from backtrack sequence
        /// </returns>
        public virtual bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
            return false;
        }
    }
}
