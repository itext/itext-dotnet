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
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Test;

namespace iText.Layout.Renderer {
    public class TypographyUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void VerifyPdfCalligraphIsNotAvailable() {
            NUnit.Framework.Assert.IsFalse(TypographyUtils.IsPdfCalligraphAvailable());
        }

        [NUnit.Framework.Test]
        public virtual void UpdateAnchorDeltaMarkNotReorderedTest() {
            // original line 'abm', and 'm' is a mark based on 'b'
            Glyph mGlyph = new Glyph(100, 'm');
            mGlyph.SetAnchorDelta((short)-1);
            mGlyph.SetXAdvance((short)15);
            mGlyph.SetYAdvance((short)25);
            LineRenderer.RendererGlyph b = new LineRenderer.RendererGlyph(new Glyph(100, 'b'), null);
            LineRenderer.RendererGlyph m = new LineRenderer.RendererGlyph(mGlyph, null);
            LineRenderer.RendererGlyph a = new LineRenderer.RendererGlyph(new Glyph(100, 'a'), null);
            IList<LineRenderer.RendererGlyph> reorderedLine = JavaUtil.ArraysAsList(b, m, a);
            int[] reorder = new int[] { 1, 2, 0 };
            int[] inverseReorder = new int[] { 2, 0, 1 };
            TypographyUtils.UpdateAnchorDeltaForReorderedLineGlyphs(reorder, inverseReorder, reorderedLine);
            NUnit.Framework.Assert.AreSame(mGlyph, m.glyph);
            NUnit.Framework.Assert.AreEqual(-1, m.glyph.GetAnchorDelta());
        }

        [NUnit.Framework.Test]
        public virtual void UpdateAnchorDeltaMarkReorderedTest() {
            // original line 'abm', and 'm' is a mark based on 'b'
            Glyph mGlyph = new Glyph(100, 'm');
            mGlyph.SetAnchorDelta((short)-1);
            mGlyph.SetXAdvance((short)15);
            mGlyph.SetYAdvance((short)25);
            LineRenderer.RendererGlyph m = new LineRenderer.RendererGlyph(mGlyph, null);
            LineRenderer.RendererGlyph b = new LineRenderer.RendererGlyph(new Glyph(100, 'b'), null);
            LineRenderer.RendererGlyph a = new LineRenderer.RendererGlyph(new Glyph(100, 'a'), null);
            IList<LineRenderer.RendererGlyph> reorderedLine = JavaUtil.ArraysAsList(m, b, a);
            int[] reorder = new int[] { 2, 1, 0 };
            int[] inverseReorder = new int[] { 2, 1, 0 };
            TypographyUtils.UpdateAnchorDeltaForReorderedLineGlyphs(reorder, inverseReorder, reorderedLine);
            NUnit.Framework.Assert.AreNotSame(mGlyph, m.glyph);
            NUnit.Framework.Assert.AreEqual(1, m.glyph.GetAnchorDelta());
        }
    }
}
