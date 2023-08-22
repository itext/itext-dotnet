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

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 3: Alternate Substitution Subtable</summary>
    public class GsubLookupType3 : OpenTableLookup {
        private IDictionary<int, int[]> substMap;

        public GsubLookupType3(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            substMap = new Dictionary<int, int[]>();
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            if (line.idx >= line.end) {
                return false;
            }
            Glyph g = line.Get(line.idx);
            bool changed = false;
            if (!openReader.IsSkip(g.GetCode(), lookupFlag)) {
                int[] substCode = substMap.Get(g.GetCode());
                // there is no need to substitute a symbol with itself
                if (substCode != null && substCode[0] != g.GetCode()) {
                    line.SubstituteOneToOne(openReader, substCode[0]);
                    changed = true;
                }
            }
            line.idx++;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadShort();
            System.Diagnostics.Debug.Assert(substFormat == 1);
            int coverage = openReader.rf.ReadUnsignedShort();
            int alternateSetCount = openReader.rf.ReadUnsignedShort();
            int[][] substitute = new int[alternateSetCount][];
            int[] alternateLocations = openReader.ReadUShortArray(alternateSetCount, subTableLocation);
            for (int k = 0; k < alternateSetCount; k++) {
                openReader.rf.Seek(alternateLocations[k]);
                int glyphCount = openReader.rf.ReadUnsignedShort();
                substitute[k] = openReader.ReadUShortArray(glyphCount);
            }
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverage);
            for (int k = 0; k < alternateSetCount; ++k) {
                substMap.Put(coverageGlyphIds[k], substitute[k]);
            }
        }

        public override bool HasSubstitution(int index) {
            return substMap.ContainsKey(index);
        }
    }
}
