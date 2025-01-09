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
    public class OpenTypeFeature {
        private OpenTypeFontTableReader openTypeReader;

        private IList<FeatureRecord> records;

        public OpenTypeFeature(OpenTypeFontTableReader openTypeReader, int locationFeatureTable) {
            this.openTypeReader = openTypeReader;
            records = new List<FeatureRecord>();
            openTypeReader.rf.Seek(locationFeatureTable);
            TagAndLocation[] tagsLocs = openTypeReader.ReadTagAndLocations(locationFeatureTable);
            foreach (TagAndLocation tagLoc in tagsLocs) {
                // +2 don't use FeatureParams
                openTypeReader.rf.Seek(tagLoc.GetLocation() + 2L);
                int lookupCount = openTypeReader.rf.ReadUnsignedShort();
                FeatureRecord rec = new FeatureRecord();
                rec.SetTag(tagLoc.GetTag());
                rec.SetLookups(openTypeReader.ReadUShortArray(lookupCount));
                records.Add(rec);
            }
        }

        public virtual IList<FeatureRecord> GetRecords() {
            return records;
        }

        public virtual FeatureRecord GetRecord(int idx) {
            if (idx < 0 || idx >= records.Count) {
                return null;
            }
            return records[idx];
        }
    }
}
