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

namespace iText.IO.Font.Otf.Lookuptype7 {
    /// <summary>Contextual Positioning Subtable: Class-Based Glyph Contexts.</summary>
    public class PosTableLookup7Format2 : ContextualTable<ContextualPositionRule> {
        private readonly ICollection<int> posCoverageGlyphIds;

        private readonly OtfClass classDefinition;

        private IList<IList<ContextualPositionRule>> posClassSets;

        /// <summary>Creates a new Contextual Positioning Subtable.</summary>
        /// <param name="openReader">the OpenType font reader</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <param name="posCoverageGlyphIds">the positioning coverage glyph ids</param>
        /// <param name="classDefinition">the class definition</param>
        public PosTableLookup7Format2(OpenTypeFontTableReader openReader, int lookupFlag, ICollection<int> posCoverageGlyphIds
            , OtfClass classDefinition)
            : base(openReader, lookupFlag) {
            this.posCoverageGlyphIds = posCoverageGlyphIds;
            this.classDefinition = classDefinition;
        }

        /// <summary>Updates the positioning class sets.</summary>
        /// <param name="posClassSets">the positioning class sets</param>
        public virtual void SetPosClassSets(IList<IList<ContextualPositionRule>> posClassSets) {
            this.posClassSets = posClassSets;
        }

        protected internal override IList<ContextualPositionRule> GetSetOfRulesForStartGlyph(int startId) {
            if (posCoverageGlyphIds.Contains(startId) && !openReader.IsSkip(startId, lookupFlag)) {
                int gClass = classDefinition.GetOtfClass(startId);
                return posClassSets[gClass];
            }
            return JavaCollectionsUtil.EmptyList<ContextualPositionRule>();
        }

        /// <summary>Represents the positioning rule format2 of an OpenType font.</summary>
        public class PosRuleFormat2 : ContextualPositionRule {
            // inputClassIds array omits the first class in the sequence,
            // the first class is defined by corresponding index of subClassSet array
            private readonly int[] inputClassIds;

            private readonly PosLookupRecord[] posLookupRecords;

            private readonly OtfClass classDefinition;

            /// <summary>Creates a new positioning rule format2.</summary>
            /// <param name="subTable">the sub table</param>
            /// <param name="inputClassIds">the input class ids</param>
            /// <param name="posLookupRecords">the positioning lookup records</param>
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
