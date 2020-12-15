using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfArrayUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToArrayOfBooleansTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToBooleanArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_AN_ARRAY_OF_BOOLEANS))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToDoubleArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToDoubleArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_DOUBLE_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToIntArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToIntArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_INT_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToFloatArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToFloatArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_FLOAT_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToLongArrayTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToLongArray();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_LONG_ARRAY))
;
        }

        [NUnit.Framework.Test]
        public virtual void CannotConvertArrayOfPdfStringsToRectangleTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfArray pdfArray = new PdfArray(new PdfString(""));
                pdfArray.ToRectangle();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.CANNOT_CONVERT_PDF_ARRAY_TO_RECTANGLE))
;
        }
    }
}
