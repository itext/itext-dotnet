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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf.Copy {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAnnotationCopyingTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfAnnotationCopyingTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfAnnotationCopyingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("Unignore when DEVSIX-3585 would be implemented")]
        public virtual void TestCopyingPageWithAnnotationContainingPopupKey() {
            String inFilePath = SOURCE_FOLDER + "annotation-with-popup.pdf";
            String outFilePath = DESTINATION_FOLDER + "copy-annotation-with-popup.pdf";
            PdfDocument originalDocument = new PdfDocument(new PdfReader(inFilePath));
            PdfDocument outDocument = new PdfDocument(new PdfWriter(outFilePath));
            originalDocument.CopyPagesTo(1, 1, outDocument);
            // During the second copy call we have to rebuild/preserve all the annotation relationship (Popup in this case),
            // so that we don't end up with annotation on one page referring to an annotation on another page as its popup
            // or as its parent
            originalDocument.CopyPagesTo(1, 1, outDocument);
            originalDocument.Close();
            outDocument.Close();
            outDocument = new PdfDocument(new PdfReader(outFilePath));
            for (int pageNum = 1; pageNum <= outDocument.GetNumberOfPages(); pageNum++) {
                PdfPage page = outDocument.GetPage(pageNum);
                NUnit.Framework.Assert.AreEqual(2, page.GetAnnotsSize());
                NUnit.Framework.Assert.AreEqual(2, page.GetAnnotations().Count);
                bool foundMarkupAnnotation = false;
                foreach (PdfAnnotation annotation in page.GetAnnotations()) {
                    PdfDictionary annotationPageDict = annotation.GetPageObject();
                    if (annotationPageDict != null) {
                        NUnit.Framework.Assert.AreSame(page.GetPdfObject(), annotationPageDict);
                    }
                    if (annotation is PdfMarkupAnnotation) {
                        foundMarkupAnnotation = true;
                        PdfPopupAnnotation popup = ((PdfMarkupAnnotation)annotation).GetPopup();
                        NUnit.Framework.Assert.IsTrue(page.ContainsAnnotation(popup), MessageFormatUtil.Format("Popup reference must point to annotation present on the same page (# {0})"
                            , pageNum));
                        PdfDictionary parentAnnotation = popup.GetParentObject();
                        NUnit.Framework.Assert.AreSame(annotation.GetPdfObject(), parentAnnotation, "Popup annotation parent must point to the annotation that specified it as Popup"
                            );
                    }
                }
                NUnit.Framework.Assert.IsTrue(foundMarkupAnnotation, "Markup annotation expected to be present but not found"
                    );
            }
            outDocument.Close();
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("Unignore when DEVSIX-3585 would be implemented")]
        public virtual void TestCopyingPageWithAnnotationContainingIrtKey() {
            String inFilePath = SOURCE_FOLDER + "annotation-with-irt.pdf";
            String outFilePath = DESTINATION_FOLDER + "copy-annotation-with-irt.pdf";
            PdfDocument originalDocument = new PdfDocument(new PdfReader(inFilePath));
            PdfDocument outDocument = new PdfDocument(new PdfWriter(outFilePath));
            originalDocument.CopyPagesTo(1, 1, outDocument);
            // During the second copy call we have to rebuild/preserve all the annotation relationship (IRT in this case),
            // so that we don't end up with annotation on one page referring to an annotation on another page as its IRT
            // or as its parent
            originalDocument.CopyPagesTo(1, 1, outDocument);
            originalDocument.Close();
            outDocument.Close();
            outDocument = new PdfDocument(new PdfReader(outFilePath));
            for (int pageNum = 1; pageNum <= outDocument.GetNumberOfPages(); pageNum++) {
                PdfPage page = outDocument.GetPage(pageNum);
                NUnit.Framework.Assert.AreEqual(4, page.GetAnnotsSize());
                NUnit.Framework.Assert.AreEqual(4, page.GetAnnotations().Count);
                bool foundMarkupAnnotation = false;
                foreach (PdfAnnotation annotation in page.GetAnnotations()) {
                    PdfDictionary annotationPageDict = annotation.GetPageObject();
                    if (annotationPageDict != null) {
                        NUnit.Framework.Assert.AreSame(page.GetPdfObject(), annotationPageDict);
                    }
                    if (annotation is PdfMarkupAnnotation) {
                        foundMarkupAnnotation = true;
                        PdfDictionary inReplyTo = ((PdfMarkupAnnotation)annotation).GetInReplyToObject();
                        NUnit.Framework.Assert.IsTrue(page.ContainsAnnotation(PdfAnnotation.MakeAnnotation(inReplyTo)), "IRT reference must point to annotation present on the same page"
                            );
                    }
                }
                NUnit.Framework.Assert.IsTrue(foundMarkupAnnotation, "Markup annotation expected to be present but not found"
                    );
            }
            outDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopySameLinksWithGoToSmartModeTest() {
            String cmpFilePath = SOURCE_FOLDER + "cmp_copySameLinksWithGoToSmartMode.pdf";
            String outFilePath = DESTINATION_FOLDER + "copySameLinksWithGoToSmartMode.pdf";
            CopyLinksGoToActionTest(outFilePath, true, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFilePath, cmpFilePath, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyDiffDestLinksWithGoToSmartModeTest() {
            String cmpFilePath = SOURCE_FOLDER + "cmp_copyDiffDestLinksWithGoToSmartMode.pdf";
            String outFilePath = DESTINATION_FOLDER + "copyDiffDestLinksWithGoToSmartMode.pdf";
            CopyLinksGoToActionTest(outFilePath, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFilePath, cmpFilePath, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyDiffDisplayLinksWithGoToSmartModeTest() {
            String cmpFilePath = SOURCE_FOLDER + "cmp_copyDiffDisplayLinksWithGoToSmartMode.pdf";
            String outFilePath = DESTINATION_FOLDER + "copyDiffDisplayLinksWithGoToSmartMode.pdf";
            CopyLinksGoToActionTest(outFilePath, false, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFilePath, cmpFilePath, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyPagesWithWidgetAnnotGoToActionExplicitDestTest() {
            String srcFilePath = SOURCE_FOLDER + "pageToCopyWithWidgetAnnotGoToActionExplicitDest.pdf";
            String cmpFilePath = SOURCE_FOLDER + "cmp_copyPagesWithWidgetAnnotGoToActionExplicitDest.pdf";
            String outFilePath = DESTINATION_FOLDER + "copyPagesWithWidgetAnnotGoToActionExplicitDest.pdf";
            CopyPages(srcFilePath, outFilePath, cmpFilePath);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyPagesWithWidgetAnnotGoToActionNamedDestTest() {
            String srcFilePath = SOURCE_FOLDER + "pageToCopyWithWidgetAnnotGoToActionNamedDest.pdf";
            String cmpFilePath = SOURCE_FOLDER + "cmp_copyPagesWithWidgetAnnotGoToActionNamedDest.pdf";
            String outFilePath = DESTINATION_FOLDER + "copyPagesWithWidgetAnnotGoToActionNamedDest.pdf";
            CopyPages(srcFilePath, outFilePath, cmpFilePath);
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithScreenAnnotGoToActionExplicitDestTest() {
            String srcFilePath = SOURCE_FOLDER + "pageToCopyWithScreenAnnotGoToActionExplicitDest.pdf";
            String cmpFilePath = SOURCE_FOLDER + "cmp_copyPagesWithScreenAnnotGoToActionExplicitDest.pdf";
            String outFilePath = DESTINATION_FOLDER + "copyPagesWithScreenAnnotGoToActionExplicitDest.pdf";
            CopyPages(srcFilePath, outFilePath, cmpFilePath);
        }

        private void CopyLinksGoToActionTest(String dest, bool isTheSameLinks, bool diffDisplayOptions) {
            PdfDocument destDoc = new PdfDocument(new PdfWriter(dest).SetSmartMode(true));
            MemoryStream sourceBaos1 = CreatePdfWithGoToAnnot(isTheSameLinks, diffDisplayOptions);
            PdfDocument sourceDoc1 = new PdfDocument(new PdfReader(new MemoryStream(sourceBaos1.ToArray())));
            sourceDoc1.CopyPagesTo(1, sourceDoc1.GetNumberOfPages(), destDoc);
            sourceDoc1.Close();
            destDoc.Close();
        }

        private void CopyPages(String sourceFile, String outFilePath, String cmpFilePath) {
            using (PdfWriter writer = new PdfWriter(outFilePath)) {
                using (PdfDocument pdfDoc = new PdfDocument(writer)) {
                    pdfDoc.AddNewPage();
                    pdfDoc.AddNewPage();
                    using (PdfReader reader = new PdfReader(sourceFile)) {
                        using (PdfDocument srcDoc = new PdfDocument(reader)) {
                            srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), pdfDoc);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFilePath, cmpFilePath, DESTINATION_FOLDER
                ));
        }

        private MemoryStream CreatePdfWithGoToAnnot(bool isTheSameLink, bool diffDisplayOptions) {
            MemoryStream stream = new MemoryStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(stream));
            pdfDocument.AddNewPage();
            pdfDocument.AddNewPage();
            pdfDocument.AddNewPage();
            Rectangle linkLocation = new Rectangle(523, 770, 36, 36);
            PdfExplicitDestination destination = PdfExplicitDestination.CreateFit(pdfDocument.GetPage(3));
            PdfAnnotation annotation = new PdfLinkAnnotation(linkLocation).SetAction(PdfAction.CreateGoTo(destination)
                ).SetBorder(new PdfArray(new int[] { 0, 0, 1 }));
            pdfDocument.GetFirstPage().AddAnnotation(annotation);
            if (!isTheSameLink) {
                destination = (diffDisplayOptions) ? PdfExplicitDestination.Create(pdfDocument.GetPage(3), PdfName.XYZ, 350
                    , 350, 0, 0, 1) : PdfExplicitDestination.CreateFit(pdfDocument.GetPage(1));
            }
            annotation = new PdfLinkAnnotation(linkLocation).SetAction(PdfAction.CreateGoTo(destination)).SetBorder(new 
                PdfArray(new int[] { 0, 0, 1 }));
            pdfDocument.GetPage(2).AddAnnotation(annotation);
            pdfDocument.Close();
            return stream;
        }
    }
}
