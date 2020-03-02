/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfReaderTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfReaderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfReaderTest/";

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 6";

        internal const String title = "Empty iText 6 Document";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void OpenSimpleDoc() {
            String filename = destinationFolder + "openSimpleDoc.pdf";
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
            String filename = sourceFolder + "simpleCanvasWithFullCompression.pdf";
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
            String filename = sourceFolder + "objectStreamIncrementalUpdate.pdf";
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
            String filename = sourceFolder + "twoCustomDictionariesInObjectStream.pdf";
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
            String filename = sourceFolder + "100PagesDocumentWithFlateFilter.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(100, document.GetNumberOfPages(), "Page count");
            String contentTemplate = "q\n" + "BT\n" + "36 700 Td\n" + "/F1 72 Tf\n" + "({0})Tj\n" + "ET\n" + "Q\n" + "100 500 100 100 re\n"
                 + "f\n";
            for (int i = 1; i <= document.GetNumberOfPages(); i++) {
                PdfPage page = document.GetPage(i);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(contentTemplate, i), iText.IO.Util.JavaUtil.GetStringForBytes
                    (content), "Page content " + i);
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesRead() {
            String filename = destinationFolder + "primitivesRead.pdf";
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
            String filename = destinationFolder + "indirectsChain1.pdf";
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
            String filename = destinationFolder + "indirectsChain2.pdf";
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
            String filename = sourceFolder + "indirectsChain3.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void InvalidIndirect() {
            String filename = sourceFolder + "invalidIndirect.pdf";
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
            String filename = sourceFolder + "1000PagesDocument.pdf";
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            reader.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            for (int i = 1; i < pageCount + 1; i++) {
                int pageNum = document.GetNumberOfPages();
                PdfPage page = document.GetPage(pageNum);
                document.RemovePage(pageNum);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest02() {
            String filename = sourceFolder + "1000PagesDocumentWithFullCompression.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest03() {
            String filename = sourceFolder + "10PagesDocumentWithLeafs.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest04() {
            String filename = sourceFolder + "PagesDocument.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(3, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(pageNum + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest05() {
            String filename = sourceFolder + "PagesDocument05.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(3, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(i + "00"));
            }
            for (int i = 1; i < pageCount + 1; i++) {
                PdfPage page = document.GetPage(1);
                document.RemovePage(page);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.StartsWith(pageNum + "00"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest06() {
            String filename = sourceFolder + "PagesDocument06.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(2, pageCount);
            PdfPage page = document.GetPage(1);
            String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            page = document.GetPage(2);
            content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("300"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
            reader = new PdfReader(filename);
            document = new PdfDocument(reader);
            page = document.GetPage(2);
            document.RemovePage(page);
            content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("300"));
            page = document.GetPage(1);
            document.RemovePage(1);
            content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest07() {
            String filename = sourceFolder + "PagesDocument07.pdf";
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
            String filename = sourceFolder + "PagesDocument08.pdf";
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
            String filename = sourceFolder + "PagesDocument09.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1, pageCount);
            PdfPage page = document.GetPage(1);
            String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            page = document.GetPage(1);
            document.RemovePage(1);
            content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
            NUnit.Framework.Assert.IsTrue(content.StartsWith("100"));
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest10() {
            String filename = sourceFolder + "1000PagesDocumentWithFullCompression.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            Random rnd = new Random();
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                int pageNum = rnd.Next(document.GetNumberOfPages()) + 1;
                PdfPage page = document.GetPage(pageNum);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + pageNum + ")"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PagesTest11() {
            String filename = sourceFolder + "hello.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            try {
                document.GetPage(-30);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfException.RequestedPageNumberIsOutOfBounds, -30
                    ), e.Message);
            }
            try {
                document.GetPage(0);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfException.RequestedPageNumberIsOutOfBounds, 0)
                    , e.Message);
            }
            document.GetPage(1);
            try {
                document.GetPage(25);
            }
            catch (IndexOutOfRangeException e) {
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfException.RequestedPageNumberIsOutOfBounds, 25
                    ), e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void CorrectSimpleDoc1() {
            String filename = sourceFolder + "correctSimpleDoc1.pdf";
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
            String filename = sourceFolder + "correctSimpleDoc2.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT, Count = 1)]
        public virtual void CorrectSimpleDoc3() {
            String filename = sourceFolder + "correctSimpleDoc3.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void CorrectSimpleDoc4() {
            String filename = sourceFolder + "correctSimpleDoc4.pdf";
            PdfReader reader = new PdfReader(filename);
            try {
                //NOTE test with abnormal object declaration that iText can't resolve.
                PdfDocument document = new PdfDocument(reader);
                NUnit.Framework.Assert.Fail("Expect exception");
            }
            catch (PdfException e) {
                NUnit.Framework.Assert.AreEqual(PdfException.InvalidPageStructurePagesPagesMustBePdfDictionary, e.Message);
            }
            finally {
                reader.Close();
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest01() {
            String filename = sourceFolder + "OnlyTrailer.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest02() {
            String filename = sourceFolder + "CompressionShift1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest03() {
            String filename = sourceFolder + "CompressionShift2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest04() {
            String filename = sourceFolder + "CompressionWrongObjStm.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest05() {
            String filename = sourceFolder + "CompressionWrongShift.pdf";
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
            String filename = sourceFolder + "InvalidOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasFixedXref(), "Need fixXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 2)]
        public virtual void FixPdfTest07() {
            String filename = sourceFolder + "XRefSectionWithFreeReferences1.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest08() {
            String filename = sourceFolder + "XRefSectionWithFreeReferences2.pdf";
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest09() {
            String filename = sourceFolder + "XRefSectionWithFreeReferences3.pdf";
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        public virtual void FixPdfTest10() {
            String filename = sourceFolder + "XRefSectionWithFreeReferences4.pdf";
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
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest11() {
            String filename = sourceFolder + "XRefSectionWithoutSize.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest12() {
            String filename = sourceFolder + "XRefWithBreaks.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FixPdfTest13() {
            String filename = sourceFolder + "XRefWithInvalidGenerations1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < 10; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
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
            catch (iText.IO.IOException) {
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
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FixPdfTest14() {
            String filename = sourceFolder + "XRefWithInvalidGenerations2.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest15() {
            String filename = sourceFolder + "XRefWithInvalidGenerations3.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FixPdfTest16() {
            String filename = sourceFolder + "XrefWithInvalidOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref(), "No need in fixXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasFixedXref(), "Need live fixXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest17() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void FixPdfTest18() {
            String filename = sourceFolder + "noXrefAndTrailerWithInfo.pdf";
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
            String filename = sourceFolder + "1000PagesDocumentAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith1000PagesWithCompression() {
            String filename = sourceFolder + "1000PagesDocumentWithFullCompressionAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(1000, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith10Pages() {
            String filename = sourceFolder + "10PagesDocumentAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AppendModeWith10PagesWithCompression() {
            String filename = sourceFolder + "10PagesDocumentWithFullCompressionAppended.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "No need in rebuildXref()");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void AppendModeWith10PagesFix1() {
            String filename = sourceFolder + "10PagesDocumentAppendedFix1.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void AppendModeWith10PagesFix2() {
            String filename = sourceFolder + "10PagesDocumentAppendedFix2.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            int pageCount = document.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(10, pageCount);
            for (int i = 1; i < document.GetNumberOfPages() + 1; i++) {
                PdfPage page = document.GetPage(i);
                String content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(0).GetBytes());
                NUnit.Framework.Assert.IsFalse(content.Length == 0);
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(1).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("(" + i + ")"));
                content = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetContentStream(2).GetBytes());
                NUnit.Framework.Assert.IsTrue(content.Contains("Append mode"));
            }
            NUnit.Framework.Assert.IsTrue(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectXrefSizeInTrailer() {
            String filename = sourceFolder + "HelloWorldIncorrectXRefSizeInTrailer.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectXrefSizeInTrailerAppend() {
            String filename = sourceFolder + "10PagesDocumentAppendedIncorrectXRefSize.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Need rebuildXref()");
            NUnit.Framework.Assert.IsNotNull(document.GetTrailer().Get(PdfName.ID), "Invalid trailer");
            document.Close();
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection1() {
            lock (this) {
                String filename = sourceFolder + "10PagesDocumentWithInvalidStreamLength.pdf";
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

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection2() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingLength1.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection3() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingLength2.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(697, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection4() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingLength3.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection5() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingLength4.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection6() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingWithInvalidStreamLength1.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection7() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingWithInvalidStreamLength2.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(696, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection8() {
            lock (this) {
                String filename = sourceFolder + "simpleCanvasWithDrawingWithInvalidStreamLength3.pdf";
                PdfReader.correctStreamLength = true;
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
                PdfPage page = pdfDoc.GetPage(1);
                page.GetPdfObject().Get(PdfName.MediaBox);
                byte[] content = page.GetFirstContentStream().GetBytes();
                NUnit.Framework.Assert.AreEqual(697, content.Length);
                pdfDoc.Close();
            }
        }

#if !NETSTANDARD1_6
        [NUnit.Framework.Timeout(1000)]
#endif
        [NUnit.Framework.Test]
        public virtual void StreamLengthCorrection9() {
            lock (this) {
                String filename = sourceFolder + "10PagesDocumentWithInvalidStreamLength2.pdf";
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
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void FreeReferencesTest() {
            String filename = sourceFolder + "freeReferences.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            NUnit.Framework.Assert.IsNull(pdfDoc.GetPdfObject(8));
            //Assert.assertFalse(pdfDoc.getReader().fixedXref);
            NUnit.Framework.Assert.IsFalse(pdfDoc.GetReader().rebuiltXref);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest02() {
            String cmpFile = sourceFolder + "cmp_freeReferences02.pdf";
            String outputFile = destinationFolder + "freeReferences02.pdf";
            String inputFile = sourceFolder + "freeReferences02.pdf";
            PdfWriter writer = new PdfWriter(outputFile);
            PdfReader reader = new PdfReader(inputFile);
            PdfDocument inputPdfDocument = new PdfDocument(reader);
            PdfDocument outputPdfDocument = new PdfDocument(writer);
            int lastPage = inputPdfDocument.GetNumberOfPages();
            inputPdfDocument.CopyPagesTo(lastPage, lastPage, outputPdfDocument);
            inputPdfDocument.Close();
            outputPdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void PdfVersionTest() {
            String filename = sourceFolder + "hello.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, pdfDoc.GetPdfVersion());
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ZeroUpdateTest() {
            String filename = sourceFolder + "stationery.pdf";
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
            String filename = sourceFolder + "pdfReferenceUpdated.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDoc = new PdfDocument(reader);
            NUnit.Framework.Assert.IsFalse(reader.HasFixedXref());
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref());
            // problem that is tested here originally was found because the StructTreeRoot dictionary wasn't read
            NUnit.Framework.Assert.IsTrue(pdfDoc.IsTagged());
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        [LogMessage(iText.IO.LogMessageConstant.ENCOUNTERED_INVALID_MCR)]
        public virtual void WrongTagStructureFlushingTest() {
            //wrong /Pg number
            String source = sourceFolder + "wrongTagStructureFlushingTest.pdf";
            String dest = destinationFolder + "wrongTagStructureFlushingTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            pdfDoc.SetTagged();
            NUnit.Framework.Assert.AreEqual(PdfNull.PDF_NULL, ((PdfDictionary)pdfDoc.GetPdfObject(12)).Get(PdfName.Pg)
                );
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2649")]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE, Count = 1)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void WrongStructureFlushingTest() {
            //TODO: update after DEVSIX-2649 fix
            //wrong /key number
            String source = sourceFolder + "wrongStructureFlushingTest.pdf";
            String dest = destinationFolder + "wrongStructureFlushingTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ReaderReuseTest() {
            NUnit.Framework.Assert.That(() =>  {
                String filename = sourceFolder + "hello.pdf";
                PdfReader reader = new PdfReader(filename);
                PdfDocument pdfDoc1 = new PdfDocument(reader);
                PdfDocument pdfDoc2 = new PdfDocument(reader);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.PdfReaderHasBeenAlreadyUtilized))
;
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        public virtual void HugeInvalidIndRefObjNumberTest() {
            String filename = sourceFolder + "hugeIndRefObjNum.pdf";
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
            FileInfo nonPdfFileName = new FileInfo(sourceFolder + "text_file.txt");
            NUnit.Framework.Assert.IsTrue(nonPdfFileName.Exists);
            bool exceptionThrown = false;
            try {
                PdfReader reader = new PdfReader(nonPdfFileName);
            }
            catch (iText.IO.IOException) {
                exceptionThrown = true;
                // File should be available for writing
                Stream stream = FileUtil.GetFileOutputStream(nonPdfFileName);
                stream.Write(new byte[] { 0 });
            }
            NUnit.Framework.Assert.IsTrue(exceptionThrown);
        }

        [NUnit.Framework.Test]
        public virtual void TestManyAppendModeUpdates() {
            String file = sourceFolder + "manyAppendModeUpdates.pdf";
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
            NUnit.Framework.Assert.That(() =>  {
                PdfReader hasRebuiltXrefReader = PdfDocumentNotReadTestInit();
                hasRebuiltXrefReader.HasRebuiltXref();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void HasRebuiltXrefReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader hasRebuiltXrefReader = new _PdfReader_1746(filename);
            ReadingNotCompletedTest(hasRebuiltXrefReader);
        }

        private sealed class _PdfReader_1746 : PdfReader {
            public _PdfReader_1746(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasRebuiltXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasHybridXrefPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader hasHybridXrefPdfReader = PdfDocumentNotReadTestInit();
                hasHybridXrefPdfReader.HasHybridXref();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void HasHybridXrefReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader hasHybridXrefPdfReader = new _PdfReader_1770(filename);
            ReadingNotCompletedTest(hasHybridXrefPdfReader);
        }

        private sealed class _PdfReader_1770 : PdfReader {
            public _PdfReader_1770(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasHybridXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasXrefStmPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader hasXrefStmReader = PdfDocumentNotReadTestInit();
                hasXrefStmReader.HasXrefStm();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void HasXrefStmReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader hasXrefStmReader = new _PdfReader_1794(filename);
            ReadingNotCompletedTest(hasXrefStmReader);
        }

        private sealed class _PdfReader_1794 : PdfReader {
            public _PdfReader_1794(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasXrefStm();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void HasFixedXrefPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader hasFixedXrefReader = PdfDocumentNotReadTestInit();
                hasFixedXrefReader.HasFixedXref();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void HasFixedXrefReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader hasFixedXrefReader = new _PdfReader_1818(filename);
            ReadingNotCompletedTest(hasFixedXrefReader);
        }

        private sealed class _PdfReader_1818 : PdfReader {
            public _PdfReader_1818(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.HasFixedXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetLastXrefPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader getLastXrefReader = PdfDocumentNotReadTestInit();
                getLastXrefReader.GetLastXref();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void GetLastXrefReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader getLastXrefReader = new _PdfReader_1842(filename);
            ReadingNotCompletedTest(getLastXrefReader);
        }

        private sealed class _PdfReader_1842 : PdfReader {
            public _PdfReader_1842(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetLastXref();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetPermissionsPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader getPermissionsReader = PdfDocumentNotReadTestInit();
                getPermissionsReader.GetPermissions();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void GetPermissionsReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader getPermissionsReader = new _PdfReader_1866(filename);
            ReadingNotCompletedTest(getPermissionsReader);
        }

        private sealed class _PdfReader_1866 : PdfReader {
            public _PdfReader_1866(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetPermissions();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsOpenedWithFullPPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader isOpenedWithFullPReader = PdfDocumentNotReadTestInit();
                isOpenedWithFullPReader.IsOpenedWithFullPermission();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void IsOpenedWithFullPReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader isOpenedWithFullPReader = new _PdfReader_1890(filename);
            ReadingNotCompletedTest(isOpenedWithFullPReader);
        }

        private sealed class _PdfReader_1890 : PdfReader {
            public _PdfReader_1890(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.IsOpenedWithFullPermission();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetCryptoModePdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader getCryptoModeReader = PdfDocumentNotReadTestInit();
                getCryptoModeReader.GetCryptoMode();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void GetCryptoModeReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader getCryptoModeReader = new _PdfReader_1914(filename);
            ReadingNotCompletedTest(getCryptoModeReader);
        }

        private sealed class _PdfReader_1914 : PdfReader {
            public _PdfReader_1914(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetCryptoMode();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComputeUserPasswordPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader computeUserPasswordReader = PdfDocumentNotReadTestInit();
                computeUserPasswordReader.ComputeUserPassword();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void ComputeUserPasswordReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader computeUserPasswordReader = new _PdfReader_1938(filename);
            ReadingNotCompletedTest(computeUserPasswordReader);
        }

        private sealed class _PdfReader_1938 : PdfReader {
            public _PdfReader_1938(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.ComputeUserPassword();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetOriginalFileIdPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader getOriginalFileIdReader = PdfDocumentNotReadTestInit();
                getOriginalFileIdReader.GetOriginalFileId();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void GetOriginalFileIdReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader getOriginalFileIdReader = new _PdfReader_1962(filename);
            ReadingNotCompletedTest(getOriginalFileIdReader);
        }

        private sealed class _PdfReader_1962 : PdfReader {
            public _PdfReader_1962(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetOriginalFileId();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetModifiedFileIdPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader getModifiedFileIdReader = PdfDocumentNotReadTestInit();
                getModifiedFileIdReader.GetModifiedFileId();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void GetModifiedFileIdReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader getModifiedFileIdReader = new _PdfReader_1986(filename);
            ReadingNotCompletedTest(getModifiedFileIdReader);
        }

        private sealed class _PdfReader_1986 : PdfReader {
            public _PdfReader_1986(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.GetModifiedFileId();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void IsEncryptedPdfDocumentNotReadTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader isEncryptedReader = PdfDocumentNotReadTestInit();
                isEncryptedReader.IsEncrypted();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        [NUnit.Framework.Test]
        public virtual void IsEncryptedReadingNotCompletedTest() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            PdfReader isEncryptedReader = new _PdfReader_2010(filename);
            ReadingNotCompletedTest(isEncryptedReader);
        }

        private sealed class _PdfReader_2010 : PdfReader {
            public _PdfReader_2010(String baseArg1)
                : base(baseArg1) {
            }

            protected internal override void ReadPdf() {
                this.IsEncrypted();
                base.ReadPdf();
            }
        }

        [NUnit.Framework.Test]
        public virtual void Pdf11VersionValidTest() {
            String fileName = sourceFolder + "pdf11Version.pdf";
            new PdfDocument(new PdfReader(fileName));
        }

        private PdfReader PdfDocumentNotReadTestInit() {
            String filename = sourceFolder + "XrefWithNullOffsets.pdf";
            return new PdfReader(filename);
        }

        private void ReadingNotCompletedTest(PdfReader reader) {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument document = new PdfDocument(reader);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.DocumentHasNotBeenReadYet))
;
        }

        private static PdfDictionary GetTestPdfDictionary() {
            Dictionary<PdfName, PdfObject> tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("b"), new PdfName("c"));
            return new PdfDictionary(tmpMap);
        }
    }
}
