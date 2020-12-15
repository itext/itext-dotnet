using System;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfXObjectUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoSubTypeProvidedTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfStream pdfStream = new PdfStream();
                PdfXObject pdfXObject = PdfXObject.MakeXObject(pdfStream);
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNSUPPORTED_XOBJECT_TYPE))
;
        }

        [NUnit.Framework.Test]
        public virtual void UnsupportedSubTypeIsSet() {
            NUnit.Framework.Assert.That(() =>  {
                PdfStream pdfStream = new PdfStream();
                pdfStream.Put(PdfName.Subtype, new PdfName("Unsupported SubType"));
                PdfXObject pdfXObject = PdfXObject.MakeXObject(pdfStream);
            }
            , NUnit.Framework.Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(KernelExceptionMessageConstant.UNSUPPORTED_XOBJECT_TYPE))
;
        }
    }
}
