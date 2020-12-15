using iText.Kernel;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    public class PdfCieBasedCsUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectEmptyTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] {  });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooLittlePointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] { 1, 2 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalGrayIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalGray(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfCalRgbIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.CalRgb(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }

        [NUnit.Framework.Test]
        public virtual void WhitePointOfLabIsIncorrectTooMuchPointsTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCieBasedCs basedCs = new PdfCieBasedCs.Lab(new float[] { 1, 2, 3, 4 });
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.WHITE_POINT_IS_INCORRECTLY_SPECIFIED))
;
        }
    }
}
