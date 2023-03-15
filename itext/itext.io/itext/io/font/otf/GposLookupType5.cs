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
using System.Collections.Generic;

namespace iText.IO.Font.Otf {
    /// <summary>
    /// Lookup Type 5:
    /// MarkToLigature Attachment Positioning Subtable
    /// </summary>
    public class GposLookupType5 : OpenTableLookup {
        private readonly IList<GposLookupType5.MarkToLigature> marksligatures;

        public GposLookupType5(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations)
            : base(openReader, lookupFlag, subTableLocations) {
            marksligatures = new List<GposLookupType5.MarkToLigature>();
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
            OpenTableLookup.GlyphIndexer ligatureGlyphIndexer = null;
            foreach (GposLookupType5.MarkToLigature mb in marksligatures) {
                OtfMarkRecord omr = mb.marks.Get(line.Get(line.idx).GetCode());
                if (omr == null) {
                    continue;
                }
                if (ligatureGlyphIndexer == null) {
                    ligatureGlyphIndexer = new OpenTableLookup.GlyphIndexer();
                    ligatureGlyphIndexer.idx = line.idx;
                    ligatureGlyphIndexer.line = line;
                    while (true) {
                        ligatureGlyphIndexer.PreviousGlyph(openReader, lookupFlag);
                        if (ligatureGlyphIndexer.glyph == null) {
                            break;
                        }
                        // not mark => ligature glyph
                        if (!mb.marks.ContainsKey(ligatureGlyphIndexer.glyph.GetCode())) {
                            break;
                        }
                    }
                    if (ligatureGlyphIndexer.glyph == null) {
                        break;
                    }
                }
                IList<GposAnchor[]> componentAnchors = mb.ligatures.Get(ligatureGlyphIndexer.glyph.GetCode());
                if (componentAnchors == null) {
                    continue;
                }
                int markClass = omr.markClass;
                // TODO DEVSIX-3732 For complex cases like (glyph1, glyph2, mark, glyph3) and
                //  (glyph1, mark, glyph2, glyph3) when the base glyphs compose a ligature and the mark
                //  is attached to the ligature afterwards, mark should be placed in the corresponding anchor
                //  of that ligature (by finding the right component's anchor).
                //  Excerpt from Microsoft Docs: "For a given mark assigned to a particular class, the appropriate
                //  base attachment point is determined by which ligature component the mark is associated with.
                //  This is dependent on the original character string and subsequent character- or glyph-sequence
                //  processing, not the font data alone. While a text-layout client is performing any character-based
                //  preprocessing or any glyph-substitution operations using the GSUB table, the text-layout client
                //  must keep track of the associations of marks to particular ligature-glyph components."
                //  For now we do not store all the substitution info and therefore not able to follow that logic.
                //  We place the mark symbol in the last available place for now (seems to be better default than
                //  first available place).
                for (int component = componentAnchors.Count - 1; component >= 0; component--) {
                    if (componentAnchors[component][markClass] != null) {
                        GposAnchor baseAnchor = componentAnchors[component][markClass];
                        GposAnchor markAnchor = omr.anchor;
                        line.Set(line.idx, new Glyph(line.Get(line.idx), baseAnchor.XCoordinate - markAnchor.XCoordinate, baseAnchor
                            .YCoordinate - markAnchor.YCoordinate, 0, 0, ligatureGlyphIndexer.idx - line.idx));
                        changed = true;
                        break;
                    }
                }
                break;
            }
            line.idx++;
            return changed;
        }

        protected internal override void ReadSubTable(int subTableLocation) {
            openReader.rf.Seek(subTableLocation);
            // skip format, always 1
            openReader.rf.ReadUnsignedShort();
            int markCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int ligatureCoverageLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int classCount = openReader.rf.ReadUnsignedShort();
            int markArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            int ligatureArrayLocation = openReader.rf.ReadUnsignedShort() + subTableLocation;
            IList<int> markCoverage = openReader.ReadCoverageFormat(markCoverageLocation);
            IList<int> ligatureCoverage = openReader.ReadCoverageFormat(ligatureCoverageLocation);
            IList<OtfMarkRecord> markRecords = OtfReadCommon.ReadMarkArray(openReader, markArrayLocation);
            GposLookupType5.MarkToLigature markToLigature = new GposLookupType5.MarkToLigature();
            for (int k = 0; k < markCoverage.Count; ++k) {
                markToLigature.marks.Put(markCoverage[k], markRecords[k]);
            }
            IList<IList<GposAnchor[]>> ligatureArray = OtfReadCommon.ReadLigatureArray(openReader, classCount, ligatureArrayLocation
                );
            for (int k = 0; k < ligatureCoverage.Count; ++k) {
                markToLigature.ligatures.Put(ligatureCoverage[k], ligatureArray[k]);
            }
            marksligatures.Add(markToLigature);
        }

        public class MarkToLigature {
            public readonly IDictionary<int, OtfMarkRecord> marks = new Dictionary<int, OtfMarkRecord>();

            // Glyph id to list of components, each component has a separate list of attachment points
            // defined for different mark classes
            public readonly IDictionary<int, IList<GposAnchor[]>> ligatures = new Dictionary<int, IList<GposAnchor[]>>
                ();
        }
    }
}
