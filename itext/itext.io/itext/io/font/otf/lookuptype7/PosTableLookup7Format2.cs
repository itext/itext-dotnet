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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Otf;

namespace iText.IO.Font.Otf.Lookuptype7 {
    public class PosTableLookup7Format2 : ContextualTable<ContextualPositionRule> {
        private ICollection<int> posCoverageGlyphIds;

        private IList<IList<ContextualPositionRule>> subClassSets;

        private OtfClass classDefinition;

        public PosTableLookup7Format2(OpenTypeFontTableReader openReader, int lookupFlag, ICollection<int> posCoverageGlyphIds
            , OtfClass classDefinition)
            : base(openReader, lookupFlag) {
            this.posCoverageGlyphIds = posCoverageGlyphIds;
            this.classDefinition = classDefinition;
        }

        public virtual void SetPosClassSets(IList<IList<ContextualPositionRule>> subClassSets) {
            this.subClassSets = subClassSets;
        }

        protected internal override IList<ContextualPositionRule> GetSetOfRulesForStartGlyph(int startId) {
            if (posCoverageGlyphIds.Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                int gClass = classDefinition.GetOtfClass(startId);
                return subClassSets[gClass];
            }
            return JavaCollectionsUtil.EmptyList<ContextualPositionRule>();
        }

        public class PosRuleFormat2 : ContextualPositionRule {
            // inputClassIds array omits the first class in the sequence,
            // the first class is defined by corresponding index of subClassSet array
            private int[] inputClassIds;

            private PosLookupRecord[] posLookupRecords;

            private OtfClass classDefinition;

            public PosRuleFormat2(PosTableLookup7Format2 subTable, int[] inputClassIds, PosLookupRecord[] posLookupRecords
                ) {
                this.inputClassIds = inputClassIds;
                this.posLookupRecords = posLookupRecords;
                this.classDefinition = subTable.classDefinition;
            }

            public override int GetContextLength() {
                return inputClassIds.Length + 1;
            }

            public override PosLookupRecord[] GetPosLookupRecords() {
                return posLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return classDefinition.GetOtfClass(glyphId) == inputClassIds[atIdx - 1];
            }
        }
    }
}
