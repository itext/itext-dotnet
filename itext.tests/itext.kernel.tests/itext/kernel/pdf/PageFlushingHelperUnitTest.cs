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
