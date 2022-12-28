/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
