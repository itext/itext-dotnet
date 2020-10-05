/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
    /// <summary>Lookup Type 1: Single Adjustment Positioning Subtable</summary>
    public class GposLookupType1 : OpenTableLookup {
        private IDictionary<int, GposValueRecord> valueRecordMap = new Dictionary<int, GposValueRecord>();

        public GposLookupType1(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            ReadSubTables();
        }

        public override bool TransformOne(GlyphLine line) {
            if (line.idx >= line.end) {
                return false;
            }
            if (openReader.IsSkip(line.Get(line.idx).GetCode(), lookupFlag)) {
                line.idx++;
                return false;
            }
            int glyphCode = line.Get(line.idx).GetCode();
            bool positionApplied = false;
            GposValueRecord valueRecord = valueRecordMap.Get(glyphCode);
            if (valueRecord != null) {
                Glyph newGlyph = new Glyph(line.Get(line.idx));
                newGlyph.xAdvance += (short)valueRecord.XAdvance;
                newGlyph.yAdvance += (short)valueRecord.YAdvance;
                line.Set(line.idx, newGlyph);
                positionApplied = true;
            }
            line.idx++;
            return positionApplied;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            openReader.rf.ReadShort();
            int coverage = openReader.rf.ReadUnsignedShort();
            int valueFormat = openReader.rf.ReadUnsignedShort();
            GposValueRecord valueRecord = OtfReadCommon.ReadGposValueRecord(openReader, valueFormat);
            IList<int> coverageGlyphIds = openReader.ReadCoverageFormat(subTableLocation + coverage);
            foreach (int? glyphId in coverageGlyphIds) {
                valueRecordMap.Put((int)glyphId, valueRecord);
            }
        }
    }
}
