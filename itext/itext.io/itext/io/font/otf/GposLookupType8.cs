/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.IO.Font.Otf.Lookuptype8;

namespace iText.IO.Font.Otf {
    /// <summary>
    /// Lookup Type 8:
    /// Chaining Contextual Positioning Subtable
    /// </summary>
    public class GposLookupType8 : GposLookupType7 {
        protected internal GposLookupType8(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations
            )
            : base(openReader, lookupFlag, subTableLocations) {
            subTables = new List<ContextualTable<ContextualPositionRule>>();
            ReadSubTables();
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadShort();
            switch (substFormat) {
                case 1: {
                    ReadSubTableFormat1(subTableLocation);
                    break;
                }

                case 2: {
                    ReadSubTableFormat2(subTableLocation);
                    break;
                }

                case 3: {
                    ReadSubTableFormat3(subTableLocation);
                    break;
                }

                default: {
                    throw new ArgumentException("Bad subtable format identifier: " + substFormat);
                }
            }
        }

        protected internal override void ReadSubTableFormat2(int subTableLocation) {
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int backtrackClassDefOffset = openReader.rf.ReadUnsignedShort();
            int inputClassDefOffset = openReader.rf.ReadUnsignedShort();
            int lookaheadClassDefOffset = openReader.rf.ReadUnsignedShort();
            int chainPosClassSetCount = openReader.rf.ReadUnsignedShort();
            int[] chainPosClassSetOffsets = openReader.ReadUShortArray(chainPosClassSetCount, subTableLocation);
            ICollection<int> coverageGlyphIds = new HashSet<int>(openReader.ReadCoverageFormat(subTableLocation + coverageOffset
                ));
            OtfClass backtrackClassDefinition = openReader.ReadClassDefinition(subTableLocation + backtrackClassDefOffset
                );
            OtfClass inputClassDefinition = openReader.ReadClassDefinition(subTableLocation + inputClassDefOffset);
            OtfClass lookaheadClassDefinition = openReader.ReadClassDefinition(subTableLocation + lookaheadClassDefOffset
                );
            PosTableLookup8Format2 t = new PosTableLookup8Format2(openReader, lookupFlag, coverageGlyphIds, backtrackClassDefinition
                , inputClassDefinition, lookaheadClassDefinition);
            for (int i = 0; i < chainPosClassSetCount; ++i) {
                IList<ContextualPositionRule> posClassSet = JavaCollectionsUtil.EmptyList<ContextualPositionRule>();
                if (chainPosClassSetOffsets[i] != 0) {
                    openReader.rf.Seek(chainPosClassSetOffsets[i]);
                    int chainPosClassRuleCount = openReader.rf.ReadUnsignedShort();
                    int[] chainPosClassRuleOffsets = openReader.ReadUShortArray(chainPosClassRuleCount, chainPosClassSetOffsets
                        [i]);
                    posClassSet = new List<ContextualPositionRule>(chainPosClassRuleCount);
                    for (int j = 0; j < chainPosClassRuleCount; ++j) {
                        openReader.rf.Seek(chainPosClassRuleOffsets[j]);
                        int backtrackClassCount = openReader.rf.ReadUnsignedShort();
                        int[] backtrackClassIds = openReader.ReadUShortArray(backtrackClassCount);
                        int inputClassCount = openReader.rf.ReadUnsignedShort();
                        int[] inputClassIds = openReader.ReadUShortArray(inputClassCount - 1);
                        int lookAheadClassCount = openReader.rf.ReadUnsignedShort();
                        int[] lookAheadClassIds = openReader.ReadUShortArray(lookAheadClassCount);
                        int substCount = openReader.rf.ReadUnsignedShort();
                        PosLookupRecord[] posLookupRecords = openReader.ReadPosLookupRecords(substCount);
                        PosTableLookup8Format2.PosRuleFormat2 rule = new PosTableLookup8Format2.PosRuleFormat2(t, backtrackClassIds
                            , inputClassIds, lookAheadClassIds, posLookupRecords);
                        posClassSet.Add(rule);
                    }
                }
                t.AddPosClassSet(posClassSet);
            }
            subTables.Add(t);
        }

