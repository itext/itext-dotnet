using System;
using iText.IO.Source;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfCopyTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfCopyTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfCopyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(iText.IO.LogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN)]
        public virtual void CopySignedDocuments() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(sourceFolder + "hello_signed.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copySignedDocuments.pdf"));
            pdfDoc1.CopyPagesTo(1, 1, pdfDoc2);
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(destinationFolder + "copySignedDocuments.pdf"));
            PdfDictionary sig = (PdfDictionary)pdfDocument.GetPdfObject(13);
            PdfDictionary sigRef = sig.GetAsArray(PdfName.Reference).GetAsDictionary(0);
            NUnit.Framework.Assert.IsTrue(PdfName.SigRef.Equals(sigRef.GetAsName(PdfName.Type)));
            NUnit.Framework.Assert.IsTrue(sigRef.Get(PdfName.Data).IsNull());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Copying1() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(destinationFolder + "copying1_1.pdf"));
            pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            pdfDoc1.GetCatalog().Put(new PdfName("a"), ((PdfName)new PdfName("b").MakeIndirect(pdfDoc1)));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(destinationFolder + "copying1_1.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copying1_2.pdf"));
            pdfDoc2.AddNewPage();
            pdfDoc2.GetDocumentInfo().GetPdfObject().Put(new PdfName("a"), pdfDoc1.GetCatalog().GetPdfObject().Get(new 
                PdfName("a")).CopyTo(pdfDoc2));
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying1_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary trailer = pdfDocument.GetTrailer();
            PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
            PdfName b = info.GetAsName(new PdfName("a"));
            NUnit.Framework.Assert.AreEqual("/b", b.ToString());
            pdfDocument.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Copying2() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(destinationFolder + "copying2_1.pdf"));
            for (int i = 0; i < 10; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + (i + 1).ToString() + "\n"
                    ));
                page1.Flush();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(destinationFolder + "copying2_1.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copying2_2.pdf"));
            for (int i = 0; i < 10; i++) {
                if (i % 2 == 0) {
                    pdfDoc2.AddPage(pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2));
                }
            }
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying2_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 5; i++) {
                byte[] bytes = pdfDocument.GetPage(i + 1).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + (i * 2 + 1).ToString() + "\n", iText.IO.Util.JavaUtil.GetStringForBytes
                    (bytes));
            }
            pdfDocument.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Copying3() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copying3_1.pdf"));
            PdfDictionary helloWorld = ((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc));
            PdfDictionary helloWorld1 = ((PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc));
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            helloWorld.Put(new PdfName("HelloWrld"), helloWorld);
            helloWorld.Put(new PdfName("HelloWrld1"), helloWorld1);
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.GetPdfObject().Put(new PdfName("HelloWorldClone"), (PdfObject)helloWorld.Clone());
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying3_1.pdf");
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            pdfDoc = new PdfDocument(reader);
            PdfDictionary dic0 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("HelloWorld"));
            NUnit.Framework.Assert.AreEqual(4, dic0.GetIndirectReference().GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, dic0.GetIndirectReference().GetGenNumber());
            PdfDictionary dic1 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("HelloWorldClone"));
            NUnit.Framework.Assert.AreEqual(8, dic1.GetIndirectReference().GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, dic1.GetIndirectReference().GetGenNumber());
            PdfString str0 = dic0.GetAsString(new PdfName("Hello"));
            PdfString str1 = dic1.GetAsString(new PdfName("Hello"));
            NUnit.Framework.Assert.AreEqual(str0.GetValue(), str1.GetValue());
            NUnit.Framework.Assert.AreEqual(str0.GetValue(), "World");
            PdfDictionary dic01 = dic0.GetAsDictionary(new PdfName("HelloWrld"));
            PdfDictionary dic11 = dic1.GetAsDictionary(new PdfName("HelloWrld"));
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), dic11.GetIndirectReference().
                GetObjNumber());
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), dic11.GetIndirectReference().
                GetGenNumber());
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), 4);
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), 0);
            PdfDictionary dic02 = dic0.GetAsDictionary(new PdfName("HelloWrld1"));
            PdfDictionary dic12 = dic1.GetAsDictionary(new PdfName("HelloWrld1"));
            NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetObjNumber(), dic12.GetIndirectReference().
                GetObjNumber());
            NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetGenNumber(), dic12.GetIndirectReference().
                GetGenNumber());
            NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetObjNumber(), 5);
            NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetGenNumber(), 0);
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyDocumentsWithFormFieldsTest() {
            String filename = sourceFolder + "fieldsOn2-sPage.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copyDocumentsWithFormFields.pdf"));
            sourceDoc.InitializeOutlines();
            sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "copyDocumentsWithFormFields.pdf"
                , sourceFolder + "cmp_copyDocumentsWithFormFields.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CopySamePageWithAnnotationsSeveralTimes() {
            String filename = sourceFolder + "rotated_annotation.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copySamePageWithAnnotationsSeveralTimes.pdf"
                ));
            sourceDoc.InitializeOutlines();
            sourceDoc.CopyPagesTo(iText.IO.Util.JavaUtil.ArraysAsList(1, 1, 1), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "copySamePageWithAnnotationsSeveralTimes.pdf"
                , sourceFolder + "cmp_copySamePageWithAnnotationsSeveralTimes.pdf", destinationFolder, "diff_"));
        }
    }
}
