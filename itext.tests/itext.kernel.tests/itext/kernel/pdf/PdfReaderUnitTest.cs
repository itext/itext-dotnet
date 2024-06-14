using System;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfReaderUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfReaderUnitTest/";

        [NUnit.Framework.Test]
        public virtual void ReadStreamBytesRawNullStreamTest() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "testFile.pdf");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => reader.ReadStreamBytesRaw(null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNABLE_TO_READ_STREAM_BYTES, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadObjectStreamNullStreamTest() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "testFile.pdf");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => reader.ReadObjectStream(null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNABLE_TO_READ_OBJECT_STREAM, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadObjectInvalidObjectStreamNumberTest() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "testFile.pdf");
            PdfDocument doc = new PdfDocument(reader);
            PdfIndirectReference @ref = new PdfIndirectReference(doc, 20);
            @ref.SetState(PdfObject.FREE);
            @ref.SetObjStreamNumber(5);
            @ref.refersTo = null;
            PdfIndirectReference ref2 = new PdfIndirectReference(doc, 5);
            ref2.SetState(PdfObject.FREE);
            ref2.refersTo = null;
            doc.GetXref().Add(ref2);
            doc.GetCatalog().GetPdfObject().Put(PdfName.StructTreeRoot, @ref);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => reader.ReadObject(@ref));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.INVALID_OBJECT_STREAM_NUMBER
                , 20, 5, 0), e.Message);
        }
    }
}
