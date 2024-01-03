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
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class GposLookupType4Test : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/GposLookupType4Test/";

        [NUnit.Framework.Test]
        public virtual void VerifyMarkToBaseAttachment() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(RESOURCE_FOLDER + "Padauk-Regular.ttf"
                );
            GlyphPositioningTableReader gposTableReader = fontProgram.GetGposTable();
            GposLookupType4 lookup = (GposLookupType4)gposTableReader.GetLookupTable(192);
            IList<Glyph> glyphs = JavaUtil.ArraysAsList(new Glyph(fontProgram.GetGlyphByCode(163)), new Glyph(fontProgram
                .GetGlyphByCode(207)), new Glyph(fontProgram.GetGlyphByCode(213)));
            GlyphLine gl = new GlyphLine(glyphs);
            gl.idx = 2;
            NUnit.Framework.Assert.AreEqual(0, gl.Get(2).GetXPlacement());
            NUnit.Framework.Assert.AreEqual(0, gl.Get(2).GetAnchorDelta());
            lookup.TransformOne(gl);
            NUnit.Framework.Assert.AreEqual(364, gl.Get(2).GetXPlacement());
            NUnit.Framework.Assert.AreEqual(-2, gl.Get(2).GetAnchorDelta());
        }
    }
}
