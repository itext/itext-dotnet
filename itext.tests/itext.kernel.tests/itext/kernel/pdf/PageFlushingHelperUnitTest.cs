/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PageFlushingHelperUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FlushingInReadingModeTest01() {
            NUnit.Framework.Assert.That(() =>  {
                int pageToFlush = 1;
                MemoryStream outputStream = new MemoryStream();
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputStream));
                pdfDocument.AddNewPage();
                pdfDocument.Close();
                pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())));
                PageFlushingHelper pageFlushingHelper = new PageFlushingHelper(pdfDocument);
                pageFlushingHelper.UnsafeFlushDeep(pageToFlush);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(KernelExceptionMessageConstant.FLUSHING_HELPER_FLUSHING_MODE_IS_NOT_FOR_DOC_READING_MODE))
;
        }

        [NUnit.Framework.Test]
        public virtual void FlushingInReadingModeTest02() {
            NUnit.Framework.Assert.That(() =>  {
                int pageToFlush = 1;
                MemoryStream outputStream = new MemoryStream();
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputStream));
                pdfDocument.AddNewPage();
                pdfDocument.Close();
                pdfDocument = new PdfDocument(new PdfReader(new MemoryStream(outputStream.ToArray())));
                PageFlushingHelper pageFlushingHelper = new PageFlushingHelper(pdfDocument);
                pageFlushingHelper.AppendModeFlush(pageToFlush);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(KernelExceptionMessageConstant.FLUSHING_HELPER_FLUSHING_MODE_IS_NOT_FOR_DOC_READING_MODE))
;
        }
    }
}
