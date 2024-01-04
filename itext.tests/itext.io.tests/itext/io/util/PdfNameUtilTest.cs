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
using iText.Test;

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfNameUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DecodeNameTest() {
            // /#C3#9Cberschrift_1
            byte[] name1Content = new byte[] { 35, 67, 51, 35, 57, 67, 98, 101, 114, 115, 99, 104, 114, 105, 102, 116, 
                95, 49 };
            NUnit.Framework.Assert.AreEqual("Ã\u009Cberschrift_1", PdfNameUtil.DecodeName(name1Content));
            // /TOC-1
            byte[] name2Content = new byte[] { 84, 79, 67, 45, 49 };
            NUnit.Framework.Assert.AreEqual("TOC-1", PdfNameUtil.DecodeName(name2Content));
            // /NormalParagraphStyle
            byte[] name3Content = new byte[] { 78, 111, 114, 109, 97, 108, 80, 97, 114, 97, 103, 114, 97, 112, 104, 83
                , 116, 121, 108, 101 };
            NUnit.Framework.Assert.AreEqual("NormalParagraphStyle", PdfNameUtil.DecodeName(name3Content));
        }
    }
}
