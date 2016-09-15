using iText.IO.Source;

namespace iText.Kernel.Pdf {
    public class PdfArrayTest {
        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(0).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(new PdfNumber(4));
            array.Add(new PdfNumber(5));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(0, false)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(1, false)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(2).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(3).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(4)));
            NUnit.Framework.Assert.IsTrue(array.Contains(array.Get(5)));
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            array.Remove(array.Get(0, false));
            array.Remove(array.Get(0, false));
            array.Remove(array.Get(0).GetIndirectReference());
            array.Remove(array.Get(0).GetIndirectReference());
            array.Remove(array.Get(0));
            array.Remove(array.Get(0));
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }
    }
}
