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
using System;
using System.Collections.Generic;

namespace iText.IO.Font {
//\cond DO_NOT_DOCUMENT
    /// <summary>Subsets a True Type font by removing the unneeded glyphs from the font.</summary>
    internal class TrueTypeFontSubsetter : AbstractTrueTypeFontModifier {
//\cond DO_NOT_DOCUMENT
        internal TrueTypeFontSubsetter(String fontName, OpenTypeParser parser, ICollection<int> glyphs, bool subsetTables
            )
            : base(fontName, subsetTables) {
            horizontalMetricMap = new Dictionary<int, byte[]>(glyphs.Count);
            glyphDataMap = new Dictionary<int, byte[]>(glyphs.Count);
            IList<int> usedGlyphs = parser.GetFlatGlyphs(glyphs);
            foreach (int? glyph in usedGlyphs) {
                byte[] glyphData = parser.GetGlyphDataForGid((int)glyph);
                glyphDataMap.Put((int)glyph, glyphData);
                byte[] glyphMetric = parser.GetHorizontalMetricForGid((int)glyph);
                horizontalMetricMap.Put((int)glyph, glyphMetric);
            }
            this.raf = parser.raf.CreateView();
            this.directoryOffset = parser.directoryOffset;
            this.numberOfHMetrics = parser.hhea.numberOfHMetrics;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override int MergeTables() {
            return base.CreateModifiedTables();
        }
//\endcond
        // cmap table subsetting isn't supported yet
    }
//\endcond
}
