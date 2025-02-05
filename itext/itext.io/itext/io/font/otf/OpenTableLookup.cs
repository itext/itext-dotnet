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
namespace iText.IO.Font.Otf {
    public abstract class OpenTableLookup {
        protected internal int lookupFlag;

        protected internal int[] subTableLocations;

        protected internal OpenTypeFontTableReader openReader;

        protected internal OpenTableLookup(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations
            ) {
            this.lookupFlag = lookupFlag;
            this.subTableLocations = subTableLocations;
            this.openReader = openReader;
        }

        public virtual int GetLookupFlag() {
            return lookupFlag;
        }

        public abstract bool TransformOne(GlyphLine line);

        public virtual bool TransformLine(GlyphLine line) {
            bool changed = false;
            line.SetIdx(line.GetStart());
            while (line.GetIdx() < line.GetEnd() && line.GetIdx() >= line.GetStart()) {
                changed = TransformOne(line) || changed;
            }
            return changed;
        }

        public virtual bool HasSubstitution(int index) {
            return false;
        }

        protected internal virtual void ReadSubTables() {
            foreach (int subTableLocation in subTableLocations) {
                ReadSubTable(subTableLocation);
            }
        }

        protected internal abstract void ReadSubTable(int subTableLocation);

        public class GlyphIndexer {
            private GlyphLine line;

            private Glyph glyph;

            private int idx;

            /// <summary>Retrieves the glyph line of the object.</summary>
            /// <returns>glyph line</returns>
            public virtual GlyphLine GetLine() {
                return line;
            }

            /// <summary>Sets the glyph line of the object.</summary>
            /// <param name="line">glyph line</param>
            public virtual void SetLine(GlyphLine line) {
                this.line = line;
            }

            /// <summary>Retrieves the glyph of the object.</summary>
            /// <returns>glyph</returns>
            public virtual Glyph GetGlyph() {
                return glyph;
            }

            /// <summary>Sets the glyph of the object.</summary>
            /// <param name="glyph">glyph</param>
            public virtual void SetGlyph(Glyph glyph) {
                this.glyph = glyph;
            }

            /// <summary>Retrieves the idx of the glyph indexer.</summary>
            /// <returns>idx</returns>
            public virtual int GetIdx() {
                return idx;
            }

            /// <summary>Sets the idx of the glyph indexer.</summary>
            /// <param name="idx">idx</param>
            public virtual void SetIdx(int idx) {
                this.idx = idx;
            }

            public virtual void NextGlyph(OpenTypeFontTableReader openReader, int lookupFlag) {
                glyph = null;
                while (++idx < line.GetEnd()) {
                    Glyph g = line.Get(idx);
                    if (!openReader.IsSkip(g.GetCode(), lookupFlag)) {
                        glyph = g;
                        break;
                    }
                }
            }

            public virtual void PreviousGlyph(OpenTypeFontTableReader openReader, int lookupFlag) {
                glyph = null;
                while (--idx >= line.GetStart()) {
                    Glyph g = line.Get(idx);
                    if (!openReader.IsSkip(g.GetCode(), lookupFlag)) {
                        glyph = g;
                        break;
                    }
                }
            }
        }
    }
}
