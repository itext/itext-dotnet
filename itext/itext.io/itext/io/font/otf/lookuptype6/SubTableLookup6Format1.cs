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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype6 {
    /// <summary>Chaining Contextual Substitution Subtable: Simple Chaining Context Glyph Substitution</summary>
    public class SubTableLookup6Format1 : ChainingContextualTable<ContextualSubstRule> {
        private readonly IDictionary<int, IList<ContextualSubstRule>> substMap;

        /// <summary>Creates a new Chaining Contextual Substitution Subtable.</summary>
        /// <param name="openReader">the OpenType font reader</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <param name="substMap">the substitution map</param>
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

        /// <summary>Represents the substitution rule format1 of an OpenType font.</summary>
        public class SubstRuleFormat1 : ContextualSubstRule {
            // inputGlyphIds array omits the first glyph in the sequence,
            // the first glyph is defined by corresponding coverage glyph
            private readonly int[] inputGlyphIds;

            private readonly int[] backtrackGlyphIds;

            private readonly int[] lookAheadGlyphIds;

            private readonly SubstLookupRecord[] substLookupRecords;

            /// <summary>Creates a new substitution rule format1.</summary>
            /// <param name="backtrackGlyphIds">the backtrack glyph ids</param>
            /// <param name="inputGlyphIds">the input glyph ids</param>
            /// <param name="lookAheadGlyphIds">the look ahead glyph ids</param>
            /// <param name="substLookupRecords">the substitution lookup records</param>
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
