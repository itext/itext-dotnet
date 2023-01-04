/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class Type3FontTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddGlyphTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
        }

        [NUnit.Framework.Test]
        public virtual void AddGlyphsWithDifferentUnicodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            font.AddGlyph(2, 2, 600, null, null);
            NUnit.Framework.Assert.AreEqual(2, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(1, font.GetGlyphByCode(1).GetUnicode());
            NUnit.Framework.Assert.AreEqual(2, font.GetGlyphByCode(2).GetUnicode());
        }

        [NUnit.Framework.Test]
        public virtual void AddGlyphsWithDifferentCodesTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(2, -1, 700, null, null);
            NUnit.Framework.Assert.AreEqual(2, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(600, font.GetGlyphByCode(1).GetWidth());
            NUnit.Framework.Assert.AreEqual(700, font.GetGlyphByCode(2).GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphsWithSameUnicodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 1, 600, null, null);
            font.AddGlyph(2, 1, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(2, font.GetGlyph(1).GetCode());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphWithSameCodeTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(1, -1, 700, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(700, font.GetGlyphByCode(1).GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void NotAddGlyphWithSameCodeEmptyUnicodeFirstTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, -1, 600, null, null);
            font.AddGlyph(1, 100, 600, null, null);
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(1, font.GetGlyph(100).GetCode());
            NUnit.Framework.Assert.AreEqual(100, font.GetGlyphByCode(1).GetUnicode());
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceGlyphWithSameCodeEmptyUnicodeLastTest() {
            Type3Font font = new Type3Font(false);
            font.AddGlyph(1, 100, 600, null, null);
            font.AddGlyph(1, -1, 600, null, null);
            NUnit.Framework.Assert.IsNull(font.GetGlyph(-1));
            NUnit.Framework.Assert.IsNull(font.GetGlyph(100));
            NUnit.Framework.Assert.AreEqual(1, font.GetNumberOfGlyphs());
            NUnit.Framework.Assert.AreEqual(-1, font.GetGlyphByCode(1).GetUnicode());
        }
    }
}
