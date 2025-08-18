/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ObjectPathTest : ExtendedITextTest {
        private void Init(PdfDocument pdfDocument) {
            pdfDocument.AddNewPage();
            pdfDocument.AddNewPage();
        }

        [NUnit.Framework.Test]
        public virtual void GetIndirectObjectsTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
                    PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    localPath.Push(new ArrayPathItem(1));
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    indirectPathItems.Push(new IndirectPathItem(cmpIndirect, outIndirect));
                    ObjectPath objectPath = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    NUnit.Framework.Assert.AreEqual(cmpIndirect, objectPath.GetBaseCmpObject());
                    NUnit.Framework.Assert.AreEqual(outIndirect, objectPath.GetBaseOutObject());
                    NUnit.Framework.Assert.AreEqual(localPath, objectPath.GetLocalPath());
                    NUnit.Framework.Assert.AreEqual(indirectPathItems, objectPath.GetIndirectPath());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeWithoutNullParametersTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
                    PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    localPath.Push(new ArrayPathItem(1));
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    indirectPathItems.Push(new IndirectPathItem(cmpIndirect, outIndirect));
                    ObjectPath objectPath1 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    ObjectPath objectPath2 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    NUnit.Framework.Assert.AreNotEqual(0, objectPath1.GetHashCode());
                    NUnit.Framework.Assert.AreEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void HashCodeWithNullParametersTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    ObjectPath objectPath1 = new ObjectPath(null, null, localPath, indirectPathItems);
                    ObjectPath objectPath2 = new ObjectPath(null, null, localPath, indirectPathItems);
                    NUnit.Framework.Assert.AreEqual(0, objectPath1.GetHashCode());
                    NUnit.Framework.Assert.AreEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
                    PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    localPath.Push(new ArrayPathItem(1));
                    localPath.Push(new ArrayPathItem(2));
                    localPath.Push(new ArrayPathItem(3));
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    indirectPathItems.Push(new IndirectPathItem(cmpIndirect, outIndirect));
                    ObjectPath objectPath1 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    ObjectPath objectPath2 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    bool result = objectPath1.Equals(objectPath2);
                    NUnit.Framework.Assert.IsTrue(result);
                    NUnit.Framework.Assert.AreEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
                    PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    localPath.Push(new ArrayPathItem(1));
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    indirectPathItems.Push(new IndirectPathItem(cmpIndirect, outIndirect));
                    ObjectPath objectPath1 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    localPath = new Stack<LocalPathItem>();
                    indirectPathItems = new Stack<IndirectPathItem>();
                    ObjectPath objectPath2 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    bool result = objectPath1.Equals(objectPath2);
                    NUnit.Framework.Assert.IsFalse(result);
                    NUnit.Framework.Assert.AreNotEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CloneConstructorTest() {
            using (PdfDocument testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    Init(testCmp);
                    Init(testOut);
                    PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
                    PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
                    Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
                    localPath.Push(new ArrayPathItem(1));
                    Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
                    indirectPathItems.Push(new IndirectPathItem(cmpIndirect, outIndirect));
                    ObjectPath objectPath1 = new ObjectPath(cmpIndirect, outIndirect, localPath, indirectPathItems);
                    ObjectPath objectPath2 = new ObjectPath(objectPath1);
                    bool result = objectPath1.Equals(objectPath2);
                    NUnit.Framework.Assert.IsTrue(result);
                    NUnit.Framework.Assert.AreEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
                }
            }
        }
    }
}
