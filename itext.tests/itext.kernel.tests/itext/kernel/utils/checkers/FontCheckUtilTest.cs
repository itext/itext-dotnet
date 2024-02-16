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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Kernel.Utils.Checkers {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCheckUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckFontAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.IsTrue(FontCheckUtil.DoesFontContainAllUsedGlyphs("123", font));
        }

        [NUnit.Framework.Test]
        public virtual void CheckFontNotAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            NUnit.Framework.Assert.IsFalse(FontCheckUtil.DoesFontContainAllUsedGlyphs("⫊", font));
        }
    }
}
