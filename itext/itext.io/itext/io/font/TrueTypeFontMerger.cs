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
using iText.Commons.Utils;
using iText.IO.Exceptions;

namespace iText.IO.Font {
//\cond DO_NOT_DOCUMENT
    /// <summary>Merges TrueType fonts and subset merged font by leaving only needed glyphs in the font.</summary>
    internal class TrueTypeFontMerger : AbstractTrueTypeFontModifier {
//\cond DO_NOT_DOCUMENT
        internal TrueTypeFontMerger(String fontName, IDictionary<OpenTypeParser, ICollection<int>> fontsToMerge)
            : base(fontName, true) {
            horizontalMetricMap = new Dictionary<int, byte[]>();
            glyphDataMap = new Dictionary<int, byte[]>();
            OpenTypeParser parserExample = null;
            foreach (KeyValuePair<OpenTypeParser, ICollection<int>> entry in fontsToMerge) {
                OpenTypeParser parser = entry.Key;
                IList<int> usedGlyphs = parser.GetFlatGlyphs(entry.Value);
                foreach (int? glyphObj in usedGlyphs) {
                    int glyph = (int)glyphObj;
                    byte[] glyphData = parser.GetGlyphDataForGid((int)glyph);
                    if (glyphDataMap.ContainsKey((int)glyph) && !JavaUtil.ArraysEquals(glyphDataMap.Get((int)glyph), glyphData
                        )) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INCOMPATIBLE_GLYPH_DATA_DURING_FONT_MERGING
                            ).SetMessageParams(fontName);
                    }
                    glyphDataMap.Put(glyph, glyphData);
                    byte[] glyphMetric = parser.GetHorizontalMetricForGid(glyph);
                    if (horizontalMetricMap.ContainsKey(glyph) && !JavaUtil.ArraysEquals(horizontalMetricMap.Get(glyph), glyphMetric
                        )) {
                        throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.INCOMPATIBLE_GLYPH_DATA_DURING_FONT_MERGING
                            ).SetMessageParams(fontName);
                    }
                    horizontalMetricMap.Put(glyph, glyphMetric);
                }
                // hmtx table size is defined by numberOfHMetrics field from hhea table. To avoid hmtx table resizing and
                // updating numberOfHMetrics, as a base font, on which merged font will be built, we choose the font with
                // the biggest numberOfHMetrics value. Biggest numberOfHMetrics guarantee that hmtx table will contain
                // gids from all the fonts.
                if (parserExample == null || parser.hhea.numberOfHMetrics > parserExample.hhea.numberOfHMetrics) {
                    parserExample = parser;
                }
            }
            this.raf = parserExample.raf.CreateView();
            this.directoryOffset = parserExample.directoryOffset;
            this.numberOfHMetrics = parserExample.hhea.numberOfHMetrics;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override int MergeTables() {
            return base.CreateModifiedTables();
        }
//\endcond
        // cmap table merging isn't supported yet
    }
//\endcond
}
