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
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Utils.Objectpathitems {
    [NUnit.Framework.Category("IntegrationTest")]
    public class IndirectPathItemTest : ExtendedITextTest {
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
            IndirectPathItem indirectPathItem = new IndirectPathItem(cmpIndirect, outIndirect);
            NUnit.Framework.Assert.AreEqual(cmpIndirect, indirectPathItem.GetCmpObject());
            NUnit.Framework.Assert.AreEqual(outIndirect, indirectPathItem.GetOutObject());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsAndHashCodeTest() {
            PdfIndirectReference cmpIndirect = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
            PdfIndirectReference outIndirect = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
            IndirectPathItem indirectPathItem1 = new IndirectPathItem(cmpIndirect, outIndirect);
            IndirectPathItem indirectPathItem2 = new IndirectPathItem(cmpIndirect, outIndirect);
            bool result = indirectPathItem1.Equals(indirectPathItem2);
            NUnit.Framework.Assert.IsTrue(result);
            NUnit.Framework.Assert.AreEqual(indirectPathItem1.GetHashCode(), indirectPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsCmpObjAndHashCodeTest() {
            PdfIndirectReference cmpIndirect1 = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
            PdfIndirectReference outIndirect1 = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
            IndirectPathItem indirectPathItem1 = new IndirectPathItem(cmpIndirect1, outIndirect1);
            PdfIndirectReference cmpIndirect2 = testCmp.GetPage(2).GetPdfObject().GetIndirectReference();
            PdfIndirectReference outIndirect2 = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
            IndirectPathItem indirectPathItem2 = new IndirectPathItem(cmpIndirect2, outIndirect2);
            bool result = indirectPathItem1.Equals(indirectPathItem2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(indirectPathItem1.GetHashCode(), indirectPathItem2.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsOutObjAndHashCodeTest() {
            PdfIndirectReference cmpIndirect1 = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
            PdfIndirectReference outIndirect1 = testOut.GetFirstPage().GetPdfObject().GetIndirectReference();
            IndirectPathItem indirectPathItem1 = new IndirectPathItem(cmpIndirect1, outIndirect1);
            PdfIndirectReference cmpIndirect2 = testCmp.GetFirstPage().GetPdfObject().GetIndirectReference();
            PdfIndirectReference outIndirect2 = testOut.GetPage(2).GetPdfObject().GetIndirectReference();
            IndirectPathItem indirectPathItem2 = new IndirectPathItem(cmpIndirect2, outIndirect2);
            bool result = indirectPathItem1.Equals(indirectPathItem2);
            NUnit.Framework.Assert.IsFalse(result);
            NUnit.Framework.Assert.AreNotEqual(indirectPathItem1.GetHashCode(), indirectPathItem2.GetHashCode());
        }
    }
}
