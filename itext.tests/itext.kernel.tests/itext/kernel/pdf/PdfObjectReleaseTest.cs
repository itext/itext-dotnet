using System;
using iText.Kernel;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfObjectReleaseTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfObjectReleaseTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfObjectReleaseTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET, Count = 108)]
        public virtual void ReleaseObjectsInDocWithStructTreeRootTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInDocWithStructTreeRoot.pdf", "releaseObjectsInDocWithStructTreeRoot_stamping.pdf"
                , "releaseObjectsInDocWithStructTreeRoot_stamping_release.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET, Count = 5)]
        public virtual void ReleaseObjectsInDocWithXfaTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInDocWithXfa.pdf", "releaseObjectsInDocWithXfa_stamping.pdf", "releaseObjectsInDocWithXfa_stamping_release.pdf"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET, Count = 3)]
        public virtual void ReleaseObjectsInSimpleDocTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInSimpleDoc.pdf", "releaseObjectsInSimpleDoc_stamping.pdf", "releaseObjectsInSimpleDoc_stamping_release.pdf"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleaseCatalogTest() {
            String srcFile = sourceFolder + "releaseObjectsInSimpleDoc.pdf";
            String release = destinationFolder + "outReleaseObjectsInSimpleDoc.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetCatalog().GetPdfObject().Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleasePagesTest() {
            String srcFile = sourceFolder + "releaseObjectsInSimpleDoc.pdf";
            String release = destinationFolder + "outReleaseObjectsInSimpleDoc.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleaseStructTreeRootTest() {
            String srcFile = sourceFolder + "releaseObjectsInDocWithStructTreeRoot.pdf";
            String release = destinationFolder + "outReleaseObjectsInDocWithStructTreeRoot.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetStructTreeRoot().GetPdfObject().Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void NoForbidReleaseObjectsModifyingTest() {
            String srcFile = sourceFolder + "noForbidReleaseObjectsModifying.pdf";
            String stampReleased = sourceFolder + "noForbidReleaseObjectsModified.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(destinationFolder + "noForbidReleaseObjectsModifying.pdf"
                ), new StampingProperties().UseAppendMode())) {
                PdfAnnotation annots = doc.GetPage(1).GetAnnotations()[0];
                annots.SetRectangle(new PdfArray(new Rectangle(100, 100, 80, 50)));
                annots.GetRectangle().Release();
            }
            using (PdfDocument openPrev = new PdfDocument(new PdfReader(stampReleased))) {
                NUnit.Framework.Assert.IsTrue(new Rectangle(100, 100, 80, 50).EqualsWithEpsilon(openPrev.GetPage(1).GetAnnotations
                    ()[0].GetRectangle().ToRectangle()));
            }
            NUnit.Framework.Assert.IsNotNull(new CompareTool().CompareByContent(srcFile, stampReleased, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AddingReleasedObjectToDocumentTest() {
            String srcFile = sourceFolder + "releaseObjectsInSimpleDoc.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(sourceFolder + "addingReleasedObjectToDocument.pdf"
                ));
            try {
                PdfObject releasedObj = doc.GetPdfObject(1);
                releasedObj.Release();
                doc.GetCatalog().Put(PdfName.Outlines, releasedObj);
            }
            finally {
                NUnit.Framework.Assert.That(() =>  {
                    doc.Close();
                }
                , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo("Cannot write object after it was released." + " In normal situation the object must be read once again before being written."))
;
            }
        }

        private void SinglePdfObjectReleaseTest(String inputFilename, String outStampingFilename, String outStampingReleaseFilename
            ) {
            String srcFile = sourceFolder + inputFilename;
            String outPureStamping = destinationFolder + outStampingFilename;
            String outStampingRelease = destinationFolder + outStampingReleaseFilename;
            PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outPureStamping));
            // We open/close document to make sure that the results of release logic and simple overwriting coincide.
            doc.Close();
            PdfDocument stamperRelease = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outStampingRelease));
            for (int i = 0; i < stamperRelease.GetNumberOfPdfObjects(); i++) {
                PdfObject pdfObject = stamperRelease.GetPdfObject(i);
                if (pdfObject != null) {
                    stamperRelease.GetPdfObject(i).Release();
                }
            }
            stamperRelease.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outStampingRelease, outPureStamping, destinationFolder
                ));
        }
    }
}
