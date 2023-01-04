/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
