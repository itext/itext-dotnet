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
using System.Collections.Generic;

namespace iText.IO.Font.Otf {
    /// <summary>
    /// Lookup Type 2:
    /// Pair Adjustment Positioning Subtable
    /// </summary>
    public class GposLookupType2 : OpenTableLookup {
        private IList<OpenTableLookup> listRules = new List<OpenTableLookup>();

        public GposLookupType2(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
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
            foreach (OpenTableLookup lookup in listRules) {
                if (lookup.TransformOne(line)) {
                    return true;
                }
            }
            ++line.idx;
            return false;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            int gposFormat = openReader.rf.ReadShort();
            switch (gposFormat) {
                case 1: {
                    GposLookupType2.PairPosAdjustmentFormat1 format1 = new GposLookupType2.PairPosAdjustmentFormat1(openReader
                        , lookupFlag, subTableLocation);
                    listRules.Add(format1);
                    break;
                }

                case 2: {
                    GposLookupType2.PairPosAdjustmentFormat2 format2 = new GposLookupType2.PairPosAdjustmentFormat2(openReader
                        , lookupFlag, subTableLocation);
                    listRules.Add(format2);
                    break;
                }
            }
        }

        private class PairPosAdjustmentFormat1 : OpenTableLookup {
            private IDictionary<int, IDictionary<int, GposLookupType2.PairValueFormat>> gposMap = new Dictionary<int, 
                IDictionary<int, GposLookupType2.PairValueFormat>>();

            public PairPosAdjustmentFormat1(OpenTypeFontTableReader openReader, int lookupFlag, int subtableLocation)
                : base(openReader, lookupFlag, null) {
                ReadFormat(subtableLocation);
            }

            public override bool TransformOne(GlyphLine line) {
                if (line.idx >= line.end || line.idx < line.start) {
                    return false;
                }
                bool changed = false;
                Glyph g1 = line.Get(line.idx);
                IDictionary<int, GposLookupType2.PairValueFormat> m = gposMap.Get(g1.GetCode());
                if (m != null) {
                    OpenTableLookup.GlyphIndexer gi = new OpenTableLookup.GlyphIndexer();
                    gi.line = line;
                    gi.idx = line.idx;
                    gi.NextGlyph(openReader, lookupFlag);
                    if (gi.glyph != null) {
                        GposLookupType2.PairValueFormat pv = m.Get(gi.glyph.GetCode());
                        if (pv != null) {
                            Glyph g2 = gi.glyph;
                            line.Set(line.idx, new Glyph(g1, 0, 0, pv.first.XAdvance, pv.first.YAdvance, 0));
                            line.Set(gi.idx, new Glyph(g2, 0, 0, pv.second.XAdvance, pv.second.YAdvance, 0));
                            line.idx = gi.idx;
                            changed = true;
                        }
                    }
                }
                return changed;
            }

            protected internal virtual void ReadFormat(int subTableLocation) {
                int coverage = openReader.rf.ReadUnsignedShort() + subTableLocation;
                int valueFormat1 = openReader.rf.ReadUnsignedShort();
                int valueFormat2 = openReader.rf.ReadUnsignedShort();
                int pairSetCount = openReader.rf.ReadUnsignedShort();
                int[] locationRule = openReader.ReadUShortArray(pairSetCount, subTableLocation);
                IList<int> coverageList = openReader.ReadCoverageFormat(coverage);
                for (int k = 0; k < pairSetCount; ++k) {
                    openReader.rf.Seek(locationRule[k]);
                    IDictionary<int, GposLookupType2.PairValueFormat> pairs = new Dictionary<int, GposLookupType2.PairValueFormat
                        >();
                    gposMap.Put(coverageList[k], pairs);
                    int pairValueCount = openReader.rf.ReadUnsignedShort();
                    for (int j = 0; j < pairValueCount; ++j) {
                        int glyph2 = openReader.rf.ReadUnsignedShort();
                        GposLookupType2.PairValueFormat pair = new GposLookupType2.PairValueFormat();
                        pair.first = OtfReadCommon.ReadGposValueRecord(openReader, valueFormat1);
                        pair.second = OtfReadCommon.ReadGposValueRecord(openReader, valueFormat2);
                        pairs.Put(glyph2, pair);
                    }
                }
            }

