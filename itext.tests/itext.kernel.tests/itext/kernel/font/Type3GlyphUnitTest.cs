using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    public class Type3GlyphUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/Type3GlyphUnitTest/";

        [NUnit.Framework.Test]
        public virtual void AddImageWithoutMaskTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => glyph.AddImageWithTransformationMatrix
                (img, 100, 0, 0, 100, 0, 0, false));
            NUnit.Framework.Assert.AreEqual("Not colorized type3 fonts accept only mask images.", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AddInlineImageMaskTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            img.MakeMask();
            NUnit.Framework.Assert.IsNull(glyph.AddImageWithTransformationMatrix(img, 100, 0, 0, 100, 0, 0, true));
        }

        [NUnit.Framework.Test]
        public virtual void AddImageMaskAsNotInlineTest() {
            //TODO DEVSIX-5764 Display message error for non-inline images in type 3 glyph
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Type3Glyph glyph = new Type3Glyph(new PdfStream(), pdfDoc);
            ImageData img = ImageDataFactory.Create(SOURCE_FOLDER + "imageTest.png");
            img.MakeMask();
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => glyph.AddImageWithTransformationMatrix(
                img, 100, 0, 0, 100, 0, 0, false));
        }
    }
}
