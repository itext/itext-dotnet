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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Otf.Lookuptype7;

namespace iText.IO.Font.Otf {
    /// <summary>
    /// Lookup Type 7:
    /// Contextual Positioning Subtables
    /// </summary>
    public class GposLookupType7 : OpenTableLookup {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.IO.Font.Otf.GposLookupType7
            ));

        protected internal IList<ContextualTable<ContextualPositionRule>> subTables;

        public GposLookupType7(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            subTables = new List<ContextualTable<ContextualPositionRule>>();
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            bool changed = false;
            int oldLineStart = line.start;
            int oldLineEnd = line.end;
            int initialLineIndex = line.idx;
            foreach (ContextualTable<ContextualPositionRule> subTable in subTables) {
                ContextualPositionRule contextRule = subTable.GetMatchingContextRule(line);
                if (contextRule == null) {
                    continue;
                }
                int lineEndBeforeTransformations = line.end;
                PosLookupRecord[] posLookupRecords = contextRule.GetPosLookupRecords();
                OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
                gidx.line = line;
                foreach (PosLookupRecord posRecord in posLookupRecords) {
                    // There could be some skipped glyphs inside the context sequence, therefore currently GlyphIndexer and
                    // nextGlyph method are used to get to the glyph at "substRecord.sequenceIndex" index
                    gidx.idx = initialLineIndex;
                    for (int i = 0; i < posRecord.sequenceIndex; ++i) {
                        gidx.NextGlyph(openReader, lookupFlag);
                    }
                    line.idx = gidx.idx;
                    OpenTableLookup lookupTable = openReader.GetLookupTable(posRecord.lookupListIndex);
                    changed = lookupTable.TransformOne(line) || changed;
                }
                line.idx = line.end;
                line.start = oldLineStart;
                int lenDelta = lineEndBeforeTransformations - line.end;
                line.end = oldLineEnd - lenDelta;
                return changed;
            }
            line.idx++;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadShort();
            switch (substFormat) {
                case 2: {
                    ReadSubTableFormat2(subTableLocation);
                    break;
                }

                case 1:
                case 3: {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.GPOS_LOOKUP_SUBTABLE_FORMAT_NOT_SUPPORTED
                        , substFormat, 7));
                    break;
                }

                default: {
                    throw new ArgumentException("Bad subtable format identifier: " + substFormat);
                }
            }
        }

        protected internal virtual void ReadSubTableFormat2(int subTableLocation) {
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int classDefOffset = openReader.rf.ReadUnsignedShort();
            int posClassSetCount = openReader.rf.ReadUnsignedShort();
            int[] posClassSetOffsets = openReader.ReadUShortArray(posClassSetCount, subTableLocation);
            ICollection<int> coverageGlyphIds = new HashSet<int>(openReader.ReadCoverageFormat(subTableLocation + coverageOffset
                ));
            OtfClass classDefinition = openReader.ReadClassDefinition(subTableLocation + classDefOffset);
            PosTableLookup7Format2 t = new PosTableLookup7Format2(openReader, lookupFlag, coverageGlyphIds, classDefinition
                );
            IList<IList<ContextualPositionRule>> subClassSets = new List<IList<ContextualPositionRule>>(posClassSetCount
                );
            for (int i = 0; i < posClassSetCount; ++i) {
                IList<ContextualPositionRule> subClassSet = null;
                if (posClassSetOffsets[i] != 0) {
                    openReader.rf.Seek(posClassSetOffsets[i]);
                    int posClassRuleCount = openReader.rf.ReadUnsignedShort();
                    int[] posClassRuleOffsets = openReader.ReadUShortArray(posClassRuleCount, posClassSetOffsets[i]);
                    subClassSet = new List<ContextualPositionRule>(posClassRuleCount);
                    for (int j = 0; j < posClassRuleCount; ++j) {
                        ContextualPositionRule rule;
                        openReader.rf.Seek(posClassRuleOffsets[j]);
                        int glyphCount = openReader.rf.ReadUnsignedShort();
                        int posCount = openReader.rf.ReadUnsignedShort();
                        int[] inputClassIds = openReader.ReadUShortArray(glyphCount - 1);
                        PosLookupRecord[] posLookupRecords = openReader.ReadPosLookupRecords(posCount);
                        rule = new PosTableLookup7Format2.PosRuleFormat2(t, inputClassIds, posLookupRecords);
                        subClassSet.Add(rule);
                    }
                }
                subClassSets.Add(subClassSet);
            }
            t.SetPosClassSets(subClassSets);
            subTables.Add(t);
        }
    }
}
