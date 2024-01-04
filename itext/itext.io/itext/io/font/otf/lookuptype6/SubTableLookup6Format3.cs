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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype6 {
    /// <summary>Chaining Contextual Substitution Subtable: Coverage-based Chaining Context Glyph Substitution</summary>
    public class SubTableLookup6Format3 : ChainingContextualTable<ContextualSubstRule> {
        internal ContextualSubstRule substitutionRule;

        public SubTableLookup6Format3(OpenTypeFontTableReader openReader, int lookupFlag, SubTableLookup6Format3.SubstRuleFormat3
             rule)
            : base(openReader, lookupFlag) {
            this.substitutionRule = rule;
        }

        protected internal override IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startId) {
            SubTableLookup6Format3.SubstRuleFormat3 ruleFormat3 = (SubTableLookup6Format3.SubstRuleFormat3)this.substitutionRule;
            if (ruleFormat3.inputCoverages[0].Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                return JavaCollectionsUtil.SingletonList<ContextualSubstRule>(this.substitutionRule);
            }
            return JavaCollectionsUtil.EmptyList<ContextualSubstRule>();
        }

        public class SubstRuleFormat3 : ContextualSubstRule {
            internal IList<ICollection<int>> backtrackCoverages;

            internal IList<ICollection<int>> inputCoverages;

            internal IList<ICollection<int>> lookaheadCoverages;

            internal SubstLookupRecord[] substLookupRecords;

            public SubstRuleFormat3(IList<ICollection<int>> backtrackCoverages, IList<ICollection<int>> inputCoverages
                , IList<ICollection<int>> lookaheadCoverages, SubstLookupRecord[] substLookupRecords) {
                this.backtrackCoverages = backtrackCoverages;
                this.inputCoverages = inputCoverages;
                this.lookaheadCoverages = lookaheadCoverages;
                this.substLookupRecords = substLookupRecords;
            }

            public override int GetContextLength() {
                return inputCoverages.Count;
            }

            public override int GetLookaheadContextLength() {
                return lookaheadCoverages.Count;
            }

            public override int GetBacktrackContextLength() {
                return backtrackCoverages.Count;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return inputCoverages[atIdx].Contains(glyphId);
            }

            public override bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
                return lookaheadCoverages[atIdx].Contains(glyphId);
            }

            public override bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
                return backtrackCoverages[atIdx].Contains(glyphId);
            }
        }
    }
}
