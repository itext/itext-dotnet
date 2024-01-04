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
    /// <summary>Chaining Contextual Substitution Subtable: Class-based Chaining Context Glyph Substitution</summary>
    public class SubTableLookup6Format2 : ChainingContextualTable<ContextualSubstRule> {
        private ICollection<int> substCoverageGlyphIds;

        private IList<IList<ContextualSubstRule>> subClassSets;

        private OtfClass backtrackClassDefinition;

        private OtfClass inputClassDefinition;

        private OtfClass lookaheadClassDefinition;

        public SubTableLookup6Format2(OpenTypeFontTableReader openReader, int lookupFlag, ICollection<int> substCoverageGlyphIds
            , OtfClass backtrackClassDefinition, OtfClass inputClassDefinition, OtfClass lookaheadClassDefinition)
            : base(openReader, lookupFlag) {
            this.substCoverageGlyphIds = substCoverageGlyphIds;
            this.backtrackClassDefinition = backtrackClassDefinition;
            this.inputClassDefinition = inputClassDefinition;
            this.lookaheadClassDefinition = lookaheadClassDefinition;
        }

        public virtual void SetSubClassSets(IList<IList<ContextualSubstRule>> subClassSets) {
            this.subClassSets = subClassSets;
        }

        protected internal override IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startId) {
            if (substCoverageGlyphIds.Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                int gClass = inputClassDefinition.GetOtfClass(startId);
                return subClassSets[gClass];
            }
            return JavaCollectionsUtil.EmptyList<ContextualSubstRule>();
        }

        public class SubstRuleFormat2 : ContextualSubstRule {
            // inputClassIds array omits the first class in the sequence,
            // the first class is defined by corresponding index of subClassSet array
            private int[] backtrackClassIds;

            private int[] inputClassIds;

            private int[] lookAheadClassIds;

            private SubstLookupRecord[] substLookupRecords;

            private SubTableLookup6Format2 subTable;

            public SubstRuleFormat2(SubTableLookup6Format2 subTable, int[] backtrackClassIds, int[] inputClassIds, int
                [] lookAheadClassIds, SubstLookupRecord[] substLookupRecords) {
                this.subTable = subTable;
                this.backtrackClassIds = backtrackClassIds;
                this.inputClassIds = inputClassIds;
                this.lookAheadClassIds = lookAheadClassIds;
                this.substLookupRecords = substLookupRecords;
            }

            public override int GetContextLength() {
                return inputClassIds.Length + 1;
            }

            public override int GetLookaheadContextLength() {
                return lookAheadClassIds.Length;
            }

            public override int GetBacktrackContextLength() {
                return backtrackClassIds.Length;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return subTable.inputClassDefinition.GetOtfClass(glyphId) == inputClassIds[atIdx - 1];
            }

            public override bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
                return subTable.lookaheadClassDefinition.GetOtfClass(glyphId) == lookAheadClassIds[atIdx];
            }

            public override bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
                return subTable.backtrackClassDefinition.GetOtfClass(glyphId) == backtrackClassIds[atIdx];
            }
        }
    }
}
