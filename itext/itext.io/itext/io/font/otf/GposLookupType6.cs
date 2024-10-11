/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    /// Lookup Type 6:
    /// MarkToMark Attachment Positioning Subtable
    /// </summary>
    public class GposLookupType6 : OpenTableLookup {
        private readonly IList<GposLookupType6.MarkToBaseMark> marksbases;

        public GposLookupType6(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            marksbases = new List<GposLookupType6.MarkToBaseMark>();
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
            bool changed = false;
            OpenTableLookup.GlyphIndexer gi = null;
            foreach (GposLookupType6.MarkToBaseMark mb in marksbases) {
                OtfMarkRecord omr = mb.marks.Get(line.Get(line.GetIdx()).GetCode());
                if (omr == null) {
                    continue;
                }
                if (gi == null) {
                    gi = new OpenTableLookup.GlyphIndexer();
                    gi.SetIdx(line.GetIdx());
                    gi.SetLine(line);
                    while (true) {
                        int prev = gi.GetIdx();
                        // avoid attaching this mark glyph to another very distant mark glyph
                        bool foundBaseGlyph = false;
                        gi.PreviousGlyph(openReader, lookupFlag);
                        if (gi.GetIdx() != -1) {
                            for (int i = gi.GetIdx(); i < prev; i++) {
                                if (openReader.GetGlyphClass(line.Get(i).GetCode()) == OtfClass.GLYPH_BASE) {
                                    foundBaseGlyph = true;
                                    break;
                                }
                            }
                        }
                        if (foundBaseGlyph) {
                            gi.SetGlyph(null);
                            break;
                        }
                        if (gi.GetGlyph() == null) {
                            break;
                        }
                        if (mb.baseMarks.ContainsKey(gi.GetGlyph().GetCode())) {
                            break;
                        }
                    }
                    if (gi.GetGlyph() == null) {
                        break;
                    }
                }
                GposAnchor[] gpas = mb.baseMarks.Get(gi.GetGlyph().GetCode());
                if (gpas == null) {
                    continue;
                }
                int markClass = omr.GetMarkClass();
                GposAnchor baseAnchor = gpas[markClass];
                GposAnchor markAnchor = omr.GetAnchor();
                line.Set(line.GetIdx(), new Glyph(line.Get(line.GetIdx()), -markAnchor.GetXCoordinate() + baseAnchor.GetXCoordinate
                    (), -markAnchor.GetYCoordinate() + baseAnchor.GetYCoordinate(), 0, 0, gi.GetIdx() - line.GetIdx()));
                changed = true;
                break;
            }
            line.SetIdx(line.GetIdx() + 1);
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            // skip format, always 1
            openReader.rf.ReadUnsignedShort();
            int markCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int baseCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int classCount = openReader.rf.ReadUnsignedShort();
            int markArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int baseArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            IList<int> markCoverage = openReader.ReadCoverageFormat(markCoverageLocation);
            IList<int> baseCoverage = openReader.ReadCoverageFormat(baseCoverageLocation);
            IList<OtfMarkRecord> markRecords = OtfReadCommon.ReadMarkArray(openReader, markArrayLocation);
            GposLookupType6.MarkToBaseMark markToBaseMark = new GposLookupType6.MarkToBaseMark();
            for (int k = 0; k < markCoverage.Count; ++k) {
                markToBaseMark.marks.Put(markCoverage[k], markRecords[k]);
            }
            IList<GposAnchor[]> baseArray = OtfReadCommon.ReadBaseArray(openReader, classCount, baseArrayLocation);
            for (int k = 0; k < baseCoverage.Count; ++k) {
                markToBaseMark.baseMarks.Put(baseCoverage[k], baseArray[k]);
            }
            marksbases.Add(markToBaseMark);
        }

        private class MarkToBaseMark {
            public readonly IDictionary<int, OtfMarkRecord> marks = new Dictionary<int, OtfMarkRecord>();

            public readonly IDictionary<int, GposAnchor[]> baseMarks = new Dictionary<int, GposAnchor[]>();
        }
    }
}
