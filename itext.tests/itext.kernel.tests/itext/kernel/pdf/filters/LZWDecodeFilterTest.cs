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
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Filters {
    [NUnit.Framework.Category("UnitTest")]
    public class LZWDecodeFilterTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DecodingTestStatic() {
            byte[] bytes = new byte[] { (byte)0x80, 0x0B, 0x60, 0x50, 0x22, 0x0C, 0x0C, (byte)0x85, 0x01 };
            String expectedResult = "-----A---B";
            String decoded = iText.Commons.Utils.JavaUtil.GetStringForBytes(LZWDecodeFilter.LZWDecode(bytes));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }

        [NUnit.Framework.Test]
        public virtual void DecodingTestNonStatic() {
            byte[] bytes = new byte[] { (byte)0x80, 0x0B, 0x60, 0x50, 0x22, 0x0C, 0x0C, (byte)0x85, 0x01 };
            String expectedResult = "-----A---B";
            LZWDecodeFilter filter = new LZWDecodeFilter();
            String decoded = iText.Commons.Utils.JavaUtil.GetStringForBytes(filter.Decode(bytes, null, new PdfDictionary
                (), new PdfDictionary()));
            NUnit.Framework.Assert.AreEqual(expectedResult, decoded);
        }
    }
}
