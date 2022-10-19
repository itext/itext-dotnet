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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfStreamUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotCreatePdfStreamWithoutDocumentTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfStream(null, null, 1
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CREATE_PDFSTREAM_BY_INPUT_STREAM_WITHOUT_PDF_DOCUMENT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SetDataToPdfStreamWithInputStreamTest() {
            Stream inputStream = new MemoryStream(new byte[] {  });
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfStream pdfStream = new PdfStream(pdfDocument, inputStream, 1);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfStream.SetData(new byte[
                ] {  }, true));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_SET_DATA_TO_PDF_STREAM_WHICH_WAS_CREATED_BY_INPUT_STREAM
                , exception.Message);
        }
    }
}
