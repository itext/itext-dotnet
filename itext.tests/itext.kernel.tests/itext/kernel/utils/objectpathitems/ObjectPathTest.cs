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
using System.Collections.Generic;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ObjectPathTest : ExtendedITextTest {
        private PdfDocument testCmp;

        private PdfDocument testOut;

        [NUnit.Framework.SetUp]
        public virtual void SetUpPdfDocuments() {
            testCmp = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            testCmp.AddNewPage();
            testCmp.AddNewPage();
            testOut = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            testOut.AddNewPage();
            testOut.AddNewPage();
        }

        [NUnit.Framework.TearDown]
        public virtual void ClosePdfDocuments() {
            testCmp.Close();
            testOut.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetIndirectObjectsTest() {
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

        [NUnit.Framework.Test]
        public virtual void HashCodeWithoutNullParametersTest() {
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

        [NUnit.Framework.Test]
        public virtual void HashCodeWithNullParametersTest() {
            Stack<LocalPathItem> localPath = new Stack<LocalPathItem>();
            Stack<IndirectPathItem> indirectPathItems = new Stack<IndirectPathItem>();
            ObjectPath objectPath1 = new ObjectPath(null, null, localPath, indirectPathItems);
            ObjectPath objectPath2 = new ObjectPath(null, null, localPath, indirectPathItems);
            NUnit.Framework.Assert.AreEqual(0, objectPath1.GetHashCode());
            NUnit.Framework.Assert.AreEqual(objectPath1.GetHashCode(), objectPath2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
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

        [NUnit.Framework.Test]
        public virtual void NotEqualsAndHashCodeTest() {
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

        [NUnit.Framework.Test]
        public virtual void CloneConstructorTest() {
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
