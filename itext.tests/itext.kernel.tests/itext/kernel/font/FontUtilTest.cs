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
using iText.IO.Font.Cmap;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseUniversalNotExistedCMapTest() {
            NUnit.Framework.Assert.IsNull(FontUtil.ParseUniversalToUnicodeCMap("NotExisted"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP, LogLevel = LogLevelConstants
            .ERROR)]
        public virtual void ProcessInvalidToUnicodeTest() {
            PdfStream toUnicode = new PdfStream();
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                toUnicode.MakeIndirect(pdfDocument);
                toUnicode.Flush();
                CMapToUnicode cmap = FontUtil.ProcessToUnicode(toUnicode);
                NUnit.Framework.Assert.IsNotNull(cmap);
                NUnit.Framework.Assert.IsFalse(cmap.HasByteMappings());
            }
        }
    }
}