            protected internal override void ReadSubTable(int subTableLocation) {
            }
            //never called here
        }

        private class PairPosAdjustmentFormat2 : OpenTableLookup {
            private OtfClass classDef1;

            private OtfClass classDef2;

            private HashSet<int> coverageSet;

            private IDictionary<int, GposLookupType2.PairValueFormat[]> posSubs = new Dictionary<int, GposLookupType2.PairValueFormat
                []>();

            public PairPosAdjustmentFormat2(OpenTypeFontTableReader openReader, int lookupFlag, int subtableLocation)
                : base(openReader, lookupFlag, null) {
                ReadFormat(subtableLocation);
            }

            public override bool TransformOne(GlyphLine line) {
                if (line.idx >= line.end || line.idx < line.start) {
                    return false;
                }
                Glyph g1 = line.Get(line.idx);
                if (!coverageSet.Contains(g1.GetCode())) {
                    return false;
                }
                int c1 = classDef1.GetOtfClass(g1.GetCode());
                GposLookupType2.PairValueFormat[] pvs = posSubs.Get(c1);
                if (pvs == null) {
                    return false;
                }
                OpenTableLookup.GlyphIndexer gi = new OpenTableLookup.GlyphIndexer();
                gi.line = line;
                gi.idx = line.idx;
                gi.NextGlyph(openReader, lookupFlag);
                if (gi.glyph == null) {
                    return false;
                }
                Glyph g2 = gi.glyph;
                int c2 = classDef2.GetOtfClass(g2.GetCode());
                if (c2 >= pvs.Length) {
                    return false;
                }
                GposLookupType2.PairValueFormat pv = pvs[c2];
                line.Set(line.idx, new Glyph(g1, 0, 0, pv.first.XAdvance, pv.first.YAdvance, 0));
                line.Set(gi.idx, new Glyph(g2, 0, 0, pv.second.XAdvance, pv.second.YAdvance, 0));
                line.idx = gi.idx;
                return true;
            }

            protected internal virtual void ReadFormat(int subTableLocation) {
                int coverage = openReader.rf.ReadUnsignedShort() + subTableLocation;
                int valueFormat1 = openReader.rf.ReadUnsignedShort();
                int valueFormat2 = openReader.rf.ReadUnsignedShort();
                int locationClass1 = openReader.rf.ReadUnsignedShort() + subTableLocation;
                int locationClass2 = openReader.rf.ReadUnsignedShort() + subTableLocation;
                int class1Count = openReader.rf.ReadUnsignedShort();
                int class2Count = openReader.rf.ReadUnsignedShort();
                for (int k = 0; k < class1Count; ++k) {
                    GposLookupType2.PairValueFormat[] pairs = new GposLookupType2.PairValueFormat[class2Count];
                    posSubs.Put(k, pairs);
                    for (int j = 0; j < class2Count; ++j) {
                        GposLookupType2.PairValueFormat pair = new GposLookupType2.PairValueFormat();
                        pair.first = OtfReadCommon.ReadGposValueRecord(openReader, valueFormat1);
                        pair.second = OtfReadCommon.ReadGposValueRecord(openReader, valueFormat2);
                        pairs[j] = pair;
                    }
                }
                coverageSet = new HashSet<int>(openReader.ReadCoverageFormat(coverage));
                classDef1 = openReader.ReadClassDefinition(locationClass1);
                classDef2 = openReader.ReadClassDefinition(locationClass2);
            }

            protected internal override void ReadSubTable(int subTableLocation) {
            }
            //never called here
        }

        private class PairValueFormat {
            public GposValueRecord first;

            public GposValueRecord second;
        }
    }
}
