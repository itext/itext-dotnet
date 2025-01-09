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
using iText.IO.Font.Otf.Lookuptype6;

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 6: Chaining Contextual Substitution Subtable</summary>
    public class GsubLookupType6 : GsubLookupType5 {
        protected internal GsubLookupType6(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations
            )
            : base(openReader, lookupFlag, subTableLocations) {
        }

        protected internal override void ReadSubTableFormat1(int subTableLocation) {
            IDictionary<int, IList<ContextualSubstRule>> substMap = new Dictionary<int, IList<ContextualSubstRule>>();
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int chainSubRuleSetCount = openReader.rf.ReadUnsignedShort();
            int[] chainSubRuleSetOffsets = openReader.ReadUShortArray(chainSubRuleSetCount, subTableLocation);
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverageOffset);
            for (int i = 0; i < chainSubRuleSetCount; ++i) {
                openReader.rf.Seek(chainSubRuleSetOffsets[i]);
                int chainSubRuleCount = openReader.rf.ReadUnsignedShort();
                int[] chainSubRuleOffsets = openReader.ReadUShortArray(chainSubRuleCount, chainSubRuleSetOffsets[i]);
                IList<ContextualSubstRule> chainSubRuleSet = new List<ContextualSubstRule>(chainSubRuleCount);
                for (int j = 0; j < chainSubRuleCount; ++j) {
                    openReader.rf.Seek(chainSubRuleOffsets[j]);
                    int backtrackGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] backtrackGlyphIds = openReader.ReadUShortArray(backtrackGlyphCount);
                    int inputGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] inputGlyphIds = openReader.ReadUShortArray(inputGlyphCount - 1);
                    int lookAheadGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] lookAheadGlyphIds = openReader.ReadUShortArray(lookAheadGlyphCount);
                    int substCount = openReader.rf.ReadUnsignedShort();
                    SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
                    chainSubRuleSet.Add(new SubTableLookup6Format1.SubstRuleFormat1(backtrackGlyphIds, inputGlyphIds, lookAheadGlyphIds
                        , substLookupRecords));
                }
                substMap.Put(coverageGlyphIds[i], chainSubRuleSet);
            }
            subTables.Add(new SubTableLookup6Format1(openReader, lookupFlag, substMap));
        }

        protected internal override void ReadSubTableFormat2(int subTableLocation) {
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int backtrackClassDefOffset = openReader.rf.ReadUnsignedShort();
            int inputClassDefOffset = openReader.rf.ReadUnsignedShort();
            int lookaheadClassDefOffset = openReader.rf.ReadUnsignedShort();
            int chainSubClassSetCount = openReader.rf.ReadUnsignedShort();
            int[] chainSubClassSetOffsets = openReader.ReadUShortArray(chainSubClassSetCount, subTableLocation);
            ICollection<int> coverageGlyphIds = new HashSet<int>(openReader.ReadCoverageFormat(subTableLocation + coverageOffset
                ));
            OtfClass backtrackClassDefinition = openReader.ReadClassDefinition(subTableLocation + backtrackClassDefOffset
                );
            OtfClass inputClassDefinition = openReader.ReadClassDefinition(subTableLocation + inputClassDefOffset);
            OtfClass lookaheadClassDefinition = openReader.ReadClassDefinition(subTableLocation + lookaheadClassDefOffset
                );
            SubTableLookup6Format2 t = new SubTableLookup6Format2(openReader, lookupFlag, coverageGlyphIds, backtrackClassDefinition
                , inputClassDefinition, lookaheadClassDefinition);
            IList<IList<ContextualSubstRule>> subClassSets = new List<IList<ContextualSubstRule>>(chainSubClassSetCount
                );
            for (int i = 0; i < chainSubClassSetCount; ++i) {
                IList<ContextualSubstRule> subClassSet = null;
                if (chainSubClassSetOffsets[i] != 0) {
                    openReader.rf.Seek(chainSubClassSetOffsets[i]);
                    int chainSubClassRuleCount = openReader.rf.ReadUnsignedShort();
                    int[] chainSubClassRuleOffsets = openReader.ReadUShortArray(chainSubClassRuleCount, chainSubClassSetOffsets
                        [i]);
                    subClassSet = new List<ContextualSubstRule>(chainSubClassRuleCount);
                    for (int j = 0; j < chainSubClassRuleCount; ++j) {
                        SubTableLookup6Format2.SubstRuleFormat2 rule;
                        openReader.rf.Seek(chainSubClassRuleOffsets[j]);
                        int backtrackClassCount = openReader.rf.ReadUnsignedShort();
                        int[] backtrackClassIds = openReader.ReadUShortArray(backtrackClassCount);
                        int inputClassCount = openReader.rf.ReadUnsignedShort();
                        int[] inputClassIds = openReader.ReadUShortArray(inputClassCount - 1);
                        int lookAheadClassCount = openReader.rf.ReadUnsignedShort();
                        int[] lookAheadClassIds = openReader.ReadUShortArray(lookAheadClassCount);
                        int substCount = openReader.rf.ReadUnsignedShort();
                        SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
                        rule = new SubTableLookup6Format2.SubstRuleFormat2(t, backtrackClassIds, inputClassIds, lookAheadClassIds, 
                            substLookupRecords);
                        subClassSet.Add(rule);
                    }
                }
                subClassSets.Add(subClassSet);
            }
            t.SetSubClassSets(subClassSets);
            subTables.Add(t);
        }

        protected internal override void ReadSubTableFormat3(int subTableLocation) {
            int backtrackGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] backtrackCoverageOffsets = openReader.ReadUShortArray(backtrackGlyphCount, subTableLocation);
            int inputGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] inputCoverageOffsets = openReader.ReadUShortArray(inputGlyphCount, subTableLocation);
            int lookaheadGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] lookaheadCoverageOffsets = openReader.ReadUShortArray(lookaheadGlyphCount, subTableLocation);
            int substCount = openReader.rf.ReadUnsignedShort();
            SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
            IList<ICollection<int>> backtrackCoverages = new List<ICollection<int>>(backtrackGlyphCount);
            openReader.ReadCoverages(backtrackCoverageOffsets, backtrackCoverages);
            IList<ICollection<int>> inputCoverages = new List<ICollection<int>>(inputGlyphCount);
            openReader.ReadCoverages(inputCoverageOffsets, inputCoverages);
            IList<ICollection<int>> lookaheadCoverages = new List<ICollection<int>>(lookaheadGlyphCount);
            openReader.ReadCoverages(lookaheadCoverageOffsets, lookaheadCoverages);
            SubTableLookup6Format3.SubstRuleFormat3 rule = new SubTableLookup6Format3.SubstRuleFormat3(backtrackCoverages
                , inputCoverages, lookaheadCoverages, substLookupRecords);
            subTables.Add(new SubTableLookup6Format3(openReader, lookupFlag, rule));
        }
    }
}
