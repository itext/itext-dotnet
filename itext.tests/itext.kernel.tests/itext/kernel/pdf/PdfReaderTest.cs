/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfReaderTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfReaderTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfReaderTest/";

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 6";

        internal const String title = "Empty iText 6 Document";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void OpenSimpleDoc() {
            String filename = DESTINATION_FOLDER + "openSimpleDoc.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            pdfDoc.AddNewPage();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            pdfDoc = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(author, pdfDoc.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(creator, pdfDoc.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.AreEqual(title, pdfDoc.GetDocumentInfo().GetTitle());
            PdfObject @object = pdfDoc.GetPdfObject(1);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Catalog));
            @object = pdfDoc.GetPdfObject(2);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Pages));
            @object = pdfDoc.GetPdfObject(3);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            @object = pdfDoc.GetPdfObject(4);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, pdfDoc.GetPdfObject(5).GetObjectType());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void OpenSimpleDocWithFullCompression() {
            String filename = SOURCE_FOLDER + "simpleCanvasWithFullCompression.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfObject @object = pdfDoc.GetPdfObject(1);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Catalog));
            @object = pdfDoc.GetPdfObject(2);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Pages));
            @object = pdfDoc.GetPdfObject(3);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            @object = pdfDoc.GetPdfObject(4);
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Page));
            @object = pdfDoc.GetPdfObject(5);
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, @object.GetObjectType());
            String content = "100 100 100 100 re\nf\n";
            NUnit.Framework.Assert.AreEqual(ByteUtils.GetIsoBytes(content), ((PdfStream)@object).GetBytes());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            reader.Close();
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ObjectStreamIncrementalUpdateReading() {
            /*
            This test ensures that if certain object stored in objects streams
            has incremental updates, the right object instance is found and initialized
            even if the object stream with the older object's increment is read as well.
            
            One peculiar thing covered by this test is that older object increment contains
            indirect refernce to the object number 8 which is freed in document incremental
            update. Such document and particulary this object is perfectly valid.
            */
            String filename = SOURCE_FOLDER + "objectStreamIncrementalUpdate.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDictionary catalogDict = pdfDoc.GetCatalog().GetPdfObject();
            PdfDictionary customDict1 = catalogDict.GetAsDictionary(new PdfName("CustomDict1"));
            PdfDictionary customDict2 = catalogDict.GetAsDictionary(new PdfName("CustomDict2"));
            NUnit.Framework.Assert.AreEqual(1, customDict1.Size());
            NUnit.Framework.Assert.AreEqual(1, customDict2.Size());
            NUnit.Framework.Assert.AreEqual("Hello world updated.", customDict1.GetAsString(new PdfName("Key1")).GetValue
                ());
            NUnit.Framework.Assert.AreEqual("Hello world for second dictionary.", customDict2.GetAsString(new PdfName(
                "Key1")).GetValue());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RereadReleasedObjectFromObjectStream() {
            String filename = SOURCE_FOLDER + "twoCustomDictionariesInObjectStream.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDictionary catalogDict = pdfDoc.GetCatalog().GetPdfObject();
            PdfDictionary customDict1 = catalogDict.GetAsDictionary(new PdfName("CustomDict1"));
            PdfDictionary customDict2 = catalogDict.GetAsDictionary(new PdfName("CustomDict2"));
            NUnit.Framework.Assert.IsTrue(customDict1.ContainsKey(new PdfName("CustomDict1Key1")));
            NUnit.Framework.Assert.IsTrue(customDict2.ContainsKey(new PdfName("CustomDict2Key1")));
            customDict2.Clear();
            customDict1.Release();
            // reread released dictionary and also modified dictionary
            customDict1 = catalogDict.GetAsDictionary(new PdfName("CustomDict1"));
            customDict2 = catalogDict.GetAsDictionary(new PdfName("CustomDict2"));
            NUnit.Framework.Assert.IsTrue(customDict1.ContainsKey(new PdfName("CustomDict1Key1")));
            NUnit.Framework.Assert.IsFalse(customDict2.ContainsKey(new PdfName("CustomDict2Key1")));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void OpenDocWithFlateFilter() {
            String filename = SOURCE_FOLDER + "100PagesDocumentWithFlateFilter.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(100, document.GetNumberOfPages(), "Page count");
            String contentTemplate = "q\n" + "BT\n" + "36 700 Td\n" + "/F1 72 Tf\n" + "({0})Tj\n" + "ET\n" + "Q\n" + "100 500 100 100 re\n"
                 + "f\n";
            for (int i = 1; i <= document.GetNumberOfPages(); i++) {
                PdfPage page = document.GetPage(i);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(contentTemplate, i), iText.Commons.Utils.JavaUtil.GetStringForBytes
                    (content), "Page content " + i);
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesRead() {
            String filename = DESTINATION_FOLDER + "primitivesRead.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(filename));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            catalog.Put(new PdfName("a"), new PdfBoolean(true).MakeIndirect(document));
            document.Close();
            PdfReader reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            PdfObject @object = document.GetXref().Get(1).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Catalog));
            @object = document.GetXref().Get(2).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Pages));
            @object = document.GetXref().Get(3).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            @object = document.GetXref().Get(4).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, document.GetXref().Get(5).GetRefersTo().GetObjectType());
            @object = document.GetXref().Get(6).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.BOOLEAN, @object.GetObjectType());
            NUnit.Framework.Assert.IsNotNull(@object.GetIndirectReference());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain1() {
            String filename = DESTINATION_FOLDER + "indirectsChain1.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(filename));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfObject pdfObject = GetTestPdfDictionary();
            for (int i = 0; i < 5; i++) {
                pdfObject = pdfObject.MakeIndirect(document).GetIndirectReference();
            }
            catalog.Put(new PdfName("a"), pdfObject);
            document.Close();
            PdfReader reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            pdfObject = document.GetXref().Get(1).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Catalog));
            pdfObject = document.GetXref().Get(2).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Pages));
            pdfObject = document.GetXref().Get(3).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            pdfObject = document.GetXref().Get(4).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, document.GetXref().Get(5).GetRefersTo().GetObjectType());
            for (int i = 6; i < document.GetXref().Size(); i++) {
                NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, document.GetXref().Get(i).GetRefersTo().GetObjectType
                    ());
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain2() {
            String filename = DESTINATION_FOLDER + "indirectsChain2.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(filename));
            document.AddNewPage();
            PdfDictionary catalog = document.GetCatalog().GetPdfObject();
            PdfObject pdfObject = GetTestPdfDictionary();
            for (int i = 0; i < 100; i++) {
                pdfObject = pdfObject.MakeIndirect(document).GetIndirectReference();
            }
            catalog.Put(new PdfName("a"), pdfObject);
            document.Close();
            PdfReader reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            pdfObject = document.GetXref().Get(1).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Catalog));
            pdfObject = document.GetXref().Get(2).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Pages));
            pdfObject = document.GetXref().Get(3).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            pdfObject = document.GetXref().Get(4).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, pdfObject.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(pdfObject, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, document.GetXref().Get(5).GetRefersTo().GetObjectType());
            for (int i = 6; i < 6 + 32; i++) {
                NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, document.GetXref().Get(6).GetRefersTo().GetObjectType
                    ());
            }
            for (int i = 6 + 32; i < document.GetXref().Size(); i++) {
                NUnit.Framework.Assert.AreEqual(PdfObject.INDIRECT_REFERENCE, document.GetXref().Get(i).GetRefersTo().GetObjectType
                    ());
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IndirectsChain3() {
            String filename = SOURCE_FOLDER + "indirectsChain3.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            PdfObject @object = document.GetXref().Get(1).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Catalog));
            @object = document.GetXref().Get(2).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Pages));
            @object = document.GetXref().Get(3).GetRefersTo();
            NUnit.Framework.Assert.IsTrue(@object.GetObjectType() == PdfObject.DICTIONARY);
            @object = document.GetXref().Get(4).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, document.GetXref().Get(5).GetRefersTo().GetObjectType());
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, document.GetXref().Get(6).GetRefersTo().GetObjectType
                ());
            for (int i = 7; i < document.GetXref().Size(); i++) {
                NUnit.Framework.Assert.AreEqual(PdfObject.INDIRECT_REFERENCE, document.GetXref().Get(i).GetRefersTo().GetObjectType
                    ());
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void InvalidIndirect() {
            String filename = SOURCE_FOLDER + "invalidIndirect.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            PdfObject @object = document.GetXref().Get(1).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Catalog));
            @object = document.GetXref().Get(2).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Pages));
            @object = document.GetXref().Get(3).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            @object = document.GetXref().Get(4).GetRefersTo();
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, @object.GetObjectType());
            NUnit.Framework.Assert.IsTrue(ObjectTypeEqualTo(@object, PdfName.Page));
            NUnit.Framework.Assert.AreEqual(PdfObject.STREAM, document.GetXref().Get(5).GetRefersTo().GetObjectType());
            NUnit.Framework.Assert.AreEqual(PdfObject.DICTIONARY, document.GetXref().Get(6).GetRefersTo().GetObjectType
                ());
            for (int i = 7; i < document.GetXref().Size(); i++) {
                NUnit.Framework.Assert.IsNull(document.GetXref().Get(i).GetRefersTo());
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest01() {
            String filename = SOURCE_FOLDER + "1000PagesDocument.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            PdfDocument document = new PdfDocument(reader, writer);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            PdfPage testPage = document.GetPage(1000);
            int testXref = testPage.GetPdfObject().GetIndirectReference().GetObjNumber();
            document.MovePage(1000, 1000);
            NUnit.Framework.Assert.AreEqual(testXref, testPage.GetPdfObject().GetIndirectReference().GetObjNumber());
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            reader.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest02() {
            String filename = SOURCE_FOLDER + "1000PagesDocumentWithFullCompression.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest03() {
            String filename = SOURCE_FOLDER + "10PagesDocumentWithLeafs.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest04() {
            String filename = SOURCE_FOLDER + "PagesDocument.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(3, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(pageNum + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest05() {
            String filename = SOURCE_FOLDER + "PagesDocument05.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(3, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(pageNum + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest06() {
            String filename = SOURCE_FOLDER + "PagesDocument06.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(2, pageCount);
            PdfPage page = document.GetPage(1);
            String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            page = document.GetPage(2);
            content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("300"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            page = document.GetPage(2);
            document.RemovePage(page);
            content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("300"));
            page = document.GetPage(1);
            document.RemovePage(1);
            content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest07() {
            String filename = SOURCE_FOLDER + "PagesDocument07.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(2, pageCount);
            bool exception = false;
            try {
                document.GetPage(1);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest08() {
            String filename = SOURCE_FOLDER + "PagesDocument08.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            bool exception = false;
            try {
                document.GetPage(1);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest09() {
            String filename = SOURCE_FOLDER + "PagesDocument09.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            PdfPage page = document.GetPage(1);
            String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            page = document.GetPage(1);
            document.RemovePage(1);
            content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest10() {
            String filename = SOURCE_FOLDER + "1000PagesDocumentWithFullCompression.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            Random rnd = new Random();
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                int pageNum = rnd.Next(document.GetNumberOfPages()) + 1;
                PdfPage page = document.GetPage(pageNum);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            IList<int> pageNums = new List<int>(1000);
            for (int i = 0; i < 1000; i++) {
                pageNums.Add(i + 1);
            }
            for (int i = 1; i < pageCount + 1; i++) {
                int index = rnd.Next(document.GetNumberOfPages()) + 1;
                int pageNum = (int)pageNums.JRemoveAt(index - 1);
                PdfPage page = document.GetPage(index);
                document.RemovePage(index);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest11() {
            String filename = SOURCE_FOLDER + "hello.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            try {
                document.GetPage(-30);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                    , -30), e.Message);
            }
            try {
                document.GetPage(0);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                    , 0), e.Message);
            }
            document.GetPage(1);
            try {
                document.GetPage(25);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                    , 25), e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void CorrectSimpleDoc1() {
            String filename = SOURCE_FOLDER + "correctSimpleDoc1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.IsNotNull(page.GetContentStream(0).GetBytes());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CorrectSimpleDoc2() {
            String filename = SOURCE_FOLDER + "correctSimpleDoc2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasFixedXref(), "Need fixXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.IsNotNull(page.GetContentStream(0).GetBytes());
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void CorrectSimpleDoc3() {
            String filename = SOURCE_FOLDER + "correctSimpleDoc3.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.IsNotNull(page.GetContentStream(0).GetBytes());
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void CorrectSimpleDoc4() {
            String filename = SOURCE_FOLDER + "correctSimpleDoc4.pdf";
            PdfReader reader = new PdfReader(filename);
            try {
                //NOTE test with abnormal object declaration that iText can't resolve.
                PdfDocument document = new PdfDocument(reader);
                NUnit.Framework.Assert.Fail("Expect exception");
            }
            catch (PdfException e) {
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_PAGE_STRUCTURE_PAGES_MUST_BE_PDF_DICTIONARY
                    , e.Message);
            }
            finally {
                reader.Close();
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest01() {
            String filename = SOURCE_FOLDER + "OnlyTrailer.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest02() {
            String filename = SOURCE_FOLDER + "CompressionShift1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest03() {
            String filename = SOURCE_FOLDER + "CompressionShift2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest04() {
            String filename = SOURCE_FOLDER + "CompressionWrongObjStm.pdf";
            PdfReader reader = new PdfReader(filename);
            bool exception = false;
            try {
                new PdfDocument(reader);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            reader.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest05() {
            String filename = SOURCE_FOLDER + "CompressionWrongShift.pdf";
            PdfReader reader = new PdfReader(filename);
            bool exception = false;
            try {
                new PdfDocument(reader);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest06() {
            String filename = SOURCE_FOLDER + "InvalidOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasFixedXref(), "Need fixXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 2)]
        public virtual void FixPdfTest07() {
            String filename = SOURCE_FOLDER + "XRefSectionWithFreeReferences1.pdf";
            PdfReader reader = new PdfReader(filename);
            bool exception = false;
            try {
                new PdfDocument(reader);
            }
            catch (InvalidCastException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            reader.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest08() {
            String filename = SOURCE_FOLDER + "XRefSectionWithFreeReferences2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.AreEqual(author, document.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(creator, document.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.AreEqual(title, document.GetDocumentInfo().GetTitle());
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest09() {
            String filename = SOURCE_FOLDER + "XRefSectionWithFreeReferences3.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.AreEqual(author, document.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(creator, document.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.AreEqual(title, document.GetDocumentInfo().GetTitle());
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        public virtual void FixPdfTest10() {
            String filename = SOURCE_FOLDER + "XRefSectionWithFreeReferences4.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            NUnit.Framework.Assert.AreEqual(null, document.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(null, document.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.AreEqual(null, document.GetDocumentInfo().GetTitle());
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest11() {
            String filename = SOURCE_FOLDER + "XRefSectionWithoutSize.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest12() {
            String filename = SOURCE_FOLDER + "XRefWithBreaks.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FixPdfTest13() {
            String filename = SOURCE_FOLDER + "XRefWithInvalidGenerations1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < 10; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            bool exception = false;
            int i_1;
            PdfObject fontF1 = document.GetPage(997).GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary
                (PdfName.Font).Get(new PdfName("F1"));
            NUnit.Framework.Assert.IsTrue(fontF1 is PdfNull);
            //There is a generation number mismatch in xref table and object for 3093
            try {
                document.GetPdfObject(3093);
            }
            catch (iText.IO.Exceptions.IOException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            exception = false;
            try {
                for (i_1 = 11; i_1 < document.GetNumberOfPages() + 1; i_1++) {
                    PdfPage page = document.GetPage(i_1);
                    page.GetContentStream(0).GetBytes();
                }
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsFalse(exception);
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FixPdfTest14() {
            String filename = SOURCE_FOLDER + "XRefWithInvalidGenerations2.pdf";
            PdfReader reader = new PdfReader(filename);
            bool exception = false;
            try {
                new PdfDocument(reader);
            }
            catch (PdfException) {
                exception = true;
            }
            NUnit.Framework.Assert.IsTrue(exception);
            reader.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest15() {
            String filename = SOURCE_FOLDER + "XRefWithInvalidGenerations3.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest16() {
            String filename = SOURCE_FOLDER + "XrefWithInvalidOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasFixedXref(), "Need live fixXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest17() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest18() {
            String filename = SOURCE_FOLDER + "noXrefAndTrailerWithInfo.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            NUnit.Framework.Assert.IsTrue(document.GetDocumentInfo().GetProducer().Contains("iText Group NV (AGPL-version)"
                ));
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith1000Pages() {
            String filename = SOURCE_FOLDER + "1000PagesDocumentAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith1000PagesWithCompression() {
            String filename = SOURCE_FOLDER + "1000PagesDocumentWithFullCompressionAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith10Pages() {
            String filename = SOURCE_FOLDER + "10PagesDocumentAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith10PagesWithCompression() {
            String filename = SOURCE_FOLDER + "10PagesDocumentWithFullCompressionAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void AppendModeWith10PagesFix1() {
            String filename = SOURCE_FOLDER + "10PagesDocumentAppendedFix1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void AppendModeWith10PagesFix2() {
            String filename = SOURCE_FOLDER + "10PagesDocumentAppendedFix2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectXrefSizeInTrailer() {
            String filename = SOURCE_FOLDER + "HelloWorldIncorrectXRefSizeInTrailer.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectXrefSizeInTrailerAppend() {
            String filename = SOURCE_FOLDER + "10PagesDocumentAppendedIncorrectXRefSize.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection1() {
            lock (this) {
                String filename = SOURCE_FOLDER + "10PagesDocumentWithInvalidStreamLength.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                int pageCount = pdfDoc.GetNumberOfPages();
                for (int k = 1; k < pageCount + 1; k++) {
                    PdfPage page = pdfDoc.GetPage(k);
                    page.GetPdfObject().Get(PdfName.MediaBox);
                    byte[] content = page.GetFirstContentStream().GetBytes();
                    NUnit.Framework.Assert.AreEqual(57, content.Length);
                }
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection2() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingLength1.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection3() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingLength2.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(697, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection4() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingLength3.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection5() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingLength4.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection6() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingWithInvalidStreamLength1.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection7() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingWithInvalidStreamLength2.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection8() {
            lock (this) {
                String filename = SOURCE_FOLDER + "simpleCanvasWithDrawingWithInvalidStreamLength3.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(697, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(1000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection9() {
            lock (this) {
                String filename = SOURCE_FOLDER + "10PagesDocumentWithInvalidStreamLength2.pdf";
                PdfReader.correctStreamLength = false;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                int pageCount = pdfDoc.GetNumberOfPages();
                for (int k = 1; k < pageCount + 1; k++) {
                    PdfPage page = pdfDoc.GetPage(k);
                    page.GetPdfObject().Get(PdfName.MediaBox);
                    byte[] content = page.GetFirstContentStream().GetBytes();
                    NUnit.Framework.Assert.AreEqual(20, content.Length);
                }
                pdfDoc.Close();
                PdfReader.correctStreamLength = true;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FreeReferencesTest() {
            String filename = SOURCE_FOLDER + "freeReferences.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            NUnit.Framework.Assert.IsNull(pdfDoc.GetPdfObject(8));
            //Assert.assertFalse(pdfDoc.getReader().fixedXref);
            NUnit.Framework.Assert.IsFalse(pdfDoc.GetReader().rebuiltXref);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest02() {
            String cmpFile = SOURCE_FOLDER + "cmp_freeReferences02.pdf";
            String outputFile = DESTINATION_FOLDER + "freeReferences02.pdf";
            String inputFile = SOURCE_FOLDER + "freeReferences02.pdf";
            PdfWriter writer = new PdfWriter(outputFile);
            PdfReader reader = new PdfReader(inputFile);
            PdfDocument inputPdfDocument = new PdfDocument(reader);
            PdfDocument outputPdfDocument = new PdfDocument(writer);
            int lastPage = inputPdfDocument.GetNumberOfPages();
            inputPdfDocument.CopyPagesTo(lastPage, lastPage, outputPdfDocument);
            inputPdfDocument.Close();
            outputPdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, cmpFile, DESTINATION_FOLDER, 
                "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PdfVersionTest() {
            String filename = SOURCE_FOLDER + "hello.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, pdfDoc.GetPdfVersion());
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ZeroUpdateTest() {
            String filename = SOURCE_FOLDER + "stationery.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            //      Test such construction:
            //      xref
            //      0 0
            //      trailer
            //      <</Size 27/Root 1 0 R/Info 12 0 R//Prev 245232/XRefStm 244927>>
            //      startxref
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref());
            NUnit.Framework.Assert.IsTrue(((PdfDictionary)pdfDoc.GetPdfObject(1)).ContainsKey(PdfName.AcroForm));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncrementalUpdateWithOnlyZeroObjectUpdate() {
            String filename = SOURCE_FOLDER + "pdfReferenceUpdated.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref());
            // problem that is tested here originally was found because the StructTreeRoot dictionary wasn't read
            NUnit.Framework.Assert.IsTrue(pdfDoc.IsTagged());
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ENCOUNTERED_INVALID_MCR)]
        public virtual void WrongTagStructureFlushingTest() {
            //wrong /Pg number
            String source = SOURCE_FOLDER + "wrongTagStructureFlushingTest.pdf";
            String dest = DESTINATION_FOLDER + "wrongTagStructureFlushingTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            pdfDoc.SetTagged();
            NUnit.Framework.Assert.AreEqual(PdfNull.PDF_NULL, ((PdfDictionary)pdfDoc.GetPdfObject(12)).Get(PdfName.Pg)
                );
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2649")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void WrongStructureFlushingTest() {
            //TODO: update after DEVSIX-2649 fix
            //wrong /key number
            String source = SOURCE_FOLDER + "wrongStructureFlushingTest.pdf";
            String dest = DESTINATION_FOLDER + "wrongStructureFlushingTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ReaderReuseTest() {
            String filename = SOURCE_FOLDER + "hello.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc1 = new PdfDocument(reader);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_READER_HAS_BEEN_ALREADY_UTILIZED, e.Message
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void HugeInvalidIndRefObjNumberTest() {
            String filename = SOURCE_FOLDER + "hugeIndRefObjNum.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfObject pdfObject = pdfDoc.GetPdfObject(4);
            NUnit.Framework.Assert.IsTrue(pdfObject.IsDictionary());
            NUnit.Framework.Assert.AreEqual(PdfNull.PDF_NULL, ((PdfDictionary)pdfObject).Get(PdfName.Pg));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2133")]
        public virtual void TestFileIsNotLockedOnException() {
            FileInfo nonPdfFileName = new FileInfo(SOURCE_FOLDER + "text_file.txt");
            NUnit.Framework.Assert.IsTrue(nonPdfFileName.Exists);
            bool exceptionThrown = false;
            try {
                PdfReader reader = new PdfReader(nonPdfFileName);
            }
            catch (iText.IO.Exceptions.IOException) {
                exceptionThrown = true;
                // File should be available for writing
                Stream stream = FileUtil.GetFileOutputStream(nonPdfFileName);
                stream.Write(new byte[] { 0 });
            }
            NUnit.Framework.Assert.IsTrue(exceptionThrown);
        }

        [NUnit.Framework.Test]
        public virtual void TestManyAppendModeUpdates() {
            String file = SOURCE_FOLDER + "manyAppendModeUpdates.pdf";
            PdfReader reader = new PdfReader(file);
            PdfDocument document = new PdfDocument(reader);
            document.Close();
        }

        private bool ObjectTypeEqualTo(PdfObject @object, PdfName type) {
            PdfName objectType = ((PdfDictionary)@object).GetAsName(PdfName.Type);
            return type.Equals(objectType);
        }

        [NUnit.Framework.Test]
        public virtual void HasRebuiltXrefPdfDocumentNotReadTest() {
            PdfReader hasRebuiltXrefReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => hasRebuiltXrefReader.HasRebuiltXref
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void HasRebuiltXrefReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader hasRebuiltXrefReader = new _PdfReader_1759(filename);
            ReadingNotCompletedTest(hasRebuiltXrefReader);
        }

        private sealed class _PdfReader_1759 : PdfReader {
            public _PdfReader_1759(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasRebuiltXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasHybridXrefPdfDocumentNotReadTest() {
            PdfReader hasHybridXrefPdfReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => hasHybridXrefPdfReader.HasHybridXref
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void HasHybridXrefReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader hasHybridXrefPdfReader = new _PdfReader_1782(filename);
            ReadingNotCompletedTest(hasHybridXrefPdfReader);
        }

        private sealed class _PdfReader_1782 : PdfReader {
            public _PdfReader_1782(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasHybridXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasXrefStmPdfDocumentNotReadTest() {
            PdfReader hasXrefStmReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => hasXrefStmReader.HasXrefStm());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void HasXrefStmReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader hasXrefStmReader = new _PdfReader_1805(filename);
            ReadingNotCompletedTest(hasXrefStmReader);
        }

        private sealed class _PdfReader_1805 : PdfReader {
            public _PdfReader_1805(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasXrefStm();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasFixedXrefPdfDocumentNotReadTest() {
            PdfReader hasFixedXrefReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => hasFixedXrefReader.HasFixedXref());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void HasFixedXrefReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader hasFixedXrefReader = new _PdfReader_1828(filename);
            ReadingNotCompletedTest(hasFixedXrefReader);
        }

        private sealed class _PdfReader_1828 : PdfReader {
            public _PdfReader_1828(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasFixedXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetLastXrefPdfDocumentNotReadTest() {
            PdfReader getLastXrefReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getLastXrefReader.GetLastXref());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetLastXrefReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader getLastXrefReader = new _PdfReader_1851(filename);
            ReadingNotCompletedTest(getLastXrefReader);
        }

        private sealed class _PdfReader_1851 : PdfReader {
            public _PdfReader_1851(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetLastXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPermissionsPdfDocumentNotReadTest() {
            PdfReader getPermissionsReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getPermissionsReader.GetPermissions
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetPermissionsReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader getPermissionsReader = new _PdfReader_1874(filename);
            ReadingNotCompletedTest(getPermissionsReader);
        }

        private sealed class _PdfReader_1874 : PdfReader {
            public _PdfReader_1874(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetPermissions();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsOpenedWithFullPPdfDocumentNotReadTest() {
            PdfReader isOpenedWithFullPReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => isOpenedWithFullPReader.IsOpenedWithFullPermission
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IsOpenedWithFullPReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader isOpenedWithFullPReader = new _PdfReader_1899(filename);
            ReadingNotCompletedTest(isOpenedWithFullPReader);
        }

        private sealed class _PdfReader_1899 : PdfReader {
            public _PdfReader_1899(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.IsOpenedWithFullPermission();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetCryptoModePdfDocumentNotReadTest() {
            PdfReader getCryptoModeReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getCryptoModeReader.GetCryptoMode()
                );
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetCryptoModeReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader getCryptoModeReader = new _PdfReader_1922(filename);
            ReadingNotCompletedTest(getCryptoModeReader);
        }

        private sealed class _PdfReader_1922 : PdfReader {
            public _PdfReader_1922(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetCryptoMode();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComputeUserPasswordPdfDocumentNotReadTest() {
            PdfReader computeUserPasswordReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => computeUserPasswordReader.ComputeUserPassword
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ComputeUserPasswordReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader computeUserPasswordReader = new _PdfReader_1947(filename);
            ReadingNotCompletedTest(computeUserPasswordReader);
        }

        private sealed class _PdfReader_1947 : PdfReader {
            public _PdfReader_1947(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.ComputeUserPassword();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetOriginalFileIdPdfDocumentNotReadTest() {
            PdfReader getOriginalFileIdReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getOriginalFileIdReader.GetOriginalFileId
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOriginalFileIdReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader getOriginalFileIdReader = new _PdfReader_1970(filename);
            ReadingNotCompletedTest(getOriginalFileIdReader);
        }

        private sealed class _PdfReader_1970 : PdfReader {
            public _PdfReader_1970(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetOriginalFileId();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetModifiedFileIdPdfDocumentNotReadTest() {
            PdfReader getModifiedFileIdReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getModifiedFileIdReader.GetModifiedFileId
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetModifiedFileIdReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader getModifiedFileIdReader = new _PdfReader_1993(filename);
            ReadingNotCompletedTest(getModifiedFileIdReader);
        }

        private sealed class _PdfReader_1993 : PdfReader {
            public _PdfReader_1993(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetModifiedFileId();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsEncryptedPdfDocumentNotReadTest() {
            PdfReader isEncryptedReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => isEncryptedReader.IsEncrypted());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IsEncryptedReadingNotCompletedTest() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            PdfReader isEncryptedReader = new _PdfReader_2016(filename);
            ReadingNotCompletedTest(isEncryptedReader);
        }

        private sealed class _PdfReader_2016 : PdfReader {
            public _PdfReader_2016(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.IsEncrypted();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void Pdf11VersionValidTest() {
            String fileName = SOURCE_FOLDER + "pdf11Version.pdf";
            new PdfDocument(new PdfReader(fileName));
        }

        [NUnit.Framework.Test]
        public virtual void NoPdfVersionTest() {
            PdfReader pdfReader = new PdfReader(SOURCE_FOLDER + "noPdfVersion.pdf");
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfReader.ReadPdf());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_VERSION_IS_NOT_VALID, exception.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void StartxrefIsNotFollowedByANumberTest() {
            PdfReader pdfReader = new PdfReader(SOURCE_FOLDER + "startxrefIsNotFollowedByANumber.pdf");
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfReader.ReadXref());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_STARTXREF_IS_NOT_FOLLOWED_BY_A_NUMBER, 
                exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StartxrefNotFoundTest() {
            PdfReader pdfReader = new PdfReader(SOURCE_FOLDER + "startxrefNotFound.pdf");
            Exception exception = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => pdfReader
                .ReadXref());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_STARTXREF_NOT_FOUND, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CloseStreamCreatedByITextTest() {
            String fileName = SOURCE_FOLDER + "emptyPdf.pdf";
            String copiedFileName = DESTINATION_FOLDER + "emptyPdf.pdf";
            //Later in the test we will need to delete a file. Since we do not want to delete it from sources, we will
            // copy it to destination folder.
            FileInfo copiedFile = CopyFileForTest(fileName, copiedFileName);
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new PdfReader(fileName
                ));
            NUnit.Framework.Assert.AreEqual(iText.IO.Exceptions.IOException.PdfHeaderNotFound, e.Message);
            //This check is meaningfull only on Windows, since on other OS the fact of a stream being open doesn't
            // prevent the stream from being deleted.
            NUnit.Framework.Assert.IsTrue(FileUtil.DeleteFile(copiedFile));
        }

        [NUnit.Framework.Test]
        public virtual void NotCloseUserStreamTest() {
            String fileName = SOURCE_FOLDER + "emptyPdf.pdf";
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().CreateSource(pdfStream);
                Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new PdfReader(randomAccessSource
                    , new ReaderProperties()));
                //An exception would be thrown, if stream is closed.
                NUnit.Framework.Assert.AreEqual(-1, pdfStream.Read());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        public virtual void EndDicInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayEndDictToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        public virtual void EndArrayClosingBracketInsteadOfEndDicTest() {
            String fileName = SOURCE_FOLDER + "endArrayClosingBracketInsteadOfEndDic.pdf";
            Exception exception = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new PdfDocument
                (new PdfReader(fileName)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, 
                "]"), exception.InnerException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EndDicClosingBracketInsideTheDicTest() {
            String fileName = SOURCE_FOLDER + "endDicClosingBracketInsideTheDic.pdf";
            Exception exception = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => new PdfDocument
                (new PdfReader(fileName)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, 
                ">>"), exception.InnerException.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        public virtual void EofInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayEOFToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        public virtual void EndObjInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayEndObjToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void NameInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayNameToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        public virtual void ObjInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayObjToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        public virtual void RefInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayRefToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, Count = 2)]
        public virtual void StartArrayInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayStartArrayToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelExceptionMessageConstant.UNEXPECTED_TOKEN)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void StringInsteadOfArrayClosingBracketTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayStringToken.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(fileName));
            PdfArray actual = (PdfArray)document.GetPdfObject(4);
            PdfArray expected = new PdfArray(new float[] { 5, 10, 15, 20 });
            for (int i = 0; i < expected.Size(); i++) {
                NUnit.Framework.Assert.AreEqual(expected.Get(i), actual.Get(i));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ClosingArrayBracketMissingConservativeTest() {
            String fileName = SOURCE_FOLDER + "invalidArrayObjToken.pdf";
            PdfReader reader = new PdfReader(fileName);
            reader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
            PdfDocument document = new PdfDocument(reader);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => document
                .GetPdfObject(4));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, 
                "obj"), exception.InnerException.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ReadRASInputStreamClosedTest() {
            String fileName = SOURCE_FOLDER + "hello.pdf";
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(pdfStream);
                RASInputStream rasInputStream = new RASInputStream(randomAccessSource);
                randomAccessSource.Close();
                Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => new PdfReader(rasInputStream
                    ));
                NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadRASInputStreamTest() {
            String fileName = SOURCE_FOLDER + "hello.pdf";
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(pdfStream);
                RASInputStream rasInputStream = new RASInputStream(randomAccessSource);
                using (PdfReader reader = new PdfReader(rasInputStream)) {
                    randomAccessSource.Close();
                    Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => new PdfDocument(reader
                        ));
                    NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadRASInputStreamValidTest() {
            String fileName = SOURCE_FOLDER + "hello.pdf";
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(pdfStream);
                RASInputStream rasInputStream = new RASInputStream(randomAccessSource);
                using (PdfReader reader = new PdfReader(rasInputStream)) {
                    NUnit.Framework.Assert.DoesNotThrow(() => new PdfDocument(reader));
                }
            }
        }

        private static FileInfo CopyFileForTest(String fileName, String copiedFileName) {
            FileInfo copiedFile = new FileInfo(copiedFileName);
            File.Copy(System.IO.Path.Combine(fileName), System.IO.Path.Combine(copiedFileName));
            return copiedFile;
        }

        private PdfReader PdfDocumentNotReadTestInit() {
            String filename = SOURCE_FOLDER + "XrefWithNullOffsets.pdf";
            return new PdfReader(filename);
        }

        private void ReadingNotCompletedTest(PdfReader reader) {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevelPdfDocumentNotReadTest() {
            PdfReader getModifiedFileIdReader = PdfDocumentNotReadTestInit();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => getModifiedFileIdReader.GetPdfAConformanceLevel
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_HAS_NOT_BEEN_READ_YET, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void XrefStreamPointsItselfTest() {
            String fileName = SOURCE_FOLDER + "xrefStreamPointsItself.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.LENIENT, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STREAM_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XrefStreamPointsItselfConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "xrefStreamPointsItself.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                pdfReader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.CONSERVATIVE, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STREAM_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XrefStreamsHaveCycledReferencesTest() {
            String fileName = SOURCE_FOLDER + "cycledReferencesInXrefStreams.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.LENIENT, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STREAM_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XrefStreamsHaveCycledReferencesConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "cycledReferencesInXrefStreams.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                pdfReader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.CONSERVATIVE, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STREAM_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void XrefTablesHaveCycledReferencesTest() {
            String fileName = SOURCE_FOLDER + "cycledReferencesInXrefTables.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                NUnit.Framework.Assert.DoesNotThrow(() => new PdfDocument(pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.LENIENT, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.IsTrue(pdfReader.HasRebuiltXref());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void XrefTablePointsItselfTest() {
            String fileName = SOURCE_FOLDER + "xrefTablePointsItself.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                NUnit.Framework.Assert.DoesNotThrow(() => new PdfDocument(pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.LENIENT, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.IsTrue(pdfReader.HasRebuiltXref());
            }
        }

        [NUnit.Framework.Test]
        public virtual void XrefTablePointsItselfConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "xrefTablePointsItself.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                pdfReader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.CONSERVATIVE, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_TABLE_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XrefTablesHaveCycledReferencesConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "cycledReferencesInXrefTables.pdf";
            using (PdfReader pdfReader = new PdfReader(fileName)) {
                pdfReader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(XrefCycledReferencesException), () => new PdfDocument
                    (pdfReader));
                NUnit.Framework.Assert.AreEqual(PdfReader.StrictnessLevel.CONSERVATIVE, pdfReader.GetStrictnessLevel());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_TABLE_HAS_CYCLED_REFERENCES, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckXrefStreamInvalidSize() {
            String fileName = SOURCE_FOLDER + "xrefStreamInvalidSize.pdf";
            using (PdfReader reader = new PdfReader(fileName)) {
                Exception ex = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => new PdfDocument(reader
                    ));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, ex.
                    Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckXrefPrevWithDifferentTypesTest() {
            PdfNumber numberXrefPrev = new PdfNumber(20);
            PdfString stringXrefPrev = new PdfString("iText", PdfEncodings.UNICODE_BIG);
            PdfIndirectReference indirectReferenceXrefPrev = new PdfIndirectReference(null, 41);
            PdfIndirectReference indirectReferenceToString = new PdfIndirectReference(null, 42);
            indirectReferenceXrefPrev.SetRefersTo(numberXrefPrev);
            indirectReferenceToString.SetRefersTo(stringXrefPrev);
            using (PdfReader reader = new PdfReader(new MemoryStream(CreatePdfDocumentForTest()))) {
                reader.SetStrictnessLevel(PdfReader.StrictnessLevel.LENIENT);
                NUnit.Framework.Assert.DoesNotThrow(() => reader.GetXrefPrev(numberXrefPrev));
                NUnit.Framework.Assert.DoesNotThrow(() => reader.GetXrefPrev(indirectReferenceXrefPrev));
                // Check string xref prev with StrictnessLevel#LENIENT.
                Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => reader.GetXrefPrev
                    (stringXrefPrev));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
                // Check indirect reference to string xref prev with StrictnessLevel#LENIENT.
                exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => reader.GetXrefPrev(indirectReferenceToString
                    ));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckXrefPrevWithDifferentTypesConservativeModeTest() {
            PdfNumber numberXrefPrev = new PdfNumber(20);
            PdfString stringXrefPrev = new PdfString("iText", PdfEncodings.UNICODE_BIG);
            PdfIndirectReference indirectReferenceXrefPrev = new PdfIndirectReference(null, 41);
            PdfIndirectReference indirectReferenceToString = new PdfIndirectReference(null, 42);
            indirectReferenceXrefPrev.SetRefersTo(numberXrefPrev);
            indirectReferenceToString.SetRefersTo(stringXrefPrev);
            using (PdfReader reader = new PdfReader(new MemoryStream(CreatePdfDocumentForTest()))) {
                reader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                NUnit.Framework.Assert.DoesNotThrow(() => reader.GetXrefPrev(numberXrefPrev));
                // Check indirect reference to number xref prev with StrictnessLevel#CONSERVATIVE.
                Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => reader.GetXrefPrev
                    (indirectReferenceXrefPrev));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
                // Check string xref prev with StrictnessLevel#CONSERVATIVE.
                exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => reader.GetXrefPrev(stringXrefPrev
                    ));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
                // Check indirect reference to string xref prev with StrictnessLevel#CONSERVATIVE.
                exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => reader.GetXrefPrev(indirectReferenceToString
                    ));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithIndirectPrevTest() {
            String fileName = SOURCE_FOLDER + "indirectPrev.pdf";
            String outputName = DESTINATION_FOLDER + "documentWithIndirectPrev.pdf";
            // Open pdf doc and check that xref prev is indirect.
            using (PdfReader reader = new PdfReader(fileName)) {
                using (PdfDocument document = new PdfDocument(reader)) {
                    PdfDictionary documentTrailer = document.GetTrailer();
                    NUnit.Framework.Assert.IsTrue(documentTrailer.Get(PdfName.Prev, false).IsIndirectReference());
                }
            }
            // Read/write pdf document to rewrite xref structure.
            using (PdfReader reader_1 = new PdfReader(fileName)) {
                using (PdfWriter writer = new PdfWriter(outputName)) {
                    using (PdfDocument document_1 = new PdfDocument(reader_1, writer)) {
                    }
                }
            }
            // Read and check that in created pdf we have valid xref prev.
            using (PdfReader reader_2 = new PdfReader(outputName)) {
                using (PdfDocument document_2 = new PdfDocument(reader_2)) {
                    PdfDictionary trailer = document_2.GetTrailer();
                    NUnit.Framework.Assert.IsNull(trailer.Get(PdfName.Prev, false));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotChangeInvalidPrevInAppendModeTest() {
            String fileName = SOURCE_FOLDER + "indirectPrev.pdf";
            String outputName = DESTINATION_FOLDER + "invalidPrevAppendMode.pdf";
            // Read document and check that we have indirect prev.
            using (PdfReader reader = new PdfReader(fileName)) {
                using (PdfDocument document = new PdfDocument(reader)) {
                    PdfDictionary documentTrailer = document.GetTrailer();
                    NUnit.Framework.Assert.IsTrue(documentTrailer.Get(PdfName.Prev, false).IsIndirectReference());
                }
            }
            // Read and write document in append mode to not change previous xref prev.
            StampingProperties properties = new StampingProperties().UseAppendMode();
            using (PdfReader reader_1 = new PdfReader(fileName)) {
                using (PdfWriter writer = new PdfWriter(outputName)) {
                    using (PdfDocument document_1 = new PdfDocument(reader_1, writer, properties)) {
                        document_1.AddNewPage();
                    }
                }
            }
            // Read resulted document and check, that previous xref prev doesn't change and current is pdfNumber.
            using (PdfReader reader_2 = new PdfReader(outputName)) {
                using (PdfDocument document_2 = new PdfDocument(reader_2)) {
                    PdfDictionary trailer = document_2.GetTrailer();
                    NUnit.Framework.Assert.IsFalse(trailer.Get(PdfName.Prev, false).IsIndirectReference());
                    PdfNumber prevPointer = (PdfNumber)trailer.Get(PdfName.Prev);
                    reader_2.tokens.Seek(prevPointer.LongValue());
                    PdfDictionary previousTrailer = reader_2.ReadXrefSection();
                    NUnit.Framework.Assert.IsTrue(previousTrailer.Get(PdfName.Prev, false).IsIndirectReference());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadPdfInvalidPrevConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "indirectPrev.pdf";
            // Simply open document with StrictnessLevel#CONSERVATIVE.
            using (PdfReader reader = new PdfReader(fileName)) {
                reader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => new PdfDocument
                    (reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                    .Message);
            }
            // Open document for read/write with stamping properties and StrictnessLevel#CONSERVATIVE.
            StampingProperties properties = new StampingProperties().UseAppendMode();
            using (PdfReader reader_1 = new PdfReader(fileName)) {
                using (PdfWriter writer = new PdfWriter(new ByteArrayOutputStream())) {
                    reader_1.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                    Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => new PdfDocument
                        (reader_1, writer, properties));
                    NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                        .Message);
                }
            }
            // Open document for read/write without stamping properties but with StrictnessLevel#CONSERVATIVE.
            using (PdfReader reader_2 = new PdfReader(fileName)) {
                using (PdfWriter writer_1 = new PdfWriter(new ByteArrayOutputStream())) {
                    reader_2.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                    Exception exception = NUnit.Framework.Assert.Catch(typeof(InvalidXRefPrevException), () => new PdfDocument
                        (reader_2, writer_1));
                    NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_PREV_SHALL_BE_DIRECT_NUMBER_OBJECT, exception
                        .Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StreamWithoutEndstreamKeywordTest() {
            String fileName = SOURCE_FOLDER + "NoEndstreamKeyword.pdf";
            using (PdfReader reader = new PdfReader(fileName)) {
                reader.SetStrictnessLevel(PdfReader.StrictnessLevel.LENIENT);
                using (PdfDocument document = new PdfDocument(reader)) {
                    PdfCatalog catalog = new PdfCatalog((PdfDictionary)reader.trailer.Get(PdfName.Root, true));
                    PdfStream xmpMetadataStream = catalog.GetPdfObject().GetAsStream(PdfName.Metadata);
                    int xmpMetadataStreamLength = ((PdfNumber)xmpMetadataStream.Get(PdfName.Length)).IntValue();
                    // 27600 is actual invalid length of stream. In reader StrictnessLevel#LENIENT we expect, that this
                    // length will be fixed.
                    NUnit.Framework.Assert.AreNotEqual(27600, xmpMetadataStreamLength);
                    // 3090 is expected length of the stream after fix.
                    NUnit.Framework.Assert.AreEqual(3090, xmpMetadataStreamLength);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void StreamWithoutEndstreamKeywordConservativeModeTest() {
            String fileName = SOURCE_FOLDER + "NoEndstreamKeyword.pdf";
            using (PdfReader reader = new PdfReader(fileName)) {
                reader.SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.STREAM_SHALL_END_WITH_ENDSTREAM, exception.
                    Message);
                PdfCatalog catalog = new PdfCatalog((PdfDictionary)reader.trailer.Get(PdfName.Root, true));
                PdfStream xmpMetadataStream = catalog.GetPdfObject().GetAsStream(PdfName.Metadata);
                // 27600 is actual invalid length of stream. In reader StrictnessLevel#CONSERVATIVE we expect, that
                // exception would be thrown and length wouldn't be fixed.
                NUnit.Framework.Assert.AreEqual(27600, ((PdfNumber)xmpMetadataStream.Get(PdfName.Length)).IntValue());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TokensPositionIsNotUpdatedWhileReadingLengthTest() {
            String filename = SOURCE_FOLDER + "simpleDocWithIndirectLength.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename))) {
                PdfTokenizer tokenizer = pdfDoc.GetReader().tokens;
                // we will try to get the content stream object
                // since it's not been gotten yet, iText will read this object,
                // which will change the tokenizer's position
                PdfStream pageContentStream = (PdfStream)pdfDoc.GetPdfObject(5);
                // tokenizer's position after reading object should point to the end of the object's stream
                NUnit.Framework.Assert.AreEqual(pageContentStream.GetOffset() + pageContentStream.GetLength(), tokenizer.GetPosition
                    ());
                // let's read next valid token and check that it means ending stream
                tokenizer.NextValidToken();
                tokenizer.TokenValueEqualsTo(ByteUtils.GetIsoBytes("endstream"));
            }
        }

        private static PdfDictionary GetTestPdfDictionary() {
            Dictionary<PdfName, PdfObject> tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("b"), new PdfName("c"));
            return new PdfDictionary(tmpMap);
        }

        private static byte[] CreatePdfDocumentForTest() {
            using (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                    pdfDoc.AddNewPage();
                }
                return baos.ToArray();
            }
        }
    }
}
