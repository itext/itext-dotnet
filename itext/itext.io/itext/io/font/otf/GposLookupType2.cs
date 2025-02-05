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
            if (line.GetIdx() >= line.GetEnd()) {
                return false;
            }
            if (openReader.IsSkip(line.Get(line.GetIdx()).GetCode(), lookupFlag)) {
                line.SetIdx(line.GetIdx() + 1);
                return false;
            }
            foreach (OpenTableLookup lookup in listRules) {
                if (lookup.TransformOne(line)) {
                    return true;
                }
            }
            line.SetIdx(line.GetIdx() + 1);
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
                if (line.GetIdx() >= line.GetEnd() || line.GetIdx() < line.GetStart()) {
                    return false;
                }
                bool changed = false;
                Glyph g1 = line.Get(line.GetIdx());
                IDictionary<int, GposLookupType2.PairValueFormat> m = gposMap.Get(g1.GetCode());
                if (m != null) {
                    OpenTableLookup.GlyphIndexer gi = new OpenTableLookup.GlyphIndexer();
                    gi.SetLine(line);
                    gi.SetIdx(line.GetIdx());
                    gi.NextGlyph(openReader, lookupFlag);
                    if (gi.GetGlyph() != null) {
                        GposLookupType2.PairValueFormat pv = m.Get(gi.GetGlyph().GetCode());
                        if (pv != null) {
                            Glyph g2 = gi.GetGlyph();
                            line.Set(line.GetIdx(), new Glyph(g1, 0, 0, pv.GetFirst().GetXAdvance(), pv.GetFirst().GetYAdvance(), 0));
                            line.Set(gi.GetIdx(), new Glyph(g2, 0, 0, pv.GetSecond().GetXAdvance(), pv.GetSecond().GetYAdvance(), 0));
                            line.SetIdx(gi.GetIdx());
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
                        pair.SetFirst(OtfReadCommon.ReadGposValueRecord(openReader, valueFormat1));
                        pair.SetSecond(OtfReadCommon.ReadGposValueRecord(openReader, valueFormat2));
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
                if (line.GetIdx() >= line.GetEnd() || line.GetIdx() < line.GetStart()) {
                    return false;
                }
                Glyph g1 = line.Get(line.GetIdx());
                if (!coverageSet.Contains(g1.GetCode())) {
                    return false;
                }
                int c1 = classDef1.GetOtfClass(g1.GetCode());
                GposLookupType2.PairValueFormat[] pvs = posSubs.Get(c1);
                if (pvs == null) {
                    return false;
                }
                OpenTableLookup.GlyphIndexer gi = new OpenTableLookup.GlyphIndexer();
                gi.SetLine(line);
                gi.SetIdx(line.GetIdx());
                gi.NextGlyph(openReader, lookupFlag);
                if (gi.GetGlyph() == null) {
                    return false;
                }
                Glyph g2 = gi.GetGlyph();
                int c2 = classDef2.GetOtfClass(g2.GetCode());
                if (c2 >= pvs.Length) {
                    return false;
                }
                GposLookupType2.PairValueFormat pv = pvs[c2];
                line.Set(line.GetIdx(), new Glyph(g1, 0, 0, pv.GetFirst().GetXAdvance(), pv.GetFirst().GetYAdvance(), 0));
                line.Set(gi.GetIdx(), new Glyph(g2, 0, 0, pv.GetSecond().GetXAdvance(), pv.GetSecond().GetYAdvance(), 0));
                line.SetIdx(gi.GetIdx());
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
                        pair.SetFirst(OtfReadCommon.ReadGposValueRecord(openReader, valueFormat1));
                        pair.SetSecond(OtfReadCommon.ReadGposValueRecord(openReader, valueFormat2));
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
            private GposValueRecord first;

            private GposValueRecord second;

            /// <summary>Retrieves the first object of the pair.</summary>
            /// <returns>first object</returns>
            public virtual GposValueRecord GetFirst() {
                return first;
            }

            /// <summary>Sets the first object of the pair.</summary>
            /// <param name="first">first object</param>
            public virtual void SetFirst(GposValueRecord first) {
                this.first = first;
            }

            /// <summary>Retrieves the second object of the pair.</summary>
            /// <returns>second object</returns>
            public virtual GposValueRecord GetSecond() {
                return second;
            }

            /// <summary>Sets the second object of the pair.</summary>
            /// <param name="second">second object</param>
            public virtual void SetSecond(GposValueRecord second) {
                this.second = second;
            }
        }
    }
}
