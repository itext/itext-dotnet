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
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype6 {
    /// <summary>Chaining Contextual Substitution Subtable: Simple Chaining Context Glyph Substitution</summary>
    public class SubTableLookup6Format1 : ChainingContextualTable<ContextualSubstRule> {
        private IDictionary<int, IList<ContextualSubstRule>> substMap;

        public SubTableLookup6Format1(OpenTypeFontTableReader openReader, int lookupFlag, IDictionary<int, IList<ContextualSubstRule
            >> substMap)
            : base(openReader, lookupFlag) {
            this.substMap = substMap;
        }

        protected internal override IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startGlyphId) {
            if (substMap.ContainsKey(startGlyphId) && !openReader.IsSkip(startGlyphId, lookupFlag)) {
                return substMap.Get(startGlyphId);
            }
            return JavaCollectionsUtil.EmptyList<ContextualSubstRule>();
        }

        public class SubstRuleFormat1 : ContextualSubstRule {
            // inputGlyphIds array omits the first glyph in the sequence,
            // the first glyph is defined by corresponding coverage glyph
            private int[] inputGlyphIds;

            private int[] backtrackGlyphIds;

            private int[] lookAheadGlyphIds;

            private SubstLookupRecord[] substLookupRecords;

            public SubstRuleFormat1(int[] backtrackGlyphIds, int[] inputGlyphIds, int[] lookAheadGlyphIds, SubstLookupRecord
                [] substLookupRecords) {
                this.backtrackGlyphIds = backtrackGlyphIds;
                this.inputGlyphIds = inputGlyphIds;
                this.lookAheadGlyphIds = lookAheadGlyphIds;
                this.substLookupRecords = substLookupRecords;
            }

            public override int GetContextLength() {
                return inputGlyphIds.Length + 1;
            }

            public override int GetLookaheadContextLength() {
                return lookAheadGlyphIds.Length;
            }

            public override int GetBacktrackContextLength() {
                return backtrackGlyphIds.Length;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return glyphId == inputGlyphIds[atIdx - 1];
            }

            public override bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
                return glyphId == lookAheadGlyphIds[atIdx];
            }

            public override bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
                return glyphId == backtrackGlyphIds[atIdx];
            }
        }
    }
}
