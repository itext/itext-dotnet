/*
*
* This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
* Authors: Bruno Lowagie, Paulo Soares, et al.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License version 3
* as published by the Free Software Foundation with the addition of the
* following permission added to Section 15 as permitted in Section 7(a):
* FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
* ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
* OF THIRD PARTY RIGHTS
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
* or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU Affero General Public License for more details.
* You should have received a copy of the GNU Affero General Public License
* along with this program; if not, see http://www.gnu.org/licenses or write to
* the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
* Boston, MA, 02110-1301 USA, or download the license from the following URL:
* http://itextpdf.com/terms-of-use/
*
* The interactive user interfaces in modified source and object code versions
* of this program must display Appropriate Legal Notices, as required under
* Section 5 of the GNU Affero General Public License.
*
* In accordance with Section 7(b) of the GNU Affero General Public License,
* a covered work must retain the producer line in every PDF that is created
* or manipulated using iText.
*
* You can be released from the requirements of the license by purchasing
* a commercial license. Buying such a license is mandatory as soon as you
* develop commercial activities involving the iText software without
* disclosing the source code of your own applications.
* These activities include: offering paid services to customers as an ASP,
* serving PDFs on the fly in a web application, shipping iText with a closed
* source product.
*
* For more information, please contact iText Software Corp. at this
* address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.IO.Font.Otf {
    /// <author><a href="mailto:paawak@gmail.com">Palash Ray</a></author>
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
