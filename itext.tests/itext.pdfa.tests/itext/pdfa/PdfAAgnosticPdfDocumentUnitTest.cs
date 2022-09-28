using System;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.XMP;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfa {
    [NUnit.Framework.Category("Unit test")]
    public class PdfAAgnosticPdfDocumentUnitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/AgnosticPdfDocumentUnitTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LoadPdfDocumentTest() {
            PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument pdfDoc = new PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument
                (this, new PdfReader(sourceFolder + "pdfs/simpleDoc.pdf"), new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.FlushObjectPublic(pdfDoc.GetPage(1).GetPdfObject(), true);
            NUnit.Framework.Assert.IsTrue(pdfDoc.GetPage(1).GetPdfObject().IsFlushed());
            pdfDoc.CheckIsoConformancePublic();
            // Does nothing for PdfDocument
            NUnit.Framework.Assert.IsFalse(pdfDoc.GetPageFactoryPublic() is PdfAPageFactory);
            NUnit.Framework.Assert.IsNull(pdfDoc.GetConformanceLevel());
            pdfDoc.UpdateXmpMetadataPublic();
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(pdfDoc.GetXmpMetadata(true));
            NUnit.Framework.Assert.IsNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART));
            NUnit.Framework.Assert.IsNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void LoadPdfADocumentTest() {
            PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument pdfADoc = new PdfAAgnosticPdfDocumentUnitTest.TestAgnosticPdfDocument
                (this, new PdfReader(sourceFolder + "pdfs/pdfa.pdf"), new PdfWriter(new ByteArrayOutputStream()), new 
                StampingProperties());
            pdfADoc.FlushObjectPublic(pdfADoc.GetPage(1).GetPdfObject(), true);
            NUnit.Framework.Assert.IsFalse(pdfADoc.GetPage(1).GetPdfObject().IsFlushed());
            pdfADoc.CheckIsoConformancePublic();
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_2B, pdfADoc.GetConformanceLevel());
            NUnit.Framework.Assert.IsTrue(pdfADoc.GetPageFactoryPublic() is PdfAPageFactory);
            pdfADoc.UpdateXmpMetadataPublic();
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(pdfADoc.GetXmpMetadata(true));
            NUnit.Framework.Assert.IsNotNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART));
            NUnit.Framework.Assert.IsNotNull(xmpMeta.GetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE));
            // Extra PdfA error message check
            pdfADoc.FlushObjectPublic(pdfADoc.GetCatalog().GetPdfObject(), true);
            NUnit.Framework.Assert.IsFalse(pdfADoc.GetCatalog().GetPdfObject().IsFlushed());
            pdfADoc.Close();
        }

        private class TestAgnosticPdfDocument : PdfAAgnosticPdfDocument {
            public TestAgnosticPdfDocument(PdfAAgnosticPdfDocumentUnitTest _enclosing, PdfReader reader, PdfWriter writer
                )
                : base(reader, writer, new StampingProperties()) {
                this._enclosing = _enclosing;
            }

            public TestAgnosticPdfDocument(PdfAAgnosticPdfDocumentUnitTest _enclosing, PdfReader reader, PdfWriter writer
                , StampingProperties properties)
                : base(reader, writer, properties) {
                this._enclosing = _enclosing;
            }

            public virtual void CheckIsoConformancePublic() {
                base.CheckIsoConformance();
            }

            public virtual IPdfPageFactory GetPageFactoryPublic() {
                return base.GetPageFactory();
            }

            public virtual void UpdateXmpMetadataPublic() {
                base.UpdateXmpMetadata();
            }

            public virtual void FlushObjectPublic(PdfObject pdfObject, bool canBeInObjStm) {
                base.FlushObject(pdfObject, canBeInObjStm);
            }

            private readonly PdfAAgnosticPdfDocumentUnitTest _enclosing;
        }
    }
}
