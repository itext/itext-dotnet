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
/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/
using System;
using iText.Test;

namespace iText.Kernel.Pdf {
    /// <author>benoit</author>
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfXrefTableTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfXrefTableTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfXrefTableTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateAndUpdateXMP() {
            String created = destinationFolder + "testCreateAndUpdateXMP_create.pdf";
            String updated = destinationFolder + "testCreateAndUpdateXMP_update.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated));
            PdfXrefTable xref = pdfDocument.GetXref();
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            PdfIndirectReference ref0 = xref.Get(0);
            PdfIndirectReference freeRef = xref.Get(6);
            pdfDocument.Close();
            /*
            Current xref structure:
            xref
            0 8
            0000000006 65535 f % this is object 0; 6 refers to free object 6
            0000000203 00000 n
            0000000510 00000 n
            0000000263 00000 n
            0000000088 00000 n
            0000000015 00000 n
            0000000000 00001 f % this is object 6; 0 refers to free object 0; note generation number
            0000000561 00000 n
            */
            NUnit.Framework.Assert.IsTrue(freeRef.IsFree());
            NUnit.Framework.Assert.AreEqual(ref0.offsetOrIndex, freeRef.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef.genNr);
        }

        [NUnit.Framework.Test]
        public virtual void TestCreateAndUpdateTwiceXMP() {
            String created = destinationFolder + "testCreateAndUpdateTwiceXMP_create.pdf";
            String updated = destinationFolder + "testCreateAndUpdateTwiceXMP_update.pdf";
            String updatedAgain = destinationFolder + "testCreateAndUpdateTwiceXMP_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated));
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(updated), new PdfWriter(updatedAgain));
            catalog = pdfDocument.GetCatalog().GetPdfObject();
            ((PdfIndirectReference)catalog.Remove(PdfName.Metadata)).SetFree();
            PdfXrefTable xref = pdfDocument.GetXref();
            PdfIndirectReference ref0 = xref.Get(0);
            PdfIndirectReference freeRef1 = xref.Get(6);
            PdfIndirectReference freeRef2 = xref.Get(7);
            pdfDocument.Close();
            /*
            Current xref structure:
            xref
            0 9
            0000000006 65535 f % this is object 0; 6 refers to free object 6
            0000000203 00000 n
            0000000510 00000 n
            0000000263 00000 n
            0000000088 00000 n
            0000000015 00000 n
            0000000007 00001 f % this is object 6; 7 refers to free object 7; note generation number
            0000000000 00001 f % this is object 7; 0 refers to free object 0; note generation number
            0000000561 00000 n
            */
            NUnit.Framework.Assert.IsTrue(freeRef1.IsFree());
            NUnit.Framework.Assert.AreEqual(ref0.offsetOrIndex, freeRef1.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef1.genNr);
            NUnit.Framework.Assert.IsTrue(freeRef2.IsFree());
            NUnit.Framework.Assert.AreEqual(freeRef1.offsetOrIndex, freeRef2.objNr);
            NUnit.Framework.Assert.AreEqual(1, freeRef2.genNr);
            pdfDocument.Close();
        }
    }
}
