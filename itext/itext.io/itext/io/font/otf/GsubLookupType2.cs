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

namespace iText.IO.Font.Otf {
    /// <summary>LookupType 2: Multiple Substitution Subtable</summary>
    public class GsubLookupType2 : OpenTableLookup {
        private IDictionary<int, int[]> substMap;

        public GsubLookupType2(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
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
                int[] substSequence = substMap.Get(g.GetCode());
                if (substSequence != null) {
                    // The use of multiple substitution for deletion of an input glyph is prohibited. GlyphCount should always be greater than 0.
                    if (substSequence.Length > 0) {
                        line.SubstituteOneToMany(openReader, substSequence);
                        changed = true;
                    }
                }
            }
            line.idx++;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int substFormat = openReader.rf.ReadUnsignedShort();
            if (substFormat == 1) {
                int coverage = openReader.rf.ReadUnsignedShort();
                int sequenceCount = openReader.rf.ReadUnsignedShort();
                int[] sequenceLocations = openReader.ReadUShortArray(sequenceCount, subTableLocation);
                IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverage);
                for (int i = 0; i < sequenceCount; ++i) {
                    openReader.rf.Seek(sequenceLocations[i]);
                    int glyphCount = openReader.rf.ReadUnsignedShort();
                    substMap.Put(coverageGlyphIds[i], openReader.ReadUShortArray(glyphCount));
                }
            }
            else {
                throw new ArgumentException("Bad substFormat: " + substFormat);
            }
        }

        public override bool HasSubstitution(int index) {
            return substMap.ContainsKey(index);
        }
    }
}
