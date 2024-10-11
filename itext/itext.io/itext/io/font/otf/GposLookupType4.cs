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
    /// Lookup Type 4:
    /// MarkToBase Attachment Positioning Subtable
    /// </summary>
    public class GposLookupType4 : OpenTableLookup {
        private readonly IList<GposLookupType4.MarkToBase> marksbases;

        public GposLookupType4(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            marksbases = new List<GposLookupType4.MarkToBase>();
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
            foreach (GposLookupType4.MarkToBase mb in marksbases) {
                OtfMarkRecord omr = mb.marks.Get(line.Get(line.GetIdx()).GetCode());
                if (omr == null) {
                    continue;
                }
                if (gi == null) {
                    gi = new OpenTableLookup.GlyphIndexer();
                    gi.SetIdx(line.GetIdx());
                    gi.SetLine(line);
                    while (true) {
                        gi.PreviousGlyph(openReader, lookupFlag);
                        if (gi.GetGlyph() == null) {
                            break;
                        }
                        // not mark => base glyph
                        if (openReader.GetGlyphClass(gi.GetGlyph().GetCode()) != OtfClass.GLYPH_MARK) {
                            break;
                        }
                    }
                    if (gi.GetGlyph() == null) {
                        break;
                    }
                }
                GposAnchor[] gpas = mb.bases.Get(gi.GetGlyph().GetCode());
                if (gpas == null) {
                    continue;
                }
                int markClass = omr.GetMarkClass();
                int xPlacement = 0;
                int yPlacement = 0;
                GposAnchor baseAnchor = gpas[markClass];
                if (baseAnchor != null) {
                    xPlacement = baseAnchor.GetXCoordinate();
                    yPlacement = baseAnchor.GetYCoordinate();
                }
                GposAnchor markAnchor = omr.GetAnchor();
                if (markAnchor != null) {
                    xPlacement -= markAnchor.GetXCoordinate();
                    yPlacement -= markAnchor.GetYCoordinate();
                }
                line.Set(line.GetIdx(), new Glyph(line.Get(line.GetIdx()), xPlacement, yPlacement, 0, 0, gi.GetIdx() - line
                    .GetIdx()));
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
            GposLookupType4.MarkToBase markToBase = new GposLookupType4.MarkToBase();
            for (int k = 0; k < markCoverage.Count; ++k) {
                markToBase.marks.Put(markCoverage[k], markRecords[k]);
            }
            IList<GposAnchor[]> baseArray = OtfReadCommon.ReadBaseArray(openReader, classCount, baseArrayLocation);
            for (int k = 0; k < baseCoverage.Count; ++k) {
                markToBase.bases.Put(baseCoverage[k], baseArray[k]);
            }
            marksbases.Add(markToBase);
        }

        public class MarkToBase {
            public readonly IDictionary<int, OtfMarkRecord> marks = new Dictionary<int, OtfMarkRecord>();

            public readonly IDictionary<int, GposAnchor[]> bases = new Dictionary<int, GposAnchor[]>();
        }
    }
}
