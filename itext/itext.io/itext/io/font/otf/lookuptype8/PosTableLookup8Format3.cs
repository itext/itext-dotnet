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

namespace iText.IO.Font.Otf.Lookuptype8 {
    /// <summary>Chained Contexts Positioning Format 3: Coverage-based Glyph Contexts</summary>
    public class PosTableLookup8Format3 : ChainingContextualTable<ContextualPositionRule> {
        private PosTableLookup8Format3.PosRuleFormat3 posRule;

        public PosTableLookup8Format3(OpenTypeFontTableReader openReader, int lookupFlag, PosTableLookup8Format3.PosRuleFormat3
             rule)
            : base(openReader, lookupFlag) {
            this.posRule = rule;
        }

        protected internal override IList<ContextualPositionRule> GetSetOfRulesForStartGlyph(int startId) {
            PosTableLookup8Format3.PosRuleFormat3 ruleFormat3 = (PosTableLookup8Format3.PosRuleFormat3)this.posRule;
            if (ruleFormat3.GetInputCoverage(0).Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                return JavaCollectionsUtil.SingletonList<ContextualPositionRule>(this.posRule);
            }
            return JavaCollectionsUtil.EmptyList<ContextualPositionRule>();
        }

        public class PosRuleFormat3 : ContextualPositionRule {
            private IList<ICollection<int>> inputCoverages;

            private IList<ICollection<int>> backtrackCoverages;

            private IList<ICollection<int>> lookaheadCoverages;

            private PosLookupRecord[] posLookupRecords;

            public PosRuleFormat3(IList<ICollection<int>> backtrackCoverages, IList<ICollection<int>> inputCoverages, 
                IList<ICollection<int>> lookaheadCoverages, PosLookupRecord[] posLookupRecords) {
                this.backtrackCoverages = backtrackCoverages;
                this.inputCoverages = inputCoverages;
                this.lookaheadCoverages = lookaheadCoverages;
                this.posLookupRecords = posLookupRecords;
            }

            public override PosLookupRecord[] GetPosLookupRecords() {
                return posLookupRecords;
            }

            public override int GetContextLength() {
                return inputCoverages.Count;
            }

            public virtual ICollection<int> GetInputCoverage(int idx) {
                return inputCoverages[idx];
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return GetInputCoverage(atIdx).Contains(glyphId);
            }

            public override int GetLookaheadContextLength() {
                return lookaheadCoverages.Count;
            }

            public override bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
                return lookaheadCoverages[atIdx].Contains(glyphId);
            }

            public override int GetBacktrackContextLength() {
                return backtrackCoverages.Count;
            }

            public override bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
                return backtrackCoverages[atIdx].Contains(glyphId);
            }
        }
    }
}
