using System;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class SmartModeTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/SmartModeTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/SmartModeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingAndFlushing() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingAndFlushing.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameResourcesCopyingAndFlushing.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(outFile, new WriterProperties().UseSmartMode()));
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            outputDoc.Close();
            PdfDocument assertDoc = new PdfDocument(new PdfReader(outFile));
            PdfIndirectReference page1ResFontObj = assertDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page2ResFontObj = assertDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page3ResFontObj = assertDoc.GetPage(3).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            NUnit.Framework.Assert.IsTrue(page1ResFontObj.Equals(page2ResFontObj));
            NUnit.Framework.Assert.IsTrue(page1ResFontObj.Equals(page3ResFontObj));
            assertDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingModifyingAndFlushing() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingModifyingAndFlushing.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            bool exceptionCaught = false;
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(outFile, new WriterProperties().UseSmartMode()));
            int lastPageNum = 1;
            PdfFont font = PdfFontFactory.CreateFont();
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                int i;
                for (i = lastPageNum; i <= outputDoc.GetNumberOfPages(); ++i) {
                    PdfCanvas canvas;
                    try {
                        canvas = new PdfCanvas(outputDoc.GetPage(i));
                    }
                    catch (NullReferenceException) {
                        // Smart mode makes it possible to share objects coming from different source documents.
                        // Flushing one object documents might make it impossible to modify further copied objects.
                        NUnit.Framework.Assert.AreEqual(2, i);
                        exceptionCaught = true;
                        break;
                    }
                    canvas.BeginText().MoveText(36, 36).SetFontAndSize(font, 12).ShowText("Page " + i).EndText();
                }
                lastPageNum = i;
                if (exceptionCaught) {
                    break;
                }
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            if (!exceptionCaught) {
                NUnit.Framework.Assert.Fail();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SmartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh() {
            String outFile = destinationFolder + "smartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh.pdf";
            String cmpFile = sourceFolder + "cmp_smartModeSameResourcesCopyingModifyingAndFlushing_ensureObjectFresh.pdf";
            String[] srcFiles = new String[] { sourceFolder + "indirectResourcesStructure.pdf", sourceFolder + "indirectResourcesStructure2.pdf"
                 };
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(outFile, new WriterProperties().UseSmartMode()));
            int lastPageNum = 1;
            PdfFont font = PdfFontFactory.CreateFont();
            foreach (String srcFile in srcFiles) {
                PdfDocument sourceDoc = new PdfDocument(new PdfReader(srcFile));
                for (int i = 1; i <= sourceDoc.GetNumberOfPages(); ++i) {
                    PdfDictionary srcRes = sourceDoc.GetPage(i).GetPdfObject().GetAsDictionary(PdfName.Resources);
                    // Ensures that objects copied to the output document are fresh,
                    // i.e. are not reused from already copied objects cache.
                    bool ensureObjectIsFresh = true;
                    // it's crucial to copy first inner objects and then the container object!
                    foreach (PdfObject v in srcRes.Values()) {
                        if (v.GetIndirectReference() != null) {
                            // We are not interested in returned copied objects instances, they will be picked up by
                            // general copying mechanism from copied objects cache by default.
                            v.CopyTo(outputDoc, ensureObjectIsFresh);
                        }
                    }
                    if (srcRes.GetIndirectReference() != null) {
                        srcRes.CopyTo(outputDoc, ensureObjectIsFresh);
                    }
                }
                sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), outputDoc);
                sourceDoc.Close();
                int i_1;
                for (i_1 = lastPageNum; i_1 <= outputDoc.GetNumberOfPages(); ++i_1) {
                    PdfPage page = outputDoc.GetPage(i_1);
                    PdfCanvas canvas = new PdfCanvas(page);
                    canvas.BeginText().MoveText(36, 36).SetFontAndSize(font, 12).ShowText("Page " + i_1).EndText();
                }
                lastPageNum = i_1;
                outputDoc.FlushCopiedObjects(sourceDoc);
            }
            outputDoc.Close();
            PdfDocument assertDoc = new PdfDocument(new PdfReader(outFile));
            PdfIndirectReference page1ResFontObj = assertDoc.GetPage(1).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page2ResFontObj = assertDoc.GetPage(2).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            PdfIndirectReference page3ResFontObj = assertDoc.GetPage(3).GetPdfObject().GetAsDictionary(PdfName.Resources
                ).GetAsDictionary(PdfName.Font).GetIndirectReference();
            NUnit.Framework.Assert.IsFalse(page1ResFontObj.Equals(page2ResFontObj));
            NUnit.Framework.Assert.IsFalse(page1ResFontObj.Equals(page3ResFontObj));
            NUnit.Framework.Assert.IsFalse(page2ResFontObj.Equals(page3ResFontObj));
            assertDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder));
        }
    }
}
