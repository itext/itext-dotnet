/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils.Objectpathitems;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class CompareToolObjectPathTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BaseEqualsTest() {
            PdfIndirectReference firstReference = CreateIndirectReference(null, 41, 0);
            PdfIndirectReference secondReference = CreateIndirectReference(null, 42, 0);
            ObjectPath path = new ObjectPath(firstReference, secondReference);
            NUnit.Framework.Assert.IsTrue(path.Equals(path));
            NUnit.Framework.Assert.IsFalse(path.Equals(null));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(firstReference, secondReference).Equals(new ObjectPath(null, 
                secondReference)));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(null, secondReference).Equals(new ObjectPath(firstReference, 
                secondReference)));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(firstReference, secondReference).Equals(new ObjectPath(firstReference
                , null)));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(firstReference, secondReference).Equals(new ObjectPath(null, 
                secondReference)));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(firstReference, secondReference).Equals(new ObjectPath(new CompareToolObjectPathTest.TestIndirectReference
                (null, 41, 0), secondReference)));
            NUnit.Framework.Assert.IsFalse(new ObjectPath(firstReference, secondReference).Equals(new ObjectPath(firstReference
                , new CompareToolObjectPathTest.TestIndirectReference(null, 42, 0))));
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
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(CreateIndirectReference
                        (firstDoc, 41, 0), obj42Gen0)));
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(obj41Gen0, CreateIndirectReference
                        (firstDoc, 42, 0))));
                    NUnit.Framework.Assert.IsFalse(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(CreateIndirectReference
                        (firstDoc, 42, 0), obj42Gen0)));
                    NUnit.Framework.Assert.IsFalse(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(obj41Gen0, CreateIndirectReference
                        (firstDoc, 41, 0))));
                    NUnit.Framework.Assert.IsFalse(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(CreateIndirectReference
                        (firstDoc, 41, 1), obj42Gen0)));
                    NUnit.Framework.Assert.IsFalse(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(obj41Gen0, CreateIndirectReference
                        (firstDoc, 42, 1))));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(CreateIndirectReference
                        (null, 41, 0), obj42Gen0)));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(obj41Gen0, CreateIndirectReference
                        (null, 42, 0))));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(CreateIndirectReference
                        (secondDoc, 41, 0), obj42Gen0)));
                    // TODO: DEVSIX-4756 start asserting false
                    NUnit.Framework.Assert.IsTrue(new ObjectPath(obj41Gen0, obj42Gen0).Equals(new ObjectPath(obj41Gen0, CreateIndirectReference
                        (secondDoc, 42, 0))));
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
                    NUnit.Framework.Assert.AreEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (firstDoc, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    NUnit.Framework.Assert.AreNotEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (firstDoc, 42, 0), obj42Gen0).GetHashCode());
                    NUnit.Framework.Assert.AreNotEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(obj41Gen0
                        , CreateIndirectReference(firstDoc, 41, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (null, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (firstDoc, 41, 0), CreateIndirectReference(null, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (secondDoc, 41, 0), CreateIndirectReference(firstDoc, 42, 0)).GetHashCode());
                    // TODO: DEVSIX-4756 start asserting not equals
                    NUnit.Framework.Assert.AreEqual(new ObjectPath(obj41Gen0, obj42Gen0).GetHashCode(), new ObjectPath(CreateIndirectReference
                        (firstDoc, 41, 0), CreateIndirectReference(secondDoc, 42, 0)).GetHashCode());
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
