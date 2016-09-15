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

        [NUnit.Framework.Test]
        public virtual void TestContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array2.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            foreach (PdfObject obj in array2) {
                NUnit.Framework.Assert.IsTrue(array.Contains(obj));
            }
            for (int i = 0; i < array2.Size(); i++) {
                NUnit.Framework.Assert.IsTrue(array.Contains(array2.Get(i)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array2.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            foreach (PdfObject obj in array2) {
                array.Remove(obj);
            }
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestRemove2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array2.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            for (int i = 0; i < array2.Size(); i++) {
                array.Remove(array2.Get(i));
            }
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestIndexOf() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array2.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            int i = 0;
            foreach (PdfObject obj in array2) {
                NUnit.Framework.Assert.AreEqual(i++, array.IndexOf(obj));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestIndexOf2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfArray array = new PdfArray();
            array.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array.Add(new PdfNumber(5));
            array.Add(new PdfNumber(6));
            PdfArray array2 = new PdfArray();
            array2.Add(((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            array2.Add(((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            array2.Add(((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            array2.Add(new PdfNumber(5));
            array2.Add(new PdfNumber(6));
            for (int i = 0; i < array2.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(i, array.IndexOf(array2.Get(i)));
            }
        }
    }
}
