/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfWriterTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocument() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "emptyDocument.pdf"
                ));
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "emptyDocument.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
            String date = pdfDocument.GetDocumentInfo().GetPdfObject().GetAsString(PdfName.CreationDate).GetValue();
            DateTime cl = PdfDate.Decode(date);
            double diff = DateTimeUtil.GetUtcMillisFromEpoch(null) - DateTimeUtil.GetUtcMillisFromEpoch(cl);
            String message = "Unexpected creation date. Different from now is " + (float)diff / 1000 + "s";
            NUnit.Framework.Assert.IsTrue(diff < 5000, message);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void UseObjectForMultipleTimes1() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "useObjectForMultipleTimes1.pdf"
                ));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.Flush();
            pdfDoc.GetCatalog().GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            pdfDoc.Close();
            ValidateUseObjectForMultipleTimesTest(destinationFolder + "useObjectForMultipleTimes1.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void UseObjectForMultipleTimes2() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "useObjectForMultipleTimes2.pdf"
                ));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            helloWorld.Flush();
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.Flush();
            pdfDoc.GetCatalog().GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            pdfDoc.Close();
            ValidateUseObjectForMultipleTimesTest(destinationFolder + "useObjectForMultipleTimes2.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void UseObjectForMultipleTimes3() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "useObjectForMultipleTimes3.pdf"
                ));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.Flush();
            helloWorld.Flush();
            pdfDoc.GetCatalog().GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            pdfDoc.Close();
            ValidateUseObjectForMultipleTimesTest(destinationFolder + "useObjectForMultipleTimes3.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void UseObjectForMultipleTimes4() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "useObjectForMultipleTimes4.pdf"
                ));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.Flush();
            pdfDoc.GetCatalog().GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            helloWorld.Flush();
            pdfDoc.Close();
            ValidateUseObjectForMultipleTimesTest(destinationFolder + "useObjectForMultipleTimes4.pdf");
        }

        private void ValidateUseObjectForMultipleTimesTest(String filename) {
            PdfReader reader = CompareTool.CreateOutputReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary page = pdfDoc.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(page);
            PdfDictionary helloWorld = page.GetAsDictionary(new PdfName("HelloWorld"));
            NUnit.Framework.Assert.IsNotNull(helloWorld);
            PdfString world = helloWorld.GetAsString(new PdfName("Hello"));
            NUnit.Framework.Assert.AreEqual("World", world.ToString());
            helloWorld = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(new PdfName("HelloWorld"));
            NUnit.Framework.Assert.IsNotNull(helloWorld);
            world = helloWorld.GetAsString(new PdfName("Hello"));
            NUnit.Framework.Assert.AreEqual("World", world.ToString());
            pdfDoc.Close();
        }

        /// <summary>Copying direct objects.</summary>
        /// <remarks>Copying direct objects. Objects of all types are added into document catalog.</remarks>
        [NUnit.Framework.Test]
        public virtual void CopyObject1() {
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject1_1.pdf"
                ));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.Flush();
            PdfDictionary catalog1 = pdfDoc1.GetCatalog().GetPdfObject();
            PdfArray aDirect = new PdfArray();
            List<PdfObject> tmpArray = new List<PdfObject>(2);
            tmpArray.Add(new PdfNumber(1));
            tmpArray.Add(new PdfNumber(2));
            aDirect.Add(new PdfArray(tmpArray));
            aDirect.Add(new PdfBoolean(true));
            SortedDictionary<PdfName, PdfObject> tmpMap = new SortedDictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("one"), new PdfNumber(1));
            tmpMap.Put(new PdfName("two"), new PdfNumber(2));
            aDirect.Add(new PdfDictionary(tmpMap));
            aDirect.Add(new PdfName("name"));
            aDirect.Add(new PdfNull());
            aDirect.Add(new PdfNumber(100));
            aDirect.Add(new PdfString("string"));
            catalog1.Put(new PdfName("aDirect"), aDirect);
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject1_2.pdf"
                ));
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.Flush();
            PdfDictionary catalog2 = pdfDoc2.GetCatalog().GetPdfObject();
            catalog2.Put(new PdfName("aDirect"), aDirect.CopyTo(pdfDoc2));
            pdfDoc1.Close();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject1_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            PdfArray a = (PdfArray)catalog.Get(new PdfName("aDirect"));
            NUnit.Framework.Assert.IsNotNull(a);
            NUnit.Framework.Assert.AreEqual(1, ((PdfNumber)((PdfArray)a.Get(0)).Get(0)).IntValue());
            NUnit.Framework.Assert.AreEqual(2, ((PdfNumber)((PdfArray)a.Get(0)).Get(1)).IntValue());
            NUnit.Framework.Assert.AreEqual(true, ((PdfBoolean)a.Get(1)).GetValue());
            NUnit.Framework.Assert.AreEqual(1, ((PdfNumber)((PdfDictionary)a.Get(2)).Get(new PdfName("one"))).IntValue
                ());
            NUnit.Framework.Assert.AreEqual(2, ((PdfNumber)((PdfDictionary)a.Get(2)).Get(new PdfName("two"))).IntValue
                ());
            NUnit.Framework.Assert.AreEqual(new PdfName("name"), a.Get(3));
            NUnit.Framework.Assert.IsTrue(a.Get(4).IsNull());
            NUnit.Framework.Assert.AreEqual(100, ((PdfNumber)a.Get(5)).IntValue());
            NUnit.Framework.Assert.AreEqual("string", ((PdfString)a.Get(6)).ToUnicodeString());
            pdfDocument.Close();
        }

        /// <summary>Copying objects, some of those are indirect.</summary>
        /// <remarks>Copying objects, some of those are indirect. Objects of all types are added into document catalog.
        ///     </remarks>
        [NUnit.Framework.Test]
        public virtual void CopyObject2() {
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject2_1.pdf"
                ));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.Flush();
            PdfDictionary catalog1 = pdfDoc1.GetCatalog().GetPdfObject();
            PdfName aDirectName = new PdfName("aDirect");
            PdfArray aDirect = (PdfArray)new PdfArray().MakeIndirect(pdfDoc1);
            List<PdfObject> tmpArray = new List<PdfObject>(2);
            tmpArray.Add(new PdfNumber(1));
            tmpArray.Add(new PdfNumber(2).MakeIndirect(pdfDoc1));
            aDirect.Add(new PdfArray(tmpArray));
            aDirect.Add(new PdfBoolean(true));
            SortedDictionary<PdfName, PdfObject> tmpMap = new SortedDictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("one"), new PdfNumber(1));
            tmpMap.Put(new PdfName("two"), new PdfNumber(2).MakeIndirect(pdfDoc1));
            aDirect.Add(new PdfDictionary(tmpMap));
            aDirect.Add(new PdfName("name"));
            aDirect.Add(new PdfNull().MakeIndirect(pdfDoc1));
            aDirect.Add(new PdfNumber(100));
            aDirect.Add(new PdfString("string"));
            catalog1.Put(aDirectName, aDirect);
            pdfDoc1.Close();
            PdfDocument pdfDoc1R = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "copyObject2_1.pdf"
                ));
            aDirect = (PdfArray)pdfDoc1R.GetCatalog().GetPdfObject().Get(aDirectName);
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject2_2.pdf"
                ));
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.Flush();
            PdfDictionary catalog2 = pdfDoc2.GetCatalog().GetPdfObject();
            catalog2.Put(aDirectName, aDirect.CopyTo(pdfDoc2));
            pdfDoc1R.Close();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject2_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            PdfArray a = catalog.GetAsArray(new PdfName("aDirect"));
            NUnit.Framework.Assert.IsNotNull(a);
            NUnit.Framework.Assert.AreEqual(1, ((PdfNumber)((PdfArray)a.Get(0)).Get(0)).IntValue());
            NUnit.Framework.Assert.AreEqual(2, ((PdfArray)a.Get(0)).GetAsNumber(1).IntValue());
            NUnit.Framework.Assert.AreEqual(true, ((PdfBoolean)a.Get(1)).GetValue());
            NUnit.Framework.Assert.AreEqual(1, ((PdfNumber)((PdfDictionary)a.Get(2)).Get(new PdfName("one"))).IntValue
                ());
            NUnit.Framework.Assert.AreEqual(2, ((PdfDictionary)a.Get(2)).GetAsNumber(new PdfName("two")).IntValue());
            NUnit.Framework.Assert.AreEqual(new PdfName("name"), a.Get(3));
            NUnit.Framework.Assert.IsTrue(a.Get(4).IsNull());
            NUnit.Framework.Assert.AreEqual(100, ((PdfNumber)a.Get(5)).IntValue());
            NUnit.Framework.Assert.AreEqual("string", ((PdfString)a.Get(6)).ToUnicodeString());
            pdfDocument.Close();
        }

        /// <summary>Copy objects recursively.</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject3() {
 {
                PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject3_1.pdf"
                    ));
                PdfPage page1 = pdfDoc1.AddNewPage();
                page1.Flush();
                PdfDictionary catalog1 = pdfDoc1.GetCatalog().GetPdfObject();
                PdfArray arr1 = (PdfArray)new PdfArray().MakeIndirect(pdfDoc1);
                PdfArray arr2 = (PdfArray)new PdfArray().MakeIndirect(pdfDoc1);
                arr1.Add(arr2);
                PdfDictionary dic1 = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc1);
                arr2.Add(dic1);
                PdfDictionary dic2 = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc1);
                dic1.Put(new PdfName("dic2"), dic2);
                PdfName arr1Name = new PdfName("arr1");
                dic2.Put(arr1Name, arr1);
                catalog1.Put(arr1Name, arr1);
                pdfDoc1.Close();
                PdfDocument pdfDoc1R = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "copyObject3_1.pdf"
                    ));
                arr1 = (PdfArray)pdfDoc1R.GetCatalog().GetPdfObject().Get(arr1Name);
                PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject3_2.pdf"
                    ));
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.Flush();
                PdfDictionary catalog2 = pdfDoc2.GetCatalog().GetPdfObject();
                catalog2.Put(arr1Name, arr1.CopyTo(pdfDoc2));
                pdfDoc1R.Close();
                pdfDoc2.Close();
            }
 {
                PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject3_2.pdf");
                PdfDocument pdfDocument = new PdfDocument(reader);
                NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
                PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
                PdfArray arr1 = catalog.GetAsArray(new PdfName("arr1"));
                PdfArray arr2 = arr1.GetAsArray(0);
                PdfDictionary dic1 = arr2.GetAsDictionary(0);
                PdfDictionary dic2 = dic1.GetAsDictionary(new PdfName("dic2"));
                NUnit.Framework.Assert.AreEqual(arr1, dic2.GetAsArray(new PdfName("arr1")));
                pdfDocument.Close();
            }
        }

        /// <summary>Copies stream.</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject4() {
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject4_1.pdf"
                ));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.Flush();
            PdfDictionary catalog1 = pdfDoc1.GetCatalog().GetPdfObject();
            PdfStream stream1 = (PdfStream)new PdfStream().MakeIndirect(pdfDoc1);
            List<PdfObject> tmpArray = new List<PdfObject>(3);
            tmpArray.Add(new PdfNumber(1));
            tmpArray.Add(new PdfNumber(2));
            tmpArray.Add(new PdfNumber(3));
            stream1.GetOutputStream().Write(new PdfArray(tmpArray));
            catalog1.Put(new PdfName("stream"), stream1);
            pdfDoc1.Close();
            PdfDocument pdfDoc1R = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "copyObject4_1.pdf"
                ));
            stream1 = (PdfStream)pdfDoc1R.GetCatalog().GetPdfObject().Get(new PdfName("stream"));
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject4_2.pdf"
                ));
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.Flush();
            PdfDictionary catalog2 = pdfDoc2.GetCatalog().GetPdfObject();
            catalog2.Put(new PdfName("stream"), stream1.CopyTo(pdfDoc2));
            pdfDoc1R.Close();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject4_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary catalog = pdfDocument.GetCatalog().GetPdfObject();
            PdfStream stream = (PdfStream)catalog.GetAsStream(new PdfName("stream"));
            byte[] bytes = stream.GetBytes();
            NUnit.Framework.Assert.AreEqual(ByteUtils.GetIsoBytes("[1 2 3]"), bytes);
            pdfDocument.Close();
        }

        /// <summary>Copies page.</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject5() {
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject5_1.pdf"
                ));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Page_1"));
            page1.Flush();
            pdfDoc1.Close();
            PdfDocument pdfDoc1R = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "copyObject5_1.pdf"
                ));
            page1 = pdfDoc1R.GetPage(1);
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject5_2.pdf"
                ));
            PdfPage page2 = page1.CopyTo(pdfDoc2);
            pdfDoc2.AddPage(page2);
            page2.Flush();
            page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Page_2"));
            page2.Flush();
            pdfDoc1R.Close();
            pdfDoc2.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject5_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(8, reader.trailer.GetAsNumber(PdfName.Size).IntValue());
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            // getting content bytes results in adding '\n' for each content stream
            // so we should compare String with '\n' at the end
            NUnit.Framework.Assert.AreEqual(ByteUtils.GetIsoBytes("%Page_1\n"), bytes);
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual(ByteUtils.GetIsoBytes("%Page_2\n"), bytes);
            pdfDocument.Close();
        }

        /// <summary>Copies object with different method overloads.</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject6() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject6_1.pdf"
                ));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            pdfDoc.Close();
            pdfDoc = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + "copyObject6_1.pdf"));
            helloWorld = (PdfDictionary)pdfDoc.GetPage(1).GetPdfObject().Get(new PdfName("HelloWorld"));
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject6_2.pdf"
                ));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetPdfObject().Put(new PdfName("HelloWorldCopy1"), helloWorld.CopyTo(pdfDoc1));
            page1.GetPdfObject().Put(new PdfName("HelloWorldCopy2"), helloWorld.CopyTo(pdfDoc1, true));
            page1.GetPdfObject().Put(new PdfName("HelloWorldCopy3"), helloWorld.CopyTo(pdfDoc1, false));
            page1.Flush();
            pdfDoc.Close();
            pdfDoc1.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "copyObject6_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfObject obj1 = pdfDocument.GetPage(1).GetPdfObject().Get(new PdfName("HelloWorldCopy1"));
            PdfIndirectReference ref1 = obj1.GetIndirectReference();
            NUnit.Framework.Assert.AreEqual(6, ref1.objNr);
            NUnit.Framework.Assert.AreEqual(0, ref1.genNr);
            PdfObject obj2 = pdfDocument.GetPage(1).GetPdfObject().Get(new PdfName("HelloWorldCopy2"));
            PdfIndirectReference ref2 = obj2.GetIndirectReference();
            NUnit.Framework.Assert.AreEqual(7, ref2.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, ref2.GetGenNumber());
            PdfObject obj3 = pdfDocument.GetPage(1).GetPdfObject().Get(new PdfName("HelloWorldCopy3"));
            PdfIndirectReference ref3 = obj3.GetIndirectReference();
            NUnit.Framework.Assert.AreEqual(7, ref3.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, ref3.GetGenNumber());
            pdfDocument.Close();
        }

        /// <summary>Attempts to copy from the document that is being written.</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject7() {
            String exceptionMessage = null;
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject6_1.pdf"
                ));
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject6_2.pdf"
                ));
            try {
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfDictionary directDict = new PdfDictionary();
                PdfObject indirectDict = new PdfDictionary().MakeIndirect(pdfDoc1);
                page1.GetPdfObject().Put(new PdfName("HelloWorldDirect"), directDict);
                page1.GetPdfObject().Put(new PdfName("HelloWorldIndirect"), indirectDict);
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.GetPdfObject().Put(new PdfName("HelloWorldDirect"), directDict.CopyTo(pdfDoc2));
                page2.GetPdfObject().Put(new PdfName("HelloWorldIndirect"), indirectDict.CopyTo(pdfDoc2));
            }
            catch (PdfException ex) {
                exceptionMessage = ex.Message;
            }
            finally {
                pdfDoc1.Close();
                pdfDoc2.Close();
            }
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_COPY_INDIRECT_OBJECT_FROM_THE_DOCUMENT_THAT_IS_BEING_WRITTEN
                , exceptionMessage);
        }

        /// <summary>Attempts to copy to copy with null document</summary>
        [NUnit.Framework.Test]
        public virtual void CopyObject8() {
            String exceptionMessage = null;
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "copyObject6_1.pdf"
                ));
            try {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfDictionary directDict = new PdfDictionary();
                PdfObject indirectDict = new PdfDictionary().MakeIndirect(pdfDoc);
                page1.GetPdfObject().Put(new PdfName("HelloWorldDirect"), directDict);
                page1.GetPdfObject().Put(new PdfName("HelloWorldIndirect"), indirectDict);
                indirectDict.CopyTo(null);
            }
            catch (PdfException ex) {
                exceptionMessage = ex.Message;
            }
            finally {
                pdfDoc.Close();
            }
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_FOR_COPY_TO_CANNOT_BE_NULL, exceptionMessage
                );
        }

        [NUnit.Framework.Test]
        public virtual void CloseStream1() {
            Stream fos = FileUtil.GetFileOutputStream(destinationFolder + "closeStream1.pdf");
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            try {
                fos.Write(1);
                NUnit.Framework.Assert.Fail("Exception expected");
            }
            catch (Exception) {
            }
        }

        //ignored
        [NUnit.Framework.Test]
        public virtual void CloseStream2() {
            Stream fos = FileUtil.GetFileOutputStream(destinationFolder + "closeStream2.pdf");
            PdfWriter writer = new PdfWriter(fos);
            writer.SetCloseStream(false);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            fos.Write(1);
        }

        [NUnit.Framework.Test]
        public virtual void DirectInIndirectChain() {
            String filename = destinationFolder + "directInIndirectChain.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            PdfArray level1 = new PdfArray();
            level1.Add(new PdfNumber(1).MakeIndirect(pdfDoc));
            PdfDictionary level2 = new PdfDictionary();
            level1.Add(level2);
            PdfArray level3 = new PdfArray();
            level2.Put(new PdfName("level3"), level3);
            level2.Put(new PdfName("num"), new PdfNumber(2).MakeIndirect(pdfDoc));
            level3.Add(new PdfNumber(3).MakeIndirect(pdfDoc));
            level3.Add(new PdfNumber(3).MakeIndirect(pdfDoc));
            PdfDictionary level4 = new PdfDictionary();
            level4.Put(new PdfName("num"), new PdfNumber(4).MakeIndirect(pdfDoc));
            level3.Add(level4);
            PdfPage page1 = pdfDoc.AddNewPage();
            page1.GetPdfObject().Put(new PdfName("test"), level1);
            pdfDoc.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfStreamByInputStream() {
            String filename = destinationFolder + "createPdfStreamByInputStream.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(filename));
            document.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            PdfPage page = document.AddNewPage();
            page.Flush();
            String streamContent = "Some text content with strange symbols ∞²";
            PdfStream stream = new PdfStream(document, new MemoryStream(streamContent.GetBytes()));
            stream.Flush();
            int streamIndirectNumber = stream.GetIndirectReference().GetObjNumber();
            document.Close();
            //        com.itextpdf.text.pdf.PdfReader reader = new PdfReader(filename);
            //        Assert.assertEquals("Rebuilt", false, reader.isRebuilt());
            //        Assert.assertNotNull(reader.getPageN(1));
            //        String date = reader.getDocumentInfo().get("CreationDate");
            //        Calendar cl = com.itextpdf.text.pdf.PdfDate.decode(date);
            //        long diff = new GregorianCalendar().getTimeInMillis() - cl.getTimeInMillis();
            //        String message = "Unexpected creation date. Different from now is " + (float)diff/1000 + "s";
            //        Assert.assertTrue(message, diff < 5000);
            //        reader.close();
            PdfReader reader6 = CompareTool.CreateOutputReader(filename);
            document = new PdfDocument(reader6);
            NUnit.Framework.Assert.AreEqual(false, reader6.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader6.HasFixedXref(), "Fixed");
            PdfStream pdfStream = (PdfStream)document.GetXref().Get(streamIndirectNumber).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(streamContent.GetBytes(), pdfStream.GetBytes(), "Stream by InputStream");
            document.Close();
        }
    }
}
