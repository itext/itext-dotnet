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
using System;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class XrefStreamDocumentUpdatesTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XrefStreamDocumentUpdatesTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/XrefStreamDocumentUpdatesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outputFile)
                .SetCompressionLevel(CompressionConstants.NO_COMPRESSION));
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
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outputFile)
                .SetCompressionLevel(CompressionConstants.NO_COMPRESSION), new StampingProperties().UseAppendMode());
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
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(sourceFolder + "pdfWithRemovedObjInOldVer.pdf"), CompareTool
                .CreateTestPdfWriter(filename).SetCompressionLevel(CompressionConstants.NO_COMPRESSION), new StampingProperties
                ().UseAppendMode());
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

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CheckEncryptionInXrefStmInIncrementsTest() {
            String inFileName = sourceFolder + "encryptedDocWithXrefStm.pdf";
            String outFileName = destinationFolder + "checkEncryptionInXrefStmInIncrements.pdf";
            PdfReader pdfReader = new PdfReader(inFileName).SetUnethicalReading(true);
            PdfDocument pdfDocument = new PdfDocument(pdfReader, CompareTool.CreateTestPdfWriter(outFileName), new StampingProperties
                ().UseAppendMode().PreserveEncryption());
            PdfDictionary xrefStm = (PdfDictionary)pdfDocument.GetPdfObject(6);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, inFileName, destinationFolder
                ));
            NUnit.Framework.Assert.AreEqual(PdfName.XRef, xrefStm.Get(PdfName.Type));
        }

        [NUnit.Framework.Test]
        public virtual void HybridReferenceInIncrementsTest() {
            String inFileName = sourceFolder + "hybridReferenceDocument.pdf";
            String outFileName = destinationFolder + "hybridReferenceInIncrements.pdf";
            PdfReader pdfReader = new PdfReader(inFileName);
            PdfDocument pdfDocument = new PdfDocument(pdfReader, CompareTool.CreateTestPdfWriter(outFileName), new StampingProperties
                ().UseAppendMode());
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, inFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void XrefStmInWriteModeTest() {
            String fileName = destinationFolder + "xrefStmInWriteMode.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(fileName, new WriterProperties().SetFullCompressionMode
                (true).SetCompressionLevel(CompressionConstants.NO_COMPRESSION));
            PdfDocument pdfDocument = new PdfDocument(writer);
            PdfPage page = pdfDocument.AddNewPage();
            PdfTextAnnotation textannot = new PdfTextAnnotation(new Rectangle(100, 600, 50, 40));
            textannot.SetText(new PdfString("Text Annotation 01")).SetContents(new PdfString("Some contents..."));
            page.AddAnnotation(textannot);
            pdfDocument.Close();
            PdfDocument doc = new PdfDocument(CompareTool.CreateOutputReader(fileName));
            int xrefTableCounter = 0;
            for (int i = 1; i < doc.GetNumberOfPdfObjects(); i++) {
                PdfObject obj = doc.GetPdfObject(i);
                if (obj is PdfDictionary) {
                    PdfDictionary objStmDict = (PdfDictionary)doc.GetPdfObject(i);
                    PdfObject type = objStmDict.Get(PdfName.Type);
                    if (type != null && type.Equals(PdfName.XRef)) {
                        xrefTableCounter++;
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(((PdfNumber)doc.GetTrailer().Get(PdfName.Size)).IntValue(), doc.GetNumberOfPdfObjects
                ());
            doc.Close();
            NUnit.Framework.Assert.AreEqual(1, xrefTableCounter);
        }

        [NUnit.Framework.Test]
        public virtual void XrefStmInAppendModeTest() {
            String fileName = destinationFolder + "xrefStmInAppendMode.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "xrefStmInWriteMode.pdf"), CompareTool
                .CreateTestPdfWriter(fileName).SetCompressionLevel(CompressionConstants.NO_COMPRESSION), new StampingProperties
                ().UseAppendMode());
            pdfDocument.Close();
            PdfDocument doc = new PdfDocument(CompareTool.CreateOutputReader(fileName));
            int xrefTableCounter = 0;
            for (int i = 1; i < doc.GetNumberOfPdfObjects(); i++) {
                PdfObject obj = doc.GetPdfObject(i);
                if (obj is PdfDictionary) {
                    PdfDictionary objStmDict = (PdfDictionary)doc.GetPdfObject(i);
                    PdfObject type = objStmDict.Get(PdfName.Type);
                    if (type != null && type.Equals(PdfName.XRef)) {
                        xrefTableCounter++;
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(((PdfNumber)doc.GetTrailer().Get(PdfName.Size)).IntValue(), doc.GetNumberOfPdfObjects
                ());
            doc.Close();
            NUnit.Framework.Assert.AreEqual(2, xrefTableCounter);
        }

        [NUnit.Framework.Test]
        public virtual void CloseDocumentWithoutModificationsTest() {
            String fileName = destinationFolder + "xrefStmInAppendMode.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "xrefStmInWriteMode.pdf"), CompareTool
                .CreateTestPdfWriter(fileName).SetCompressionLevel(CompressionConstants.NO_COMPRESSION), new StampingProperties
                ().UseAppendMode());
            // Clear state for document info indirect reference so that there are no modified objects
            // in the document due to which, the document will have only one xref table.
            pdfDocument.GetDocumentInfo().GetPdfObject().GetIndirectReference().ClearState(PdfObject.MODIFIED);
            pdfDocument.Close();
            PdfDocument doc = new PdfDocument(CompareTool.CreateOutputReader(fileName));
            int xrefTableCounter = 0;
            for (int i = 1; i < doc.GetNumberOfPdfObjects(); i++) {
                PdfObject obj = doc.GetPdfObject(i);
                if (obj is PdfDictionary) {
                    PdfDictionary objStmDict = (PdfDictionary)doc.GetPdfObject(i);
                    PdfObject type = objStmDict.Get(PdfName.Type);
                    if (type != null && type.Equals(PdfName.XRef)) {
                        xrefTableCounter++;
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(((PdfNumber)doc.GetTrailer().Get(PdfName.Size)).IntValue(), doc.GetNumberOfPdfObjects
                ());
            doc.Close();
            NUnit.Framework.Assert.AreEqual(1, xrefTableCounter);
        }

        [NUnit.Framework.Test]
        public virtual void HybridReferenceIncrementTwiceTest() {
            String inFileName = sourceFolder + "hybridReferenceDocument.pdf";
            String outFileName = destinationFolder + "hybridReferenceDocumentUpdateTwice.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(inFileName), new PdfWriter(destinationFolder + "hybridReferenceDocumentUpdate.pdf"
                ), new StampingProperties().UseAppendMode());
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(new PdfReader(destinationFolder + "hybridReferenceDocumentUpdate.pdf"
                ), CompareTool.CreateTestPdfWriter(outFileName), new StampingProperties().UseAppendMode());
            pdfDoc2.Close();
            //if document processed correctly, no errors should occur
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, inFileName, destinationFolder
                ));
        }
    }
}
