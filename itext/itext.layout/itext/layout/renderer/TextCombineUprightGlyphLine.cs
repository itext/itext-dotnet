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
using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Font.Otf;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Layout-only view of combined text.</summary>
    /// <remarks>
    /// Layout-only view of combined text. Each run is one atomic, one-em glyph; forced newlines remain separate glyphs.
    /// The source glyphs (including shaping and ActualText) are never modified or replaced for drawing.
    /// </remarks>
    internal sealed class TextCombineUprightGlyphLine : GlyphLine {
        private readonly GlyphLine source;

        private readonly IList<int> sourcePositions = new List<int>();

        private readonly float ascender;

        private readonly float descender;

//\cond DO_NOT_DOCUMENT
        internal TextCombineUprightGlyphLine(GlyphLine source, float ascender, float descender) {
            this.source = source;
            this.ascender = ascender;
            this.descender = descender;
            int pos = source.GetStart();
            while (pos < source.GetEnd()) {
                sourcePositions.Add(pos);
                Add(new TextCombineUprightGlyphLine.PlaceholderGlyph(Math.Max(FontProgram.UNITS_NORMALIZATION, ascender - 
                    descender)));
                while (pos < source.GetEnd() && !iText.IO.Util.TextUtil.IsNewLine(source.Get(pos))) {
                    ++pos;
                }
                if (pos < source.GetEnd()) {
                    bool crlf = iText.IO.Util.TextUtil.IsCarriageReturnFollowedByLineFeed(source, pos);
                    sourcePositions.Add(pos);
                    Add(source.Get(pos++));
                    if (crlf) {
                        sourcePositions.Add(pos);
                        Add(source.Get(pos++));
                    }
                }
            }
            sourcePositions.Add(source.GetEnd());
            SetEnd(Size());
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float GetAscender() {
            return ascender;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal float GetDescender() {
            return descender;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int GetSourcePosition(int position) {
            return sourcePositions[Math.Max(0, position)];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal GlyphLine Restore(GlyphLine laidOutLine) {
            GlyphLine restored = new GlyphLine(source);
            restored.SetStart(GetSourcePosition(laidOutLine.GetStart()));
            restored.SetEnd(GetSourcePosition(laidOutLine.GetEnd()));
            return restored;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class PlaceholderGlyph : Glyph {
            private readonly float layoutWidth;

//\cond DO_NOT_DOCUMENT
            internal PlaceholderGlyph(float layoutWidth)
                : base(
                                // U+FFFC OBJECT REPLACEMENT CHARACTER is only used for layout, never written to the PDF.
                                -1, (int)Math.Ceiling(layoutWidth), 0xFFFC) {
                this.layoutWidth = layoutWidth;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal float GetLayoutWidth() {
                return layoutWidth;
            }
//\endcond
        }
//\endcond
    }
//\endcond
}
