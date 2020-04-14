/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using System;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class XrefStreamDocumentUpdatesTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XrefStreamDocumentUpdatesTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/XrefStreamDocumentUpdatesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ReadFreeRefReusingInIncrementTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "readFreeRefReusingInIncrement.pdf"));
            PdfArray array = (PdfArray)document.GetCatalog().GetPdfObject().Get(new PdfName("CustomKey"));
            NUnit.Framework.Assert.IsTrue(array is PdfArray);
            NUnit.Framework.Assert.AreEqual(0, array.Size());
        }

        [NUnit.Framework.Test]
        public virtual void NotReuseIndirectRefForObjectStreamTest() {
            String inputFile = sourceFolder + "notReuseIndirectRefForObjectStream.pdf";
            String outputFile = destinationFolder + "adjustingsInObjStm.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile).SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION));
            PdfArray media = pdfDoc.GetPage(1).GetPdfObject().GetAsArray(PdfName.MediaBox);
            media.Remove(2);
            media.Add(new PdfNumber(500));
            media.SetModified();
            pdfDoc.Close();
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "adjustingsInObjStm.pdf"));
            PdfObject @object = doc.GetPdfObject(8);
            PdfDictionary pageDict = (PdfDictionary)@object;
            int expectNumberOfObjects = pdfDoc.GetNumberOfPdfObjects();
            //output pdf document should be openable
            NUnit.Framework.Assert.AreEqual(10, expectNumberOfObjects);
            NUnit.Framework.Assert.AreEqual(PdfName.ObjStm, pageDict.Get(PdfName.Type));
        }

        [NUnit.Framework.Test]
        public virtual void NotReuseIndRefForObjStreamInIncrementTest() {
            String inputFile = sourceFolder + "notReuseIndirectRefForObjectStream.pdf";
            String outputFile = destinationFolder + "adjustingsInObjStmInIncrement.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputFile).SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION), new StampingProperties().UseAppendMode());
            PdfObject newObj = pdfDoc.GetPage(1).GetPdfObject();
            newObj.SetModified();
            pdfDoc.Close();
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "adjustingsInObjStmInIncrement.pdf"));
            PdfDictionary objStmDict = (PdfDictionary)doc.GetPdfObject(8);
            int expectNumberOfObjects = doc.GetNumberOfPdfObjects();
            //output pdf document should be openable
            NUnit.Framework.Assert.AreEqual(9, expectNumberOfObjects);
            NUnit.Framework.Assert.AreEqual(PdfName.ObjStm, objStmDict.Get(PdfName.Type));
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FreeRefReuseWhenAddNewObjTest() {
            String filename = destinationFolder + "freeRefReuseWhenAddNewObj.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(sourceFolder + "pdfWithRemovedObjInOldVer.pdf"), new PdfWriter
                (filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION), new StampingProperties().UseAppendMode
                ());
            pdfDoc1.GetCatalog().GetPdfObject().Put(new PdfName("CustomKey"), new PdfArray().MakeIndirect(pdfDoc1));
            PdfObject newObj = pdfDoc1.GetCatalog().GetPdfObject();
            newObj.SetModified();
            int expectObjNumber = pdfDoc1.GetCatalog().GetPdfObject().Get(new PdfName("CustomKey")).GetIndirectReference
                ().GetObjNumber();
            int expectGenNumber = pdfDoc1.GetCatalog().GetPdfObject().Get(new PdfName("CustomKey")).GetIndirectReference
                ().GetGenNumber();
            PdfXrefTable xref = pdfDoc1.GetXref();
            NUnit.Framework.Assert.AreEqual(8, expectObjNumber);
            NUnit.Framework.Assert.AreEqual(0, expectGenNumber);
            NUnit.Framework.Assert.IsTrue(xref.Get(5).IsFree());
            pdfDoc1.Close();
        }
    }
}
