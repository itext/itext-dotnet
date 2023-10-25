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
using System.IO;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Kernel.XMP.Options;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfDocumentTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDocumentTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void MissingProducerTest() {
            String inputFile = SOURCE_FOLDER + "missingProducer.pdf";
            MemoryStream outputStream = new MemoryStream();
            using (PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputStream))) {
                PdfDocumentInfo documentInfo = document.GetDocumentInfo();
                NUnit.Framework.Assert.IsNull(documentInfo.GetPdfObject().Get(PdfName.Producer));
                NUnit.Framework.Assert.IsNull(documentInfo.GetProducer());
            }
            MemoryStream inputStream = new MemoryStream(outputStream.ToArray());
            using (PdfDocument document_1 = new PdfDocument(new PdfReader(inputStream), new PdfWriter(new MemoryStream
                ()))) {
                PdfDocumentInfo documentInfo = document_1.GetDocumentInfo();
                NUnit.Framework.Assert.IsNotNull(documentInfo.GetPdfObject().Get(PdfName.Producer));
                NUnit.Framework.Assert.IsNotNull(document_1.GetDocumentInfo().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NullProducerTest() {
            String inputFile = SOURCE_FOLDER + "nullProducer.pdf";
            MemoryStream outputStream = new MemoryStream();
            using (PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputStream))) {
                PdfDocumentInfo documentInfo = document.GetDocumentInfo();
                NUnit.Framework.Assert.AreEqual(PdfNull.PDF_NULL, documentInfo.GetPdfObject().Get(PdfName.Producer));
                NUnit.Framework.Assert.IsNull(documentInfo.GetProducer());
            }
            MemoryStream inputStream = new MemoryStream(outputStream.ToArray());
            using (PdfDocument document_1 = new PdfDocument(new PdfReader(inputStream), new PdfWriter(new MemoryStream
                ()))) {
                PdfDocumentInfo documentInfo = document_1.GetDocumentInfo();
                NUnit.Framework.Assert.IsNotNull(documentInfo.GetPdfObject().Get(PdfName.Producer));
                NUnit.Framework.Assert.IsNotNull(document_1.GetDocumentInfo().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NameProducerTest() {
            String inputFile = SOURCE_FOLDER + "nameProducer.pdf";
            MemoryStream outputStream = new MemoryStream();
            using (PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outputStream))) {
                PdfDocumentInfo documentInfo = document.GetDocumentInfo();
                NUnit.Framework.Assert.AreEqual(new PdfName("producerAsName"), documentInfo.GetPdfObject().Get(PdfName.Producer
                    ));
                NUnit.Framework.Assert.IsNull(documentInfo.GetProducer());
            }
            MemoryStream inputStream = new MemoryStream(outputStream.ToArray());
            using (PdfDocument document_1 = new PdfDocument(new PdfReader(inputStream), new PdfWriter(new MemoryStream
                ()))) {
                PdfDocumentInfo documentInfo = document_1.GetDocumentInfo();
                NUnit.Framework.Assert.IsNotNull(documentInfo.GetPdfObject().Get(PdfName.Producer));
                NUnit.Framework.Assert.IsNotNull(document_1.GetDocumentInfo().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void WritingVersionTest01() {
            // There is a possibility to override version in stamping mode
            String @out = DESTINATION_FOLDER + "writing_pdf_version.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(@out, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            PdfDocument assertPdfDoc = new PdfDocument(CompareTool.CreateOutputReader(@out));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
            assertPdfDoc.Close();
        }

        //We have this test in PdfOutlineTest as well, because we had some issues with outlines before. One test worked
        // fine, while another one failed.
        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations01() {
            String filename = DESTINATION_FOLDER + "outlinesWithNamedDestinations01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf"), CompareTool.CreateTestPdfWriter
                (filename));
            PdfArray array1 = new PdfArray();
            array1.Add(pdfDoc.GetPage(2).GetPdfObject());
            array1.Add(PdfName.XYZ);
            array1.Add(new PdfNumber(36));
            array1.Add(new PdfNumber(806));
            array1.Add(new PdfNumber(0));
            PdfArray array2 = new PdfArray();
            array2.Add(pdfDoc.GetPage(3).GetPdfObject());
            array2.Add(PdfName.XYZ);
            array2.Add(new PdfNumber(36));
            array2.Add(new PdfNumber(806));
            array2.Add(new PdfNumber(1.25));
            PdfArray array3 = new PdfArray();
            array3.Add(pdfDoc.GetPage(4).GetPdfObject());
            array3.Add(PdfName.XYZ);
            array3.Add(new PdfNumber(36));
            array3.Add(new PdfNumber(806));
            array3.Add(new PdfNumber(1));
            pdfDoc.AddNamedDestination("test1", array2);
            pdfDoc.AddNamedDestination("test2", array3);
            pdfDoc.AddNamedDestination("test3", array1);
            PdfOutline root = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = root.AddOutline("Test1");
            firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test1")));
            PdfOutline secondOutline = root.AddOutline("Test2");
            secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test2")));
            PdfOutline thirdOutline = root.AddOutline("Test3");
            thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test3")));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_outlinesWithNamedDestinations01.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesInObjectStream() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "styledLineArts_Redacted.pdf");
            PdfWriter writer = new PdfWriter(new MemoryStream());
            PdfDocument document = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            PdfDictionary dict = new PdfDictionary();
            dict.MakeIndirect(document);
            NUnit.Framework.Assert.IsTrue(dict.GetIndirectReference().GetObjNumber() > 0);
        }

        [NUnit.Framework.Test]
        public virtual void RemoveUnusedObjectsInWriterModeTest() {
            String filename = "removeUnusedObjectsInWriter.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filename));
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument);
            NUnit.Framework.Assert.AreEqual(pdfDocument.GetXref().Size(), 8);
            //on closing, all unused objects shall not be written to resultant document
            pdfDocument.Close();
            PdfDocument testerDocument = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filename)
                );
            NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 6);
            testerDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RemoveUnusedObjectsInStampingModeTest() {
            String filenameIn = "docWithUnusedObjects_1.pdf";
            String filenameOut = "removeUnusedObjectsInStamping.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filenameIn)
                );
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            PdfDocument doc = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filenameIn), CompareTool
                .CreateTestPdfWriter(DESTINATION_FOLDER + filenameOut));
            NUnit.Framework.Assert.AreEqual(doc.GetXref().Size(), 8);
            //on closing, all unused objects shall not be written to resultant document
            doc.Close();
            PdfDocument testerDocument = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filenameOut
                ));
            NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 6);
            testerDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AddUnusedObjectsInWriterModeTest() {
            String filename = "addUnusedObjectsInWriter.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filename));
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument);
            NUnit.Framework.Assert.AreEqual(pdfDocument.GetXref().Size(), 8);
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            PdfDocument testerDocument = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filename)
                );
            NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 8);
            testerDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AddUnusedObjectsInStampingModeTest() {
            String filenameIn = "docWithUnusedObjects_2.pdf";
            String filenameOut = "addUnusedObjectsInStamping.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filenameIn)
                );
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument).Flush();
            pdfDocument.Close();
            PdfDocument doc = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filenameIn), CompareTool
                .CreateTestPdfWriter(DESTINATION_FOLDER + filenameOut));
            NUnit.Framework.Assert.AreEqual(doc.GetXref().Size(), 8);
            doc.SetFlushUnusedObjects(true);
            doc.Close();
            PdfDocument testerDocument = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filenameOut
                ));
            NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 8);
            testerDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AddUnusedStreamObjectsTest() {
            String filenameIn = "docWithUnusedObjects_3.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filenameIn)
                );
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            PdfStream stream = new PdfStream(new byte[] { 1, 2, 34, 45 }, 0);
            unusedArray.Add(stream);
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument).Flush();
            pdfDocument.SetFlushUnusedObjects(true);
            pdfDocument.Close();
            PdfDocument testerDocument = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + filenameIn
                ));
            NUnit.Framework.Assert.AreEqual(testerDocument.GetXref().Size(), 9);
            testerDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestImageCompressLevel() {
            byte[] b = ImageDataFactory.Create(SOURCE_FOLDER + "berlin2013.jpg").GetData();
            ByteArrayOutputStream image = new ByteArrayOutputStream();
            image.AssignBytes(b, b.Length);
            MemoryStream byteArrayStream1 = new ByteArrayOutputStream();
            DeflaterOutputStream zip = new DeflaterOutputStream(byteArrayStream1, 9);
            image.WriteTo(zip);
            MemoryStream byteArrayStream2 = new ByteArrayOutputStream();
            DeflaterOutputStream zip2 = new DeflaterOutputStream(byteArrayStream2, -1);
            image.WriteTo(zip2);
            NUnit.Framework.Assert.IsTrue(byteArrayStream1.Length == byteArrayStream2.Length);
            zip.Dispose();
            zip2.Dispose();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE)]
        public virtual void TestFreeReference() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "freeReference.pdf", new WriterProperties
                ().SetFullCompressionMode(false));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "baseFreeReference.pdf"), writer);
            pdfDocument.GetPage(1).GetResources().GetPdfObject().GetAsArray(new PdfName("d")).Get(0).GetIndirectReference
                ().SetFree();
            PdfStream pdfStream = new PdfStream();
            pdfStream.SetData(new byte[] { 24, 23, 67 });
            pdfStream.MakeIndirect(pdfDocument);
            pdfDocument.GetPage(1).GetResources().GetPdfObject().GetAsArray(new PdfName("d")).Add(pdfStream);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "freeReference.pdf", 
                SOURCE_FOLDER + "cmp_freeReference.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FullCompressionAppendMode() {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "fullCompressionAppendMode.pdf", new 
                WriterProperties().SetFullCompressionMode(true).SetCompressionLevel(CompressionConstants.NO_COMPRESSION
                ));
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fullCompressionDoc.pdf"), writer, 
                new StampingProperties().UseAppendMode());
            PdfPage page = pdfDocument.GetPage(1);
            PdfStream contentStream = new PdfStream();
            String contentStr = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdfDocument.GetPage(1).GetFirstContentStream
                ().GetBytes(), System.Text.Encoding.ASCII);
            contentStream.SetData(contentStr.Replace("/F1 16", "/F1 24").GetBytes(System.Text.Encoding.ASCII));
            page.GetPdfObject().Put(PdfName.Contents, contentStream);
            page.SetModified();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "fullCompressionAppendMode.pdf"
                , SOURCE_FOLDER + "cmp_fullCompressionAppendMode.pdf", DESTINATION_FOLDER, "diff_"));
            PdfDocument assertDoc = new PdfDocument(CompareTool.CreateOutputReader(DESTINATION_FOLDER + "fullCompressionAppendMode.pdf"
                ));
            NUnit.Framework.Assert.IsTrue(assertDoc.GetPdfObject(9).IsStream());
            NUnit.Framework.Assert.AreEqual(1, ((PdfDictionary)assertDoc.GetPdfObject(9)).GetAsNumber(PdfName.N).IntValue
                ());
        }

        [NUnit.Framework.Test]
        public virtual void CheckAndResolveCircularReferences() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "datasheet.pdf"), CompareTool.CreateTestPdfWriter
                (DESTINATION_FOLDER + "datasheet_mode.pdf"));
            PdfDictionary pdfObject = (PdfDictionary)pdfDocument.GetPdfObject(53);
            pdfDocument.GetPage(1).GetResources().AddForm((PdfStream)pdfObject);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "datasheet_mode.pdf"
                , SOURCE_FOLDER + "cmp_datasheet_mode.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReadEncryptedDocumentWithFullCompression() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "source.pdf", new ReaderProperties().SetPassword("123".GetBytes
                ()));
            PdfDocument pdfDocument = new PdfDocument(reader);
            PdfDictionary form = pdfDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.AcroForm);
            PdfDictionary field = form.GetAsArray(PdfName.Fields).GetAsDictionary(0);
            NUnit.Framework.Assert.AreEqual("ch", field.GetAsString(PdfName.T).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("SomeStringValueInDictionary", field.GetAsDictionary(new PdfName("TestDic"
                )).GetAsString(new PdfName("TestString")).ToUnicodeString());
            NUnit.Framework.Assert.AreEqual("SomeStringValueInArray", field.GetAsArray(new PdfName("TestArray")).GetAsString
                (0).ToUnicodeString());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AddAssociatedFilesTest01() {
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "add_associated_files01.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            pdfDocument.AddAssociatedFile("af_1", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 1".
                GetBytes(), "af_1.txt", PdfName.Data));
            pdfDocument.AddNewPage();
            pdfDocument.GetFirstPage().AddAssociatedFile("af_2", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 2"
                .GetBytes(), "af_2.txt", PdfName.Data));
            PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
            root.AddAssociatedFile("af_3", PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 3".GetBytes
                (), "af_3.txt", PdfName.Data));
            PdfFileSpec af5 = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 5".GetBytes(), "af_5", 
                "af_5.txt", PdfName.Data);
            PdfTextAnnotation textannot = new PdfTextAnnotation(new Rectangle(100, 600, 50, 40));
            textannot.SetText(new PdfString("Text Annotation 01")).SetContents(new PdfString("Some contents..."));
            textannot.AddAssociatedFile(af5);
            pdfDocument.GetFirstPage().AddAnnotation(textannot);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "add_associated_files01.pdf"
                , SOURCE_FOLDER + "cmp_add_associated_files01.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddAssociatedFilesTest02() {
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + "add_associated_files02.pdf"
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            pdfDocument.SetTagged();
            PdfCanvas pageCanvas = new PdfCanvas(pdfDocument.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "berlin2013.jpg"
                ));
            imageXObject.AddAssociatedFile(PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 1".GetBytes
                (), "af_1.txt", PdfName.Data));
            pageCanvas.AddXObjectAt(imageXObject, 40, 400);
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(200, 200));
            PdfCanvas formCanvas = new PdfCanvas(formXObject, pdfDocument);
            formCanvas.SaveState().Circle(100, 100, 50).SetColor(ColorConstants.BLACK, true).Fill().RestoreState();
            formCanvas.Release();
            formXObject.AddAssociatedFile(PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "Associated File 2".GetBytes
                (), "af_2.txt", PdfName.Data));
            pageCanvas.AddXObjectAt(formXObject, 40, 100);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + "add_associated_files02.pdf"
                , SOURCE_FOLDER + "cmp_add_associated_files02.pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void IgnoreTagStructureTest() {
            String srcFile = SOURCE_FOLDER + "ignoreTagStructureTest.pdf";
            PdfDocument doNotIgnoreTagStructureDocument = new PdfDocument(new PdfReader(srcFile));
            PdfDocumentTest.IgnoreTagStructurePdfDocument ignoreTagStructureDocument = new PdfDocumentTest.IgnoreTagStructurePdfDocument
                (new PdfReader(srcFile));
            NUnit.Framework.Assert.IsTrue(doNotIgnoreTagStructureDocument.IsTagged());
            NUnit.Framework.Assert.IsFalse(ignoreTagStructureDocument.IsTagged());
            doNotIgnoreTagStructureDocument.Close();
            ignoreTagStructureDocument.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OUTLINE_DESTINATION_PAGE_NUMBER_IS_OUT_OF_BOUNDS, LogLevel = 
            LogLevelConstants.WARN)]
        public virtual void RemovePageWithInvalidOutlineTest() {
            String source = SOURCE_FOLDER + "invalid_outline.pdf";
            String destination = DESTINATION_FOLDER + "invalid_outline.pdf";
            String cmp = SOURCE_FOLDER + "cmp_invalid_outline.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(new FileStream(source, FileMode.Open, FileAccess.Read
                )), CompareTool.CreateTestPdfWriter(destination));
            document.RemovePage(4);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destination, cmp, DESTINATION_FOLDER, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_VERSION_IN_CATALOG_CORRUPTED, LogLevel = LogLevelConstants
            .ERROR)]
        public virtual void OpenDocumentWithInvalidCatalogVersionTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "sample-with-invalid-catalog-version.pdf")) {
                using (PdfDocument pdfDocument = new PdfDocument(reader)) {
                    NUnit.Framework.Assert.IsNotNull(pdfDocument);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void OpenDocumentWithInvalidCatalogVersionAndConservativeStrictnessReadingTest() {
            using (PdfReader reader = new PdfReader(SOURCE_FOLDER + "sample-with-invalid-catalog-version.pdf").SetStrictnessLevel
                (PdfReader.StrictnessLevel.CONSERVATIVE)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_VERSION_IN_CATALOG_CORRUPTED, 
                    e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WidgetDaEntryRemovePageTest() {
            String testName = "widgetDaEntryRemovePage.pdf";
            String outPdf = DESTINATION_FOLDER + testName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "widgetWithDaEntry.pdf"), CompareTool
                .CreateTestPdfWriter(outPdf))) {
                pdfDocument.RemovePage(3);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + testName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MergedAndSimpleWidgetsRemovePageTest() {
            String testName = "mergedAndSimpleWidgetsRemovePage.pdf";
            String outPdf = DESTINATION_FOLDER + testName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "mergedAndSimpleWidgets.pdf"
                ), CompareTool.CreateTestPdfWriter(outPdf))) {
                pdfDocument.RemovePage(1);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + testName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MergedSiblingWidgetsRemovePageTest() {
            String testName = "mergedSiblingWidgetsRemovePage.pdf";
            String outPdf = DESTINATION_FOLDER + testName;
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "mergedSiblingWidgets.pdf")
                , CompareTool.CreateTestPdfWriter(outPdf))) {
                pdfDocument.RemovePage(2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_" + testName
                , DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void RootCannotBeReferenceFromTrailerTest() {
            String filename = SOURCE_FOLDER + "rootCannotBeReferenceFromTrailerTest.pdf";
            PdfReader corruptedReader = new PdfReader(filename);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(corruptedReader));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CORRUPTED_ROOT_ENTRY_IN_TRAILER, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SetSerializeOptionsTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            SerializeOptions options = new SerializeOptions().SetUseCanonicalFormat(true);
            document.SetSerializeOptions(options);
            NUnit.Framework.Assert.AreEqual(options, document.GetSerializeOptions());
        }

        [NUnit.Framework.Test]
        public virtual void GetDiContainer() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            NUnit.Framework.Assert.IsNotNull(document.GetDiContainer());
        }

        private class IgnoreTagStructurePdfDocument : PdfDocument {
            internal IgnoreTagStructurePdfDocument(PdfReader reader)
                : base(reader) {
            }

            protected internal override void TryInitTagStructure(PdfDictionary str) {
            }
        }
    }
}
