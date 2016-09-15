using System;
using System.Collections.Generic;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    public class PdfDictionaryTest {
        [NUnit.Framework.Test]
        public virtual void TestEntrySet() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(((PdfNumber)e.Value).IntValue())) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Contains(e));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetRemove() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<KeyValuePair<PdfName, PdfObject>> toRemove = new List<KeyValuePair<PdfName, PdfObject>>();
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                toRemove.Add(e);
            }
            foreach (KeyValuePair<PdfName, PdfObject> e_1 in toRemove) {
                NUnit.Framework.Assert.IsTrue(dict.EntrySet().Remove(e_1));
            }
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-851")]
        public virtual void TestEntrySetRemove2() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            PdfDictionary dict2 = new PdfDictionary();
            dict2.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict2.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict2.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict2.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict2.Put(new PdfName("5"), new PdfNumber(5));
            dict2.Put(new PdfName("6"), new PdfNumber(6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                if (e.Value.GetIndirectReference() != null) {
                    continue;
                }
                NUnit.Framework.Assert.IsTrue(dict2.EntrySet().Remove(e));
            }
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestEntrySetClear() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            dict.EntrySet().Clear();
            NUnit.Framework.Assert.AreEqual(0, dict.EntrySet().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Values().Count);
            NUnit.Framework.Assert.AreEqual(0, dict.Size());
        }

        [NUnit.Framework.Test]
        public virtual void TestValues() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            IList<int> nums = new List<int>(iText.IO.Util.JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6));
            foreach (KeyValuePair<PdfName, PdfObject> e in dict.EntrySet()) {
                NUnit.Framework.Assert.AreEqual(e.Key.ToString(), "/" + e.Value);
                if (!nums.Remove(((PdfNumber)e.Value).IntValue())) {
                    NUnit.Framework.Assert.Fail("Element not found");
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            foreach (PdfObject v in dict.Values()) {
                NUnit.Framework.Assert.IsTrue(dict.Values().Contains(v));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValuesIndirectContains() {
            PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDictionary dict = new PdfDictionary();
            dict.Put(new PdfName("1"), ((PdfNumber)new PdfNumber(1).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("2"), ((PdfNumber)new PdfNumber(2).MakeIndirect(doc)).GetIndirectReference());
            dict.Put(new PdfName("3"), ((PdfNumber)new PdfNumber(3).MakeIndirect(doc)));
            dict.Put(new PdfName("4"), ((PdfNumber)new PdfNumber(4).MakeIndirect(doc)));
            dict.Put(new PdfName("5"), new PdfNumber(5));
            dict.Put(new PdfName("6"), new PdfNumber(6));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("1"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("2"), false)));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("3")).GetIndirectReference()));
            NUnit.Framework.Assert.IsTrue(dict.Values().Contains(dict.Get(new PdfName("4")).GetIndirectReference()));
        }
    }
}
