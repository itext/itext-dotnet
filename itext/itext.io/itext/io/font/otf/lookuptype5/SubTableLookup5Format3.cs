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
    /// <summary>Contextual Substitution Subtable: Coverage-based context glyph substitution</summary>
    public class SubTableLookup5Format3 : ContextualTable<ContextualSubstRule> {
//\cond DO_NOT_DOCUMENT
        internal ContextualSubstRule substitutionRule;
//\endcond

        public SubTableLookup5Format3(OpenTypeFontTableReader openReader, int lookupFlag, SubTableLookup5Format3.SubstRuleFormat3
             rule)
            : base(openReader, lookupFlag) {
            this.substitutionRule = rule;
        }

        protected internal override IList<ContextualSubstRule> GetSetOfRulesForStartGlyph(int startId) {
            SubTableLookup5Format3.SubstRuleFormat3 ruleFormat3 = (SubTableLookup5Format3.SubstRuleFormat3)this.substitutionRule;
            if (ruleFormat3.coverages[0].Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                return JavaCollectionsUtil.SingletonList(this.substitutionRule);
            }
            return JavaCollectionsUtil.EmptyList<ContextualSubstRule>();
        }

        public class SubstRuleFormat3 : ContextualSubstRule {
//\cond DO_NOT_DOCUMENT
            internal IList<ICollection<int>> coverages;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal SubstLookupRecord[] substLookupRecords;
//\endcond

            public SubstRuleFormat3(IList<ICollection<int>> coverages, SubstLookupRecord[] substLookupRecords) {
                this.coverages = coverages;
                this.substLookupRecords = substLookupRecords;
            }

            public override int GetContextLength() {
                return coverages.Count;
            }

            public override SubstLookupRecord[] GetSubstLookupRecords() {
                return substLookupRecords;
            }

            public override bool IsGlyphMatchesInput(int glyphId, int atIdx) {
                return coverages[atIdx].Contains(glyphId);
            }
        }
    }
}
