using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils {
    public class CompareToolObjectPathTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BaseEqualsTest() {
            CompareTool compareTool = new CompareTool();
            PdfIndirectReference firstReference = CreateIndirectReference(null, 41, 0);
            PdfIndirectReference secondReference = CreateIndirectReference(null, 42, 0);
            CompareTool.ObjectPath path = new CompareTool.ObjectPath(firstReference, secondReference);
            NUnit.Framework.Assert.IsTrue(path.Equals(path));
            NUnit.Framework.Assert.IsFalse(path.Equals(null));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(firstReference, secondReference).Equals(new CompareTool.ObjectPath
                (null, secondReference)));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(null, secondReference).Equals(new CompareTool.ObjectPath
                (firstReference, secondReference)));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(firstReference, secondReference).Equals(new CompareTool.ObjectPath
                (firstReference, null)));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(firstReference, secondReference).Equals(new CompareTool.ObjectPath
                (null, secondReference)));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(firstReference, secondReference).Equals(new CompareTool.ObjectPath
                (new CompareToolObjectPathTest.TestIndirectReference(null, 41, 0), secondReference)));
            NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(firstReference, secondReference).Equals(new CompareTool.ObjectPath
                (firstReference, new CompareToolObjectPathTest.TestIndirectReference(null, 42, 0))));
        }

        [NUnit.Framework.Test]
        public virtual void EqualsWithDocTest() {
            CompareTool compareTool = new CompareTool();
            using (PdfDocument firstDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument secondDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    // add a page to avoid exception throwing on close
                    firstDoc.AddNewPage();
                    secondDoc.AddNewPage();
                    PdfIndirectReference obj41Gen0 = CreateIndirectReference(firstDoc, 41, 0);
                    PdfIndirectReference obj42Gen0 = CreateIndirectReference(firstDoc, 42, 0);
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 41, 0), obj42Gen0)));
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(firstDoc, 42, 0))));
                    NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 42, 0), obj42Gen0)));
                    NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(firstDoc, 41, 0))));
                    NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 41, 1), obj42Gen0)));
                    NUnit.Framework.Assert.IsFalse(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(firstDoc, 42, 1))));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (CreateIndirectReference(null, 41, 0), obj42Gen0)));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(null, 42, 0))));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (CreateIndirectReference(secondDoc, 41, 0), obj42Gen0)));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).Equals(new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(secondDoc, 42, 0))));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeTest() {
            CompareTool compareTool = new CompareTool();
            using (PdfDocument firstDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (PdfDocument secondDoc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                    // add a page to avoid exception throwing on close
                    firstDoc.AddNewPage();
                    secondDoc.AddNewPage();
                    PdfIndirectReference obj41Gen0 = CreateIndirectReference(firstDoc, 41, 0);
                    PdfIndirectReference obj42Gen0 = CreateIndirectReference(firstDoc, 42, 0);
                    NUnit.Framework.Assert.AreEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    NUnit.Framework.Assert.AreNotEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 42, 0), obj42Gen0).GetHashCode());
                    NUnit.Framework.Assert.AreNotEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (obj41Gen0, CreateIndirectReference(firstDoc, 41, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(null, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 41, 0), CreateIndirectReference(null, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(secondDoc, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new CompareTool.ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new CompareTool.ObjectPath
                        (CreateIndirectReference(firstDoc, 41, 0), CreateIndirectReference(secondDoc, 42, 0)).GetHashCode());
                }
            }
        }

        private PdfIndirectReference CreateIndirectReference(PdfDocument doc, int objNr, int genNr) {
            return new CompareToolObjectPathTest.PdfIndirectReferenceWithPublicConstructor(doc, objNr, genNr);
        }

        private class PdfIndirectReferenceWithPublicConstructor : PdfIndirectReference {
            public PdfIndirectReferenceWithPublicConstructor(PdfDocument doc, int objNr, int genNr)
                : base(doc, objNr, genNr) {
            }
        }

        private class TestIndirectReference : CompareToolObjectPathTest.PdfIndirectReferenceWithPublicConstructor {
            public TestIndirectReference(PdfDocument doc, int objNr, int genNr)
                : base(doc, objNr, genNr) {
            }
        }
    }
}
