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
    public class OpenTypeScript {
        public const String DEFAULT_SCRIPT = "DFLT";

        private OpenTypeFontTableReader openTypeReader;

        private IList<ScriptRecord> records;

        public OpenTypeScript(OpenTypeFontTableReader openTypeReader, int locationScriptTable) {
            this.openTypeReader = openTypeReader;
            records = new List<ScriptRecord>();
            openTypeReader.rf.Seek(locationScriptTable);
            TagAndLocation[] tagsLocs = openTypeReader.ReadTagAndLocations(locationScriptTable);
            foreach (TagAndLocation tagLoc in tagsLocs) {
                ReadScriptRecord(tagLoc);
            }
        }

        public virtual IList<ScriptRecord> GetScriptRecords() {
            return records;
        }

        public virtual LanguageRecord GetLanguageRecord(String[] scripts, String language) {
            ScriptRecord scriptFound = null;
            ScriptRecord scriptDefault = null;
            foreach (ScriptRecord sr in records) {
                if (DEFAULT_SCRIPT.Equals(sr.tag)) {
                    scriptDefault = sr;
                    break;
                }
            }
            foreach (String script in scripts) {
                foreach (ScriptRecord sr in records) {
                    if (sr.tag.Equals(script)) {
                        scriptFound = sr;
                        break;
                    }
                    if (DEFAULT_SCRIPT.Equals(script)) {
                        scriptDefault = sr;
                    }
                }
                if (scriptFound != null) {
                    break;
                }
            }
            if (scriptFound == null) {
                scriptFound = scriptDefault;
            }
            if (scriptFound == null) {
                return null;
            }
            LanguageRecord lang = null;
            foreach (LanguageRecord lr in scriptFound.languages) {
                if (lr.tag.Equals(language)) {
                    lang = lr;
                    break;
                }
            }
            if (lang == null) {
                lang = scriptFound.defaultLanguage;
            }
            return lang;
        }

        private void ReadScriptRecord(TagAndLocation tagLoc) {
            openTypeReader.rf.Seek(tagLoc.location);
            int locationDefaultLanguage = openTypeReader.rf.ReadUnsignedShort();
            if (locationDefaultLanguage > 0) {
                locationDefaultLanguage += tagLoc.location;
            }
            TagAndLocation[] tagsLocs = openTypeReader.ReadTagAndLocations(tagLoc.location);
            ScriptRecord srec = new ScriptRecord();
            srec.tag = tagLoc.tag;
            srec.languages = new LanguageRecord[tagsLocs.Length];
            for (int k = 0; k < tagsLocs.Length; ++k) {
                srec.languages[k] = ReadLanguageRecord(tagsLocs[k]);
            }
            if (locationDefaultLanguage > 0) {
                TagAndLocation t = new TagAndLocation();
                t.tag = "";
                t.location = locationDefaultLanguage;
                srec.defaultLanguage = ReadLanguageRecord(t);
            }
            records.Add(srec);
        }

        private LanguageRecord ReadLanguageRecord(TagAndLocation tagLoc) {
            LanguageRecord rec = new LanguageRecord();
            //skip lookup order
            openTypeReader.rf.Seek(tagLoc.location + 2);
            rec.featureRequired = openTypeReader.rf.ReadUnsignedShort();
            int count = openTypeReader.rf.ReadUnsignedShort();
            rec.features = openTypeReader.ReadUShortArray(count);
            rec.tag = tagLoc.tag;
            return rec;
        }
    }
}
