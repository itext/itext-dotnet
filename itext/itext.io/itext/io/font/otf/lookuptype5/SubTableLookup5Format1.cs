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

namespace iText.IO.Font.Otf.Lookuptype5 {
    /// <summary>Contextual Substitution Subtable: Simple context glyph substitution</summary>
    public class SubTableLookup5Format1 : ContextualTable<ContextualSubstRule> {
        private IDictionary<int, IList<ContextualSubstRule>> substMap;

        public SubTableLookup5Format1(OpenTypeFontTableReader openReader, int lookupFlag, IDictionary<int, IList<ContextualSubstRule
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

            private SubstLookupRecord[] substLookupRecords;

            public SubstRuleFormat1(int[] inputGlyphIds, SubstLookupRecord[] substLookupRecords) {
                this.inputGlyphIds = inputGlyphIds;
                this.substLookupRecords = substLookupRecords;
            }

            public override int GetContextLength() {
                return inputGlyphIds.Length + 1;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return glyphId == inputGlyphIds[atIdx - 1];
            }
        }
    }
}
