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

namespace iText.IO.Font.Otf.Lookuptype5 {
    /// <summary>Contextual Substitution Subtable: Class-based context glyph substitution</summary>
    public class SubTableLookup5Format2 : ContextualTable<ContextualSubstRule> {
        private ICollection<int> substCoverageGlyphIds;

        private IList<IList<ContextualSubstRule>> subClassSets;

        private OtfClass classDefinition;

        public SubTableLookup5Format2(OpenTypeFontTableReader openReader, int lookupFlag, ICollection<int> substCoverageGlyphIds
            , OtfClass classDefinition)
            : base(openReader, lookupFlag) {
            this.substCoverageGlyphIds = substCoverageGlyphIds;
            this.classDefinition = classDefinition;
        }

        public virtual void SetSubClassSets(IList<IList<ContextualSubstRule>> subClassSets) {
            this.subClassSets = subClassSets;
        }

        protected internal override IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startId) {
            if (substCoverageGlyphIds.Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                int gClass = classDefinition.GetOtfClass(startId);
                return subClassSets[gClass];
            }
            return JavaCollectionsUtil.EmptyList<ContextualSubstRule>();
        }

        public class SubstRuleFormat2 : ContextualSubstRule {
            // inputClassIds array omits the first class in the sequence,
            // the first class is defined by corresponding index of subClassSet array
            private int[] inputClassIds;

            private SubstLookupRecord[] substLookupRecords;

            private OtfClass classDefinition;

            public SubstRuleFormat2(SubTableLookup5Format2 subTable, int[] inputClassIds, SubstLookupRecord[] substLookupRecords
                ) {
                this.inputClassIds = inputClassIds;
                this.substLookupRecords = substLookupRecords;
                this.classDefinition = subTable.classDefinition;
            }

            public override int GetContextLength() {
                return inputClassIds.Length + 1;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return classDefinition.GetOtfClass(glyphId) == inputClassIds[atIdx - 1];
            }
        }
    }
}
