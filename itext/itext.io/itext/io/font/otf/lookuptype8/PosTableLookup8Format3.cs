/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
        private readonly PosTableLookup8Format3.PosRuleFormat3 posRule;

        /// <summary>Creates a new Chained Contexts Positioning Format 3.</summary>
        /// <param name="openReader">the OpenType font reader</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <param name="rule">the rule</param>
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

        /// <summary>Represents the positioning rule format3 of an OpenType font.</summary>
        public class PosRuleFormat3 : ContextualPositionRule {
            private readonly IList<ICollection<int>> inputCoverages;

            private readonly IList<ICollection<int>> backtrackCoverages;

            private readonly IList<ICollection<int>> lookaheadCoverages;

            private readonly PosLookupRecord[] posLookupRecords;

            /// <summary>Creates a new positioning rule format3.</summary>
            /// <param name="backtrackCoverages">the backtrack coverages</param>
            /// <param name="inputCoverages">the input coverages</param>
            /// <param name="lookaheadCoverages">the lookahead coverages</param>
            /// <param name="posLookupRecords">the positioning lookup records</param>
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

            /// <summary>Returns the input coverage.</summary>
            /// <param name="idx">the idx</param>
            /// <returns>the requested result</returns>
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
