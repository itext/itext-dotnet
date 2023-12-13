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
using System.Collections.Generic;

namespace iText.IO.Font.Otf {
    public abstract class ContextualTable<T>
        where T : ContextualRule {
        protected internal OpenTypeFontTableReader openReader;

        protected internal int lookupFlag;

        protected internal ContextualTable(OpenTypeFontTableReader openReader, int lookupFlag) {
            this.openReader = openReader;
            this.lookupFlag = lookupFlag;
        }

        /// <summary>Gets a most preferable context rule that matches the line at current position.</summary>
        /// <remarks>
        /// Gets a most preferable context rule that matches the line at current position. If no matching context rule is
        /// found, it returns <c>null</c>.
        /// <br /><br />
        /// NOTE: if matching context rule is found, the <c>GlyphLine.start</c> and <c>GlyphLine.end</c>
        /// will be changed in such way that they will point at start and end of the matching context glyph sequence
        /// inside the glyph line.
        /// </remarks>
        /// <param name="line">a line, which is to be checked if it matches some context.</param>
        /// <returns>matching context rule or null, if none was found.</returns>
        public virtual T GetMatchingContextRule(GlyphLine line) {
            if (line.idx >= line.end) {
                return null;
            }
            Glyph g = line.Get(line.idx);
            IList<T> rules = GetSetOfRulesForStartGlyph(g.GetCode());
            foreach (T rule in rules) {
                int lastGlyphIndex = CheckIfContextMatch(line, rule);
                if (lastGlyphIndex != -1) {
                    line.start = line.idx;
                    line.end = lastGlyphIndex + 1;
                    return rule;
                }
            }
            return null;
        }

        /// <summary>Gets a set of rules, which start with given glyph id.</summary>
        /// <param name="startId">id of the first glyph in the sequence</param>
        /// <returns>
        /// a list of
        /// <see cref="ContextualSubstRule"/>
        /// instances. The list will be empty if there are no rules
        /// that start with a given glyph id
        /// </returns>
        protected internal abstract IList<T> GetSetOfRulesForStartGlyph(int startId);

        /// <summary>Checks if given glyph line matches given rule.</summary>
        /// <param name="line">glyph line to be checked</param>
        /// <param name="rule">rule to be compared with a given glyph line</param>
        /// <returns>
        /// either index which corresponds to the last glyph of the matching context inside the glyph line
        /// if context matches, or -1 if context doesn't match
        /// </returns>
        protected internal virtual int CheckIfContextMatch(GlyphLine line, T rule) {
            int j;
            OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
            gidx.line = line;
            gidx.idx = line.idx;
            //Note, that starting index shall be 1
            for (j = 1; j < rule.GetContextLength(); ++j) {
                gidx.NextGlyph(openReader, lookupFlag);
                if (gidx.glyph == null || !rule.IsGlyphMatchesInput(gidx.glyph.GetCode(), j)) {
                    break;
                }
            }
            bool isMatch = j == rule.GetContextLength();
            if (isMatch) {
                return gidx.idx;
            }
            else {
                return -1;
            }
        }
    }
}
