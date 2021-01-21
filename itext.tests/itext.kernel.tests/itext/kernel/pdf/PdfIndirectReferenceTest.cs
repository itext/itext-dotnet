using System.IO;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfIndirectReferenceTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BaseEqualsTest() {
            PdfIndirectReference reference = new PdfIndirectReference(null, 41, 0);
            NUnit.Framework.Assert.IsTrue(reference.Equals(reference));
            NUnit.Framework.Assert.IsFalse(reference.Equals(null));
            PdfIndirectReferenceTest.TestIndirectReference testIndirectReference = new PdfIndirectReferenceTest.TestIndirectReference
                (null, 41, 0);
            NUnit.Framework.Assert.IsFalse(reference.Equals(testIndirectReference));
            NUnit.Framework.Assert.IsFalse(testIndirectReference.Equals(reference));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithDocTest() {
            using (PdfDocument firstDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument secondDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    // add a page to avoid exception throwing on close
                    firstDoc.AddNewPage();
                    secondDoc.AddNewPage();
                    PdfIndirectReference obj41Gen0 = new PdfIndirectReference(firstDoc, 41, 0);
                    NUnit.Framework.Assert.IsTrue(obj41Gen0.Equals(new PdfIndirectReference(firstDoc, 41, 0)));
                    NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(firstDoc, 42, 0)));
                    NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(firstDoc, 41, 1)));
                    NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(null, 41, 0)));
                    NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(secondDoc, 41, 0)));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithNullDocsTest() {
            PdfIndirectReference obj41Gen0 = new PdfIndirectReference(null, 41, 0);
            NUnit.Framework.Assert.IsTrue(obj41Gen0.Equals(new PdfIndirectReference(null, 41, 0)));
            NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(null, 42, 0)));
            NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(null, 41, 1)));
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                // add a page to avoid exception throwing on close
                doc.AddNewPage();
                NUnit.Framework.Assert.IsFalse(obj41Gen0.Equals(new PdfIndirectReference(doc, 41, 0)));
            }
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeTest() {
            PdfIndirectReference firstReference = new PdfIndirectReference(null, 41, 7);
            PdfIndirectReference secondReference = new PdfIndirectReference(null, 41, 7);
            NUnit.Framework.Assert.AreNotSame(firstReference, secondReference);
            NUnit.Framework.Assert.AreEqual(firstReference.GetHashCode(), secondReference.GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(firstReference.GetHashCode(), new PdfIndirectReference(null, 42, 7).GetHashCode
                ());
            NUnit.Framework.Assert.AreNotEqual(firstReference.GetHashCode(), new PdfIndirectReference(null, 41, 5).GetHashCode
                ());
            using (PdfDocument firstDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument secondDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    // add a page to avoid exception throwing on close
                    firstDoc.AddNewPage();
                    secondDoc.AddNewPage();
                    PdfIndirectReference obj41Gen0 = new PdfIndirectReference(firstDoc, 41, 0);
                    NUnit.Framework.Assert.AreEqual(obj41Gen0.GetHashCode(), new PdfIndirectReference(firstDoc, 41, 0).GetHashCode
                        ());
                    NUnit.Framework.Assert.AreNotEqual(obj41Gen0.GetHashCode(), new PdfIndirectReference(secondDoc, 41, 0).GetHashCode
                        ());
                    NUnit.Framework.Assert.AreNotEqual(obj41Gen0.GetHashCode(), new PdfIndirectReference(null, 41, 0).GetHashCode
                        ());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompareToWithDocTest() {
            using (PdfDocument firstDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument secondDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    // add a page to avoid exception throwing on close
                    firstDoc.AddNewPage();
                    secondDoc.AddNewPage();
                    PdfIndirectReference obj41Gen7 = new PdfIndirectReference(firstDoc, 41, 7);
                    NUnit.Framework.Assert.AreEqual(0, obj41Gen7.CompareTo(new PdfIndirectReference(firstDoc, 41, 7)));
                    NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(firstDoc, 11, 17)));
                    NUnit.Framework.Assert.AreEqual(-1, obj41Gen7.CompareTo(new PdfIndirectReference(firstDoc, 51, 0)));
                    NUnit.Framework.Assert.AreEqual(-1, obj41Gen7.CompareTo(new PdfIndirectReference(firstDoc, 41, 17)));
                    NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(firstDoc, 41, 0)));
                    NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(null, 41, 7)));
                    // we do not expect that ids could be equal
                    int docIdsCompareResult = firstDoc.GetDocumentId() > secondDoc.GetDocumentId() ? 1 : -1;
                    NUnit.Framework.Assert.AreEqual(docIdsCompareResult, obj41Gen7.CompareTo(new PdfIndirectReference(secondDoc
                        , 41, 7)));
                    NUnit.Framework.Assert.AreEqual(-docIdsCompareResult, new PdfIndirectReference(secondDoc, 41, 7).CompareTo
                        (obj41Gen7));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompareToWithNullDocsTest() {
            PdfIndirectReference obj41Gen7 = new PdfIndirectReference(null, 41, 7);
            NUnit.Framework.Assert.AreEqual(0, obj41Gen7.CompareTo(new PdfIndirectReference(null, 41, 7)));
            NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(null, 11, 17)));
            NUnit.Framework.Assert.AreEqual(-1, obj41Gen7.CompareTo(new PdfIndirectReference(null, 51, 0)));
            NUnit.Framework.Assert.AreEqual(-1, obj41Gen7.CompareTo(new PdfIndirectReference(null, 41, 17)));
            NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(null, 41, 0)));
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                // add a page to avoid exception throwing on close
                doc.AddNewPage();
                NUnit.Framework.Assert.AreEqual(-1, obj41Gen7.CompareTo(new PdfIndirectReference(doc, 41, 7)));
                NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(doc, 11, 17)));
                NUnit.Framework.Assert.AreEqual(1, obj41Gen7.CompareTo(new PdfIndirectReference(doc, 41, 0)));
            }
        }

        private class TestIndirectReference : PdfIndirectReference {
            public TestIndirectReference(PdfDocument doc, int objNr, int genNr)
                : base(doc, objNr, genNr) {
            }
        }
    }
}
