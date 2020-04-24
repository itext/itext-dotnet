/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
