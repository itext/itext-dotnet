using System.Collections.Generic;

namespace iText.IO.Font.Otf {
    /// <summary>
    /// Lookup Type 6:
    /// MarkToMark Attachment Positioning Subtable
    /// </summary>
    public class GposLookupType6 : OpenTableLookup {
        private readonly IList<GposLookupType6.MarkToBase> marksbases;

        /// <exception cref="System.IO.IOException"/>
        public GposLookupType6(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            marksbases = new List<GposLookupType6.MarkToBase>();
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
            bool changed = false;
            OpenTableLookup.GlyphIndexer gi = null;
            foreach (GposLookupType6.MarkToBase mb in marksbases) {
                OtfMarkRecord omr = mb.marks.Get(line.Get(line.idx).GetCode());
                if (omr == null) {
                    continue;
                }
                if (gi == null) {
                    gi = new OpenTableLookup.GlyphIndexer();
                    gi.idx = line.idx;
                    gi.line = line;
                    while (true) {
                        gi.PreviousGlyph(openReader, lookupFlag);
                        if (gi.glyph == null) {
                            break;
                        }
                        if (mb.marks.ContainsKey(gi.glyph.GetCode())) {
                            break;
                        }
                    }
                    if (gi.glyph == null) {
                        break;
                    }
                }
                GposAnchor[] gpas = mb.bases.Get(gi.glyph.GetCode());
                if (gpas == null) {
                    continue;
                }
                int markClass = omr.markClass;
                GposAnchor baseAnchor = gpas[markClass];
                GposAnchor markAnchor = omr.anchor;
                line.Set(line.idx, new Glyph(line.Get(line.idx), -markAnchor.XCoordinate + baseAnchor.XCoordinate, -markAnchor
                    .YCoordinate + baseAnchor.YCoordinate, 0, 0, gi.idx - line.idx));
                changed = true;
                break;
            }
            line.idx++;
            return changed;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            openReader.rf.ReadUnsignedShort();
            //skip format, always 1
            int markCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int baseCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int classCount = openReader.rf.ReadUnsignedShort();
            int markArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int baseArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            IList<int> markCoverage = openReader.ReadCoverageFormat(markCoverageLocation);
            IList<int> baseCoverage = openReader.ReadCoverageFormat(baseCoverageLocation);
            IList<OtfMarkRecord> markRecords = OtfReadCommon.ReadMarkArray(openReader, markArrayLocation);
            GposLookupType6.MarkToBase markToBase = new GposLookupType6.MarkToBase();
            for (int k = 0; k < markCoverage.Count; ++k) {
                markToBase.marks[markCoverage[k]] = markRecords[k];
            }
            IList<GposAnchor[]> baseArray = OtfReadCommon.ReadBaseArray(openReader, classCount, baseArrayLocation);
            for (int k_1 = 0; k_1 < baseCoverage.Count; ++k_1) {
                markToBase.bases[baseCoverage[k_1]] = baseArray[k_1];
            }
            marksbases.Add(markToBase);
        }

        private class MarkToBase {
            public readonly IDictionary<int, OtfMarkRecord> marks = new Dictionary<int, OtfMarkRecord>();

            public readonly IDictionary<int, GposAnchor[]> bases = new Dictionary<int, GposAnchor[]>();
        }
    }
}
