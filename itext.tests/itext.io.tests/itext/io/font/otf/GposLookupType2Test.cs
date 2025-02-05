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
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GposLookupType2Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType2Test/";

        private static readonly String DEJAVU_FONT_PATH = RESOURCE_FOLDER + "DejaVuSans.ttf";

        [NUnit.Framework.Test]
        public virtual void IdxEqualToEndLineGpos2Test() {
            TrueTypeFont font = new TrueTypeFont(DEJAVU_FONT_PATH);
            GlyphPositioningTableReader gposTableReader = font.GetGposTable();
            GposLookupType2 lookup = (GposLookupType2)gposTableReader.GetLookupTable(15);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(font.GetGlyphByCode(174)), new Glyph(font.GetGlyphByCode
                (5)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.SetIdx(2);
            bool transform = lookup.TransformOne(gl);
            NUnit.Framework.Assert.IsFalse(transform);
        }

        [NUnit.Framework.Test]
        public virtual void IdxSmallerThanEndLineGpos2Test() {
            TrueTypeFont font = new TrueTypeFont(DEJAVU_FONT_PATH);
            GlyphPositioningTableReader gposTableReader = font.GetGposTable();
            GposLookupType2 lookup = (GposLookupType2)gposTableReader.GetLookupTable(15);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(font.GetGlyphByCode(174)), new Glyph(font.GetGlyphByCode
                (5)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.SetIdx(0);
            bool transform = lookup.TransformOne(gl);
            NUnit.Framework.Assert.IsFalse(transform);
        }
    }
}
