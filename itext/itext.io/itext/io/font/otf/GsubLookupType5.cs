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
using System;
using System.Collections.Generic;
using iText.IO.Font.Otf.Lookuptype5;

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 5: Contextual Substitution Subtable</summary>
    public class GsubLookupType5 : OpenTableLookup {
        protected internal IList<ContextualTable<ContextualSubstRule>> subTables;

        protected internal GsubLookupType5(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations
            )
            : base(openReader, lookupFlag, subTableLocations) {
            subTables = new List<ContextualTable<ContextualSubstRule>>();
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            bool changed = false;
            int oldLineStart = line.start;
            int oldLineEnd = line.end;
            int initialLineIndex = line.idx;
            foreach (ContextualTable<ContextualSubstRule> subTable in subTables) {
                ContextualSubstRule contextRule = subTable.GetMatchingContextRule(line);
                if (contextRule == null) {
                    continue;
                }
                int lineEndBeforeSubstitutions = line.end;
                SubstLookupRecord[] substLookupRecords = contextRule.GetSubstLookupRecords();
                OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
                gidx.line = line;
                foreach (SubstLookupRecord substRecord in substLookupRecords) {
                    // There could be some skipped glyphs inside the context sequence, therefore currently GlyphIndexer and
                    // nextGlyph method are used to get to the glyph at "substRecord.sequenceIndex" index
                    gidx.idx = initialLineIndex;
                    for (int i = 0; i < substRecord.sequenceIndex; ++i) {
                        gidx.NextGlyph(openReader, lookupFlag);
                    }
                    line.idx = gidx.idx;
                    OpenTableLookup lookupTable = openReader.GetLookupTable(substRecord.lookupListIndex);
                    changed = lookupTable.TransformOne(line) || changed;
                }
                line.idx = line.end;
                line.start = oldLineStart;
                int lenDelta = lineEndBeforeSubstitutions - line.end;
                line.end = oldLineEnd - lenDelta;
                return changed;
            }
            ++line.idx;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadShort();
            if (substFormat == 1) {
                ReadSubTableFormat1(subTableLocation);
            }
            else {
                if (substFormat == 2) {
                    ReadSubTableFormat2(subTableLocation);
                }
                else {
                    if (substFormat == 3) {
                        ReadSubTableFormat3(subTableLocation);
                    }
                    else {
                        throw new ArgumentException("Bad substFormat: " + substFormat);
                    }
                }
            }
        }

        protected internal virtual void ReadSubTableFormat1(int subTableLocation) {
            IDictionary<int, IList<ContextualSubstRule>> substMap = new Dictionary<int, IList<ContextualSubstRule>>();
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int subRuleSetCount = openReader.rf.ReadUnsignedShort();
            int[] subRuleSetOffsets = openReader.ReadUShortArray(subRuleSetCount, subTableLocation);
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverageOffset);
            for (int i = 0; i < subRuleSetCount; ++i) {
                openReader.rf.Seek(subRuleSetOffsets[i]);
                int subRuleCount = openReader.rf.ReadUnsignedShort();
                int[] subRuleOffsets = openReader.ReadUShortArray(subRuleCount, subRuleSetOffsets[i]);
                IList<ContextualSubstRule> subRuleSet = new List<ContextualSubstRule>(subRuleCount);
                for (int j = 0; j < subRuleCount; ++j) {
                    openReader.rf.Seek(subRuleOffsets[j]);
                    int glyphCount = openReader.rf.ReadUnsignedShort();
                    int substCount = openReader.rf.ReadUnsignedShort();
                    int[] inputGlyphIds = openReader.ReadUShortArray(glyphCount - 1);
                    SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
                    subRuleSet.Add(new SubTableLookup5Format1.SubstRuleFormat1(inputGlyphIds, substLookupRecords));
                }
                substMap.Put(coverageGlyphIds[i], subRuleSet);
            }
            subTables.Add(new SubTableLookup5Format1(openReader, lookupFlag, substMap));
        }

        protected internal virtual void ReadSubTableFormat2(int subTableLocation) {
            int coverageOffset = openReader.rf.ReadUnsignedShort();
            int classDefOffset = openReader.rf.ReadUnsignedShort();
            int subClassSetCount = openReader.rf.ReadUnsignedShort();
            int[] subClassSetOffsets = openReader.ReadUShortArray(subClassSetCount, subTableLocation);
            ICollection<int> coverageGlyphIds = new HashSet<int>(openReader.ReadCoverageFormat(subTableLocation + coverageOffset
                ));
            OtfClass classDefinition = openReader.ReadClassDefinition(subTableLocation + classDefOffset);
            SubTableLookup5Format2 t = new SubTableLookup5Format2(openReader, lookupFlag, coverageGlyphIds, classDefinition
                );
            IList<IList<ContextualSubstRule>> subClassSets = new List<IList<ContextualSubstRule>>(subClassSetCount);
            for (int i = 0; i < subClassSetCount; ++i) {
                IList<ContextualSubstRule> subClassSet = null;
                if (subClassSetOffsets[i] != 0) {
                    openReader.rf.Seek(subClassSetOffsets[i]);
                    int subClassRuleCount = openReader.rf.ReadUnsignedShort();
                    int[] subClassRuleOffsets = openReader.ReadUShortArray(subClassRuleCount, subClassSetOffsets[i]);
                    subClassSet = new List<ContextualSubstRule>(subClassRuleCount);
                    for (int j = 0; j < subClassRuleCount; ++j) {
                        ContextualSubstRule rule;
                        openReader.rf.Seek(subClassRuleOffsets[j]);
                        int glyphCount = openReader.rf.ReadUnsignedShort();
                        int substCount = openReader.rf.ReadUnsignedShort();
                        int[] inputClassIds = openReader.ReadUShortArray(glyphCount - 1);
                        SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
                        rule = new SubTableLookup5Format2.SubstRuleFormat2(t, inputClassIds, substLookupRecords);
                        subClassSet.Add(rule);
                    }
                }
                subClassSets.Add(subClassSet);
            }
            t.SetSubClassSets(subClassSets);
            subTables.Add(t);
        }

        protected internal virtual void ReadSubTableFormat3(int subTableLocation) {
            int glyphCount = openReader.rf.ReadUnsignedShort();
            int substCount = openReader.rf.ReadUnsignedShort();
            int[] coverageOffsets = openReader.ReadUShortArray(glyphCount, subTableLocation);
            SubstLookupRecord[] substLookupRecords = openReader.ReadSubstLookupRecords(substCount);
            IList<ICollection<int>> coverages = new List<ICollection<int>>(glyphCount);
            openReader.ReadCoverages(coverageOffsets, coverages);
            SubTableLookup5Format3.SubstRuleFormat3 rule = new SubTableLookup5Format3.SubstRuleFormat3(coverages, substLookupRecords
                );
            subTables.Add(new SubTableLookup5Format3(openReader, lookupFlag, rule));
        }
    }
}
