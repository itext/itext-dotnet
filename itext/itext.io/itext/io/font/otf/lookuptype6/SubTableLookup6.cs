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
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype6 {
    public abstract class SubTableLookup6 : ContextualSubTable {
        protected internal SubTableLookup6(OpenTypeFontTableReader openReader, int lookupFlag)
            : base(openReader, lookupFlag) {
        }

        public override ContextualSubstRule GetMatchingContextRule(GlyphLine line) {
            if (line.idx >= line.end) {
                return null;
            }
            Glyph g = line.Get(line.idx);
            IList<ContextualSubstRule> rules = GetSetOfRulesForStartGlyph(g.GetCode());
            foreach (ContextualSubstRule rule in rules) {
                int lastGlyphIndex = CheckIfContextMatch(line, rule);
                if (lastGlyphIndex != -1 && CheckIfLookaheadContextMatch(line, rule, lastGlyphIndex) && CheckIfBacktrackContextMatch
                    (line, rule)) {
                    line.start = line.idx;
                    line.end = lastGlyphIndex + 1;
                    return rule;
                }
            }
            return null;
        }

        /// <summary>Checks if given glyph line at the given position matches given rule.</summary>
        /// <param name="line">glyph line to be checked</param>
        /// <param name="rule">rule to be compared with a given line</param>
        /// <param name="startIdx">glyph line position</param>
        /// <returns>true if given glyph line at the given position matches given rule</returns>
        protected internal virtual bool CheckIfLookaheadContextMatch(GlyphLine line, ContextualSubstRule rule, int
             startIdx) {
            int j;
            OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
            gidx.line = line;
            gidx.idx = startIdx;
            for (j = 0; j < rule.GetLookaheadContextLength(); ++j) {
                gidx.NextGlyph(openReader, lookupFlag);
                if (gidx.glyph == null || !rule.IsGlyphMatchesLookahead(gidx.glyph.GetCode(), j)) {
                    break;
                }
            }
            return j == rule.GetLookaheadContextLength();
        }

        /// <summary>Checks if given glyph line at the given position matches given rule.</summary>
        /// <param name="line">glyph line to be checked</param>
        /// <param name="rule">rule to be compared with a given line</param>
        /// <returns>true if given glyph line matches given rule</returns>
        protected internal virtual bool CheckIfBacktrackContextMatch(GlyphLine line, ContextualSubstRule rule) {
            int j;
            OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
            gidx.line = line;
            gidx.idx = line.idx;
            for (j = 0; j < rule.GetBacktrackContextLength(); ++j) {
                gidx.PreviousGlyph(openReader, lookupFlag);
                if (gidx.glyph == null || !rule.IsGlyphMatchesBacktrack(gidx.glyph.GetCode(), j)) {
                    break;
                }
            }
            return j == rule.GetBacktrackContextLength();
        }
    }
}
