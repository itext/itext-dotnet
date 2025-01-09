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

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 4: Ligature Substitution Subtable</summary>
    public class GsubLookupType4 : OpenTableLookup {
        /// <summary>The key is the first character.</summary>
        /// <remarks>
        /// The key is the first character. The first element in the int array is the
        /// output ligature
        /// </remarks>
        private IDictionary<int, IList<int[]>> ligatures;

        public GsubLookupType4(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            ligatures = new Dictionary<int, IList<int[]>>();
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            if (line.GetIdx() >= line.GetEnd()) {
                return false;
            }
            bool changed = false;
            Glyph g = line.Get(line.GetIdx());
            bool match = false;
            if (ligatures.ContainsKey(g.GetCode()) && !openReader.IsSkip(g.GetCode(), lookupFlag)) {
                OpenTableLookup.GlyphIndexer gidx = new OpenTableLookup.GlyphIndexer();
                gidx.SetLine(line);
                IList<int[]> ligs = ligatures.Get(g.GetCode());
                foreach (int[] lig in ligs) {
                    match = true;
                    gidx.SetIdx(line.GetIdx());
                    for (int j = 1; j < lig.Length; ++j) {
                        gidx.NextGlyph(openReader, lookupFlag);
                        if (gidx.GetGlyph() == null || gidx.GetGlyph().GetCode() != lig[j]) {
                            match = false;
                            break;
                        }
                    }
                    if (match) {
                        line.SubstituteManyToOne(openReader, lookupFlag, lig.Length - 1, lig[0]);
                        break;
                    }
                }
            }
            if (match) {
                changed = true;
            }
            line.SetIdx(line.GetIdx() + 1);
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            // subformat - always 1
            openReader.rf.ReadShort();
            int coverage = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int ligSetCount = openReader.rf.ReadUnsignedShort();
            int[] ligatureSet = new int[ligSetCount];
            for (int k = 0; k < ligSetCount; ++k) {
                ligatureSet[k] = openReader.rf.ReadUnsignedShort() + subTableLocation;
            }
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(coverage);
            for (int k = 0; k < ligSetCount; ++k) {
                openReader.rf.Seek(ligatureSet[k]);
                int ligatureCount = openReader.rf.ReadUnsignedShort();
                int[] ligature = new int[ligatureCount];
                for (int j = 0; j < ligatureCount; ++j) {
                    ligature[j] = openReader.rf.ReadUnsignedShort() + ligatureSet[k];
                }
                IList<int[]> components = new List<int[]>(ligatureCount);
                for (int j = 0; j < ligatureCount; ++j) {
                    openReader.rf.Seek(ligature[j]);
                    int ligGlyph = openReader.rf.ReadUnsignedShort();
                    int compCount = openReader.rf.ReadUnsignedShort();
                    int[] component = new int[compCount];
                    component[0] = ligGlyph;
                    for (int i = 1; i < compCount; ++i) {
                        component[i] = openReader.rf.ReadUnsignedShort();
                    }
                    components.Add(component);
                }
                ligatures.Put(coverageGlyphIds[k], components);
            }
        }
    }
}
