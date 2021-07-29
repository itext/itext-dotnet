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
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfStructTreeRootUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotMovePageInPartlyFlushedDocTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfPage pdfPage = pdfDocument.AddNewPage(1);
                pdfPage.Flush();
                PdfStructTreeRoot pdfStructTreeRoot = new PdfStructTreeRoot(pdfDocument);
                pdfStructTreeRoot.Move(pdfPage, -1);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_MOVE_PAGES_IN_PARTLY_FLUSHED_DOCUMENT, 1)))
;
        }
    }
}
