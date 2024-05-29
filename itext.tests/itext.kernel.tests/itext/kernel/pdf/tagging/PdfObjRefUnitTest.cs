using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Tagging {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfObjRefUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void RefObjAsStreamTest() {
            PdfDictionary @ref = new PdfStream();
            @ref.Put(PdfName.Name, new PdfString("reference"));
            PdfDictionary obj = new PdfDictionary();
            obj.Put(PdfName.Obj, @ref);
            PdfObjRef objRef = new PdfObjRef(obj, new PdfStructElem(new PdfDictionary()));
            NUnit.Framework.Assert.IsTrue(objRef.GetReferencedObject() is PdfStream);
            NUnit.Framework.Assert.IsTrue(objRef.GetReferencedObject().ContainsKey(PdfName.Name));
        }

        [NUnit.Framework.Test]
        public virtual void RefObjAsInvalidTypeTest() {
            PdfDictionary obj = new PdfDictionary();
            obj.Put(PdfName.Obj, new PdfString("incorrect type"));
            PdfObjRef objRef = new PdfObjRef(obj, new PdfStructElem(new PdfDictionary()));
            NUnit.Framework.Assert.IsNull(objRef.GetReferencedObject());
        }
    }
}
