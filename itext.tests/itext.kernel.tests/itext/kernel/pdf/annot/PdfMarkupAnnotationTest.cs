using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    public class PdfMarkupAnnotationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ExternalDataTest() {
            PdfMarkupAnnotation annotation = new PdfCircleAnnotation(new Rectangle(0, 0, 10, 10));
            annotation.SetExternalData(new PdfDictionary());
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, annotation.GetExternalData().GetObjectType());
        }
    }
}
