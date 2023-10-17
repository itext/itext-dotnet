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
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    public abstract class OpenTypeFontTableReader {
        protected internal readonly RandomAccessFileOrArray rf;

        protected internal readonly int tableLocation;

        protected internal IList<OpenTableLookup> lookupList;

        protected internal OpenTypeScript scriptsType;

        protected internal OpenTypeFeature featuresType;

        private readonly IDictionary<int, Glyph> indexGlyphMap;

        private readonly OpenTypeGdefTableReader gdef;

        private readonly int unitsPerEm;

        protected internal OpenTypeFontTableReader(RandomAccessFileOrArray rf, int tableLocation, OpenTypeGdefTableReader
             gdef, IDictionary<int, Glyph> indexGlyphMap, int unitsPerEm) {
            this.rf = rf;
            this.tableLocation = tableLocation;
            this.indexGlyphMap = indexGlyphMap;
            this.gdef = gdef;
            this.unitsPerEm = unitsPerEm;
        }

        public virtual Glyph GetGlyph(int index) {
            return indexGlyphMap.Get(index);
        }

        public virtual OpenTableLookup GetLookupTable(int idx) {
            if (idx < 0 || idx >= lookupList.Count) {
                return null;
            }
            return lookupList[idx];
        }

        public virtual IList<ScriptRecord> GetScriptRecords() {
            return scriptsType.GetScriptRecords();
        }

        public virtual IList<FeatureRecord> GetFeatureRecords() {
            return featuresType.GetRecords();
        }

        public virtual IList<FeatureRecord> GetFeatures(String[] scripts, String language) {
            LanguageRecord rec = scriptsType.GetLanguageRecord(scripts, language);
            if (rec == null) {
                return null;
            }
            IList<FeatureRecord> ret = new List<FeatureRecord>();
            foreach (int f in rec.features) {
                ret.Add(featuresType.GetRecord(f));
            }
            return ret;
        }

        public virtual IList<FeatureRecord> GetSpecificFeatures(IList<FeatureRecord> features, String[] specific) {
            if (specific == null) {
                return features;
            }
            ICollection<String> hs = new HashSet<String>();
            //noinspection ManualArrayToCollectionCopy
            foreach (String s in specific) {
                hs.Add(s);
            }
            IList<FeatureRecord> recs = new List<FeatureRecord>();
            foreach (FeatureRecord rec in features) {
                if (hs.Contains(rec.tag)) {
                    recs.Add(rec);
                }
            }
            return recs;
        }

        public virtual FeatureRecord GetRequiredFeature(String[] scripts, String language) {
            LanguageRecord rec = scriptsType.GetLanguageRecord(scripts, language);
            if (rec == null) {
                return null;
            }
            return featuresType.GetRecord(rec.featureRequired);
        }

        public virtual IList<OpenTableLookup> GetLookups(FeatureRecord[] features) {
            IntHashtable hash = new IntHashtable();
            foreach (FeatureRecord rec in features) {
                foreach (int idx in rec.lookups) {
                    hash.Put(idx, 1);
                }
            }
            IList<OpenTableLookup> ret = new List<OpenTableLookup>();
            foreach (int idx in hash.ToOrderedKeys()) {
                ret.Add(lookupList[idx]);
            }
            return ret;
        }

        public virtual IList<OpenTableLookup> GetLookups(FeatureRecord feature) {
            IList<OpenTableLookup> ret = new List<OpenTableLookup>(feature.lookups.Length);
            foreach (int idx in feature.lookups) {
                ret.Add(lookupList[idx]);
            }
            return ret;
        }

        public virtual bool IsSkip(int glyph, int flag) {
            return gdef.IsSkip(glyph, flag);
        }

        public virtual int GetGlyphClass(int glyphCode) {
            return gdef.GetGlyphClassTable().GetOtfClass(glyphCode);
        }

        public virtual int GetUnitsPerEm() {
            return unitsPerEm;
        }

        public virtual LanguageRecord GetLanguageRecord(String otfScriptTag) {
            return GetLanguageRecord(otfScriptTag, null);
        }

        public virtual LanguageRecord GetLanguageRecord(String otfScriptTag, String langTag) {
            if (otfScriptTag == null) {
                return null;
            }
            foreach (ScriptRecord record in GetScriptRecords()) {
                if (!otfScriptTag.Equals(record.tag)) {
                    continue;
                }
                if (langTag == null) {
                    return record.defaultLanguage;
                }
                foreach (LanguageRecord lang in record.languages) {
                    if (langTag.Equals(lang.tag)) {
                        return lang;
                    }
                }
            }
            return null;
        }

        protected internal abstract OpenTableLookup ReadLookupTable(int lookupType, int lookupFlag, int[] subTableLocations
            );

        protected internal OtfClass ReadClassDefinition(int classLocation) {
            return OtfClass.Create(rf, classLocation);
        }

        protected internal int[] ReadUShortArray(int size, int location) {
            return OtfReadCommon.ReadUShortArray(rf, size, location);
        }

        protected internal int[] ReadUShortArray(int size) {
            return OtfReadCommon.ReadUShortArray(rf, size);
        }

        protected internal virtual void ReadCoverages(int[] locations, IList<ICollection<int>> coverage) {
            OtfReadCommon.ReadCoverages(rf, locations, coverage);
        }

        protected internal IList<int> ReadCoverageFormat(int coverageLocation) {
            return OtfReadCommon.ReadCoverageFormat(rf, coverageLocation);
        }

        protected internal virtual SubstLookupRecord[] ReadSubstLookupRecords(int substCount) {
            return OtfReadCommon.ReadSubstLookupRecords(rf, substCount);
        }

        protected internal virtual PosLookupRecord[] ReadPosLookupRecords(int substCount) {
            return OtfReadCommon.ReadPosLookupRecords(rf, substCount);
        }

        protected internal virtual TagAndLocation[] ReadTagAndLocations(int baseLocation) {
            int count = rf.ReadUnsignedShort();
            TagAndLocation[] tagslLocs = new TagAndLocation[count];
            for (int k = 0; k < count; ++k) {
                TagAndLocation tl = new TagAndLocation();
                tl.tag = rf.ReadString(4, "utf-8");
                tl.location = rf.ReadUnsignedShort() + baseLocation;
                tagslLocs[k] = tl;
            }
            return tagslLocs;
        }

        /// <summary>This is the starting point of the class.</summary>
        /// <remarks>
        /// This is the starting point of the class. A sub-class must call this
        /// method to start getting call backs to the
        /// <see cref="ReadLookupTable(int, int, int[])"/>
        /// method.
        /// </remarks>
        internal void StartReadingTable() {
            try {
                rf.Seek(tableLocation);
                /*int version =*/
                // version not used
                rf.ReadInt();
                int scriptListOffset = rf.ReadUnsignedShort();
                int featureListOffset = rf.ReadUnsignedShort();
                int lookupListOffset = rf.ReadUnsignedShort();
                // read the Script tables
                scriptsType = new OpenTypeScript(this, tableLocation + scriptListOffset);
                // read Feature table
                featuresType = new OpenTypeFeature(this, tableLocation + featureListOffset);
                // read LookUpList table
                ReadLookupListTable(tableLocation + lookupListOffset);
            }
            catch (System.IO.IOException e) {
                throw new FontReadingException("Error reading font file", e);
            }
        }

        private void ReadLookupListTable(int lookupListTableLocation) {
            lookupList = new List<OpenTableLookup>();
            rf.Seek(lookupListTableLocation);
            int lookupCount = rf.ReadUnsignedShort();
            int[] lookupTableLocations = ReadUShortArray(lookupCount, lookupListTableLocation);
            // read LookUp tables
            foreach (int lookupLocation in lookupTableLocations) {
                // be tolerant to NULL offset in LookupList table
                if (lookupLocation == 0) {
                    continue;
                }
                ReadLookupTable(lookupLocation);
            }
        }

        private void ReadLookupTable(int lookupTableLocation) {
            rf.Seek(lookupTableLocation);
            int lookupType = rf.ReadUnsignedShort();
            int lookupFlag = rf.ReadUnsignedShort();
            int subTableCount = rf.ReadUnsignedShort();
            int[] subTableLocations = ReadUShortArray(subTableCount, lookupTableLocation);
            lookupList.Add(ReadLookupTable(lookupType, lookupFlag, subTableLocations));
        }
    }
}
