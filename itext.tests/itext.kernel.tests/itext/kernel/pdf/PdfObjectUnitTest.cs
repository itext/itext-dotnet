using System;
using iText.IO.Source;
using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfObjectUnitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfObjectUnitTest/";

        [NUnit.Framework.Test]
        public virtual void NoWriterForMakingIndirectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "noWriterForMakingIndirect.pdf"));
                PdfDictionary pdfDictionary = new PdfDictionary();
                pdfDictionary.MakeIndirect(pdfDocument);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.THERE_IS_NO_ASSOCIATE_PDF_WRITER_FOR_MAKING_INDIRECTS))
;
        }

        [NUnit.Framework.Test]
        public virtual void CopyDocInReadingModeTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "copyDocInReadingMode.pdf"));
                PdfDictionary pdfDictionary = new PdfDictionary();
                pdfDictionary.ProcessCopying(pdfDocument, true);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_COPY_TO_DOCUMENT_OPENED_IN_READING_MODE))
;
        }

        [NUnit.Framework.Test]
        public virtual void CopyIndirectObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfObject pdfObject = pdfDocument.GetPdfObject(1);
                pdfObject.CopyTo(pdfDocument, true);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_COPY_INDIRECT_OBJECT_FROM_THE_DOCUMENT_THAT_IS_BEING_WRITTEN))
;
        }

        [NUnit.Framework.Test]
        public virtual void CopyFlushedObjectTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
                PdfObject pdfObject = pdfDocument.GetPdfObject(1);
                pdfObject.Flush();
                pdfObject.CopyContent(pdfObject, pdfDocument);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_COPY_FLUSHED_OBJECT))
;
        }
    }
}
