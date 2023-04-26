/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfObjectUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfObjectUnitTest/";

        [NUnit.Framework.Test]
        public virtual void NoWriterForMakingIndirectTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "noWriterForMakingIndirect.pdf"));
            PdfDictionary pdfDictionary = new PdfDictionary();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDictionary.MakeIndirect(
                pdfDocument));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.THERE_IS_NO_ASSOCIATE_PDF_WRITER_FOR_MAKING_INDIRECTS
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CopyDocInReadingModeTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "copyDocInReadingMode.pdf"));
            PdfDictionary pdfDictionary = new PdfDictionary();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDictionary.ProcessCopying
                (pdfDocument, true));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_COPY_TO_DOCUMENT_OPENED_IN_READING_MODE
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CopyIndirectObjectTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfObject pdfObject = pdfDocument.GetPdfObject(1);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfObject.CopyTo(pdfDocument
                , true));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_COPY_INDIRECT_OBJECT_FROM_THE_DOCUMENT_THAT_IS_BEING_WRITTEN
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CopyFlushedObjectTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfObject pdfObject = pdfDocument.GetPdfObject(1);
            pdfObject.Flush();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfObject.CopyContent(pdfObject
                , pdfDocument));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_COPY_FLUSHED_OBJECT, exception.Message
                );
        }
    }
}
