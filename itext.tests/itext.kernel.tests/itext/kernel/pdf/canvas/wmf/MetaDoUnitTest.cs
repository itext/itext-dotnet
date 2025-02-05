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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Wmf {
    [NUnit.Framework.Category("UnitTest")]
    public class MetaDoUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InputStreamShallBeStartFromSpecificValueTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage pdfPage = pdfDocument.AddNewPage(1);
            PdfCanvas pdfCanvas = new PdfCanvas(pdfPage);
            // InputStream value shall be start with 0x9AC6CDD7
            Stream inputStream = new MemoryStream(new byte[] {  });
            MetaDo metaDo = new MetaDo(inputStream, pdfCanvas);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => metaDo.ReadAll());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NOT_A_PLACEABLE_WINDOWS_METAFILE, exception
                .Message);
        }
    }
}
