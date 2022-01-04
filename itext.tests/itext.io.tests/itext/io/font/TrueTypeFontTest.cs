/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    public class TrueTypeFontTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/TrueTypeFontTest/";

        [NUnit.Framework.Test]
        public virtual void NotoSansJpCmapTest() {
            // 信
            char jpChar = '\u4FE1';
            FontProgram fontProgram = FontProgramFactory.CreateFont(sourceFolder + "NotoSansJP-Regular.otf");
            Glyph glyph = fontProgram.GetGlyph(jpChar);
            NUnit.Framework.Assert.AreEqual(new char[] { jpChar }, glyph.GetUnicodeChars());
            NUnit.Framework.Assert.AreEqual(20449, glyph.GetUnicode());
            // TODO DEVSIX-5767 actual expected value is 0x27d3
            NUnit.Framework.Assert.AreEqual(0x0a72, glyph.GetCode());
        }
    }
}