        private void ReadSubTableFormat1(int subTableLocation) {
            IDictionary<int, IList<ContextualPositionRule>> posMap = new Dictionary<int, IList<ContextualPositionRule>
                >();
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int chainPosRuleSetCount = openReader.rf.ReadUnsignedShort();
            int[] chainPosRuleSetOffsets = openReader.ReadUShortArray(chainPosRuleSetCount, subTableLocation);
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverageOffset);
            for (int i = 0; i < chainPosRuleSetCount; ++i) {
                openReader.rf.Seek(chainPosRuleSetOffsets[i]);
                int chainPosRuleCount = openReader.rf.ReadUnsignedShort();
                int[] chainPosRuleOffsets = openReader.ReadUShortArray(chainPosRuleCount, chainPosRuleSetOffsets[i]);
                IList<ContextualPositionRule> chainPosRuleSet = new List<ContextualPositionRule>(chainPosRuleCount);
                for (int j = 0; j < chainPosRuleCount; ++j) {
                    openReader.rf.Seek(chainPosRuleOffsets[j]);
                    int backtrackGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] backtrackGlyphIds = openReader.ReadUShortArray(backtrackGlyphCount);
                    int inputGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] inputGlyphIds = openReader.ReadUShortArray(inputGlyphCount - 1);
                    int lookAheadGlyphCount = openReader.rf.ReadUnsignedShort();
                    int[] lookAheadGlyphIds = openReader.ReadUShortArray(lookAheadGlyphCount);
                    int posCount = openReader.rf.ReadUnsignedShort();
                    PosLookupRecord[] posLookupRecords = openReader.ReadPosLookupRecords(posCount);
                    chainPosRuleSet.Add(new PosTableLookup8Format1.PosRuleFormat1(backtrackGlyphIds, inputGlyphIds, lookAheadGlyphIds
                        , posLookupRecords));
                }
                posMap.Put(coverageGlyphIds[i], chainPosRuleSet);
            }
            subTables.Add(new PosTableLookup8Format1(openReader, lookupFlag, posMap));
        }

        private void ReadSubTableFormat3(int subTableLocation) {
            int backtrackGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] backtrackCoverageOffsets = openReader.ReadUShortArray(backtrackGlyphCount, subTableLocation);
            int inputGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] inputCoverageOffsets = openReader.ReadUShortArray(inputGlyphCount, subTableLocation);
            int lookaheadGlyphCount = openReader.rf.ReadUnsignedShort();
            int[] lookaheadCoverageOffsets = openReader.ReadUShortArray(lookaheadGlyphCount, subTableLocation);
            int posCount = openReader.rf.ReadUnsignedShort();
            PosLookupRecord[] posLookupRecords = openReader.ReadPosLookupRecords(posCount);
            IList<ICollection<int>> backtrackCoverages = new List<ICollection<int>>(backtrackGlyphCount);
            openReader.ReadCoverages(backtrackCoverageOffsets, backtrackCoverages);
            IList<ICollection<int>> inputCoverages = new List<ICollection<int>>(inputGlyphCount);
            openReader.ReadCoverages(inputCoverageOffsets, inputCoverages);
            IList<ICollection<int>> lookaheadCoverages = new List<ICollection<int>>(lookaheadGlyphCount);
            openReader.ReadCoverages(lookaheadCoverageOffsets, lookaheadCoverages);
            PosTableLookup8Format3.PosRuleFormat3 rule = new PosTableLookup8Format3.PosRuleFormat3(backtrackCoverages, 
                inputCoverages, lookaheadCoverages, posLookupRecords);
            subTables.Add(new PosTableLookup8Format3(openReader, lookupFlag, rule));
        }
    }
}
