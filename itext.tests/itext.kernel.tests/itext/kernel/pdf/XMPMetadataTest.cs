using System;
using System.IO;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class XMPMetadataTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XmpWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/XmpWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithXmp() {
            String filename = "emptyDocumentWithXmp.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename, new WriterProperties().AddXmpMetadata());
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 7").SetTitle("Empty iText 7 Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            int delta = ReadFile(sourceFolder + "emptyDocumentWithXmp.xml").Length - pdfDocument.GetXmpMetadata().Length;
            //Difference could be because of -SNAPSHOT postfix.
            NUnit.Framework.Assert.IsTrue(delta == 0 || delta == 9, "Unexpected length delta");
            NUnit.Framework.Assert.IsNotNull(reader.pdfDocument.GetPage(1));
            reader.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithAbcXmp() {
            MemoryStream fos = new MemoryStream();
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 7").SetTitle("Empty iText 7 Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.SetXmpMetadata("abc".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            pdfDoc.Close();
            PdfReader reader = new PdfReader(new MemoryStream(fos.ToArray()));
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), pdfDocument.GetXmpMetadata
                ());
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
            reader.Close();
        }
    }
}
