/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype8 {
    /// <summary>Chained Contexts Positioning Format 2: Class-based Glyph Contexts</summary>
    public class PosTableLookup8Format2 : ChainingContextualTable<ContextualPositionRule> {
        private ICollection<int> posCoverageGlyphIds;

        private IList<IList<ContextualPositionRule>> posClassSets;

        private OtfClass backtrackClassDefinition;

        private OtfClass inputClassDefinition;

        private OtfClass lookaheadClassDefinition;

        public PosTableLookup8Format2(OpenTypeFontTableReader openReader, int lookupFlag, ICollection<int> posCoverageGlyphIds
            , OtfClass backtrackClassDefinition, OtfClass inputClassDefinition, OtfClass lookaheadClassDefinition)
            : base(openReader, lookupFlag) {
            this.posCoverageGlyphIds = posCoverageGlyphIds;
            this.backtrackClassDefinition = backtrackClassDefinition;
            this.inputClassDefinition = inputClassDefinition;
            this.lookaheadClassDefinition = lookaheadClassDefinition;
            this.posClassSets = new List<IList<ContextualPositionRule>>();
        }

        public virtual void AddPosClassSet(IList<ContextualPositionRule> posClassSet) {
            foreach (ContextualPositionRule rule in posClassSet) {
                if (((PosTableLookup8Format2.PosRuleFormat2)rule).GetPosTable() != this) {
                    throw new ArgumentException("Position class set is invalid. Position rule refers to another table");
                }
            }
            this.posClassSets.Add(posClassSet);
        }

        protected internal override IList<ContextualPositionRule> GetSetOfRulesForStartGlyph(int startId) {
            if (posCoverageGlyphIds.Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                int gClass = inputClassDefinition.GetOtfClass(startId);
                return posClassSets[gClass];
            }
            return JavaCollectionsUtil.EmptyList<ContextualPositionRule>();
        }

        public class PosRuleFormat2 : ContextualPositionRule {
            private const long serialVersionUID = 8583758144617770335L;

            // inputClassIds array omits the first class in the sequence,
            // the first class is defined by corresponding index of subClassSet array
            private int[] backtrackClassIds;

            private int[] inputClassIds;

            private int[] lookAheadClassIds;

            private PosLookupRecord[] posLookupRecords;

            private PosTableLookup8Format2 posTable;

            public PosRuleFormat2(PosTableLookup8Format2 posTable, int[] backtrackClassIds, int[] inputClassIds, int[]
                 lookAheadClassIds, PosLookupRecord[] posLookupRecords) {
                this.posTable = posTable;
                this.backtrackClassIds = backtrackClassIds;
                this.inputClassIds = inputClassIds;
                this.lookAheadClassIds = lookAheadClassIds;
                this.posLookupRecords = posLookupRecords;
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

            public override PosLookupRecord[] GetPosLookupRecords() {
                return posLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return posTable.inputClassDefinition.GetOtfClass(glyphId) == inputClassIds[atIdx - 1];
            }

            public override bool IsGlyphMatchesLookahead(int glyphId, int atIdx) {
                return posTable.lookaheadClassDefinition.GetOtfClass(glyphId) == lookAheadClassIds[atIdx];
            }

            public override bool IsGlyphMatchesBacktrack(int glyphId, int atIdx) {
                return posTable.backtrackClassDefinition.GetOtfClass(glyphId) == backtrackClassIds[atIdx];
            }

            public virtual PosTableLookup8Format2 GetPosTable() {
                return posTable;
            }
        }
    }
}
