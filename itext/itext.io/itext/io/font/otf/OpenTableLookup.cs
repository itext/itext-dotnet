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
namespace iText.IO.Font.Otf {
    /// <summary>
    /// A Lookup table defines the specific conditions, type, and results of
    /// substitution or positioning actions that are used to implement a feature.
    /// </summary>
    /// <remarks>
    /// A Lookup table defines the specific conditions, type, and results of
    /// substitution or positioning actions that are used to implement a feature.
    /// <para />
    /// The data describing the actions of a lookup are contained in one or more lookup subtables.
    /// Different lookup types support different types of operation; for example, positioning
    /// adjustment on a single glyph versus positioning adjustments on pairs of glyphs.
    /// <para />
    /// For more information see <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
    /// </remarks>
    public abstract class OpenTableLookup {
        /// <summary>Indicates to a text-processing client certain processing options to use when substituting or positioning glyphs.
        ///     </summary>
        protected internal int lookupFlag;

        /// <summary>Subtables locations.</summary>
        protected internal int[] subTableLocations;

        /// <summary>OpenType font table reader.</summary>
        protected internal OpenTypeFontTableReader openReader;

        private int indexInLookupList;

        /// <summary>
        /// Instantiates a new instance of
        /// <see cref="OpenTableLookup"/>.
        /// </summary>
        /// <param name="openReader">the OpenType font table reader</param>
        /// <param name="lookupFlag">
        /// specifies processing options, e.g. whether to skip base glyphs, marks or
        /// ligatures during glyph substitution or positioning. See
        /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </param>
        /// <param name="subTableLocations">the subtables locations</param>
        protected internal OpenTableLookup(OpenTypeFontTableReader openReader, int lookupFlag, int[] subTableLocations
            ) {
            this.lookupFlag = lookupFlag;
            this.subTableLocations = subTableLocations;
            this.openReader = openReader;
        }

        /// <summary>Gets the lookup flag.</summary>
        /// <remarks>
        /// Gets the lookup flag.
        /// <para />
        /// The flag indicates to a text-processing client certain processing
        /// options to use when substituting or positioning glyphs.
        /// e.g. whether to skip base glyphs, marks or ligatures during glyph substitution or positioning.
        /// See <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
        /// </remarks>
        /// <returns>the lookup flag</returns>
        public virtual int GetLookupFlag() {
            return lookupFlag;
        }

        /// <summary>Apply transformation to only one glyph from the glyph line.</summary>
        /// <param name="line">the glyph line to transform</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if transformation was applied,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public abstract bool TransformOne(GlyphLine line);

        /// <summary>Apply transformation to the glyph line.</summary>
        /// <param name="line">the glyph line to transform</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if transformation was applied,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool TransformLine(GlyphLine line) {
            bool changed = false;
            line.SetIdx(line.GetStart());
            while (line.GetIdx() < line.GetEnd() && line.GetIdx() >= line.GetStart()) {
                changed = TransformOne(line) || changed;
            }
            return changed;
        }

        /// <summary>
        /// Checks whether there is a substitution (replacement) for the specified index in
        /// <c>this</c>
        /// lookup table.
        /// </summary>
        /// <param name="index">the index to check for a substitution</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if there is substitution,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool HasSubstitution(int index) {
            return false;
        }

        /// <summary>Reads subtables.</summary>
        protected internal virtual void ReadSubTables() {
            foreach (int subTableLocation in subTableLocations) {
                ReadSubTable(subTableLocation);
            }
        }

        /// <summary>Reads subtable from the specified location.</summary>
        /// <param name="subTableLocation">the subtable location</param>
        protected internal abstract void ReadSubTable(int subTableLocation);

        /// <summary>
        /// Gets
        /// <c>this</c>
        /// lookup table index in the LookupList.
        /// </summary>
        /// <returns>the table index in the LookupList</returns>
        public virtual int GetIndexInLookupList() {
            return indexInLookupList;
        }

        /// <summary>Sets lookup table index in the LookupList.</summary>
        /// <param name="indexInLookupList">the table index in the LookupList</param>
        public virtual void SetIndexInLookupList(int indexInLookupList) {
            this.indexInLookupList = indexInLookupList;
        }

        /// <summary>
        /// Utility class to iterate over
        /// <see cref="GlyphLine"/>.
        /// </summary>
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

            /// <summary>Reads the next glyph taking into account glyph class and lookup flag.</summary>
            /// <param name="openReader">the OpenType reader to check glyph class against lookup flag</param>
            /// <param name="lookupFlag">
            /// specifies processing options, e.g. whether to skip base glyphs, marks or
            /// ligatures during glyph substitution or positioning. See
            /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
            /// </param>
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

            /// <summary>Reads the previous glyph taking into account glyph class and lookup flag.</summary>
            /// <param name="openReader">the OpenType reader to check glyph class against lookup flag</param>
            /// <param name="lookupFlag">
            /// specifies processing options, e.g. whether to skip base glyphs, marks or
            /// ligatures during glyph substitution or positioning. See
            /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/chapter2#lookup-table">Lookup table</a>
            /// </param>
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
