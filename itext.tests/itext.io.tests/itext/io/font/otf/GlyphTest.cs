/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("UnitTest")]
    public class GlyphTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void HasPlacementIfAnchorDeltaNonZeroTest() {
            Glyph glyph = CreateDummyGlyph();
            NUnit.Framework.Assert.AreEqual(0, glyph.GetXPlacement());
            NUnit.Framework.Assert.AreEqual(0, glyph.GetYPlacement());
            NUnit.Framework.Assert.AreEqual(0, glyph.GetAnchorDelta());
            NUnit.Framework.Assert.IsFalse(glyph.HasPlacement());
            glyph.SetAnchorDelta((short)10);
            NUnit.Framework.Assert.IsTrue(glyph.HasPlacement());
        }

        [NUnit.Framework.Test]
        public virtual void HasOffsetsIfAnchorDeltaNonZeroTest() {
            Glyph glyph = CreateDummyGlyph();
            NUnit.Framework.Assert.AreEqual(0, glyph.GetXPlacement());
            NUnit.Framework.Assert.AreEqual(0, glyph.GetYPlacement());
            NUnit.Framework.Assert.AreEqual(0, glyph.GetAnchorDelta());
            NUnit.Framework.Assert.IsFalse(glyph.HasOffsets());
            glyph.SetAnchorDelta((short)10);
            NUnit.Framework.Assert.IsTrue(glyph.HasOffsets());
        }

        private static Glyph CreateDummyGlyph() {
            return new Glyph(0, 0, 0);
        }
    }
}
