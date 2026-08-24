/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Internal.Runtime;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    /// <summary>Reads the common structures of an OpenType layout table.</summary>
    public abstract class OpenTypeFontTableReader {
        protected internal readonly RandomAccessFileOrArray rf;

        /// <summary>Stores table location.</summary>
        protected internal readonly int tableLocation;

        /// <summary>Stores lookup list.</summary>
        protected internal IList<OpenTableLookup> lookupList;

        /// <summary>Stores scripts type.</summary>
        protected internal OpenTypeScript scriptsType;

        /// <summary>Stores features type.</summary>
        protected internal OpenTypeFeature featuresType;

        private readonly IDictionary<int, Glyph> indexGlyphMap;

        private readonly OpenTypeGdefTableReader gdef;

        private readonly int unitsPerEm;

        /// <summary>Creates a new reader.</summary>
        /// <param name="rf">the source</param>
        /// <param name="tableLocation">the table location</param>
        /// <param name="gdef">the GDEF reader</param>
        /// <param name="indexGlyphMap">the index glyph map</param>
        /// <param name="unitsPerEm">the units per em</param>
        protected internal OpenTypeFontTableReader(RandomAccessFileOrArray rf, int tableLocation, OpenTypeGdefTableReader
             gdef, IDictionary<int, Glyph> indexGlyphMap, int unitsPerEm) {
            this.rf = rf;
            this.tableLocation = tableLocation;
            this.indexGlyphMap = indexGlyphMap;
            this.gdef = gdef;
            this.unitsPerEm = unitsPerEm;
        }

        /// <summary>Returns the glyph by index.</summary>
        /// <param name="index">the index</param>
        /// <returns>the requested result</returns>
        public virtual Glyph GetGlyph(int index) {
            return indexGlyphMap.Get(index);
        }

        /// <summary>Returns the lookup table.</summary>
        /// <param name="idx">the idx</param>
        /// <returns>the requested result</returns>
        public virtual OpenTableLookup GetLookupTable(int idx) {
            if (idx < 0 || idx >= lookupList.Count) {
                return null;
            }
            return lookupList[idx];
        }

        /// <summary>Returns the script records.</summary>
        /// <returns>the requested result</returns>
        public virtual IList<ScriptRecord> GetScriptRecords() {
            return scriptsType.GetScriptRecords();
        }

        /// <summary>Returns the feature records.</summary>
        /// <returns>the requested result</returns>
        public virtual IList<FeatureRecord> GetFeatureRecords() {
            return featuresType.GetRecords();
        }

        /// <summary>
        /// Returns the features represented by
        /// <see cref="FeatureRecord"/>
        /// list.
        /// </summary>
        /// <param name="scripts">the scripts</param>
        /// <param name="language">the language</param>
        /// <returns>the requested result</returns>
        public virtual IList<FeatureRecord> GetFeatures(String[] scripts, String language) {
            LanguageRecord rec = scriptsType.GetLanguageRecord(scripts, language);
            if (rec == null) {
                return null;
            }
            IList<FeatureRecord> ret = new List<FeatureRecord>();
            foreach (int f in rec.GetFeatures()) {
                ret.Add(featuresType.GetRecord(f));
            }
            return ret;
        }

        /// <summary>
        /// Returns the specific features represented by
        /// <see cref="FeatureRecord"/>
        /// list.
        /// </summary>
        /// <param name="features">
        /// 
        /// <see cref="FeatureRecord"/>
        /// list
        /// </param>
        /// <param name="specific">specific tags of the feature record</param>
        /// <returns>the requested result</returns>
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
                if (hs.Contains(rec.GetTag())) {
                    recs.Add(rec);
                }
            }
            return recs;
        }

        /// <summary>
        /// Returns the required feature represented by
        /// <see cref="FeatureRecord"/>.
        /// </summary>
        /// <param name="scripts">the scripts</param>
        /// <param name="language">the language</param>
        /// <returns>the requested result</returns>
        public virtual FeatureRecord GetRequiredFeature(String[] scripts, String language) {
            LanguageRecord rec = scriptsType.GetLanguageRecord(scripts, language);
            if (rec == null) {
                return null;
            }
            return featuresType.GetRecord(rec.GetFeatureRequired());
        }

        /// <summary>Returns the lookups.</summary>
        /// <param name="features">the features</param>
        /// <returns>the requested result</returns>
        public virtual IList<OpenTableLookup> GetLookups(FeatureRecord[] features) {
            IntHashtable hash = new IntHashtable();
            foreach (FeatureRecord rec in features) {
                foreach (int idx in rec.GetLookups()) {
                    hash.Put(idx, 1);
                }
            }
            IList<OpenTableLookup> ret = new List<OpenTableLookup>();
            foreach (int idx in hash.ToOrderedKeys()) {
                ret.Add(lookupList[idx]);
            }
            return ret;
        }

        /// <summary>Returns the lookups.</summary>
        /// <param name="feature">the feature</param>
        /// <returns>the requested result</returns>
        public virtual IList<OpenTableLookup> GetLookups(FeatureRecord feature) {
            IList<OpenTableLookup> ret = new List<OpenTableLookup>(feature.GetLookups().Length);
            foreach (int idx in feature.GetLookups()) {
                ret.Add(lookupList[idx]);
            }
            return ret;
        }

        /// <summary>Checks if lookup must ignore the specified glyph when processing glyph sequences.</summary>
        /// <param name="glyph">glyph to check</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the specified glyph should be skipped,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsSkip(int glyph, int lookupFlag) {
            return gdef.IsSkip(glyph, lookupFlag);
        }

        /// <summary>Returns the glyph class.</summary>
        /// <param name="glyphCode">the glyph code</param>
        /// <returns>the requested result</returns>
        public virtual int GetGlyphClass(int glyphCode) {
            return gdef.GetGlyphClassTable().GetOtfClass(glyphCode);
        }

        /// <summary>Returns the units per em.</summary>
        /// <returns>the requested result</returns>
        public virtual int GetUnitsPerEm() {
            return unitsPerEm;
        }

        /// <summary>Returns the language record.</summary>
        /// <param name="otfScriptTag">the otf script tag</param>
        /// <returns>the requested result</returns>
        public virtual LanguageRecord GetLanguageRecord(String otfScriptTag) {
            return GetLanguageRecord(otfScriptTag, null);
        }

        /// <summary>Returns the language record.</summary>
        /// <param name="otfScriptTag">the otf script tag</param>
        /// <param name="langTag">the lang tag</param>
        /// <returns>the requested result</returns>
        public virtual LanguageRecord GetLanguageRecord(String otfScriptTag, String langTag) {
            if (otfScriptTag == null) {
                return null;
            }
            foreach (ScriptRecord record in GetScriptRecords()) {
                if (!otfScriptTag.Equals(record.GetTag())) {
                    continue;
                }
                if (langTag == null) {
                    return record.GetDefaultLanguage();
                }
                foreach (LanguageRecord lang in record.GetLanguages()) {
                    if (langTag.Equals(lang.GetTag())) {
                        return lang;
                    }
                }
            }
            return null;
        }

        /// <summary>Reads the lookup table from OpenType data.</summary>
        /// <param name="lookupType">the lookup type</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <param name="subTableLocations">the sub table locations</param>
        /// <returns>the requested result</returns>
        protected internal abstract OpenTableLookup ReadLookupTable(int lookupType, int lookupFlag, int[] subTableLocations
            );

        /// <summary>Reads the class definition from OpenType data.</summary>
        /// <param name="classLocation">the class location</param>
        /// <returns>the requested result</returns>
        protected internal OtfClass ReadClassDefinition(int classLocation) {
            return OtfClass.Create(rf, classLocation);
        }

        /// <summary>Reads the ushort array from OpenType data.</summary>
        /// <param name="size">the size</param>
        /// <param name="location">the location</param>
        /// <returns>the requested result</returns>
        protected internal int[] ReadUShortArray(int size, int location) {
            return OtfReadCommon.ReadUShortArray(rf, size, location);
        }

        /// <summary>Reads the ushort array from OpenType data.</summary>
        /// <param name="size">the size</param>
        /// <returns>the requested result</returns>
        protected internal int[] ReadUShortArray(int size) {
            return OtfReadCommon.ReadUShortArray(rf, size);
        }

        /// <summary>Reads the coverages from OpenType data.</summary>
        /// <param name="locations">the locations</param>
        /// <param name="coverage">the coverage to retain the result</param>
        protected internal virtual void ReadCoverages(int[] locations, IList<ICollection<int>> coverage) {
            OtfReadCommon.ReadCoverages(rf, locations, coverage);
        }

        /// <summary>Reads the coverage format from OpenType data.</summary>
        /// <param name="coverageLocation">the coverage location</param>
        /// <returns>the requested result</returns>
        protected internal IList<int> ReadCoverageFormat(int coverageLocation) {
            return OtfReadCommon.ReadCoverageFormat(rf, coverageLocation);
        }

        /// <summary>Reads the substitution lookup records from OpenType data.</summary>
        /// <param name="substCount">the substitution lookups count to read</param>
        /// <returns>the requested result</returns>
        protected internal virtual SubstLookupRecord[] ReadSubstLookupRecords(int substCount) {
            return OtfReadCommon.ReadSubstLookupRecords(rf, substCount);
        }

        /// <summary>Reads the positioning lookup records from OpenType data.</summary>
        /// <param name="substCount">the positioning lookups count to read</param>
        /// <returns>the requested result</returns>
        protected internal virtual PosLookupRecord[] ReadPosLookupRecords(int substCount) {
            return OtfReadCommon.ReadPosLookupRecords(rf, substCount);
        }

        /// <summary>Reads the tag and locations from OpenType data.</summary>
        /// <param name="baseLocation">the base location</param>
        /// <returns>the requested result</returns>
        protected internal virtual TagAndLocation[] ReadTagAndLocations(int baseLocation) {
            int count = rf.ReadUnsignedShort();
            TagAndLocation[] tagslLocs = new TagAndLocation[count];
            for (int k = 0; k < count; ++k) {
                TagAndLocation tl = new TagAndLocation();
                tl.SetTag(rf.ReadString(4, "utf-8"));
                tl.SetLocation(rf.ReadUnsignedShort() + baseLocation);
                tagslLocs[k] = tl;
            }
            return tagslLocs;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>This is the starting point of the class.</summary>
        /// <remarks>
        /// This is the starting point of the class. A subclass must call this
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
//\endcond

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
            OpenTableLookup lookup = ReadLookupTable(lookupType, lookupFlag, subTableLocations);
            if (lookup != null) {
                lookup.SetIndexInLookupList(lookupList.Count);
            }
            lookupList.Add(lookup);
        }
    }
}
